//#define DETAILED_LOG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using NRPlanes.ServerData.MutableInformations;
using NRPlanes.ServerData.OperationResults;
using NRPlanes.ServerData;
using NRPlanes.Core.Logging;
using NRPlanes.Core.Aliens;
using NRPlanes.ServerData.EventsLog;
using NRPlanes.Core.Equipments;

namespace NRPlanes.Server
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single, 
        ConcurrencyMode = ConcurrencyMode.Single)]
    public class GameService : IGameService
    {
        public readonly GameWorld World;
        public double FPS { get; private set; }

        private int m_lastObjectID;
        private Dictionary<Guid, Plane> m_playerToPlaneMapping;
        private Action<string> m_log;

        private GameEventsLog m_worldEventsLog;
        public GameEventsLog WorldEventsLog
        {
            get
            {
                return m_worldEventsLog;
            }
        }

        private void GameWorldUpdate()
        {
            Stopwatch watch = new Stopwatch();
            TimeSpan elapsed = TimeSpan.FromMilliseconds(1.0);

            while (true)
            {
                watch.Start();

                World.Update(elapsed);

                UpdateEventLog();

                if (watch.Elapsed.TotalMilliseconds < 1.0) // limit fps (max = 1000 fps)
                    Thread.Sleep(1);
                
                FPS = 0.9 * FPS + 0.1 / elapsed.TotalSeconds; // add fps smoothness

                elapsed = watch.Elapsed;

                watch.Reset();
            }
        }

        public GameService()
            :this(null)
        { }

        public GameService(Action<string> logAction)
        {
            m_log = logAction;

            m_worldEventsLog = new GameEventsLog();

            Logger.LogItemReceived += logItem => LogMessage(logItem.ToString());

            m_playerToPlaneMapping = new Dictionary<Guid, Plane>();

            World = new GameWorld(new Size(800, 800));
            World.AddGravityBoundsWithPlanets(50, 13);
            //World.AliensAppearingStrategy = new BasicAliensAppearingStrategy(World, TimeSpan.FromSeconds(60));
            World.AliensAppearingStrategy = new SingleAliensAppearingStrategy(World);
            
            World.GameObjectStatusChanged += GameObjectStatusChangedHandler;
            World.BonusApplied += BonusAppliedHandler;
            World.Explosion += ExplosionHandler;

            Task.Factory.StartNew(GameWorldUpdate);                
        }

        private void UpdateEventLog()
        {
            // when event log contains add and remove event for same game object then this two entries is depriciated

            // copy log to avoid collection changing while enumeration (e.g. when user commiting object while enumerating log)
            var logCopy = m_worldEventsLog.GetAll(true).ToList();

            var gameobjectAddLogItems = logCopy.Where(entry => entry is GameObjectAddedLogItem).Cast<GameObjectAddedLogItem>();

            foreach (var deleteLogItem in logCopy.Where(entry => entry is GameObjectDeletedLogItem).Cast<GameObjectDeletedLogItem>().ToList())
            {
                var conjugatedAddLogItem = gameobjectAddLogItems.SingleOrDefault(addLogItem => addLogItem.GameObject.Id == deleteLogItem.GameObjectId);

                if (conjugatedAddLogItem != null)
                {
                    deleteLogItem.IsDepriciated = true;
                    conjugatedAddLogItem.IsDepriciated = true;
                }
            }
        }
        private void ExplosionHandler(object sender, ExplosionEventArgs args)
        {
            var entry = new GameObjectExplodedLogItem(Timestamp.Create(), args.Exploded.Id.Value);

            // marks as depritiated right now - this event has not to be transferred to new players
            entry.IsDepriciated = true;

            m_worldEventsLog.AddEntry(entry);
        }
        private void BonusAppliedHandler(object sender, BonusAppliedEventArgs args)
        {
            var entry = new BonusAppliedLogItem(Timestamp.Create(), args.Bonus.Id.Value, args.Plane.Id.Value);

            // marks as depritiated right now - this event has not to be transferred to new players
            entry.IsDepriciated = true;

            m_worldEventsLog.AddEntry(entry);
        }
        private void GameObjectStatusChangedHandler(object sender, GameObjectStatusChangedEventArg arg)
        {
            switch (arg.Status)
            {
                case GameObjectStatus.Created:
                    AssignGameObjectID(arg.GameObject);

                    // add PlaneEquipmentStatusChanged handler when game object is Plane
                    if (arg.GameObject is Plane)
                        ((Plane)arg.GameObject).EquipmentStatusChanged += PlaneEquipmentStatusChanged;

                    m_worldEventsLog.AddEntry(new GameObjectAddedLogItem(Timestamp.Create(), arg.GameObject));
                    break;
                case GameObjectStatus.Deleted:
                    m_worldEventsLog.AddEntry(new GameObjectDeletedLogItem(Timestamp.Create(), arg.GameObject));
                    break;
                default:
                    throw new Exception("Unknown state");
            }
        }

        private void PlaneEquipmentStatusChanged(object sender, GameObjectEquipmentStatusChangedArgs args)
        {
            switch (args.Status)
            {
                case GameObjectEquipmentStatus.Added:
                    m_worldEventsLog.AddEntry(new PlaneEquipmentAddedLogItem(
                        Timestamp.Create(),
                        (Plane)args.Equipment.RelatedGameObject,
                        (PlaneEquipment)args.Equipment));
                    break;
                case GameObjectEquipmentStatus.Removed:
                    m_worldEventsLog.AddEntry(new PlaneEquipmentRemovedLogItem(
                       Timestamp.Create(),
                       (Plane)args.Equipment.RelatedGameObject,
                       (PlaneEquipment)args.Equipment));
                    break;
                default:
                    throw new Exception("Unknown state");
            }
        }

        public JoinResult Join()
        {
            JoinResult result = new JoinResult()
            {
                PlayerGuid = Guid.NewGuid(),
                LogicalSize = World.Size,
                StaticObjects = World.StaticObjects
            };

            m_playerToPlaneMapping[result.PlayerGuid] = null;

            LogMessage(string.Format("Player joined (id={0})", result.PlayerGuid));

            return result;
        }        

        public CommitResult CommitObjects(Guid playerGuid, List<GameObject> objects)
        {
            CommitResult result = new CommitResult() { ObjectsIds = new List<int>(objects.Count) };

            foreach (var obj in objects)
            {
                if (obj is Plane)
                    m_playerToPlaneMapping[playerGuid] = obj as Plane; // If player commit plane when it his own plane                

                IntegrityDataHelper.ProcessRecieved(obj, World);

                World.AddGameObject(obj);

                result.ObjectsIds.Add(obj.Id.Value);
            }

            LogMessage(string.Format("{0} object(s) has been commited (last ID={1})", objects.Count, m_lastObjectID));

            return result;
        }

        public GetEventsLogSinceResult GetEventsLogSince(Guid playerGuid, Timestamp timestamp)
        {
            IEnumerable<GameEventsLogItem> logItems;

            // equals null when first time request
            if (timestamp != null)
                logItems = m_worldEventsLog.GetLogSince(timestamp, excludeDepriciated: false);
            else
                logItems = m_worldEventsLog.GetAll(excludeDepriciated: true); // when first time request exclude depritiated items

            GetEventsLogSinceResult result = new GetEventsLogSinceResult() { LogItems = logItems };

            if (result.LogItems.Count() > 0)
                LogMessage(string.Format("Sending {0} log items since {1} ({2})", result.LogItems.Count(), timestamp, playerGuid));

            return result;
        }

        //public GetNewObjectsResult GetNewObjects(Guid playerGuid, int minId)
        //{
        //    GetNewObjectsResult newObjectsResult = new GetNewObjectsResult();

        //    using (var handle = World.GameObjectsSafeReadHandle)
        //    {
        //        newObjectsResult.Objects = handle.Items.Where(o => o.Id.Value >= minId && !o.IsGarbage).ToList();
        //    }

        //    if (newObjectsResult.Objects.Count > 0)
        //        LogMessage(string.Format("{0} objects has been sent to player with id={1} (last ID={2})", newObjectsResult.Objects.Count, playerGuid, minId));

        //    return newObjectsResult;            
        //}

        public void UpdatePlane(Guid playerGuid, PlaneMutableInformation info)
        {
            Plane playerPlane = m_playerToPlaneMapping[playerGuid];

            info.ApplyToPlayerPlaneOnServer(playerPlane);
        }


        public IEnumerable<PlaneMutableInformation> GetPlanesInfo(Guid playerGuid)
        {
#if DETAILED_LOG
            LogMessage(string.Format("Planes infos for player with GUID={0}", playerGuid));
#endif
            //Plane playerPlane = m_playerToPlaneMapping[playerGuid];

            List<PlaneMutableInformation> infos = new List<PlaneMutableInformation>();

            using (var handle = World.GameObjectsSafeReadHandle)
            {
                foreach (Plane plane in handle.Items.Where(o => o is Plane))
                {
                    //if (plane != playerPlane)
                    {
                        PlaneMutableInformation planeInfo = new PlaneMutableInformation(plane);
                        infos.Add(planeInfo);
#if DETAILED_LOG
                        LogMessage(planeInfo.ToString());
#endif
                    }
                }
            }

            return infos;
        }

        private void AssignGameObjectID(GameObject obj)
        {
            obj.Id = m_lastObjectID++;
        }

        private void LogMessage(string message)
        {
            if (m_log != null)
                m_log.BeginInvoke(message, null, null);
        }


       
    }
}

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

        private void GameWorldUpdate()
        {
            Stopwatch watch = new Stopwatch();
            TimeSpan elapsed = TimeSpan.FromMilliseconds(1.0);

            while (true)
            {
                watch.Start();

                World.Update(elapsed);

                if (watch.Elapsed.TotalMilliseconds < 1.0) // limit fps (max = 1000 fps)
                    Thread.Sleep(1);
                
                FPS = 0.9 * FPS + 0.1 / elapsed.TotalSeconds; // add fps smoothness

                elapsed = watch.Elapsed;

                watch.Reset();
            }
        }

        public GameService()
            :this(null)
        {
        }

        public GameService(Action<string> logAction)
        {
            m_log = logAction;

            Logger.LogItemReceived += logItem => LogMessage(logItem.ToString());

            m_playerToPlaneMapping = new Dictionary<Guid, Plane>();

            World = new GameWorld(new Size(800, 800));
            World.AddGravityBoundsWithPlanets(50, 13);
            World.AliensAppearingStrategy = new BasicAliensAppearingStrategy(World, TimeSpan.FromSeconds(10));

            Task.Factory.StartNew(GameWorldUpdate);                
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

                AssignGameObjectID(obj);                

                World.AddGameObject(IntegrityDataHelper.PreprocessRecieved(obj));

                result.ObjectsIds.Add(obj.Id.Value);
            }

            LogMessage(string.Format("{0} object(s) has commited (last ID={1})", objects.Count, m_lastObjectID));

            return result;
        }

        public GetNewObjectsResult GetNewObjects(Guid playerGuid, int minId)
        {
            GetNewObjectsResult newObjectsResult = new GetNewObjectsResult();

            // preventing objects collectiong changing
            World.PerformSafeGameObjectCollectionOperation(() =>
                {
                    // fill GameObject's ID field (For objects that have been created on the server)
                    foreach (var createdObject in World.GameObjects.Where(o => !o.Id.HasValue))
                    {
                        AssignGameObjectID(createdObject);
                    }

                    newObjectsResult.Objects = World.GameObjects.Where(o => o.Id.Value >= minId && !o.IsGarbage).ToList();
                });

            if (newObjectsResult.Objects.Count > 0)
                LogMessage(string.Format("{0} objects has sent to player with id={1} (last ID={2})", newObjectsResult.Objects.Count, playerGuid, minId));

            return newObjectsResult;
        }

        public void UpdatePlane(Guid playerGuid, PlaneMutableInformation info)
        {
            Plane playerPlane = m_playerToPlaneMapping[playerGuid];

            info.Apply(playerPlane);
        }


        public IEnumerable<PlaneMutableInformation> GetPlanesInfo(Guid playerGuid)
        {
            Plane playerPlane = m_playerToPlaneMapping[playerGuid];

            List<PlaneMutableInformation> info = new List<PlaneMutableInformation>();

            foreach (Plane plane in World.GameObjects.Where(o => o is Plane))
            {
                if (plane != playerPlane)
                {
                    info.Add(new PlaneMutableInformation(plane));
                }
            }

            return info;
        }

        private void AssignGameObjectID(GameObject obj)
        {
            obj.Id = m_lastObjectID++;
        }

        private void LogMessage(string message)
        {
            if (m_log != null)
                m_log(message);
        }
    }
}

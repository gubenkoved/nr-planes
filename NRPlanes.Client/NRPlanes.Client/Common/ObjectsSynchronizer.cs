using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.ServerData.MutableInformations;
using NRPlanes.ServerData.OperationResults;
using NRPlanes.ServerData;
using System.Threading;
using NRPlanes.Core.Common.Client;
using NRPlanes.Client.ServiceReference;
using NRPlanes.ServerData.EventsLog;
using NRPlanes.Core.Bonuses;

namespace NRPlanes.Client.Common
{
    /// <summary>
    /// Synchronizes game object sets on client- and server- sides
    /// <para>- subscribes on GameWorld events to commit created objects</para>
    /// <para>- and receive objects created by another users</para>
    /// <para>Works in dedicated thread</para>
    /// </summary>
    public class ObjectsSynchronizer
    {
        private readonly GameServiceClient m_client;
        private readonly ClientGameWorld m_clientWorld;
        private readonly Guid m_ownGuid;
        private readonly Plane m_ownPlane;

        private int m_maxId;
        private Timestamp m_lastTimestamp;
        private List<GameObject> m_commitQueue;

        private Thread m_workerThread;
        private AutoResetEvent m_updateEvent;
        private bool m_isFirstUpdate;

        private const float MAX_UPDATE_RATE = 50.0f;

        // minimal time since getting from server DELETE event and before it will be removed from client GameWorld
        // this time is need to give client game world instance possibility to handle collsions by it's own
        private readonly TimeSpan DELETE_DELAY = TimeSpan.FromMilliseconds(100);

        private List<Tuple<DateTime, int>> m_deleteIdsQueue;

        public ObjectsSynchronizer(GameServiceClient client, ClientGameWorld world, Guid ownGuid, Plane ownPlane)
        {
            m_ownGuid = ownGuid;
            m_ownPlane = ownPlane;
            m_client = client;
            m_clientWorld = world;

            m_isFirstUpdate = true;
            m_maxId = -1;
            m_commitQueue = new List<GameObject>();

            m_updateEvent = new AutoResetEvent(false);
            m_workerThread = new Thread(DoUpdateWork);
            m_workerThread.Name = "ObjectsSynchronizer worker";            
            m_workerThread.Start();

            m_deleteIdsQueue = new List<Tuple<DateTime, int>>();

            world.GameObjectStatusChanged += GameObjectStatusChanged;            
        }

        private void GameObjectStatusChanged(object sedner, GameObjectStatusChangedEventArg args)
        {
            GameObject obj = args.GameObject;

            // commit only uncommited objects
            if (args.Status == GameObjectStatus.Created && obj.Id == null)
            {
                m_commitQueue.Add(obj);
            }
        }

        private void UpdateMaxId(int id)
        {
            m_maxId = Math.Max(m_maxId, id);
        }

        private void CommitNewObjects()
        {
            if (m_commitQueue.Count == 0)
                return;

            CommitResult result = m_client.CommitObjects(m_ownGuid, m_commitQueue.ToArray());

            for (int i = 0; i < result.ObjectsIds.Count; i++)
            {
                UpdateMaxId(result.ObjectsIds[i]);

                m_commitQueue[i].Id = result.ObjectsIds[i];
            }

            m_commitQueue.Clear();
        }
        private void ProcessServerGameWorldEvents()
        {
            GetEventsLogSinceResult getNewObjectsResult = m_client.GetEventsLogSince(m_ownGuid, m_lastTimestamp);

            if (getNewObjectsResult.LogItems.Any())
                m_lastTimestamp = getNewObjectsResult.LogItems.Last().Timestamp;            

            foreach (var logItem in getNewObjectsResult.LogItems)
            {
                ProcessServerEvent(logItem);
            }
        }

        private void ProcessServerEvent(GameEventsLogItem logItem)
        {
            if (logItem is GameObjectAddedLogItem)
            {
                var item = logItem as GameObjectAddedLogItem;

                // preventing repeated game objects adding (e.g. own plane or any another commited objects such as bullets)
                if (!m_clientWorld.ContainsGameObjectWithId(item.GameObject.Id.Value))
                {
                    UpdateMaxId(item.GameObject.Id.Value);

                    m_clientWorld.AddGameObject(IntegrityDataHelper.ProcessRecieved(item.GameObject));
                }
            }
            else if (logItem is GameObjectDeletedLogItem)
            {
                var item = logItem as GameObjectDeletedLogItem;

                m_deleteIdsQueue.Add(new Tuple<DateTime, int>(DateTime.Now, item.GameObjectId));
            }
            else if (logItem is BonusAppliedLogItem)
            {
                var item = logItem as BonusAppliedLogItem;

                Bonus bonus = (Bonus)m_clientWorld.GetObjectById(item.BonusId);
                Plane plane = (Plane)m_clientWorld.GetObjectById(item.PlaneId);

                if (bonus != null && plane != null)
                    m_clientWorld.RaiseBonusAppliedEvent(bonus, plane);
            }
            else if (logItem is GameObjectExplodedLogItem)
            {
                var item = logItem as GameObjectExplodedLogItem;

                GameObject exploded = m_clientWorld.GetObjectById(item.GameObjectId);

                if (exploded != null)
                {
                    m_clientWorld.RaiseExplosionEvent(exploded);
                }
            }
        }
        private void ProcessDefferedRemoving()
        {
            var deleted = new List<Tuple<DateTime, int>>();

            foreach (var item in m_deleteIdsQueue)
            {
                // when DELETE DELAY passed
                if ((DateTime.Now - item.Item1 /*server's delete event receiving time*/) > DELETE_DELAY)
                {
                    GameObject deleting = m_clientWorld.GetObjectById(item.Item2);

                    if (deleting != null) // if game world have object with specified ID
                    {
                        m_clientWorld.ExplicitlyRemoveGameObject(deleting);
                        deleted.Add(item);
                    }
                }
                else
                {
                    // see further is needless - time have not passed yet
                    return;
                }
            }

            deleted.ForEach(item => m_deleteIdsQueue.Remove(item));
        }
        private void SendOwnPlaneParameters()
        {
            m_client.UpdatePlane(m_ownGuid, new PlaneMutableInformation(m_ownPlane));
        }
        private void UpdateEnemyPlanes()
        {
            // when server does not contains some client's worls planes - we should to destroy this plane on the client side

            List<Plane> localPlanes;

            using (var handle = m_clientWorld.GameObjectsSafeReadHandle)
            {
                localPlanes = handle.Items
                    .Where(gameObj => gameObj is Plane)
                    .Cast<Plane>()
                    .ToList();
            }

            List<Plane> detectedPlanes = new List<Plane>();

            foreach (var planeInfo in m_client.GetPlanesInfo(m_ownGuid))
            {
                Plane localPlane = localPlanes.SingleOrDefault(o => o.Id == planeInfo.Id);

                if (localPlane != null)
                {
                    if (localPlane != m_ownPlane)
                        planeInfo.Apply(localPlane);
                    else
                        planeInfo.ApplyToOwnPlaneOnClient(localPlane);

                    detectedPlanes.Add(localPlane);
                }
                else
                {
                    //  there are some enemy planes that exists on the server but does not exists on the client side
                    throw new Exception("FIX! Incorrect game situation");
                }
            }

            // if some local plane remains it means that on server side it was destructed - then we should to destruct it on the client side
            foreach (var garbagePlane in localPlanes.Except(detectedPlanes))
            {
                m_clientWorld.ExplicitlyRemoveGameObject(garbagePlane);
            }
        }

        private void DoUpdateWork()
        {
            while (true)
            {
                m_updateEvent.WaitOne();

                if (m_isFirstUpdate)
                {
                    ProcessServerGameWorldEvents();
                    m_isFirstUpdate = false;
                    continue;
                }

                CommitNewObjects();
                SendOwnPlaneParameters();
                ProcessServerGameWorldEvents();
                ProcessDefferedRemoving();
                UpdateEnemyPlanes(); // only after getting new objects from server
            }
        }

        public void Update()
        {
            // allows worker thread to do the job
            m_updateEvent.Set();
        }
    }
}

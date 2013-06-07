using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Core.Common;
using NRPlanes.Client.NRPlanesServerReference;
using NRPlanes.ServerData.MutableInformations;
using NRPlanes.ServerData.OperationResults;
using NRPlanes.ServerData;
using System.Threading;

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
        private readonly GameWorld m_world;
        private readonly Guid m_ownGuid;
        private readonly Plane m_ownPlane;

        private int m_maxId;
        private List<GameObject> m_commitQueue;

        private Thread m_workerThread;
        private AutoResetEvent m_updateEvent;
        private bool m_isFirstUpdate;

        private const float MAX_UPDATE_RATE = 50.0f;

        public ObjectsSynchronizer(GameServiceClient client, GameWorld world, Guid ownGuid, Plane ownPlane)
        {
            m_ownGuid = ownGuid;
            m_ownPlane = ownPlane;
            m_client = client;
            m_world = world;

            m_isFirstUpdate = true;
            m_maxId = -1;
            m_commitQueue = new List<GameObject>();

            m_updateEvent = new AutoResetEvent(false);
            m_workerThread = new Thread(DoUpdateWork);
            m_workerThread.Start();

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
        private void GetAndProcessNewObjects()
        {
            GetNewObjectsResult getNewObjectsResult = m_client.GetNewObjects(m_ownGuid, m_maxId + 1);

            foreach (var obj in getNewObjectsResult.Objects)
            {
                UpdateMaxId(obj.Id.Value);

                m_world.AddGameObject(IntegrityDataHelper.PreprocessRecieved(obj));
            }
        }
        private void SendOwnPlaneParameters()
        {
            m_client.UpdatePlane(m_ownGuid, new PlaneMutableInformation(m_ownPlane));
        }
        private void UpdateEnemyPlanes()
        {
            foreach (var enemyPlaneInfo in m_client.GetPlanesInfo(m_ownGuid))
            {
                Plane enemyPlane = null;

                using (var handle = m_world.GameObjectsSafeReadHandle)
                {
                    enemyPlane = (Plane)handle.Items.SingleOrDefault(o => o.Id == enemyPlaneInfo.Id);
                }

                if (enemyPlane != null)
                    enemyPlaneInfo.Apply(enemyPlane);
            }
        }

        private void DoUpdateWork()
        {
            while (true)
            {
                m_updateEvent.WaitOne();

                if (m_isFirstUpdate)
                {
                    GetAndProcessNewObjects();
                    m_isFirstUpdate = false;
                    continue;
                }

                CommitNewObjects();
                SendOwnPlaneParameters();
                GetAndProcessNewObjects();
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

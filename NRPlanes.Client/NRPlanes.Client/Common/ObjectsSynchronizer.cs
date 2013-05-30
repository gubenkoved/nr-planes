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
    /// - subscribes on GameWorld events to commit created objects
    /// - and receive objects created by another users.
    /// </summary>
    public class ObjectsSynchronizer
    {
        private readonly GameServiceClient _client;
        private readonly GameWorld _world;
        private readonly Guid _ownGuid;
        private readonly Plane _ownPlane;

        private int _maxId;
        private List<GameObject> _commitQueue;

        private Thread _workerThread;
        private AutoResetEvent _updateEvent;
        private bool _isFirstUpdate;

        private const float MAX_UPDATE_RATE = 50.0f;

        public ObjectsSynchronizer(GameServiceClient client, GameWorld world, Guid ownGuid, Plane ownPlane)
        {
            _ownGuid = ownGuid;
            _ownPlane = ownPlane;
            _client = client;
            _world = world;

            _isFirstUpdate = true;
            _maxId = -1;
            _commitQueue = new List<GameObject>();

            _updateEvent = new AutoResetEvent(false);
            _workerThread = new Thread(DoUpdateWork);
            _workerThread.Start();

            world.GameObjectStatusChanged += GameObjectStatusChanged;            
        }

        private void GameObjectStatusChanged(object sedner, GameObjectStatusChangedEventArg args)
        {
            GameObject obj = args.GameObject;

            // commit only uncommited objects
            if (args.Status == GameObjectStatus.Created && obj.Id == null)
            {
                _commitQueue.Add(obj);
            }
        }

        private void UpdateMaxId(int id)
        {
            _maxId = Math.Max(_maxId, id);
        }

        private void CommitNewObjects()
        {
            if (_commitQueue.Count == 0)
                return;

            CommitResult result = _client.CommitObjects(_ownGuid, _commitQueue.ToArray());

            for (int i = 0; i < result.ObjectsIds.Count; i++)
            {
                UpdateMaxId(result.ObjectsIds[i]);

                _commitQueue[i].Id = result.ObjectsIds[i];
            }

            _commitQueue.Clear();
        }
        private void GetAndProcessNewObjects()
        {
            GetNewObjectsResult getNewObjectsResult = _client.GetNewObjects(_ownGuid, _maxId + 1);

            foreach (var obj in getNewObjectsResult.Objects)
            {
                UpdateMaxId(obj.Id.Value);

                _world.AddGameObject(IntegrityDataHelper.PreprocessRecieved(obj));
            }
        }
        private void SendOwnPlaneParameters()
        {
            _client.UpdatePlane(_ownGuid, new PlaneMutableInformation(_ownPlane));
        }
        private void UpdateEnemyPlanes()
        {
            foreach (var enemyPlaneInfo in _client.GetPlanesInfo(_ownGuid))
            {
                Plane enemyPlane = null;

                _world.PerformSafeGameObjectCollectionOperation( () =>
                    enemyPlane = (Plane)_world.GameObjects.SingleOrDefault(o => o.Id == enemyPlaneInfo.Id));

                if (enemyPlane != null)
                    enemyPlaneInfo.Apply(enemyPlane);
            }
        }

        private void DoUpdateWork()
        {
            while (true)
            {
                _updateEvent.WaitOne();

                if (_isFirstUpdate)
                {
                    GetAndProcessNewObjects();
                    _isFirstUpdate = false;
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
            _updateEvent.Set();
        }
    }
}

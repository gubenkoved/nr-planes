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

namespace NRPlanes.Server
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single, 
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : IGameService
    {
        public GameWorld World { get; private set; }
        public double FPS { get; private set; }

        private int _lastObjectId;
        private Dictionary<Guid, Plane> _playerToPlaneMapping;
        private Action<string> _log;

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
        {
            _playerToPlaneMapping = new Dictionary<Guid, Plane>();

            World = new GameWorld(new Size(800, 800), 50, 13);

            Task.Factory.StartNew(GameWorldUpdate);                
        }

        public GameService(Action<string> logAction)
            :this()
        {
            _log = logAction;
        }

        public JoinResult Join()
        {
            JoinResult result = new JoinResult()
            {
                PlayerGuid = Guid.NewGuid(),
                LogicalSize = World.Size,
                StaticObjects = World.StaticObjects
            };

            _playerToPlaneMapping[result.PlayerGuid] = null;

            if (_log != null)
                _log(string.Format("Player joined (id={0})", result.PlayerGuid));

            return result;
        }        

        public CommitResult CommitObjects(Guid playerGuid, List<GameObject> objects)
        {
            CommitResult result = new CommitResult() { ObjectsIds = new List<int>(objects.Count) };

            foreach (var obj in objects)
            {
                if (obj is Plane)
                    _playerToPlaneMapping[playerGuid] = obj as Plane; // If player commit plane when it his own plane

                obj.Id = _lastObjectId++;

                World.AddGameObject(IntegrityDataHelper.PreprocessRecieved(obj));

                result.ObjectsIds.Add(obj.Id.Value);
            }

            if (_log != null)
                _log(string.Format("{0} object(s) has commited", objects.Count));

            return result;
        }

        public GetNewObjectsResult GetNewObjects(Guid playerGuid, int minId)
        {
            GetNewObjectsResult newObjectsResult = new GetNewObjectsResult();

            // preventing objects collectiong changing
            World.PerformSafeGameObjectCollectionOperation(() => 
                newObjectsResult.Objects = World.GameObjects.Where(o => o.Id.Value >= minId && !o.IsGarbage).ToList());

            if (newObjectsResult.Objects.Count > 0)
                if (_log != null)
                    _log(string.Format("{0} objects has sent to player with id={1} (minId={2})", newObjectsResult.Objects.Count, playerGuid, minId));

            return newObjectsResult;
        }

        public void UpdatePlane(Guid playerGuid, PlaneMutableInformation info)
        {
            Plane playerPlane = _playerToPlaneMapping[playerGuid];

            info.Apply(playerPlane);
        }


        public IEnumerable<PlaneMutableInformation> GetPlanesInfo(Guid playerGuid)
        {
            Plane playerPlane = _playerToPlaneMapping[playerGuid];

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
    }
}

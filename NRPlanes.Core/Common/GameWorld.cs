using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.StaticObjects;
using NRPlanes.Core.Controllers;
using NRPlanes.Core.Aliens;
using NRPlanes.Core.Logging;

namespace NRPlanes.Core.Common
{
    public class GameWorld : IUpdatable
    {
        private readonly ThreadSafeCollection<GameObject> m_safeGameObjects;        
        public ThreadSafeCollection<GameObject>.SafeReadHandle<GameObject> GameObjectsSafeReadHandle
        {
            get { return m_safeGameObjects.SafeRead(); }
        }
        public int GameObjectsCount
        {
            get
            {
                return m_safeGameObjects.Count;
            }
        }

        private readonly List<StaticObject> m_staticObjects;
        public IEnumerable<StaticObject> StaticObjects
        {
            get { return m_staticObjects; }
        }

        private readonly List<PlaneControllerBase> m_planeControllers;
        public IEnumerable<PlaneControllerBase> PlaneControllers
        {
            get
            {
                return m_planeControllers;
            }
        }

        public AliensAppearingStrategy AliensAppearingStrategy { get; set; }

        public Size Size { get; private set; }
        public Random Random { get; private set; }

        public GameWorld(Size logicalSize)
        {
            Logger.Log("Constructing GameWorld...");

            m_safeGameObjects = new ThreadSafeCollection<GameObject>(); 
            m_staticObjects = new List<StaticObject>();
            m_planeControllers = new List<PlaneControllerBase>();           

            Random = new Random(Environment.TickCount);
            Size = logicalSize;            
        }

        #region Public methods

        public void Update(TimeSpan elapsed)
        {
            if (AliensAppearingStrategy != null)
                AliensAppearingStrategy.Update(elapsed);

            ProcessControllersUpdate(elapsed);
            
            ProcessUpdate(elapsed);

            ProcessStaticObjectsAffection(elapsed);

            ProcessCollisions();
        }

        public void AddGameObject(GameObject obj)
        {
            obj.GameWorldAddObjectDelegate = AddGameObject;

            m_safeGameObjects.Add(obj);

            OnGameObjectStatusChanged(null, new GameObjectStatusChangedEventArg(GameObjectStatus.Created, obj));
        }

        public void AddStaticObject(StaticObject obj)
        {
            m_staticObjects.Add(obj);
        }

        public void AddPlaneController(PlaneControllerBase controller)
        {
            m_planeControllers.Add(controller);
        }

        public void RemovePlaneController(PlaneControllerBase controller)
        {
            m_planeControllers.Remove(controller);
        }

        public void AddGravityBoundsWithPlanets(double gravityBoundsLength, int planetsAmount)
        {
            IEnumerable<StaticObject> planets = GenerateRandomPlanets(planetsAmount, new Rect(gravityBoundsLength, gravityBoundsLength, Size.Width - 2 * gravityBoundsLength, Size.Height - 2 * gravityBoundsLength));
            IEnumerable<StaticObject> gravityBounds = GenerateGravityBounds(Size, gravityBoundsLength);

            m_staticObjects.AddRange(planets);
            m_staticObjects.AddRange(gravityBounds);
        }
        #endregion

        #region Private methods
        private void ProcessControllersUpdate(TimeSpan elapsed)
        {
            foreach (var controller in m_planeControllers)
            {
                controller.Update(elapsed);
            }
        }

        private void ProcessUpdate(TimeSpan elapsed)
        {
            GameObject[] copy = m_safeGameObjects.ToArray();

            for (int i = copy.Length - 1; i >= 0; i--)
            {
                if (!copy[i].IsGarbage)
                    copy[i].Update(elapsed);
                else
                    DeleteGameObject(copy[i]);
            }
        }

        private void ProcessStaticObjectsAffection(TimeSpan elapsed)
        {
            // Note: Can be optimized by Sweep&Prune alg

            foreach (var staticObject in StaticObjects)
            {
                using (var handle = m_safeGameObjects.SafeRead())
                {
                    foreach (var gameObject in handle.Items)
                    {
                        if (staticObject.AbsoluteGeometry.HitTest(gameObject.Position))
                        {
                            staticObject.AffectOnGameObject(gameObject, elapsed);
                        }
                    }
                }
            }
        }

        private void ProcessCollisions()
        {
            var collisions = PhysicEngine.GetCollisions(m_safeGameObjects.ToArray()).ToList();

            foreach (var collision in collisions)
            {
                var a = collision.One;
                var b = collision.Two;

                if (a is Bullet)
                    ((Bullet)a).CollideWith(b);
                else if (b is Bullet)
                    ((Bullet)b).CollideWith(a);

                OnCollisionDetected(this, new CollisionEventArgs(collision));
            }
        }        

        private void DeleteGameObject(GameObject obj)
        {
            m_safeGameObjects.Remove(obj);

            OnGameObjectStatusChanged(null, new GameObjectStatusChangedEventArg(GameObjectStatus.Deleted, obj));
        }

        private static IEnumerable<StaticObject> GenerateGravityBounds(Size worldSize, double gravityBoundsLenght)
        {
            const double acceleration = 100;

            List<StaticObject> gravityFields = new List<StaticObject>();

            var topField = new RectangleGravityField(new Rect(0, worldSize.Height - gravityBoundsLenght, worldSize.Width, gravityBoundsLenght), new Vector(0, -1), acceleration);
            var bottomField = new RectangleGravityField(new Rect(0, 0, worldSize.Width, gravityBoundsLenght), new Vector(0, +1), acceleration);
            var leftField = new RectangleGravityField(new Rect(0, 0, gravityBoundsLenght, worldSize.Height), new Vector(+1, 0), acceleration);
            var rightField = new RectangleGravityField(new Rect(worldSize.Width - gravityBoundsLenght, 0, gravityBoundsLenght, worldSize.Height), new Vector(-1, 0), acceleration);

            gravityFields.Add(topField);
            gravityFields.Add(bottomField);
            gravityFields.Add(leftField);
            gravityFields.Add(rightField);

            return gravityFields;
        }

        private static IEnumerable<StaticObject> GenerateRandomPlanets(int amount, Rect rect)
        {
            Random random = new Random(Environment.TickCount);

            List<StaticObject> planets = new List<StaticObject>();

            for (int i = 0; i < amount; i++)
            {
                //var xPos = GravityBoundsLength + random.NextDouble() * (Size.Width - 2 * GravityBoundsLength);
                var xPos = rect.X + random.NextDouble() * rect.Width;
                //var yPos = GravityBoundsLength + random.NextDouble() * (Size.Height - 2 * GravityBoundsLength);
                var yPos = rect.Y + random.NextDouble() * rect.Height;

                var position = new Vector(xPos, yPos);

                var planetValue = 0.5 + 0.5 * random.NextDouble(); // in [0.5, 1.0]

                var planet = new HealthRecoveryPlanet(position, planetValue * 30.0, planetValue * 6.0);

                planets.Add(planet);
            }

            return planets;
        }
        #endregion

        #region Events
        private void OnGameObjectStatusChanged(object sender, GameObjectStatusChangedEventArg arg)
        {
            if (GameObjectStatusChanged != null)
                GameObjectStatusChanged.Invoke(sender, arg);
        }

        public event EventHandler<GameObjectStatusChangedEventArg> GameObjectStatusChanged;

        private void OnCollisionDetected(object sender, CollisionEventArgs args)
        {
            if (CollisionDetected != null)
                CollisionDetected.Invoke(sender, args);
        }

        public event EventHandler<CollisionEventArgs> CollisionDetected; 
        #endregion
    }
}
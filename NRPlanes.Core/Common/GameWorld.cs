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
        private readonly List<GameObject> m_gameObjects;
        public IEnumerable<GameObject> GameObjects
        {
            get { return m_gameObjects; }
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

            m_gameObjects = new List<GameObject>();
            m_staticObjects = new List<StaticObject>();
            m_planeControllers = new List<PlaneControllerBase>();           

            Random = new Random(Environment.TickCount);
            Size = logicalSize;            
        }

        #region Public methods
        /// <summary>
        /// Perform operation which requires protection from modification game objects collection
        /// </summary>
        /// <param name="safeOperation"></param>
        public void PerformSafeGameObjectCollectionOperation(Action safeOperation)
        {
            // preventing changing game objects collection while this operations is executing
            lock (m_gameObjects)
            {
                safeOperation.Invoke();
            }
        }

        public void Update(TimeSpan elapsed)
        {
            if (AliensAppearingStrategy != null)
                AliensAppearingStrategy.Update(elapsed);

            ProcessControllersUpdate(elapsed);

            // preventing changing game objects collection while this operations is executing
            PerformSafeGameObjectCollectionOperation(() =>
            {
                ProcessUpdate(elapsed);

                ProcessStaticObjectsAffection(elapsed);

                ProcessCollisions();
            });
        }

        public void AddGameObject(GameObject obj)
        {
            obj.GameWorldAddObjectDelegate = AddGameObject;

            PerformSafeGameObjectCollectionOperation(() => m_gameObjects.Add(obj));

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
            for (int i = m_gameObjects.Count - 1; i >= 0; i--)
            {
                if (!m_gameObjects[i].IsGarbage)
                    m_gameObjects[i].Update(elapsed);
                else
                    DeleteGameObject(m_gameObjects[i]);
            }
        }

        private void ProcessStaticObjectsAffection(TimeSpan elapsed)
        {
            // Note: Can be optimized by Sweep&Prune analog
            foreach (var staticObject in StaticObjects)
            {
                foreach (var gameObject in m_gameObjects)
                {
                    if (staticObject.AbsoluteGeometry.HitTest(gameObject.Position))
                    {
                        staticObject.AffectOnGameObject(gameObject, elapsed);
                    }
                }
            }
        }

        private void ProcessCollisions()
        {
            var collisions = PhysicEngine.GetCollisions(m_gameObjects).ToList();

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
            m_gameObjects.Remove(obj);

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
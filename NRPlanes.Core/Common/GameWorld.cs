using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.StaticObjects;
using NRPlanes.Core.Controllers;
using NRPlanes.Core.Aliens;
using NRPlanes.Core.Logging;
using NRPlanes.Core.Bonuses;
using NRPlanes.Core.Bullets;

namespace NRPlanes.Core.Common
{
    public class GameWorld : IUpdatable
    {
        protected readonly ThreadSafeCollection<GameObject> m_safeGameObjects;        
        public ThreadSafeCollection<GameObject>.SafeReadHandle GameObjectsSafeReadHandle
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

        protected readonly List<StaticObject> m_staticObjects;
        public IEnumerable<StaticObject> StaticObjects
        {
            get { return m_staticObjects; }
        }

        protected readonly List<PlaneControllerBase> m_planeControllers;
        public IEnumerable<PlaneControllerBase> PlaneControllers
        {
            get
            {
                return m_planeControllers;
            }
        }

        protected readonly Random m_random = new Random(Environment.TickCount);

        public AliensAppearingStrategy AliensAppearingStrategy { get; set; }
        public Size Size { get; protected set; }

        public GameWorld(Size logicalSize)
        {
            Logger.Log("Constructing GameWorld...");

            m_safeGameObjects = new ThreadSafeCollection<GameObject>(); 
            m_staticObjects = new List<StaticObject>();
            m_planeControllers = new List<PlaneControllerBase>();           

            Size = logicalSize;            
        }

        #region Public methods

        public void Update(TimeSpan elapsed)
        {
            if (AliensAppearingStrategy != null)
                AliensAppearingStrategy.Update(elapsed);

            ProcessControllersUpdate(elapsed);
            ProcessStaticObjectsUpdate(elapsed);
            ProcessGameObjectsUpdate(elapsed);
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

        #region Protected methods
        protected virtual void ProcessControllersUpdate(TimeSpan elapsed)
        {
            foreach (var controller in m_planeControllers)
            {
                controller.Update(elapsed);
            }
        }

        protected virtual void ProcessStaticObjectsUpdate(TimeSpan elapsed)
        {
            foreach (var staticObject in m_staticObjects)
            {
                staticObject.Update(elapsed);
            }
        }

        protected virtual void ProcessGameObjectsUpdate(TimeSpan elapsed)
        {
            using (var handle = m_safeGameObjects.SafeRead())
            {
                List<GameObject> garbage = new List<GameObject>();

                foreach (var gameObject in handle.Items)
                {
                    if (!gameObject.IsGarbage)
                        gameObject.Update(elapsed);
                    else
                        garbage.Add(gameObject);
                }

                garbage.ForEach(garbageObject => DeleteGameObject(garbageObject));
            }
        }

        protected virtual void ProcessStaticObjectsAffection(TimeSpan elapsed)
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

        protected virtual void ProcessCollisions()
        {            
            foreach (var collision in PhysicEngine.GetCollisions(m_safeGameObjects.ToArray()))
            {
                var a = collision.FirstObject;
                var b = collision.SecondObject;

                if (collision.CheckTypesBoth(typeof(Bullet), typeof(GameObject)))
                {
                    Bullet bullet = a is Bullet ? (Bullet)a : (Bullet)b;
                    GameObject obj = a is Bullet ? b : a;

                    bullet.CollideWith(obj);
                }

                if (collision.CheckTypesBoth(typeof(Bonus), typeof(Plane)))
                {
                    Bonus bonus = a is Bonus ? (Bonus)a : (Bonus)b;
                    Plane plane = a is Bonus ? (Plane)b : (Plane)a;

                    if (!plane.IsGarbage)
                    {
                        ApplyBonus(bonus, plane);
                    }
                }

                OnCollisionDetected(this, new CollisionEventArgs(collision));
            }
        }

        protected virtual void ApplyBonus(Bonus bonus, Plane plane)
        {
            Logger.Log(string.Format("Apply bonus {0}", bonus));

            bonus.Apply(plane);
        }

        protected void DeleteGameObject(GameObject obj)
        {
            BeforeDeleteGameObject(obj);

            m_safeGameObjects.Remove(obj);

            OnGameObjectStatusChanged(null, new GameObjectStatusChangedEventArg(GameObjectStatus.Deleted, obj));
        }

        protected virtual void BeforeDeleteGameObject(GameObject obj)
        {
            // if some plane destructed - generate bonus
            if (obj is Plane)
            {
                GenerateBonus(obj.Position);
            }
        }

        private void GenerateBonus(Vector position)
        {
            Bonus bonus = new HealthBonus(position, 333);

            Logger.Log(string.Format("Add bonus {0} in position {1}", bonus, position));

            m_safeGameObjects.Add(bonus);
        }

        protected static IEnumerable<StaticObject> GenerateGravityBounds(Size worldSize, double gravityBoundsLenght)
        {
            const double acceleration = 100;

            // top field
            yield return new RectangleGravityField(new Rect(0, worldSize.Height - gravityBoundsLenght, worldSize.Width, gravityBoundsLenght), new Vector(0, -1), acceleration);
            // bottom
            yield return new RectangleGravityField(new Rect(0, 0, worldSize.Width, gravityBoundsLenght), new Vector(0, +1), acceleration);
            // left
            yield return new RectangleGravityField(new Rect(0, 0, gravityBoundsLenght, worldSize.Height), new Vector(+1, 0), acceleration);
            // right
            yield return new RectangleGravityField(new Rect(worldSize.Width - gravityBoundsLenght, 0, gravityBoundsLenght, worldSize.Height), new Vector(-1, 0), acceleration);
        }

        protected static IEnumerable<StaticObject> GenerateRandomPlanets(int amount, Rect rect)
        {
            Random random = new Random(Environment.TickCount);

            for (int i = 0; i < amount; i++)
            {
                Vector position = new Vector(
                    rect.X + random.NextDouble() * rect.Width,
                    rect.Y + random.NextDouble() * rect.Height);
                double planetValueSeed = 0.5 + 0.5 * random.NextDouble(); // in [0.5, 1.0]

                yield return new HealthRecoveryPlanet(position, planetValueSeed * 30.0, planetValueSeed * 6.0);
            }
        }
        #endregion

        #region Events
        protected void OnGameObjectStatusChanged(object sender, GameObjectStatusChangedEventArg arg)
        {
            if (GameObjectStatusChanged != null)
                GameObjectStatusChanged.Invoke(sender, arg);
        }

        public event EventHandler<GameObjectStatusChangedEventArg> GameObjectStatusChanged;

        protected void OnCollisionDetected(object sender, CollisionEventArgs args)
        {
            if (CollisionDetected != null)
                CollisionDetected.Invoke(sender, args);
        }

        public event EventHandler<CollisionEventArgs> CollisionDetected; 
        #endregion
    }
}
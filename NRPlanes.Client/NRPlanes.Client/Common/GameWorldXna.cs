//#define DEBUG_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NRPlanes.Client.GameComponents;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Common;
using NRPlanes.Core.Engines;
using NRPlanes.Core.Planes;
using NRPlanes.Core.StaticObjects;
using NRPlanes.Core.Weapons;
using NRPlanes.Client.Sound;
using System.Threading;
using System.Collections.Concurrent;
using NRPlanes.Client.Particles;

using Plane = NRPlanes.Core.Common.Plane;
using NRPlanes.Core.Bonuses;

namespace NRPlanes.Client.Common
{
    public class GameWorldXna : DrawableGameComponent
    {
        private ThreadSafeCollection<MyDrawableGameComponent> m_safeDrawableGameComponents;
        public ThreadSafeCollection<MyDrawableGameComponent>.SafeReadHandle DrawableGameComponentsSafeReadHandle
        {
            get
            {
                return m_safeDrawableGameComponents.SafeRead();
            }
        }

        private List<Particle> m_particles;
        public IEnumerable<Particle> Particles
        {
            get
            {
                return m_particles;
            }
        }

        private readonly GameWorld m_gameWorld;
        public GameWorld GameWorld
        {
            get { return m_gameWorld; }
        }

        private SpriteBatch m_spriteBatch;

        private Texture2D m_background;
        // normal (with 1.0 scale factor) visible width of background image
        private readonly int m_backgroundVisiblePartWidth;        

        private readonly InstanceMapper m_instanceMapper;        
        private readonly SoundManager m_soundManager;
        public SoundManager SoundManager
        {
            get
            {
                return m_soundManager;
            }
        }

        private RenderTarget2D m_lastFrameRenderTarget;

        private readonly Dictionary<GameObject, DrawableGameObject> m_gameObjectMapping;
        private readonly Dictionary<Equipment, DrawableEquipment> m_equipmentMapping;

        private readonly CoordinatesTransformer m_coordinatesTransformer;
        public CoordinatesTransformer CoordinatesTransformer
        {
            get { return m_coordinatesTransformer; }
        }

        public GameObject CenterOfViewGameObject { get; set; }

        public new PlanesGame Game
        {
            get { return base.Game as PlanesGame; }
        }

        public GameWorldXna(PlanesGame game, GameWorld gameWorld, Rectangle gameFieldRectangle)
            : base(game)
        {
            m_safeDrawableGameComponents = new ThreadSafeCollection<MyDrawableGameComponent>();

            m_particles = new List<Particle>();

            m_gameWorld = gameWorld;
            m_gameWorld.GameObjectStatusChanged += GameObjectStatusChanged;
            m_gameWorld.CollisionDetected += CollisionDetected;

            m_backgroundVisiblePartWidth = gameFieldRectangle.Width;

            m_coordinatesTransformer = new CoordinatesTransformer(m_gameWorld.Size, gameFieldRectangle, 180);
            m_coordinatesTransformer.ScaleToFit();

            m_instanceMapper = new InstanceMapper(game, m_coordinatesTransformer);

            m_gameObjectMapping = new Dictionary<GameObject, DrawableGameObject>();
            m_equipmentMapping = new Dictionary<Equipment, DrawableEquipment>();

            m_soundManager = new SoundManager(game, () => m_coordinatesTransformer.VisibleLogicalRectangle);

            m_lastFrameRenderTarget = new RenderTarget2D(game.Graphics.GraphicsDevice,
                game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight,
                false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PlatformContents);

            FillInstanceMapper();
        }

        public override void Initialize()
        {
            m_spriteBatch = new SpriteBatch(Game.Graphics.GraphicsDevice);

            m_background = Game.Content.Load<Texture2D>("Images/background");

            GrabStaticObjects();

            base.Initialize();
        }

        private void FillInstanceMapper()
        {
            #region Game objects
            m_instanceMapper.AddMapping(typeof(XWingPlane), typeof(XWingPlaneXna));
            m_instanceMapper.AddMapping(typeof(LaserBullet), typeof(LaserBulletXna));
            #endregion

            #region Equipments
            m_instanceMapper.AddMapping(typeof(RocketEngine), typeof(RocketEngineXna));
            m_instanceMapper.AddMapping(typeof(IonEngine), typeof(IonEngineXna));
            m_instanceMapper.AddMapping(typeof(LaserGun), typeof(LaserGunXna));
            m_instanceMapper.AddMapping(typeof(Shield), typeof(ShieldXna));
            #endregion

            #region Static objects
            m_instanceMapper.AddMapping(typeof(RectangleGravityField), typeof(RectangleGravityFieldXna));
            m_instanceMapper.AddMapping(typeof(HealthRecoveryPlanet), typeof(HealthRecoveryPlanetXna));
            #endregion

            #region Bonuses
            m_instanceMapper.AddMapping(typeof(HealthBonus), typeof(BonusXna), new object[] { Color.Red, Game.Content.Load<Texture2D>("Bonuses/bonus") });
            #endregion

        }
        private void GameObjectStatusChanged(object sender, GameObjectStatusChangedEventArg arg)
        {
            switch (arg.Status)
            {
                case GameObjectStatus.Created:
                    WhenGameObjectAdded(arg.GameObject);
                    break;
                case GameObjectStatus.Deleted:
                    WhenGameObjectDeleted(arg.GameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        private void CollisionDetected(object sender, CollisionEventArgs args)
        {
            Collision collision = args.Collision;

            if (collision.CheckTypes(typeof(Bullet)) 
                || collision.CheckTypes(typeof(Bullet), typeof(Bonus)))
            {
                if (collision.FirstObject.IsGarbage)
                    AddExplosion(collision.FirstObject);

                if (collision.SecondObject.IsGarbage)
                    AddExplosion(collision.SecondObject);
            }

            if (collision.CheckTypes(typeof(Bullet), typeof(Plane)))
            {
                if (collision.FirstObject is Bullet)
                    AddExplosion(collision.FirstObject);
                else
                    AddExplosion(collision.SecondObject);
            }           
        }
        private void AddExplosion(GameObject exploded)
        {
            ExplosionXna explosion = new ExplosionXna(Game, exploded, m_coordinatesTransformer);

            m_safeDrawableGameComponents.Add(explosion);
        }
        private void WhenGameObjectAdded(GameObject gameObject)
        {            
            var xnaRelatedGameObject = (DrawableGameObject)m_instanceMapper.CreateInstance(gameObject);

            m_gameObjectMapping[gameObject] = xnaRelatedGameObject;
            
            m_safeDrawableGameComponents.Add(xnaRelatedGameObject);

            if (gameObject is IHaveEquipment)
            {
                foreach (var equipment in (gameObject as IHaveEquipment).AllEquipment)
                {
                    AddEquipment(equipment);
                }
            }
        }
        private void WhenGameObjectDeleted(GameObject gameObject)
        {
            // add explosion when plane deleted
            if (gameObject is Plane)
            {
                AddExplosion(gameObject);
            }

            DrawableGameObject drawableGameObject = m_gameObjectMapping[gameObject];

            m_gameObjectMapping.Remove(gameObject);
            m_safeDrawableGameComponents.Remove(drawableGameObject);

            if (gameObject is IHaveEquipment)
            {
                var equipmentContainer = gameObject as IHaveEquipment;

                foreach (var equipment in equipmentContainer.AllEquipment)
                {
                    DeleteEquipment(equipment);
                }
            }
        }

        private void AddEquipment(Equipment equipment)
        {
            var xnaRelatedEquipment = (DrawableEquipment)m_instanceMapper.CreateInstance(equipment);

            m_equipmentMapping[equipment] = xnaRelatedEquipment;

            m_safeDrawableGameComponents.Add(xnaRelatedEquipment);
        }
        private void DeleteEquipment(Equipment equipment)
        {
            var drawableEquipment = m_equipmentMapping[equipment];

            m_equipmentMapping.Remove(equipment);

            m_safeDrawableGameComponents.Remove(drawableEquipment);
        }

        public void AddParticle(Particle particle)
        {
            m_particles.Add(particle);
        }
        public void ForceSetCameraOnCenterOfView()
        {
            m_coordinatesTransformer.SetCenterOfView(CenterOfViewGameObject.Position);
        }
        public override void Update(GameTime gameTime)
        {            
            m_soundManager.Update(gameTime.ElapsedGameTime);

            UpdateView(gameTime);

            for (int i = 0; i < m_particles.Count; i++)
            {
                if (m_particles[i].IsGarbage)
                    m_particles.RemoveAt(i);
                else
                    m_particles[i].Update(gameTime);
            }

            using (var handle = m_safeDrawableGameComponents.SafeRead())
            {
                MyDrawableGameComponent[] copy = m_safeDrawableGameComponents.ToArray();

                for (int i = copy.Length - 1; i >= 0; i--)
                {
                    if (!copy[i].IsGarbage)
                        copy[i].Update(gameTime);
                    else
                        m_safeDrawableGameComponents.Remove(copy[i]);
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
                        
            m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);            
            DrawBackground();            
            m_spriteBatch.End();

            m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            using (var handle = m_safeDrawableGameComponents.SafeRead())
            {
                foreach (var drawableGameObject in handle.Items)
                {
                    drawableGameObject.Draw(gameTime, m_spriteBatch);
                }
            }
            m_spriteBatch.End();

            m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            foreach (var particle in m_particles)
            {
                particle.Draw(gameTime, m_spriteBatch);
            }
            m_spriteBatch.End();

            // sprite sorting not available through SpriteBatch.Begin calls            

            m_spriteBatch.Begin();
            DrawAdditionalInfo(gameTime);
            m_spriteBatch.End();

#if DEBUG_MODE
            m_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            DrawDebugInfo();

            m_spriteBatch.End();
#endif
        }

        private void UpdateView(GameTime gameTime)
        {
            const double followingSpeedCoeff = 0.02; // 0 - static, 1 - instantaneous camera following
            const double scaleInertness = 0.995;

            if (CenterOfViewGameObject != null)
            {
                #region View position

                var oldCenter = m_coordinatesTransformer.VisibleLogicalRectangle.Center;

                var offset = CenterOfViewGameObject.Position - oldCenter;

                m_coordinatesTransformer.SetCenterOfView(oldCenter + followingSpeedCoeff * offset);
                #endregion


                #region Scale

                double newScale = Math.Exp(-0.01 * CenterOfViewGameObject.Velocity.Length);

                m_coordinatesTransformer.Scale = m_coordinatesTransformer.Scale * scaleInertness +
                                                newScale * (1.0 - scaleInertness);
                #endregion
            }
        }
        private void DrawBackground()
        {
            Rectangle destination = m_coordinatesTransformer.PhysicalRectangle;

            double backgroundVisiblePartHeight = m_backgroundVisiblePartWidth / m_coordinatesTransformer.Aspect;

            Rect vis = m_coordinatesTransformer.VisibleLogicalRectangle;

            // moving factors belongs range [0; 1]
            double xMovingFactor = (vis.Center.X - vis.Width / 2) / (m_coordinatesTransformer.FullLogicalSize.Width - vis.Width);
            double yMovingFactor = (vis.Center.Y - vis.Height / 2) / (m_coordinatesTransformer.FullLogicalSize.Height - vis.Height);

            //Rectangle source = new Rectangle(
            //    (int)(xMovingFactor * (m_background.Width - m_backgroundVisiblePartWidth)),
            //    (int)((m_background.Height - height) * (1 - yMovingFactor)),
            //    m_backgroundVisiblePartWidth, (int)height); 

            //m_spriteBatch.Draw(m_background, destination, source, Color.White); //, 0.0f, new Vector2(), SpriteEffects.None, 1.0f);

            Vector2 pos = new Vector2((float)(-xMovingFactor * (m_background.Width - m_backgroundVisiblePartWidth)),
                (float)(-1 * (m_background.Height - backgroundVisiblePartHeight) * (1 - yMovingFactor)));

            //float screensCount = (float) (m_coordinatesTransformer.FullLogicalSize.Width / m_coordinatesTransformer.NormalSize.Width);            
            float scale = m_coordinatesTransformer.PhysicalRectangle.Width / m_backgroundVisiblePartWidth;

            m_spriteBatch.Draw(m_background, pos, null, Color.White, 0.0f, new Vector2(), scale, SpriteEffects.None, 0.0f);
        }
        private void DrawDebugInfo()
        {
            GeometryDrawer debugGeomertyDrawer = 
                new GeometryDrawer(                    
                    //Game.Content.Load<Texture2D>("Debug/line"), 
                    Game.Content.Load<Texture2D>("Debug/pixel"),
                    Game.Content.Load<Texture2D>("Debug/point2"),
                    Game.Content.Load<SpriteFont>("Fonts/debug"));

            using (var handle = m_safeDrawableGameComponents.SafeRead())
            {
                foreach (var drawableObj in handle.Items)
                {
                    if (drawableObj is DrawableGameObject)
                    {
                        GameObject gameObject = ((DrawableGameObject)drawableObj).GameObject;

                        debugGeomertyDrawer.Draw(m_spriteBatch, m_coordinatesTransformer, gameObject.CalculateAbsoluteGeometry());

                    }
                    else if (drawableObj is DrawableStaticObject)
                    {
                        StaticObject staticObject = ((DrawableStaticObject)drawableObj).StaticObject;

                        debugGeomertyDrawer.Draw(m_spriteBatch, m_coordinatesTransformer, staticObject.AbsoluteGeometry);
                    }
                }
            }

            
        }
        private void DrawAdditionalInfo(GameTime gameTime)
        {
            var font = Game.Content.Load<SpriteFont>("Fonts/information_font");
            var strings = new[]
                {
                    string.Format("{0:F1} fps", gameTime.ElapsedGameTime.TotalSeconds != 0 ? (1.0 / gameTime.ElapsedGameTime.TotalSeconds) : 0),
                    string.Format(@"{0:hh\:mm\:ss}", gameTime.TotalGameTime),
                    string.Format(@"Particles: {0}", m_particles.Count),
                    string.Format(@"Game objects: {0}", Game.GameManager.GameWorld.GameObjectsCount),
                    string.Format(@"Drawable components: {0}", m_safeDrawableGameComponents.Count),
                };

            const float x = 10f;
            float y = 10f;
            float dy = 12f;

            for (int i = 0; i < strings.Length; i++)
            {
                m_spriteBatch.DrawString(font, strings[i], new Vector2(x, y), Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 0f);

                y += dy;
            }            
        }
        private void GrabStaticObjects()
        {
            foreach (var staticObject in m_gameWorld.StaticObjects)
            {
                var xnaRelatedStaticObject = m_instanceMapper.CreateInstance(staticObject);

                m_safeDrawableGameComponents.Add(xnaRelatedStaticObject);                
            }
        }
    }
}
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
using Plane = NRPlanes.Core.Common.Plane;
using NRPlanes.Client.Sound;
using System.Threading;
using System.Collections.Concurrent;

namespace NRPlanes.Client.Common
{
    public class GameWorldXna : DrawableGameComponent
    {
        private object m_sync = new object();

        private readonly List<MyDrawableGameComponent> m_drawableGameComponents;
        public IEnumerable<MyDrawableGameComponent> DrawableGameComponents
        {
            get 
            {
                // return copy for thread-safety reasons
                return m_drawableGameComponents.ToList(); 
            }
        }

        private readonly GameWorld m_gameWorld;
        public GameWorld GameWorld
        {
            get { return m_gameWorld; }
        }

        private SpriteBatch m_spriteBatch;
        private Texture2D m_background;

        private readonly InstanceMapper m_instanceMapper;
        private readonly SoundManager m_soundManager;

        private readonly CoordinatesTransformer m_coordinatesTransformer;
        public CoordinatesTransformer CoordinatesTransformer
        {
            get { return m_coordinatesTransformer; }
        }

        public GameObject CenterOfView { get; set; }

        public new PlanesGame Game
        {
            get { return base.Game as PlanesGame; }
        }

        public GameWorldXna(PlanesGame game, GameWorld gameWorld, Rectangle gameFieldRectangle)
            : base(game)
        {
            m_drawableGameComponents = new List<MyDrawableGameComponent>();

            m_gameWorld = gameWorld;
            m_gameWorld.GameObjectStatusChanged += GameObjectStatusChanged;
            m_gameWorld.CollisionDetected += CollisionDetected;

            m_coordinatesTransformer = new CoordinatesTransformer(m_gameWorld.Size, gameFieldRectangle, 180);
            m_coordinatesTransformer.ScaleToFit();

            m_instanceMapper = new InstanceMapper(game, m_coordinatesTransformer);

            m_soundManager = SoundManager.CreateInstance(game, () => m_coordinatesTransformer.VisibleLogicalRectangle);

            FillInstanceMapper();

            GrabStaticObjects();
        }

        public override void Initialize()
        {
            m_spriteBatch = new SpriteBatch(Game.Graphics.GraphicsDevice);

            m_background = Game.Content.Load<Texture2D>("Images/background");

            base.Initialize();
        }

        private void FillInstanceMapper()
        {
            #region GameObjects

            m_instanceMapper.AddMapping(typeof(XWingPlane), typeof(XWingPlaneXna));
            m_instanceMapper.AddMapping(typeof(LaserBullet), typeof(LaserBulletXna));

            #endregion

            #region Equipments

            m_instanceMapper.AddMapping(typeof(RocketEngine), typeof(RocketEngineXna));
            m_instanceMapper.AddMapping(typeof(IonEngine), typeof(IonEngineXna));
            m_instanceMapper.AddMapping(typeof(LaserGun), typeof(LaserGunXna));
            m_instanceMapper.AddMapping(typeof(Shield), typeof(ShieldXna));

            #endregion

            #region StaticObjects

            m_instanceMapper.AddMapping(typeof(RectangleGravityField), typeof(RectangleGravityFieldXna));
            m_instanceMapper.AddMapping(typeof(HealthRecoveryPlanet), typeof(HealthRecoveryPlanetXna));

            #endregion

        }
        private void GameObjectStatusChanged(object sender, GameObjectStatusChangedEventArg arg)
        {
            switch (arg.Status)
            {
                case GameObjectStatus.Created:
                    AddGameObject(arg.GameObject);
                    break;
                case GameObjectStatus.Deleted:
                    DeleteGameObject(arg.GameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        private void CollisionDetected(object sender, CollisionEventArgs args)
        {
            var collision = args.Collision;

            if (collision.One.IsGarbage)
                AddExplosion(collision.One);

            if (collision.Two.IsGarbage)
                AddExplosion(collision.Two);
        }
        private void AddDrawableGameComponent(MyDrawableGameComponent component)
        {
            lock (m_sync)
            {
                m_drawableGameComponents.Add(component);
            }
        }
        private void RemoveDrawableGameComponent(MyDrawableGameComponent component)
        {
            lock (m_sync)
            {
                m_drawableGameComponents.Remove(component);
            }
        }
        private void RemoveDrawableGameComponentAt(int i)
        {
            lock (m_sync)
            {
                m_drawableGameComponents.RemoveAt(i);
            }
        }
        private void AddExplosion(GameObject exploded)
        {            
            ExplosionXna explosion = new ExplosionXna(Game, exploded, m_coordinatesTransformer);

            AddDrawableGameComponent(explosion);
        }
        private void AddGameObject(GameObject gameObject)
        {            
            var xnaRelatedGameObject = m_instanceMapper.CreateInstance(gameObject);

            AddDrawableGameComponent(xnaRelatedGameObject);

            if (gameObject is IHaveEquipment)
            {
                foreach (var equipment in (gameObject as IHaveEquipment).AllEquipment)
                {
                    var xnaRelatedEquipment = m_instanceMapper.CreateInstance(equipment);

                    AddDrawableGameComponent(xnaRelatedEquipment);
                }
            }
        }
        private void DeleteGameObject(GameObject gameObject)
        {
            var drawableGameObject = m_drawableGameComponents.OfType<DrawableGameObject>().Single(d => d.GameObject == gameObject);

            RemoveDrawableGameComponent(drawableGameObject);

            if (gameObject is IHaveEquipment)
            {
                var equipmentContainer = gameObject as IHaveEquipment;

                foreach (var equipment in equipmentContainer.AllEquipment)
                {
                    DeleteEquipment(equipment);
                }
            }
        }
        private void DeleteEquipment(Equipment equipment)
        {
            var drawableEquipment = m_drawableGameComponents.OfType<DrawableEquipment>().Single(e => e.Equipment == equipment);

            RemoveDrawableGameComponent(drawableEquipment);
        }

        public void ForceSetCameraOnCenterOfView()
        {
            m_coordinatesTransformer.SetCenterOfView(CenterOfView.Position);
        }

        public override void Update(GameTime gameTime)
        {            
            m_soundManager.Update(gameTime.ElapsedGameTime);

            UpdateView(gameTime);

            for (int i = m_drawableGameComponents.Count - 1; i >= 0; i--)
            {
                if (!m_drawableGameComponents[i].IsGarbage)
                    m_drawableGameComponents[i].Update(gameTime);
                else
                    RemoveDrawableGameComponentAt(i);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            m_spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            DrawBackground();
            DrawAdditionalInfo(gameTime);

            lock (m_sync) // because m_drawableGameComponents collection can be modified while iterating
            {
                foreach (var drawableGameObject in m_drawableGameComponents)
                {
                    drawableGameObject.Draw(gameTime, m_spriteBatch);
                }
            }

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

            if (CenterOfView != null)
            {
                #region View position

                var oldCenter = m_coordinatesTransformer.VisibleLogicalRectangle.Center;

                var offset = CenterOfView.Position - oldCenter;

                m_coordinatesTransformer.SetCenterOfView(oldCenter + followingSpeedCoeff * offset);
                #endregion


                #region Scale

                double newScale = Math.Exp(-0.01 * CenterOfView.Velocity.Length);

                m_coordinatesTransformer.Scale = m_coordinatesTransformer.Scale * scaleInertness +
                                                newScale * (1.0 - scaleInertness);
                #endregion
            }
        }

        private void DrawBackground()
        {
            return;

            var destination = m_coordinatesTransformer.PhysicalRectangle;
            
            m_spriteBatch.Draw(m_background, destination, null, Color.White, 0.0f, new Vector2(), SpriteEffects.None, 1.0f);
        }

        private void DrawDebugInfo()
        {
            var debugGeomertyDrawer = 
                new GeometryDrawer(
                    Game.Content.Load<Texture2D>("Debug/line"), 
                    Game.Content.Load<Texture2D>("Debug/point"),
                    Game.Content.Load<SpriteFont>("Fonts/debug"));

            foreach (var drawableObj in DrawableGameComponents)
            {
                if (drawableObj is DrawableGameObject)
                {
                    var gameObject = ((DrawableGameObject) drawableObj).GameObject;

                    debugGeomertyDrawer.Draw(m_spriteBatch, m_coordinatesTransformer, gameObject.CalculateAbsoluteGeometry());
                
                } else if (drawableObj is DrawableStaticObject)
                {
                    var staticObject = ((DrawableStaticObject)drawableObj).StaticObject;

                    debugGeomertyDrawer.Draw(m_spriteBatch, m_coordinatesTransformer, staticObject.AbsoluteGeometry);
                }
            }
        }

        private void DrawAdditionalInfo(GameTime gameTime)
        {
            var font = Game.Content.Load<SpriteFont>("Fonts/information_font");

            var fps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;

            if (!double.IsInfinity(fps))
                m_spriteBatch.DrawString(font, string.Format("{0:F1} fps", fps), new Vector2(10, 10), Color.White);

            var time = gameTime.TotalGameTime;

            m_spriteBatch.DrawString(font, string.Format(@"{0:hh\:mm\:ss}", time), new Vector2(10, 22), Color.White);
        }

        private void GrabStaticObjects()
        {
            foreach (var staticObject in m_gameWorld.StaticObjects)
            {
                var xnaRelatedStaticObject = m_instanceMapper.CreateInstance(staticObject);
                
                m_drawableGameComponents.Add(xnaRelatedStaticObject);
            }
        }
    }
}
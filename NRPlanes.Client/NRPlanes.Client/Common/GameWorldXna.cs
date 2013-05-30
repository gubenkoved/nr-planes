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

namespace NRPlanes.Client.Common
{
    public class GameWorldXna : DrawableGameComponent
    {
        private readonly List<MyDrawableGameComponent> _drawableGameComponents;
        public IEnumerable<MyDrawableGameComponent> DrawableGameComponents
        {
            get { return _drawableGameComponents; }
        }

        private readonly GameWorld _gameWorld;
        public GameWorld GameWorld
        {
            get { return _gameWorld; }
        }

        private SpriteBatch _spriteBatch;
        private Texture2D _background;

        private readonly InstanceMapper _instanceMapper;
        private readonly SoundManager _soundManager;

        private readonly CoordinatesTransformer _coordinatesTransformer;
        public CoordinatesTransformer CoordinatesTransformer
        {
            get { return _coordinatesTransformer; }
        }

        public GameObject CenterOfView { get; set; }

        public new PlanesGame Game
        {
            get { return base.Game as PlanesGame; }
        }

        public GameWorldXna(PlanesGame game, GameWorld gameWorld, Rectangle gameFieldRectangle)
            : base(game)
        {
            _drawableGameComponents = new List<MyDrawableGameComponent>();

            _gameWorld = gameWorld;

            _gameWorld.GameObjectStatusChanged += GameObjectStatusChanged;

            _gameWorld.CollisionDetected += CollisionDetected;

            _coordinatesTransformer = new CoordinatesTransformer(_gameWorld.Size, gameFieldRectangle, 180);
            
            _coordinatesTransformer.ScaleToFit();

            _instanceMapper = new InstanceMapper(game, _coordinatesTransformer);

            _soundManager = SoundManager.CreateInstance(game, () => _coordinatesTransformer.VisibleLogicalRectangle);

            FillInstanceMapper();

            GrabStaticObjects();
        }

        public override void Initialize()
        {
            _spriteBatch = new SpriteBatch(Game.Graphics.GraphicsDevice);

            _background = Game.Content.Load<Texture2D>("Images/background");

            base.Initialize();
        }

        private void FillInstanceMapper()
        {
            #region GameObjects

            _instanceMapper.AddMapping(typeof(XWingPlane), typeof(XWingPlaneXna));
            _instanceMapper.AddMapping(typeof(LaserBullet), typeof(LaserBulletXna));

            #endregion

            #region Equipments

            _instanceMapper.AddMapping(typeof(RocketEngine), typeof(RocketEngineXna));
            _instanceMapper.AddMapping(typeof(IonEngine), typeof(IonEngineXna));
            _instanceMapper.AddMapping(typeof(LaserGun), typeof(LaserGunXna));
            _instanceMapper.AddMapping(typeof(Shield), typeof(ShieldXna));

            #endregion

            #region StaticObjects

            _instanceMapper.AddMapping(typeof(RectangleGravityField), typeof(RectangleGravityFieldXna));
            _instanceMapper.AddMapping(typeof(HealthRecoveryPlanet), typeof(HealthRecoveryPlanetXna));

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

        private void AddExplosion(GameObject exploded)
        {            
            ExplosionXna explosion = new ExplosionXna(Game, exploded, _coordinatesTransformer);

            _drawableGameComponents.Add(explosion);
        }

        private void AddGameObject(GameObject gameObject)
        {
            var xnaRelatedGameObject = _instanceMapper.CreateInstance(gameObject);

            _drawableGameComponents.Add(xnaRelatedGameObject);

            if (gameObject is IHaveEquipment)
            {
                foreach (var equipment in (gameObject as IHaveEquipment).AllEquipment)
                {
                    var xnaRelatedEquipment = _instanceMapper.CreateInstance(equipment);
                    
                    _drawableGameComponents.Add(xnaRelatedEquipment);
                }
            }
        }

        private void DeleteGameObject(GameObject gameObject)
        {
            var drawableGameObject = _drawableGameComponents.OfType<DrawableGameObject>().Single(d => d.GameObject == gameObject);

            _drawableGameComponents.Remove(drawableGameObject);

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
            var drawableEquipment = _drawableGameComponents.OfType<DrawableEquipment>().Single(e => e.Equipment == equipment);

            _drawableGameComponents.Remove(drawableEquipment);
        }

        public void ForceSetCameraOnCenterOfView()
        {
            _coordinatesTransformer.SetCenterOfView(CenterOfView.Position);
        }

        public override void Update(GameTime gameTime)
        {            
            _soundManager.Update(gameTime.ElapsedGameTime);

            UpdateView(gameTime);

            for (int i = _drawableGameComponents.Count - 1; i >= 0; i--)
            {
                if (!_drawableGameComponents[i].IsGarbage)
                    _drawableGameComponents[i].Update(gameTime);
                else
                    _drawableGameComponents.RemoveAt(i);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            DrawBackground();
            DrawAdditionalInfo(gameTime);

            foreach (var drawableGameObject in _drawableGameComponents)
            {
                drawableGameObject.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();
            
            // set it flag TRUE, if you want to se bounding rectangles, and other debug info
            if (false)
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    DrawDebugInfo();
                _spriteBatch.End();
            }
        }

        private void UpdateView(GameTime gameTime)
        {
            const double followingSpeedCoeff = 0.02; // 0 - static, 1 - instantaneous camera following
            const double scaleInertness = 0.995;

            if (CenterOfView != null)
            {
                #region View position

                var oldCenter = _coordinatesTransformer.VisibleLogicalRectangle.Center;

                var offset = CenterOfView.Position - oldCenter;

                _coordinatesTransformer.SetCenterOfView(oldCenter + followingSpeedCoeff * offset);
                #endregion


                #region Scale

                double newScale = Math.Exp(-0.01 * CenterOfView.Velocity.Length);

                _coordinatesTransformer.Scale = _coordinatesTransformer.Scale * scaleInertness +
                                                newScale * (1.0 - scaleInertness);
                #endregion
            }
        }

        private void DrawBackground()
        {
            return;

            var destination = _coordinatesTransformer.PhysicalRectangle;
            
            _spriteBatch.Draw(_background, destination, null, Color.White, 0.0f, new Vector2(), SpriteEffects.None, 1.0f);
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

                    debugGeomertyDrawer.Draw(_spriteBatch, _coordinatesTransformer, gameObject.CalculateAbsoluteGeometry());
                
                } else if (drawableObj is DrawableStaticObject)
                {
                    var staticObject = ((DrawableStaticObject)drawableObj).StaticObject;

                    debugGeomertyDrawer.Draw(_spriteBatch, _coordinatesTransformer, staticObject.AbsoluteGeometry);
                }
            }
        }

        private void DrawAdditionalInfo(GameTime gameTime)
        {
            var font = Game.Content.Load<SpriteFont>("Fonts/information_font");

            var fps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;

            if (!double.IsInfinity(fps))
                _spriteBatch.DrawString(font, string.Format("{0:F1} fps", fps), new Vector2(10, 10), Color.White);

            var time = gameTime.TotalGameTime;

            _spriteBatch.DrawString(font, string.Format(@"{0:hh\:mm\:ss}", time), new Vector2(10, 22), Color.White);
        }

        private void GrabStaticObjects()
        {
            foreach (var staticObject in _gameWorld.StaticObjects)
            {
                var xnaRelatedStaticObject = _instanceMapper.CreateInstance(staticObject);
                
                _drawableGameComponents.Add(xnaRelatedStaticObject);
            }
        }
    }
}
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Client.InfoPanels;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.StaticObjects;

namespace NRPlanes.Client.GameComponents
{
    public class HealthRecoveryPlanetXna : DrawableStaticObject, IOnMinimapDrawable
    {
        public new HealthRecoveryPlanet StaticObject
        {
            get { return base.StaticObject as HealthRecoveryPlanet; }
        }

        private AnimationSpriteDrawer _animationSpriteDrawer;

        private Texture2D _minimapImage;

        public HealthRecoveryPlanetXna(PlanesGame game, HealthRecoveryPlanet healthRecoveryPlanet, CoordinatesTransformer coordinatesTransformer)
            : base(game, healthRecoveryPlanet, coordinatesTransformer)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var frameSize = new Size(400, 400);

            if (_animationSpriteDrawer == null)
            {
                _animationSpriteDrawer
                    = new AnimationSpriteDrawer(
                        Game.Content.Load<Texture2D>("Animations/health_recovery"),
                        frameSize, TimeSpan.FromMilliseconds(50), true, true);
            }

            var origin = new Vector2((float) _animationSpriteDrawer.FrameSize.Width / 2.0f,
                                     (float) _animationSpriteDrawer.FrameSize.Height / 2.0f);

            var scaleVector =
                CoordinatesTransformer.CreateScaleVector(StaticObject.AbsoluteGeometry.BoundingRectangle.Size,
                                                         new Size(_animationSpriteDrawer.FrameSize.Width,
                                                                  _animationSpriteDrawer.FrameSize.Height));

            _animationSpriteDrawer.Draw(gameTime,
                                        spriteBatch,
                                        CoordinatesTransformer.Transform(StaticObject.AbsoluteGeometry.Center),
                                        Color.Red,
                                        0f,
                                        origin,
                                        scaleVector,
                                        SpriteEffects.None,
                                        LayersDepths.StaticObject);
        }

        public void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer)
        {
            if (_minimapImage == null)
                _minimapImage = Game.Content.Load<Texture2D>("Minimap/circle");

            var origin = new Vector2((float) _minimapImage.Width / 2.0f,
                                     (float) _minimapImage.Height / 2.0f);

            var scaleVector =
                coordinatesTransformer.CreateScaleVector(StaticObject.AbsoluteGeometry.BoundingRectangle.Size,
                                                         new Size(_minimapImage.Width,
                                                                  _minimapImage.Height));

            minimapSpriteBatch.Draw(_minimapImage,
                                    coordinatesTransformer.Transform(StaticObject.AbsoluteGeometry.Center),
                                    null,
                                    Color.Red,
                                    0,
                                    origin,
                                    scaleVector,
                                    SpriteEffects.None,
                                    LayersDepths.StaticObject);
        }
    }
}
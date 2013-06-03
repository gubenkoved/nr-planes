using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Client.InfoPanels;
using NRPlanes.Core.Common;
using NRPlanes.Core.Primitives;
using NRPlanes.Core.StaticObjects;
using NRPlanes.Client.Particles;

namespace NRPlanes.Client.GameComponents
{
    public class HealthRecoveryPlanetXna : DrawableStaticObject, IOnMinimapDrawable
    {
        public new HealthRecoveryPlanet StaticObject
        {
            get { return base.StaticObject as HealthRecoveryPlanet; }
        }

        private AnimationSpriteDrawer m_animationSpriteDrawer;
        private Texture2D m_minimapImage;
        private SymmetricParticlesEmitter m_particlesEmitter;
        private int m_drawCount;

        public HealthRecoveryPlanetXna(PlanesGame game, HealthRecoveryPlanet healthRecoveryPlanet, CoordinatesTransformer coordinatesTransformer)
            : base(game, healthRecoveryPlanet, coordinatesTransformer)
        {
            m_particlesEmitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                PositionDeviationRadius = 6,
                VelocityDeviationRadius = 4,
                AlphaVelocityDeviationFactor = 0.5
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_animationSpriteDrawer == null)
            {
                m_animationSpriteDrawer
                    = new AnimationSpriteDrawer(Game.Content.Load<Texture2D>("Animations/health_recovery"),
                        new Size(400, 400), TimeSpan.FromMilliseconds(50), true, true);
            }

            var origin = new Vector2((float) m_animationSpriteDrawer.FrameSize.Width / 2.0f,
                                     (float) m_animationSpriteDrawer.FrameSize.Height / 2.0f);

            var scaleVector =
                CoordinatesTransformer.CreateScaleVector(
                    StaticObject.AbsoluteGeometry.BoundingRectangle.Size,
                    new Size(m_animationSpriteDrawer.FrameSize.Width,
                    m_animationSpriteDrawer.FrameSize.Height));

            m_animationSpriteDrawer.Draw(gameTime,
                                        spriteBatch,
                                        CoordinatesTransformer.Transform(StaticObject.AbsoluteGeometry.Center),
                                        Color.Red,
                                        0f,
                                        origin,
                                        scaleVector,
                                        SpriteEffects.None,
                                        LayersDepths.StaticObject);

            // do every 4 draw
            if (m_drawCount++ % 4 == 0)
            {
                
                foreach (var affectedObject in StaticObject.AffectedGameObjects)
                {
                    double crossSize = StaticObject.RecoverySpeed / 4;

                    m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer, ParticleType.Cross)
                    {
                        Position = affectedObject.Position,
                        Color = Color.Red,
                        Size = new Size(crossSize, crossSize),
                        AlphaVelocity = -0.5f
                    }, 1);
                }
            }
        }

        public void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer)
        {
            if (m_minimapImage == null)
                m_minimapImage = Game.Content.Load<Texture2D>("Minimap/circle");

            var origin = new Vector2((float) m_minimapImage.Width / 2.0f,
                                     (float) m_minimapImage.Height / 2.0f);

            var scaleVector =
                coordinatesTransformer.CreateScaleVector(StaticObject.AbsoluteGeometry.BoundingRectangle.Size,
                                                         new Size(m_minimapImage.Width,
                                                                  m_minimapImage.Height));

            minimapSpriteBatch.Draw(m_minimapImage,
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
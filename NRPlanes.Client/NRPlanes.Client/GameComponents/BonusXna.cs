using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NRPlanes.Client.Common;
using NRPlanes.Client.InfoPanels;
using NRPlanes.Core.Bonuses;
using NRPlanes.Core.Primitives;
using NRPlanes.Client.Sound;
using NRPlanes.Client.Particles;

namespace NRPlanes.Client.GameComponents
{
    public class BonusXna : DrawableGameObject, IOnMinimapDrawable
    {
        private Texture2D m_minimapTexture;
        protected Texture2D m_texture;

        public Color Color { get; private set; }
        public Bonus Bonus { get; private set; }

        private SymmetricParticlesEmitter m_emitter;

        private const double PARTICLES_DENSITY = 0.5;

        public BonusXna(PlanesGame game, Bonus bonus, CoordinatesTransformer coordinatesTransformer, Color color, Texture2D texture)
            :base(game, bonus, coordinatesTransformer)
        {
            bonus.Applied += WhenBonusApplied;

            Bonus = bonus;
            Color = color;

            m_texture = texture;

            m_emitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                PositionDeviationRadius = Bonus.RelativeGeometry.BoundingRectangle.LongSide * 2.0 / 3.0,
                VelocityDeviationRadius = 10,
                AlphaVelocityDeviationFactor = 0.3
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {            
            Vector2 origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);
            Vector2 scaleVector = CoordinatesTransformer.CreateScaleVector(Bonus.RelativeGeometry.BoundingRectangle.Size, new Size(m_texture.Width, m_texture.Height));

            // pulsation
            scaleVector = Vector2.Multiply(scaleVector, (float)(1.0 + Math.Sin(gameTime.TotalGameTime.TotalSeconds * 5) / 10.0));

            spriteBatch.Draw(
                m_texture,
                CoordinatesTransformer.Transform(Bonus.Position),
                null,
                Color,
                (float)Bonus.Rotation,
                origin,
                scaleVector,
                SpriteEffects.None,
                LayersDepths.Bonuses);
        }

        public void DrawOnMinimap(GameTime gameTime, SpriteBatch minimapSpriteBatch, CoordinatesTransformer coordinatesTransformer)
        {
            if (m_minimapTexture == null)
                m_minimapTexture = Game.Content.Load<Texture2D>("Minimap/point");

            var origin = new Vector2(m_minimapTexture.Width / 2.0f, m_minimapTexture.Height / 2.0f);

            minimapSpriteBatch.Draw(m_minimapTexture,
                                    coordinatesTransformer.Transform(GameObject.Position),
                                    null,
                                    Color,
                                    (float)Helper.ToRadians(GameObject.Rotation),
                                    origin,
                                    1.0f,
                                    SpriteEffects.None,
                                    1.0f);
        }

        private void WhenBonusApplied(Bonus bonus, NRPlanes.Core.Common.Plane plane)
        {
            if (bonus is HealthBonus)
            {
                BasicSoundEffect effect = Game.GameManager.GameWorldXna.SoundManager.CreateBasicSoundEffect("health_bonus", true);
                effect.Position = plane.Position;
                effect.Play();

                m_emitter.Emit(new Particle(Game, CoordinatesTransformer, ParticleType.Cross)
                {
                    Color = Color.Red,
                    Position = bonus.Position,
                    Size = new Size(2, 2),
                    AlphaVelocity = -0.3f,
                    TimeToLive = TimeSpan.FromSeconds(5),
                    Depth = LayersDepths.BonusesParticles,                    
                }, (int)(Math.Max(1, bonus.RelativeGeometry.BoundingRectangle.Area * PARTICLES_DENSITY)));
            }
        }
    }
}

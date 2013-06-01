using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Common;
using NRPlanes.Client.Sound;
using NRPlanes.Client.Particles;

namespace NRPlanes.Client.GameComponents
{
    public class LaserBulletXna : DrawableGameObject
    {
        public new LaserBullet GameObject
        {
            get { return base.GameObject as LaserBullet; }
        }

        private AsymmetricParticlesEmitter m_particlesEmitter;
        private Texture2D m_texture;

        public LaserBulletXna(PlanesGame game, LaserBullet bullet, CoordinatesTransformer coordinatesTransformer)
            : base(game, bullet, coordinatesTransformer)
        {
            var sound = SoundManager.Instance.CreateBasicSoundEffect("bullet_sound");
            sound.Position = bullet.Position;
            sound.Play();

            m_particlesEmitter = new AsymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                LongitualPositionDeviationRadius = 0,
                TransversePositionDeviationRadius = 0,
                LongitualVelocityDeviationRadius = 0,
                TransverseVelocityDeviationRadius = 0,
                AlphaVelocityDeviationFactor = 0
            };
        }        

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_texture == null)
                m_texture = Game.Content.Load<Texture2D>("Images/circle_bullet");

            if (GameObject.TimeToLive > TimeSpan.Zero)
            {
                m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                {
                    Color = Color.FromNonPremultiplied(20, 20, 20, 255),
                    Position = GameObject.Position,
                    Size = new Size(1, 15),                    
                    AlphaVelocity = -0.01f,
                    TimeToLive = TimeSpan.FromSeconds(5),
                    Rotation = GameObject.Velocity.Angle()
                }, 1);

                var origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(GameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                           new Size(m_texture.Width, m_texture.Height));

                spriteBatch.Draw(m_texture,
                                 CoordinatesTransformer.Transform(GameObject.Position),
                                 null,
                                 Color.White,
                                 MathHelper.ToRadians((float) GameObject.Rotation),
                                 origin,
                                 scaleVector,
                                 SpriteEffects.None,
                                 LayersDepths.Bullet);
            }
        }
    }
}
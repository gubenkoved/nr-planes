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

        private SymmetricParticlesEmitter m_particlesEmitter;
        private Texture2D m_texture;

        public LaserBulletXna(PlanesGame game, LaserBullet bullet, CoordinatesTransformer coordinatesTransformer)
            : base(game, bullet, coordinatesTransformer)
        {
            var sound = SoundManager.Instance.CreateBasicSoundEffect("bullet_sound");
            sound.Position = bullet.Position;
            sound.Play();

            m_particlesEmitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna);
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
                    Size = new Size(1, 10),
                    AlphaVelocity = -1f,
                    TimeToLive = TimeSpan.FromSeconds(1),
                    Rotation = GameObject.Velocity.Angle(),
                    IsStatic = true
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
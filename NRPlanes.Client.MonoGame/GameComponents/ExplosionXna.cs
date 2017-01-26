using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using NRPlanes.Client.Sound;
using NRPlanes.Core.Common;
using NRPlanes.Client.Particles;
using NRPlanes.Core.Bonuses;
using NRPlanes.Core.Bullets;

namespace NRPlanes.Client.GameComponents
{
    public class ExplosionXna : MyDrawableGameComponent
    {        
        private ParticlesEmitterBase m_particlesEmitter;
        private const int PARTICLES_DENSITY = 20;

        //private AnimationSpriteDrawer m_animationSpriteDrawer;
        //private float m_angle;
        private Rect m_explosionPosition;
        private GameObject m_exploded;

        public ExplosionXna(PlanesGame game, GameObject exploded, CoordinatesTransformer coordinatesTransformer)
            : base(game, coordinatesTransformer)
        {
            m_exploded = exploded;
            m_explosionPosition = new Rect(exploded.Position, exploded.RelativeGeometry.BoundingRectangle.Size);

            if (exploded is NRPlanes.Core.Common.Plane || exploded is Bullet)
            {
                m_particlesEmitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna)
                {
                    PositionDeviationRadius = m_explosionPosition.Width / 2,
                    VelocityDeviationRadius = 5
                };
            }
            else if (exploded is Bonus)
            {
                m_particlesEmitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna)
                {
                    PositionDeviationRadius = m_explosionPosition.Width / 3,
                    VelocityDeviationRadius = 20,
                    RotationDeviation = 180,
                    RotationVelocityDeviation = 60
                };
            }

            BasicSoundEffect sound = null;

            if (exploded is NRPlanes.Core.Common.Plane)
                sound = game.GameManager.GameWorldXna.SoundManager.CreateBasicSoundEffect("plane_explosion");
            else if (exploded is Bullet)
                sound = game.GameManager.GameWorldXna.SoundManager.CreateBasicSoundEffect("bullet_explosion");
            else if (exploded is Bonus)
                sound = game.GameManager.GameWorldXna.SoundManager.CreateBasicSoundEffect("debris_explosion");

            if (sound != null)
            {
                sound.Position = m_explosionPosition.Center;
                sound.Play();
            }

            //m_angle = (float) (2 * Math.PI * m_random.NextDouble());
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //var frameSize = new Size(320, 240);

            //if (m_drawCount++ == 0)
            {
                if (m_exploded is Bullet || m_exploded is NRPlanes.Core.Common.Plane)
                {
                    m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                    {
                        Color = Color.OrangeRed,
                        Position = m_explosionPosition.Center,
                        Size = new Size(3, 3),
                        AlphaVelocity = -0.35f,
                        TimeToLive = TimeSpan.FromSeconds(5),
                        Velocity = Vector.Zero,
                        Rotation = 0,
                        Depth = LayersDepths.Explosion,
                        SizeFactorVelocity = new Vector(0.3, 0.3)
                    }, (int)(Math.Max(3, m_explosionPosition.Area * Math.Pow(PARTICLES_DENSITY, 0.7))));

                    m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                    {
                        Color = Color.OrangeRed,
                        Position = m_explosionPosition.Center,
                        Size = new Size(1, 1),
                        AlphaVelocity = -0.9f,
                        TimeToLive = TimeSpan.FromSeconds(5),
                        Velocity = Vector.Zero,
                        Rotation = 0,
                        Depth = LayersDepths.Explosion,
                        SizeFactorVelocity = new Vector(40, 40)
                    }, (int)(Math.Max(1, m_explosionPosition.Area * Math.Pow(PARTICLES_DENSITY, 0.3))));
                }
                else if (m_exploded is Bonus)
                {                    
                    m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer, ParticleType.Debris)
                    {
                        Color = Color.Red,
                        Position = m_explosionPosition.Center,
                        Size = new Size(2, 2),
                        AlphaVelocity = -0.5f,
                        TimeToLive = TimeSpan.FromSeconds(5),
                        Velocity = Vector.Zero,
                        Rotation = 0,
                        Depth = LayersDepths.Explosion,
                        SizeFactorVelocity = new Vector(0.3, 0.3)
                    }, (int)(m_explosionPosition.Area * Math.Pow(PARTICLES_DENSITY, 0.3)));

                    m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                    {
                        Color = Color.OrangeRed,
                        Position = m_explosionPosition.Center,
                        Size = new Size(1, 1),
                        AlphaVelocity = -0.9f,
                        TimeToLive = TimeSpan.FromSeconds(2),
                        Velocity = Vector.Zero,
                        Rotation = 0,
                        Depth = LayersDepths.Explosion,
                        SizeFactorVelocity = new Vector(25, 25)
                    }, (int)(Math.Max(1, m_explosionPosition.Area * Math.Pow(PARTICLES_DENSITY, 0.3))));
                }

                IsGarbage = true;
            }

            //if (_animationSpriteDrawer == null)
            //    _animationSpriteDrawer = new AnimationSpriteDrawer(Game.Content.Load<Texture2D>("Animations/explosion"),
            //                                                       frameSize,
            //                                                       TimeSpan.FromMilliseconds(30),
            //                                                       false);

            //var origin = new Vector2((float)(frameSize.Width / 2.0), (float)(frameSize.Height / 2.0));

            //var scaleVector = CoordinatesTransformer.CreateScaleVector(new Size(_explosionPosition.Size.Height * frameSize.Aspect, _explosionPosition.Size.Height), frameSize);

            //_animationSpriteDrawer.Draw(
            //    gameTime,
            //    spriteBatch,
            //    CoordinatesTransformer.Transform(_explosionPosition.Center),
            //    Color.White,
            //    _angle,
            //    origin,
            //    scaleVector,
            //    SpriteEffects.None,
            //    LayersDepths.Exsplosion);

            //if (_animationSpriteDrawer.CurrentFrame == _animationSpriteDrawer.TotalFrames - 1)
            //    IsGarbage = true;
        }
    }
}
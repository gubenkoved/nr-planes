using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using NRPlanes.Client.Sound;
using NRPlanes.Core.Common;
using NRPlanes.Client.Particles;

namespace NRPlanes.Client.GameComponents
{
    public class ExplosionXna : MyDrawableGameComponent
    {
        private static Random _random = new Random(Environment.TickCount);

        private ParticlesEmitterBase m_particlesEmitter;
        private const int PARTICLES_DENSITY = 20;
        private int m_drawCount;

        private AnimationSpriteDrawer _animationSpriteDrawer;
        private float _angle;
        private Rect _explosionPosition;

        public ExplosionXna(PlanesGame game, GameObject exploded, CoordinatesTransformer coordinatesTransformer)
            : base(game, coordinatesTransformer)
        {
            //double explosionSideSize = Math.Max(10.0, exploded.RelativeGeometry.BoundingRectangle.LongSide * 5.0);            
            _explosionPosition = new Rect(exploded.Position, exploded.RelativeGeometry.BoundingRectangle.Size);            

            m_particlesEmitter = new SymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                PositionDeviationRadius = _explosionPosition.Width / 2,
                VelocityDeviationRadius = 5
            };

            BasicSoundEffect sound = null;

            if (exploded is NRPlanes.Core.Common.Plane)
                sound = SoundManager.Instance.CreateBasicSoundEffect("plane_explosion");
            else if (exploded is Bullet)
                sound = SoundManager.Instance.CreateBasicSoundEffect("bullet_explosion");

            if (sound != null)
            {
                sound.Position = _explosionPosition.Center;
                sound.Play();
            }

            _angle = (float) (2 * Math.PI * _random.NextDouble());
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var frameSize = new Size(320, 240);

            if (m_drawCount++ == 0)
            {
                m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                {
                    Color = Color.OrangeRed,
                    Position = _explosionPosition.Center,
                    Size = new Size(3, 3),
                    AlphaVelocity = -0.5f,
                    TimeToLive = TimeSpan.FromSeconds(5),
                    Velocity = new Vector(),
                    Rotation = 0,
                    Depth = LayersDepths.Explosion,
                    SizeFactorVelocity = new Vector(0.3, 0.3)
                }, (int)(_explosionPosition.Area * Math.Pow(PARTICLES_DENSITY, 0.7)));

                m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                {
                    Color = Color.OrangeRed,
                    Position = _explosionPosition.Center,
                    Size = new Size(1, 1),
                    AlphaVelocity = -0.9f,
                    TimeToLive = TimeSpan.FromSeconds(5),
                    Velocity = new Vector(),
                    Rotation = 0,
                    Depth = LayersDepths.Explosion,
                    SizeFactorVelocity = new Vector(25, 25)
                }, (int)(Math.Max(1, _explosionPosition.Area * Math.Pow(PARTICLES_DENSITY, 0.3))));
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
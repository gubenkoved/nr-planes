using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Engines;
using NRPlanes.Client.Sound;
using NRPlanes.Client.Particles;

namespace NRPlanes.Client.GameComponents
{
    public class RocketEngineXna : DrawableEquipment
    {
        public new RocketEngine Equipment { get { return base.Equipment as RocketEngine; } }

        private AnimationSpriteDrawer m_animationSpriteDrawer;
        private FadeInOutSoundEffect m_workSoundEffect;
        private AsymmetricParticlesEmitter m_particlesEmitter;

        public RocketEngineXna(PlanesGame game, RocketEngine rocketEngine, CoordinatesTransformer coordinatesTransformer)
            : base(game, rocketEngine, coordinatesTransformer)
        {
            m_workSoundEffect = SoundManager.Instance.CreateFadeInOutSoundEffect("engine_work", 
                TimeSpan.FromSeconds(0.4), TimeSpan.FromSeconds(0.1));

            m_particlesEmitter = new AsymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                LongitualPositionDeviationRadius = 2,
                TransversePositionDeviationRadius = 0.4,
                LongitualVelocityDeviationRadius = 0.1,
                TransverseVelocityDeviationRadius = 0.02,
                AlphaVelocityDeviationFactor = 0.3
            };
        }

        public override void Update(GameTime gameTime)
        {
            m_workSoundEffect.Position = Equipment.GetAbsolutePosition();

            if (Equipment.IsActive)
                m_workSoundEffect.Play();

            if (!Equipment.IsActive)
                m_workSoundEffect.Stop();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var frameSize = new Size(24, 24);

            if (m_animationSpriteDrawer == null)
            {
                m_animationSpriteDrawer
                    = new AnimationSpriteDrawer(
                        Game.Content.Load<Texture2D>("Animations/flame_anim"),
                        frameSize, TimeSpan.FromMilliseconds(30));
            }

            if (Equipment.IsActive)
            {
                m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                {
                    Color = Color.OrangeRed,
                    Position = Equipment.GetAbsolutePosition(),
                    Size = new Size(2.5, 3),
                    AlphaVelocity = -0.05f,
                    TimeToLive = TimeSpan.FromSeconds(2),
                    Velocity = new Vector(0, -0.5).Rotate(Equipment.GetAbsoluteRotation()),
                    Rotation = Equipment.GetAbsoluteRotation()
                }, 5);
                
                //var origin = new Vector2((float) m_animationSpriteDrawer.FrameSize.Width / 2.0f,
                //                         (float) m_animationSpriteDrawer.FrameSize.Height / 2.0f);

                //var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.Size, frameSize);

                //m_animationSpriteDrawer.Draw(
                //    gameTime,
                //    spriteBatch,
                //    CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                //    Color.White,
                //    MathHelper.ToRadians((float) Equipment.GetAbsoluteRotation()),
                //    origin,
                //    scaleVector,
                //    SpriteEffects.None,
                //    LayersDepths.Engine);
            }
        }
    }
}
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
    public class IonEngineXna : DrawableEquipment
    {
        public IonEngine Equipment { get { return base.Equipment as IonEngine; } }

        private AnimationSpriteDrawer m_animationSpriteDrawer;
        private FadeInOutSoundEffect m_workSoundEffect;
        private AsymmetricParticlesEmitter m_particlesEmitter;

        public IonEngineXna(PlanesGame game, IonEngine ionEngine, CoordinatesTransformer coordinatesTransformer)
            : base(game, ionEngine, coordinatesTransformer)
        {
            m_workSoundEffect = SoundManager.Instance.CreateFadeInOutSoundEffect("deflecting_engine_work",
                TimeSpan.FromSeconds(0.4), TimeSpan.FromSeconds(0.1));

            m_particlesEmitter = new AsymmetricParticlesEmitter(game.GameManager.GameWorldXna)
            {
                LongitualPositionDeviationRadius = 1,
                TransversePositionDeviationRadius = 0.3,
                LongitualVelocityDeviationRadius = 5,
                TransverseVelocityDeviationRadius = 1,
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

            //if (m_animationSpriteDrawer == null)
            //    m_animationSpriteDrawer
            //        = new AnimationSpriteDrawer(
            //            Game.Content.Load<Texture2D>("Animations/flame_anim_2"),
            //            frameSize, TimeSpan.FromMilliseconds(30));

            if (Equipment.IsActive)
            {
                m_particlesEmitter.LongitualDirection = new Vector(0, 1).Rotate(Equipment.GetAbsoluteRotation());

                m_particlesEmitter.Emit(new Particle(Game, CoordinatesTransformer)
                {
                    Color = Color.LightBlue,
                    Position = Equipment.GetAbsolutePosition(),
                    Size = new Size(1.5, 1.5),
                    AlphaVelocity = -5.0f,
                    TimeToLive = TimeSpan.FromSeconds(1),
                    Velocity = Equipment.RelatedGameObject.Velocity + new Vector(0, -20).Rotate(Equipment.GetAbsoluteRotation()),
                    Rotation = Equipment.GetAbsoluteRotation()
                }, 5);

                //var origin = new Vector2((float)m_animationSpriteDrawer.FrameSize.Width / 2.0f,
                //                         (float)m_animationSpriteDrawer.FrameSize.Height / 2.0f);

                //var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.Size, frameSize);

                //m_animationSpriteDrawer.Draw(gameTime,
                //                            spriteBatch,
                //                            CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                //                            Color.White,
                //                            MathHelper.ToRadians((float) Equipment.GetAbsoluteRotation()),
                //                            origin,
                //                            scaleVector,
                //                            SpriteEffects.None,
                //                            LayersDepths.Engine);
            }
        }
    }
}
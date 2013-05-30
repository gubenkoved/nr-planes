using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Engines;
using NRPlanes.Client.Sound;

namespace NRPlanes.Client.GameComponents
{
    public class RocketEngineXna : DrawableEquipment
    {
        public new RocketEngine Equipment { get { return base.Equipment as RocketEngine; } }

        private AnimationSpriteDrawer _animationSpriteDrawer;

        private FadeInOutSoundEffect _workSoundEffect;

        public RocketEngineXna(PlanesGame game, RocketEngine rocketEngine, CoordinatesTransformer coordinatesTransformer)
            : base(game, rocketEngine, coordinatesTransformer)
        {
            _workSoundEffect = SoundManager.Instance.CreateFadeInOutSoundEffect("engine_work", 
                TimeSpan.FromSeconds(0.4), TimeSpan.FromSeconds(0.1));
        }

        public override void Update(GameTime gameTime)
        {
            _workSoundEffect.Position = Equipment.GetAbsolutePosition();

            if (Equipment.IsActive)
                _workSoundEffect.Play();

            if (!Equipment.IsActive)
                _workSoundEffect.Stop();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var frameSize = new Size(24, 24);

            if (_animationSpriteDrawer == null)
            {
                _animationSpriteDrawer
                    = new AnimationSpriteDrawer(
                        Game.Content.Load<Texture2D>("Animations/flame_anim"),
                        frameSize, TimeSpan.FromMilliseconds(30));
            }

            if (Equipment.IsActive)
            {
                var origin = new Vector2((float) _animationSpriteDrawer.FrameSize.Width / 2.0f,
                                         (float) _animationSpriteDrawer.FrameSize.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.Size, frameSize);

                _animationSpriteDrawer.Draw(
                    gameTime,
                    spriteBatch,
                    CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                    Color.White,
                    MathHelper.ToRadians((float) Equipment.GetAbsoluteRotation()),
                    origin,
                    scaleVector,
                    SpriteEffects.None,
                    LayersDepths.Engine);
            }
        }
    }
}
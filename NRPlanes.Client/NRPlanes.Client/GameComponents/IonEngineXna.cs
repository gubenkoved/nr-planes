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
    public class IonEngineXna : DrawableEquipment
    {
        public IonEngine Equipment { get { return base.Equipment as IonEngine; } }

        private AnimationSpriteDrawer _animationSpriteDrawer;

        private FadeInOutSoundEffect _workSoundEffect;

        public IonEngineXna(PlanesGame game, IonEngine ionEngine, CoordinatesTransformer coordinatesTransformer)
            : base(game, ionEngine, coordinatesTransformer)
        {
            _workSoundEffect = SoundManager.Instance.CreateFadeInOutSoundEffect("deflecting_engine_work",
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
                _animationSpriteDrawer
                    = new AnimationSpriteDrawer(
                        Game.Content.Load<Texture2D>("Animations/flame_anim_2"),
                        frameSize, TimeSpan.FromMilliseconds(30));

            if (Equipment.IsActive)
            {
                var origin = new Vector2((float)_animationSpriteDrawer.FrameSize.Width / 2.0f,
                                         (float)_animationSpriteDrawer.FrameSize.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.Size, frameSize);

                _animationSpriteDrawer.Draw(gameTime,
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
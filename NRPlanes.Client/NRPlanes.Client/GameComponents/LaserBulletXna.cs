using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Bullets;
using NRPlanes.Core.Common;
using NRPlanes.Client.Sound;

namespace NRPlanes.Client.GameComponents
{
    public class LaserBulletXna : DrawableGameObject
    {
        public new LaserBullet GameObject
        {
            get { return base.GameObject as LaserBullet; }
        }

        private Texture2D _texture;

        public LaserBulletXna(PlanesGame game, LaserBullet bullet, CoordinatesTransformer coordinatesTransformer)
            : base(game, bullet, coordinatesTransformer)
        {
            var sound = SoundManager.Instance.CreateBasicSoundEffect("bullet_sound");
            sound.Position = bullet.Position;
            sound.Play();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
                _texture = Game.Content.Load<Texture2D>("Images/bullet");

            if (GameObject.TimeToLive > TimeSpan.Zero)
            {
                var origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(GameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                           new Size(_texture.Width, _texture.Height));

                spriteBatch.Draw(_texture,
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
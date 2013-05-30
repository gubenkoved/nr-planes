using System;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Weapons;

namespace NRPlanes.Client.GameComponents
{
    public class LaserGunXna : DrawableEquipment
    {
        public LaserGun Equipment
        {
            get { return base.Equipment as LaserGun; }
        }
        
        private Texture2D _texture;

        public LaserGunXna(PlanesGame game, LaserGun weapon, CoordinatesTransformer coordinatesTransformer)
            : base(game, weapon, coordinatesTransformer)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
                _texture = Game.Content.Load<Texture2D>("Images/weapon");

            var origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);

            var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.Size,
                                                                       new Size(_texture.Width, _texture.Height));

            spriteBatch.Draw(_texture,
                             CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                             null,
                             Color.White,
                             MathHelper.ToRadians((float) Equipment.GetAbsoluteRotation()),
                             origin,
                             scaleVector,
                             SpriteEffects.None,
                             LayersDepths.Weapon);
        }
    }
}
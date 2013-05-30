using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRPlanes.Client.Common;
using NRPlanes.Core.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NRPlanes.Core.Planes;
using NRPlanes.Core.Primitives;

namespace NRPlanes.Client.GameComponents
{
    public class ShieldXna : DrawableEquipment
    {
        public new Shield Equipment { get { return base.Equipment as Shield; } }

        private Texture2D _texture;
        private Color _color;

        public ShieldXna(PlanesGame game, Shield shield, CoordinatesTransformer coordinatesTransformer)
            : base(game, shield, coordinatesTransformer)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                if (Equipment.RelatedGameObject is XWingPlane)
                {
                    _texture = Game.Content.Load<Texture2D>("Images/x_wing_shield");
                    _color = Color.Red;
                }
                else
                    throw new Exception("Unknown plane to create shield");
            }

            if (Equipment.IsActive)
            {
                var origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.RelatedGameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                           new Size(_texture.Width, _texture.Height));

                spriteBatch.Draw(_texture,
                                 CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                                 null,
                                 _color,
                                 MathHelper.ToRadians((float)Equipment.GetAbsoluteRotation()),
                                 origin,
                                 scaleVector,
                                 SpriteEffects.None,
                                 LayersDepths.Shield);
            }
        }
    }
}

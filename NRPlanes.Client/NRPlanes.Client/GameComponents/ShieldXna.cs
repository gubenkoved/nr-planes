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
using NRPlanes.Core.Equipments;

namespace NRPlanes.Client.GameComponents
{
    public class ShieldXna : DrawableEquipment
    {
        public new Shield Equipment { get { return base.Equipment as Shield; } }

        private Texture2D m_texture;
        private Color m_color;
        private int m_drawCounter;

        public ShieldXna(PlanesGame game, Shield shield, CoordinatesTransformer coordinatesTransformer)
            : base(game, shield, coordinatesTransformer)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_texture == null)
            {
                if (Equipment.RelatedGameObject is XWingPlane)
                {
                    m_texture = Game.Content.Load<Texture2D>("Images/x_wing_shield");
                    m_color = Color.YellowGreen;
                }
                else
                    throw new Exception("Unknown plane to create shield");
            }

            if (Equipment.IsActive)
            {
                ++m_drawCounter;

                var origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);

                var scaleVector = CoordinatesTransformer.CreateScaleVector(Equipment.RelatedGameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                           new Size(m_texture.Width, m_texture.Height));

                spriteBatch.Draw(m_texture,
                                 CoordinatesTransformer.Transform(Equipment.GetAbsolutePosition()),
                                 null,
                                 Color.FromNonPremultiplied(m_color.R, m_color.G, m_color.B, (int)(195 + 60 * Math.Sin(m_drawCounter / 3.0))),
                                 MathHelper.ToRadians((float)Equipment.GetAbsoluteRotation()),
                                 origin,
                                 scaleVector,
                                 SpriteEffects.None,
                                 LayersDepths.Shield);
            }
        }
    }
}

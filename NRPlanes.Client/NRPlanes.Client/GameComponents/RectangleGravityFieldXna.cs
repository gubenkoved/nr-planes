using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Common;
using NRPlanes.Core.StaticObjects;

namespace NRPlanes.Client.GameComponents
{
    public class RectangleGravityFieldXna : DrawableStaticObject
    {
        public new RectangleGravityField StaticObject
        {
            get { return base.StaticObject as RectangleGravityField; }
        }

        private Texture2D m_texture;
        private Color m_color = Color.FromNonPremultiplied(5, 20, 50, 255);

        public RectangleGravityFieldXna(PlanesGame game, RectangleGravityField gravityField, CoordinatesTransformer coordinatesTransformer)
            : base(game, gravityField, coordinatesTransformer)
        {
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_texture == null)
                m_texture = Game.Content.Load<Texture2D>("Images/gravity_field");

            var origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);

            var angle = StaticObject.ForceDirection.Angle();

            var scaleVector =
                CoordinatesTransformer.CreateScaleVector(
                    new Size(StaticObject.AbsoluteGeometry.BoundingRectangle.LongSide,
                             StaticObject.AbsoluteGeometry.BoundingRectangle.ShortSide),
                    new Size(m_texture.Width, m_texture.Height));

            spriteBatch.Draw(m_texture,
                             CoordinatesTransformer.Transform(StaticObject.AbsoluteGeometry.Center),
                             null,
                             m_color,
                             (float) Helper.ToRadians(angle),
                             origin,
                             scaleVector,
                             SpriteEffects.None,
                             LayersDepths.GravityField);
        }
    }
}
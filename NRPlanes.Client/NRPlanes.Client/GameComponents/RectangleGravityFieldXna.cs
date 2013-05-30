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

        private Texture2D _texture;

        public RectangleGravityFieldXna(PlanesGame game, RectangleGravityField gravityField, CoordinatesTransformer coordinatesTransformer)
            : base(game, gravityField, coordinatesTransformer)
        {
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
                _texture = Game.Content.Load<Texture2D>("Images/gravity_field");

            var origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);

            var angle = StaticObject.ForceDirection.Angle();

            var scaleVector =
                CoordinatesTransformer.CreateScaleVector(
                    new Size(StaticObject.AbsoluteGeometry.BoundingRectangle.LongSide,
                             StaticObject.AbsoluteGeometry.BoundingRectangle.ShortSide),
                    new Size(_texture.Width, _texture.Height));


            spriteBatch.Draw(_texture,
                             CoordinatesTransformer.Transform(StaticObject.AbsoluteGeometry.Center),
                             null,
                             Color.CornflowerBlue,
                             (float) Helper.ToRadians(angle),
                             origin,
                             scaleVector,
                             SpriteEffects.None,
                             LayersDepths.GravityField);
        }
    }
}
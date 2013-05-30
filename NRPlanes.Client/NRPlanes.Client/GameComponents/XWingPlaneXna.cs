using NRPlanes.Client.InfoPanels;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NRPlanes.Client.Common;
using NRPlanes.Core.Common;
using NRPlanes.Core.Planes;

namespace NRPlanes.Client.GameComponents
{
    public class XWingPlaneXna : DrawablePlane
    {
        public new XWingPlane GameObject { get { return base.GameObject as XWingPlane; } }

        private Texture2D _texture;

        public XWingPlaneXna(PlanesGame game, XWingPlane xWingPlane, CoordinatesTransformer coordinatesTransformer)
            : base(game, xWingPlane, coordinatesTransformer)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture == null)
                _texture = Game.Content.Load<Texture2D>("Images/x_wing");

            var origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);

            var scaleVector = CoordinatesTransformer.CreateScaleVector(GameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                       new Size(_texture.Width, _texture.Height));

            spriteBatch.Draw(_texture,
                             CoordinatesTransformer.Transform(GameObject.Position),
                             null,
                             Color.White,
                             MathHelper.ToRadians((float)GameObject.Rotation),
                             origin,
                             scaleVector,
                             SpriteEffects.None,
                             LayersDepths.Plane);

        }
    }
}
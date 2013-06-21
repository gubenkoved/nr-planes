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

        private Texture2D m_texture;

        public XWingPlaneXna(PlanesGame game, XWingPlane xWingPlane, CoordinatesTransformer coordinatesTransformer)
            : base(game, xWingPlane, coordinatesTransformer)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (m_texture == null)
                m_texture = Game.Content.Load<Texture2D>("Images/x_wing");

            Vector2 origin = new Vector2(m_texture.Width / 2.0f, m_texture.Height / 2.0f);
            Vector2 scaleVector = CoordinatesTransformer.CreateScaleVector(GameObject.RelativeGeometry.BoundingRectangle.Size,
                                                                       new Size(m_texture.Width, m_texture.Height));
            
            spriteBatch.Draw(m_texture,
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
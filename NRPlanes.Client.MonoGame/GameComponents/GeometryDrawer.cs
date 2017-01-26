using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using System.Linq;

namespace NRPlanes.Client.GameComponents
{
    public class GeometryDrawer
    {
        private Texture2D m_line;
        private Texture2D m_point;
        private SpriteFont m_font;

        public GeometryDrawer(Texture2D line, Texture2D point, SpriteFont font)
        {
            m_line = line;
            m_point = point;
            m_font = font;
        }

        public void Draw(SpriteBatch spriteBatch, CoordinatesTransformer coordinatesTransformer, Geometry geometry, bool drawCenter = true)
        {
            if (drawCenter)
            {
                spriteBatch.DrawString(
                    m_font,
                    geometry.Center.ToString(),
                    coordinatesTransformer.Transform(geometry.Center).Round(),
                    Color.Yellow);

                spriteBatch.Draw(
                    m_point,
                    coordinatesTransformer.Transform(geometry.Center),
                    null,
                    Color.White,
                    0,
                    new Vector2(m_point.Width / 2.0f, m_point.Height / 2.0f),
                    1.0f,
                    SpriteEffects.None,
                    0f);
            }

            foreach (var segment in PolygonGeometry.FromRectangle(geometry.BoundingRectangle).Segments)
            {
                Vector centerSegment = (segment.Start + segment.End) / 2.0;
                double rotation = segment.Offset.Angle();
                Vector2 scale = coordinatesTransformer.CreateScaleVector(new Size(1.0, segment.Length), new Size(m_line.Width, m_line.Height));
                Vector2 origin = new Vector2(m_line.Width / 2.0f, m_line.Height / 2.0f);

                spriteBatch.Draw(
                    m_line,
                    coordinatesTransformer.Transform(centerSegment),
                    null,
                    Color.White * 0.8f,
                    (float)Helper.ToRadians(rotation),
                    origin,
                    new Vector2(1, scale.Y),
                    SpriteEffects.None,
                    0);
            }

            if (geometry is PolygonGeometry)
            {
                var polygon = geometry as PolygonGeometry;

                foreach (var segment in polygon.Segments)
                {
                    Vector centerSegment = (segment.Start + segment.End) / 2.0;
                    double rotation = segment.Offset.Angle();
                    Vector2 scale = coordinatesTransformer.CreateScaleVector(new Size(1.0, segment.Length), new Size(m_line.Width, m_line.Height));
                    Vector2 origin = new Vector2(m_line.Width / 2.0f, m_line.Height / 2.0f);

                    spriteBatch.Draw(
                        m_line,
                        coordinatesTransformer.Transform(centerSegment),
                        null,
                        Color.White * 0.8f,
                        (float)Helper.ToRadians(rotation),
                        origin,
                        new Vector2(1, scale.Y),
                        SpriteEffects.None,
                        0);
                }

            }
        }
    }
}
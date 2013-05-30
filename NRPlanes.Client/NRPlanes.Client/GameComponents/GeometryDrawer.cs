using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using System.Linq;

namespace NRPlanes.Client.GameComponents
{
    public class GeometryDrawer
    {
        private Texture2D _line;
        private Texture2D _point;
        private SpriteFont _font;

        public GeometryDrawer(Texture2D line, Texture2D point, SpriteFont font)
        {
            _line = line;
            _point = point;
            _font = font;
        } 

        public void Draw(SpriteBatch spriteBatch, CoordinatesTransformer coordinatesTransformer, Geometry geometry)
        {
            if (geometry is PolygonGeometry)
            {
                var polygon = geometry as PolygonGeometry;

                spriteBatch.DrawString(
                    _font,
                    geometry.Center.ToString(),
                    coordinatesTransformer.Transform(geometry.Center).Round(),
                    Color.Yellow);

                spriteBatch.Draw(
                    _point,
                    coordinatesTransformer.Transform(geometry.Center),
                    null,
                    Color.White,
                    0,
                    new Vector2(_point.Width / 2.0f, _point.Height / 2.0f),
                    1.0f,
                    SpriteEffects.None,
                    0f);
                
                var boundingRectanglePolygon = PolygonGeometry.FromRectangle(geometry.BoundingRectangle);

                foreach (var segment in polygon.Segments.Union(boundingRectanglePolygon.Segments))
                {
                    var centerSegment = (segment.Start + segment.End) / 2.0;

                    var rotation = segment.Offset.Angle();

                    var scale = coordinatesTransformer.CreateScaleVector(new Size(1.0, segment.Length), new Size(1.0, _line.Height));

                    var origin = new Vector2(_line.Width / 2.0f, _line.Height / 2.0f);

                    spriteBatch.Draw(
                        _line,
                        coordinatesTransformer.Transform(centerSegment),
                        null,
                        Color.White,
                        (float) Helper.ToRadians(rotation),
                        origin,
                        new Vector2(1, scale.Y),
                        SpriteEffects.None,
                        0);
                }

            }
        }
    }
}
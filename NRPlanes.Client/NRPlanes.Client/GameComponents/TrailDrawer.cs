using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NRPlanes.Client.Common;
using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework.Graphics;

namespace NRPlanes.Client.GameComponents
{
    public class TrailDrawer
    {
        public CoordinatesTransformer CoordinatesTransformer;
        public Color Color;
        public double SegmentLen;
        public int SegmentsCount;
        public float MaxAlpha;
        public float MinAlpha;
        public double Width;

        public List<Vector> PathPoints;

        private Texture2D m_pixelTexture;

        public TrailDrawer(Texture2D pixelTexture, CoordinatesTransformer coordinatesTransformer, Color color, double width, float maxAlpha, float minAlpha, int segmentsCount, double segmentLen)
        {
            PathPoints = new List<Vector>();

            CoordinatesTransformer = coordinatesTransformer;
            Color = color;
            Width = width;
            MaxAlpha = maxAlpha;
            MinAlpha = minAlpha;
            SegmentsCount = segmentsCount;
            SegmentLen = segmentLen;

            m_pixelTexture = pixelTexture;
        }

        public void DrawTrail(SpriteBatch spriteBatch)
        {
            if (PathPoints.Count < 2)
                return;

            List<Segment> trailSegments = new List<Segment>();
            Vector segmentStartPoint = PathPoints[PathPoints.Count - 1];

            for (int i = PathPoints.Count - 2; i >= 0; i--)
            {
                Vector currentPoint = PathPoints[i];
                double currentSegmentLen = (currentPoint - segmentStartPoint).Length;

                // draw to first point in case of shortage key points even when segment is short
                if (currentSegmentLen > SegmentLen
                    || (i == 0 && trailSegments.Count < SegmentsCount))
                {
                    trailSegments.Add(new Segment(segmentStartPoint, currentPoint));

                    segmentStartPoint = currentPoint;
                }

                if (trailSegments.Count == SegmentsCount)
                {
                    // rest part can be deleted to reduce memory consumptions
                    PathPoints.RemoveRange(0, i);

                    break;
                }
            }

            // draw trail segments
            float deltaAlpha = (MaxAlpha - MinAlpha) / (SegmentsCount - 1);
            float curAlpha = MaxAlpha;

            foreach (var trailSegment in trailSegments)
            {
                Vector centerSegment = (trailSegment.Start + trailSegment.End) / 2.0;
                double rotation = trailSegment.Offset.Angle();
                Vector2 scale = CoordinatesTransformer.CreateScaleVector(new Size(Width, trailSegment.Length), new Size(m_pixelTexture.Width, m_pixelTexture.Height));
                Vector2 origin = new Vector2(m_pixelTexture.Width / 2.0f, m_pixelTexture.Height / 2.0f);

                spriteBatch.Draw(
                    m_pixelTexture,
                    CoordinatesTransformer.Transform(centerSegment),
                    null,
                    Color.FromNonPremultiplied(255, 255, 255, (int)(curAlpha * 255)),
                    (float) Helper.ToRadians(rotation),
                    origin,
                    scale,
                    SpriteEffects.None,
                    0);

                curAlpha -= deltaAlpha;
            }
        }
    }
}

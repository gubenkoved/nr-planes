using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using System;

namespace NRPlanes.Client.Common
{
    public class CoordinatesTransformer
    {
        private readonly Size m_fullLogicalSize;
        public Size FullLogicalSize
        {
            get
            {
                return m_fullLogicalSize;
            }
        }
        
        private readonly Rectangle m_physicalRectangle;
        public Rectangle PhysicalRectangle 
        { 
            get 
            {
                return m_physicalRectangle;
            }
        }

        private Rect m_visibleLogicalRectangle;
        public Rect VisibleLogicalRectangle
        {
            get { return m_visibleLogicalRectangle; }
        }

        private readonly double m_aspect;
        public double Aspect
        {
            get
            {
                return m_aspect;
            }
        }

        private readonly Size m_normalSize;
        /// <summary>
        /// Visible logical rectangle size then scale factor equals 1
        /// </summary>
        public Size NormalSize
        {
            get
            {
                return m_normalSize;
            }
        }

        private double m_scale;
        public double Scale
        {
            get { return m_scale; }
            set
            {
                m_scale = value;

                ScaleFrom(NormalSize, m_scale);
            }
        }

        public CoordinatesTransformer(Size fullLogicalSize, Rectangle physicalRectangle)
            :this(fullLogicalSize, physicalRectangle, fullLogicalSize.Width)
        {
        }

        public CoordinatesTransformer(Size fullLogicalSize, Rectangle physicalRectangle, double normalLogicalWidth)
        {
            m_aspect = physicalRectangle.GetSize().Aspect;
            m_fullLogicalSize = fullLogicalSize;
            m_physicalRectangle = physicalRectangle;
            m_visibleLogicalRectangle = new Rect(
                new Size(MaximalVisibleLogicalSize().Width, 
                    MaximalVisibleLogicalSize().Height));
            m_normalSize = new Size(normalLogicalWidth, normalLogicalWidth / m_aspect);

            Scale = 1.0;
        }

        private void ScaleFrom(Size initSize, double scale)
        {
            var newVisibleWidth = initSize.Width / scale;
            var newVisibleHeight = initSize.Height / scale;

            m_visibleLogicalRectangle.X -= (newVisibleWidth - m_visibleLogicalRectangle.Width) / 2.0;
            m_visibleLogicalRectangle.Y -= (newVisibleHeight - m_visibleLogicalRectangle.Height) / 2.0;

            m_visibleLogicalRectangle.Width = newVisibleWidth;
            m_visibleLogicalRectangle.Height = newVisibleHeight;
        }
        private Size MaximalVisibleLogicalSize()
        {
            double logicalAspect = FullLogicalSize.Aspect;
            double physicalAspect = PhysicalRectangle.GetSize().Aspect;

            double visibleHeight;
            double visibleWidth;

            if (logicalAspect > physicalAspect) // logical wider than physical
            {
                visibleHeight = FullLogicalSize.Height;
                visibleWidth = visibleHeight * physicalAspect;
            }
            else // physical wider than logical
            {
                visibleWidth = FullLogicalSize.Width;
                visibleHeight = visibleWidth / physicalAspect;
            }

            return new Size(visibleWidth, visibleHeight);
        }

        public void ScaleToFit()
        {
            double minScale = Math.Min(NormalSize.Width / FullLogicalSize.Width,
                                       NormalSize.Height / FullLogicalSize.Height);

            Scale = minScale;
        }
        public void SetCenterOfView(Vector center, bool applyConstraints = true)
        {
            m_visibleLogicalRectangle.X = center.X - VisibleLogicalRectangle.Width / 2.0;

            m_visibleLogicalRectangle.Y = center.Y - VisibleLogicalRectangle.Height / 2.0;

            if (applyConstraints)
            {
                m_visibleLogicalRectangle.X = Math.Max(0, m_visibleLogicalRectangle.X);
                m_visibleLogicalRectangle.X = Math.Min(FullLogicalSize.Width - m_visibleLogicalRectangle.Width, m_visibleLogicalRectangle.X);

                m_visibleLogicalRectangle.Y = Math.Max(0, m_visibleLogicalRectangle.Y);
                m_visibleLogicalRectangle.Y = Math.Min(FullLogicalSize.Height - m_visibleLogicalRectangle.Height, m_visibleLogicalRectangle.Y);
            }
        }
        public Vector2 Transform(Vector vector)
        {
            double multX = PhysicalRectangle.GetSize().Width / m_visibleLogicalRectangle.Width;
            double multY = PhysicalRectangle.GetSize().Height / m_visibleLogicalRectangle.Height;

            var transformed = new Vector2((float)(PhysicalRectangle.X + multX * (vector.X - m_visibleLogicalRectangle.Left)),
                                          (float)(PhysicalRectangle.Y + PhysicalRectangle.GetSize().Height - multY * (vector.Y - m_visibleLogicalRectangle.Bottom)));

            return transformed;            
        }
        public Rectangle Transform(Rect rect)
        {
            var leftTopTransormed = Transform(rect.TopLeft);
            var rightBottomTransormed = Transform(rect.BottomRight);

            var transformed = new Rectangle((int)leftTopTransormed.X,
                                            (int)leftTopTransormed.Y,
                                            (int)(rightBottomTransormed.X - leftTopTransormed.X),
                                            (int)(rightBottomTransormed.Y - leftTopTransormed.Y));

            return transformed;
        }
        public Vector2 CreateScaleVector(Size logicalSize, Size physicalSize)
        {
            var needPhysicalWidth = logicalSize.Width * PhysicalRectangle.GetSize().Width / m_visibleLogicalRectangle.Width;

            var needPhysicalHeight = logicalSize.Height * PhysicalRectangle.GetSize().Height / m_visibleLogicalRectangle.Height;

            var scaleVector = new Vector(needPhysicalWidth / physicalSize.Width,
                                         needPhysicalHeight / physicalSize.Height);

            return scaleVector.ToVector2();
        }
    }
}
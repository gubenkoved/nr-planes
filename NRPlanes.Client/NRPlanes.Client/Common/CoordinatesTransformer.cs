using NRPlanes.Core.Primitives;
using Microsoft.Xna.Framework;
using System;

namespace NRPlanes.Client.Common
{
    public class CoordinatesTransformer
    {
        public Size FullLogicalSize { get; private set; }
        
        public Rectangle PhysicalRectangle { get; private set; }

        private Rect _visibleLogicalRectangle;
        public Rect VisibleLogicalRectangle
        {
            get { return _visibleLogicalRectangle; }
        }

        /// <summary>
        /// Visible logical rectangle size then scale factor equals 1
        /// </summary>
        public Size NormalSize { get; private set; }

        private double _scale;
        public double Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;

                ScaleFrom(NormalSize, _scale);
            }
        }

        public CoordinatesTransformer(Size fullLogicalSize, Rectangle physicalRectangle)
            :this(fullLogicalSize, physicalRectangle, fullLogicalSize.Width)
        {
        }

        public CoordinatesTransformer(Size fullLogicalSize, Rectangle physicalRectangle, double normalWidth)
        {
            FullLogicalSize = fullLogicalSize;

            PhysicalRectangle = physicalRectangle;

            _visibleLogicalRectangle = new Rect(new Size(MaximalVisibleLogicalSize().Width,
                                                        MaximalVisibleLogicalSize().Height));

            SetNormalSizeFromNormalWidth(normalWidth);

            Scale = 1.0;
        }

        private void ScaleFrom(Size initSize, double scale)
        {
            var newVisibleWidth = initSize.Width / scale;
            var newVisibleHeight = initSize.Height / scale;

            _visibleLogicalRectangle.X -= (newVisibleWidth - _visibleLogicalRectangle.Width) / 2.0;
            _visibleLogicalRectangle.Y -= (newVisibleHeight - _visibleLogicalRectangle.Height) / 2.0;

            _visibleLogicalRectangle.Width = newVisibleWidth;
            _visibleLogicalRectangle.Height = newVisibleHeight;
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

        public void SetNormalSizeFromNormalWidth(double normalLogicalWidth)
        {
            NormalSize = new Size(normalLogicalWidth, normalLogicalWidth / PhysicalRectangle.GetSize().Aspect);
        }

        public void ScaleToFit()
        {
            double minScale = Math.Min(NormalSize.Width / FullLogicalSize.Width,
                                       NormalSize.Height / FullLogicalSize.Height);

            Scale = minScale;
        }

        public void SetCenterOfView(Vector center, bool applyConstraints = true)
        {
            _visibleLogicalRectangle.X = center.X - VisibleLogicalRectangle.Width / 2.0;

            _visibleLogicalRectangle.Y = center.Y - VisibleLogicalRectangle.Height / 2.0;

            if (applyConstraints)
            {
                _visibleLogicalRectangle.X = Math.Max(0, _visibleLogicalRectangle.X);
                _visibleLogicalRectangle.X = Math.Min(FullLogicalSize.Width - _visibleLogicalRectangle.Width, _visibleLogicalRectangle.X);

                _visibleLogicalRectangle.Y = Math.Max(0, _visibleLogicalRectangle.Y);
                _visibleLogicalRectangle.Y = Math.Min(FullLogicalSize.Height - _visibleLogicalRectangle.Height, _visibleLogicalRectangle.Y);
            }
        }

        public Vector2 Transform(Vector vector)
        {
            double multX = PhysicalRectangle.GetSize().Width / _visibleLogicalRectangle.Width;
            double multY = PhysicalRectangle.GetSize().Height / _visibleLogicalRectangle.Height;

            var transformed = new Vector2((float)(PhysicalRectangle.X + multX * (vector.X - _visibleLogicalRectangle.Left)),
                                          (float)(PhysicalRectangle.Y + PhysicalRectangle.GetSize().Height - multY * (vector.Y - _visibleLogicalRectangle.Bottom)));

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
            var needPhysicalWidth = logicalSize.Width * PhysicalRectangle.GetSize().Width / _visibleLogicalRectangle.Width;

            var needPhysicalHeight = logicalSize.Height * PhysicalRectangle.GetSize().Height / _visibleLogicalRectangle.Height;

            var scaleVector = new Vector(needPhysicalWidth / physicalSize.Width,
                                         needPhysicalHeight / physicalSize.Height);

            return scaleVector.ToVector2();
        }
    }
}
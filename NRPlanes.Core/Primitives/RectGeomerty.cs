using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.Core.Primitives
{
    /// <summary>
    /// This geometry created only for speed up.
    /// <para>Disclaimer! This geometry CAN NOT BE ROTATED</para>
    /// </summary>
    [DataContract]
    public class RectGeometry : Geometry
    {
        [DataMember]
        private Rect m_rect;
        public Rect Rectangle
        {
            get
            {
                return m_rect;
            }
        }

        public RectGeometry(Rect rect)
        {
            m_rect = rect;
        }

        public override Rect BoundingRectangle
        {
            get 
            {
                return m_rect;
            }
        }

        public override Vector Center
        {
            get 
            {
                return m_rect.Center;
            }
        }

        public override void Translate(Vector translateVector)
        {
            m_rect = new Rect(m_rect.Center + translateVector, m_rect.Size);            
        }

        public override void Rotate(double rotation)
        {
            throw new InvalidOperationException("This geometry can not be rotated. Use polygon geometry instead.");
        }

        public override Geometry Clone()
        {
            return (Geometry)MemberwiseClone();
        }
        public override bool HitTest(Vector vector)
        {
            return m_rect.HitTest(vector);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NRPlanes.ServerData.EventsLog
{
    [DataContract]
    public class Timestamp : IComparable<Timestamp>
    {
        private static object m_sync = new object();
        private static DateTime m_lastDateTime;
        private static int m_lastTriesCount;

        [DataMember]
        private DateTime m_dateTime;
        public DateTime DateTime
        {
            get
            {
                return m_dateTime;
            }
        }

        [DataMember]
        private int m_triesCount;
        public int TriesCount
        {
            get
            {
                return m_triesCount;
            }
        }

        private Timestamp(DateTime dateTime, int triesCount)
        {
            m_dateTime = dateTime;
            m_triesCount = triesCount;
        }

        /// <summary>
        /// Returns unique timestamp
        /// </summary>
        /// <returns></returns>
        public static Timestamp Create()
        {
            lock (m_sync)
            {
                DateTime dateTime = DateTime.Now;

                if (dateTime != m_lastDateTime)
                    m_lastTriesCount = 0;

                m_lastDateTime = dateTime;

                return new Timestamp(dateTime, m_lastTriesCount++);                
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Timestamp)
            {
                return m_dateTime == ((Timestamp)obj).m_dateTime
                    && m_triesCount == ((Timestamp) obj).m_triesCount;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return m_dateTime.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0:yyyyMMddHHmmssffff}_{1}", m_dateTime, m_triesCount);
        }

        public int CompareTo(Timestamp other)
        {
            int cmpRes = m_dateTime.CompareTo(other.m_dateTime);

            if (cmpRes == 0)
                return m_triesCount.CompareTo(other.m_triesCount);
            else
                return cmpRes;
        }

        static public bool operator >(Timestamp t1, Timestamp t2)
        {
            int cmpRes = t1.m_dateTime.CompareTo(t2.m_dateTime);

            if (cmpRes == 0)
                return t1.m_triesCount.CompareTo(t2.m_triesCount) > 0;
            else
                return cmpRes > 0;
        }

        static public bool operator <(Timestamp t1, Timestamp t2)
        {
            int cmpRes = t1.m_dateTime.CompareTo(t2.m_dateTime);

            if (cmpRes == 0)
                return t1.m_triesCount.CompareTo(t2.m_triesCount) < 0;
            else
                return cmpRes < 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NRPlanes.ServerData.EventsLog
{
    public class GameEventsLog
    {
        private ReaderWriterLockSlim m_rwl;
        private SortedList<Timestamp, GameEventsLogItem> m_items;

        public GameEventsLog()
        {
            m_items = new SortedList<Timestamp, GameEventsLogItem>();
            m_rwl = new ReaderWriterLockSlim();
        }

        public IEnumerable<GameEventsLogItem> GetAll(bool excludeDepriciated)
        {
            m_rwl.EnterReadLock();

            IEnumerable<GameEventsLogItem> result;

            if (excludeDepriciated)
                result = m_items.Values.Where(logItem => !logItem.IsDepriciated).ToList(); // force enumeration
            else
                result = m_items.Values;

            m_rwl.ExitReadLock();

            return result;
        }
       
        public IEnumerable<GameEventsLogItem> GetLogSince(Timestamp timestamp, bool excludeDepriciated)
        {
            m_rwl.EnterReadLock();

            List<GameEventsLogItem> result = new List<GameEventsLogItem>();

            for (int i = 0; i < m_items.Keys.Count; i++)
            {
                if (m_items.Keys[i] > timestamp && (!excludeDepriciated || !m_items.Values[i].IsDepriciated))
                    result.Add(m_items.Values[i]);
                else
                    continue;
            }

            m_rwl.ExitReadLock();

            return result;
        }

        public void AddEntry(GameEventsLogItem entry)
        {
            m_rwl.EnterWriteLock();

            m_items.Add(entry.Timestamp, entry);

            m_rwl.ExitWriteLock();
        }
    }
}

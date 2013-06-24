using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.ServerData.EventsLog
{
    public class GameEventsLog
    {
        private SortedList<Timestamp, GameEventsLogItem> m_items;

        public GameEventsLog()
        {
            m_items = new SortedList<Timestamp, GameEventsLogItem>();
        }

        public IEnumerable<GameEventsLogItem> GetAll()
        {
            return m_items.Values;
        }
        public IEnumerable<GameEventsLogItem> GetLogSince(Timestamp timestamp)
        {
            for (int i = 0; i < m_items.Keys.Count; i++)
            {
                if (m_items.Keys[i] > timestamp)
                    yield return m_items.Values[i];
                else
                    continue;
            }
        }

        public void AddEntry(GameEventsLogItem entry)
        {
            m_items.Add(entry.Timestamp, entry);
        }
    }
}

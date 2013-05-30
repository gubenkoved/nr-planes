using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NRPlanes.Core.Logging
{
    public static class Logger
    {
        public delegate void LogItemReceivedHandler(object item);
        public static event LogItemReceivedHandler LogItemReceived;
        private static void OnLogItemReceived(object item)
        {
            if (LogItemReceived != null)
                LogItemReceived.BeginInvoke(item, null, null);
        }

        private static List<object> m_log = new List<object>();

        public static void Log(string message)
        {
            m_log.Add(message);

            OnLogItemReceived(message);
        }
    }
}

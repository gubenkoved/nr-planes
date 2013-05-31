using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NRPlanes.Core.Common
{
    /// <summary>
    /// Thread-safe collection for:
    /// <para>- Add/remove items</para>
    /// <para>- Enumerating items through safe-handle</para>
    /// <para>Uses ReaderWriterLockSlim</para>
    /// <para>Enumeration usage example: </para>
    /// <para>using(var handle = collection.SafeRead()){ </para>
    /// <para>foreach(var item in handle.Items) {} }</para>
    /// </summary>
    public class ThreadSafeCollection<T>
    {
        public class SafeReadHandle : IDisposable
        {
            private ThreadSafeCollection<T> m_collection;
            public List<T> Items
            {
                get
                {
                    return m_collection.m_list;
                }
            }

            public SafeReadHandle(ThreadSafeCollection<T> threadSafeCollection)
            {
                m_collection = threadSafeCollection;

                threadSafeCollection.m_RWLock.EnterUpgradeableReadLock();
            }

            public void Dispose()
            {
                m_collection.m_RWLock.ExitUpgradeableReadLock();
            }
        }

        internal ReaderWriterLockSlim m_RWLock = new ReaderWriterLockSlim();
        private List<T> m_list = new List<T>();

        public int Count
        {
            get
            {
                return m_list.Count;
            }
        }

        public ThreadSafeCollection()
        {
        }

        T this[int i]
        {
            get
            {
                return m_list[i];
            }
            set
            {
                m_list[i] = value;
            }
        }
        public void Add(T item)
        {            
            m_RWLock.EnterWriteLock();

            m_list.Add(item);

            m_RWLock.ExitWriteLock();
        }
        public void AddRange(IEnumerable<T> items)
        {
            m_RWLock.EnterWriteLock();

            m_list.AddRange(items);

            m_RWLock.ExitWriteLock();
        }
        public void Remove(T item)
        {
            m_RWLock.EnterWriteLock();

            m_list.Remove(item);

            m_RWLock.ExitWriteLock();
        }
        public void RemoveAt(int i)
        {
            m_RWLock.EnterWriteLock();

            m_list.RemoveAt(i);

            m_RWLock.ExitWriteLock();
        }        
        public SafeReadHandle SafeRead()
        {
            return new SafeReadHandle(this);
        }
        public T[] ToArray()
        {
            return m_list.ToArray();
        }
    }
}

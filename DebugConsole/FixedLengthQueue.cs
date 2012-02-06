using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS
using System.Collections.Specialized;
#endif
using System.Collections;

namespace Gearset
{
    public class FixedLengthQueue<T> : 
#if WINDOWS
        INotifyCollectionChanged, 
#endif
        IEnumerable<T>
    {
        private Queue<T> queue;
        private int capacity;

        /// <summary>
        /// When an item is dequeued it will get queued into <c>DequeueTarget</c>, if any.
        /// </summary>
        public FixedLengthQueue<T> DequeueTarget;

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        /// <value>The count</value>
        public int Count { get { return queue.Count; } }

        /// <summary>
        /// Setting this is O(abs(Capacity - value))
        /// </summary>
        public int Capacity { 
            get { return capacity; } 
            set 
            {
                int diff = Count - value;
                while (diff-- > 0)
                    Dequeue();
                capacity = value;
            }
        }

#if WINDOWS
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#endif

        public FixedLengthQueue(int capacity)
        {
            queue = new Queue<T>(capacity);
            this.capacity = capacity;
        }

        /// <summary>
        /// Enqueues the specified item. If the queue is full, the oldest item
        /// will be droppped.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Enqueue(T item)
        {
            if (queue.Count + 1 > capacity && queue.Count > 0)
            {
                T removedItem = queue.Dequeue();
                if (DequeueTarget != null)
                    DequeueTarget.Enqueue(removedItem);
#if WINDOWS
                if (CollectionChanged != null)
                {
                    NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, 0);
                    CollectionChanged(this, args);
                }
#endif
            }
            queue.Enqueue(item);

#if WINDOWS
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, queue.Count - 1);
                CollectionChanged(this, args);
            }
#endif
                
        }

        /// <summary>
        /// Dequeues the oldest item.
        /// </summary>
        public T Dequeue()
        {
            T t = queue.Dequeue();
#if WINDOWS
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, t);
                CollectionChanged(this, args);
            }
#endif
            return t;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }

    }
}

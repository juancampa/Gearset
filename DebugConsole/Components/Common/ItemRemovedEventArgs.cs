using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components
{
    public class ItemRemovedEventArgs<T> : EventArgs
    {
        public ItemRemovedEventArgs(T removedItem)
        {
            this.RemovedItem = removedItem;
        }

        public T RemovedItem { get; private set; }
    }

    public class ItemAddedEventArgs<T> : EventArgs
    {
        public ItemAddedEventArgs(T addedItem)
        {
            this.AddedItem = addedItem;
        }

        public T AddedItem { get; private set; }
    }
}

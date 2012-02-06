using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// A collection of Key Wrappers. It can be accessed as a dictionary
    /// using the [] semantic.
    /// </summary>
    public class KeyWrapperCollection : ICollection<KeyWrapper>
    {
        private Dictionary<long, KeyWrapper> keys;

        public KeyWrapperCollection()
        {
            keys = new Dictionary<long, KeyWrapper>();
        }

        public KeyWrapper this[long index]
        {
            get
            {
                return keys[index];
            }

            set
            {
                keys[index] = value;
            }
            
        }

        public void Add(KeyWrapper item)
        {
            if (keys.ContainsKey(item.Id))
            {
                throw new InvalidOperationException("The same Curve Key was added twice.");
            }
            keys.Add(item.Id, item);
        }

        public void Clear()
        {
            keys.Clear();
        }

        public bool Contains(KeyWrapper item)
        {
            bool result = keys.ContainsKey(item.Id);
            // Check for inconsistencies.
            System.Diagnostics.Debug.Assert(keys[item.Id] == item, "Inconsistency found in KeyCollection");
            return result;
        }

        /// <summary>
        /// Returns the KeyWrapper of the provided key. This is a relatively
        /// expensive O(n) method because it iterates over the values of a dict.
        /// </summary>
        public KeyWrapper GetWrapper(CurveKey key)
        {
            foreach (KeyWrapper wrapper in keys.Values)
            {
                if (Object.ReferenceEquals(wrapper.Key, key))
                    return wrapper;
            }

            return null;
        }

        public void CopyTo(KeyWrapper[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return keys.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyWrapper item)
        {
            return keys.Remove(item.Id);
        }

        public IEnumerator<KeyWrapper> GetEnumerator()
        {
            foreach (var item in keys)
            {
                yield return item.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return keys.GetEnumerator();
        }

    }
}

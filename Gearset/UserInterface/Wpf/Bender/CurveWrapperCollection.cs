using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// A collection of Curve Wrappers. It can be accessed as a dictionary
    /// using the [] semantic either by name or index.
    /// </summary>
    public class CurveWrapperCollection : ICollection<CurveWrapper>
    {
        private Dictionary<long, CurveWrapper> curves;
        public event EventHandler<ItemAddedEventArgs<CurveWrapper>> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs<CurveWrapper>> ItemRemoved;
        public CurveWrapperCollection()
        {
            curves = new Dictionary<long, CurveWrapper>();
        }

        public CurveWrapper this[long index]
        {
            get
            {
                return curves[index];
            }
            set
            {
                curves[index] = value;
            }
            
        }

        public void Add(CurveWrapper item)
        {
            if (curves.ContainsKey(item.Id))
            {
                throw new InvalidOperationException("The same Curve was added twice.");
            }
            curves.Add(item.Id, item);
            if (ItemAdded != null)
                ItemAdded(this, new ItemAddedEventArgs<CurveWrapper>(item));
        }

        public void Clear()
        {
            curves.Clear();
        }

        public bool Contains(CurveWrapper item)
        {
            bool result = curves.ContainsKey(item.Id);
            // Check for inconsistencies.
            System.Diagnostics.Debug.Assert(curves[item.Id] == item, "Inconsistency found in CurveCollection");
            return result;
        }

        public void CopyTo(CurveWrapper[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return curves.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(CurveWrapper item)
        {
            if (ItemRemoved != null)
                ItemRemoved(this, new ItemRemovedEventArgs<CurveWrapper>(item));
            return curves.Remove(item.Id);
        }

        public IEnumerator<CurveWrapper> GetEnumerator()
        {
            foreach (var item in curves)
            {
                yield return item.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return curves.GetEnumerator();
        }
    }
}

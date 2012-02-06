using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Contains the selected key set. It will automatically set the IsSelected
    /// property to the keys in it, and unset it when they're removed. If a key
    /// is already selected, it won't be added again.
    /// </summary>
    public class KeySelection : Collection<KeyWrapper>
    {
        protected override void InsertItem(int index, KeyWrapper item)
        {
            if (item.IsSelected)
                return;
            item.IsSelected = true;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (index < Count)
                this[index].IsSelected = false;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.IsSelected = false;
            }
            base.ClearItems();
        }

    }
}

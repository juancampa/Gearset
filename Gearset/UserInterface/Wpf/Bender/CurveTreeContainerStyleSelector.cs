using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Gearset.Components
{
    public class CurveTreeContainerStyleSelector : StyleSelector
    {
        public Style LeafStyle { get; set; }
        public Style NodeStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is CurveTreeLeaf)
                return LeafStyle;
            else
                return NodeStyle;
        }
    }
}

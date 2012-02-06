using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms.Integration;

namespace Gearset.Components.InspectorWPF
{
    public class MethodCallerTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        static MethodCallerTemplateSelector()
        {
        }
        
        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                element.DataContext = item;

                if (item is MethodCaller)
                    return element.FindResource("methodCallerItemTemplate") as DataTemplate;
                if (item is MethodParamContainer)
                    return element.FindResource("methodParamItemTemplate") as DataTemplate;
            }

            return null;
        }
    }
}

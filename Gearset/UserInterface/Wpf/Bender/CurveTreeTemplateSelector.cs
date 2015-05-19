using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace Gearset.Components
{
    public class CurveTreeTemplateSelector : DataTemplateSelector
    {
        private static Dictionary<Type, CachedTemplate> TypeTemplateMap;

        /// <summary>
        /// Static constructor
        /// </summary>
        static CurveTreeTemplateSelector()
        {
            TypeTemplateMap = new Dictionary<Type, CachedTemplate>();

            TypeTemplateMap.Add(typeof(CurveTreeNode), new CachedTemplate("curveTreeNodeTemplate"));
            TypeTemplateMap.Add(typeof(CurveTreeLeaf), new CachedTemplate("curveTreeLeafTemplate"));
        }
        
        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                Type nodeType = item.GetType();
                
                if (TypeTemplateMap.ContainsKey(nodeType))
                {
                    CachedTemplate cache = TypeTemplateMap[nodeType];
                    if (cache.DataTemplate == null)
                        cache.DataTemplate = element.FindResource(cache.Name) as DataTemplate;
                    return cache.DataTemplate;
                }
            }

            return null;
        }
    }
}

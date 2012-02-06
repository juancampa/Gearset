using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms.Integration;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace Gearset.Components
{
    internal class CachedTemplate
    {
        internal String Name;
        internal DataTemplate DataTemplate;
        internal CachedTemplate(String name)
        {
            Name = name;
        }
    }

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

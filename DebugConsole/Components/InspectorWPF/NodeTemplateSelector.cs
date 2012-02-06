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
using Gearset.Components;

namespace Gearset.Components.InspectorWPF
{

    public class NodeTemplateSelector : DataTemplateSelector
    {
        private static Dictionary<Type, CachedTemplate> TypeTemplateMap;
        private static CachedTemplate genericTemplateCache = new CachedTemplate("genericFieldTemplate");
        private static CachedTemplate gearConfigTemplateCache = new CachedTemplate("gearConfigTemplate");
        private static CachedTemplate rootTemplateCache = new CachedTemplate("rootTemplate");

        /// <summary>
        /// Static constructor
        /// </summary>
        static NodeTemplateSelector()
        {
            TypeTemplateMap = new Dictionary<Type, CachedTemplate>();

            // Primitive types
            TypeTemplateMap.Add(typeof(float), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(double), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(decimal), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(char), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(short), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(ushort), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(int), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(uint), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(long), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(ulong), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(byte), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(sbyte), new CachedTemplate("floatFieldTemplate"));
            TypeTemplateMap.Add(typeof(bool), new CachedTemplate("boolFieldTemplate"));

            // Other System stuff
            TypeTemplateMap.Add(typeof(String), new CachedTemplate("stringFieldTemplate"));
            TypeTemplateMap.Add(typeof(Enum), new CachedTemplate("enumFieldTemplate"));
            TypeTemplateMap.Add(typeof(void), new CachedTemplate("actionFieldTemplate"));

            // XNA stuff
            TypeTemplateMap.Add(typeof(Vector2), new CachedTemplate("vector2FieldTemplate"));
            TypeTemplateMap.Add(typeof(Vector3), new CachedTemplate("vector3FieldTemplate"));
            TypeTemplateMap.Add(typeof(Quaternion), new CachedTemplate("quaternionFieldTemplate"));
            TypeTemplateMap.Add(typeof(Texture2D), new CachedTemplate("texture2DFieldTemplate"));
            TypeTemplateMap.Add(typeof(Color), new CachedTemplate("colorFieldTemplate"));
            TypeTemplateMap.Add(typeof(Curve), new CachedTemplate("curveFieldTemplate"));

            // Gearset stuff
            TypeTemplateMap.Add(typeof(CollectionMarker), new CachedTemplate("collectionFieldTemplate"));
            TypeTemplateMap.Add(typeof(Texture2DMarker), new CachedTemplate("texture2DMarkerTemplate"));
            TypeTemplateMap.Add(typeof(LineDrawerConfig), new CachedTemplate("clearableGearConfigTemplate"));
            TypeTemplateMap.Add(typeof(LabelerConfig), new CachedTemplate("clearableGearConfigTemplate"));
            TypeTemplateMap.Add(typeof(TreeViewConfig), new CachedTemplate("clearableGearConfigTemplate"));
            TypeTemplateMap.Add(typeof(PlotterConfig), new CachedTemplate("clearableGearConfigTemplate"));

        }
        
        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                element.DataContext = item;
                InspectorNode node = item as InspectorNode;
                Type nodeType = node.Type;

                // Enums are handled differently
                if (nodeType.IsEnum)
                    nodeType = typeof(Enum);

                //// The root has a especial case.
                //if (node.Parent == null)
                //{
                //    if (rootTemplateCache.DataTemplate == null)
                //        rootTemplateCache.DataTemplate = element.FindResource(rootTemplateCache.Name) as DataTemplate;
                //    return rootTemplateCache.DataTemplate;
                //}

                
                if (TypeTemplateMap.ContainsKey(nodeType))
                {
                    CachedTemplate cache = TypeTemplateMap[nodeType];
                    if (cache.DataTemplate == null)
                        cache.DataTemplate = element.FindResource(cache.Name) as DataTemplate;
                    return cache.DataTemplate;
                }
                else if (typeof(GearConfig).IsAssignableFrom(nodeType))
                {
                    if (gearConfigTemplateCache.DataTemplate == null)
                        gearConfigTemplateCache.DataTemplate = element.FindResource(gearConfigTemplateCache.Name) as DataTemplate;
                    return gearConfigTemplateCache.DataTemplate;
                }
                else
                {
                    if (genericTemplateCache.DataTemplate == null)
                        genericTemplateCache.DataTemplate = element.FindResource(genericTemplateCache.Name) as DataTemplate;
                    return genericTemplateCache.DataTemplate;
                }
            }

            return null;
        }
    }
}

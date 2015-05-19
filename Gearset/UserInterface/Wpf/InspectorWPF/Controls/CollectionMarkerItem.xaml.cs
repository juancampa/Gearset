using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections;


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class CollectionMarkerItem : VisualItemBase
    {
        

        List<Object> items;
        private bool updateRequested = true;

        public CollectionMarkerItem()
        {
            InitializeComponent();
            items = new List<Object>();

            ListBox1.ItemsSource = items;
        }

        public override void UpdateUI(Object value)
        {
            if (updateRequested)
            {
                items.Clear();
                var enumerable = value as IEnumerable;
                foreach (var item in enumerable)
                {
                    items.Add(item);
                }

                // Update the listbox and count.
                ListBox1.ItemsSource = null;
                ListBox1.ItemsSource = items;
                CountTextBlock.Text = items.Count.ToString();


                updateRequested = false;
            }
        }

        protected void CollectionListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Inspect collection items.
            FrameworkElement element = e.OriginalSource as FrameworkElement;
            if (element != null && element.DataContext != null)
                GearsetResources.Console.Inspect("Item (" + element.DataContext.ToString() + ")", element.DataContext);
            e.Handled = true;
        }

        public void ListView_LostFocus(object sender, RoutedEventArgs e)
        {
            ListView list = sender as ListView;
            if (list != null)
                list.UnselectAll();
        }

        public void ListView_GotFocus(object sender, RoutedEventArgs e)
        {
            InspectorNode node = GearsetResources.Console.Inspector.SelectedItem as InspectorNode;
            if (node != null)
            {
                TreeViewItem item = node.UIContainer as TreeViewItem;
                if (item != null)
                    item.IsSelected = false;
            }
        }

        public void ListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void ListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            updateRequested = true;
        }
    }

    public class CollectionMarkerListTemplateSelector : DataTemplateSelector
    {
        private static CachedTemplate texture2DTemplateCache = new CachedTemplate("textureTemplate");
        private static CachedTemplate genericTemplateCache = new CachedTemplate("genericTemplate");

        /// <summary>
        /// Static constructor
        /// </summary>
        static CollectionMarkerListTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                element.DataContext = item;

                if (item is Texture2D)
                {
                    if (texture2DTemplateCache.DataTemplate == null)
                        texture2DTemplateCache.DataTemplate = element.FindResource(texture2DTemplateCache.Name) as DataTemplate;
                    return texture2DTemplateCache.DataTemplate;
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

    ///// <summary>
    ///// Wraps objects in the list.
    ///// </summary>
    //internal class ObjectWrapper
    //{
    //    /// <summary>
    //    /// Name is legacy from InspectorNode.
    //    /// </summary>
    //    internal Object Property { get; private set; }
    //    internal ObjectWrapper(Object o)
    //    {
    //        this.Property = o;
    //    }
    //    internal override string ToString()
    //    {
    //        return Property.ToString();
    //    }
    //}
}

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
using Gearset.Components.Persistor;
//using Arction.LightningChartBasic;
//using Arction.LightningChartBasic.Axes;
//using Arction.LightningChartBasic.Series;

namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Inspector : Window
    {
        /// <summary>
        /// position where the mouse was clicked.
        /// </summary>
        private Point downPosition;
        private bool isDragging;

        /// <summary>
        /// If the node expansion was generated because the currently selected node
        /// dissapeared (because we're adding private fields, for example) then this
        /// would generate a conflict with the expansion.
        /// </summary>
        internal InspectorNode nodeToExpandAfterUpdate;

        internal bool WasHiddenByGameMinimize { get; set; }

        public Inspector()
        {
            InitializeComponent();

            this.SizeChanged += new SizeChangedEventHandler(Inspector_SizeChanged);
            Closing += new System.ComponentModel.CancelEventHandler(Inspector_Closing);
        }

        void Inspector_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized)
                this.Hide();
        }

        public void Inspector_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void Item_MouseMove(object sender, RoutedEventArgs e)
        {
            if (TreeView1.SelectedItem == null)
                return;
            if (Mouse.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
                if (e.OriginalSource is System.Windows.Controls.TextBox)
                    return;
                Point pos = Mouse.GetPosition(this);
                if (Math.Abs(pos.X - downPosition.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - downPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                {   
                    StartDrag();
                }
            }
        }

        public void Item_MouseDown(object sender, RoutedEventArgs e)
        {
            downPosition = Mouse.GetPosition(this);
        }


        private void StartDrag()
        {
            isDragging = true;
            DataObject data = new DataObject(typeof(Object), ((InspectorNode)TreeView1.SelectedItem).Property);
            DragDropEffects de = DragDrop.DoDragDrop(this.TreeView1, data, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.None);
            isDragging = false;
        }

        public void TreeView1_DragOver(object sender, DragEventArgs e)
        {
            InspectorNode targetTreeNode = ((FrameworkElement)e.OriginalSource).DataContext as InspectorNode;
            if (targetTreeNode == null)
            {
                return;
            }

            Object data = e.Data.GetData(typeof(Object));
            if (data == null)
                return;
            Type dataType = data.GetType();
            Type targetType = targetTreeNode.Type;

            if (targetType.IsAssignableFrom(dataType) ||
                targetType == typeof(float) && dataType == typeof(double) ||
                targetType == typeof(double) && dataType == typeof(float))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        public void TreeView1_Drop(object sender, DragEventArgs e)
        {
            InspectorNode targetTreeNode = ((FrameworkElement)e.OriginalSource).DataContext as InspectorNode;
            if (targetTreeNode == null)
                return;

            Object data = e.Data.GetData(typeof(Object));
            if (data == null)
                return;
            Type dataType = data.GetType();
            Type targetType = targetTreeNode.Type;

            if (targetType.IsAssignableFrom(dataType))
            {
                targetTreeNode.Property = data;
                e.Handled = true;
            }
            else if (targetType == typeof(float) && dataType == typeof(double))
                targetTreeNode.Property = (float)((double)data);
            else if (targetType == typeof(double) && dataType == typeof(float))
                targetTreeNode.Property = (double)((float)data);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = true;
            base.OnGiveFeedback(e);
        }

        public void Inspect_Click(object sender, RoutedEventArgs e)
        {
            InspectorNode item = ((InspectorNode)TreeView1.SelectedItem);
            if (item == null)
                return;

            Object o = item.Property;
            if (o != null && !item.Type.IsValueType)
                GearsetResources.Console.Inspect(item.GetPath(), o);
        }

        public void ShowPrivate_Click(object sender, RoutedEventArgs e)
        {
            InspectorNode item = ((InspectorNode)TreeView1.SelectedItem);
            if (item == null)
                return;

            item.IsShowingPrivate = true;
        }

        public void Watch_Click(object sender, RoutedEventArgs e)
        {
            InspectorNode item = ((InspectorNode)TreeView1.SelectedItem);
            if (item != null)
                GearsetResources.Console.Inspector.Watch(item);
        }

        public void Clear_Click(object sender, RoutedEventArgs e)
        {
            InspectorNode item = ((InspectorNode)TreeView1.SelectedItem);
            if (item == null)
                return;

            if (item.CanWrite && !item.Type.IsValueType)
                item.Property = null;
        }

        public void Remove_Click(object sender, RoutedEventArgs e)
        {
            InspectorNode item = ((InspectorNode)TreeView1.SelectedItem);
            if (item == null)
                return;

            Object o = item.Property;
            if (o != null)
                GearsetResources.Console.RemoveInspect(o);
        }

        //void SavePersistorData_Click(object sender, RoutedEventArgs e)
        //{
        //    GearsetResources.Console.SavePersistorData();
        //}

        public void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                InspectorNode node = e.NewValue as InspectorNode;
                if (node != null)
                    nodeToExpandAfterUpdate = node;
            }
        }

        public void InvokeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void TreeView1_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                // If we're right-clicking on a Collection Marker, don't take the focus.
                bool takeFocus = true;
                InspectorNode node = treeViewItem.DataContext as InspectorNode;
                if (node != null)
                {
                    if (node.VisualItem != null && node.VisualItem is CollectionMarkerItem)
                        takeFocus = false;
                }
                if (takeFocus)
                {
                    treeViewItem.Focus();
                    e.Handled = true;
                }
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T)) source = VisualTreeHelper.GetParent(source); return source;
        }

        public void Expander_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Prevent selecting the whole expander in the Inspector treeview.
            e.Handled = true;
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void Notice_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((InspectorManager)DataContext).Config.SearchText = String.Empty;
        }

        private void HidePlotsButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Plotter.HideAll();
        }

        private void ShowPlotsButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Plotter.ShowAll();
        }

        private void ResetPlotsPositionsButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Plotter.ResetPositions();
        }
    }
}

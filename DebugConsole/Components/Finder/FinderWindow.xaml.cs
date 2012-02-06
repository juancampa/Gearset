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

namespace Gearset.Components
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FinderWindow : Window
    {
        private bool isDragging;
        private Point downPosition;
        internal bool WasHiddenByGameMinimize { get; set; }
        public FinderWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Finder)DataContext).Config.SearchText = String.Empty;
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

        protected void Results_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource != null)
            {
                ObjectDescription o = ((FrameworkElement)e.OriginalSource).DataContext as ObjectDescription;
                if (o != null)
                    GearsetResources.Console.Inspect(o.Name, o.Object);
            }
        }

        public void Item_MouseMove(object sender, RoutedEventArgs e)
        {
            if (ResultsListBox.SelectedItem == null || !(((FrameworkElement)e.OriginalSource).DataContext is ObjectDescription))
                return;
            if (Mouse.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
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

            ObjectDescription selected = ResultsListBox.SelectedItem as ObjectDescription;
            if (selected != null)
            {
                Object o = selected.Object;
                if (o != null)
                {
                    DataObject data = new DataObject(o.GetType(), o);
                    DragDropEffects de = DragDrop.DoDragDrop(this.ResultsListBox, data, DragDropEffects.Move);
                    isDragging = false;
                }
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (ResultsListBox.SelectedIndex == -1)
            {
                if (ResultsListBox.Items.Count > 0)
                    ResultsListBox.SelectedIndex = 0;
                else
                    return;
            }
            if (e.Key == Key.Down)
            {
                ResultsListBox.SelectedIndex = (ResultsListBox.SelectedIndex + 1) % ResultsListBox.Items.Count;
            }
            else if (e.Key == Key.Up)
            {
                ResultsListBox.SelectedIndex = (ResultsListBox.SelectedIndex - 1) % ResultsListBox.Items.Count;
            }
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                ObjectDescription o = ResultsListBox.SelectedItem as ObjectDescription;
                if (o != null)
                    GearsetResources.Console.Inspect(o.Name, o.Object);
            }
        }

        private void ResultsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                ObjectDescription o = ResultsListBox.SelectedItem as ObjectDescription;
                if (o != null)
                    GearsetResources.Console.Inspect(o.Name, o.Object);
            }
            else
                SearchTextBox.Focus();
            //SearchTextBox.RaiseEvent(new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
        }
    }
}

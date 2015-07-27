using System;
using System.Windows;
using System.Windows.Input;

namespace Gearset.UserInterface.Wpf.Finder
{
    /// <summary>
    /// Interaction logic for FinderWindow.xaml
    /// </summary>
    public partial class FinderWindow : IWindow
    {
        public class ObjectSelectedEventArgs : EventArgs
        {
            public string Name { get; private set; }
            public object Object { get; private set; }

            public ObjectSelectedEventArgs(string name, object o)
            {
                Name = name;
                Object = o;
            }
        }

        public event EventHandler<ObjectSelectedEventArgs> ObjectSelected;

        public bool WasHiddenByGameMinimize { get; set; }

        bool _isDragging;
        Point _downPosition;

        public FinderWindow()
        {
            InitializeComponent();
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Components.Finder.Finder)DataContext).Config.SearchText = string.Empty;
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        protected void Results_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == null) 
                return;

            var o = ((FrameworkElement)e.OriginalSource).DataContext as ObjectDescription;
            if (o != null)
                OnObjectSelected(new ObjectSelectedEventArgs(o.Name, o.Object));
        }

        public void Item_MouseMove(object sender, RoutedEventArgs e)
        {
            if (ResultsListBox.SelectedItem == null || !(((FrameworkElement)e.OriginalSource).DataContext is ObjectDescription))
                return;

            if (Mouse.LeftButton != MouseButtonState.Pressed || _isDragging) 
                return;

            var pos = Mouse.GetPosition(this);
            if (Math.Abs(pos.X - _downPosition.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(pos.Y - _downPosition.Y) > SystemParameters.MinimumVerticalDragDistance)
                StartDrag();
        }

        public void Item_MouseDown(object sender, RoutedEventArgs e)
        {
            _downPosition = Mouse.GetPosition(this);
        }

        private void StartDrag()
        {
            _isDragging = true;

            var selected = ResultsListBox.SelectedItem as ObjectDescription;
            if (selected == null) 
                return;

            var o = selected.Object;
            if (o == null) 
                return;

            var data = new DataObject(o.GetType(), o);
            DragDrop.DoDragDrop(ResultsListBox, data, DragDropEffects.Move);
            _isDragging = false;
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

            switch (e.Key)
            {
                case Key.Down:
                    ResultsListBox.SelectedIndex = (ResultsListBox.SelectedIndex + 1) % ResultsListBox.Items.Count;
                    break;

                case Key.Up:
                    ResultsListBox.SelectedIndex = (ResultsListBox.SelectedIndex - 1) % ResultsListBox.Items.Count;
                    break;

                default:
                    if (e.Key == Key.Enter || e.Key == Key.Return)
                    {
                        var o = ResultsListBox.SelectedItem as ObjectDescription;
                        if (o != null)
                            OnObjectSelected(new ObjectSelectedEventArgs(o.Name, o.Object));
                    }
                    break;
            }
        }

        private void ResultsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                var o = ResultsListBox.SelectedItem as ObjectDescription;
                if (o != null)
                    OnObjectSelected(new ObjectSelectedEventArgs(o.Name, o.Object));
            }
            else
                SearchTextBox.Focus();
        }

        protected virtual void OnObjectSelected(ObjectSelectedEventArgs e)
        {
            var handler = ObjectSelected;
            if (handler != null)
                handler(this, e);
        }
    }
}

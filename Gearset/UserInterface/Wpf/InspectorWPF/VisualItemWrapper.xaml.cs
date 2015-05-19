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


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Holds items
    /// </summary>
    public partial class VisualItemWrapper : UserControl
    {
        #region Attached Events (Mouse Move & Mouse Down)
        public static readonly RoutedEvent TextMouseMoveEvent = EventManager.RegisterRoutedEvent("TextMouseMove", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VisualItemWrapper));
        public static readonly RoutedEvent TextMouseDownEvent = EventManager.RegisterRoutedEvent("TextMouseDown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(VisualItemWrapper));

        public static void AddTextMouseMoveHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
                uie.AddHandler(VisualItemWrapper.MouseMoveEvent, handler);
        }
        public static void RemoveTextMouseMoveHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
                uie.RemoveHandler(VisualItemWrapper.MouseMoveEvent, handler);
        }

        public static void AddTextMouseDownHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
                uie.AddHandler(VisualItemWrapper.MouseDownEvent, handler);
        }
        public static void RemoveTextMouseDownHandler(DependencyObject d, RoutedEventHandler handler)
        {
            UIElement uie = d as UIElement;
            if (uie != null)
                uie.RemoveHandler(VisualItemWrapper.MouseDownEvent, handler);
        }

        #endregion

        private VisualItemBase visualItem;
        public VisualItemBase VisualItem
        {
            get
            {
                return visualItem;
            }
            set
            {
                Grid1.Children.Add(value);
                Grid.SetColumn(value, 2);
                visualItem = value;
            }
        }

        /// <summary>
        /// The InspectorTreeNode that this control affetcs.
        /// </summary>
        public String Text
        {
            get
            {
                return (String)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Registers a dependency property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(String), typeof(VisualItemWrapper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsParentArrange, TextChangedCallback));

        /// <summary>
        /// Calls the handler that updates the textBlock with the new value.
        /// </summary>
        public static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VisualItemWrapper)d).UpdateText((string)e.NewValue);
        }

        /// <summary>
        /// Updates the textBlock with the new value.
        /// </summary>
        private void UpdateText(String text)
        {
            TextBlock1.Text = text;
        }

        public VisualItemWrapper()
        {
            InitializeComponent();
            labelsPanel.MouseDown += new MouseButtonEventHandler(VisualItemWrapper_PreviewMouseDown);
            labelsPanel.MouseMove += new MouseEventHandler(VisualItemWrapper_PreviewMouseMove);

            //transparentRectangle.MouseDown += new MouseButtonEventHandler(VisualItemWrapper_PreviewMouseDown);
            //transparentRectangle.MouseMove += new MouseEventHandler(VisualItemWrapper_PreviewMouseMove);
        }

        public void VisualItemWrapper_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(VisualItemWrapper.TextMouseDownEvent, e.OriginalSource));
        }

        public void VisualItemWrapper_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            this.RaiseEvent(new RoutedEventArgs(VisualItemWrapper.TextMouseMoveEvent, e.OriginalSource));
        }

    }
}

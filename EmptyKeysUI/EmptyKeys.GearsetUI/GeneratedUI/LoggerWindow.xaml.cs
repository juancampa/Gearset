// -----------------------------------------------------------
//  
//  This file was generated, please do not modify.
//  
// -----------------------------------------------------------
namespace EmptyKeys.UserInterface.Generated {
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.ObjectModel;
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Data;
    using EmptyKeys.UserInterface.Controls;
    using EmptyKeys.UserInterface.Controls.Primitives;
    using EmptyKeys.UserInterface.Input;
    using EmptyKeys.UserInterface.Media;
    using EmptyKeys.UserInterface.Media.Animation;
    using EmptyKeys.UserInterface.Media.Imaging;
    using EmptyKeys.UserInterface.Shapes;
    using EmptyKeys.UserInterface.Renderers;
    using EmptyKeys.UserInterface.Themes;
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "1.10.0.0")]
    public partial class LoggerWindow : UserControl {
        
        private Grid e_18;
        
        private ItemsControl _streamListBox;
        
        private ScrollViewer _scrollViewer;
        
        private ItemsControl _logListBox;
        
        public LoggerWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_18 element
            this.e_18 = new Grid();
            this.Content = this.e_18;
            this.e_18.Name = "e_18";
            this.e_18.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            ColumnDefinition col_e_18_0 = new ColumnDefinition();
            col_e_18_0.Width = new GridLength(1F, GridUnitType.Auto);
            this.e_18.ColumnDefinitions.Add(col_e_18_0);
            ColumnDefinition col_e_18_1 = new ColumnDefinition();
            col_e_18_1.Width = new GridLength(7F, GridUnitType.Star);
            this.e_18.ColumnDefinitions.Add(col_e_18_1);
            // _streamListBox element
            this._streamListBox = new ItemsControl();
            this.e_18.Children.Add(this._streamListBox);
            this._streamListBox.Name = "_streamListBox";
            this._streamListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this._streamListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> _streamListBox_dtFunc = _streamListBox_dtMethod;
            this._streamListBox.ItemTemplate = new DataTemplate(_streamListBox_dtFunc);
            Grid.SetColumn(this._streamListBox, 0);
            Binding binding__streamListBox_ItemsSource = new Binding("Streams");
            this._streamListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding__streamListBox_ItemsSource);
            // _scrollViewer element
            this._scrollViewer = new ScrollViewer();
            this.e_18.Children.Add(this._scrollViewer);
            this._scrollViewer.Name = "_scrollViewer";
            Grid.SetColumn(this._scrollViewer, 1);
            // _logListBox element
            this._logListBox = new ItemsControl();
            this._scrollViewer.Content = this._logListBox;
            this._logListBox.Name = "_logListBox";
            this._logListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this._logListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> _logListBox_dtFunc = _logListBox_dtMethod;
            this._logListBox.ItemTemplate = new DataTemplate(_logListBox_dtFunc);
            Binding binding__logListBox_ItemsSource = new Binding("VisibleLogItems");
            this._logListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding__logListBox_ItemsSource);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
            // Resource - [streamTemplate] DataTemplate
            Func<UIElement, UIElement> r_0_dtFunc = r_0_dtMethod;
            elem.Resources.Add("streamTemplate", new DataTemplate(r_0_dtFunc));
            // Resource - [logContentTemplate] DataTemplate
            Func<UIElement, UIElement> r_1_dtFunc = r_1_dtMethod;
            elem.Resources.Add("logContentTemplate", new DataTemplate(r_1_dtFunc));
        }
        
        private static UIElement r_0_dtMethod(UIElement parent) {
            // e_12 element
            DockPanel e_12 = new DockPanel();
            e_12.Parent = parent;
            e_12.Name = "e_12";
            // e_13 element
            CheckBox e_13 = new CheckBox();
            e_12.Children.Add(e_13);
            e_13.Name = "e_13";
            e_13.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_13, Dock.Left);
            Binding binding_e_13_IsChecked = new Binding("Enabled");
            e_13.SetBinding(CheckBox.IsCheckedProperty, binding_e_13_IsChecked);
            // e_14 element
            TextBlock e_14 = new TextBlock();
            e_12.Children.Add(e_14);
            e_14.Name = "e_14";
            e_14.Margin = new Thickness(0F, 0F, 10F, 0F);
            e_14.VerticalAlignment = VerticalAlignment.Center;
            Binding binding_e_14_Foreground = new Binding("Color");
            e_14.SetBinding(TextBlock.ForegroundProperty, binding_e_14_Foreground);
            Binding binding_e_14_Text = new Binding("Name");
            e_14.SetBinding(TextBlock.TextProperty, binding_e_14_Text);
            return e_12;
        }
        
        private static UIElement r_1_dtMethod(UIElement parent) {
            // e_15 element
            Grid e_15 = new Grid();
            e_15.Parent = parent;
            e_15.Name = "e_15";
            ColumnDefinition col_e_15_0 = new ColumnDefinition();
            col_e_15_0.Width = new GridLength(45F, GridUnitType.Pixel);
            e_15.ColumnDefinitions.Add(col_e_15_0);
            ColumnDefinition col_e_15_1 = new ColumnDefinition();
            col_e_15_1.Width = new GridLength(7F, GridUnitType.Star);
            e_15.ColumnDefinitions.Add(col_e_15_1);
            // e_16 element
            TextBlock e_16 = new TextBlock();
            e_15.Children.Add(e_16);
            e_16.Name = "e_16";
            e_16.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(e_16, 0);
            Binding binding_e_16_Text = new Binding("UpdateNumber");
            e_16.SetBinding(TextBlock.TextProperty, binding_e_16_Text);
            // e_17 element
            TextBlock e_17 = new TextBlock();
            e_15.Children.Add(e_17);
            e_17.Name = "e_17";
            Grid.SetColumn(e_17, 1);
            Binding binding_e_17_Foreground = new Binding("Color");
            e_17.SetBinding(TextBlock.ForegroundProperty, binding_e_17_Foreground);
            Binding binding_e_17_Text = new Binding("Content");
            e_17.SetBinding(TextBlock.TextProperty, binding_e_17_Text);
            return e_15;
        }
        
        private static UIElement _streamListBox_dtMethod(UIElement parent) {
            // e_19 element
            DockPanel e_19 = new DockPanel();
            e_19.Parent = parent;
            e_19.Name = "e_19";
            // e_20 element
            CheckBox e_20 = new CheckBox();
            e_19.Children.Add(e_20);
            e_20.Name = "e_20";
            e_20.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_20, Dock.Left);
            Binding binding_e_20_IsChecked = new Binding("Enabled");
            e_20.SetBinding(CheckBox.IsCheckedProperty, binding_e_20_IsChecked);
            // e_21 element
            TextBlock e_21 = new TextBlock();
            e_19.Children.Add(e_21);
            e_21.Name = "e_21";
            e_21.Margin = new Thickness(0F, 0F, 10F, 0F);
            e_21.VerticalAlignment = VerticalAlignment.Center;
            Binding binding_e_21_Foreground = new Binding("Color");
            e_21.SetBinding(TextBlock.ForegroundProperty, binding_e_21_Foreground);
            Binding binding_e_21_Text = new Binding("Name");
            e_21.SetBinding(TextBlock.TextProperty, binding_e_21_Text);
            return e_19;
        }
        
        private static UIElement _logListBox_dtMethod(UIElement parent) {
            // e_22 element
            Grid e_22 = new Grid();
            e_22.Parent = parent;
            e_22.Name = "e_22";
            ColumnDefinition col_e_22_0 = new ColumnDefinition();
            col_e_22_0.Width = new GridLength(45F, GridUnitType.Pixel);
            e_22.ColumnDefinitions.Add(col_e_22_0);
            ColumnDefinition col_e_22_1 = new ColumnDefinition();
            col_e_22_1.Width = new GridLength(7F, GridUnitType.Star);
            e_22.ColumnDefinitions.Add(col_e_22_1);
            // e_23 element
            TextBlock e_23 = new TextBlock();
            e_22.Children.Add(e_23);
            e_23.Name = "e_23";
            e_23.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(e_23, 0);
            Binding binding_e_23_Text = new Binding("UpdateNumber");
            e_23.SetBinding(TextBlock.TextProperty, binding_e_23_Text);
            // e_24 element
            TextBlock e_24 = new TextBlock();
            e_22.Children.Add(e_24);
            e_24.Name = "e_24";
            Grid.SetColumn(e_24, 1);
            Binding binding_e_24_Foreground = new Binding("Color");
            e_24.SetBinding(TextBlock.ForegroundProperty, binding_e_24_Foreground);
            Binding binding_e_24_Text = new Binding("Content");
            e_24.SetBinding(TextBlock.TextProperty, binding_e_24_Text);
            return e_22;
        }
    }
}

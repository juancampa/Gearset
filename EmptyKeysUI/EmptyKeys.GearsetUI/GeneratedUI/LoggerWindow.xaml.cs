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
        
        private Grid e_27;
        
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
            // e_27 element
            this.e_27 = new Grid();
            this.Content = this.e_27;
            this.e_27.Name = "e_27";
            this.e_27.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            ColumnDefinition col_e_27_0 = new ColumnDefinition();
            col_e_27_0.Width = new GridLength(1F, GridUnitType.Auto);
            this.e_27.ColumnDefinitions.Add(col_e_27_0);
            ColumnDefinition col_e_27_1 = new ColumnDefinition();
            col_e_27_1.Width = new GridLength(7F, GridUnitType.Star);
            this.e_27.ColumnDefinitions.Add(col_e_27_1);
            // _streamListBox element
            this._streamListBox = new ItemsControl();
            this.e_27.Children.Add(this._streamListBox);
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
            this.e_27.Children.Add(this._scrollViewer);
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
            // e_21 element
            DockPanel e_21 = new DockPanel();
            e_21.Parent = parent;
            e_21.Name = "e_21";
            // e_22 element
            CheckBox e_22 = new CheckBox();
            e_21.Children.Add(e_22);
            e_22.Name = "e_22";
            e_22.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_22, Dock.Left);
            Binding binding_e_22_IsChecked = new Binding("Enabled");
            e_22.SetBinding(CheckBox.IsCheckedProperty, binding_e_22_IsChecked);
            // e_23 element
            TextBlock e_23 = new TextBlock();
            e_21.Children.Add(e_23);
            e_23.Name = "e_23";
            e_23.Margin = new Thickness(0F, 0F, 10F, 0F);
            e_23.VerticalAlignment = VerticalAlignment.Center;
            Binding binding_e_23_Foreground = new Binding("Color");
            e_23.SetBinding(TextBlock.ForegroundProperty, binding_e_23_Foreground);
            Binding binding_e_23_Text = new Binding("Name");
            e_23.SetBinding(TextBlock.TextProperty, binding_e_23_Text);
            return e_21;
        }
        
        private static UIElement r_1_dtMethod(UIElement parent) {
            // e_24 element
            Grid e_24 = new Grid();
            e_24.Parent = parent;
            e_24.Name = "e_24";
            ColumnDefinition col_e_24_0 = new ColumnDefinition();
            col_e_24_0.Width = new GridLength(45F, GridUnitType.Pixel);
            e_24.ColumnDefinitions.Add(col_e_24_0);
            ColumnDefinition col_e_24_1 = new ColumnDefinition();
            col_e_24_1.Width = new GridLength(7F, GridUnitType.Star);
            e_24.ColumnDefinitions.Add(col_e_24_1);
            // e_25 element
            TextBlock e_25 = new TextBlock();
            e_24.Children.Add(e_25);
            e_25.Name = "e_25";
            e_25.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(e_25, 0);
            Binding binding_e_25_Text = new Binding("UpdateNumber");
            e_25.SetBinding(TextBlock.TextProperty, binding_e_25_Text);
            // e_26 element
            TextBlock e_26 = new TextBlock();
            e_24.Children.Add(e_26);
            e_26.Name = "e_26";
            Grid.SetColumn(e_26, 1);
            Binding binding_e_26_Foreground = new Binding("Color");
            e_26.SetBinding(TextBlock.ForegroundProperty, binding_e_26_Foreground);
            Binding binding_e_26_Text = new Binding("Content");
            e_26.SetBinding(TextBlock.TextProperty, binding_e_26_Text);
            return e_24;
        }
        
        private static UIElement _streamListBox_dtMethod(UIElement parent) {
            // e_28 element
            DockPanel e_28 = new DockPanel();
            e_28.Parent = parent;
            e_28.Name = "e_28";
            // e_29 element
            CheckBox e_29 = new CheckBox();
            e_28.Children.Add(e_29);
            e_29.Name = "e_29";
            e_29.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_29, Dock.Left);
            Binding binding_e_29_IsChecked = new Binding("Enabled");
            e_29.SetBinding(CheckBox.IsCheckedProperty, binding_e_29_IsChecked);
            // e_30 element
            TextBlock e_30 = new TextBlock();
            e_28.Children.Add(e_30);
            e_30.Name = "e_30";
            e_30.Margin = new Thickness(0F, 0F, 10F, 0F);
            e_30.VerticalAlignment = VerticalAlignment.Center;
            Binding binding_e_30_Foreground = new Binding("Color");
            e_30.SetBinding(TextBlock.ForegroundProperty, binding_e_30_Foreground);
            Binding binding_e_30_Text = new Binding("Name");
            e_30.SetBinding(TextBlock.TextProperty, binding_e_30_Text);
            return e_28;
        }
        
        private static UIElement _logListBox_dtMethod(UIElement parent) {
            // e_31 element
            Grid e_31 = new Grid();
            e_31.Parent = parent;
            e_31.Name = "e_31";
            ColumnDefinition col_e_31_0 = new ColumnDefinition();
            col_e_31_0.Width = new GridLength(45F, GridUnitType.Pixel);
            e_31.ColumnDefinitions.Add(col_e_31_0);
            ColumnDefinition col_e_31_1 = new ColumnDefinition();
            col_e_31_1.Width = new GridLength(7F, GridUnitType.Star);
            e_31.ColumnDefinitions.Add(col_e_31_1);
            // e_32 element
            TextBlock e_32 = new TextBlock();
            e_31.Children.Add(e_32);
            e_32.Name = "e_32";
            e_32.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(e_32, 0);
            Binding binding_e_32_Text = new Binding("UpdateNumber");
            e_32.SetBinding(TextBlock.TextProperty, binding_e_32_Text);
            // e_33 element
            TextBlock e_33 = new TextBlock();
            e_31.Children.Add(e_33);
            e_33.Name = "e_33";
            Grid.SetColumn(e_33, 1);
            Binding binding_e_33_Foreground = new Binding("Color");
            e_33.SetBinding(TextBlock.ForegroundProperty, binding_e_33_Foreground);
            Binding binding_e_33_Text = new Binding("Content");
            e_33.SetBinding(TextBlock.TextProperty, binding_e_33_Text);
            return e_31;
        }
    }
}

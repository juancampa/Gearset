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
    public partial class WidgetWindow : UserControl {
        
        private StackPanel e_41;
        
        private ToggleButton e_42;
        
        private ToggleButton e_43;
        
        private ToggleButton e_44;
        
        private ToggleButton e_45;
        
        private Grid e_46;
        
        private TextBlock e_47;
        
        private Slider e_48;
        
        public WidgetWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_41 element
            this.e_41 = new StackPanel();
            this.Content = this.e_41;
            this.e_41.Name = "e_41";
            this.e_41.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.e_41.Background = new SolidColorBrush(new ColorW(0, 0, 0, 153));
            this.e_41.Orientation = Orientation.Horizontal;
            // e_42 element
            this.e_42 = new ToggleButton();
            this.e_41.Children.Add(this.e_42);
            this.e_42.Name = "e_42";
            Style e_42_s = new Style(typeof(ToggleButton));
            Setter e_42_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_42_s.Setters.Add(e_42_s_S_0);
            Setter e_42_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_42_s.Setters.Add(e_42_s_S_1);
            Setter e_42_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_42_s.Setters.Add(e_42_s_S_2);
            Setter e_42_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_42_s.Setters.Add(e_42_s_S_3);
            this.e_42.Style = e_42_s;
            this.e_42.Content = "Master Switch";
            Binding binding_e_42_Command = new Binding("MasterSwitchButtonClick");
            this.e_42.SetBinding(ToggleButton.CommandProperty, binding_e_42_Command);
            Binding binding_e_42_IsChecked = new Binding("Enabled");
            this.e_42.SetBinding(ToggleButton.IsCheckedProperty, binding_e_42_IsChecked);
            // e_43 element
            this.e_43 = new ToggleButton();
            this.e_41.Children.Add(this.e_43);
            this.e_43.Name = "e_43";
            Style e_43_s = new Style(typeof(ToggleButton));
            Setter e_43_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_43_s.Setters.Add(e_43_s_S_0);
            Setter e_43_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_43_s.Setters.Add(e_43_s_S_1);
            Setter e_43_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_43_s.Setters.Add(e_43_s_S_2);
            Setter e_43_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_43_s.Setters.Add(e_43_s_S_3);
            this.e_43.Style = e_43_s;
            this.e_43.Content = "Finder";
            Binding binding_e_43_Command = new Binding("FinderButtonClick");
            this.e_43.SetBinding(ToggleButton.CommandProperty, binding_e_43_Command);
            Binding binding_e_43_IsChecked = new Binding("FinderWindowVisible");
            this.e_43.SetBinding(ToggleButton.IsCheckedProperty, binding_e_43_IsChecked);
            // e_44 element
            this.e_44 = new ToggleButton();
            this.e_41.Children.Add(this.e_44);
            this.e_44.Name = "e_44";
            Style e_44_s = new Style(typeof(ToggleButton));
            Setter e_44_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_44_s.Setters.Add(e_44_s_S_0);
            Setter e_44_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_44_s.Setters.Add(e_44_s_S_1);
            Setter e_44_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_44_s.Setters.Add(e_44_s_S_2);
            Setter e_44_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_44_s.Setters.Add(e_44_s_S_3);
            this.e_44.Style = e_44_s;
            this.e_44.Content = "Logger";
            Binding binding_e_44_Command = new Binding("LoggerButtonClick");
            this.e_44.SetBinding(ToggleButton.CommandProperty, binding_e_44_Command);
            Binding binding_e_44_IsChecked = new Binding("LoggerWindowVisible");
            this.e_44.SetBinding(ToggleButton.IsCheckedProperty, binding_e_44_IsChecked);
            // e_45 element
            this.e_45 = new ToggleButton();
            this.e_41.Children.Add(this.e_45);
            this.e_45.Name = "e_45";
            Style e_45_s = new Style(typeof(ToggleButton));
            Setter e_45_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_45_s.Setters.Add(e_45_s_S_0);
            Setter e_45_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_45_s.Setters.Add(e_45_s_S_1);
            Setter e_45_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_45_s.Setters.Add(e_45_s_S_2);
            Setter e_45_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_45_s.Setters.Add(e_45_s_S_3);
            this.e_45.Style = e_45_s;
            this.e_45.Content = "Profiler";
            Binding binding_e_45_Command = new Binding("ProfilerButtonClick");
            this.e_45.SetBinding(ToggleButton.CommandProperty, binding_e_45_Command);
            Binding binding_e_45_IsChecked = new Binding("ProfilerWindowVisible");
            this.e_45.SetBinding(ToggleButton.IsCheckedProperty, binding_e_45_IsChecked);
            // e_46 element
            this.e_46 = new Grid();
            this.e_41.Children.Add(this.e_46);
            this.e_46.Name = "e_46";
            this.e_46.Background = new SolidColorBrush(new ColorW(0, 0, 0, 255));
            ColumnDefinition col_e_46_0 = new ColumnDefinition();
            this.e_46.ColumnDefinitions.Add(col_e_46_0);
            ColumnDefinition col_e_46_1 = new ColumnDefinition();
            this.e_46.ColumnDefinitions.Add(col_e_46_1);
            // e_47 element
            this.e_47 = new TextBlock();
            this.e_46.Children.Add(this.e_47);
            this.e_47.Name = "e_47";
            this.e_47.Margin = new Thickness(10F, 4F, 1F, 0F);
            this.e_47.Text = "Opacity";
            Grid.SetColumn(this.e_47, 0);
            // e_48 element
            this.e_48 = new Slider();
            this.e_46.Children.Add(this.e_48);
            this.e_48.Name = "e_48";
            this.e_48.Width = 100F;
            this.e_48.Margin = new Thickness(10F, 3F, 20F, 0F);
            Grid.SetColumn(this.e_48, 1);
            Binding binding_e_48_Value = new Binding("SliderValue");
            this.e_48.SetBinding(Slider.ValueProperty, binding_e_48_Value);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
            // Resource - [toggleButton] Style
            Style r_0_s = new Style(typeof(ToggleButton));
            Setter r_0_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            r_0_s.Setters.Add(r_0_s_S_0);
            Setter r_0_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            r_0_s.Setters.Add(r_0_s_S_1);
            Setter r_0_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            r_0_s.Setters.Add(r_0_s_S_2);
            Setter r_0_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            r_0_s.Setters.Add(r_0_s_S_3);
            elem.Resources.Add("toggleButton", r_0_s);
        }
    }
}

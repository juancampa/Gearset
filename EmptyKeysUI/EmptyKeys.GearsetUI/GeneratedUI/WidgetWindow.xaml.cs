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
        
        private StackPanel e_50;
        
        private ToggleButton e_51;
        
        private ToggleButton e_52;
        
        private ToggleButton e_53;
        
        private ToggleButton e_54;
        
        private ToggleButton e_55;
        
        private Grid e_56;
        
        private TextBlock e_57;
        
        private Slider e_58;
        
        public WidgetWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_50 element
            this.e_50 = new StackPanel();
            this.Content = this.e_50;
            this.e_50.Name = "e_50";
            this.e_50.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.e_50.Background = new SolidColorBrush(new ColorW(0, 0, 0, 153));
            this.e_50.Orientation = Orientation.Horizontal;
            // e_51 element
            this.e_51 = new ToggleButton();
            this.e_50.Children.Add(this.e_51);
            this.e_51.Name = "e_51";
            Style e_51_s = new Style(typeof(ToggleButton));
            Setter e_51_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_51_s.Setters.Add(e_51_s_S_0);
            Setter e_51_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_51_s.Setters.Add(e_51_s_S_1);
            Setter e_51_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_51_s.Setters.Add(e_51_s_S_2);
            Setter e_51_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_51_s.Setters.Add(e_51_s_S_3);
            this.e_51.Style = e_51_s;
            this.e_51.Content = "Master Switch";
            Binding binding_e_51_Command = new Binding("MasterSwitchButtonClick");
            this.e_51.SetBinding(ToggleButton.CommandProperty, binding_e_51_Command);
            Binding binding_e_51_IsChecked = new Binding("Enabled");
            this.e_51.SetBinding(ToggleButton.IsCheckedProperty, binding_e_51_IsChecked);
            // e_52 element
            this.e_52 = new ToggleButton();
            this.e_50.Children.Add(this.e_52);
            this.e_52.Name = "e_52";
            Style e_52_s = new Style(typeof(ToggleButton));
            Setter e_52_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_52_s.Setters.Add(e_52_s_S_0);
            Setter e_52_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_52_s.Setters.Add(e_52_s_S_1);
            Setter e_52_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_52_s.Setters.Add(e_52_s_S_2);
            Setter e_52_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_52_s.Setters.Add(e_52_s_S_3);
            this.e_52.Style = e_52_s;
            this.e_52.Content = "Finder";
            Binding binding_e_52_Command = new Binding("FinderButtonClick");
            this.e_52.SetBinding(ToggleButton.CommandProperty, binding_e_52_Command);
            Binding binding_e_52_IsChecked = new Binding("FinderWindowVisible");
            this.e_52.SetBinding(ToggleButton.IsCheckedProperty, binding_e_52_IsChecked);
            // e_53 element
            this.e_53 = new ToggleButton();
            this.e_50.Children.Add(this.e_53);
            this.e_53.Name = "e_53";
            Style e_53_s = new Style(typeof(ToggleButton));
            Setter e_53_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_53_s.Setters.Add(e_53_s_S_0);
            Setter e_53_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_53_s.Setters.Add(e_53_s_S_1);
            Setter e_53_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_53_s.Setters.Add(e_53_s_S_2);
            Setter e_53_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_53_s.Setters.Add(e_53_s_S_3);
            this.e_53.Style = e_53_s;
            this.e_53.Content = "Logger";
            Binding binding_e_53_Command = new Binding("LoggerButtonClick");
            this.e_53.SetBinding(ToggleButton.CommandProperty, binding_e_53_Command);
            Binding binding_e_53_IsChecked = new Binding("LoggerWindowVisible");
            this.e_53.SetBinding(ToggleButton.IsCheckedProperty, binding_e_53_IsChecked);
            // e_54 element
            this.e_54 = new ToggleButton();
            this.e_50.Children.Add(this.e_54);
            this.e_54.Name = "e_54";
            Style e_54_s = new Style(typeof(ToggleButton));
            Setter e_54_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_54_s.Setters.Add(e_54_s_S_0);
            Setter e_54_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_54_s.Setters.Add(e_54_s_S_1);
            Setter e_54_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_54_s.Setters.Add(e_54_s_S_2);
            Setter e_54_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_54_s.Setters.Add(e_54_s_S_3);
            this.e_54.Style = e_54_s;
            this.e_54.Content = "Profiler";
            Binding binding_e_54_Command = new Binding("ProfilerButtonClick");
            this.e_54.SetBinding(ToggleButton.CommandProperty, binding_e_54_Command);
            Binding binding_e_54_IsChecked = new Binding("ProfilerWindowVisible");
            this.e_54.SetBinding(ToggleButton.IsCheckedProperty, binding_e_54_IsChecked);
            // e_55 element
            this.e_55 = new ToggleButton();
            this.e_50.Children.Add(this.e_55);
            this.e_55.Name = "e_55";
            Style e_55_s = new Style(typeof(ToggleButton));
            Setter e_55_s_S_0 = new Setter(ToggleButton.PaddingProperty, new Thickness(5F, 0F, 1F, 0F));
            e_55_s.Setters.Add(e_55_s_S_0);
            Setter e_55_s_S_1 = new Setter(ToggleButton.VerticalAlignmentProperty, VerticalAlignment.Top);
            e_55_s.Setters.Add(e_55_s_S_1);
            Setter e_55_s_S_2 = new Setter(ToggleButton.HeightProperty, 24F);
            e_55_s.Setters.Add(e_55_s_S_2);
            Setter e_55_s_S_3 = new Setter(ToggleButton.IsThreeStateProperty, false);
            e_55_s.Setters.Add(e_55_s_S_3);
            this.e_55.Style = e_55_s;
            this.e_55.Content = "Console";
            Binding binding_e_55_Command = new Binding("CommandConsoleButtonClick");
            this.e_55.SetBinding(ToggleButton.CommandProperty, binding_e_55_Command);
            Binding binding_e_55_IsChecked = new Binding("CommandConsoleWindowVisible");
            this.e_55.SetBinding(ToggleButton.IsCheckedProperty, binding_e_55_IsChecked);
            // e_56 element
            this.e_56 = new Grid();
            this.e_50.Children.Add(this.e_56);
            this.e_56.Name = "e_56";
            this.e_56.Background = new SolidColorBrush(new ColorW(0, 0, 0, 255));
            ColumnDefinition col_e_56_0 = new ColumnDefinition();
            this.e_56.ColumnDefinitions.Add(col_e_56_0);
            ColumnDefinition col_e_56_1 = new ColumnDefinition();
            this.e_56.ColumnDefinitions.Add(col_e_56_1);
            // e_57 element
            this.e_57 = new TextBlock();
            this.e_56.Children.Add(this.e_57);
            this.e_57.Name = "e_57";
            this.e_57.Margin = new Thickness(10F, 4F, 1F, 0F);
            this.e_57.Text = "Opacity";
            Grid.SetColumn(this.e_57, 0);
            // e_58 element
            this.e_58 = new Slider();
            this.e_56.Children.Add(this.e_58);
            this.e_58.Name = "e_58";
            this.e_58.Width = 100F;
            this.e_58.Margin = new Thickness(10F, 3F, 20F, 0F);
            Grid.SetColumn(this.e_58, 1);
            Binding binding_e_58_Value = new Binding("SliderValue");
            this.e_58.SetBinding(Slider.ValueProperty, binding_e_58_Value);
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

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
    public partial class ProfilerWindow : UserControl {
        
        private StackPanel e_37;
        
        private ToggleButton e_38;
        
        private ToggleButton e_39;
        
        private ToggleButton e_40;
        
        private ItemsControl pgLevelsListBox;
        
        private ItemsControl trLevelsListBox;
        
        private ItemsControl psLevelsListBox;
        
        public ProfilerWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_37 element
            this.e_37 = new StackPanel();
            this.Content = this.e_37;
            this.e_37.Name = "e_37";
            this.e_37.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            // e_38 element
            this.e_38 = new ToggleButton();
            this.e_37.Children.Add(this.e_38);
            this.e_38.Name = "e_38";
            this.e_38.Content = "Performance Graph";
            Binding binding_e_38_Command = new Binding("PerformanceGraphButtonClick");
            this.e_38.SetBinding(ToggleButton.CommandProperty, binding_e_38_Command);
            Binding binding_e_38_IsChecked = new Binding("PerformanceGraphActive");
            this.e_38.SetBinding(ToggleButton.IsCheckedProperty, binding_e_38_IsChecked);
            // e_39 element
            this.e_39 = new ToggleButton();
            this.e_37.Children.Add(this.e_39);
            this.e_39.Name = "e_39";
            this.e_39.Content = "Time Ruler";
            Binding binding_e_39_Command = new Binding("TimeRulerGraphButtonClick");
            this.e_39.SetBinding(ToggleButton.CommandProperty, binding_e_39_Command);
            Binding binding_e_39_IsChecked = new Binding("TimeRulerActive");
            this.e_39.SetBinding(ToggleButton.IsCheckedProperty, binding_e_39_IsChecked);
            // e_40 element
            this.e_40 = new ToggleButton();
            this.e_37.Children.Add(this.e_40);
            this.e_40.Name = "e_40";
            this.e_40.Content = "Summary Log";
            Binding binding_e_40_Command = new Binding("SummaryLogButtonClick");
            this.e_40.SetBinding(ToggleButton.CommandProperty, binding_e_40_Command);
            Binding binding_e_40_IsChecked = new Binding("SummaryLogActive");
            this.e_40.SetBinding(ToggleButton.IsCheckedProperty, binding_e_40_IsChecked);
            // pgLevelsListBox element
            this.pgLevelsListBox = new ItemsControl();
            this.e_37.Children.Add(this.pgLevelsListBox);
            this.pgLevelsListBox.Name = "pgLevelsListBox";
            this.pgLevelsListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.pgLevelsListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> pgLevelsListBox_dtFunc = pgLevelsListBox_dtMethod;
            this.pgLevelsListBox.ItemTemplate = new DataTemplate(pgLevelsListBox_dtFunc);
            Binding binding_pgLevelsListBox_Visibility = new Binding("PerformanceGraphVisibility");
            this.pgLevelsListBox.SetBinding(ItemsControl.VisibilityProperty, binding_pgLevelsListBox_Visibility);
            Binding binding_pgLevelsListBox_ItemsSource = new Binding("PgLevels");
            this.pgLevelsListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding_pgLevelsListBox_ItemsSource);
            // trLevelsListBox element
            this.trLevelsListBox = new ItemsControl();
            this.e_37.Children.Add(this.trLevelsListBox);
            this.trLevelsListBox.Name = "trLevelsListBox";
            this.trLevelsListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.trLevelsListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> trLevelsListBox_dtFunc = trLevelsListBox_dtMethod;
            this.trLevelsListBox.ItemTemplate = new DataTemplate(trLevelsListBox_dtFunc);
            Binding binding_trLevelsListBox_Visibility = new Binding("TimeRulerVisibility");
            this.trLevelsListBox.SetBinding(ItemsControl.VisibilityProperty, binding_trLevelsListBox_Visibility);
            Binding binding_trLevelsListBox_ItemsSource = new Binding("TrLevels");
            this.trLevelsListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding_trLevelsListBox_ItemsSource);
            // psLevelsListBox element
            this.psLevelsListBox = new ItemsControl();
            this.e_37.Children.Add(this.psLevelsListBox);
            this.psLevelsListBox.Name = "psLevelsListBox";
            this.psLevelsListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.psLevelsListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> psLevelsListBox_dtFunc = psLevelsListBox_dtMethod;
            this.psLevelsListBox.ItemTemplate = new DataTemplate(psLevelsListBox_dtFunc);
            Binding binding_psLevelsListBox_Visibility = new Binding("SummaryLogVisibility");
            this.psLevelsListBox.SetBinding(ItemsControl.VisibilityProperty, binding_psLevelsListBox_Visibility);
            Binding binding_psLevelsListBox_ItemsSource = new Binding("PsLevels");
            this.psLevelsListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding_psLevelsListBox_ItemsSource);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
            // Resource - [levelTemplate] DataTemplate
            Func<UIElement, UIElement> r_0_dtFunc = r_0_dtMethod;
            elem.Resources.Add("levelTemplate", new DataTemplate(r_0_dtFunc));
        }
        
        private static UIElement r_0_dtMethod(UIElement parent) {
            // e_34 element
            DockPanel e_34 = new DockPanel();
            e_34.Parent = parent;
            e_34.Name = "e_34";
            // e_35 element
            CheckBox e_35 = new CheckBox();
            e_34.Children.Add(e_35);
            e_35.Name = "e_35";
            e_35.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_35, Dock.Left);
            Binding binding_e_35_IsChecked = new Binding("Enabled");
            e_35.SetBinding(CheckBox.IsCheckedProperty, binding_e_35_IsChecked);
            // e_36 element
            TextBlock e_36 = new TextBlock();
            e_34.Children.Add(e_36);
            e_36.Name = "e_36";
            e_36.Padding = new Thickness(0F, 3F, 0F, 0F);
            Binding binding_e_36_Text = new Binding("Name");
            e_36.SetBinding(TextBlock.TextProperty, binding_e_36_Text);
            return e_34;
        }
        
        private static UIElement pgLevelsListBox_dtMethod(UIElement parent) {
            // e_41 element
            DockPanel e_41 = new DockPanel();
            e_41.Parent = parent;
            e_41.Name = "e_41";
            // e_42 element
            CheckBox e_42 = new CheckBox();
            e_41.Children.Add(e_42);
            e_42.Name = "e_42";
            e_42.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_42, Dock.Left);
            Binding binding_e_42_IsChecked = new Binding("Enabled");
            e_42.SetBinding(CheckBox.IsCheckedProperty, binding_e_42_IsChecked);
            // e_43 element
            TextBlock e_43 = new TextBlock();
            e_41.Children.Add(e_43);
            e_43.Name = "e_43";
            e_43.Padding = new Thickness(0F, 3F, 0F, 0F);
            Binding binding_e_43_Text = new Binding("Name");
            e_43.SetBinding(TextBlock.TextProperty, binding_e_43_Text);
            return e_41;
        }
        
        private static UIElement trLevelsListBox_dtMethod(UIElement parent) {
            // e_44 element
            DockPanel e_44 = new DockPanel();
            e_44.Parent = parent;
            e_44.Name = "e_44";
            // e_45 element
            CheckBox e_45 = new CheckBox();
            e_44.Children.Add(e_45);
            e_45.Name = "e_45";
            e_45.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_45, Dock.Left);
            Binding binding_e_45_IsChecked = new Binding("Enabled");
            e_45.SetBinding(CheckBox.IsCheckedProperty, binding_e_45_IsChecked);
            // e_46 element
            TextBlock e_46 = new TextBlock();
            e_44.Children.Add(e_46);
            e_46.Name = "e_46";
            e_46.Padding = new Thickness(0F, 3F, 0F, 0F);
            Binding binding_e_46_Text = new Binding("Name");
            e_46.SetBinding(TextBlock.TextProperty, binding_e_46_Text);
            return e_44;
        }
        
        private static UIElement psLevelsListBox_dtMethod(UIElement parent) {
            // e_47 element
            DockPanel e_47 = new DockPanel();
            e_47.Parent = parent;
            e_47.Name = "e_47";
            // e_48 element
            CheckBox e_48 = new CheckBox();
            e_47.Children.Add(e_48);
            e_48.Name = "e_48";
            e_48.Margin = new Thickness(0F, 0F, 5F, 0F);
            DockPanel.SetDock(e_48, Dock.Left);
            Binding binding_e_48_IsChecked = new Binding("Enabled");
            e_48.SetBinding(CheckBox.IsCheckedProperty, binding_e_48_IsChecked);
            // e_49 element
            TextBlock e_49 = new TextBlock();
            e_47.Children.Add(e_49);
            e_49.Name = "e_49";
            e_49.Padding = new Thickness(0F, 3F, 0F, 0F);
            Binding binding_e_49_Text = new Binding("Name");
            e_49.SetBinding(TextBlock.TextProperty, binding_e_49_Text);
            return e_47;
        }
    }
}

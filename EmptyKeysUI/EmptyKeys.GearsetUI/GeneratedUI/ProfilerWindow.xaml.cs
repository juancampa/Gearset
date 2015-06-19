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
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "1.7.0.0")]
    public partial class ProfilerWindow : UserControl {
        
        private StackPanel e_28;
        
        private ToggleButton e_29;
        
        private ToggleButton e_30;
        
        private ToggleButton e_31;
        
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
            // e_28 element
            this.e_28 = new StackPanel();
            this.Content = this.e_28;
            this.e_28.Name = "e_28";
            this.e_28.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            // e_29 element
            this.e_29 = new ToggleButton();
            this.e_28.Children.Add(this.e_29);
            this.e_29.Name = "e_29";
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            this.e_29.Content = "Performance Graph";
            Binding binding_e_29_Command = new Binding("PerformanceGraphButtonClick");
            this.e_29.SetBinding(ToggleButton.CommandProperty, binding_e_29_Command);
            Binding binding_e_29_IsChecked = new Binding("PerformanceGraphActive");
            this.e_29.SetBinding(ToggleButton.IsCheckedProperty, binding_e_29_IsChecked);
            // e_30 element
            this.e_30 = new ToggleButton();
            this.e_28.Children.Add(this.e_30);
            this.e_30.Name = "e_30";
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            this.e_30.Content = "Time Ruler";
            Binding binding_e_30_Command = new Binding("TimeRulerGraphButtonClick");
            this.e_30.SetBinding(ToggleButton.CommandProperty, binding_e_30_Command);
            Binding binding_e_30_IsChecked = new Binding("TimeRulerActive");
            this.e_30.SetBinding(ToggleButton.IsCheckedProperty, binding_e_30_IsChecked);
            // e_31 element
            this.e_31 = new ToggleButton();
            this.e_28.Children.Add(this.e_31);
            this.e_31.Name = "e_31";
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            this.e_31.Content = "Summary Log";
            Binding binding_e_31_Command = new Binding("SummaryLogButtonClick");
            this.e_31.SetBinding(ToggleButton.CommandProperty, binding_e_31_Command);
            Binding binding_e_31_IsChecked = new Binding("SummaryLogActive");
            this.e_31.SetBinding(ToggleButton.IsCheckedProperty, binding_e_31_IsChecked);
            // pgLevelsListBox element
            this.pgLevelsListBox = new ItemsControl();
            this.e_28.Children.Add(this.pgLevelsListBox);
            this.pgLevelsListBox.Name = "pgLevelsListBox";
            this.pgLevelsListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.pgLevelsListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Func<UIElement, UIElement> pgLevelsListBox_dtFunc = pgLevelsListBox_dtMethod;
            this.pgLevelsListBox.ItemTemplate = new DataTemplate(pgLevelsListBox_dtFunc);
            Binding binding_pgLevelsListBox_Visibility = new Binding("PerformanceGraphVisibility");
            this.pgLevelsListBox.SetBinding(ItemsControl.VisibilityProperty, binding_pgLevelsListBox_Visibility);
            Binding binding_pgLevelsListBox_ItemsSource = new Binding("PgLevels");
            this.pgLevelsListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding_pgLevelsListBox_ItemsSource);
            // trLevelsListBox element
            this.trLevelsListBox = new ItemsControl();
            this.e_28.Children.Add(this.trLevelsListBox);
            this.trLevelsListBox.Name = "trLevelsListBox";
            this.trLevelsListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.trLevelsListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Func<UIElement, UIElement> trLevelsListBox_dtFunc = trLevelsListBox_dtMethod;
            this.trLevelsListBox.ItemTemplate = new DataTemplate(trLevelsListBox_dtFunc);
            Binding binding_trLevelsListBox_Visibility = new Binding("TimeRulerVisibility");
            this.trLevelsListBox.SetBinding(ItemsControl.VisibilityProperty, binding_trLevelsListBox_Visibility);
            Binding binding_trLevelsListBox_ItemsSource = new Binding("TrLevels");
            this.trLevelsListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding_trLevelsListBox_ItemsSource);
            // psLevelsListBox element
            this.psLevelsListBox = new ItemsControl();
            this.e_28.Children.Add(this.psLevelsListBox);
            this.psLevelsListBox.Name = "psLevelsListBox";
            this.psLevelsListBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this.psLevelsListBox.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Func<UIElement, UIElement> psLevelsListBox_dtFunc = psLevelsListBox_dtMethod;
            this.psLevelsListBox.ItemTemplate = new DataTemplate(psLevelsListBox_dtFunc);
            Binding binding_psLevelsListBox_Visibility = new Binding("SummaryLogVisibility");
            this.psLevelsListBox.SetBinding(ItemsControl.VisibilityProperty, binding_psLevelsListBox_Visibility);
            Binding binding_psLevelsListBox_ItemsSource = new Binding("PsLevels");
            this.psLevelsListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding_psLevelsListBox_ItemsSource);
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
            // Resource - [levelTemplate] DataTemplate
            Func<UIElement, UIElement> r_0_dtFunc = r_0_dtMethod;
            elem.Resources.Add("levelTemplate", new DataTemplate(r_0_dtFunc));
        }
        
        private static UIElement r_0_dtMethod(UIElement parent) {
            // e_25 element
            DockPanel e_25 = new DockPanel();
            e_25.Parent = parent;
            e_25.Name = "e_25";
            // e_26 element
            CheckBox e_26 = new CheckBox();
            e_25.Children.Add(e_26);
            e_26.Name = "e_26";
            e_26.Margin = new Thickness(0F, 0F, 5F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            DockPanel.SetDock(e_26, Dock.Left);
            Binding binding_e_26_IsChecked = new Binding("Enabled");
            e_26.SetBinding(CheckBox.IsCheckedProperty, binding_e_26_IsChecked);
            // e_27 element
            TextBlock e_27 = new TextBlock();
            e_25.Children.Add(e_27);
            e_27.Name = "e_27";
            e_27.Padding = new Thickness(0F, 3F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Binding binding_e_27_Text = new Binding("Name");
            e_27.SetBinding(TextBlock.TextProperty, binding_e_27_Text);
            return e_25;
        }
        
        private static UIElement pgLevelsListBox_dtMethod(UIElement parent) {
            // e_32 element
            DockPanel e_32 = new DockPanel();
            e_32.Parent = parent;
            e_32.Name = "e_32";
            // e_33 element
            CheckBox e_33 = new CheckBox();
            e_32.Children.Add(e_33);
            e_33.Name = "e_33";
            e_33.Margin = new Thickness(0F, 0F, 5F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            DockPanel.SetDock(e_33, Dock.Left);
            Binding binding_e_33_IsChecked = new Binding("Enabled");
            e_33.SetBinding(CheckBox.IsCheckedProperty, binding_e_33_IsChecked);
            // e_34 element
            TextBlock e_34 = new TextBlock();
            e_32.Children.Add(e_34);
            e_34.Name = "e_34";
            e_34.Padding = new Thickness(0F, 3F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Binding binding_e_34_Text = new Binding("Name");
            e_34.SetBinding(TextBlock.TextProperty, binding_e_34_Text);
            return e_32;
        }
        
        private static UIElement trLevelsListBox_dtMethod(UIElement parent) {
            // e_35 element
            DockPanel e_35 = new DockPanel();
            e_35.Parent = parent;
            e_35.Name = "e_35";
            // e_36 element
            CheckBox e_36 = new CheckBox();
            e_35.Children.Add(e_36);
            e_36.Name = "e_36";
            e_36.Margin = new Thickness(0F, 0F, 5F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            DockPanel.SetDock(e_36, Dock.Left);
            Binding binding_e_36_IsChecked = new Binding("Enabled");
            e_36.SetBinding(CheckBox.IsCheckedProperty, binding_e_36_IsChecked);
            // e_37 element
            TextBlock e_37 = new TextBlock();
            e_35.Children.Add(e_37);
            e_37.Name = "e_37";
            e_37.Padding = new Thickness(0F, 3F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Binding binding_e_37_Text = new Binding("Name");
            e_37.SetBinding(TextBlock.TextProperty, binding_e_37_Text);
            return e_35;
        }
        
        private static UIElement psLevelsListBox_dtMethod(UIElement parent) {
            // e_38 element
            DockPanel e_38 = new DockPanel();
            e_38.Parent = parent;
            e_38.Name = "e_38";
            // e_39 element
            CheckBox e_39 = new CheckBox();
            e_38.Children.Add(e_39);
            e_39.Name = "e_39";
            e_39.Margin = new Thickness(0F, 0F, 5F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            DockPanel.SetDock(e_39, Dock.Left);
            Binding binding_e_39_IsChecked = new Binding("Enabled");
            e_39.SetBinding(CheckBox.IsCheckedProperty, binding_e_39_IsChecked);
            // e_40 element
            TextBlock e_40 = new TextBlock();
            e_38.Children.Add(e_40);
            e_40.Name = "e_40";
            e_40.Padding = new Thickness(0F, 3F, 0F, 0F);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
            Binding binding_e_40_Text = new Binding("Name");
            e_40.SetBinding(TextBlock.TextProperty, binding_e_40_Text);
            return e_38;
        }
    }
}

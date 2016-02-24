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
    public partial class CommandConsoleWindow : UserControl {
        
        private Grid e_2;
        
        private DockPanel e_3;
        
        private ScrollViewer _scrollViewer;
        
        private ItemsControl _outputListView;
        
        private Grid e_6;
        
        private TextBlock e_7;
        
        private TextBox _commandTextBox;
        
        private Button e_8;
        
        public CommandConsoleWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_2 element
            this.e_2 = new Grid();
            this.Content = this.e_2;
            this.e_2.Name = "e_2";
            this.e_2.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            RowDefinition row_e_2_0 = new RowDefinition();
            row_e_2_0.Height = new GridLength(20F, GridUnitType.Pixel);
            this.e_2.RowDefinitions.Add(row_e_2_0);
            RowDefinition row_e_2_1 = new RowDefinition();
            this.e_2.RowDefinitions.Add(row_e_2_1);
            RowDefinition row_e_2_2 = new RowDefinition();
            row_e_2_2.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_2.RowDefinitions.Add(row_e_2_2);
            // e_3 element
            this.e_3 = new DockPanel();
            this.e_2.Children.Add(this.e_3);
            this.e_3.Name = "e_3";
            Grid.SetRow(this.e_3, 1);
            // _scrollViewer element
            this._scrollViewer = new ScrollViewer();
            this.e_3.Children.Add(this._scrollViewer);
            this._scrollViewer.Name = "_scrollViewer";
            // _outputListView element
            this._outputListView = new ItemsControl();
            this._scrollViewer.Content = this._outputListView;
            this._outputListView.Name = "_outputListView";
            this._outputListView.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this._outputListView.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> _outputListView_dtFunc = _outputListView_dtMethod;
            this._outputListView.ItemTemplate = new DataTemplate(_outputListView_dtFunc);
            Binding binding__outputListView_ItemsSource = new Binding("Output");
            this._outputListView.SetBinding(ItemsControl.ItemsSourceProperty, binding__outputListView_ItemsSource);
            // e_6 element
            this.e_6 = new Grid();
            this.e_2.Children.Add(this.e_6);
            this.e_6.Name = "e_6";
            this.e_6.Margin = new Thickness(4F, 4F, 4F, 4F);
            this.e_6.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(this.e_6, 2);
            // e_7 element
            this.e_7 = new TextBlock();
            this.e_6.Children.Add(this.e_7);
            this.e_7.Name = "e_7";
            this.e_7.Height = 30F;
            this.e_7.Margin = new Thickness(4F, 0F, 0F, 0F);
            this.e_7.VerticalAlignment = VerticalAlignment.Center;
            this.e_7.Foreground = new SolidColorBrush(new ColorW(119, 119, 119, 255));
            this.e_7.Text = "Type Commands Here";
            this.e_7.Padding = new Thickness(0F, 7F, 0F, 7F);
            Binding binding_e_7_Visibility = new Binding("CommandTextEmpty");
            this.e_7.SetBinding(TextBlock.VisibilityProperty, binding_e_7_Visibility);
            // _commandTextBox element
            this._commandTextBox = new TextBox();
            this.e_6.Children.Add(this._commandTextBox);
            this._commandTextBox.Name = "_commandTextBox";
            this._commandTextBox.Height = 30F;
            this._commandTextBox.Margin = new Thickness(0F, 0F, 55F, 0F);
            this._commandTextBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this._commandTextBox.BorderThickness = new Thickness(1F, 1F, 1F, 1F);
            this._commandTextBox.CaretBrush = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding__commandTextBox_Text = new Binding("CommandText");
            this._commandTextBox.SetBinding(TextBox.TextProperty, binding__commandTextBox_Text);
            // e_8 element
            this.e_8 = new Button();
            this.e_6.Children.Add(this.e_8);
            this.e_8.Name = "e_8";
            this.e_8.Width = 50F;
            this.e_8.HorizontalAlignment = HorizontalAlignment.Right;
            this.e_8.Content = "Execute";
            Binding binding_e_8_Command = new Binding("ExecuteButtonClick");
            this.e_8.SetBinding(Button.CommandProperty, binding_e_8_Command);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
            // Resource - [outputTemplate] DataTemplate
            Func<UIElement, UIElement> r_0_dtFunc = r_0_dtMethod;
            elem.Resources.Add("outputTemplate", new DataTemplate(r_0_dtFunc));
        }
        
        private static UIElement r_0_dtMethod(UIElement parent) {
            // e_0 element
            Grid e_0 = new Grid();
            e_0.Parent = parent;
            e_0.Name = "e_0";
            // e_1 element
            TextBlock e_1 = new TextBlock();
            e_0.Children.Add(e_1);
            e_1.Name = "e_1";
            e_1.Margin = new Thickness(4F, 0F, 0F, 0F);
            Grid.SetColumn(e_1, 0);
            Binding binding_e_1_Foreground = new Binding("Color");
            e_1.SetBinding(TextBlock.ForegroundProperty, binding_e_1_Foreground);
            Binding binding_e_1_Text = new Binding("Text");
            e_1.SetBinding(TextBlock.TextProperty, binding_e_1_Text);
            return e_0;
        }
        
        private static UIElement _outputListView_dtMethod(UIElement parent) {
            // e_4 element
            Grid e_4 = new Grid();
            e_4.Parent = parent;
            e_4.Name = "e_4";
            // e_5 element
            TextBlock e_5 = new TextBlock();
            e_4.Children.Add(e_5);
            e_5.Name = "e_5";
            e_5.Margin = new Thickness(4F, 0F, 0F, 0F);
            Grid.SetColumn(e_5, 0);
            Binding binding_e_5_Foreground = new Binding("Color");
            e_5.SetBinding(TextBlock.ForegroundProperty, binding_e_5_Foreground);
            Binding binding_e_5_Text = new Binding("Text");
            e_5.SetBinding(TextBlock.TextProperty, binding_e_5_Text);
            return e_4;
        }
    }
}

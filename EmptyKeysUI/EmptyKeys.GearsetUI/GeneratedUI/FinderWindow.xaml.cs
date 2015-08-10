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
    
    
    [GeneratedCodeAttribute("Empty Keys UI Generator", "1.8.0.0")]
    public partial class FinderWindow : UserControl {
        
        private Grid e_5;
        
        private Grid e_6;
        
        private TextBlock e_7;
        
        private TextBox _searchTextBox;
        
        private DataGrid _resultsDataGrid;
        
        public FinderWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_5 element
            this.e_5 = new Grid();
            this.Content = this.e_5;
            this.e_5.Name = "e_5";
            this.e_5.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            RowDefinition row_e_5_0 = new RowDefinition();
            row_e_5_0.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_5.RowDefinitions.Add(row_e_5_0);
            RowDefinition row_e_5_1 = new RowDefinition();
            row_e_5_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_5.RowDefinitions.Add(row_e_5_1);
            // e_6 element
            this.e_6 = new Grid();
            this.e_5.Children.Add(this.e_6);
            this.e_6.Name = "e_6";
            this.e_6.Margin = new Thickness(4F, 4F, 4F, 4F);
            this.e_6.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(this.e_6, 0);
            // e_7 element
            this.e_7 = new TextBlock();
            this.e_6.Children.Add(this.e_7);
            this.e_7.Name = "e_7";
            this.e_7.Height = 30F;
            this.e_7.Margin = new Thickness(4F, 0F, 0F, 0F);
            this.e_7.VerticalAlignment = VerticalAlignment.Center;
            this.e_7.Foreground = new SolidColorBrush(new ColorW(119, 119, 119, 255));
            this.e_7.Text = "Search finder results...";
            this.e_7.Padding = new Thickness(0F, 7F, 0F, 7F);
            Binding binding_e_7_Visibility = new Binding("SearchTextEmpty");
            this.e_7.SetBinding(TextBlock.VisibilityProperty, binding_e_7_Visibility);
            // _searchTextBox element
            this._searchTextBox = new TextBox();
            this.e_6.Children.Add(this._searchTextBox);
            this._searchTextBox.Name = "_searchTextBox";
            this._searchTextBox.Height = 30F;
            this._searchTextBox.Margin = new Thickness(0F, 0F, 0F, 0F);
            this._searchTextBox.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this._searchTextBox.BorderThickness = new Thickness(1F, 1F, 1F, 1F);
            this._searchTextBox.CaretBrush = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Grid.SetColumn(this._searchTextBox, 0);
            Binding binding__searchTextBox_Text = new Binding("SearchText");
            this._searchTextBox.SetBinding(TextBox.TextProperty, binding__searchTextBox_Text);
            // _resultsDataGrid element
            this._resultsDataGrid = new DataGrid();
            this.e_5.Children.Add(this._resultsDataGrid);
            this._resultsDataGrid.Name = "_resultsDataGrid";
            this._resultsDataGrid.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            this._resultsDataGrid.BorderThickness = new Thickness(0F, 0F, 0F, 0F);
            Func<UIElement, UIElement> _resultsDataGrid_dtFunc = _resultsDataGrid_dtMethod;
            this._resultsDataGrid.ItemTemplate = new DataTemplate(_resultsDataGrid_dtFunc);
            this._resultsDataGrid.AutoGenerateColumns = false;
            DataGridTextColumn _resultsDataGrid_Col0 = new DataGridTextColumn();
            _resultsDataGrid_Col0.Header = "Name";
            Binding _resultsDataGrid_Col0_b = new Binding("Name");
            _resultsDataGrid_Col0.Binding = _resultsDataGrid_Col0_b;
            this._resultsDataGrid.Columns.Add(_resultsDataGrid_Col0);
            DataGridTextColumn _resultsDataGrid_Col1 = new DataGridTextColumn();
            _resultsDataGrid_Col1.Header = "Description";
            Binding _resultsDataGrid_Col1_b = new Binding("Description");
            _resultsDataGrid_Col1.Binding = _resultsDataGrid_Col1_b;
            this._resultsDataGrid.Columns.Add(_resultsDataGrid_Col1);
            Grid.SetRow(this._resultsDataGrid, 1);
            Binding binding__resultsDataGrid_ItemsSource = new Binding("Items");
            this._resultsDataGrid.SetBinding(DataGrid.ItemsSourceProperty, binding__resultsDataGrid_ItemsSource);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
            // Resource - [template] DataTemplate
            Func<UIElement, UIElement> r_0_dtFunc = r_0_dtMethod;
            elem.Resources.Add("template", new DataTemplate(r_0_dtFunc));
        }
        
        private static UIElement r_0_dtMethod(UIElement parent) {
            // e_2 element
            DockPanel e_2 = new DockPanel();
            e_2.Parent = parent;
            e_2.Name = "e_2";
            // e_3 element
            TextBlock e_3 = new TextBlock();
            e_2.Children.Add(e_3);
            e_3.Name = "e_3";
            e_3.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_3_Text = new Binding("Name");
            e_3.SetBinding(TextBlock.TextProperty, binding_e_3_Text);
            // e_4 element
            TextBlock e_4 = new TextBlock();
            e_2.Children.Add(e_4);
            e_4.Name = "e_4";
            e_4.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_4_Text = new Binding("Description");
            e_4.SetBinding(TextBlock.TextProperty, binding_e_4_Text);
            return e_2;
        }
        
        private static UIElement _resultsDataGrid_dtMethod(UIElement parent) {
            // e_8 element
            DockPanel e_8 = new DockPanel();
            e_8.Parent = parent;
            e_8.Name = "e_8";
            // e_9 element
            TextBlock e_9 = new TextBlock();
            e_8.Children.Add(e_9);
            e_9.Name = "e_9";
            e_9.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_9_Text = new Binding("Name");
            e_9.SetBinding(TextBlock.TextProperty, binding_e_9_Text);
            // e_10 element
            TextBlock e_10 = new TextBlock();
            e_8.Children.Add(e_10);
            e_10.Name = "e_10";
            e_10.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_10_Text = new Binding("Description");
            e_10.SetBinding(TextBlock.TextProperty, binding_e_10_Text);
            return e_8;
        }
    }
}

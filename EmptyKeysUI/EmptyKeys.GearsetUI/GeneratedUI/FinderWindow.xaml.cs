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
    public partial class FinderWindow : UserControl {
        
        private Grid e_14;
        
        private Grid e_15;
        
        private TextBlock e_16;
        
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
            // e_14 element
            this.e_14 = new Grid();
            this.Content = this.e_14;
            this.e_14.Name = "e_14";
            this.e_14.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            RowDefinition row_e_14_0 = new RowDefinition();
            row_e_14_0.Height = new GridLength(1F, GridUnitType.Auto);
            this.e_14.RowDefinitions.Add(row_e_14_0);
            RowDefinition row_e_14_1 = new RowDefinition();
            row_e_14_1.Height = new GridLength(1F, GridUnitType.Star);
            this.e_14.RowDefinitions.Add(row_e_14_1);
            // e_15 element
            this.e_15 = new Grid();
            this.e_14.Children.Add(this.e_15);
            this.e_15.Name = "e_15";
            this.e_15.Margin = new Thickness(4F, 4F, 4F, 4F);
            this.e_15.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(this.e_15, 0);
            // e_16 element
            this.e_16 = new TextBlock();
            this.e_15.Children.Add(this.e_16);
            this.e_16.Name = "e_16";
            this.e_16.Height = 30F;
            this.e_16.Margin = new Thickness(4F, 0F, 0F, 0F);
            this.e_16.VerticalAlignment = VerticalAlignment.Center;
            this.e_16.Foreground = new SolidColorBrush(new ColorW(119, 119, 119, 255));
            this.e_16.Text = "Search finder results...";
            this.e_16.Padding = new Thickness(0F, 7F, 0F, 7F);
            Binding binding_e_16_Visibility = new Binding("SearchTextEmpty");
            this.e_16.SetBinding(TextBlock.VisibilityProperty, binding_e_16_Visibility);
            // _searchTextBox element
            this._searchTextBox = new TextBox();
            this.e_15.Children.Add(this._searchTextBox);
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
            this.e_14.Children.Add(this._resultsDataGrid);
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
            // e_11 element
            DockPanel e_11 = new DockPanel();
            e_11.Parent = parent;
            e_11.Name = "e_11";
            // e_12 element
            TextBlock e_12 = new TextBlock();
            e_11.Children.Add(e_12);
            e_12.Name = "e_12";
            e_12.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_12_Text = new Binding("Name");
            e_12.SetBinding(TextBlock.TextProperty, binding_e_12_Text);
            // e_13 element
            TextBlock e_13 = new TextBlock();
            e_11.Children.Add(e_13);
            e_13.Name = "e_13";
            e_13.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_13_Text = new Binding("Description");
            e_13.SetBinding(TextBlock.TextProperty, binding_e_13_Text);
            return e_11;
        }
        
        private static UIElement _resultsDataGrid_dtMethod(UIElement parent) {
            // e_17 element
            DockPanel e_17 = new DockPanel();
            e_17.Parent = parent;
            e_17.Name = "e_17";
            // e_18 element
            TextBlock e_18 = new TextBlock();
            e_17.Children.Add(e_18);
            e_18.Name = "e_18";
            e_18.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_18_Text = new Binding("Name");
            e_18.SetBinding(TextBlock.TextProperty, binding_e_18_Text);
            // e_19 element
            TextBlock e_19 = new TextBlock();
            e_17.Children.Add(e_19);
            e_19.Name = "e_19";
            e_19.Foreground = new SolidColorBrush(new ColorW(221, 221, 221, 255));
            Binding binding_e_19_Text = new Binding("Description");
            e_19.SetBinding(TextBlock.TextProperty, binding_e_19_Text);
            return e_17;
        }
    }
}

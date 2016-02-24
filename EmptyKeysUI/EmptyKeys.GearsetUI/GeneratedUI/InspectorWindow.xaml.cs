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
    public partial class InspectorWindow : UserControl {
        
        private StackPanel e_20;
        
        private TreeView _treeView;
        
        public InspectorWindow() {
            Style style = UserControlStyle.CreateUserControlStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            InitializeElementResources(this);
            // e_20 element
            this.e_20 = new StackPanel();
            this.Content = this.e_20;
            this.e_20.Name = "e_20";
            this.e_20.Background = new SolidColorBrush(new ColorW(0, 0, 0, 0));
            // _treeView element
            this._treeView = new TreeView();
            this.e_20.Children.Add(this._treeView);
            this._treeView.Name = "_treeView";
            this._treeView.VerticalAlignment = VerticalAlignment.Stretch;
            Binding binding__treeView_ItemsSource = new Binding("TreeItems");
            this._treeView.SetBinding(TreeView.ItemsSourceProperty, binding__treeView_ItemsSource);
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
        }
    }
}

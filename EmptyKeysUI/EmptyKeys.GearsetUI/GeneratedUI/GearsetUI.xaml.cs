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
    public partial class GearsetUI : UIRoot {
        
        private StackPanel _stackPanel;
        
        public GearsetUI(int width, int height) : 
                base(width, height) {
            Style style = RootStyle.CreateRootStyle();
            style.TargetType = this.GetType();
            this.Style = style;
            this.InitializeComponent();
        }
        
        private void InitializeComponent() {
            this.SetResourceReference(SoundManager.SoundsProperty, "Sounds");
            InitializeElementResources(this);
            // _stackPanel element
            this._stackPanel = new StackPanel();
            this.Content = this._stackPanel;
            this._stackPanel.Name = "_stackPanel";
            this._stackPanel.Background = new SolidColorBrush(new ColorW(255, 255, 255, 0));
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static void InitializeElementResources(UIElement elem) {
            elem.Resources.MergedDictionaries.Add(CommonStyle.Instance);
        }
    }
}

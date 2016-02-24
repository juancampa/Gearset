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
    public sealed class CommonStyle : ResourceDictionary {
        
        private static CommonStyle singleton = new CommonStyle();
        
        public CommonStyle() {
            this.InitializeResources();
        }
        
        public static CommonStyle Instance {
            get {
                return singleton;
            }
        }
        
        private void InitializeResources() {
            // Resource - [textSelectionHighlight1] SolidColorBrush
            this.Add("textSelectionHighlight1", new SolidColorBrush(new ColorW(68, 68, 68, 255)));
            // Resource - [highlightTextColor] Color
            this.Add("highlightTextColor", new ColorW(17, 17, 17, 255));
            // Resource - [alertColor] Color
            this.Add("alertColor", new ColorW(187, 68, 68, 255));
            // Resource - [Sounds] SoundSourceCollection
            var r_3_sounds = new SoundSourceCollection();
            r_3_sounds.Add(new SoundSource { SoundType = SoundType.ButtonsClick, SoundAsset = "Click" });
            SoundManager.Instance.AddSound("Click");
            r_3_sounds.Add(new SoundSource { SoundType = SoundType.TextBoxKeyPress, SoundAsset = "KeyPress" });
            SoundManager.Instance.AddSound("KeyPress");
            r_3_sounds.Add(new SoundSource { SoundType = SoundType.TabControlMove, SoundAsset = "Move" });
            SoundManager.Instance.AddSound("Move");
            r_3_sounds.Add(new SoundSource { SoundType = SoundType.TabControlSelect, SoundAsset = "Select" });
            SoundManager.Instance.AddSound("Select");
            this.Add("Sounds", r_3_sounds);
            // Resource - [textSelectionHighlightColor] Color
            this.Add("textSelectionHighlightColor", new ColorW(68, 68, 68, 255));
            // Resource - [hyperlink1] SolidColorBrush
            this.Add("hyperlink1", new SolidColorBrush(new ColorW(85, 153, 255, 255)));
            // Resource - [caretBrush] SolidColorBrush
            this.Add("caretBrush", new SolidColorBrush(new ColorW(221, 221, 221, 255)));
            // Resource - [border1] SolidColorBrush
            this.Add("border1", new SolidColorBrush(new ColorW(85, 85, 85, 255)));
            // Resource - [highlight1] SolidColorBrush
            this.Add("highlight1", new SolidColorBrush(new ColorW(138, 198, 49, 255)));
            // Resource - [highlightText1] SolidColorBrush
            this.Add("highlightText1", new SolidColorBrush(new ColorW(17, 17, 17, 255)));
            // Resource - [DataTemplateKey(EmptyKeys.GearsetModel.TreeNode)] DataTemplate
            Func<UIElement, UIElement> r_10_dtFunc = r_10_dtMethod;
            this.Add(typeof(EmptyKeys.GearsetModel.TreeNode), new DataTemplate(typeof(EmptyKeys.GearsetModel.TreeNode), r_10_dtFunc));
            // Resource - [alert1] SolidColorBrush
            this.Add("alert1", new SolidColorBrush(new ColorW(187, 68, 68, 255)));
            // Resource - [textSelection1] SolidColorBrush
            this.Add("textSelection1", new SolidColorBrush(new ColorW(119, 119, 119, 255)));
            // Resource - [translucidColor] Color
            this.Add("translucidColor", new ColorW(119, 119, 119, 170));
            // Resource - [translucid1] SolidColorBrush
            this.Add("translucid1", new SolidColorBrush(new ColorW(119, 119, 119, 170)));
            // Resource - [subtleColor] Color
            this.Add("subtleColor", new ColorW(119, 119, 119, 255));
            // Resource - [normalText1] SolidColorBrush
            this.Add("normalText1", new SolidColorBrush(new ColorW(221, 221, 221, 255)));
            // Resource - [backgroundColor] Color
            this.Add("backgroundColor", new ColorW(0, 0, 0, 0));
            // Resource - [normalTextMouseOverColor] Color
            this.Add("normalTextMouseOverColor", new ColorW(255, 255, 255, 255));
            // Resource - [subtle1] SolidColorBrush
            this.Add("subtle1", new SolidColorBrush(new ColorW(119, 119, 119, 255)));
            // Resource - [hyperlinkColor] Color
            this.Add("hyperlinkColor", new ColorW(85, 153, 255, 255));
            // Resource - [normal1] SolidColorBrush
            this.Add("normal1", new SolidColorBrush(new ColorW(64, 64, 64, 255)));
            // Resource - [background1] SolidColorBrush
            this.Add("background1", new SolidColorBrush(new ColorW(0, 0, 0, 0)));
            // Resource - [textSelectionColor] Color
            this.Add("textSelectionColor", new ColorW(119, 119, 119, 255));
            // Resource - [normalTextMouseOver1] SolidColorBrush
            this.Add("normalTextMouseOver1", new SolidColorBrush(new ColorW(255, 255, 255, 255)));
            // Resource - [highlightColor] Color
            this.Add("highlightColor", new ColorW(138, 198, 49, 255));
            // Resource - [normalTextColor] Color
            this.Add("normalTextColor", new ColorW(221, 221, 221, 255));
            // Resource - [normalColor] Color
            this.Add("normalColor", new ColorW(64, 64, 64, 255));
            // Resource - [borderColor] Color
            this.Add("borderColor", new ColorW(85, 85, 85, 255));
            FontManager.Instance.AddFont("Segoe UI", 12F, FontStyle.Regular, "Segoe_UI_9_Regular");
        }
        
        private static UIElement r_10_dtMethod(UIElement parent) {
            // e_9 element
            Grid e_9 = new Grid();
            e_9.Parent = parent;
            e_9.Name = "e_9";
            ColumnDefinition col_e_9_0 = new ColumnDefinition();
            col_e_9_0.Width = new GridLength(1F, GridUnitType.Star);
            e_9.ColumnDefinitions.Add(col_e_9_0);
            // e_10 element
            TextBlock e_10 = new TextBlock();
            e_9.Children.Add(e_10);
            e_10.Name = "e_10";
            e_10.Margin = new Thickness(2F, 2F, 2F, 2F);
            Grid.SetColumn(e_10, 0);
            Binding binding_e_10_Text = new Binding("Name");
            e_10.SetBinding(TextBlock.TextProperty, binding_e_10_Text);
            return e_9;
        }
    }
}

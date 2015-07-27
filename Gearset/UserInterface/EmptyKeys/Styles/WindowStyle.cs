#if EMPTYKEYS
    using System;
    using EmptyKeys.UserInterface;
    using EmptyKeys.UserInterface.Controls;
    using EmptyKeys.UserInterface.Data;
    using EmptyKeys.UserInterface.Themes;

    namespace Gearset.UserInterface.EmptyKeys.Styles
    { 
        public static class WindowStyle
	    {
		    public static Style CreateWindowStyle()
		    {
			    var style = new Style(typeof(Window));
			    var item = new Setter(Control.BackgroundProperty, new ResourceReferenceExpression(CommonResourceKeys.WindowBackgroundBrushKey));
			    style.Setters.Add(item);
			    var item2 = new Setter(Control.ForegroundProperty, new ResourceReferenceExpression(CommonResourceKeys.WindowTextColorKey));
			    style.Setters.Add(item2);
			    var item3 = new Setter(UIElement.SnapsToDevicePixelsProperty, true);
			    style.Setters.Add(item3);
			    var value = CreateControlTemplate();
			    var item4 = new Setter(Control.TemplateProperty, value);
			    style.Setters.Add(item4);
			    return style;
		    }
		    public static ControlTemplate CreateControlTemplate()
		    {
                var createMethod = new Func<UIElement, UIElement>(CreateControlTemplateCreateWindowUIElement);
			    var controlTemplate = new ControlTemplate(typeof(Window), createMethod);
			    var item = CommonHelpers.CreateBackgroundBorderForegroundTrigger(Window.IsActiveProperty, true, CommonResourceKeys.WindowActiveBrushKey, null, CommonResourceKeys.ItemTextSelectedColorKey, "PART_WindowTitleBorder");
			    controlTemplate.Triggers.Add(item);
			    return controlTemplate;
		    }
            public static UIElement CreateControlTemplateCreateWindowUIElement(UIElement parent)
		    {
                var border = new Border {Parent = parent, BorderThickness = new Thickness(1f)};
                border.SetResourceReference(Control.BorderBrushProperty, CommonResourceKeys.WindowBorderBrushKey);

                var binding = new Binding("SnapsToDevicePixels") {Source = parent};
                border.SetBinding(UIElement.SnapsToDevicePixelsProperty, binding);

                var binding2 = new Binding("Background") {Source = parent};
                border.SetBinding(Control.BackgroundProperty, binding2);

			    var grid = new Grid();
			    border.Child = grid;
			    grid.ColumnDefinitions.Add(new ColumnDefinition());
			    grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			    grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var border2 = new Border {Name = "PART_WindowTitleBorder"};
                border2.SetResourceReference(Control.BackgroundProperty, CommonResourceKeys.WindowBorderBrushKey);
			    grid.Children.Add(border2);

                var contentPresenter = CommonHelpers.CreateContentPresenter(parent, "Title");
			    contentPresenter.IsHitTestVisible = false;
			    contentPresenter.Name = "PART_WindowTitle";
                contentPresenter.Height = 22;
			    contentPresenter.HorizontalAlignment = HorizontalAlignment.Center;
			    contentPresenter.VerticalAlignment = VerticalAlignment.Center;
			    contentPresenter.Margin = new Thickness(0f);
			    border2.Child = contentPresenter;

                var scrollViewer = new ScrollViewer
                {
                    Margin = new Thickness(4f),
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled
                };
                grid.Children.Add(scrollViewer);
			    Grid.SetRow(scrollViewer, 1);

                var contentPresenter2 = CommonHelpers.CreateContentPresenter(parent, "Content");
			    contentPresenter2.Name = "PART_WindowContent";
			    scrollViewer.Content = contentPresenter2;

                var binding3 = new Binding("Width") {Source = parent};
                contentPresenter2.SetBinding(UIElement.MaxWidthProperty, binding3);

                var binding4 = new Binding("Height") {Source = parent};
                contentPresenter2.SetBinding(UIElement.MaxHeightProperty, binding4);

                var border3 = new Border
                {
                    Name = "PART_WindowResizeBorder",
                    Width = 8f,
                    Height = 8f,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                border3.SetResourceReference(Control.BackgroundProperty, CommonResourceKeys.WindowResizeThumbKey);
                //border3.SetResourceReference(Control.BackgroundProperty, CommonResourceKeys.AccentColorBrushKey);
			    grid.Children.Add(border3);
			    Grid.SetRow(border3, 1);
			    return border;
		    }
	    }
    }
#endif

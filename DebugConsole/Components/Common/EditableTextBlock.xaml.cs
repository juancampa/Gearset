using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.Xna.Framework;


namespace Gearset.Components
{

    /// <summary>
    /// Textblock that can be eddited.
    /// </summary>
    [TemplatePart(Name="PART_TextBlock", Type=typeof(TextBlock))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public partial class EditableTextBlock : TextBox
    {
        /// <summary>
        /// Gets or sets whether this TextBlock is editing or not.
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableTextBlock),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnIsEditingChanged));

        private TextBlock block;
        private TextBox box;

        int clickCount = 0;

        public bool IsEditing { get { return (bool)this.GetValue(IsEditingProperty); } set { this.SetValue(IsEditingProperty, value); } }
        public EditableTextBlock()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Same as setting IsEditing
        /// </summary>
        public void StartEdit()
        {
            IsEditing = true;
        }

        /// <summary>
        /// Same as unsetting IsEditing
        /// </summary>
        public void StopEdit()
        {
            IsEditing = false;
        }

        protected static void OnIsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var o = ((EditableTextBlock)d);
            if ((bool)args.NewValue)
            {
                o.box.Visibility = Visibility.Visible;
                o.block.Visibility = Visibility.Hidden;
                o.box.Focus();
                o.box.SelectAll();
            }
            else
            {
                o.box.Visibility = Visibility.Hidden;
                o.block.Visibility = Visibility.Visible;
                o.clickCount = 0;
            }
        }

        public override void OnApplyTemplate()
        {
            block = GetTemplateChild("PART_TextBlock") as TextBlock;
            box = GetTemplateChild("PART_TextBox") as TextBox;

            if (box == null || block == null)
                throw new NullReferenceException("Parts of the EditableTextBlock are not available in the provided Template.");

            box.PreviewKeyDown += new KeyEventHandler(box_PreviewKeyDown);
            box.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(box_LostKeyboardFocus);
            base.OnApplyTemplate();
        }


        void box_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            IsEditing = false;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;
            if (!(e.Source is TextBlock))
            clickCount++;
            if (clickCount == 2)
            {
                StartEdit();
                e.Handled = true;
            }
            else
            {
                // Don't handle it so the TreeViewItem can become selected.
                e.Handled = false;
            }
        }

        void box_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StopEdit();
            }
        }
    }
}

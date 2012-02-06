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


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class NumericItem : VisualItemBase
    {
        /// <summary>
        /// The mode of the Numeric Item
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(NumericSpinnerMode), typeof(NumericItem),
            new FrameworkPropertyMetadata(NumericSpinnerMode.Float, FrameworkPropertyMetadataOptions.AffectsRender, OnModeChanged));

        private System.Windows.Point downPosition;
        private bool isDragging = false;
        private Object mouseDownValue = 0f;

        private bool plotValues;

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((NumericItem)d).Mode = (NumericSpinnerMode)args.NewValue;
        }
        /// <summary>
        /// What type of numeric value will this spinner handle
        /// </summary>
        public NumericSpinnerMode Mode { get { return (NumericSpinnerMode)GetValue(ModeProperty); } set { SetValue(ModeProperty, value); } }


        /// <summary>
        /// Defines the state of the spinner, if IsEditing 
        /// is because the user is currently editing the value
        /// so the Updating value of the TreeNode will be set 
        /// to false until the control loses focus.
        /// </summary>
        public bool IsEditing;

        /// <summary>
        /// True if the TreeNode was Updating before the user
        /// started to edit it.
        /// </summary>
        private bool WasUpdating;

        public Object Value
        {
            get { return realValue; }
            set
            {
                realValue = value;
                TextBox1.Text = realValue.ToString();
            }
        }
        public Object realValue;

        public NumericItem()
        {
            InitializeComponent();

            this.TextBox1.PreviewMouseDown += MouseDownHandler;

            this.Button1.PreviewMouseDown += Button1_MouseDownHandler;
            this.Button1.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button1.PreviewMouseMove += Button1_MouseMoveHandler;

            TextBox1.LostFocus += new RoutedEventHandler(TextBox1_LostFocus);
            TextBox1.KeyDown += new KeyEventHandler(TextBox1_KeyDown);
        }
        
         public void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox1.MoveFocus(traversalRequest);
            }
            if (e.Key == Key.Subtract)
            {
                System.Windows.Controls.TextBox box = (System.Windows.Controls.TextBox)sender;
                String text = box.Text;
                int caret = box.CaretIndex;
                if (box.SelectionLength > 0)
                    text = text.Substring(0, box.SelectionStart) + text.Substring(box.SelectionStart + box.SelectionLength);
                box.Text = text.Insert(box.CaretIndex, "-");
                box.CaretIndex = caret + 1;
                e.Handled = true;
            }
        }

        #region Mouse movement events
         public void Button1_MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null, CaptureMode.None);
            isDragging = false;
            TreeNode.Updating = WasUpdating;
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max), min);
        }

         public void Button1_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isDragging)
            {
                System.Windows.Point pos = Mouse.GetPosition(Button1);

                Object newValue = null;
                try
                {
                    switch (Mode)
                    {
                        case NumericSpinnerMode.Byte:
                            {
                                byte delta = Math.Max((byte)((byte)mouseDownValue / 100), (byte)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (byte)Clamp((byte)mouseDownValue - movement * delta, byte.MinValue, byte.MaxValue);
                                break;
                            }
                        case NumericSpinnerMode.Char: break;
                        case NumericSpinnerMode.Decimal:
                            {
                                decimal delta = Math.Max((decimal)((decimal)mouseDownValue / 100), (decimal)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (decimal)((decimal)mouseDownValue - movement * delta);
                                break;
                            }
                        case NumericSpinnerMode.Int:
                            {
                                int delta = Math.Max((int)((int)mouseDownValue / 100), (int)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (int)mouseDownValue - movement * delta;
                                break;
                            }
                        case NumericSpinnerMode.Long:
                            {
                                long delta = Math.Max((long)((long)mouseDownValue / 100), (long)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (long)((long)mouseDownValue - movement * delta);
                                break;
                            }
                        case NumericSpinnerMode.SByte:
                            {
                                sbyte delta = Math.Max((sbyte)((sbyte)mouseDownValue / 100), (sbyte)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (sbyte)((sbyte)mouseDownValue - movement * delta);
                                break;
                            }
                        case NumericSpinnerMode.Short:
                            {
                                short delta = Math.Max((short)((short)mouseDownValue / 100), (short)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (short)((short)mouseDownValue - movement * delta);
                                break;
                            }
                        case NumericSpinnerMode.UInt:
                            {
                                uint delta = Math.Max((uint)((uint)mouseDownValue / 100), (uint)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (uint)mouseDownValue + (uint)Math.Max(-movement * (int)delta, -(int)(uint)mouseDownValue);
                                break;
                            }
                        case NumericSpinnerMode.ULong:
                            {
                                ulong delta = Math.Max((ulong)((ulong)mouseDownValue / 100), (ulong)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (ulong)mouseDownValue + (ulong)Math.Max(-movement * (int)delta, -(int)(ulong)mouseDownValue);
                                break;
                            }
                        case NumericSpinnerMode.UShort:
                            {
                                ushort delta = Math.Max((ushort)((ushort)mouseDownValue / 100), (ushort)1);
                                int movement = (int)(pos.Y - downPosition.Y);
                                newValue = (ushort)((ushort)mouseDownValue + (ushort)Math.Max(-movement * (int)delta, -(int)(ushort)mouseDownValue));
                                break;
                            }
                        case NumericSpinnerMode.Double:
                            {
                                double delta = Math.Max((double)mouseDownValue / 100.0, 0.01);
                                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                                    delta *= 10;
                                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                                    delta *= 0.1;
                                double movement = (int)(pos.Y - downPosition.Y);
                                newValue = (double)mouseDownValue - movement * delta;
                                break;
                            }
                        case NumericSpinnerMode.Float:
                            {
                                float delta = Math.Max((float)mouseDownValue / 100.0f, 0.01f);
                                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                                    delta *= 10f;
                                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                                    delta *= 0.1f;
                                float movement = (int)(pos.Y - downPosition.Y);
                                newValue = (float)mouseDownValue - movement * delta;
                                break;
                            }
                    }
                }
                catch
                {
                    // Silently ignore if value is not good for the specified type.
                }
               
                if (newValue != Value && newValue != null)
                {
                    realValue = newValue;
                    UpdateVariable();
                    UpdateUI(TreeNode.Property);
                }

            }
        }

         public void Button1_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (!IsEditing)
            {
                WasUpdating = TreeNode.Updating;
            }
            TreeNode.Updating = false;
            Mouse.Capture(Button1);
            mouseDownValue = TreeNode.Property;
            downPosition = e.GetPosition(Button1);
            isDragging = true;
        }

        void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            // If we're already editing the TextBox, don't do anything
            if (!IsEditing)
            {
                // Save the value of Updating so we can restore it
                // after the edit.
                WasUpdating = TreeNode.Updating;
                TreeNode.Updating = false;

                // Flag that we're current
                IsEditing = true;
            }
        }
        
        #endregion

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public override void UpdateUI(Object value)
        {
            try
            {
                switch (Mode)
                {
                    case NumericSpinnerMode.Byte: this.Value = (byte)value; break;
                    case NumericSpinnerMode.Char: this.Value = (char)value; break;
                    case NumericSpinnerMode.Decimal: this.Value = (decimal)value; break;
                    case NumericSpinnerMode.Double: this.Value = (double)value; break;
                    case NumericSpinnerMode.Float: this.Value = (float)value; break;
                    case NumericSpinnerMode.Int: this.Value = (int)value; break;
                    case NumericSpinnerMode.Long: this.Value = (long)value; break;
                    case NumericSpinnerMode.SByte: this.Value = (sbyte)value; break;
                    case NumericSpinnerMode.Short: this.Value = (short)value; break;
                    case NumericSpinnerMode.UInt: this.Value = (uint)value; break;
                    case NumericSpinnerMode.ULong: this.Value = (ulong)value; break;
                    case NumericSpinnerMode.UShort: this.Value = (ushort)value; break;
                }

                if (plotValues)
                {
                    switch (Mode)
                    {
                        case NumericSpinnerMode.Byte: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(byte)TreeNode.Property); break;
                        case NumericSpinnerMode.Char: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(char)TreeNode.Property); break;
                        case NumericSpinnerMode.Decimal: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(decimal)TreeNode.Property); break;
                        case NumericSpinnerMode.Double: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(double)TreeNode.Property); break;
                        case NumericSpinnerMode.Float: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)TreeNode.Property); break;
                        case NumericSpinnerMode.Int: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(int)TreeNode.Property); break;
                        case NumericSpinnerMode.Long: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(long)TreeNode.Property); break;
                        case NumericSpinnerMode.SByte: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(sbyte)TreeNode.Property); break;
                        case NumericSpinnerMode.Short: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(short)TreeNode.Property); break;
                        case NumericSpinnerMode.UInt: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(uint)TreeNode.Property); break;
                        case NumericSpinnerMode.ULong: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(ulong)TreeNode.Property); break;
                        case NumericSpinnerMode.UShort: GearsetResources.Console.Plot(TreeNode.FriendlyName, (float)(ushort)TreeNode.Property); break;
                    }
                }
            }
            catch {
                GearsetResources.Console.Log("Error while updating NumericItem");
                throw;
            }
        }

        /// <summary>
        /// Updates the variable fromt he UI.
        /// </summary>
        public override void UpdateVariable()
        {
            TreeNode.Property = Value;
        }

         public void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            // If we don't have an assigned TreeNode
            if (TreeNode == null)
            {
                return;
            }
            Object boxedValue;
            try
            {
                switch (Mode)
                {
                    case NumericSpinnerMode.Byte: boxedValue = byte.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Char: boxedValue = char.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Decimal: boxedValue = decimal.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Double: boxedValue = double.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Float: boxedValue = float.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Int: boxedValue = int.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Long: boxedValue = long.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.SByte: boxedValue = sbyte.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.Short: boxedValue = short.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.UInt: boxedValue = uint.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.ULong: boxedValue = ulong.Parse(TextBox1.Text); break;
                    case NumericSpinnerMode.UShort: boxedValue = ushort.Parse(TextBox1.Text); break;
                    default: boxedValue = null; break;
                }
                this.realValue = boxedValue;
                UpdateVariable();

                // If we're dragging is because the focus has 
                // changed to the UpDown Button so we should 
                // change the mouseDownValue to reflect any change
                // we've made while editing.
                // If the focus has changed to the spinner
                // do not return the value of Updating yet because
                // the Button1_MouseUp will do it.
                if (isDragging)
                    mouseDownValue = boxedValue;
                else
                    TreeNode.Updating = WasUpdating;
            }
            catch
            {
                // If something happened while parsing the new value
                // update the UI so it has the previous value.
                UpdateUI(TreeNode.Property);
                GearsetResources.Console.Log("Error while parsing the entered value.");
            }
            finally
            {
                // Whatever happes, we won't be editing anymore.
                IsEditing = false;
            }

            
        }

         private void PlotToggleButton_Checked(object sender, RoutedEventArgs e)
         {
             plotValues = PlotToggleButton.IsChecked ?? false;

             if (!plotValues)
                 GearsetResources.Console.Plotter.RemovePlot(TreeNode.Name);
         }

    }
}

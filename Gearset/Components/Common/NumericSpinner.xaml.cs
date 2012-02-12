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
    /// Interaction logic for the NumericSpinner control
    /// </summary>
    public partial class NumericSpinner : UserControl
    {
        /// <summary>
        /// What type of numeric value will this spinner handle
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(NumericSpinnerMode), typeof(NumericSpinner),
            new FrameworkPropertyMetadata(NumericSpinnerMode.Float, FrameworkPropertyMetadataOptions.AffectsRender, OnModeChanged));
        public NumericSpinnerMode Mode
        {
            get { return (NumericSpinnerMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// The numeric value held by this spinner. It must be unboxed
        /// to the type defined by the spinner mode.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(Object), typeof(NumericSpinner), new PropertyMetadata(0f, ValueChangedCallback));
        public object Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Determines whether the spinner should be blank instead of 
        /// showing a "NaN" value.
        /// </summary>
        public static readonly DependencyProperty ShowNaNProperty =
            DependencyProperty.Register("ShowNaN", typeof(bool), typeof(NumericSpinner), new PropertyMetadata(true, ValueChangedCallback));
        public bool ShowNaN
        {
            get { return (bool)GetValue(ShowNaNProperty); }
            set { SetValue(ShowNaNProperty, value); }
        }
        private System.Windows.Point downPosition;
        private bool isDragging = false;
        private Object mouseDownValue = 0f;

        private bool plotValues;

        

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((NumericSpinner)d).Mode = (NumericSpinnerMode)args.NewValue;
        }


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

        /// <summary>
        /// Defines a way to move the focus out of the
        /// textbox when enter is pressed.
        /// </summary>
        protected static readonly TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
        
        public NumericSpinner()
        {
            InitializeComponent();

            this.TextBox1.PreviewMouseDown += MouseDownHandler;

            this.Button1.PreviewMouseDown += Button1_MouseDownHandler;
            this.Button1.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button1.PreviewMouseMove += Button1_MouseMoveHandler;

            TextBox1.LostFocus += new RoutedEventHandler(TextBox1_LostFocus);
            TextBox1.KeyDown += new KeyEventHandler(TextBox1_KeyDown);

            Value = 0.0f;
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            String str = e.NewValue.ToString();
            if (str == "NaN" && !((NumericSpinner)d).ShowNaN)
                ((NumericSpinner)d).TextBox1.Text = String.Empty;
            else
                ((NumericSpinner)d).TextBox1.Text = e.NewValue.ToString();
        }
        
         public void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //TextBox1.MoveFocus(traversalRequest);
                TextBox1_LostFocus(this, null);
                TextBox1.SelectAll();
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
                    Value = newValue;
                }

            }
        }

         public void Button1_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(Button1);
            mouseDownValue = Value;
            downPosition = e.GetPosition(Button1);
            isDragging = true;
        }

        void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            // If we're already editing the TextBox, don't do anything
            if (!IsEditing)
            {
                // Flag that we're current
                IsEditing = true;
            }
        }
        
        #endregion

        public void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
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
                this.Value = boxedValue;

                // If we're dragging is because the focus has 
                // changed to the UpDown Button so we should 
                // change the mouseDownValue to reflect any change
                // we've made while editing.
                // If the focus has changed to the spinner
                // do not return the value of Updating yet because
                // the Button1_MouseUp will do it.
                if (isDragging)
                    mouseDownValue = boxedValue;
            }
            catch
            {
                GearsetResources.Console.Log("Error while parsing the entered value.");
            }
            finally
            {
                // Whatever happes, we won't be editing anymore.
                IsEditing = false;
            }


        }
    }

    public enum NumericSpinnerMode
    {
        Byte,
        Char,
        Decimal,
        Double,
        Float,
        Int,
        Long,
        SByte,
        Short,
        UInt,
        ULong,
        UShort
    }
}

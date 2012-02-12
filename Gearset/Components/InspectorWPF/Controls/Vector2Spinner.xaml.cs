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
    public partial class Vector2Spinner : VisualItemBase
    {
        private Vector2 downPosition;

        /// <summary>
        /// True if the TreeNode was Updating before the user
        /// started to edit it.
        /// </summary>
        private bool WasUpdating;

        public Vector2 Value
        {
            get { return realValue; }
            set
            {
                realValue = value;
                float x = realValue.X;
                float y = realValue.Y;
                float l = realValue.Length();
                String xstr;
                String ystr;
                String lstr;
                if (Math.Abs(x) > 1e-4) xstr = String.Format("{0:0.0000}", x); else xstr = String.Format("{0:0.0#e+00}", x);
                if (Math.Abs(y) > 1e-4) ystr = String.Format("{0:0.0000}", y); else ystr = String.Format("{0:0.0#e+00}", y);
                if (Math.Abs(l) > 1e-4) lstr = String.Format("{0:0.0000}", l); else lstr = String.Format("{0:0.0#e+00}", l);
                TextBlock1.Text = String.Format("({0}, {1}) {2} ", xstr, ystr, lstr);
            }
        }
        public Vector2 realValue;

        public Vector2Spinner()
        {
            InitializeComponent();

            this.Button1.PreviewMouseDown += Button1_MouseDownHandler;
            this.Button1.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button1.PreviewMouseMove += Button1_MouseMoveHandler;

            this.Button2.PreviewMouseDown += Button2_MouseDownHandler;
            this.Button2.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button2.PreviewMouseMove += Button1_MouseMoveHandler;
        }

        private enum ModeEnum
        {
            XY,
            L,
        }

        private ModeEnum currentMode;
        private Vector2 mouseDownDirection;
        private Vector2 mouseDownModeValue;
        private bool isDragging;

        #region Mouse movement events
         public void Button1_MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null, CaptureMode.None);
            isDragging = false;
            TreeNode.Updating = WasUpdating;
        }

         public void Button1_MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && isDragging)
            {
                System.Windows.Point p = new System.Windows.Point();

                switch (currentMode)
                {
                    case ModeEnum.XY: p = Mouse.GetPosition(Button1); break;
                    case ModeEnum.L: p = Mouse.GetPosition(Button2); p.Y = -p.Y; break;
                }
                Vector2 currentPos = new Vector2((float)p.X, (float)p.Y);

                Vector2 deltaVector = new Vector2
                {
                    X = Math.Max((float)Math.Abs(mouseDownModeValue.X) / 100.0f, 0.01f),
                    Y = Math.Max((float)Math.Abs(mouseDownModeValue.Y) / 100.0f, 0.01f),
                };

                float delta = deltaVector.Length();
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    delta *= 10f;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    delta *= 0.1f;

                Vector2 movement = currentPos - downPosition;
                movement *= -1;
                Vector2 newValue = mouseDownModeValue - movement * delta;

                Vector2 currentModeValue = GetCurrentModeValue();
                Vector2 currentPropertyValue = (Vector2)TreeNode.Property;

                if (newValue != currentModeValue && newValue != null)
                {
                    Vector2 v = new Vector2();
                    switch (currentMode)
                    {
                        case ModeEnum.XY: v = new Vector2(newValue.X, newValue.Y); break;
                        case ModeEnum.L: v = mouseDownDirection * newValue.Y; break;
                    }
                    realValue = v;
                    UpdateVariable();
                    UpdateUI(TreeNode.Property);
                }
            }
        }

        private Vector2 GetCurrentModeValue()
        {
            Vector2 result = new Vector2();
            Vector2 currentPropertyValue = (Vector2)TreeNode.Property;
            switch (currentMode)
            {
                case ModeEnum.XY: result = new Vector2(currentPropertyValue.X, currentPropertyValue.Y); break;
                case ModeEnum.L: result = new Vector2(0, currentPropertyValue.Length()); break;
            }
            return result;
        }

         public void Button1_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.XY;
            WasUpdating = TreeNode.Updating;
            mouseDownModeValue = GetCurrentModeValue();
            TreeNode.Updating = false;
            Mouse.Capture(Button1);
            System.Windows.Point p = e.GetPosition(Button1);
            downPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }

         public void Button2_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.L;
            WasUpdating = TreeNode.Updating;
            mouseDownModeValue = GetCurrentModeValue();

            // Get the current vector direction so we can scale it
            mouseDownDirection = (Vector2)TreeNode.Property;
            if (mouseDownDirection != Vector2.Zero)
                mouseDownDirection.Normalize();
            else
                mouseDownDirection = Vector2.Normalize(Vector2.One);

            TreeNode.Updating = false;
            Mouse.Capture(Button2);
            System.Windows.Point p = e.GetPosition(Button2);
            downPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }
        #endregion

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public override void UpdateUI(Object value)
        {
            this.Value = (Vector2)value;
        }

        /// <summary>
        /// Updates the variable fromt he UI.
        /// </summary>
        public override void UpdateVariable()
        {
            TreeNode.Property = Value;
        }

    }
}

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
    public partial class Vector3Spinner : VisualItemBase
    {
        private Vector2 downPosition;
        private Vector2 mouseDownModeValue;
        private bool isDragging = false;

        /// <summary>
        /// True if the TreeNode was Updating before the user
        /// started to edit it.
        /// </summary>
        private bool WasUpdating;

        public Vector3 Value
        {
            get { return realValue; }
            set
            {
                realValue = value;
                float x = realValue.X;
                float y = realValue.Y;
                float z = realValue.Z;
                float l = realValue.Length();
                String xstr;
                String ystr;
                String zstr;
                String lstr;
                if (x * x > 1e-7 || x == 0) xstr = String.Format("{0:0.0000}", x); else xstr = String.Format("{0:0.0#e+00}", x);
                if (y * y > 1e-7 || y == 0) ystr = String.Format("{0:0.0000}", y); else ystr = String.Format("{0:0.0#e+00}", y);
                if (z * z > 1e-7 || z == 0) zstr = String.Format("{0:0.0000}", z); else zstr = String.Format("{0:0.0#e+00}", z);
                if (l * l > 1e-7 || l == 0) lstr = String.Format("{0:0.0000}", l); else lstr = String.Format("{0:0.0#e+00}", l);
                TextBlock1.Text = String.Format("({0}, {1}, {2}) {3}", xstr, ystr, zstr, lstr);
            }
        }
        public Vector3 realValue;

        public Vector3Spinner()
        {
            InitializeComponent();

            this.Button1.PreviewMouseDown += Button1_MouseDownHandler;
            this.Button1.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button1.PreviewMouseMove += Button1_MouseMoveHandler;

            this.Button2.PreviewMouseDown += Button2_MouseDownHandler;
            this.Button2.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button2.PreviewMouseMove += Button1_MouseMoveHandler;

            this.Button3.PreviewMouseDown += Button3_MouseDownHandler;
            this.Button3.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button3.PreviewMouseMove += Button1_MouseMoveHandler;

            this.Button4.PreviewMouseDown += Button4_MouseDownHandler;
            this.Button4.PreviewMouseUp += Button1_MouseUpHandler;
            this.Button4.PreviewMouseMove += Button1_MouseMoveHandler;
        }

        public enum ModeEnum
        {
            XY,
            XZ,
            YZ,
            L,
        }
        private ModeEnum currentMode;
        private Vector3 mouseDownDirection;

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
                    case ModeEnum.XZ: p = Mouse.GetPosition(Button2); break;
                    case ModeEnum.YZ: p = Mouse.GetPosition(Button3); break;
                    case ModeEnum.L: p = Mouse.GetPosition(Button4); break;
                }
                Vector2 currentPos = new Vector2((float)p.X, (float)p.Y);

                Vector2 deltaVector = new Vector2
                {
                    X = Math.Max((float)(mouseDownModeValue.X) / 100.0f, 0.01f),
                    Y = Math.Max((float)(mouseDownModeValue.Y) / 100.0f, 0.01f),
                };

                float delta = deltaVector.Length();
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    delta *= 10f;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    delta *= 0.1f;

                Vector2 movement = currentPos - downPosition;
                movement.X = -movement.X;
                Vector2 newValue = mouseDownModeValue - movement * delta;

                Vector2 currentModeValue = GetCurrentModeValue();
                Vector3 currentPropertyValue = (Vector3)TreeNode.Property;

                if (newValue != currentModeValue && newValue != null)
                {
                    Vector3 v = new Vector3();
                    switch (currentMode)
                    {
                        case ModeEnum.XY: v = new Vector3(newValue.X, newValue.Y, currentPropertyValue.Z); break;
                        case ModeEnum.XZ: v = new Vector3(newValue.X, currentPropertyValue.Y, newValue.Y); break;
                        case ModeEnum.YZ: v = new Vector3(currentPropertyValue.X, newValue.X, newValue.Y); break;
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
            Vector3 currentPropertyValue = (Vector3)TreeNode.Property;
            switch (currentMode)
            {
                case ModeEnum.XY: result = new Vector2(currentPropertyValue.X, currentPropertyValue.Y); break;
                case ModeEnum.XZ: result = new Vector2(currentPropertyValue.X, currentPropertyValue.Z); break;
                case ModeEnum.YZ: result = new Vector2(currentPropertyValue.Y, currentPropertyValue.Z); break;
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
            currentMode = ModeEnum.XZ;
            WasUpdating = TreeNode.Updating;
            mouseDownModeValue = GetCurrentModeValue();
            TreeNode.Updating = false;
            Mouse.Capture(Button2);
            System.Windows.Point p = e.GetPosition(Button2);
            downPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }
         public void Button3_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.YZ;
            WasUpdating = TreeNode.Updating;
            mouseDownModeValue = GetCurrentModeValue();
            TreeNode.Updating = false;
            Mouse.Capture(Button3);
            System.Windows.Point p = e.GetPosition(Button3);
            downPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }
         public void Button4_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.L;
            WasUpdating = TreeNode.Updating;
            mouseDownModeValue = GetCurrentModeValue();

            // Get the current vector direction so we can scale it
            mouseDownDirection = (Vector3)TreeNode.Property;
            if (mouseDownDirection != Vector3.Zero)
                mouseDownDirection.Normalize();
            else
                mouseDownDirection = Vector3.Normalize(Vector3.One);

            TreeNode.Updating = false;
            Mouse.Capture(Button4);
            System.Windows.Point p = e.GetPosition(Button4);
            downPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }        
        #endregion

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public override void UpdateUI(Object value)
        {
            this.Value = (Vector3)value;
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

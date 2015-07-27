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
    public partial class QuaternionSpinner : VisualItemBase
    {
        private Quaternion mouseDownValue;
        private bool isDragging = false;

        /// <summary>
        /// True if the TreeNode was Updating before the user
        /// started to edit it.
        /// </summary>
        private bool WasUpdating;

        public Quaternion Value
        {
            get { return realValue; }
            set
            {
                realValue = value;
                TextBlock1.Text = realValue.ToString();
            }
        }
        public Quaternion realValue;

        public QuaternionSpinner()
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
        }

        public enum ModeEnum
        {
            X,
            Y,
            Z,
        }
        private ModeEnum currentMode;
        private Vector2 mouseDownPosition;

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
                    case ModeEnum.X: p = Mouse.GetPosition(Button1); break;
                    case ModeEnum.Y: p = Mouse.GetPosition(Button2); break;
                    case ModeEnum.Z: p = Mouse.GetPosition(Button3); break;
                }
                float currentPos = (float)p.Y;

                // Angle to rotate per pixel moved
                float delta = 0.004f;

                float movement = currentPos - mouseDownPosition.Y;
                float angle = movement * delta;
               

                Vector3 angles = new Vector3();
                switch (currentMode)
                {
                    case ModeEnum.X: angles = new Vector3(angle, 0, 0); break;
                    case ModeEnum.Y: angles = new Vector3(0, angle, 0); break;
                    case ModeEnum.Z: angles = new Vector3(0, 0, angle); break;
                }

                // Yaw and Roll are switched.
                Quaternion newValue = mouseDownValue * Quaternion.CreateFromYawPitchRoll(angles.Y, angles.X, angles.Z);
                realValue = newValue;
                UpdateVariable();
                UpdateUI(TreeNode.Property);

            }
        }

         public void Button1_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.X;
            WasUpdating = TreeNode.Updating;
            mouseDownValue = (Quaternion)TreeNode.Property;
            TreeNode.Updating = false;
            Mouse.Capture(Button1);
            System.Windows.Point p = e.GetPosition(Button1);
            mouseDownPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }
         public void Button2_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.Y;
            WasUpdating = TreeNode.Updating;
            mouseDownValue = (Quaternion)TreeNode.Property;
            TreeNode.Updating = false;
            Mouse.Capture(Button2);
            System.Windows.Point p = e.GetPosition(Button2);
            mouseDownPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }
         public void Button3_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            currentMode = ModeEnum.Z;
            WasUpdating = TreeNode.Updating;
            mouseDownValue = (Quaternion)TreeNode.Property;
            TreeNode.Updating = false;
            Mouse.Capture(Button3);
            System.Windows.Point p = e.GetPosition(Button3);
            mouseDownPosition = new Vector2((float)p.X, (float)p.Y);
            isDragging = true;
        }
        #endregion

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public override void UpdateUI(Object value)
        {
            this.Value = (Quaternion)value;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gearset.Components.CurveEditorControl;
using System.Windows;

namespace Gearset.Components
{
    class Bender : Gear
    {
        private CurveTreeViewModel curveTreeViewModel;

        public CurveEditorWindow Window { get; private set; }
        public float SeekNeedlePosition { get { return (float)Window.horizontalRuler.NeedlePosition; } }
        public BenderConfig Config { get { return GearsetSettings.Instance.BenderConfig; } }

        public Bender()
            : base (GearsetSettings.Instance.BenderConfig)
        {
            Window = new CurveEditorWindow();
            Window.Show();

            curveTreeViewModel = new CurveTreeViewModel(Window.curveEditorControl);

            Window.DataContext = Window.curveEditorControl.ControlsViewModel;
            Window.curveTree.DataContext = curveTreeViewModel;
            Window.Top = Config.Top;
            Window.Left = Config.Left;
            Window.Width = Config.Width;
            Window.Height = Config.Height;
            Window.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(Window_IsVisibleChanged);


            Curve c1 = new Curve();
            AddCurve("MyObject.X", c1);
            c1 = new Curve();
            AddCurve("MyObject.Y", c1);
            c1 = new Curve();
            AddCurve("MyObject.Z", c1);
            c1 = new Curve();
            AddCurve("MyObject.Alpha", c1);

            c1 = new Curve();
            AddCurve("MyObject2.X", c1);
            c1 = new Curve();
            AddCurve("MyObject2.Y", c1);
            c1 = new Curve();
            AddCurve("MyObject2.Z", c1);
            c1 = new Curve();
            AddCurve("MyObject2.Alpha", c1);

            c1 = new Curve();
            AddCurve("Lonely curve", c1);
            c1 = new Curve();
            AddCurve("Another curve", c1);

            RemoveCurveOrGroup("Another curve");
        }

        void Window_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            Config.Visible = Window.IsVisible;
        }

        protected override void OnVisibleChanged()
        {
            if (Window != null)
                Window.Visibility = Visible ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Adds the provided curve to Bender
        /// </summary>
        /// <param name="name">The name of the curve, a dot-separated path can be used to group curves</param>
        /// <param name="curve">The curve to add to Bender</param>
        public void AddCurve(String name, Curve curve)
        {
            curveTreeViewModel.AddCurve(name, curve);
        }

        /// <summary>
        /// Removes the provided Curve from Bender.
        /// </summary>
        public void RemoveCurve(Curve curve)
        {
            curveTreeViewModel.RemoveCurve(curve);
        }

        /// <summary>
        /// Removes a Curve or a Group by name. The complete dot-separated
        /// path to the curve or group must be given.
        /// </summary>
        public void RemoveCurveOrGroup(String name)
        {
            curveTreeViewModel.RemoveCurveOrGroup(name);
        }

        public override void Update(GameTime gameTime)
        {
            Window.curveEditorControl.UpdateRender();
            Window.horizontalRuler.UpdateRender();
            Window.verticalRuler.UpdateRender();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            
            base.Draw(gameTime);
        }
    }
}

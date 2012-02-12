using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    internal class Transform3Drawer : Gear
    {
        private Vector3Drawer vectors;

        internal float AxisSize = 1f;

        internal Transform3Drawer()
            : base(GearsetSettings.Instance.LineDrawerConfig)
        {
            vectors = new Vector3Drawer();
            this.Children.Add(vectors);
        }

        internal void ShowTransform(String name, Matrix transform)
        {
            ShowTransform(name, transform, 1);
        }

        internal void ShowTransform(String name, Matrix transform, float axisScale)
        {
            Vector3 t = transform.Translation;
            float scale = AxisSize * axisScale;
            vectors.ShowVector3(name + "x", t, t + transform.Right * scale, Color.Red);
            vectors.ShowVector3(name + "y", t, t + transform.Up * scale, Color.Green);
            vectors.ShowVector3(name + "z", t, t + transform.Forward * scale, Color.Blue);
        }

        internal void ShowTransformOnce(Matrix transform)
        {
            ShowTransformOnce(transform, 1);
        }

        internal void ShowTransformOnce(Matrix transform, float axisScale)
        {
            Vector3 t = transform.Translation;
            float scale = AxisSize * axisScale;
            vectors.ShowVector3Once(t, t + transform.Right * scale, Color.Red);
            vectors.ShowVector3Once(t, t + transform.Up * scale, Color.Green);
            vectors.ShowVector3Once(t, t + transform.Forward * scale, Color.Blue);
        }
    }
}

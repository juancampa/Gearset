using System;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    internal class SphereDrawer : Gear
    {
        private readonly InternalLineDrawer lines;

        internal int CircleSteps = 20;
        internal int Sides = 12;

        internal SphereDrawer()
            : base(GearsetSettings.Instance.LineDrawerConfig)
        {
            lines = new InternalLineDrawer();
            this.Children.Add(lines);
        }

        internal void ShowSphere(String name, BoundingSphere sphere)
        {
            ShowSphere(name, sphere, Color.White);
        }

        internal void ShowSphere(String name, BoundingSphere sphere, Color color)
        {
            ShowSphere(name, sphere.Center, sphere.Radius, color);
        }

        internal void ShowSphere(String name, Vector3 center, float radius)
        {
            ShowSphere(name, center, radius, Color.White);
        }

        internal void ShowSphere(String name, Vector3 center, float radius, Color color)
        {
            for (int j = 0; j < Sides; j++)
            {
                float angle = j / (float)Sides * MathHelper.TwoPi;
                Vector3 x = new Vector3((float)Math.Cos(angle), 0, (float)Math.Sin(angle)) * radius;
                Vector3 y = Vector3.UnitY * radius;

                // Draw the vertical circles.
                for (int i = 0; i < CircleSteps; i++)
                {
                    float angle1 = i / (float)CircleSteps * MathHelper.TwoPi;
                    float angle2 = (i + 1) / (float)CircleSteps * MathHelper.TwoPi;
                    float sin1 = (float)Math.Sin(angle1);
                    float cos1 = (float)Math.Cos(angle1);
                    float sin2 = (float)Math.Sin(angle2);
                    float cos2 = (float)Math.Cos(angle2);
                    lines.ShowLine(name + "x" + j + i.ToString(), center + y * sin1 + x * cos1 , center + y * sin2 + x * cos2 , color);
                }
            }

            Vector3 x2 = Vector3.UnitX * radius;
            Vector3 z2 = Vector3.UnitZ * radius;
            // Draw the equator.
            for (int i = 0; i < CircleSteps; i++)
            {
                float sin1 = (float)Math.Sin(i / (float)CircleSteps * MathHelper.TwoPi);
                float cos1 = (float)Math.Cos(i / (float)CircleSteps * MathHelper.TwoPi);
                float sin2 = (float)Math.Sin((i + 1) / (float)CircleSteps * MathHelper.TwoPi);
                float cos2 = (float)Math.Cos((i + 1) / (float)CircleSteps * MathHelper.TwoPi);
                lines.ShowLine(name + "y" + i, center + x2 * sin1 + z2 * cos1, center + x2 * sin2 + z2 * cos2, color);
            }
        }

        internal void ShowSphereOnce(BoundingSphere sphere)
        {
            ShowSphereOnce(sphere, Color.White);
        }

        internal void ShowSphereOnce(BoundingSphere sphere, Color color)
        {
            ShowSphereOnce(sphere.Center, sphere.Radius, color);
        }

        internal void ShowSphereOnce(Vector3 center, float radius)
        {
            ShowSphereOnce(center, radius, Color.White);
        }

        internal void ShowSphereOnce(Vector3 center, float radius, Color color)
        {
            for (int j = 0; j < Sides; j++)
            {
                float angle = j / (float)Sides * MathHelper.TwoPi;
                Vector3 x = new Vector3((float)Math.Cos(angle), 0, (float)Math.Sin(angle)) * radius;
                Vector3 y = Vector3.UnitY * radius;

                // Draw the vertical circles.
                for (int i = 0; i < CircleSteps; i++)
                {
                    float angle1 = i / (float)CircleSteps * MathHelper.TwoPi;
                    float angle2 = (i + 1) / (float)CircleSteps * MathHelper.TwoPi;
                    float sin1 = (float)Math.Sin(angle1);
                    float cos1 = (float)Math.Cos(angle1);
                    float sin2 = (float)Math.Sin(angle2);
                    float cos2 = (float)Math.Cos(angle2);
                    lines.ShowLineOnce(center + y * sin1 + x * cos1, center + y * sin2 + x * cos2, color);
                }
            }

            Vector3 x2 = Vector3.UnitX * radius;
            Vector3 z2 = Vector3.UnitZ * radius;
            // Draw the equator.
            for (int i = 0; i < CircleSteps; i++)
            {
                float sin1 = (float)Math.Sin(i / (float)CircleSteps * MathHelper.TwoPi);
                float cos1 = (float)Math.Cos(i / (float)CircleSteps * MathHelper.TwoPi);
                float sin2 = (float)Math.Sin((i + 1) / (float)CircleSteps * MathHelper.TwoPi);
                float cos2 = (float)Math.Cos((i + 1) / (float)CircleSteps * MathHelper.TwoPi);
                lines.ShowLineOnce(center + x2 * sin1 + z2 * cos1, center + x2 * sin2 + z2 * cos2, color);
            }
        }

        internal void ShowCylinderOnce(Vector3 center, Vector3 radius) 
        {
            ShowCylinderOnce(center, radius, Color.White);
        }

        internal void ShowCylinderOnce(Vector3 center, Vector3 radius, Color color) 
        {
            var x2 = Vector3.UnitX * radius.X;
            var z2 = Vector3.UnitZ * radius.Z;
            var yp = Vector3.UnitY * radius.Y;
            
            // Draw the equator.
            for (var i = 0; i < CircleSteps; i++) 
            {
                var sin1 = (float)Math.Sin(i / (float)CircleSteps * MathHelper.TwoPi);
                var cos1 = (float)Math.Cos(i / (float)CircleSteps * MathHelper.TwoPi);
                var sin2 = (float)Math.Sin((i+1) / (float)CircleSteps * MathHelper.TwoPi);
                var cos2 = (float)Math.Cos((i + 1) / (float)CircleSteps * MathHelper.TwoPi);

                lines.ShowLineOnce(center + x2 * sin1 + z2 * cos1 - yp, center + x2 * sin1 + z2 * cos1 + yp, color);
                lines.ShowLineOnce(center + x2 * sin1 + z2 * cos1 - yp, center + x2 * sin2 + z2 * cos2 - yp, color);
                lines.ShowLineOnce(center + x2 * sin1 + z2 * cos1 + yp, center + x2 * sin2 + z2 * cos2 + yp, color);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Draws 2D/3D Boxes 
    /// </summary>
    internal class BoxDrawer : Gear
    {
        private InternalLineDrawer lines;

        internal BoxDrawer()
            : base(GearsetSettings.Instance.LineDrawerConfig)
        {
            lines = new InternalLineDrawer();
            this.Children.Add(lines);
        }

        #region 3D Boxes
        internal void ShowBox(String name, BoundingBox box)
        {
            ShowBox(name, box.Min, box.Max, Color.White);
        }

        internal void ShowBox(String name, BoundingBox box, Color color)
        {
            ShowBox(name, box.Min, box.Max, color);
        }

        internal void ShowBox(String name, Vector3 min, Vector3 max)
        {
            ShowBox(name, min, max, Color.White);
        }

        internal void ShowBox(String name, Vector3 min, Vector3 max, Color color)
        {
            lines.ShowLine(name + "a1", new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, min.Y, min.Z), color);
            lines.ShowLine(name + "a2", new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            lines.ShowLine(name + "a3", new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color);
            lines.ShowLine(name + "a4", new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            lines.ShowLine(name + "a5", new Vector3(min.X, min.Y, max.Z), new Vector3(max.X, min.Y, max.Z), color);
            lines.ShowLine(name + "a6", new Vector3(min.X, max.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines.ShowLine(name + "a7", new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            lines.ShowLine(name + "a8", new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines.ShowLine(name + "a9", new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, min.Y, max.Z), color);
            lines.ShowLine(name + "aa", new Vector3(min.X, max.Y, min.Z), new Vector3(min.X, max.Y, max.Z), color);
            lines.ShowLine(name + "ab", new Vector3(max.X, max.Y, min.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines.ShowLine(name + "ac", new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, min.Y, max.Z), color);
        }

        internal void ShowBoxOnce(BoundingBox box)
        {
            ShowBoxOnce(box.Min, box.Max, Color.White);
        }

        internal void ShowBoxOnce(BoundingBox box, Color color)
        {
            ShowBoxOnce(box.Min, box.Max, color);
        }

        internal void ShowBoxOnce(Vector3 min, Vector3 max)
        {
            ShowBoxOnce(min, max, Color.White);
        }

        internal void ShowBoxOnce(Vector3 min, Vector3 max, Color color)
        {
            lines.ShowLineOnce(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, min.Y, min.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, max.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, max.Y, min.Z), color);
            lines.ShowLineOnce(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, max.Y, min.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, min.Y, max.Z), new Vector3(max.X, min.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, max.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, min.Y, max.Z), new Vector3(min.X, max.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(max.X, min.Y, max.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, min.Y, min.Z), new Vector3(min.X, min.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(min.X, max.Y, min.Z), new Vector3(min.X, max.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(max.X, max.Y, min.Z), new Vector3(max.X, max.Y, max.Z), color);
            lines.ShowLineOnce(new Vector3(max.X, min.Y, min.Z), new Vector3(max.X, min.Y, max.Z), color);
        }
        #endregion

        #region 2D Boxes
        internal void ShowBox(String name, Rectangle box)
        {
            ShowBox(name, new Vector2(box.Left, box.Top), new Vector2(box.Right, box.Bottom), Color.White);
        }

        internal void ShowBox(String name, Rectangle box, Color color)
        {
            ShowBox(name, new Vector2(box.Left, box.Top), new Vector2(box.Right, box.Bottom), color);
        }

        internal void ShowBox(String name, Point min, Point max)
        {
            ShowBox(name, new Vector2(min.X, min.Y), new Vector2(min.X, min.Y), Color.White);
        }

        internal void ShowBox(String name, Vector2 min, Vector2 max)
        {
            ShowBox(name, min, max, Color.White);
        }

        internal void ShowBox(String name, Vector2 min, Vector2 max, Color color)
        {
            lines.ShowLine(name + "a1", new Vector2(min.X, min.Y), new Vector2(max.X, min.Y), color);
            lines.ShowLine(name + "a2", new Vector2(max.X, min.Y), new Vector2(max.X, max.Y), color);
            lines.ShowLine(name + "a3", new Vector2(max.X, max.Y), new Vector2(min.X, max.Y), color);
            lines.ShowLine(name + "a4", new Vector2(min.X, max.Y), new Vector2(min.X, min.Y), color);
        }

        internal void ShowBoxOnce(Rectangle box)
        {
            ShowBoxOnce(new Vector2(box.Left, box.Top), new Vector2(box.Right, box.Bottom), Color.White);
        }

        internal void ShowBoxOnce(Rectangle box, Color color)
        {
            ShowBoxOnce(new Vector2(box.Left, box.Top), new Vector2(box.Right, box.Bottom), color);
        }

        internal void ShowBoxOnce(Point min, Point max)
        {
            ShowBoxOnce(new Vector2(min.X, min.Y), new Vector2(min.X, min.Y), Color.White);
        }

        internal void ShowBoxOnce(Vector2 min, Vector2 max)
        {
            ShowBoxOnce(min, max, Color.White);
        }

        internal void ShowBoxOnce(Vector2 min, Vector2 max, Color color)
        {
            lines.ShowLineOnce(new Vector2(min.X, min.Y), new Vector2(max.X, min.Y), color);
            lines.ShowLineOnce(new Vector2(max.X, min.Y), new Vector2(max.X, max.Y), color);
            lines.ShowLineOnce(new Vector2(max.X, max.Y), new Vector2(min.X, max.Y), color);
            lines.ShowLineOnce(new Vector2(min.X, max.Y), new Vector2(min.X, min.Y), color);
        } 
        #endregion
    }
}

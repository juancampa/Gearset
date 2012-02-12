using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    internal class Vector2Drawer : Gear
    {
        private InternalLineDrawer lines;

        internal Vector2Drawer()
            : base(GearsetSettings.Instance.LineDrawerConfig)
        {
            lines = new InternalLineDrawer();
            lines.CoordinateSpace = CoordinateSpace.GameSpace;
            this.Children.Add(lines);
        }

        internal void ShowVector2(String name, Vector2 location, Vector2 vector)
        {
            ShowVector2(name, location, vector, Color.White);
        }

        internal void ShowVector2(String name, Vector2 location, Vector2 vector, Color color)
        {
            Vector2 v1 = location;
            Vector2 v2 = location + vector;
            // Difference vector
            Vector2 diff = v1 - v2;
            float distance = Vector2.Distance(location, v2);
            if (distance == 0)
                return;
            diff *= 1 / distance;
            // Craft a vector that is normal to diff
            Vector2 normal1 = new Vector2(diff.Y, -diff.X);

            // TODO: Move this value to a config
            normal1 *= 8;
            diff *= 8;
            lines.ShowLine(name + "1", v1, v2, color);
            lines.ShowLine(name + "2", v2 + normal1 + diff, v2, color);
            lines.ShowLine(name + "3", v2 - normal1 + diff, v2, color);
        }

        internal void ShowVector2Once(Vector2 location, Vector2 vector)
        {
            ShowVector2Once(location, vector, Color.White);
        }

        internal void ShowVector2Once(Vector2 location, Vector2 vector, Color color)
        {
            Vector2 v1 = location;
            Vector2 v2 = vector;
            // Difference vector
            Vector2 diff = v1 - v2;
            float distance = Vector2.Distance(location, v2);
            if (distance == 0)
                return;
            diff *= 1 / distance;
            // Craft a vector that is normal to diff
            Vector2 normal1 = new Vector2(diff.Y, -diff.X);

            // TODO: Move this value to a config
            normal1 *= 8;
            diff *= 8;
            lines.ShowLineOnce(v1, v2, color);
            lines.ShowLineOnce(v2 + normal1 + diff, v2, color);
            lines.ShowLineOnce(v2 - normal1 + diff, v2, color);
        }
    }
}

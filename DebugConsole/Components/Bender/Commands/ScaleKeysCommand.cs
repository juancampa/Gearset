using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Scales a set of keys.
    /// </summary>
    public class ScaleKeysCommand : CurveEditorCommand
    {
        private long[] affectedKeys;
        private Point[] normalizedPos;

        private Point handleOffset;
        private Point min;
        private Point max;
        private ScaleBoxHandle handle;
        private Point newMin;
        private Point newMax;

        public override bool CanUndo
        {
            get { return affectedKeys != null; }
        }

        /// <summary>
        /// Creates a new command to scale a set of keys.
        /// </summary>
        /// <param name="min">The minimum position of the scale box grabbing all selected keys</param>
        /// <param name="max">The maximum position of the scale box grabbing all selected keys</param>
        public ScaleKeysCommand(CurveEditorControl2 control, Point min, Point max, ScaleBoxHandle handle)
            : base(control)
        {
            // Store the parameters.
            this.min = min;
            this.max = max;
            this.newMin = min;
            this.newMax = max;
            this.handle = handle;

            // Store the current selection, if any.
            affectedKeys = new long[control.Selection.Count];
            normalizedPos = new Point[control.Selection.Count];
            int i = 0;
            foreach (KeyWrapper key in control.Selection)
            {
                affectedKeys[i] = key.Id;

                Point pos = key.GetPosition();
                normalizedPos[i].X = (pos.X - min.X) / (max.X - min.X);
                normalizedPos[i].Y = (pos.Y - min.Y) / (max.Y - min.Y);

                i++;
            }
        }

        public override void Do()
        {
            ScaleKeys(newMin, newMax);
        }

        public override void Undo()
        {
            ScaleKeys(min, max);
        }

        /// <summary>
        /// This method will move will update the offset and move the keys accordingly.
        /// This is to be used while dragging, before the mouse is released and the command
        /// added (without calling Do()) to the history.
        /// </summary>
        /// <param name="offset">offset in curve coords</param>
        public void UpdateOffsets(Point offset)
        {
            newMin = min;
            newMax = max;

            // Calculate the size of the new box.
            switch (handle)
            {
                case ScaleBoxHandle.BottomLeft:
                    newMin = Point.Add(min, (Vector)offset);
                    break;
                case ScaleBoxHandle.BottomRight:
                    newMin.Y = min.Y + offset.Y;
                    newMax.X = max.X + offset.X;
                    break;
                case ScaleBoxHandle.TopRight:
                    newMax = Point.Add(max, (Vector)offset);
                    break;
                case ScaleBoxHandle.TopLeft:
                    newMin.X = min.X + offset.X;
                    newMax.Y = max.Y + offset.Y;
                    break;
            }
            ScaleKeys(newMin, newMax);
            
        }

        /// <summary>
        /// Performs the actual movement of the keys.
        /// </summary>
        private void ScaleKeys(Point newMin, Point newMax)
        {
            for (int i = 0; i < affectedKeys.Length; i++)
            {
                // Remap to the new box;
                Point newPosition = new Point((newMax.X - newMin.X) * normalizedPos[i].X + newMin.X, (newMax.Y - newMin.Y) * normalizedPos[i].Y + newMin.Y);
                KeyWrapper k = Control.Keys[affectedKeys[i]];
                k.MoveKey((float)(newPosition.X - k.Key.Position), (float)(newPosition.Y - k.Key.Value));
            }
        }

    }

    public enum ScaleBoxHandle
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
    }
}

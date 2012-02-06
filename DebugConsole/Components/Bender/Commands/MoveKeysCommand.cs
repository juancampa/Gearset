using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Moves a set of keys.
    /// </summary>
    public class MoveKeysCommand : CurveEditorCommand
    {
        private long[] affectedKeys;
        private float positionOffset;
        private float valueOffset;

        public override bool CanUndo
        {
            get { return affectedKeys != null; }
        }

        /// <summary>
        /// Creates a new command to move a set of keys.
        /// </summary>
        public MoveKeysCommand(CurveEditorControl2 control, float positionOffset, float valueOffset)
            : base(control)
        {
            // Store the parameters.
            this.positionOffset = positionOffset;
            this.valueOffset = valueOffset;

            // Store the current selection, if any.
            affectedKeys = new long[control.Selection.Count];
            int i = 0;
            foreach (KeyWrapper key in control.Selection)
            {
                affectedKeys[i++] = key.Id;
            }
        }

        public override void Do()
        {
            MoveKeys(positionOffset, valueOffset);
        }

        public override void Undo()
        {
            MoveKeys(-positionOffset, -valueOffset);
        }

        /// <summary>
        /// This method will move will update the offset and move the keys accordingly.
        /// This is to be used while dragging, before the mouse is released and the command
        /// added (without calling Do()) to the history.
        /// </summary>
        public void UpdateOffsets(float positionOffset, float valueOffset)
        {
            MoveKeys(positionOffset - this.positionOffset, valueOffset - this.valueOffset);
            this.positionOffset = positionOffset;
            this.valueOffset = valueOffset;
        }

        /// <summary>
        /// Performs the actual movement of the keys.
        /// </summary>
        private void MoveKeys(float positionOffset, float valueOffset)
        {
            List<CurveWrapper> affectedCurves = new List<CurveWrapper>();
            for (int i = 0; i < affectedKeys.Length; i++)
            {
                KeyWrapper key = Control.Keys[affectedKeys[i]];
                key.MoveKey(positionOffset, valueOffset);

                // Create the set of curve affected by this move so we can recalculate
                // the auto tangents.
                if (!affectedCurves.Contains(key.Curve))
                    affectedCurves.Add(key.Curve);
            }

            // Compute all auto tangents.
            foreach (CurveWrapper curve in affectedCurves)
            {
                curve.ComputeTangents();
            }
        }

    }
}

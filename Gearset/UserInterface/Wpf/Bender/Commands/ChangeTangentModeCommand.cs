using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.UserInterface.Wpf.Bender;
using Microsoft.Xna.Framework;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Changes the tangent value of a given key.
    /// </summary>
    public class ChangeTangentModeCommand : CurveEditorCommand
    {
        private KeyTangentMode? newTangentInMode;
        private KeyTangentMode? newTangentOutMode;
        
        // Saved state.
        private long[] affectedKeys;
        private KeyTangentMode[] prevTangentInMode;
        private KeyTangentMode[] prevTangentOutMode;

        public override bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new command to select the given keys. You can pass null to deselect all.
        /// </summary>
        public ChangeTangentModeCommand(CurveEditorControl2 control, KeyTangentMode? newTangentInMode, KeyTangentMode? newTangentOutMode)
            : base(control)
        {
            // Store the parameters.
            this.newTangentInMode = newTangentInMode;
            this.newTangentOutMode = newTangentOutMode;

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
            // Do we need to save prev values?
            if (prevTangentInMode == null)
            {
                System.Diagnostics.Debug.Assert(prevTangentOutMode == null);
                prevTangentInMode = new KeyTangentMode[affectedKeys.Length];
                prevTangentOutMode = new KeyTangentMode[affectedKeys.Length];

                // Save the prev values.
                for (int i = 0; i < affectedKeys.Length; i++)
                {
                    prevTangentInMode[i] = Control.Keys[affectedKeys[i]].TangentInMode;
                    prevTangentInMode[i] = Control.Keys[affectedKeys[i]].TangentOutMode;
                }
            }

            HashSet<CurveWrapper> affectedCurves = new HashSet<CurveWrapper>();

            // Change the tangent modes.
            for (int i = 0; i < affectedKeys.Length; i++)
            {
                if (newTangentInMode.HasValue)
                    Control.Keys[affectedKeys[i]].TangentInMode = newTangentInMode.Value;
                if (newTangentOutMode.HasValue)
                    Control.Keys[affectedKeys[i]].TangentOutMode = newTangentOutMode.Value;
                affectedCurves.Add(Control.Keys[affectedKeys[i]].Curve);
            }

            // Recalc tangents on affected curves.
            foreach (var curve in affectedCurves)
                curve.ComputeTangents();

            // Redraw!
            Control.InvalidateVisual();
        }

        public override void Undo()
        {
            HashSet<CurveWrapper> affectedCurves = new HashSet<CurveWrapper>();

            // Revert to previous values.
            for (int i = 0; i < affectedKeys.Length; i++)
            {
                if (newTangentInMode.HasValue)
                    Control.Keys[affectedKeys[i]].TangentInMode = prevTangentInMode[i];
                if (newTangentOutMode.HasValue)
                    Control.Keys[affectedKeys[i]].TangentOutMode = prevTangentOutMode[i];
                affectedCurves.Add(Control.Keys[affectedKeys[i]].Curve);
            }

            // Recalc tangents on affected curves.
            foreach (var curve in affectedCurves)
                curve.ComputeTangents();

            // Redraw!
            Control.InvalidateVisual();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.UserInterface.Wpf.Bender;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Selects the provided set of keys.
    /// </summary>
    public class SelectKeysCommand : CurveEditorCommand
    {
        private long[] previousSelection;
        private long[] newSelection;

        public override bool CanUndo
        {
            get { return previousSelection != null; }
        }

        /// <summary>
        /// Creates a new command to select the given keys. You can pass null to deselect all.
        /// </summary>
        public SelectKeysCommand(CurveEditorControl2 control, IList<long> keysToSelect)
            : base(control)
        {
            int count = 0;
            // Store the new selection, if any.
            if (keysToSelect != null)
                count = keysToSelect.Count;
            newSelection = new long[count];
            for (int i = 0; i < count; i++)
            {
                newSelection[i] = keysToSelect[i];
            }
        }

        public override void Do()
        {
            // Save the previous selection before we make the change.
            if (previousSelection == null)
            {
                previousSelection = new long[Control.Selection.Count];
                for (int i = 0; i < Control.Selection.Count; i++)
                {
                    previousSelection[i] = Control.Selection[i].Id;
                }
            }

            // Change the selection
            Control.Selection.Clear();
            for (int i = 0; i < newSelection.Length; i++)
            {
                Control.Selection.Add(Control.Keys[newSelection[i]]);
            }
        }

        public override void Undo()
        {
            System.Diagnostics.Debug.Assert(previousSelection != null, "Inconsistent state.");

            // Change the selection
            Control.Selection.Clear();
            for (int i = 0; i < previousSelection.Length; i++)
            {
                Control.Selection.Add(Control.Keys[previousSelection[i]]);
            }
        }

    }
}

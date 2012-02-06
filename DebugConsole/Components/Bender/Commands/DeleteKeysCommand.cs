using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Selects the provided set of keys.
    /// </summary>
    public class DeleteKeysCommand : CurveEditorCommand
    {
        private List<KeyWrapper> deletedKeys;

        public override bool CanUndo
        {
            get { return deletedKeys != null; }
        }

        /// <summary>
        /// Creates a new command to select the given keys. You can pass null to deselect all.
        /// </summary>
        public DeleteKeysCommand(CurveEditorControl2 control)
            : base(control)
        {
        }

        public override void Do()
        {
            // Store the keys to be removed
            if (deletedKeys == null)
            {
                deletedKeys = new List<KeyWrapper>();
                foreach (KeyWrapper key in Control.Selection)
                {
                    deletedKeys.Add(key);
                }
            }
            // Delete them.
            foreach (KeyWrapper key in deletedKeys)
            {
                key.Curve.RemoveKey(key.Id);
            }
            // Remove the reference from the selection
            Control.Selection.Clear();
        }

        public override void Undo()
        {
            // Restore the keys removed.
            foreach (KeyWrapper key in deletedKeys)
            {
                key.Curve.RestoreKey(key);
                Control.Selection.Add(key);
            }
        }

    }
}

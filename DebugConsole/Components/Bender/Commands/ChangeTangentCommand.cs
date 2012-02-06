using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Changes the tangent value of a given key.
    /// </summary>
    public class ChangeTangentCommand : CurveEditorCommand
    {
        private long affectedKey;
        private TangentSelectionMode selectedTangent;
        private float tangentValue;

        // Saved state.
        private float prevTangentInValue;
        private float prevTangentOutValue;
        private KeyTangentMode prevTangentInMode;
        private KeyTangentMode prevTangentOutMode;

        public override bool CanUndo
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new command to select the given keys. You can pass null to deselect all.
        /// </summary>
        public ChangeTangentCommand(CurveEditorControl2 control, long keyId, TangentSelectionMode selectedTangent)
            : base(control)
        {
            // Store the parameters.
            this.selectedTangent = selectedTangent;
            this.affectedKey = keyId;

            KeyWrapper key = control.Keys[keyId];

            prevTangentInValue = key.Key.TangentIn;
            prevTangentOutValue = key.Key.TangentOut;
            prevTangentInMode = key.TangentInMode;
            prevTangentOutMode = key.TangentOutMode;
        }

        public override void Do()
        {
            ChangeTangent(tangentValue);
        }

        public override void Undo()
        {
            KeyWrapper key = Control.Keys[affectedKey];

            key.SetInTangent(prevTangentInValue);
            key.SetOutTangent(prevTangentOutValue);
            key.TangentInMode = prevTangentInMode;
            key.TangentOutMode = prevTangentOutMode;
        }

        /// <summary>
        /// This method will update the offset and update the key tangent accordingly.
        /// This is to be used while dragging, before the mouse is released and the command
        /// added (without calling Do()) to the history.
        /// </summary>
        public void UpdateOffset(float tangent)
        {
            ChangeTangent(tangent);
            this.tangentValue = tangent;
        }


        /// <summary>
        /// Performs the actual movement of the keys.
        /// </summary>
        private void ChangeTangent(float tangent)
        {
            KeyWrapper key = Control.Keys[affectedKey];
            if (selectedTangent == TangentSelectionMode.In)
                key.SetInTangent(tangent);
            else
                key.SetOutTangent(tangent);
        }

    }
}

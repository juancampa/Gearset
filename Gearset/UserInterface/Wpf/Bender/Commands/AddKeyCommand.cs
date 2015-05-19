using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.UserInterface.Wpf.Bender;
using Microsoft.Xna.Framework;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Selects the provided set of keys.
    /// </summary>
    public class AddKeyCommand : CurveEditorCommand
    {
        private long keyId;
        public long KeyId { get { return keyId; } }
        private long curveId;

        private float position;
        private float value;

        public override bool CanUndo
        {
            get { return keyId >= 0; }
        }

        /// <summary>
        /// Creates a new command to select the given keys. You can pass null to deselect all.
        /// </summary>
        public AddKeyCommand(CurveEditorControl2 control, long curveId, float position, float value)
            : base(control)
        {
            this.curveId = curveId;
            this.keyId = -1;
            this.position = position;
            this.value = value;
        }

        public override void Do()
        {
            KeyWrapper wrapper;
            if (keyId < 0)
            {
                wrapper = Control.Curves[curveId].AddKey(new CurveKey(position, value));
                keyId = wrapper.Id;
            }
            else
            {
                wrapper = Control.Curves[curveId].AddKey(new CurveKey(position, value), keyId);
            }
        }

        public override void Undo()
        {
            Control.Curves[curveId].RemoveKey(keyId);
        }

    }
}

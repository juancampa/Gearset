using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.UserInterface.Wpf.Bender;

namespace Gearset.Components.CurveEditorControl
{
    /// <summary>
    /// Base class for all curve editor commands.
    /// </summary>
    public abstract class CurveEditorCommand : IUndoable
    {
        public CurveEditorControl2 Control { get; private set; }
        public abstract bool CanUndo { get; }

        // Might be abstract but not needed yet.
        public bool CanRedo { get { return true; } }

        public CurveEditorCommand(CurveEditorControl2 control)
        {
            Control = control;
        }

        public abstract void Do(); 
        public abstract void Undo();
    }
}

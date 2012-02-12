using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset
{
    public class UndoEngine
    {
        /// <summary>
        /// History of commands for undo.
        /// </summary>
        private LinkedList<IUndoable> undoStack;

        /// <summary>
        /// History of commands for redo.
        /// </summary>
        private LinkedList<IUndoable> redoStack;

        public UndoEngine()
        {
            undoStack = new LinkedList<IUndoable>();
            redoStack = new LinkedList<IUndoable>();

#if DEBUG
            GearsetResources.Console.Inspect("Undo Engine", this);
#endif
        }

        /// <summary>
        /// Executes a command and adds it to the command history so it can
        /// be undone/redone.
        /// </summary>
        /// <param name="command">The command to execute and keep history of.</param>
        public void Execute(IUndoable command)
        {
            command.Do();
            AddCommand(command);
        }

        /// <summary>
        /// Undoes the last done command
        /// </summary>
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                IUndoable command = undoStack.Last.Value;
                if (command.CanUndo)
                {
                    command.Undo();
                    undoStack.RemoveLast();
                    redoStack.AddLast(command);
                }
                else
                {
#if DEBUG
                    // WHY CANT UNDO?
                    System.Diagnostics.Debugger.Break();
#endif
                }
            }
        }

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                IUndoable command = redoStack.Last.Value;
                if (command.CanRedo)
                {
                    command.Do();
                    redoStack.RemoveLast();
                    undoStack.AddLast(command);
                }
            }
        }

        /// <summary>
        /// Adds a command to the history without executing it. This is usefull if
        /// the command was executed somewhere else but still needs undo/redo.
        /// </summary>
        /// <param name="currentMover"></param>
        public void AddCommand(IUndoable command)
        {
            undoStack.AddLast(command);

            // Clear the redo stack (it might be empty already) because we just editted.
            redoStack.Clear();
        }
    }
}

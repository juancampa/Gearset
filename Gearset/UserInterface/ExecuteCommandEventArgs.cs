using System;

namespace Gearset.UserInterface
{
    public class ExecuteCommandEventArgs : EventArgs
    {
        public readonly string Command;
        public ExecuteCommandEventArgs(string command)
        {
            Command = command;
        }
    }
}

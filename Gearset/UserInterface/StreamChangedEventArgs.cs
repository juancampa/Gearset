using System;

namespace Gearset.UserInterface
{
    public class StreamChangedEventArgs : EventArgs
    {
        public readonly string Name;
        public readonly bool Enabled;
        public StreamChangedEventArgs(string name, bool enabled)
        {
            Name = name;
            Enabled = enabled;
        }
    }
}

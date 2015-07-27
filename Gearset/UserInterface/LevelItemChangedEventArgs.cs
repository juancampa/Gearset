using System;

namespace Gearset.UserInterface
{
    public class LevelItemChangedEventArgs : EventArgs
    {
        public readonly string Name;
        public readonly int LevelId;
        public readonly bool Enabled;
        public LevelItemChangedEventArgs(string name, int levelId, bool enabled)
        {
            Name = name;
            LevelId = levelId;
            Enabled = enabled;
        }
    }
}

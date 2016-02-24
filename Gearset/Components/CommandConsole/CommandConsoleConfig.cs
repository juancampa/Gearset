using System;
using System.Collections.Generic;

namespace Gearset.Components
{
    [Serializable]
    public class CommandConsoleConfig : GearConfig
    {
        [InspectorIgnore]
        public double Top { get; internal set; }

        [InspectorIgnore]
        public double Left { get; internal set; }

        [InspectorIgnore]
        public double Width { get; internal set; }

        [InspectorIgnore]
        public double Height { get; internal set; }

        [InspectorIgnore]
        public List<string> HiddenStreams { get; internal set; }

        public CommandConsoleConfig()
        {
            // Defaults
            Top = 300;
            Left = 140;
            Width = 700;
            Height = 340;

            HiddenStreams = new List<string>();
        }
    }
}

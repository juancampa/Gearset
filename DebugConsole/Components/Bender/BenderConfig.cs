using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components
{
    [Serializable]
    public class BenderConfig : GearConfig
    {
        [InspectorIgnoreAttribute]
        public double Top { get; internal set; }
        [InspectorIgnoreAttribute]
        public double Left { get; internal set; }
        [InspectorIgnoreAttribute]
        public double Width { get; internal set; }
        [InspectorIgnoreAttribute]
        public double Height { get; internal set; }

        public BenderConfig()
        {
            // Defaults
            Top = 300;
            Left = 40;
            Width = 700;
            Height = 340;
        }
    }
}

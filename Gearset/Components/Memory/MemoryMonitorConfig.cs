using System;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Memory
{
    [Serializable]
    public class MemoryMonitorConfig : GearConfig
    {
        [InspectorIgnore]
        public MemoryGraphUIConfig MemoryGraphConfig { get; internal set; }

        [Serializable]
        public abstract class UIViewConfig
        {
            [InspectorIgnore]
            public Vector2 Position { get; set; }

            [InspectorIgnore]
            public Vector2 Size { get; set; }

            [InspectorIgnore]
            public bool Visible { get; set; }

            protected UIViewConfig()
            {
                Visible = true;
            }

            public abstract Vector2 DefaultPosition { get;  }
            public abstract Vector2 DefaultSize { get; }
        }

        [Serializable]
        public class MemoryGraphUIConfig : UIViewConfig 
        {
            public override Vector2 DefaultPosition => new Vector2(100, 100);
            public override Vector2 DefaultSize => new Vector2(500, 65);
        }

        public MemoryMonitorConfig()
        {
            MemoryGraphConfig = new MemoryGraphUIConfig()
            {
                Visible = true,
            };
            MemoryGraphConfig.Position = MemoryGraphConfig.DefaultPosition;
            MemoryGraphConfig.Size = MemoryGraphConfig.DefaultSize;
        }
    }
}

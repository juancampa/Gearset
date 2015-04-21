using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    [Serializable]
    public class ProfilerConfig : GearConfig
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
        public List<String> HiddenStreams { get; internal set; }

        [InspectorIgnore]
        public TimeRulerUIViewConfig TimeRulerConfig { get; internal set; }

        [InspectorIgnore]
        public PerformanceGraphUIViewConfig PerformanceGraphConfig { get; internal set; }

        [Serializable]
        public class UIViewConfig
        {
            [InspectorIgnore]
            public Vector2 Position { get; internal set; }

            [InspectorIgnore]
            public Vector2 Size { get; internal set; }

            [InspectorIgnore]
            public bool Visible { get; internal set; }

            [InspectorIgnore]
            public int VisibleLevelsFlags { get; internal set; }

            public UIViewConfig()
            {
                Visible = true;
                VisibleLevelsFlags = 1;
            }
        }

        [Serializable]
        public class TimeRulerUIViewConfig : UIViewConfig { }

        [Serializable]
        public class PerformanceGraphUIViewConfig : UIViewConfig 
        {
            [InspectorIgnore]
            public uint SkipFrames { get; internal set; }
        }

        public ProfilerConfig()
        {
            // Defaults
            Top = 300;
            Left = 40;
            Width = 700;
            Height = 340;

            HiddenStreams = new List<string>();

            TimeRulerConfig = new TimeRulerUIViewConfig {
                Position = new Vector2(3, 3), 
                Size = new Vector2(400, 16), 
                Visible = true,
                VisibleLevelsFlags = 1
            };

            PerformanceGraphConfig = new PerformanceGraphUIViewConfig {
                Position = new Vector2(3, 20), 
                Size = new Vector2(100, 60), 
                Visible = true,
                SkipFrames = 0,
                VisibleLevelsFlags = 1 | 2 | 4
            };
        }
    }
}

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

        [InspectorIgnore]
        public ProfilerSummaryUIViewConfig ProfilerSummaryConfig { get; internal set; }

        [Serializable]
        public abstract class UIViewConfig
        {
            [InspectorIgnore]
            public Vector2 Position { get; internal set; }

            [InspectorIgnore]
            public Vector2 Size { get; internal set; }

            [InspectorIgnore]
            public bool Visible { get; internal set; }

            [InspectorIgnore]
            public int VisibleLevelsFlags { get; internal set; }

            protected UIViewConfig()
            {
                Visible = true;
                VisibleLevelsFlags = 1;
            }

            public abstract Vector2 DefaultPosition { get;  }
            public abstract Vector2 DefaultSize { get; }
        }

        [Serializable]
        public class TimeRulerUIViewConfig : UIViewConfig 
        {
            public override Vector2 DefaultPosition { get { return new Vector2(3, 3); } }
            public override Vector2 DefaultSize { get { return new Vector2(400, 16); } }
        }

        [Serializable]
        public class PerformanceGraphUIViewConfig : UIViewConfig 
        {
            [InspectorIgnore]
            public uint SkipFrames { get; internal set; }

            public override Vector2 DefaultPosition { get { return new Vector2(3, 95); } }
            public override Vector2 DefaultSize { get { return new Vector2(100, 60); } }
        }

        [Serializable]
        public class ProfilerSummaryUIViewConfig : UIViewConfig 
        {
            public override Vector2 DefaultPosition { get { return new Vector2(3, 180); } }
            public override Vector2 DefaultSize { get { return new Vector2(100, 60); } }
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
                Visible = true,
                VisibleLevelsFlags = 1
            };
            TimeRulerConfig.Position = TimeRulerConfig.DefaultPosition;
            TimeRulerConfig.Size = TimeRulerConfig.DefaultSize;

            PerformanceGraphConfig = new PerformanceGraphUIViewConfig {
                Visible = true,
                SkipFrames = 0,
                VisibleLevelsFlags = 1 | 2 | 4
            };
            PerformanceGraphConfig.Position = PerformanceGraphConfig.DefaultPosition;
            PerformanceGraphConfig.Size = PerformanceGraphConfig.DefaultSize;

            ProfilerSummaryConfig = new ProfilerSummaryUIViewConfig {
                Visible = true,
                VisibleLevelsFlags = 1 | 2 | 4
            };
            ProfilerSummaryConfig.Position = ProfilerSummaryConfig.DefaultPosition;
            ProfilerSummaryConfig.Size = ProfilerSummaryConfig.DefaultSize;
        }
    }
}

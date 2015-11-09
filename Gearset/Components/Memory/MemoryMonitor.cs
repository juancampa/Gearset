using System;
using Microsoft.Xna.Framework;
using Gearset.Components.Profiler;

namespace Gearset.Components.Memory
{
    public class MemoryMonitor : Gear
    {
        [Flags]
        internal enum CollectionType
        {
            None = 0,
            Gen0 = 1,
            Gen1 = 2,
            Gen2 = 4,
            Xbox360 = 8
        }

        internal class MemoryFrame
        {
            public long TotalMemoryBytes;
            public long TickMemoryBytes;
            public CollectionType CollectionType;
            public int Gen0CollectionCount;
            public int Gen1CollectionCount;
            public int Gen2CollectionCount;
        }
        
        internal const int MaxFrames = 480;

        //WeakReference _gcTracker = new WeakReference(new object());

        // Current frame count.
        public int FrameId { get; set; }
        internal readonly MemoryFrame[] Frames = new MemoryFrame[MaxFrames];

        const int OneMegabyte = 1024 * 1024;

        long _xbox360Memory;

        internal MemoryFrame CurrentFrame { get; private set; }
        internal MemoryFrame PreviousFrame { get; private set; }

        public long TotalMemoryBytes => Frames[FrameId].TotalMemoryBytes;
        public long TickMemoryBytes => Frames[FrameId].TickMemoryBytes;

        public int Gen0CollectionCount => Frames[FrameId].Gen0CollectionCount;
        public int Gen1CollectionCount => Frames[FrameId].Gen1CollectionCount;
        public int Gen2CollectionCount => Frames[FrameId].Gen2CollectionCount;

        public bool MonitorXbox360Garbage { get; set; } = true;

        public long TotalMemoryK => TotalMemoryBytes / 1024;
        public long TickMemoryK => TickMemoryBytes / 1024;

        //Temp Box drawer for performance grid
        internal TempBoxDrawer TempBoxDrawer = new TempBoxDrawer();

        public MemoryGraph MemoryGraph { get; private set; }

        public MemoryMonitorConfig Config => GearsetSettings.Instance.MemoryMonitorConfig;

        /// <summary>
        /// Gets or sets a value indicating how often the frame sampling occurs.
        /// </summary>
        /// <remarks>0 to sample every frame, 10 to sample every 10th frame, etc.</remarks>
        public uint SkipFrames { get; set; }
        
        readonly InternalLabeler _internalLabeler = new InternalLabeler();

        public InternalLineDrawer LineDrawer { get; }

        public MemoryMonitor() : base(GearsetSettings.Instance.MemoryMonitorConfig)
        {   
            for (var i = 0; i < Frames.Length; i++)
                Frames[i] = new MemoryFrame();
            
            LineDrawer = new InternalLineDrawer();
            Children.Add(LineDrawer);

            Children.Add(_internalLabeler);
            Children.Add(TempBoxDrawer);

            CreateMemoryGraph();
        }

        void CreateMemoryGraph()
        {
            var minSize = new Vector2(100, 16);
            var size = Vector2.Max(minSize, Config.MemoryGraphConfig.Size);

            MemoryGraph = new MemoryGraph(this, Config.MemoryGraphConfig, size) 
            {
                Visible = Config.MemoryGraphConfig.Visible
            };

            MemoryGraph.VisibleChanged += (sender, args) => 
            {
                Config.MemoryGraphConfig.Visible = MemoryGraph.Visible;
            };

            MemoryGraph.TitleBar.Dragged += (object sender, ref Vector2 args) => 
			{
                Config.MemoryGraphConfig.Position = MemoryGraph.Position;
			};

            MemoryGraph.Dragged += (object sender, ref Vector2 args) =>
            {
                Config.MemoryGraphConfig.Position = MemoryGraph.Position;
            };

            MemoryGraph.ScaleNob.Dragged += (object sender, ref Vector2 args) =>
            {
                Config.MemoryGraphConfig.Size = MemoryGraph.Size;
            };
        }

        public override void Update(GameTime gameTime)
        {
            PreviousFrame = Frames[FrameId];
            FrameId = ((FrameId + 1) % (Frames.Length));
            CurrentFrame = Frames[FrameId];

            CurrentFrame.TotalMemoryBytes = GC.GetTotalMemory(false);
            CurrentFrame.TickMemoryBytes = Math.Max(0, CurrentFrame.TotalMemoryBytes - PreviousFrame.TotalMemoryBytes);

            CurrentFrame.Gen0CollectionCount = GC.CollectionCount(0);
            CurrentFrame.Gen1CollectionCount = GC.CollectionCount(1);
            CurrentFrame.Gen2CollectionCount = GC.CollectionCount(2);

            CurrentFrame.CollectionType = CollectionType.None;
            if (CurrentFrame.Gen0CollectionCount != PreviousFrame.Gen0CollectionCount)
                CurrentFrame.CollectionType |= CollectionType.Gen0;

            if (CurrentFrame.Gen1CollectionCount != PreviousFrame.Gen1CollectionCount)
                CurrentFrame.CollectionType |= CollectionType.Gen1;

            if (CurrentFrame.Gen2CollectionCount != PreviousFrame.Gen2CollectionCount)
                CurrentFrame.CollectionType |= CollectionType.Gen2;

            if (MonitorXbox360Garbage)
            {
                _xbox360Memory += CurrentFrame.TickMemoryBytes;

                //Need to know if > 1mb as that is when we get a collection on the xbox
                if (_xbox360Memory > OneMegabyte)
                {
                    //XBox collection

                    CurrentFrame.CollectionType |= CollectionType.Xbox360;
                    _xbox360Memory = 0;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            MemoryGraph.Draw(_internalLabeler);
        }
    }
}

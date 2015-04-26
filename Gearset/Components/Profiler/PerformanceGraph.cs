using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    public class PerformanceGraph : UIView
    {
        class Frame
        {
            public readonly List<TimingInfo> TimingInfos = new List<TimingInfo>(16);
        }

        struct TimingInfo
        {
            public readonly int Level;
            public readonly float StartMilliseconds;
            public readonly float EndMilliseconds;
            public readonly Color Color;

            internal TimingInfo(int level, float startMilliseconds, float endMilliseconds, Color color)
            {
                Level = level;
                StartMilliseconds = startMilliseconds;
                EndMilliseconds = endMilliseconds;
                Color = color;
            }
        }

        internal const int MaxFrames = 120;

        private uint _frameCount;
        public uint DisplayedFrameCount
        {
            get { return _frameCount; }
            set { _frameCount = (uint)MathHelper.Clamp(value, 0, MaxFrames); }
        }

        protected int FrameCounter;

        private readonly Queue<Frame> _frames = new Queue<Frame>(MaxFrames);

        public ProfilerConfig Config { get { return GearsetSettings.Instance.ProfilerConfig; } }

        uint _skipFrames;
        
        /// <summary>
        /// Gets or sets a value indicating how often the frame sampling occurs.
        /// </summary>
        /// <remarks>0 to sample every frame, 10 to sample every 10th frame, etc.</remarks>
        public uint SkipFrames
        {
            get
            {
                return _skipFrames;
            }
            set
            {
                _skipFrames = value;
                if (SkipFramesChanged != null)
                    SkipFramesChanged(this, EventArgs.Empty);
                
                #if WINDOWS
                    if (SkipFramesChanged != null)
                        SkipFramesChanged(this, new PropertyChangedEventArgs("SkipFrames"));
                #endif
            }
        }

        internal event EventHandler SkipFramesChanged;

        internal PerformanceGraph(Profiler profiler, ProfilerConfig.UIViewConfig uiviewConfig, Vector2 size) : base(profiler, uiviewConfig, size)
        {
            for (var i = 0; i < MaxFrames; i++)
                _frames.Enqueue(new Frame());

            DisplayedFrameCount = MaxFrames;
        }

        internal void Draw(InternalLabeler labeler, Profiler.FrameLog frameLog)
        {
            if (Visible == false || Config.PerformanceGraphConfig.VisibleLevelsFlags == 0)
            {
                labeler.HideLabel("__performanceGraph");
                return;
            }

            DrawBorderLines(Color.Gray);
            GearsetResources.Console.SolidBoxDrawer.ShowGradientBoxOnce(Position, Position + Size, new Color(56, 56, 56, 150), new Color(16, 16, 16, 127));

            if (ScaleNob.IsMouseOver)
                ScaleNob.DrawBorderLines(Color.Gray);

            labeler.ShowLabel("__performanceGraph", Position + new Vector2(0, -12), "Performance Graph");
            
            FrameCounter++;
            if (FrameCounter > SkipFrames)
            {
                FrameCounter = 0;

                //If the frame buffer has capacity will just create a new one; otherwise we'll pop the oldest off and use that.
                Frame frame;
                if (_frames.Count == MaxFrames)
                {
                    frame = _frames.Dequeue();
                    frame.TimingInfos.Clear();
                }
                else
                    frame = new Frame();

                _frames.Enqueue(frame);

                //Populate the frame metrics
                for (var barId = 0; barId < frameLog.Levels.Length; barId++)
                {
                    var bar = frameLog.Levels[barId];
                    for (var j = 0; j < bar.MarkCount; ++j)
                    {
                        frame.TimingInfos.Add(new TimingInfo(
                            barId,
                            bar.Markers[j].BeginTime,
                            bar.Markers[j].EndTime,
                            bar.Markers[j].Color));
                    }
                }
            }

            const float frameSpan = 1.0f / 60.0f * 1000f;

            var msToPs = Height / frameSpan;

            //Only render the actual number of frames we are capturing - we may have space for e.g. 120 (2 seconds) but the user is only viewing 60 (1 second)
            var barWidth = Width / DisplayedFrameCount;
            var graphFloor = Position.Y + Size.Y;
            var position = new Vector2(Position.X, graphFloor);

            var s = new Vector2(barWidth, msToPs);

            //Set a pointer to the first frame to renders
            var frameId = MaxFrames - DisplayedFrameCount;
            foreach (var frame in _frames)
            {
                //Bail when we have drawn enough
                if (frameId >= MaxFrames)
                    break;
                
                frameId++;

                foreach (var timeInfo in frame.TimingInfos)
                {
                    if (Levels[timeInfo.Level].Enabled == false)
                        continue;

                    var durationMilliseconds = timeInfo.EndMilliseconds - timeInfo.StartMilliseconds;
                    if (durationMilliseconds <= 0)
                        continue;

                    s.Y = -durationMilliseconds*msToPs;
                    position.Y = graphFloor - (timeInfo.StartMilliseconds * msToPs);

                    Profiler.TempBoxDrawer.ShowGradientBoxOnce(position, position + s, timeInfo.Color, timeInfo.Color);
                }

                position.X += barWidth;
            }
        }
    }
}

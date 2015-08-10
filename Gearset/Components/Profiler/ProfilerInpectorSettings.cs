namespace Gearset.Components.Profiler
{
    /// <summary>
    /// A facade for Profiler settings.
    /// </summary>
    /// <remarks>
    /// Ideally utilised by the Inspector to hide lots of property noise - I still wanted to be able to throw a Profiler into the 
    /// inspector though so didn't want to tag the properties with [InspectorIgnore]
    /// </remarks>
    public class ProfilerInpectorSettings
    {
        readonly ProfilerManager _profiler;       

        public ProfilerInpectorSettings(ProfilerManager profiler)
        {
            _profiler = profiler;
        }

        public bool Sleep
        {
            get { return _profiler.Sleep; }
            set { _profiler.Sleep = value; }
        }

        public bool SkipUpdate
        {
            get { return _profiler.SkipUpdate; }
            set { _profiler.SkipUpdate = value; }
        }

        public uint MaxFrameCount
        {
            get { return PerformanceGraph.MaxFrames; }
        }

        public uint DisplayedFrameCount
        {
            get { return _profiler.PerformanceGraph.DisplayedFrameCount; }
            set { _profiler.PerformanceGraph.DisplayedFrameCount = value; }
        }

        public uint SkipFrames
        {
            get { return _profiler.PerformanceGraph.SkipFrames; }
            set { _profiler.PerformanceGraph.SkipFrames = value; }
        }

        public void ResetProfileWindows()
        {
            _profiler.TimeRuler.TopLeft = _profiler.Config.TimeRulerConfig.DefaultPosition;
            _profiler.TimeRuler.Size = _profiler.Config.TimeRulerConfig.DefaultSize;
            _profiler.PerformanceGraph.TopLeft = _profiler.Config.PerformanceGraphConfig.DefaultPosition;
            _profiler.PerformanceGraph.Size = _profiler.Config.PerformanceGraphConfig.DefaultSize;
            _profiler.ProfilerSummary.TopLeft = _profiler.Config.ProfilerSummaryConfig.DefaultPosition;
            _profiler.ProfilerSummary.Size = _profiler.Config.ProfilerSummaryConfig.DefaultSize;
        }
    }
}

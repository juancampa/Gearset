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
        readonly Profiler _profiler;       

        public ProfilerInpectorSettings(Profiler profiler)
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
    }
}

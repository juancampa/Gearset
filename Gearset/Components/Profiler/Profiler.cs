using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace Gearset.Components.Profiler
{
    public class Profiler : Gear
    {
        //Temp Box drawer for performance grid
        internal TempBoxDrawer TempBoxDrawer = new TempBoxDrawer();

        public class LevelItem : IComparable<LevelItem>, INotifyPropertyChanged
        {
            public int LevelId { get; private set; }
            public String Name { get; set; }

            private Boolean _enabled = true;
            public Boolean Enabled { get { return _enabled; } set { _enabled = value; OnPropertyChanged("Enabled"); } }

            public LevelItem(int levelId)
            {
                LevelId = levelId;
            }

            private void OnPropertyChanged(string p)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(p));
            }
            
            public event PropertyChangedEventHandler PropertyChanged;

            public int CompareTo(LevelItem other)
            {
                return String.Compare(Name, other.Name, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase);
            }
        }

        /// <summary>The maximum number of discrete heirarchical levels.</summary>
        public const int MaxLevels = 8;

        /// <summary>Maximum sample number for each level. </summary>
        const int MaxSamples = 2560;

        /// <summary>Maximum nest calls for each level.</summary>
        const int MaxNestCall = 32;

        /// <summary>Maximum display frames.</summary>
        const int MaxSampleFrames = 4;

        internal string[] LevelNames = new string[MaxLevels];

        protected bool RefreshSummary { get; private set; }
        public TimeRuler TimeRuler { get; private set; }
        public PerformanceGraph PerformanceGraph { get; private set; }

        public ProfilerConfig Config { get { return GearsetSettings.Instance.ProfilerConfig; } }

        //Settings the game can use for Profiling scenarios (e.g. to test if CPU/GPU bound)
        public bool Sleep { get; set; }
        public bool SkipUpdate { get; set; }

        /// <summary>
        /// Gets/Sets target sample frames.
        /// </summary>
        public int TargetSampleFrames { get; set; }        

        /// <summary>
        /// Duration (in frame count) for take snap shot of log.
        /// </summary>
        const int LogSnapDuration = 120;

        /// <summary>
        /// Marker structure.
        /// </summary>
        internal struct Marker
        {
            public int MarkerId;
            public float BeginTime;
            public float EndTime;
            public Color Color;
        }

        /// <summary>
        /// Collection of markers.
        /// </summary>
        internal class MarkerCollection
        {
            // Marker collection.
            public readonly Marker[] Markers = new Marker[MaxSamples];
            public int MarkCount;

            // Marker nest information.
            public readonly int[] MarkerNests = new int[MaxNestCall];
            public int NestCount;
        }

        /// <summary>
        /// Frame logging information.
        /// </summary>
        internal class FrameLog
        {
            public readonly MarkerCollection[] Levels;

            public FrameLog()
            {
                // Initialize markers.
                Levels = new MarkerCollection[MaxLevels];
                for (var i = 0; i < MaxLevels; ++i)
                    Levels[i] = new MarkerCollection();
            }
        }

        /// <summary>
        /// Marker information
        /// </summary>
        protected class MarkerInfo
        {
            // Name of marker.
            public readonly string Name;

            // Marker log.
            public readonly MarkerLog[] Logs = new MarkerLog[MaxLevels];

            public MarkerInfo(string name)
            {
                Name = name;
            }
        }

        /// <summary>
        /// Marker log information.
        /// </summary>
        public struct MarkerLog
        {
            public float SnapAvg { get; set; }

            public float Min;
            public float Max;
            public float Avg;

            public int Samples;

            public Color Color { get; set; }

            public bool Initialized;

            public string Name { get; set; }
            public string Level { get; set; }
        }

        /// <summary>
        /// Marker log information.
        /// </summary>
        public struct TimingSummaryItem
        {
            public float SnapAvg { get; set; }
            public string Color { get; set; }
            public string Name { get; set; }
            public string Level { get; set; }
        }

        // Logs for each frames.
        readonly FrameLog[] _logs;

        // Previous frame log.
        FrameLog _prevLog;

        // Current log.
        FrameLog _curLog;

        // Current frame count.
        int _frameCount;

        // Stopwatch for measure the time.
        readonly Stopwatch _stopwatch = new Stopwatch();

        // Marker information array.
        readonly List<MarkerInfo> _markers = new List<MarkerInfo>();

        // Dictionary that maps from marker name to marker id.
        readonly Dictionary<string, int> _markerNameToIdMap = new Dictionary<string, int>();

        readonly InternalLabeler _internalLabeler = new InternalLabeler();

        // You want to call StartFrame at beginning of Game.Update method.
        // But Game.Update gets calls multiple time when game runs slow in fixed time step mode.
        // In this case, we should ignore StartFrame call.
        // To do this, we just keep tracking of number of StartFrame calls until Draw gets called.
        int _updateCount;

        int _currentLevel = -1;
        readonly Dictionary<string, int> _nameMap = new Dictionary<string, int>();

        protected IEnumerable<MarkerInfo> Markers { get { return _markers; } }

        readonly object _locker = new object();

        public Profiler() : base(GearsetSettings.Instance.ProfilerConfig)
        {
            RefreshSummary = true;

            Children.Add(_internalLabeler);
            Children.Add(TempBoxDrawer);

            _logs = new FrameLog[2];
            for (var i = 0; i < _logs.Length; ++i)
                _logs[i] = new FrameLog();

            GenerateLevelNames();

            CreateTimeRuler();
            CreatePerformanceGraph();
        }

        void GenerateLevelNames()
        {
            for(var i = 0; i < MaxLevels; i++)
                LevelNames[i] = "Level " + (i + 1);
        }

        internal string GetLevelNameFromLevelId(int levelId)
        {
            return LevelNames[levelId];
        }
        
        void CreateTimeRuler()
        {
            TargetSampleFrames = 1;

            var minSize = new Vector2(100, 16);
            var size = Vector2.Max(minSize, Config.TimeRulerConfig.Size);

            TimeRuler = new TimeRuler(this, Config.TimeRulerConfig, size, TargetSampleFrames);

            TimeRuler.Visible = Config.TimeRulerConfig.Visible;

            TimeRuler.VisibleChanged += (sender, args) => { 
                Config.TimeRulerConfig.Visible = TimeRuler.Visible; 
            };
            
            TimeRuler.LevelsEnabledChanged += (sender, args) => { 
                Config.TimeRulerConfig.VisibleLevelsFlags = TimeRuler.VisibleLevelsFlags; 
            };
                        
            TimeRuler.Dragged += (object sender, ref Vector2 args) => { 
                Config.TimeRulerConfig.Position = TimeRuler.Position; 
            };
            
            TimeRuler.ScaleNob.Dragged += (object sender, ref Vector2 args) => { 
                Config.TimeRulerConfig.Size = TimeRuler.Size; 
            };
        }

        void CreatePerformanceGraph()
        {
            var minSize = new Vector2(100, 16);
            var size = Vector2.Max(minSize, Config.PerformanceGraphConfig.Size);

            PerformanceGraph = new PerformanceGraph(this, Config.PerformanceGraphConfig, size);

            PerformanceGraph.Visible = Config.PerformanceGraphConfig.Visible;

            PerformanceGraph.VisibleChanged += (sender, args) => {
                Config.PerformanceGraphConfig.Visible = PerformanceGraph.Visible;
            };

            PerformanceGraph.LevelsEnabledChanged += (sender, args) =>
            {
                Config.PerformanceGraphConfig.VisibleLevelsFlags = PerformanceGraph.VisibleLevelsFlags;
            };

            PerformanceGraph.Dragged += (object sender, ref Vector2 args) =>
            {
                Config.PerformanceGraphConfig.Position = PerformanceGraph.Position;
            };

            PerformanceGraph.ScaleNob.Dragged += (object sender, ref Vector2 args) =>
            {
                Config.PerformanceGraphConfig.Size = PerformanceGraph.Size;
            };
        }

        public void StartFrame()
        {
            lock (_locker)
            {
                RefreshSummary = false;

                // We skip reset frame when this method gets called multiple times.
                var count = Interlocked.Increment(ref _updateCount);
                if (Visible && (1 < count && count < MaxSampleFrames))
                    return;

                // Update current frame log.
                _prevLog = _logs[_frameCount++ & 0x1];
                _curLog = _logs[_frameCount & 0x1];

                var endFrameTime = (float)_stopwatch.Elapsed.TotalMilliseconds;

                // Update marker and create a log.
                for (var levelIdx = 0; levelIdx < _prevLog.Levels.Length; ++levelIdx)
                {
                    var prevLevel = _prevLog.Levels[levelIdx];
                    var nextLevel = _curLog.Levels[levelIdx];

                    // Re-open marker that didn't get called EndMark in previous frame.
                    for (var nest = 0; nest < prevLevel.NestCount; ++nest)
                    {
                        var markerIdx = prevLevel.MarkerNests[nest];

                        prevLevel.Markers[markerIdx].EndTime = endFrameTime;

                        nextLevel.MarkerNests[nest] = nest;
                        nextLevel.Markers[nest].MarkerId = prevLevel.Markers[markerIdx].MarkerId;
                        nextLevel.Markers[nest].BeginTime = 0;
                        nextLevel.Markers[nest].EndTime = -1;
                        nextLevel.Markers[nest].Color = prevLevel.Markers[markerIdx].Color;
                    }

                    // Update marker log.
                    for (var markerIdx = 0; markerIdx < prevLevel.MarkCount; ++markerIdx)
                    {
                        var duration = prevLevel.Markers[markerIdx].EndTime - prevLevel.Markers[markerIdx].BeginTime;
                        var markerId = prevLevel.Markers[markerIdx].MarkerId;
                        var m = _markers[markerId];

                        m.Logs[levelIdx].Color = prevLevel.Markers[markerIdx].Color;

                        if (!m.Logs[levelIdx].Initialized)
                        {
                            // First frame process.
                            m.Logs[levelIdx].Min = duration;
                            m.Logs[levelIdx].Max = duration;
                            m.Logs[levelIdx].Avg = duration;

                            m.Logs[levelIdx].Initialized = true;
                        }
                        else
                        {
                            // Process after first frame.
                            m.Logs[levelIdx].Min = Math.Min(m.Logs[levelIdx].Min, duration);
                            m.Logs[levelIdx].Max = Math.Min(m.Logs[levelIdx].Max, duration);
                            m.Logs[levelIdx].Avg += duration;
                            m.Logs[levelIdx].Avg *= 0.5f;

                            if (m.Logs[levelIdx].Samples++ >= LogSnapDuration)
                            {
                                RefreshSummary = true;

                                //m.Logs[levelIdx].SnapMin = m.Logs[levelIdx].Min;
                                //m.Logs[levelIdx].SnapMax = m.Logs[levelIdx].Max;
                                m.Logs[levelIdx].SnapAvg = m.Logs[levelIdx].Avg;
                                m.Logs[levelIdx].Samples = 0;
                            }
                        }
                    }

                    nextLevel.MarkCount = prevLevel.NestCount;
                    nextLevel.NestCount = prevLevel.NestCount;
                }

                // Start measuring.
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }

        public void BeginMark(string markerName, Color color)
        {
            lock (_locker)
            {
                //Look up the name in map or create a new level if this is a new name
                int levelIndex;
                if (_nameMap.ContainsKey(markerName)) {
                    levelIndex = _nameMap[markerName];
                } else {
                    _currentLevel++;
                    levelIndex = _currentLevel;
                    _nameMap[markerName] = levelIndex;
                }

                BeginMark(levelIndex, markerName, color);
            }
        }

        void BeginMark(int levelIndex, string markerName, Color color)
        {
            if (levelIndex < 0 || levelIndex >= MaxLevels)
                throw new ArgumentOutOfRangeException("levelIndex");

            var level = _curLog.Levels[levelIndex];

            if (level.MarkCount >= MaxSamples)
                throw new OverflowException("Exceeded sample count.\n" + "Either set larger number to TimeRuler.MaxSmpale or" + "lower sample count.");

            if (level.NestCount >= MaxNestCall)
                throw new OverflowException("Exceeded nest count.\n" + "Either set larget number to TimeRuler.MaxNestCall or" + "lower nest calls.");

            // Gets registered marker.
            int markerId;
            if (!_markerNameToIdMap.TryGetValue(markerName, out markerId))
            {
                // Register this if this marker is not registered.
                markerId = _markers.Count;
                _markerNameToIdMap.Add(markerName, markerId);
                _markers.Add(new MarkerInfo(markerName));
            }

            // Start measuring.
            level.MarkerNests[level.NestCount++] = level.MarkCount;

            // Fill marker parameters.
            level.Markers[level.MarkCount].MarkerId = markerId;
            level.Markers[level.MarkCount].Color = color;
            level.Markers[level.MarkCount].BeginTime = (float)_stopwatch.Elapsed.TotalMilliseconds;
            level.Markers[level.MarkCount].EndTime = -1;

            level.MarkCount++;
        }

        public void EndMark(string markerName)
        {
            lock (_locker)
            {
                int levelIndex;
                if (_nameMap.ContainsKey(markerName)) {
                    levelIndex = _nameMap[markerName];
                } else {
                    //End called before Begin throw!
                    throw new InvalidOperationException("EndMark could not find name: " + markerName + ". Ensure you call BeginMark first.");
                }

                var nestLevels = EndMark(levelIndex, markerName);
                if (nestLevels == 0)
                {
                    _nameMap.Remove(markerName);
                    _currentLevel--;
                }
            }
        }

        int EndMark(int levelIndex, string markerName)
        { 
            if (levelIndex < 0 || levelIndex >= MaxLevels)
                throw new ArgumentOutOfRangeException("levelIndex");

            var level = _curLog.Levels[levelIndex];

            if (level.NestCount <= 0)
                throw new InvalidOperationException("Call BeingMark method before call EndMark method.");

            int markerId;
            if (!_markerNameToIdMap.TryGetValue(markerName, out markerId))
                throw new InvalidOperationException(String.Format("Maker '{0}' is not registered." + "Make sure you specifed same name as you used for BeginMark" + " method.", markerName));

            var markerIdx = level.MarkerNests[--level.NestCount];
            if (level.Markers[markerIdx].MarkerId != markerId)
                throw new InvalidOperationException("Incorrect call order of BeginMark/EndMark method." + "You call it like BeginMark(A), BeginMark(B), EndMark(B), EndMark(A)" + " But you can't call it like " + "BeginMark(A), BeginMark(B), EndMark(A), EndMark(B).");
                
            level.Markers[markerIdx].EndTime = (float)_stopwatch.Elapsed.TotalMilliseconds;

            return level.NestCount;
        }

        /// <summary>
        /// Get average time of given level index and marker name.
        /// </summary>
        /// <param name="levelIndex">Index of level</param>
        /// <param name="markerName">name of marker</param>
        /// <returns>average spending time in ms.</returns>
        public float GetAverageTimeInMilliseconds(int levelIndex, string markerName)
        {
            if (levelIndex < 0 || levelIndex >= MaxLevels)
                throw new ArgumentOutOfRangeException("levelIndex");

            float result = 0;
            int markerId;
            if (_markerNameToIdMap.TryGetValue(markerName, out markerId))
                result = _markers[markerId].Logs[levelIndex].Avg;

            return result;
        }

        public void ResetMarkerLog()
        {
            lock (_locker)
            {
                foreach (var markerInfo in _markers)
                {
                    for (var i = 0; i < markerInfo.Logs.Length; ++i)
                    {
                        markerInfo.Logs[i].Initialized = false;
                        markerInfo.Logs[i].SnapAvg = 0;

                        markerInfo.Logs[i].Min = 0;
                        markerInfo.Logs[i].Max = 0;
                        markerInfo.Logs[i].Avg = 0;

                        markerInfo.Logs[i].Samples = 0;
                    }
                }
            }
        }

        public bool DoUpdate()
        {
            //If the sleep has no effect, I must be GPU bound.
            //If skipping Update speeds things up, I must be CPU bound.
            //If skipping Update has no effect but sleeping does slow things down, the two must be evenly balanced.

            if (Sleep)
                Thread.Sleep(1);

            return !SkipUpdate;
        }

        public override void Draw(GameTime gameTime)
        {
            // Just to make sure we're only doing this one per frame.
            if (GearsetResources.CurrentRenderPass != RenderPass.BasicEffectPass)
                return;

            // Reset update count.
            Interlocked.Exchange(ref _updateCount, 0);

            TimeRuler.Draw(_prevLog);
            PerformanceGraph.Draw(_internalLabeler, _prevLog);
        }
    }
}

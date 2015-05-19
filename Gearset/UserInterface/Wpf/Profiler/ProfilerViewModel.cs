#if WPF
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;

    namespace Gearset.UserInterface.Wpf.Profiler
    {
        public class ProfilerViewModel
        {
            const int MaxLevels = 8;

            public class LevelItem : IComparable<LevelItem>, INotifyPropertyChanged
            {
                public int LevelId { get; private set; }
                public String Name { get; set; }

                private Boolean _enabled = true;
                public Boolean Enabled 
                { 
                    get { return _enabled; } 
                    set { _enabled = value; OnPropertyChanged("Enabled"); }
                }

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

            public class LevelItemChangedEventArgs : EventArgs
            {
                public readonly string Key;
                public readonly int LevelId;
                public readonly bool Enabled;
                public LevelItemChangedEventArgs(string key, int levelId, bool enabled)
                {
                    Key = key;
                    LevelId = levelId;
                    Enabled = enabled;
                }
            }

            public event EventHandler<LevelItemChangedEventArgs> LevelItemChanged;

            internal string[] LevelNames = new string[MaxLevels];

            public readonly ObservableCollection<LevelItem> TimeRulerLevels;
            public readonly ObservableCollection<LevelItem> PerformanceGraphLevels;
            public readonly ObservableCollection<LevelItem> ProfilerSummaryLevels;

            public ProfilerViewModel(int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels)
            {
                GenerateLevelNames();

                TimeRulerLevels = CreateLevels("TimeRuler", enabledTimeRulerLevels);
                PerformanceGraphLevels = CreateLevels("PerformanceGraph", enabledPerformanceGraphLevels);
                ProfilerSummaryLevels = CreateLevels("ProfilerSummary", enabledProfilerSummaryLevels);
            }

            void GenerateLevelNames()
            {
                for (var i = 0; i < MaxLevels; i++)
                    LevelNames[i] = "Level " + (i + 1);
            }

            internal string GetLevelNameFromLevelId(int levelId)
            {
                return LevelNames[levelId];
            }

            ObservableCollection<LevelItem> CreateLevels(string key, int enabledLevels)
            {
                var levels = new ObservableCollection<LevelItem>();

                for (var i = 0; i < MaxLevels; i++)
                {
                    var level = new LevelItem(i);
                    level.Name = GetLevelNameFromLevelId(i);
                    level.Enabled = (enabledLevels & (1 << i)) > 0;

                    level.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName != "Enabled")
                            return;

                        var levelItem = (LevelItem)sender;
                        var e = new LevelItemChangedEventArgs(key, levelItem.LevelId, levelItem.Enabled);
                        OnLevelItemChanged(e);
                    };

                    levels.Add(level);
                }

                return levels;
            }

            public void EnableAllTimeRulerLevels()
            {
                SetAllLevels(TimeRulerLevels, enabled: true);
            }

            public void DisableAllTimeRulerLevels()
            {
                SetAllLevels(TimeRulerLevels, enabled: false);
            }

            public void EnableAllPerformanceGraphLevels()
            {
                SetAllLevels(PerformanceGraphLevels, enabled: true);
            }

            public void DisableAllPerformanceGraphLevels()
            {
                SetAllLevels(PerformanceGraphLevels, enabled: false);
            }

            public void EnableAllProfilerSummaryLevels()
            {
                SetAllLevels(ProfilerSummaryLevels, enabled: true);
            }

            public void DisableAllProfilerSummaryLevels()
            {
                SetAllLevels(ProfilerSummaryLevels, enabled: false);
            }

            static void SetAllLevels(IEnumerable<LevelItem> levels, bool enabled)
            {
                foreach (var level in levels)
                {
                    level.Enabled = enabled;
                }
            }

            protected virtual void OnLevelItemChanged(LevelItemChangedEventArgs e)
            {
                var handler = LevelItemChanged;
                if (handler != null)
                    handler(this, e);
            }
        }
    }
#endif
using System;
using System.Collections.ObjectModel;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// MVVM View Model for the Profiler Window.
    /// </summary>
    public class ProfilerWindowViewModel : WindowViewModel
    {
        enum ActiveItem
        {
            PerformanceGraph,
            TimeRuler,
            SummaryLog
        }

        const int MaxLevels = 8;

        public class ProfilerLevelChangedEventArgs : EventArgs
        {
            public readonly string Key;
            public readonly int LevelId;
            public readonly bool Enabled;
            public ProfilerLevelChangedEventArgs(string key, int levelId, bool enabled)
            {
                Key = key;
                LevelId = levelId;
                Enabled = enabled;
            }
        }

        public event EventHandler<ProfilerLevelChangedEventArgs> ProfilerLevelChanged;

        Visibility _performanceGraphVisibility;
        public Visibility PerformanceGraphVisibility
        {
            get { return _performanceGraphVisibility; }
            set { SetProperty(ref _performanceGraphVisibility, value); }
        }

        Visibility _timeRulerVisibility;
        public Visibility TimeRulerVisibility
        {
            get { return _timeRulerVisibility; }
            set { SetProperty(ref _timeRulerVisibility, value); }
        }

        Visibility _summaryLogVisibility;
        public Visibility SummaryLogVisibility
        {
            get { return _summaryLogVisibility; }
            set { SetProperty(ref _summaryLogVisibility, value); }
        }

        bool _performanceGraphActive;
        public bool PerformanceGraphActive
        {
            get { return _performanceGraphActive; }
            set { SetProperty(ref _performanceGraphActive, value); }
        }

        bool _timeRulerActive;
        public bool TimeRulerActive
        {
            get { return _timeRulerActive; }
            set { SetProperty(ref _timeRulerActive, value); }
        }

        bool _summaryLogActive;
        public bool SummaryLogActive
        {
            get { return _summaryLogActive; }
            set { SetProperty(ref _summaryLogActive, value); }
        }

        public ICommand PerformanceGraphButtonClick { get; set; }
        public ICommand TimeRulerGraphButtonClick { get; set; }
        public ICommand SummaryLogButtonClick { get; set; }

        ObservableCollection<ProfilerLevel> _pglevels;
        ObservableCollection<ProfilerLevel> _trlevels;
        ObservableCollection<ProfilerLevel> _pslevels;

        public ObservableCollection<ProfilerLevel> PgLevels
        {
            get { return _pglevels; }
            set { SetProperty(ref _pglevels, value); }
        }

        public ObservableCollection<ProfilerLevel> TrLevels
        {
            get { return _trlevels; }
            set { SetProperty(ref _trlevels, value); }
        }

        public ObservableCollection<ProfilerLevel> PsLevels
        {
            get { return _pslevels; }
            set { SetProperty(ref _pslevels, value); }
        }

        public ProfilerWindowViewModel(int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels)
        {
            Title = "Profiler";

            SetActiveItem(ActiveItem.PerformanceGraph);

            PerformanceGraphButtonClick = new RelayCommand(e => { SetActiveItem(ActiveItem.PerformanceGraph); });
            TimeRulerGraphButtonClick = new RelayCommand(e => { SetActiveItem(ActiveItem.TimeRuler); });
            SummaryLogButtonClick = new RelayCommand(e => { SetActiveItem(ActiveItem.SummaryLog); });

            TrLevels = CreateProfilerLevels("TimeRuler", enabledTimeRulerLevels);
            PgLevels = CreateProfilerLevels("PerformanceGraph", enabledPerformanceGraphLevels);
            PsLevels = CreateProfilerLevels("ProfilerSummary", enabledProfilerSummaryLevels);
        }

        void SetActiveItem(ActiveItem activeItem)
        {
            switch (activeItem)
            {
                default:
                    PerformanceGraphVisibility = Visibility.Visible;
                    TimeRulerVisibility = Visibility.Collapsed;
                    SummaryLogVisibility = Visibility.Collapsed;

                    PerformanceGraphActive = true;
                    TimeRulerActive = false;
                    SummaryLogActive = false;
                    break;

                case ActiveItem.TimeRuler:
                    PerformanceGraphVisibility = Visibility.Collapsed;
                    TimeRulerVisibility = Visibility.Visible;
                    SummaryLogVisibility = Visibility.Collapsed;

                    PerformanceGraphActive = false;
                    TimeRulerActive = true;
                    SummaryLogActive = false;
                    break;

                case ActiveItem.SummaryLog:
                    PerformanceGraphVisibility = Visibility.Collapsed;
                    TimeRulerVisibility = Visibility.Collapsed;
                    SummaryLogVisibility = Visibility.Visible;

                    PerformanceGraphActive = false;
                    TimeRulerActive = false;
                    SummaryLogActive = true;
                    break;
            }
        }

        ObservableCollection<ProfilerLevel> CreateProfilerLevels(string key, int enabledLevels)
        {
            var levels = new ObservableCollection<ProfilerLevel>();
            for (var i = 0; i < MaxLevels; i++)
            {
                var levelItem = new ProfilerLevel(i) {Name = "Level " + (i + 1), Enabled = (enabledLevels & (1 << i)) > 0};
                levelItem.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName != "Enabled")
                        return;

                    var level = (ProfilerLevel)sender;
                    var e = new ProfilerLevelChangedEventArgs(key, level.LevelId, level.Enabled);
                    OnProfilerLevelChanged(e);
                };
                levels.Add(levelItem);
            }

            return levels;
        }

        protected virtual void OnProfilerLevelChanged(ProfilerLevelChangedEventArgs e)
        {
            var handler = ProfilerLevelChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    public abstract class UIView : UI.Window
#if WINDOWS
        , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
#else
    {
#endif
        protected readonly Profiler Profiler;

        bool _visible;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TimeRuler"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                if (VisibleChanged != null)
                    VisibleChanged(this, EventArgs.Empty);
                
                #if WINDOWS
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Visible"));
                #endif
            }
        }

        internal event EventHandler VisibleChanged;

        internal ObservableCollection<Profiler.LevelItem> Levels = new ObservableCollection<Profiler.LevelItem>();

        protected UIView(Profiler profiler, ProfilerConfig.UIViewConfig uiviewConfig, Vector2 size)
            : base(uiviewConfig.Position, size) 
        {
            Profiler = profiler;

            Visible = true;

            VisibleLevelsFlags = uiviewConfig.VisibleLevelsFlags;

            for(var i = 0; i < Profiler.MaxLevels; i++)
            {
                var levelItem = new Profiler.LevelItem(i) { Name = Profiler.GetLevelNameFromLevelId(i), Enabled = IsVisibleLevelsFlagSet(i)};
                Levels.Add(levelItem);

                levelItem.PropertyChanged += (sender, args) => { 
                    if (args.PropertyName == "Enabled")
                        SyncVisibleLevelsFlags((Profiler.LevelItem)sender);
                };
            }
        }

        internal event EventHandler LevelsEnabledChanged;

        public void EnableAllLevels()
        {
            foreach (var level in Levels)
                level.Enabled = true;
        }

        public void DisableAllLevels()
        {
            foreach (var level in Levels)
                level.Enabled = false;
        }

        public int VisibleLevelsFlags { get; private set; }

        static int GetFlagFromLevelId(int levelId)
        {
            return (1 << levelId);
        }

        bool IsVisibleLevelsFlagSet(int levelId)
        {
            var flag = GetFlagFromLevelId(levelId);
            return (VisibleLevelsFlags & flag) != 0;
        }

        void SyncVisibleLevelsFlags(Profiler.LevelItem levelItem)
        {
            var flag = GetFlagFromLevelId(levelItem.LevelId);

            if (levelItem.Enabled)
                VisibleLevelsFlags |= flag;
            else
                VisibleLevelsFlags = VisibleLevelsFlags & ~flag;

            if (LevelsEnabledChanged != null)
                LevelsEnabledChanged(this, EventArgs.Empty);
        }
    }
}

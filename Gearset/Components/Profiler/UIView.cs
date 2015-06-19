using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    public abstract class UIView : UI.Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected readonly ProfilerManager Profiler;

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
                
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Visible"));
            }
        }

        protected void OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        internal event EventHandler VisibleChanged;

        protected UIView(ProfilerManager profiler, ProfilerConfig.UIViewConfig uiviewConfig, Vector2 size)
            : base(uiviewConfig.Position, size) 
        {
            Profiler = profiler;

            Visible = true;

            VisibleLevelsFlags = uiviewConfig.VisibleLevelsFlags;
        }

        public void EnableAllLevels()
        {
            VisibleLevelsFlags = byte.MaxValue;
        }

        public void DisableAllLevels()
        {
            VisibleLevelsFlags = 0;
        }

        public int VisibleLevelsFlags { get; set; }

        static int GetFlagFromLevelId(int levelId)
        {
            return (1 << levelId);
        }

        public bool IsVisibleLevelsFlagSet(int levelId)
        {
            var flag = GetFlagFromLevelId(levelId);
            return (VisibleLevelsFlags & flag) != 0;
        }

        public void SetLevel(int levelId, bool enabled)
        {
            var flag = GetFlagFromLevelId(levelId);
            if (enabled)
                VisibleLevelsFlags |= flag;
            else
                VisibleLevelsFlags &= ~flag;
        }
    }
}

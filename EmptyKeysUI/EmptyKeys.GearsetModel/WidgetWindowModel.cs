using System;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// MVVM View Model for the Widget Window.
    /// </summary>
    public class WidgetWindowViewModel : ViewModelBase
    {
        bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled, value); }
        }

        bool _inspectorWindowVisible;
        public bool InspectorWindowVisible
        {
            get { return _inspectorWindowVisible; }
            set { SetProperty(ref _inspectorWindowVisible, value); }
        }

        bool _loggerWindowVisible;
        public bool LoggerWindowVisible
        {
            get { return _loggerWindowVisible; }
            set { SetProperty(ref _loggerWindowVisible, value); }
        }

        bool _finderWindowVisible;
        public bool FinderWindowVisible
        {
            get { return _finderWindowVisible; }
            set { SetProperty(ref _finderWindowVisible, value); }
        }

        bool _profilerWindowVisible;
        public bool ProfilerWindowVisible
        {
            get { return _profilerWindowVisible; }
            set { SetProperty(ref _profilerWindowVisible, value); }
        }

        bool _commandConsoleWindowVisible;
        public bool CommandConsoleWindowVisible
        {
            get { return _commandConsoleWindowVisible; }
            set { SetProperty(ref _commandConsoleWindowVisible, value); }
        }

        float _sliderValue;
        public float SliderValue
        {
            get { return _sliderValue; }
            set { SetProperty(ref _sliderValue, value); }
        }


        public event EventHandler MasterSwitchButtonClicked;
        public event EventHandler InspectorButtonClicked;
        public event EventHandler LoggerButtonClicked;
        public event EventHandler FinderButtonClicked;
        public event EventHandler ProfilerButtonClicked;
        public event EventHandler CommandConsoleButtonClicked;

        public ICommand MasterSwitchButtonClick { get; set; }
        public ICommand InspectorButtonClick { get; set; }
        public ICommand LoggerButtonClick { get; set; }
        public ICommand FinderButtonClick { get; set; }
        public ICommand ProfilerButtonClick { get; set; }
        public ICommand CommandConsoleButtonClick { get; set; }

        public WidgetWindowViewModel()
        {
            SliderValue = 1.0f;

            MasterSwitchButtonClick = new RelayCommand(e => { OnMasterSwitchButtonClicked(EventArgs.Empty); });
            InspectorButtonClick = new RelayCommand(e => { OnInspectorButtonClicked(EventArgs.Empty); });
            LoggerButtonClick = new RelayCommand(e => { OnLoggerButtonClicked(EventArgs.Empty); });
            FinderButtonClick = new RelayCommand(e => { OnFinderButtonClicked(EventArgs.Empty); });
            ProfilerButtonClick = new RelayCommand(e => { OnProfilerButtonClicked(EventArgs.Empty); });
            CommandConsoleButtonClick = new RelayCommand(e => { OnCommandConsoleButtonClicked(EventArgs.Empty); });
        }

        protected void OnMasterSwitchButtonClicked(EventArgs e)
        {
            var handler = MasterSwitchButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        protected void OnInspectorButtonClicked(EventArgs e)
        {
            var handler = InspectorButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        protected void OnLoggerButtonClicked(EventArgs e)
        {
            var handler = LoggerButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        protected void OnFinderButtonClicked(EventArgs e)
        {
            var handler = FinderButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        protected void OnProfilerButtonClicked(EventArgs e)
        {
            var handler = ProfilerButtonClicked;
            if (handler != null)
                handler(this, e);
        }

        protected void OnCommandConsoleButtonClicked(EventArgs e)
        {
            var handler = CommandConsoleButtonClicked;
            if (handler != null)
                handler(this, e);
        }
    }
}

using Gearset.UserInterface;

namespace Gearset.Components.Logger
{
    public class LoggerManager : Gear
    {
        public LoggerConfig Config { get { return GearsetSettings.Instance.LoggerConfig; } }

        readonly IUserInterface _userInterface;

        public LoggerManager(IUserInterface userInterface)
            : base(GearsetSettings.Instance.LoggerConfig)
        {
            _userInterface = userInterface;
            _userInterface.CreateLogger(Config);
            _userInterface.StreamChanged += StreamChanged;
        }

        public void SaveLogToFile()
        {
            _userInterface.SaveLogToFile();
        }

        public void SaveLogToFile(string filename)
        {
            _userInterface.SaveLogToFile(filename);
        }

        protected override void OnVisibleChanged()
        {
            _userInterface.LoggerVisible = Visible;
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        public void Log(string streamName, string format, object arg0) 
        { 
            Log(streamName, string.Format(format, arg0)); 
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        public void Log(string streamName, string format, object arg0, object arg1) 
        {
            Log(streamName, string.Format(format, arg0, arg1)); 
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        /// <param name="arg2">The third format parameter</param>
        public void Log(string streamName, string format, object arg0, object arg1, object arg2) 
        { 
            Log(streamName, string.Format(format, arg0, arg1, arg2)); 
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="args">The format parameters</param>
        public void Log(string streamName, string format, params object[] args) 
        { 
            Log(streamName, string.Format(format, args)); 
        }

        /// <summary>
        /// Los a message to the specified stream.
        /// </summary>
        /// <param name="streamName">Name of the Stream to log the message to</param>
        /// <param name="message">Message to log</param>
        public void Log(string streamName, string message)
        {
            _userInterface.Log(streamName, message, GearsetResources.Console.UpdateCount);
        }

        /// <summary>
        /// Logs the specified message in the default stream.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            _userInterface.Log(message, GearsetResources.Console.UpdateCount);
        }

        void StreamChanged(object sender, StreamChangedEventArgs e)
        {         
            if (e.Enabled && Config.HiddenStreams.Contains(e.Name))
                Config.HiddenStreams.Remove(e.Name);
            else if (!e.Enabled && !(Config.HiddenStreams.Contains(e.Name)))
                Config.HiddenStreams.Add(e.Name);
        }
    }
}

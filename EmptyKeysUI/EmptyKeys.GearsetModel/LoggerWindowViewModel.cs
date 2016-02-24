using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// MVVM View Model for the Logger Window.
    /// </summary>
    public class LoggerWindowViewModel : WindowViewModel
    {
        public class StreamChangedEventArgs : EventArgs
        {
            public readonly string Name;
            public readonly bool Enabled;
            public StreamChangedEventArgs(string name, bool enabled)
            {
                Name = name;
                Enabled = enabled;
            }
        }

        public event EventHandler<StreamChangedEventArgs> StreamChanged;

        readonly StreamItem _defaultStream;

        readonly SolidColorBrush[] _colors = { 
            new SolidColorBrush(new ColorW(200, 200, 200, 255)),
            new SolidColorBrush(new ColorW(128, 200, 200, 255)),
            new SolidColorBrush(new ColorW(200, 200, 128, 255)),
            new SolidColorBrush(new ColorW(200, 128, 200, 255)),
            new SolidColorBrush(new ColorW(128, 128, 200, 255)),
            new SolidColorBrush(new ColorW(128, 200, 128, 255)),
            new SolidColorBrush(new ColorW(200, 128, 128, 255)),
            new SolidColorBrush(new ColorW(150, 110, 110, 255)),
            new SolidColorBrush(new ColorW(110, 150, 110, 255)),
            new SolidColorBrush(new ColorW(110, 110, 150, 255)),
            new SolidColorBrush(new ColorW(150, 150, 110, 255)),
            new SolidColorBrush(new ColorW(150, 110, 150, 255)),
            new SolidColorBrush(new ColorW(110, 150, 150, 255)),
        };

        private int _currentColor;

        ObservableCollection<StreamItem> _streams = new ObservableCollection<StreamItem>();
        readonly ObservableCollection<LogItem> _logItems;

        public ObservableCollection<StreamItem> Streams
        {
            get { return _streams; }
            set { SetProperty(ref _streams, value); }
        }

        public IEnumerable<LogItem> VisibleLogItems
        {
            get
            {
                foreach (var logItem in _logItems)
                {
                    if (logItem.Stream.Enabled == false)
                        continue;

                    yield return logItem;
                }
            }
        }

        public ICommand StreamClick { get; set; }

        public LoggerWindowViewModel()
        {
            Title = "Logger";

            Streams = new ObservableCollection<StreamItem>();
            _logItems = new ObservableCollection<LogItem>();

            _defaultStream = AddStreamItem("Default");
        }

        public StreamItem Log(String message, int updateNumber)
        {
            _logItems.Add(new LogItem { Stream = _defaultStream, Content = message, UpdateNumber = updateNumber, Color = _defaultStream.Color });
            return _defaultStream;
        }

        public StreamItem Log(String streamName, String message, int updateNumber)
        {
            var stream = SearchStream(streamName);
            if (stream == null)
            {
                stream = AddStreamItem(streamName);

                //Repeat the colors.
                if (_currentColor >= _colors.Length)
                    _currentColor = 0;
            }

            //Log even if the stream is disabled.
            var logItem = new LogItem { Stream = stream, Content = message, UpdateNumber = updateNumber, Color = stream.Color };
            _logItems.Add(logItem);
            return stream;
        }

        StreamItem AddStreamItem(string streamName)
        {
            var stream = new StreamItem()
            {
                Enabled = true,
                Name = streamName,
                Color = _colors[(_currentColor++)]
            };

            stream.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Enabled")
                {
                    var streamItem = (StreamItem)sender;
                    var e = new StreamChangedEventArgs(streamItem.Name, streamItem.Enabled);
                    OnStreamChanged(e);
                }
            };

            Streams.Add(stream);

            return stream;
        }

        private StreamItem SearchStream(String name)
        {
            foreach (var streamItem in Streams)
            {
                if (streamItem.Name == name)
                    return streamItem;
            }
            return null;
        }

        protected virtual void OnStreamChanged(StreamChangedEventArgs e)
        {
            var handler = StreamChanged;
            if (handler != null)
                handler(this, e);
        }
    }
}

#if WPF
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Media;

    namespace Gearset.UserInterface.Wpf.Logger
    {
        public class LoggerViewModel
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

            public class StreamItem : IComparable<StreamItem>, INotifyPropertyChanged
            {
                public String Name { get; set; }

                Boolean _enabled = true;
                public Boolean Enabled 
                { 
                    get { return _enabled; } 
                    set { _enabled = value; OnPropertyChanged("Enabled"); } 
                }

                public Brush Color { get; set; }

                private void OnPropertyChanged(string p)
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(p));
                }

                public event PropertyChangedEventHandler PropertyChanged;

                public int CompareTo(StreamItem other)
                {
                    return String.Compare(Name, other.Name, true, CultureInfo.InvariantCulture);
                }
            }

            public class LogItem
            {
                /// <summary>
                /// The background color to use for this Log, it is the same for 
                /// items in the same update.
                /// </summary>
                /// <value>The color.</value>
                public Brush Color { get; set; }

                /// <summary>
                /// The number of the update where this log was generated
                /// </summary>
                public int UpdateNumber { get; set; }

                /// <summary>
                /// The name of the string where this logItem belongs.
                /// </summary>
                public StreamItem Stream { get; set; }

                /// <summary>
                /// The actual contents of the log
                /// </summary>
                public String Content { get; set; }
            }

            readonly SolidColorBrush[] _colors = { 
                new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)),
                new SolidColorBrush(Color.FromArgb(255, 128, 200, 200)),
                new SolidColorBrush(Color.FromArgb(255, 200, 200, 128)),
                new SolidColorBrush(Color.FromArgb(255, 200, 128, 200)),
                new SolidColorBrush(Color.FromArgb(255, 128, 128, 200)),
                new SolidColorBrush(Color.FromArgb(255, 128, 200, 128)),
                new SolidColorBrush(Color.FromArgb(255, 200, 128, 128)),
                new SolidColorBrush(Color.FromArgb(255, 150, 110, 110)),
                new SolidColorBrush(Color.FromArgb(255, 110, 150, 110)),
                new SolidColorBrush(Color.FromArgb(255, 110, 110, 150)),
                new SolidColorBrush(Color.FromArgb(255, 150, 150, 110)),
                new SolidColorBrush(Color.FromArgb(255, 150, 110, 150)),
                new SolidColorBrush(Color.FromArgb(255, 110, 150, 150)),
            };

            int _currentColor;

            readonly StreamItem _defaultStream;
            public ObservableCollection<StreamItem> Streams;
            public FixedLengthQueue<LogItem> LogItems;
            public FixedLengthQueue<LogItem> LogItemsOld;

            public LoggerViewModel()
            {
                Streams = new ObservableCollection<StreamItem>();
                LogItems = new FixedLengthQueue<LogItem>(500);
                LogItemsOld = new FixedLengthQueue<LogItem>(10000);

                _defaultStream = AddStreamItem("Default");
            }

            public StreamItem Log(String message, int updateNumber)
            {
                LogItems.Enqueue(new LogItem { Stream = _defaultStream, Content = message, UpdateNumber = updateNumber, Color = _defaultStream.Color });
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
                LogItems.Enqueue(logItem);
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

            StreamItem SearchStream(String name)
            {
                foreach (var streamItem in Streams)
                {
                    if (streamItem.Name == name)
                        return streamItem;
                }
                return null;
            }

            public void EnableAllStreams()
            {
                foreach (var stream in Streams)
                {
                    stream.Enabled = true;
                }
            }

            public void DisableAllStreams()
            {
                foreach (var stream in Streams)
                {
                    stream.Enabled = false;
                }
            }

            protected virtual void OnStreamChanged(StreamChangedEventArgs e)
            {
                var handler = StreamChanged;
                if (handler != null)
                    handler(this, e);
            }
        }
    }
#endif
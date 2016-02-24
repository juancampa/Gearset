using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Gearset.Components.CommandConsole;

#if WPF
    namespace Gearset.UserInterface.Wpf.CommandConsole
    {  
        public class CommandConsoleViewModel : INotifyPropertyChanged
        {    
            readonly SolidColorBrush[] _colors = {
                new SolidColorBrush(Color.FromArgb(255, 127, 140, 141)),
                new SolidColorBrush(Color.FromArgb(255, 236, 240, 241)),
                new SolidColorBrush(Color.FromArgb(255, 241, 196, 15)),
                new SolidColorBrush(Color.FromArgb(255, 192, 57, 43))
            };

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string p)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(p));
            }

        public class EchoItem
            {
                public Brush Color { get; set; }

                public string Text { get; set; }
            }

            string _commandText;
            public string CommandText
            {
                get { return _commandText; }
                set
                {
                    _commandText = value;
                    OnPropertyChanged("CommandText");
                }
            }
        
            public ObservableCollection<EchoItem> Output { get; set; }

            public CommandConsoleViewModel()
            {
                Output = new ObservableCollection<EchoItem>();
            }

            public void ClearOutput()
            {
                Output.Clear();
            }

            public void EchoCommand(CommandConsoleManager.DebugCommandMessage messageType, string text)
            {
                Output.Add(new EchoItem{ Color = _colors[(int)messageType], Text = text });
            }
        }
    }
#endif
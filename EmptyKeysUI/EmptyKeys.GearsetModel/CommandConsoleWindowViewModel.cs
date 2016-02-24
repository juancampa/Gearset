using System;
using System.Collections.ObjectModel;
using EmptyKeys.UserInterface.Input;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// MVVM View Model for the Command Console Window.
    /// </summary>
    public class CommandConsoleWindowViewModel : WindowViewModel
    {
        readonly SolidColorBrush[] _colors = { 
            new SolidColorBrush(new ColorW(127, 140, 141, 255)),
            new SolidColorBrush(new ColorW(236, 240, 241, 255)),
            new SolidColorBrush(new ColorW(241, 196, 15, 255)),
            new SolidColorBrush(new ColorW(192, 57, 43, 255)),
        };

        public event EventHandler Execute;

        public ICommand ExecuteButtonClick { get; set; }

        string _commandText = string.Empty;
        public string CommandText
        {
            get { return _commandText; }
            set
            {
                CommandTextEmpty = (value ?? string.Empty).Length <= 0;

                SetProperty(ref _commandText, value);
            }
        }

        bool _commandTextEmpty;
        public bool CommandTextEmpty
        {
            get { return _commandTextEmpty; }
            private set { SetProperty(ref _commandTextEmpty, value); }
        }

        public ObservableCollection<EchoItem> Output { get; set; }


        public ICommand ClearClick { get; set; }

        public CommandConsoleWindowViewModel()
        {
            Title = "Command Console";

            Output = new ObservableCollection<EchoItem>();

            ExecuteButtonClick = new RelayCommand(e => { OnExecuteCommand(EventArgs.Empty); });
        }

        public void ClearOutput()
        {
            Output.Clear();
        }

        public void EchoCommand(int messageType, string text)
        {
            if (messageType < 0 || messageType >= _colors.Length)
                messageType = 0;

            Output.Add(new EchoItem { Color = _colors[messageType], Text = text });
        }

        public void CommandTextKeyDown2(object source, KeyEventArgs e)
        {

        }

        protected void OnExecuteCommand(EventArgs e)
        {
            var handler = Execute;
            if (handler != null)
                handler(this, e);

            CommandText = string.Empty;
        }
    }
}

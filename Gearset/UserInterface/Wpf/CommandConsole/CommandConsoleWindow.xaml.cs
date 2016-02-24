using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Gearset.UserInterface.Wpf;

namespace Gearset.UserInterface.Wpf.CommandConsole
{
    /// <summary>
    /// Interaction logic for CommandConsoleWindow.xaml
    /// </summary>
    public partial class CommandConsoleWindow : IWindow
    {
        public bool WasHiddenByGameMinimize { get; set; }

        public event EventHandler ClearOutput;
        public event EventHandler<ExecuteCommandEventArgs> ExecuteCommand;

        public event EventHandler PreviousCommand;
        public event EventHandler NextCommand;

        public CommandConsoleWindow()
        {
            InitializeComponent();

            Closing += CommandConsoleWindow_Closing;
        }

        public void CommandConsoleWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = ClearOutput;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    OnPreviousCommand();
                    break;

                case Key.Down:
                    OnNextCommand();
                    break;

                case Key.Enter:
                    ExecuteCurrentCommand();
                    break;
            }
        }

        void OnPreviousCommand()
        {
            var handler = PreviousCommand;
            if (handler != null)
                handler(this, EventArgs.Empty);

            CommandTextBox.CaretIndex = CommandTextBox.Text?.Length ?? 0;
        }

        void OnNextCommand()
        {
            var handler = NextCommand;
            if (handler != null)
                handler(this, EventArgs.Empty);

            CommandTextBox.CaretIndex = CommandTextBox.Text?.Length ?? 0;
        }

        void ExecuteCurrentCommand()
        {
            var handler = ExecuteCommand;
            if (handler != null)
                handler(this, new ExecuteCommandEventArgs(CommandTextBox.Text));

            CommandTextBox.Text = string.Empty;
        }
    }
}

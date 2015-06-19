using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Gearset.UserInterface.Wpf;

namespace Gearset.UserInterface.Wpf.Logger
{
    /// <summary>
    /// Interaction logic for LoggerWindow.xaml
    /// </summary>
    public partial class LoggerWindow : IWindow
    {
        public ListView StreamsListView { get { return StreamListBox; } }
        public ListView LogListView { get { return LogListBox; } }

        public bool WasHiddenByGameMinimize { get; set; }

        public event EventHandler EnableAllStreams;
        public event EventHandler DisableAllStreams;
        public event EventHandler SaveLogFile;

        public LoggerWindow()
        {
            InitializeComponent();

            Closing += LoggerWindow_Closing;
        }

        public event EventHandler<SoloRequestedEventArgs> SoloRequested;

        public void LoggerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var handler = SaveLogFile;
            if (handler != null)
                handler(sender, EventArgs.Empty);
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

        private void Solo_Click(object sender, RoutedEventArgs e)
        {
            var handler = SoloRequested;
            if (handler != null)
                handler(this, new SoloRequestedEventArgs((Gearset.UserInterface.Wpf.Logger.LoggerViewModel.StreamItem)((FrameworkElement)e.OriginalSource).DataContext));
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = EnableAllStreams;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = DisableAllStreams;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }
    }

    public class SoloRequestedEventArgs : EventArgs
    {
        public Gearset.UserInterface.Wpf.Logger.LoggerViewModel.StreamItem StreamItem { get; private set; }
        public SoloRequestedEventArgs(Gearset.UserInterface.Wpf.Logger.LoggerViewModel.StreamItem item)
        {
            StreamItem = item;
        }
    }
}

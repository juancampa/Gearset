using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gearset.Components.Persistor;
using System.Collections.ObjectModel;
using System.Data;

namespace Gearset.Components.Logger
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class LoggerWindow : Window
    {
        internal bool WasHiddenByGameMinimize { get; set; }

        public LoggerWindow()
        {
            InitializeComponent();

            Closing += new System.ComponentModel.CancelEventHandler(LoggerWindow_Closing);
        }


        internal event EventHandler<SoloRequestedEventArgs> SoloRequested;

        public void LoggerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.SaveLogToFile();
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
            if (this.WindowState == System.Windows.WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;
            else
                this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void Solo_Click(object sender, RoutedEventArgs e)
        {
            //e.OriginalSource.D
            if (SoloRequested != null)
                SoloRequested(this, new SoloRequestedEventArgs((StreamItem)((FrameworkElement)e.OriginalSource).DataContext));
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Logger.DisableAllStreams();
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Logger.EnableAllStreams();
        }


        
    }

    internal class SoloRequestedEventArgs : EventArgs
    {
        internal StreamItem StreamItem { get; private set; }
        public SoloRequestedEventArgs(StreamItem item)
        {
            this.StreamItem = item;
        }
    }
}

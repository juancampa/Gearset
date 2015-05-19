using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gearset.UserInterface.Wpf;

namespace Gearset.UserInterface.Wpf.Profiler
{
    /// <summary>
    /// Interaction logic for ProfilerWindow.xaml
    /// </summary>
    public partial class ProfilerWindow : IWindow
    {
        public event EventHandler EnableAllTimeRulerLevels;
        public event EventHandler DisableAllTimeRulerLevels;
        public event EventHandler EnableAllPerformanceGraphLevels;
        public event EventHandler DisableAllPerformanceGraphLevels;
        public event EventHandler EnableAllProfilerSummaryLevels;
        public event EventHandler DisableAllProfilerSummaryLevels;

        public ListView TrLevelsListBox { get { return trLevelsListBox; } }
        public ListView PgLevelsListBox { get { return pgLevelsListBox; } }
        public ListView PsLevelsListBox { get { return psLevelsListBox; } }

        public bool WasHiddenByGameMinimize { get; set; }

        public ProfilerWindow()
        {
            InitializeComponent();

            Closing += ProfilerWindowClosing;
        }

        public void ProfilerWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void TitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void CloseClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void MaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        void trEnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = EnableAllTimeRulerLevels;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        void trDisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = DisableAllTimeRulerLevels;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        void pgEnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = EnableAllPerformanceGraphLevels;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        void pgDisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = DisableAllPerformanceGraphLevels;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        void psEnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = EnableAllProfilerSummaryLevels;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        void psDisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            var handler = DisableAllProfilerSummaryLevels;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }
    }

}

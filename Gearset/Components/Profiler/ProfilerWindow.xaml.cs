using System.Windows;
using System.Windows.Input;

namespace Gearset.Components.Profiler
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ProfilerWindow : Window
    {
        internal bool WasHiddenByGameMinimize { get; set; }

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

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.SaveLogToFile();
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

        private void Solo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void trEnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Profiler.TimeRuler.EnableAllLevels();
        }

        private void trDisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Profiler.TimeRuler.DisableAllLevels();
        }

        private void pgEnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Profiler.PerformanceGraph.EnableAllLevels();
        }

        private void pgDisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            GearsetResources.Console.Profiler.PerformanceGraph.DisableAllLevels();
        }
    }

}

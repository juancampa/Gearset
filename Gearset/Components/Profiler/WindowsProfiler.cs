#if WINDOWS
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    internal class WindowsProfiler : Profiler
    {
        internal ProfilerWindow Window { get; private set; }
        private bool _prifilerWindowLocationChanged;

        //Circle buffer for WPF window summary - flip flop between two lists to break item source binding and force refresh
        readonly List<TimingSummaryItem>[] _summaryItems = { new List<TimingSummaryItem>(), new List<TimingSummaryItem>() };
        int _summaryLogId;

        internal WindowsProfiler() 
        {
            CreateProfilerWindow();
        }

        void CreateProfilerWindow()
        {
            Window = new ProfilerWindow
            {
                Top = Config.Top,
                Left = Config.Left,
                Width = Config.Width,
                Height = Config.Height
            };

            Window.IsVisibleChanged += ProfilerIsVisibleChanged;

            WindowHelper.EnsureOnScreen(Window);

            if (Config.Visible)
                Window.Show();

            Window.LocationChanged += ProfilerLocationChanged;
            Window.SizeChanged += ProfilerSizeChanged;

            Window.trLevelsListBox.DataContext = TimeRuler.Levels;
            Window.pgLevelsListBox.DataContext = PerformanceGraph.Levels;
        }

        void ProfilerIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Config.Visible = Window.IsVisible;
        }

        protected override void OnVisibleChanged()
        {
            if (Window != null)
                Window.Visibility = Visible ? Visibility.Visible : Visibility.Hidden;
        }


        void ProfilerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _prifilerWindowLocationChanged = true;
        }

        void ProfilerLocationChanged(object sender, EventArgs e)
        {
            _prifilerWindowLocationChanged = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_prifilerWindowLocationChanged)
            {
                _prifilerWindowLocationChanged = false;
                Config.Top = Window.Top;
                Config.Left = Window.Left;
                Config.Width = Window.Width;
                Config.Height = Window.Height;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // Just to make sure we're only doing this one per frame.
            if (GearsetResources.CurrentRenderPass != RenderPass.BasicEffectPass)
                return;

            RefreshTimingSummary();
        }

        void RefreshTimingSummary()
        {
            if (!Window.IsVisible || !RefreshSummary)
                return;

            //Get the next buffer to use - this flip flopping forces the WPF list to refresh it's ItemSource
            var summaryItems = _summaryItems[_summaryLogId++ % _summaryItems.Length];
            summaryItems.Clear();

            foreach (var markerInfo in Markers)
            {
                for (var i = 0; i < MaxLevels; ++i)
                {
                    if (!markerInfo.Logs[i].Initialized)
                        continue;

                    summaryItems.Add(new TimingSummaryItem
                    {
                        Name = markerInfo.Name,
                        SnapAvg = markerInfo.Logs[i].SnapAvg,
                        Level = GetLevelNameFromLevelId(i),
                        Color = System.Drawing.ColorTranslator.ToHtml(System.Drawing.ColorTranslator.FromWin32((int)markerInfo.Logs[i].Color.PackedValue))
                    });
                }
            }

            //Update the Profiler window - this seems to do it in a threadsafe manner without causing spikes in the XNA game.
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                Window.TimingItems.ItemsSource = summaryItems;
                var view = (CollectionView)CollectionViewSource.GetDefaultView(Window.TimingItems.ItemsSource);
                if (!view.CanGroup)
                    return;

                var groupDescription = new PropertyGroupDescription("Level");
                if (view.GroupDescriptions == null)
                    return;

                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(groupDescription);
            }));
        }
    }
}
#endif

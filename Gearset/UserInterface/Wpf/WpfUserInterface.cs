﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Media;
using Gearset.Components;
using Gearset.Components.Profiler;
using Gearset.UserInterface.Wpf.Bender;
using Gearset.UserInterface.Wpf.Finder;
using Gearset.Components.InspectorWPF;
using Gearset.UserInterface.Wpf.Logger;
using Gearset.UserInterface.Wpf.Profiler;
using Gearset.UserInterface.Wpf.Widget;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gearset.UserInterface.Wpf
{
    public class WpfUserInterface : UserInterface
    {
        readonly List<IWindow> _windows = new List<IWindow>();
            
        //Widget
        WidgetWindow _widgetWindow;
        WidgetViewModel _widgetViewModel;

        //Logger
        LoggerWindow _loggerWindow;
        LoggerViewModel _loggerViewModel;
        LoggerConfig _loggerConfig;
        ICollectionView _filteredView;
        ScrollViewer _scrollViewer;
        bool _loggerLocationJustChanged;

        //Profiler
        ProfilerWindow _profilerWindow;
        ProfilerViewModel _profilerViewModel;
        ProfilerConfig _profilerConfig;
        bool _profilerLocationJustChanged;

        //Finder
        FinderWindow _finderWindow;
        FinderConfig _finderConfig;
        bool _finderLocationJustChanged;

        //Bender
        CurveEditorWindow _curveEditorWindow;
        CurveTreeViewModel _curveTreeViewModel;
        BenderConfig _benderConfig;
        bool _benderLocationJustChanged;

        //Inspector
        InspectorUI _inspectorUI;

        public WpfUserInterface(GraphicsDevice graphicsDevice, int width, int height)
            : base(graphicsDevice, width, height)
        {

        }

        public override void Initialise(ContentManager content, int width, int height)
        {

        }

        public override void CreateWidget()
        {
            _widgetViewModel = new WidgetViewModel();

            _widgetWindow = new WidgetWindow {DataContext = GearsetSettings.Instance};

            _widgetWindow.ButtonList.ItemsSource = _widgetViewModel.ButtonActions;
            _widgetWindow.Show();

            _windows.Add(_widgetWindow);
        }

        public override void CreateInspector(InspectorManager inspectorManager)
        {
            InspectorNode.ExtensionMethodTypes.Add(typeof(InspectorExtensionsMethods));

            _inspectorUI = new InspectorUI(inspectorManager.Config, _windows);
        }

        public override void CreateLogger(LoggerConfig config)
        {
            _loggerConfig = config;

            _loggerViewModel = new LoggerViewModel();
            _loggerViewModel.StreamChanged += (sender, args) =>
            {
                OnStreamChanged(new StreamChangedEventArgs(args.Name, args.Enabled));
            };

            _filteredView = CollectionViewSource.GetDefaultView(_loggerViewModel.LogItems);
            _filteredView.Filter = a => ((LoggerViewModel.LogItem)a).Stream.Enabled;
            _filteredView.GroupDescriptions.Add(new PropertyGroupDescription("UpdateNumber"));

            _loggerWindow = new LoggerWindow
            {
                Top = config.Top,
                Left = config.Left,
                Width = config.Width,
                Height = config.Height
            };
            _loggerWindow.StreamsListView.DataContext = _loggerViewModel.Streams;
            _loggerWindow.LogListView.DataContext = _filteredView;

            _loggerWindow.SaveLogFile += (sender, args) => { SaveLogToFile(); };
            _loggerWindow.EnableAllStreams += (sender, args) => { _loggerViewModel.EnableAllStreams(); };
            _loggerWindow.DisableAllStreams += (sender, args) => { _loggerViewModel.DisableAllStreams(); };
            _loggerWindow.IsVisibleChanged += (sender, args) => _loggerConfig.Visible = _loggerWindow.IsVisible;
            _loggerWindow.LocationChanged += (sender, args) => _loggerLocationJustChanged = true;
            _loggerWindow.SizeChanged += (sender, args) => _loggerLocationJustChanged = true;
            _loggerWindow.SoloRequested += (sender, e) =>
            {
                foreach (var stream in _loggerViewModel.Streams)
                {
                    if (stream != e.StreamItem)
                        stream.Enabled = false;
                    else
                        stream.Enabled = true;
                }
            };
                
            if (_loggerConfig.Visible)
                _loggerWindow.Show();

            _windows.Add(_loggerWindow);

            _scrollViewer = GetDescendantByType(_loggerWindow.LogListView, typeof(ScrollViewer)) as ScrollViewer;

            WindowHelper.EnsureOnScreen(_loggerWindow);
        }

        public override void CreateProfiler(ProfilerConfig config, int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels)
        {
            _profilerConfig = config;

            _profilerViewModel = new ProfilerViewModel(config.TimeRulerConfig.VisibleLevelsFlags, config.PerformanceGraphConfig.VisibleLevelsFlags, config.ProfilerSummaryConfig.VisibleLevelsFlags);
            _profilerViewModel.LevelItemChanged += (sender, args) =>
            {
                OnLevelItemChanged(new LevelItemChangedEventArgs(args.Key, args.LevelId, args.Enabled));
            };

            _profilerWindow = new ProfilerWindow
            {
                Top = config.Top,
                Left = config.Left,
                Width = config.Width,
                Height = config.Height
            };

            _profilerWindow.trLevelsListBox.DataContext = _profilerViewModel.TimeRulerLevels;
            _profilerWindow.PgLevelsListBox.DataContext = _profilerViewModel.PerformanceGraphLevels;
            _profilerWindow.PsLevelsListBox.DataContext = _profilerViewModel.ProfilerSummaryLevels;

            _profilerWindow.EnableAllTimeRulerLevels += (sender, args) => { _profilerViewModel.EnableAllTimeRulerLevels(); };
            _profilerWindow.DisableAllTimeRulerLevels += (sender, args) => { _profilerViewModel.DisableAllTimeRulerLevels(); };
            _profilerWindow.EnableAllPerformanceGraphLevels += (sender, args) => { _profilerViewModel.EnableAllPerformanceGraphLevels(); };
            _profilerWindow.DisableAllPerformanceGraphLevels += (sender, args) => { _profilerViewModel.DisableAllPerformanceGraphLevels(); };
            _profilerWindow.EnableAllProfilerSummaryLevels += (sender, args) => { _profilerViewModel.EnableAllProfilerSummaryLevels(); };
            _profilerWindow.DisableAllProfilerSummaryLevels += (sender, args) => { _profilerViewModel.DisableAllProfilerSummaryLevels(); };
  
            _profilerWindow.IsVisibleChanged += (sender, args) => _profilerConfig.Visible = _profilerWindow.IsVisible;
            _profilerWindow.LocationChanged += (sender, args) => _profilerLocationJustChanged = true;
            _profilerWindow.SizeChanged += (sender, args) => _profilerLocationJustChanged = true;

            if (_profilerConfig.Visible)
                _profilerWindow.Show();

            _windows.Add(_profilerWindow);

            WindowHelper.EnsureOnScreen(_profilerWindow);
        }

        public override void CreateFinder(Components.Finder.Finder finder)
        {
            _finderConfig = finder.Config;

            _finderWindow = new FinderWindow
            {
                Top = finder.Config.Top,
                Left = finder.Config.Left,
                Width = finder.Config.Width,
                Height = finder.Config.Height,
                DataContext = finder
            };
                
            _finderWindow.IsVisibleChanged += (sender, args) => _finderConfig.Visible = _finderWindow.IsVisible;
            _finderWindow.LocationChanged += (sender, args) => _finderLocationJustChanged = true;
            _finderWindow.SizeChanged += (sender, args) => _finderLocationJustChanged = true;
            _finderWindow.ObjectSelected += (sender, args) => { GearsetResources.Console.Inspect(args.Name, args.Object); };

            if (_finderConfig.Visible)
                _finderWindow.Show();

            _windows.Add(_finderWindow);

            WindowHelper.EnsureOnScreen(_finderWindow);
            ElementHost.EnableModelessKeyboardInterop(_finderWindow);
        }

        public override void CreateBender(BenderConfig config)
        {
            _benderConfig = config;

            _curveEditorWindow = new CurveEditorWindow
            {
                Top = config.Top,
                Left = config.Left,
                Width = config.Width,
                Height = config.Height
            };

            _curveTreeViewModel = new CurveTreeViewModel(_curveEditorWindow.curveEditorControl);
            _curveEditorWindow.DataContext = _curveEditorWindow.curveEditorControl.ControlsViewModel;
            _curveEditorWindow.curveTree.DataContext = _curveTreeViewModel;

            _curveEditorWindow.IsVisibleChanged += (sender, args) => _benderConfig.Visible = _curveEditorWindow.IsVisible;
            _curveEditorWindow.LocationChanged += (sender, args) => _benderLocationJustChanged = true;
            _curveEditorWindow.SizeChanged += (sender, args) => _benderLocationJustChanged = true;

            if (_benderConfig.Visible)
                _curveEditorWindow.Show();

            _windows.Add(_curveEditorWindow);

            WindowHelper.EnsureOnScreen(_curveEditorWindow);
        }

        public override void Update(double deltaTime)
        {
            if (_loggerLocationJustChanged)
            {
                _loggerLocationJustChanged = false;
                _loggerConfig.Top = _loggerWindow.Top;
                _loggerConfig.Left = _loggerWindow.Left;
                _loggerConfig.Width = _loggerWindow.Width;
                _loggerConfig.Height = _loggerWindow.Height;
            }

            if (_profilerLocationJustChanged)
            {
                _profilerLocationJustChanged = false;
                _profilerConfig.Top = _profilerWindow.Top;
                _profilerConfig.Left = _profilerWindow.Left;
                _profilerConfig.Width = _profilerWindow.Width;
                _profilerConfig.Height = _profilerWindow.Height;
            }

            if (_finderLocationJustChanged)
            {
                _finderLocationJustChanged = false;
                _finderConfig.Top = _finderWindow.Top;
                _finderConfig.Left = _finderWindow.Left;
                _finderConfig.Width = _finderWindow.Width;
                _finderConfig.Height = _finderWindow.Height;
            }

            if (_benderLocationJustChanged)
            {
                _benderLocationJustChanged = false;
                _benderConfig.Top = _curveEditorWindow.Top;
                _benderConfig.Left = _curveEditorWindow.Left;
                _benderConfig.Width = _curveEditorWindow.Width;
                _benderConfig.Height = _curveEditorWindow.Height;
            }

            _curveEditorWindow.curveEditorControl.UpdateRender();
            _curveEditorWindow.horizontalRuler.UpdateRender();
            _curveEditorWindow.verticalRuler.UpdateRender();

            _inspectorUI.Update(deltaTime);
        }

        public override void GotFocus()
        {
            foreach (var window in _windows)
            {
                if (window.IsVisible)
                {
                    window.Topmost = true;
                    window.Topmost = false;
                }
                    
                if (window.WasHiddenByGameMinimize && !window.IsVisible)
                    window.Show();

                window.WasHiddenByGameMinimize = false;
            }
        }

        public override void Resize()
        {
            foreach (var window in _windows)
            {
                if (window.IsVisible == false) 
                    continue;

                window.Hide();
                window.WasHiddenByGameMinimize = true;
            }
        }

        public override void MoveTo(int left, int top)
        {
            _widgetWindow.Top = top - _widgetWindow.Height;
            _widgetWindow.Left = left + 20;
        }

        public override bool WidgetVisible
        {
            set { SetWindowVisibility(_widgetWindow, value); }
        }

        public override bool InspectorVisible
        {
            set 
            {
                if (_inspectorUI != null)
                    _inspectorUI.Visible = value;  
            }
        }

        public override bool LoggerVisible
        {
            set { SetWindowVisibility(_loggerWindow, value); }
        }

        public override bool ProfilerVisible
        {
            set { SetWindowVisibility(_profilerWindow, value); }
        }

        public override bool FinderVisible
        {
            set { SetWindowVisibility(_finderWindow, value); }
        }

        public override bool BenderVisible
        {
            set { SetWindowVisibility(_curveEditorWindow, value); }
        }

        static void SetWindowVisibility(UIElement window, bool isVisible)
        {
            if (window != null)
                window.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        public override void AddAction(string name, Action action)
        {
            _widgetViewModel.AddAction(name, action);
        }

        //Logger

        public override void Log(string message, int updateNumber)
        {
            var stream = _loggerViewModel.Log(message, updateNumber);

            if (stream.Enabled)
                _scrollViewer.ScrollToEnd();
        }

        public override void Log(string streamName, string message, int updateNumber)
        {
            var stream = _loggerViewModel.Log(streamName, message, updateNumber);

            if (stream.Enabled)
                _scrollViewer.ScrollToEnd();
        }

        protected override void OnStreamChanged(StreamChangedEventArgs e)
        {
            _filteredView.Refresh();
            base.OnStreamChanged(e);
        }

        /// <summary>
        /// Shows a dialog asking for a filename and saves the log file.
        /// </summary>
        public override void SaveLogToFile()
        {
            // Configure save file dialog box
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "",
                DefaultExt = ".log",
                Filter = "Log files (.log)|*.log"
            };

            // Show save file dialog box
            var result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result != true)
            {
                return;
            }

            // Generate the log file.
            SaveLogToFile(dlg.FileName);
        }

        /// <summary>
        /// Saves the log to the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to save the log (usually ending in .log)</param>
        public override void SaveLogToFile(string filename)
        {
            // Generate the log file.
            using (System.IO.TextWriter t = new System.IO.StreamWriter(filename))
            {
                var updateNumber = -1;
                var maxStreamNameSize = 0;
                foreach (var item in _loggerViewModel.Streams)
                    if (item.Name.Length > maxStreamNameSize)
                        maxStreamNameSize = item.Name.Length;

                // Store old items (not shown in the Logger window).
                foreach (var item in _loggerViewModel.LogItemsOld)
                {
                    if (item.UpdateNumber > updateNumber)
                    {
                        t.WriteLine("Update " + item.UpdateNumber);
                        updateNumber = item.UpdateNumber;
                    }
                    t.WriteLine(item.Stream.Name.PadLeft(maxStreamNameSize) + " | " + item.Content);
                }
                // Store last n items (shown in the logger window).
                foreach (var item in _loggerViewModel.LogItems)
                {
                    if (item.UpdateNumber > updateNumber)
                    {
                        t.WriteLine("Update " + item.UpdateNumber);
                        updateNumber = item.UpdateNumber;
                    }
                    t.WriteLine(item.Stream.Name.PadLeft(maxStreamNameSize) + " | " + item.Content);
                }
                t.Close();
            }
        }

        private static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
                return null;

            if (element.GetType() == type)
                return element;

            Visual foundElement = null;
            var frameworkElement = element as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        } 

        //Finder

        public override void FinderSearch(FinderResult results)
        {
            _finderWindow.ResultsListBox.ItemsSource = results;
                
            if (_finderWindow.ResultsListBox.Items.Count > 0)
                _finderWindow.ResultsListBox.SelectedIndex = 0;
        }

        //Bender
        public override void BenderShow() 
        {
            _curveEditorWindow.Show();
        }

        public override void BenderFocus() 
        {
            _curveEditorWindow.Focus();
        }

        public override void AddCurve(string name, Curve curve)
        {
            _curveTreeViewModel.AddCurve(name, curve);
        }

        public override void RemoveCurve(Curve curve)
        {
            _curveTreeViewModel.RemoveCurve(curve);
        }

        public override void RemoveCurveOrGroup(string name)
        {
            _curveTreeViewModel.RemoveCurveOrGroup(name);
        }

        public override float BenderHorizontalRulerNeedlePosition 
        {
            get { return (float)_curveEditorWindow.horizontalRuler.NeedlePosition;}
        }

        //Inspector

        public override object InspectorSelectedItem
        {
            get { return _inspectorUI.InspectorSelectedItem; }
        }

        public override void Watch(InspectorNode node) 
        {
            _inspectorUI.Watch(node);
        }

        public override void Inspect(string name, Object o, bool autoExpand, Type t) 
        {
            _inspectorUI.Inspect(name, o, autoExpand, t);
        }

        public override void RemoveInspect(Object o) 
        {
            _inspectorUI.RemoveInspect(o);
        }

        public override void CraftMethodCall(MethodInfo info)
        {
            _inspectorUI.CraftMethodCall(info);
        }

        public override void ClearInspectedObjects()
        {
            _inspectorUI.ClearInspectedObjects();
        }

        public override void ClearMethods()
        {
            _inspectorUI.ClearMethods();
        }

        public override void AddNotice(string message, string url, string linkText)
        {
            _inspectorUI.AddNotice(message, url, linkText);
        }

        public override bool FilterPredicate(object item) 
        {
            return _inspectorUI.FilterPredicate(item);
        }
            
    }
}
#if EMPTYKEYS
using System;
using System.Collections.ObjectModel;
using EmptyKeys.GearsetModel;
using EmptyKeys.UserInterface;
using EmptyKeys.UserInterface.Controls;
using EmptyKeys.UserInterface.Generated;
using EmptyKeys.UserInterface.Media;
using EmptyKeys.UserInterface.Mvvm;
using EmptyKeys.UserInterface.Themes;
using Gearset.Components;
using Gearset.Components.CommandConsole;
using Gearset.Components.InspectorWPF;
using Gearset.Components.Profiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WindowStyle = Gearset.UserInterface.EmptyKeys.Styles.WindowStyle;
using GearsetModelObjectDescription = EmptyKeys.GearsetModel.ObjectDescription;

namespace Gearset.UserInterface.EmptyKeys
{ 
    public class EmptyKeysUserInterface : UserInterface
    {
        readonly int _width;
        readonly int _height;
   
        readonly Thickness _windowPadding = new Thickness(-4);

        GearsetUIViewModel _viewModel;
        GearsetUI _gearsetUI;

        readonly ObservableCollection<WindowViewModel> _ownedWindowContent = new ObservableCollection<WindowViewModel>();

        WidgetWindow _widgetWindow;
        //InspectorWindow _inspectorWindow;
        LoggerWindow _loggerWindow;
        FinderWindow _finderWindow;
        ProfilerWindow _profilerWindow;
        CommandConsoleWindow _commandConsoleWindow;

        WidgetWindowViewModel _widgetWindowViewModel;
        //InspectorViewModel _inspectorViewModel;
        LoggerWindowViewModel _loggerWindowViewModel;
        FinderWindowViewModel _finderWindowViewModel;
        ProfilerWindowViewModel _profilerWindowViewModel;
        CommandConsoleWindowViewModel _commandConsoleWindowViewModel;

        ScrollViewer _loggerScrollViewer;
        ScrollViewer _commandConsoleScrollViewer;

        LoggerConfig _loggerConfig;
        ProfilerConfig _profilerConfig;
        FinderConfig _finderConfig;
        CommandConsoleConfig _commandConsoleConfig;

        //InspectorUI _inspectorUI;

        bool _sizedUI;

        public EmptyKeysUserInterface(bool createUI, GraphicsDevice graphicsDevice, int width, int height)
            : base(createUI, graphicsDevice, width, height)
        {
            _width = width;
            _height = height;

            var engine = new MonoGameEngine(GraphicsDevice, width, height);
        }

        public override void Initialise(ContentManager content, int width, int height)
        {
            var font = content.Load<SpriteFont>("Segoe_UI_9_Regular");
            FontManager.DefaultFont = Engine.Instance.Renderer.CreateFont(font);

            _viewModel = new GearsetUIViewModel();
            _gearsetUI = new GearsetUI(width, height)
            {
                Background = new SolidColorBrush(new ColorW(0, 0, 0, 128)),
                DataContext = _viewModel,
            };

            ResourceDictionary.DefaultDictionary = Dark.GetThemeDictionary();

            ResourceDictionary.DefaultDictionary[typeof(Window)] = WindowStyle.CreateWindowStyle();
            ResourceDictionary.DefaultDictionary[CommonResourceKeys.WindowActiveBrushKey] = new SolidColorBrush(new ColorW(0, 102, 204));

            var activeWindowBrush = (SolidColorBrush)ResourceDictionary.DefaultDictionary[CommonResourceKeys.WindowActiveBrushKey];

            var lgb = new LinearGradientBrush(ColorW.TransparentBlack, activeWindowBrush.Color, new PointF(0.5f, 0.5f), new PointF(1.0f, 1.0f));
            lgb.GradientStops[0].Offset = 0.0f;
            lgb.GradientStops[1].Offset = 0.1f;
            ResourceDictionary.DefaultDictionary[CommonResourceKeys.WindowResizeThumbKey] = lgb;
                
            _widgetWindowViewModel = new WidgetWindowViewModel();
            _widgetWindowViewModel.Enabled = GearsetSettings.Instance.Enabled;

            _widgetWindowViewModel.MasterSwitchButtonClicked += (sender, args) => { GearsetSettings.Instance.Enabled = _widgetWindowViewModel.Enabled; };
            //_widgetWindowViewModel.InspectorButtonClicked += (sender, args) => { SetWindowVisibility(_inspectorViewModel, _widgetWindowViewModel.InspectorWindowVisible); };
            _widgetWindowViewModel.LoggerButtonClicked += (sender, args) => { SetWindowVisibility(_loggerWindowViewModel, _widgetWindowViewModel.LoggerWindowVisible); };
            _widgetWindowViewModel.FinderButtonClicked += (sender, args) => { SetWindowVisibility(_finderWindowViewModel, _widgetWindowViewModel.FinderWindowVisible); };
            _widgetWindowViewModel.ProfilerButtonClicked += (sender, args) => { SetWindowVisibility(_profilerWindowViewModel, _widgetWindowViewModel.ProfilerWindowVisible); };
            _widgetWindowViewModel.CommandConsoleButtonClicked += (sender, args) => { SetWindowVisibility(_commandConsoleWindowViewModel, _widgetWindowViewModel.CommandConsoleWindowVisible); };

            _gearsetUI.OwnedWindowsContent = _ownedWindowContent;

            FontManager.Instance.LoadFonts(content);
            ImageManager.Instance.LoadImages(content);
            SoundManager.Instance.LoadSounds(content);
        }

        public override void Destory()
        {

        }

        public override void CreateWidget()
        {
            if (CreateUI == false)
                return;

            _widgetWindow = new WidgetWindow
            {
                DataContext = _widgetWindowViewModel
            };

            _gearsetUI.StackPanel.Children.Add(_widgetWindow);
        }

        public override void CreateInspector(InspectorManager inspector)
        {
                
            //_inspectorViewModel = new InspectorViewModel();

            //_inspectorWindow = new InspectorWindow { DataContext = _inspectorViewModel, Padding = _windowPadding };

            //_inspectorUI = new InspectorUI(_inspectorWindow, _inspectorViewModel);

            //var inspectorTemplate = new DataTemplate(typeof(InspectorWindowViewModel), parent =>
            //{
            //    _inspectorWindow.Parent = parent;
            //    return _inspectorWindow;
            //});
            //_gearsetUI.Resources.Add(inspectorTemplate.DataTemplateKey, inspectorTemplate);

            //_inspectorViewModel.Top = 25;
            //_inspectorViewModel.Left = 0;
            //_inspectorViewModel.Opacity = 1f;
            //_inspectorViewModel.Width = 400;
            //_inspectorViewModel.Height = 200;

            //_ownedWindowContent.Add(_inspectorViewModel);

            //_widgetWindowViewModel.InspectorWindowVisible = inspector.Config.Visible;
            //SetWindowVisibility(_inspectorViewModel, inspector.Config.Visible);
        }

        public override void CreateLogger(LoggerConfig config)
        {
            if (CreateUI == false)
                return;

            //Conversion info
            //Logger uses an ItemsControl which does't support grouping
            //I was unable to get ItemsControl to utilise CollectionViewSource correctly so have hacked the stream filtering
            //this behaviour is different to the WPF logger.

            _loggerConfig = config;

            _loggerWindowViewModel = new LoggerWindowViewModel();
            _loggerWindowViewModel.StreamChanged += (sender, args) =>
            {
                _loggerWindow.LogListBox.ItemsSource = _loggerWindowViewModel.VisibleLogItems;
                OnStreamChanged(new StreamChangedEventArgs(args.Name, args.Enabled));
            };

            //_filteredView = CollectionViewSource.GetDefaultView(_loggerWindowViewModel.LogItems);
            //_filteredView.Filter = a => ((LoggerWindowViewModel.LogItem)a).Stream.Enabled;

            _loggerWindow = new LoggerWindow { DataContext = _loggerWindowViewModel, Padding = _windowPadding };
            _loggerWindow.SizeChanged += (sender, args) =>
            {
                _loggerConfig.Width = _loggerWindow.ActualWidth;
                _loggerConfig.Height = _loggerWindow.ActualHeight;
            };

            //_loggerWindow.LogListBox.ItemsSource = _loggerWindowViewModel.VisibleLogItems;

            var loggerTemplate = new DataTemplate(typeof(LoggerWindowViewModel), parent =>
            {
                _loggerWindow.Parent = parent;
                return _loggerWindow;
            });
            _gearsetUI.Resources.Add(loggerTemplate.DataTemplateKey, loggerTemplate);

            _loggerScrollViewer = _loggerWindow.ScrollViewer;

            InitialiseWindow(_loggerWindowViewModel, config.Top, config.Left, config.Width, config.Height, config.Visible);

            _widgetWindowViewModel.LoggerWindowVisible = config.Visible;
        }

        public override void CreateCommandConsole(CommandConsoleConfig config)
        {
            if (CreateUI == false)
                return;

            _commandConsoleConfig = config;

            _commandConsoleWindowViewModel = new CommandConsoleWindowViewModel();

            _commandConsoleWindowViewModel.Execute += (sender, args) =>
            {
                OnExecuteCommand(new ExecuteCommandEventArgs(_commandConsoleWindowViewModel.CommandText));
            };

            _commandConsoleWindow = new CommandConsoleWindow { DataContext = _commandConsoleWindowViewModel, Padding = _windowPadding };
            _commandConsoleWindow.SizeChanged += (sender, args) =>
            {
                _commandConsoleConfig.Width = _commandConsoleWindow.ActualWidth;
                _commandConsoleConfig.Height = _commandConsoleWindow.ActualHeight;
            };

            _commandConsoleWindow.LogListBox.ItemsSource = _commandConsoleWindowViewModel.Output;

            var commandConsoleTemplate = new DataTemplate(typeof(CommandConsoleWindowViewModel), parent =>
            {
                _commandConsoleWindow.Parent = parent;
                return _commandConsoleWindow;
            });
            _gearsetUI.Resources.Add(commandConsoleTemplate.DataTemplateKey, commandConsoleTemplate);

            _commandConsoleScrollViewer = _commandConsoleWindow.ScrollViewer;

            InitialiseWindow(_commandConsoleWindowViewModel, config.Top, config.Left, config.Width, config.Height, config.Visible);

            _widgetWindowViewModel.CommandConsoleWindowVisible = config.Visible;
        }

        public override void CreateProfiler(ProfilerConfig config, int enabledTimeRulerLevels, int enabledPerformanceGraphLevels, int enabledProfilerSummaryLevels)
        {
            if (CreateUI == false)
                return;

            _profilerConfig = config;

            _profilerWindowViewModel = new ProfilerWindowViewModel(config.TimeRulerConfig.VisibleLevelsFlags, config.PerformanceGraphConfig.VisibleLevelsFlags, config.ProfilerSummaryConfig.VisibleLevelsFlags);
            _profilerWindowViewModel.ProfilerLevelChanged += (sender, args) =>
            {
                OnLevelItemChanged(new LevelItemChangedEventArgs(args.Key, args.LevelId, args.Enabled));
            };

            _profilerWindow = new ProfilerWindow { DataContext = _profilerWindowViewModel, Padding = _windowPadding };
            _profilerWindow.SizeChanged += (sender, args) =>
            {
                _profilerConfig.Width = _profilerWindow.ActualWidth;
                _profilerConfig.Height = _profilerWindow.ActualHeight;
            };
                
            var profilerTemplate = new DataTemplate(typeof(ProfilerWindowViewModel), parent =>
            {
                _profilerWindow.Parent = parent;
                return _profilerWindow;
            });
            _gearsetUI.Resources.Add(profilerTemplate.DataTemplateKey, profilerTemplate);

            InitialiseWindow(_profilerWindowViewModel, config.Top, config.Left, config.Width, config.Height, config.Visible);

            _widgetWindowViewModel.ProfilerWindowVisible = config.Visible;
        }

        public override void CreateFinder(Components.Finder.Finder finder)
        {
            if (CreateUI == false)
                return;

            _finderConfig = finder.Config;

            _finderWindowViewModel = new FinderWindowViewModel();
            _finderWindowViewModel.SearchText = finder.Config.SearchText;
            _finderWindow = new FinderWindow { DataContext = _finderWindowViewModel, Padding = _windowPadding };
            _finderWindow.SizeChanged += (sender, args) =>
            {
                _finderConfig.Width = _finderWindow.ActualWidth;
                _finderConfig.Height = _finderWindow.ActualHeight;
            };
            //TODO
            //_finderWindow.ObjectSelected += (sender, args) => { GearsetResources.Console.Inspect(args.Name, args.Object); };

            var finderTemplate = new DataTemplate(typeof(FinderWindowViewModel), parent =>
            {
                _finderWindow.Parent = parent;
                return _finderWindow;
            });
            _gearsetUI.Resources.Add(finderTemplate.DataTemplateKey, finderTemplate);

            InitialiseWindow(_finderWindowViewModel, _finderConfig.Top, _finderConfig.Left, _finderConfig.Width, _finderConfig.Height, _finderConfig.Visible);

            _widgetWindowViewModel.FinderWindowVisible = finder.Config.Visible;   
        }

        public override void CreateBender(BenderConfig config)
        {

        }

        void InitialiseWindow(WindowViewModel windowViewModel, double top, double left, double width, double height, bool visible)
        {
            if (CreateUI == false)
                return;

            windowViewModel.Top = (float)top;
            windowViewModel.Left = MathHelper.Clamp((float)left - 5.0f, 0.0f, _width);
            windowViewModel.Width = MathHelper.Clamp((float)width + 10.0f, 50.0f, _width);
            windowViewModel.Height = MathHelper.Clamp((float)height + 32, 50.0f, _height);

            windowViewModel.Opacity = 1f;

            _ownedWindowContent.Add(windowViewModel);

            SetWindowVisibility(windowViewModel, visible);
        }

        static void SetWindowVisibility(WindowViewModel window, bool isVsible)
        {
            if (window == null)
                return;

            window.IsVisible = isVsible;
        }

        public override void Update(double deltaTime)
        {
            if (CreateUI == false)
                return;

            if (_sizedUI == false) 
			{	
				_gearsetUI.Resize (_width, _height);
				_sizedUI = true;
			}

            if (_finderConfig.SearchText != _finderWindowViewModel.SearchText)
                _finderConfig.SearchText = _finderWindowViewModel.SearchText;

            _loggerWindowViewModel.Opacity = _widgetWindowViewModel.SliderValue;
            _finderWindowViewModel.Opacity = _widgetWindowViewModel.SliderValue;
            _profilerWindowViewModel.Opacity = _widgetWindowViewModel.SliderValue;
            _commandConsoleWindowViewModel.Opacity = _widgetWindowViewModel.SliderValue;

            _gearsetUI.UpdateInput(deltaTime);
            _gearsetUI.UpdateLayout(deltaTime);

            _loggerConfig.Top = _loggerWindow.RenderPosition.Y;
            _loggerConfig.Left = _loggerWindow.RenderPosition.X;
  
            //System.Diagnostics.Debug.WriteLine(_profilerWindow.);

            _profilerConfig.Top = _profilerWindow.RenderPosition.Y;
            _profilerConfig.Left = _profilerWindow.RenderPosition.X;

            _finderConfig.Top = _finderWindow.RenderPosition.Y;
            _finderConfig.Left = _finderWindow.RenderPosition.X;
        }

        public override void Draw(double deltaTime)
        {
            _gearsetUI.Draw(deltaTime);
        }

        public override void GotFocus()
        {
				
        }

        public override void Resize()
        {
				
        }

        public override void MoveTo(int left, int top)
        {

        }

        public override bool WidgetVisible
        {
            set { }
        }

        public override bool InspectorVisible
        {
            set { }
        }

        public override bool LoggerVisible
        {
            set {  }
        }

        public override bool ProfilerVisible
        {
            set { }
        }

        public override bool FinderVisible
        {
            set { }
        }

        public override bool BenderVisible
        {
            set { }
        }

        public override bool CommandConsoleVisible
        {
            set { }
        }

        //Widget
        public override void AddAction(string name, Action action)
        {

        }
            
        //Logger
        public override void Log(string message, int updateNumber)
        {
            if (CreateUI == false)
                return;

            var stream = _loggerWindowViewModel.Log(message, updateNumber);
            _loggerWindow.LogListBox.ItemsSource = _loggerWindowViewModel.VisibleLogItems;

            if (stream.Enabled)
                _loggerScrollViewer.ScrollToBottom();
        }

        public override void Log(string streamName, string message, int updateNumber)
        {
            if (CreateUI == false)
                return;

            var stream = _loggerWindowViewModel.Log(streamName, message, updateNumber);
            _loggerWindow.LogListBox.ItemsSource = _loggerWindowViewModel.VisibleLogItems;

            if (stream.Enabled)
                _loggerScrollViewer.ScrollToBottom();
        }

        public override void SaveLogToFile()
        {

        }

        public override void SaveLogToFile(string filename)
        {

        }

        //Finder
        public override void FinderSearch(FinderResult results)
        {
            if (CreateUI == false)
                return;

            var items = new FinderWindowViewModel.FinderResult();
            foreach (var result in results)
            {
                items.Add(new GearsetModelObjectDescription(result.Object, result.Description, result.Name));
            }

            _finderWindowViewModel.Items = items;

            if (_finderWindow.ResultsDataGrid.Items.Count > 0)
                _finderWindow.ResultsDataGrid.SelectedIndex = 0;
        }

        //Bender
        public override void AddCurve(string name, Curve curve)
        {

        }

        public override void RemoveCurve(Curve curve)
        {

        }

        public override void RemoveCurveOrGroup(string name)
        {

        }

        public override float BenderHorizontalRulerNeedlePosition 
        {
            get { return 0.0f; }
        }

        public static DependencyObject GetDescendantByType(DependencyObject element, Type type)
        {
            if (element == null)
                return null;

            if (element.GetType() == type)
                return element;

            DependencyObject foundElement = null;
            var frameworkElement = element as UIElement;
            if (frameworkElement != null)
            {
                frameworkElement.ApplyTemplate();
            }

            foreach(var child in VisualTreeHelper.Instance.GetChildren(element))
            {
                foundElement = GetDescendantByType(child, type);
                if (foundElement != null)
                    break;
            }
            return foundElement;
        }

        public override void Inspect(string name, Object o, bool autoExpand, Type t)
        {
            //_inspectorUI.Inspect(name, o, autoExpand, t);
        }

        public override void EchoCommand(CommandConsoleManager.DebugCommandMessage messageType, string text)
        {
            if (CreateUI == false)
                return;

            _commandConsoleWindowViewModel.EchoCommand((int)messageType, text);
            _commandConsoleScrollViewer.ScrollToBottom();
        }

        public override void ClearCommandOutput()
        {
            if (CreateUI == false)
                return;

            _commandConsoleWindowViewModel.ClearOutput();
        }
    }
}
#endif

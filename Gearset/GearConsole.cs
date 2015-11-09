#region Using Statements

using Gearset.Components.Finder;
using Gearset.Components.Bender;
using Gearset.UserInterface;
#if WPF
    using Gearset.UserInterface.Wpf;
#elif EMPTYKEYS
    using Gearset.UserInterface.EmptyKeys;
#endif
using Gearset.Components.Widget;
using Gearset.Components.Profiler;
using System;
using System.Collections.Generic;
using Gearset.Components;
using Gearset.Components.Logger;
using Gearset.Components.InspectorWPF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.IO;
using Gearset.Components.Memory;
using Gearset.Components.Plotter;
#if WINDOWS || LINUX || MONOMAC
using System.Net;
#endif

#endregion

namespace Gearset
{
    /// <summary>
    /// This is the main Gearset class. Provides wrapper methods for
    /// easily accessing the its components functionality.
    /// </summary>
    /// <remarks>
    /// Create a single instance of this class in your game
    /// and keep it in a static variable you can reach with ease.
    /// (i.e Util.Console).
    /// 
    /// For 3D games, the Gearset camera matrices must be updated using
    /// the SetMatrices method.
    /// </remarks>
    public class GearConsole
    {
        internal bool Enabled { get { return Settings != null && Settings.Enabled; } }

        private Game game;

        /// <summary>
        /// Collection of DebugComponents
        /// </summary>
        internal List<Gear> Components;

        /// <summary>
        /// Test variable, mainly to know if Xdtk is running or not.
        /// </summary>
        internal int DrawCount;

        /// <summary>
        /// Counts the number of updates that has occured, it is used
        /// by the Logger.
        /// </summary>
        internal int UpdateCount;

        internal Object manager;

        /// <summary>
        /// Time in ticks when the last update was made.
        /// </summary>
        private int lastSaveTime;

        /// <summary>
        /// The World Matrix of the game. The prefered method to 
        /// update all matrices is by using <c>SetMatrices</c>.
        /// </summary>
        public Matrix WorldMatrix { get { return GearsetResources.World; } set { GearsetResources.World = value; } }

        /// <summary>
        /// The View Matrix of the game camera. The prefered method to 
        /// update all matrices is by using <c>SetMatrices</c>.
        /// </summary>
        public Matrix ViewMatrix { get { return GearsetResources.View; } set { GearsetResources.View = value; } }

        /// <summary>
        /// The Projection Matrix of game camera. The prefered method to 
        /// update all matrices is by using <c>SetMatrices</c>.
        /// </summary>
        public Matrix ProjectionMatrix { get { return GearsetResources.Projection; } set { GearsetResources.Projection = value; } }

        /// <summary>
        /// The Transform Matrix to apply to all 2D overlaids of the client game. 
        /// Use this if you're creating a 2D game where the camera moves (e.g. a scroller).
        /// The prefered method to update all matrices is by using <c>SetMatrices</c>.
        /// </summary>
        public Matrix Transform2D { get { return GearsetResources.Transform2D; } set { GearsetResources.Transform2D = value; } }

        /// <summary>
        /// Returns the needle position of the curves in Bender. The game can use this
        /// value to let designers preview curve animations.
        /// </summary>
        public float BenderNeedlePosition { get { return (Bender != null) ? Bender.SeekNeedlePosition : 0; } }

        // Components
        /// <summary>
        /// Displays a Hierarchy of values on the screen for easy
        /// tracing, the values must be manually updating using the 
        /// <c>Show</c> method
        /// </summary>
        internal TreeView TreeView { get; private set; }

        /// <summary>
        /// Displays marks on 3D space.
        /// </summary>
        internal Marker Marker { get; private set; }

        /// <summary>
        /// Used to display huge text on the middle of the screen,
        /// useful when the developer want to be alerted of an important
        /// event.
        /// </summary>
        internal Alerter Alerter { get; private set; }

        /// <summary>
        /// Used to draw lines on 3D or 2D space. Lines can be persistant
        /// (with a name) or be drawn for a single frame.
        /// </summary>
        internal LineDrawer LineDrawer { get; private set; }

        /// <summary>
        /// Data Sampler used to sample data which can be plotted by a plotter.
        /// </summary>
        internal DataSamplerManager DataSamplerManager { get; private set; }

        /// <summary>
        /// Plots graph of Data.
        /// </summary>
        internal Plotter Plotter { get; private set; }

        /// <summary>
        /// Shows labels (text) on the screen.
        /// </summary>
        internal Labeler Labeler { get; private set; }

        /// <summary>
        /// Draw Vectors in 3D space as arrows
        /// </summary>
        internal Vector3Drawer Vector3Drawer { get; private set; }

        /// <summary>
        /// Draw Vectors in 2D space as arrows
        /// </summary>
        internal Vector2Drawer Vector2Drawer { get; private set; }

        /// <summary>
        /// Draw Trasforms (Matrices) in 3D space as 3 orthogonal vectors.
        /// </summary>
        internal Transform3Drawer Transform3Drawer { get; private set; }

        /// <summary>
        /// Draw 3 axis aligned, orthogonally oriented circles that represent spheres.
        /// </summary>
        internal SphereDrawer SphereDrawer { get; private set; }

        /// <summary>
        /// Draw axis aligned bounding boxes.
        /// </summary>
        internal BoxDrawer BoxDrawer { get; private set; }

        /// <summary>
        /// Draw axis aligned bounding boxes.
        /// </summary>
        internal SolidBoxDrawer SolidBoxDrawer { get; private set; }

#if WINDOWS || LINUX || MONOMAC
        /// <summary>
        /// Components that keeps a list of pickable game elements and
        /// interacts with the mouse to get notified when they are being
        /// picked. Useful to visually place objects in the Inspector.
        /// </summary>
        internal Picker Picker { get; private set; }

        ///// <summary>
        ///// This component is for internal use. Allows XDTK to save data
        ///// that can be later retreived.
        ///// </summary>
        //internal Persistor Persistor { get; private set; }
#endif

        /// <summary>
        /// Widget that places itself on top of the windows titlebar.
        /// </summary>
        internal Widget Widget { get; private set; }

        /// <summary>
        /// Component used to edit XNA curves.
        /// </summary>
        internal Bender Bender { get; private set; }

        /// <summary>
        /// Provides UI to find a Game Object.
        /// </summary>
        internal Finder Finder { get; private set; }

        /// <summary>
        /// Component that let the game developer inspect game objects
        /// in real time, showing the value of public fields. The inspector
        /// is also capable of calling methods on objects.
        /// </summary>
        internal InspectorManager Inspector { get; private set; }

        /// <summary>
        /// Component that logs events.
        /// </summary>
        internal LoggerManager Logger { get; private set; }

        /// <summary>
        /// Code profiler.
        /// </summary>
        public ProfilerManager Profiler { get; private set; }

        /// <summary>
        /// Memory monitor.
        /// </summary>
        public MemoryMonitor MemoryMonitor { get; private set; }

        /// <summary>
        /// Gearset settings
        /// </summary>
        internal GearsetSettings Settings { get { return GearsetSettings.Instance; } }

        /// <summary>
        /// Gets or sets a value indicating whether the Gearset's overlays will be visible.
        /// A shortcut for Settings.Visible;
        /// </summary>
        public bool VisibleOverlays { get { return Settings.ShowOverlays; } set { Settings.ShowOverlays = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the Gearset's Initialize method has been called.
        /// </summary>
        public bool Initialized
        {
            get { return initialized; }
            private set
            {
                Settings.Enabled = true;
                initialized = true;
            }
        }
        private bool initialized;

        public static float LiteVersionNoticeAlpha;

        readonly IUserInterface _userInterface;

        #region Constructor

        /// <summary>
        /// Creates the GearConsole. if you want the console
        /// to draw 3D debug stuff you need to update the World/View/Projection
        /// matrices using the <c>SetMatrices</c> or setting them manually.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="createUI"></param>
        public GearConsole(Game game, bool createUI)
        {
            this.game = game;
            game.Exiting += OnExit;
            Components = new List<Gear>();

            GearsetResources.Game = game;
            GearsetResources.World = Matrix.Identity;
            GearsetResources.View = Matrix.Identity;
            GearsetResources.Projection = Matrix.Identity;
            GearsetResources.Transform2D = Matrix.Identity;
            
            GearsetResources.Console = this;

            #if EMPTYKEYS
				#if MONOMAC
					var graphicsDevice = (IGraphicsDeviceManager)game.Services.GetService(typeof(IGraphicsDeviceManager));
					var graphicsDeviceManager = graphicsDevice as GraphicsDeviceManager;
					_userInterface = new EmptyKeysUserInterface(game.GraphicsDevice, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);
				#else
					_userInterface = new EmptyKeysUserInterface(game.GraphicsDevice, game.GraphicsDevice.PresentationParameters.BackBufferWidth, game.GraphicsDevice.PresentationParameters.BackBufferHeight);
				#endif
            #elif WPF
                _userInterface = new WpfUserInterface(createUI, game.GraphicsDevice, game.GraphicsDevice.PresentationParameters.BackBufferWidth, game.GraphicsDevice.PresentationParameters.BackBufferHeight);
			#else
				_userInterface = new NoUserInterface(game.GraphicsDevice, game.GraphicsDevice.PresentationParameters.BackBufferWidth, game.GraphicsDevice.PresentationParameters.BackBufferHeight);
            #endif
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize Gearset, this method should be called from your Game's Initialize
        /// and before any other Gearset method.
        /// </summary>
        public void Initialize()
        {
            GearsetSettings.Load();

            InitializeForAllPlatforms();
            InitializeForXbox();
            InitializeForDesktop();
            InitializeForWindowsPhone();
            Initialized = true;

            if (Logger != null)
            {
                Log("Gearset", "Gearset {0}. Go to www.thecomplot.com/gearset.html for more info.", typeof(GearConsole).Assembly.GetName().Version);
                Log("Gearset", "Source code is available at https://github.com/juancampa/Gearset.");        
            }
        }

        void manager_Reset(object sender, EventArgs e)
        {
        }
        #endregion

        #region Initialize (All platforms)
        /// <summary>
        /// Initializes componenets that work in all platforms.
        /// </summary>
        private void InitializeForAllPlatforms()
        {
            GearsetResources.Content = new ResourceContentManager(GearsetResources.Game.Services, Resource1.ResourceManager);

            // Graphics device stuff.
            GearsetResources.Effect = new BasicEffect(GearsetResources.Device);
            GearsetResources.Effect2D = new BasicEffect(GearsetResources.Device);
            GearsetResources.SpriteBatch = new SpriteBatch(GearsetResources.Device);
            
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix halfScreenOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            GearsetResources.Effect2D.View = halfScreenOffset * projection;
            GearsetResources.Effect2D.Projection = Matrix.Identity;
            GearsetResources.Effect2D.World = Matrix.Identity;
            GearsetResources.Effect2D.VertexColorEnabled = true;

            GearsetResources.Keyboard = new KeyboardComponent();
            BoundingBoxHelper.Initialize();

            Vector3Drawer = new Vector3Drawer();
            Components.Add(Vector3Drawer);

            Vector2Drawer = new Vector2Drawer();
            Components.Add(Vector2Drawer);

            Transform3Drawer = new Transform3Drawer();
            Components.Add(Transform3Drawer);

            SphereDrawer = new SphereDrawer();
            Components.Add(SphereDrawer);

            BoxDrawer = new BoxDrawer();
            Components.Add(BoxDrawer);

            SolidBoxDrawer = new SolidBoxDrawer();
            Components.Add(SolidBoxDrawer);

            TreeView = new TreeView();
            Components.Add(TreeView);

            Marker = new Marker();
            Components.Add(Marker);

            LineDrawer = new LineDrawer();
            Components.Add(LineDrawer);

            Labeler = new Labeler();
            Components.Add(Labeler);

            game.GraphicsDevice.DeviceReset += GraphicsDevice_DeviceReset;

            _userInterface.Initialise(game.Content, game.GraphicsDevice.PresentationParameters.BackBufferWidth, game.GraphicsDevice.PresentationParameters.BackBufferHeight);

            Logger = new LoggerManager(_userInterface);
            Components.Add(Logger);

            Profiler = new ProfilerManager(_userInterface);
            Components.Add(Profiler);

            MemoryMonitor = new MemoryMonitor();
            Components.Add(MemoryMonitor);
        }

        void RecreateGraphicResources()
        {
            GearsetResources.Effect = new BasicEffect(GearsetResources.Device);
            GearsetResources.Effect2D = new BasicEffect(GearsetResources.Device);
            GearsetResources.SpriteBatch = new SpriteBatch(GearsetResources.Device);
        }

        void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            RecreateGraphicResources();

            foreach (var component in Components)
            {
                component.OnResolutionChanged();
            }
        }
        #endregion

        #region Initialize (Windows)
        /// <summary>
        /// Initialize components that work on desktop platforms.
        /// </summary>
        private void InitializeForDesktop()
        {
#if WINDOWS || LINUX || MONOMAC
            GearsetResources.Font = GearsetResources.Content.Load<SpriteFont>("Default");
            GearsetResources.FontTiny = GearsetResources.Content.Load<SpriteFont>("Tiny");
            GearsetResources.FontAlert = GearsetResources.Content.Load<SpriteFont>("Alert");

            #if WINDOWS && !EMPTYKEYS
				GearsetResources.GameWindow = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(game.Window.Handle);

                #if MONOGAME
                    if (GearsetResources.GameWindow == null)
                    {
                        GearsetResources.GameWindow = new GameWindowMonitor(game);
                        GearsetResources.GameWindow.Show();
                        GearsetResources.GameWindow.TopMost = true;
                        GearsetResources.GameWindow.TopMost = false;
                    }
                #endif
            #endif

            GearsetResources.Mouse = new MouseComponent();

            // Add the game assembly to the compiler references.
            ReflectionHelper.CompilerParameters.ReferencedAssemblies.Add(GearsetResources.Game.GetType().Assembly.Location);

            DataSamplerManager = new DataSamplerManager();
            Components.Add(DataSamplerManager);

            Alerter = new Alerter();
            Components.Add(Alerter);

            var plotter = new Plotter();
            Plotter = plotter;
            Components.Add(plotter);

            // Create the components of the console.
            Picker = new Picker();
            Components.Add(Picker);

            Widget = new Widget(_userInterface);
            Components.Add(Widget);

            // Asynchronously check if there's a new version available
            ThreadPool.QueueUserWorkItem(CheckNewVersion);
            
            Finder = new Finder(_userInterface);
            Components.Add(Finder);

            Bender = new Bender(_userInterface);
            Components.Add(Bender);

            Inspector = new InspectorManager(_userInterface);
            Components.Add(Inspector);
            Inspector.Inspect("Gearset Settings", Settings, false);
            Inspector.Inspect("Game", GearsetResources.Game, false);

            if (GearsetResources.GameWindow != null)
            {
                GearsetResources.GameWindow.Move += GameWindow_Move;
                GearsetResources.GameWindow.Resize += GameWindow_Resize;
                GearsetResources.GameWindow.GotFocus += GameWindow_GotFocus;
            }
#endif
        }

        void GameWindow_Move(object sender, EventArgs e)
        {
            //_userInterface.MoveTo(GearsetResources.Game.Window
        }

        void GameWindow_GotFocus(object sender, EventArgs e)
        {
#if WINDOWS || LINUX || MONOMAC
            _userInterface.GotFocus();
                        
            GearsetResources.GameWindow.TopMost = true;
            GearsetResources.GameWindow.TopMost = false;
#endif
        }

        void GameWindow_Resize(object sender, EventArgs e)
        {
#if WINDOWS || LINUX || MONOMAC
            if (GearsetResources.GameWindow.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                _userInterface.Resize();
            }
#endif
        }

        /// <summary>
        /// Check if the latest version posted on The Complot site is different
        /// from our current version.
        /// </summary>
        private void CheckNewVersion(Object state)
        {
#if WINDOWS || LINUX || MONOMAC
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://www.thecomplot.com/latestversion");
            webRequest.UserAgent = typeof(GearConsole).Assembly.GetName().Version.ToString();
            webRequest.Method = "GET";
            webRequest.Proxy = WebRequest.GetSystemWebProxy();
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                WebResponse response = webRequest.GetResponse();
                using (TextReader reader = new StreamReader(response.GetResponseStream()))
                {
                    String latestVersion = reader.ReadLine();
                    var latestVersionNumbers = latestVersion.Split('.');
                    var currentVersionNumbers = typeof(GearConsole).Assembly.GetName().Version.ToString().Split('.');
                    // WARNING: 2 bools to represent 3 states. Both true is invalid.
                    var newVersionAvailable = false;
                    var currentVersionIsDev = false;
                    for (var i = 0; i < latestVersionNumbers.Length; i++)
                    {
                        int current, latest;
                        if (!int.TryParse(currentVersionNumbers[i], out current) || !int.TryParse(latestVersionNumbers[i], out latest)) 
                            continue;

                        if (latest > current)
                        {
                            newVersionAvailable = true;
                            break;
                        }
                        else if (latest < current)
                        {
                            currentVersionIsDev = true;
                            break;
                        }
                    }

                    #if WPF
                        //if (newVersionAvailable)
                        //{
                        //    Inspector.AddNotice("New Gearset version available", "http://www.thecomplot.com/gearsetdownload.html", "Get it now");
                        //}
                        //else if (currentVersionIsDev)
                        //{
                        //    Inspector.AddNotice("Unreleased version, do not distribute", "http://www.thecomplot.com/gearsetdownload.html", "Get latest release");
                        //}
                        Inspector.AddNotice("Gearset is now open source at Github", "https://github.com/PumpkinPaul/Gearset", "Please Contribute!");
                        Inspector.AddNotice("Also available on nuget for XNA and Monogame", "https://www.nuget.org/packages?q=gearset", "Get it now!");
                    #endif
                }
            }
            catch { }
            
#endif
        }

        #endregion

        #region Initialize (Xbox)
        private static void InitializeForXbox()
        {
#if XBOX
            GearsetResources.Font = XdtkResources.Content.Load<SpriteFont>("Default_Xbox360");
            GearsetResources.FontTiny = XdtkResources.Content.Load<SpriteFont>("Tiny_Xbox360");
            GearsetResources.FontAlert = XdtkResources.Content.Load<SpriteFont>("Alert_Xbox360");

            this.Alerter = new Alerter();
            this.Components.Add(Alerter);
#endif
        }
        #endregion

        #region Initialize (WINDOWS_PHONE)
        private static void InitializeForWindowsPhone()
        {
#if WINDOWS_PHONE
            GearsetResources.Font = GearsetResources.Content.Load<SpriteFont>("Default_wp");
            GearsetResources.FontTiny = GearsetResources.Content.Load<SpriteFont>("Tiny_wp");
            GearsetResources.FontAlert = GearsetResources.Content.Load<SpriteFont>("Alert_wp");

            GearsetResources.Console.Alerter = new Alerter();
            GearsetResources.Console.Components.Add(GearsetResources.Console.Alerter);
#endif
        }
        #endregion

        #region Set matrices
        /// <summary>
        /// Use this method after every Update of your game to update the camera
        /// matrices so 3D overlays can be drawn correctly.
        /// </summary>
        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection)
        {
            GearsetResources.World = world;
            GearsetResources.View = view;
            GearsetResources.Projection = projection;
            
        }
        /// <summary>
        /// Use this method after every Update of your game to update the camera
        /// matrices so 3D overlays can be drawn correctly.
        /// </summary>
        public void SetMatrices(ref Matrix world, ref Matrix view, ref Matrix projection, ref Matrix transform2D)
        {
            GearsetResources.World = world;
            GearsetResources.View = view;
            GearsetResources.Projection = projection;
            GearsetResources.Transform2D = transform2D;
        }
        #endregion

        
        #region Wrapper functions
        // Do not delete the WRAPPER FUNCTION comment below.
        // It's a mark for the function extractor
        // WRAPPER FUNCTIONS BEGIN

        #region Tree View
        /// <summary>
        /// Adds or modifiy a key without value on the overlaid tree view.
        /// </summary>
        /// <param name="key">A dot-separated list of keys.</param>
        public void Show(String key)
        {
            if (!Enabled) return;
            TreeView.Set(key, null);
        }

        /// <summary>
        /// Adds or modifies a key/value pair to the overlaid tree view.
        /// </summary>
        /// <param name="key">A dot-separated list of keys.</param>
        /// <param name="value">The value to show.</param>
        public void Show(String key, object value)
        {
            if (!Enabled) return;
            TreeView.Set(key, value);
        } 
        #endregion

        #region Actions
        /// <summary>
        /// Adds an action button to the bottom of the game window.
        /// </summary>
        /// <param name="name">Name of the action as it will appear on the button.</param>
        /// <param name="action">Action to perform when the button is clicked.</param>
        public void AddQuickAction(String name, Action action)
        {
#if WINDOWS || LINUX || MONOMAC
            if (!Enabled) return;
            Widget.AddAction(name, action);
#endif
        } 
        #endregion

        #region Plots
        /// <summary>
        /// Adds the provided value to the plot with the provided plotName.
        /// </summary>
        /// <param name="plotName">A name that represent a data set.</param>
        /// <param name="value">The value to add to the sampler</param>
        public void Plot(String plotName, float value)
        {
            if (!Enabled) 
                return;

            DataSamplerManager.AddSample(plotName, value);
            Plotter.ShowPlot(plotName);
        }

        /// <summary>
        /// Adds the provided value to the plot with the provided plotName. At the same time modifies
        /// the history length of the sampler.
        /// </summary>
        /// <param name="plotName">A name that represent a data set.</param>
        /// <param name="value">The value to add to the sampler</param>
        /// <param name="historyLength">The number of samples that the sampler will remember at any given time.</param>
        public void Plot(String plotName, float value, int historyLength)
        {
            if (!Enabled) 
                return;

            DataSamplerManager.AddSample(plotName, value, historyLength);
            Plotter.ShowPlot(plotName);
        } 
        #endregion

        #region Logger
        /// <summary>
        /// Los a message to the specified stream.
        /// </summary>
        /// <param name="streamName">Name of the Stream to log the message to</param>
        /// <param name="content">Message to log</param>
        public void Log(string streamName, string content)
        {
            if (!Enabled) 
                return;

            Logger.Log(streamName, content);
        }

        /// <summary>
        /// Logs the specified message in the default stream.
        /// </summary>
        /// <param name="content">The message to log.</param>
        public void Log(String content)
        {
            if (!Enabled) 
                return;

            Logger.Log(content);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        public void Log(String streamName, String format, Object arg0) 
        {
            if (!Enabled)
                return;

            Logger.Log(streamName, format, arg0);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        public void Log(String streamName, String format, Object arg0, Object arg1) 
        {
            if (!Enabled)
                return;

            Logger.Log(streamName, format, arg0, arg1);
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        /// <param name="arg1">The second format parameter</param>
        /// <param name="arg2">The third format parameter</param>
        public void Log(String streamName, String format, Object arg0, Object arg1, Object arg2)
        {
            if (!Enabled) 
                return;

            Logger.Log(streamName, format, arg0, arg1, arg2);
        }
        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="args">The format parameters</param>
        public void Log(String streamName, String format, params Object[] args)
        {
            if (!Enabled) 
                return;

            Logger.Log(streamName, format, args);
        }

        /// <summary>
        /// Shows a dialog asking for a filename and saves the log to the specified file.
        /// </summary>
        public void SaveLogToFile()
        {
            Logger.SaveLogToFile();
        }

        /// <summary>
        /// Saves the log to the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to save the log (usually ending in .log)</param>
        public void SaveLogToFile(string filename)
        {
            Logger.SaveLogToFile(filename);
        }
        #endregion

        #region Marks
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector3 position, Color color)
        {
            if (!Enabled) 
                return;

            Marker.ShowMark(key, position, color);
        }
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector3 position)
        {
            if (!Enabled) 
                return;

            Marker.ShowMark(key, position);
        }
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector2 position, Color color)
        {
            if (!Enabled) 
                return;

            Marker.ShowMark(key, position, color);
        }
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector2 position)
        {
            if (!Enabled) 
                return;

            Marker.ShowMark(key, position);
        } 
        #endregion

        #region Alert
        /// <summary>
        /// Shows huge text on the center of the screen which fades
        /// out quickly.
        /// </summary>
        public void Alert(String message)
        {
            if (!Enabled) 
                return;

            Alerter.Alert(message);
        } 
        #endregion

        #region Line Drawing
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector3 v1, Vector3 v2)
        {
            if (!Enabled) 
                return;

            LineDrawer.ShowLine(key, v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector3 v1, Vector3 v2, Color color)
        {
            if (!Enabled) 
                return;

            LineDrawer.ShowLine(key, v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector3 v1, Vector3 v2)
        {
            if (!Enabled) 
                return;

            LineDrawer.ShowLineOnce(v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector3 v1, Vector3 v2, Color color)
        {
            if (!Enabled)
                return;

            LineDrawer.ShowLineOnce(v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector2 v1, Vector2 v2)
        {
            if (!Enabled) 
                return;

            LineDrawer.ShowLine(key, v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector2 v1, Vector2 v2, Color color)
        {
            if (!Enabled) 
                return;

            LineDrawer.ShowLine(key, v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector2 v1, Vector2 v2)
        {
            if (!Enabled)
                return;

            LineDrawer.ShowLineOnce(v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector2 v1, Vector2 v2, Color color)
        {
            if (!Enabled) return;
            LineDrawer.ShowLineOnce(v1, v2, color);
        }
        #endregion

        #region Box Drawing
        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="key">Name of the persistent box</param>
        /// <param name="box">The box to draw</param>
        /// </summary>
        public void ShowBox(String key, BoundingBox box)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBox(key, box);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowBox(String key, Vector3 min, Vector3 max)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBox(key, min, max);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="key">Name of the persistent box</param>
        /// <param name="box">The box to draw</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        public void ShowBox(String key, BoundingBox box, Color color)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBox(key, box, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowBox(String key, Vector3 min, Vector3 max, Color color)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBox(key, min, max, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="box">The BoundingBox to draw</param>
        /// </summary>
        public void ShowBoxOnce(BoundingBox box)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBoxOnce(box);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowBoxOnce(Vector3 min, Vector3 max)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBoxOnce(min, max);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="box">The BoundingBox to draw</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        public void ShowBoxOnce(BoundingBox box, Color color)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBoxOnce(box, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        public void ShowBoxOnce(Vector3 min, Vector3 max, Color color)
        {
            if (!Enabled) 
                return;

            BoxDrawer.ShowBoxOnce(min, max, color);
        }
        #endregion

        #region Sphere Drawing
        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sphere"></param>
        public void ShowSphere(String key, BoundingSphere sphere)
        {
            if (!Enabled) 
                return;

            SphereDrawer.ShowSphere(key, sphere);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void ShowSphere(String key, Vector3 center, float radius)
        {
            if (!Enabled) 
                return;

            SphereDrawer.ShowSphere(key, center, radius);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sphere"></param>
        /// <param name="color"></param>
        public void ShowSphere(String key, BoundingSphere sphere, Color color)
        {
            if (!Enabled) 
                return;

            SphereDrawer.ShowSphere(key, sphere, color);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void ShowSphere(String key, Vector3 center, float radius, Color color)
        {
            if (!Enabled)
                return;

            SphereDrawer.ShowSphere(key, center, radius, color);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="sphere"></param>
        /// </summary>
        public void ShowSphereOnce(BoundingSphere sphere)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(sphere);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void ShowSphereOnce(Vector3 center, float radius)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(center, radius);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="color"></param>
        public void ShowSphereOnce(BoundingSphere sphere, Color color)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(sphere, color);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void ShowSphereOnce(Vector3 center, float radius, Color color)
        {
            if (!Enabled) 
                return;

            SphereDrawer.ShowSphereOnce(center, radius, color);
        }
        #endregion

        /// <summary>
        /// Shows a cylinder on the screen for one frame.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void ShowCylinderOnce(Vector3 center, Vector3 radius)
        {
            if (!Enabled) 
                return;

            SphereDrawer.ShowCylinderOnce(center, radius);
        }

        /// <summary>
        /// Shows a cylinder on the screen for one frame.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public void ShowCylinderOnce(Vector3 center, Vector3 radius, Color color)
        {
            if (!Enabled) 
                return;

            SphereDrawer.ShowCylinderOnce(center, radius, color);
        }

        #region Label Drawing
        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        public void ShowLabel(String name, Vector2 position)
        {
            if (!Enabled) 
                return;

            Labeler.ShowLabel(name, position);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        public void ShowLabel(String name, Vector2 position, String text)
        {
            if (!Enabled) return;
            Labeler.ShowLabel(name, position, text);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        /// <param name="color">Color of the text</param>
        public void ShowLabel(String name, Vector2 position, String text, Color color)
        {
            if (!Enabled) 
                return;

            Labeler.ShowLabel(name, position, text, color);
        }

        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        public void ShowLabel(String name, Vector3 position)
        {
            if (!Enabled) 
                return;

            Labeler.ShowLabel(name, position);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        public void ShowLabel(String name, Vector3 position, String text)
        {
            if (!Enabled) 
                return;

            Labeler.ShowLabel(name, position, text);
        }

        /// <summary>
        /// Shows a label at the specified positon that displays the specified text.
        /// </summary>
        /// <param name="name">Name of the label. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position of the label</param>
        /// <param name="text">Text to show on the label</param>
        /// <param name="color">Color of the text</param>
        public void ShowLabel(String name, Vector3 position, String text, Color color)
        {
            if (!Enabled) return;
            Labeler.ShowLabel(name, position, text, color);
        } 
        #endregion

        #region Inspector
        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        public void Inspect(String name, Object o)
        {
            if (!Enabled) 
                return;

            Inspector.Inspect(name, o);
        }

        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        public void Inspect(String name, Object o, bool autoExpand)
        {
            if (!Enabled) 
                return;

            Inspector.Inspect(name, o, autoExpand);
        }

        /// <summary>
        /// Removes an object from the Inspector window.
        /// </summary>
        public void RemoveInspect(Object o)
        {
            if (!Enabled) 
                return;

            Inspector.RemoveInspect(o);
        }

        /// <summary>
        /// Clears the Inspector Window.
        /// </summary>
        public void ClearInspector()
        {
            if (!Enabled) 
                return;

            Inspector.ClearInspectedObjects();
        }
        #endregion

        #region Finder
        /// <summary>
        /// Sets the function that is used by Gearset when a query is written to the
        /// Finder by the user. It usually searches through your game objects and returns
        /// a collection of the ones whose name or Type matches the query.
        /// A search function receives a String and return IEnumerable (e.g. a List)
        /// </summary>
        public void SetFinderSearchFunction(SearchFunction searchFunction)
        {
            if (!Enabled) return;
            Finder.Config.SearchFunction = searchFunction;
        }
        #endregion

        #region Transform
        /// <summary>
        /// Shows a persistent Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="name">Name of the persistent Matrix</param>
        /// <param name="transform">Transform to draw</param>
        /// <param name="axisScale">Scale to apply to each axis</param>
        public void ShowTransform(String name, Matrix transform, float axisScale)
        {
            if (!Enabled) return;
            Transform3Drawer.ShowTransform(name, transform, axisScale);
        }

        /// <summary>
        /// Shows a persistent Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="name">Name of the persistent Matrix</param>
        /// <param name="transform">Transform to draw</param>
        public void ShowTransform(String name, Matrix transform)
        {
            if (!Enabled) return;
            Transform3Drawer.ShowTransform(name, transform);
        }

        /// <summary>
        /// Shows a one-frame Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="transform">Transform to draw</param>
        public void ShowTransformOnce(Matrix transform)
        {
            if (!Enabled) return;
            Transform3Drawer.ShowTransformOnce(transform);
        }

        /// <summary>
        /// Shows a one-frame Matrix Transform on the screen as 3 orthogonal vectors.
        /// </summary>
        /// <param name="transform">Transform to draw</param>
        /// <param name="axisScale">Scale to apply to each axis</param>
        public void ShowTransformOnce(Matrix transform, float axisScale)
        {
            if (!Enabled) return;
            Transform3Drawer.ShowTransformOnce(transform, axisScale);
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Shows a persistent Vector3 on the screen as an arrow.
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        public void ShowVector3(String name, Vector3 location, Vector3 vector, Color color)
        {
            if (!Enabled) return;
            Vector3Drawer.ShowVector3(name, location, vector, color);
        }

        /// <summary>
        /// Shows a persistent Vector3 on the screen as an arrow.
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        public void ShowVector3(String name, Vector3 location, Vector3 vector)
        {
            if (!Enabled) return;
            Vector3Drawer.ShowVector3(name, location, vector);
        }

        /// <summary>
        /// Shows a Vector3 on the screen as an arrow for one frame.
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        public void ShowVector3Once(Vector3 location, Vector3 vector)
        {
            if (!Enabled) return;
            Vector3Drawer.ShowVector3Once(location, vector);
        }

        /// <summary>
        /// Shows a Vector3 on the screen as an arrow for one frame.
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        public void ShowVector3Once(Vector3 location, Vector3 vector, Color color)
        {
            if (!Enabled) return;
            Vector3Drawer.ShowVector3Once(location, vector, color);
        } 
        #endregion

        #region Vector2
        /// <summary>
        /// Shows a persistent Vector2 on the screen as an arrow (Screen space coordinates).
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        public void ShowVector2(String name, Vector2 location, Vector2 vector, Color color)
        {
            if (!Enabled) return;
            Vector2Drawer.ShowVector2(name, location, vector, color);
        }

        /// <summary>
        /// Shows a persistent Vector2 on the screen as an arrow (Screen space coordinates).
        /// </summary>
        /// <param name="name">Name of the persistent Vector</param>
        /// <param name="location">Location of the vector to draw (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        public void ShowVector2(String name, Vector2 location, Vector2 vector)
        {
            if (!Enabled) return;
            Vector2Drawer.ShowVector2(name, location, vector);
        }

        /// <summary>
        /// Shows a Vector2 on the screen as an arrow for one frame (Screen space coordinates).
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        public void ShowVector2Once(Vector2 location, Vector2 vector)
        {
            if (!Enabled) return;
            Vector2Drawer.ShowVector2Once(location, vector);
        }

        /// <summary>
        /// Shows a Vector2 on the screen as an arrow for one frame.
        /// </summary>
        /// <param name="location">Location of the vector to show (i.e. position of the start of the arrow)</param>
        /// <param name="vector">Vector to show</param>
        /// <param name="color">Color of the arrow to draw</param>
        public void ShowVector2Once(Vector2 location, Vector2 vector, Color color)
        {
            if (!Enabled) return;
            Vector2Drawer.ShowVector2Once(location, vector, color);
        } 
        #endregion

        #region Bender
        /// <summary>
        /// Adds a curve for editing in Bender
        /// </summary>
        /// <param name="name">Name of the curve to add. Group using dot separators.</param>
        /// <param name="curve">Curve to edit in Bender.</param>
        public void AddCurve(String name, Curve curve)
        {
            if (!Enabled) return;
            Bender.AddCurve(name, curve);
        }
        /// <summary>
        /// Removes the provided curve from Bender.
        /// </summary>
        public void RemoveCurve(Curve curve)
        {
            if (!Enabled) return;
            Bender.RemoveCurve(curve);
        }
        /// <summary>
        /// Removes a Curve or a Group by name. The complete dot-separated
        /// path to the curve or group must be given.
        /// </summary>
        public void RemoveCurveOrGroup(String name)
        {
            if (!Enabled) return;
            Bender.RemoveCurveOrGroup(name);
        }
        #endregion


        #region Profiler
        public void StartFrame()
        {
            if (!Enabled) return;
            Profiler.StartFrame();
        }

        /// <summary>
        /// Start measure time.
        /// </summary>
        /// <param name="markerName"></param>
        /// <param name="color"></param>
        public void BeginMark(string markerName, Color color)
        {
            if (!Enabled) return;
            Profiler.BeginMark(markerName, color);
        }

        /// <summary>
        /// End measuring.
        /// </summary>
        /// <param name="markerName">Name of marker.</param>
        public void EndMark(string markerName)
        {
            if (!Enabled) return;
            Profiler.EndMark(markerName);
        }
        #endregion

        #endregion

        /// <summary>
        /// Clears all Gearset Components erasing all retained data. Inspector and Logger won't be cleared.
        /// </summary>
        public void ClearAll()
        {
            Settings.TreeViewConfig.Clear();
            Settings.LabelerConfig.Clear();
            Settings.LineDrawerConfig.Clear();
            Settings.PlotterConfig.Clear();
        }

        // WRAPPER FUNCTIONS END

        #region Update
        /// <summary>
        /// Call this method at the end of your game's Update method.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (!Initialized)
                Console.WriteLine("Gearset has not being initialized. Call the Initialize method from your game's Initialize.");

            // Check if we have to save now.
            if ((Environment.TickCount - lastSaveTime) / 1000f >= Settings.SaveFrequency)
            {
                ThreadPool.QueueUserWorkItem(BackgroundSave);
                lastSaveTime = Environment.TickCount;
            }
            UpdateCount++;
            if (Enabled)
            {
//                Show("Gearset.Update Count", UpdateCount);
//                Show("Gearset.Draw Count", DrawCount);

                UI.UIManager.Update(gameTime);

                // Update the global visibility.
                if (GearsetResources.Keyboard.IsKeyJustDown(Keys.Space) && GearsetResources.Keyboard.IsKeyDown(Keys.LeftControl))
                {
                    VisibleOverlays = !VisibleOverlays;
                }
                if (VisibleOverlays && GearsetResources.GlobalAlpha < 1)
                {
                    GearsetResources.GlobalAlpha += 0.24f;
                    GearsetResources.GlobalAlpha = MathHelper.Clamp(GearsetResources.GlobalAlpha, 0, 1);
                }
                else if (!VisibleOverlays && GearsetResources.GlobalAlpha > 0)
                {
                    GearsetResources.GlobalAlpha -= 0.24f;
                    GearsetResources.GlobalAlpha = MathHelper.Clamp(GearsetResources.GlobalAlpha, 0, 1);
                }

                GearsetResources.Keyboard.Update(gameTime);
#if WINDOWS || LINUX || MONOMAC
                GearsetResources.Mouse.Update(gameTime);
#endif
                foreach (Gear gear in Components)
                {
                    UpdateRecursively(gear, gameTime);
                }
            }

            _userInterface.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        private void UpdateRecursively(Gear gear, GameTime gameTime)
        {
            if (gear.Enabled)
            {
                foreach (var child in gear.Children)
                {
                    UpdateRecursively(child, gameTime);
                }
                gear.Update(gameTime);
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Call this method at the end of your game's Draw method.
        /// </summary>
        public void Draw(GameTime gameTime)
        {
            DrawCount++;

            if (Settings.Enabled && GearsetResources.GlobalAlpha > 0)
            {

                if (GearsetResources.Effect.GraphicsDevice.IsDisposed)
                    RecreateGraphicResources();

                if (GearsetResources.Game.IsMouseVisible == false)
                {
                    Vector2 pos = GearsetResources.Mouse.Position;
//                    Show("Position", pos);
                    Matrix t = Matrix.Invert(GearsetResources.Transform2D);
                    LineDrawer.ShowLineOnce(Vector2.Transform(pos + Vector2.UnitY * 2, t), Vector2.Transform(pos - Vector2.UnitY * 3, t), Color.White);
                    LineDrawer.ShowLineOnce(Vector2.Transform(pos + Vector2.UnitX * 2, t), Vector2.Transform(pos - Vector2.UnitX * 3, t), Color.White);
                }

                #region Basic Effect 3D/2D Pass
                // Set up the effect parameters.
                GearsetResources.Effect.View = GearsetResources.View;
                GearsetResources.Effect.Projection = GearsetResources.Projection;
                GearsetResources.Effect.World = Matrix.Identity;
                GearsetResources.Effect.VertexColorEnabled = true;
                GearsetResources.Effect.Alpha = GearsetResources.GlobalAlpha;

                // Set the Render states
                GearsetResources.Device.RasterizerState = RasterizerState.CullNone;
                GearsetResources.Device.BlendState = BlendState.NonPremultiplied;
                if (Settings.DepthBufferEnabled)
                    GearsetResources.Device.DepthStencilState = DepthStencilState.DepthRead;
                else
                    GearsetResources.Device.DepthStencilState = DepthStencilState.None;

                // 3D
                GearsetResources.Effect.CurrentTechnique.Passes[0].Apply();
                GearsetResources.CurrentRenderPass = RenderPass.BasicEffectPass;
                foreach (Gear component in Components)
                    if (component.Visible)
                        DrawRecursively(component, gameTime);

                // 2D (Screen Space)
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 0, 0, 1);
                Matrix halfScreenOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
                GearsetResources.Effect2D.View = halfScreenOffset * projection;
                GearsetResources.Effect2D.Projection = Matrix.Identity;
                GearsetResources.Effect2D.World = Matrix.Identity;
                GearsetResources.Effect2D.VertexColorEnabled = true;
                GearsetResources.Effect2D.Alpha = GearsetResources.GlobalAlpha;
                GearsetResources.Effect2D.CurrentTechnique.Passes[0].Apply();
                GearsetResources.CurrentRenderPass = RenderPass.ScreenSpacePass;
                foreach (Gear component in Components)
                    if (component.Visible)
                        DrawRecursively(component, gameTime);

                // 2D (Screen Space)
                GearsetResources.Effect2D.Alpha = GearsetResources.GlobalAlpha;
                GearsetResources.Effect2D.World = GearsetResources.Transform2D;
                GearsetResources.Effect2D.CurrentTechnique.Passes[0].Apply();
                GearsetResources.CurrentRenderPass = RenderPass.GameSpacePass;
                foreach (Gear component in Components)
                    if (component.Visible)
                        DrawRecursively(component, gameTime);

                #endregion

                #region Sprite Batch Pass
                GearsetResources.SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.Identity);
                GearsetResources.CurrentRenderPass = RenderPass.SpriteBatchPass;
                foreach (Gear component in Components)
                    if (component.Visible)
                        DrawRecursively(component, gameTime);
                GearsetResources.SpriteBatch.End();
                #endregion

                #region Custom Pass
                GearsetResources.CurrentRenderPass = RenderPass.CustomPass;
                foreach (Gear component in Components)
                    if (component.Visible)
                        DrawRecursively(component, gameTime);
                #endregion
            }

            _userInterface.Draw(gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        private void DrawRecursively(Gear gear, GameTime gameTime)
        {
            if (gear.Visible)
            {
                gear.Draw(gameTime);
                foreach (var child in gear.Children)
                {
                    DrawRecursively(child, gameTime);
                }
            }
        }
        #endregion

        private void OnExit(Object sender, EventArgs args)
        {
#if WINDOWS || LINUX || MONOMAC
            GearsetSettings.Save();
#endif
        }

        private void BackgroundSave(Object state)
        {
            GearsetSettings.Save();
        }
    }
}


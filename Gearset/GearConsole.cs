#region Using Statements

using Gearset.Components.Profiler;
using System;
using System.Collections;
using System.Collections.Generic;
using Gearset.Components;
using Gearset.Components.Persistor;
#if WINDOWS
using Gearset.Components.InspectorWPF;
using Microsoft.CSharp;
using Gearset.Components.Logger;
#endif
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Gearset;
using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using Gearset.Components.Data;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
#if WINDOWS
using System.Net;
using System.Windows;
using System.Windows.Interop;
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
        internal bool Enabled { get { return Settings.Enabled; } }

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

#if WINDOWS
        /// <summary>
        /// Components that keeps a list of pickable game elements and
        /// interacts with the mouse to get notified when they are being
        /// picked. Useful to visually place objects in the Inspector.
        /// </summary>
        internal Picker Picker { get; private set; }

        /// <summary>
        /// This component is for internal use. Allows XDTK to save data
        /// that can be later retreived.
        /// </summary>
        //internal Persistor Persistor { get; private set; }

        /// <summary>
        /// Component that let the game developer inspect game objects
        /// in real time, showing the value of public fields. The inspector
        /// is also capable of calling methods on objects.
        /// </summary>
        internal InspectorManager Inspector { get; private set; }

        /// <summary>
        /// Widget that places itself on top of the windows titlebar.
        /// </summary>
        internal Widget Widget { get; private set; }

        /// <summary>
        /// Component that logs events.
        /// </summary>
        internal LoggerManager Logger { get; private set; }

        /// <summary>
        /// Component used to edit XNA curves.
        /// </summary>
        internal Bender Bender { get; private set; }
        
        /// <summary>
        /// Provides UI to find a Game Object.
        /// </summary>
        internal Finder Finder { get; private set; }
#endif

        /// <summary>
        /// Code profiler.
        /// </summary>
        public Gearset.Components.Profiler.Profiler Profiler { get; private set; }

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

        #region Constructor
        /// <summary>
        /// Creates the GearConsole. if you want the console
        /// to draw 3D debug stuff you need to update the World/View/Projection
        /// matrices using the <c>SetMatrices</c> or setting them manually.
        /// </summary>
        /// <param name="game"></param>
        public GearConsole(Game game)
        {
            this.game = game;
            game.Exiting += OnExit;
            this.Components = new List<Gear>();

            GearsetResources.Game = game;
            GearsetResources.World = Matrix.Identity;
            GearsetResources.View = Matrix.Identity;
            GearsetResources.Projection = Matrix.Identity;
            GearsetResources.Transform2D = Matrix.Identity;
            
            GearsetResources.Console = this;

        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize Gearset, this method should be called from your Game's Initialize
        /// and before any other Gearset method.
        /// </summary>
        public void Initialize()
        {
#if WINDOWS
            String versionFull = typeof(GearConsole).Assembly.GetName().Version.ToString();
            String version = typeof(GearConsole).Assembly.GetName().Version.Major.ToString();

            String productName = String.Empty;
            String copyright = String.Empty;
            var attributes = typeof(GearConsole).Assembly.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                var assemblyProduct = attribute as AssemblyProductAttribute;
                if (assemblyProduct != null)
                    productName = assemblyProduct.Product;
                var assemblyCopyright = attribute as AssemblyCopyrightAttribute;
                if (assemblyCopyright != null)
                    copyright = assemblyCopyright.Copyright;
            }
            GearsetResources.AboutViewModel = new About.AboutViewModel(
                productName + " v" + versionFull, copyright);
#endif

            GearsetSettings.Load();

            InitializeForAllPlatforms();
            InitializeForXbox();
            InitializeForWindows();
            InitializeForWindowsPhone();
            Initialized = true;

#if WINDOWS
            if (Logger != null)
                Log("Gearset", "Gearset {0}. Go to www.thecomplot.com/gearset.html for more info.", typeof(GearConsole).Assembly.GetName().Version);
#endif
            
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

            this.Vector3Drawer = new Vector3Drawer();
            this.Components.Add(this.Vector3Drawer);

            this.Vector2Drawer = new Vector2Drawer();
            this.Components.Add(this.Vector2Drawer);

            this.Transform3Drawer = new Transform3Drawer();
            this.Components.Add(this.Transform3Drawer);

            this.SphereDrawer = new SphereDrawer();
            this.Components.Add(this.SphereDrawer);

            this.BoxDrawer = new BoxDrawer();
            this.Components.Add(this.BoxDrawer);

            this.SolidBoxDrawer = new SolidBoxDrawer();
            this.Components.Add(this.SolidBoxDrawer);

            this.TreeView = new TreeView();
            this.Components.Add(TreeView);

            this.Marker = new Marker();
            this.Components.Add(Marker);

            this.LineDrawer = new LineDrawer();
            this.Components.Add(LineDrawer);

            this.Labeler = new Labeler();
            this.Components.Add(Labeler);

#if WINDOWS
            this.Profiler = new WindowsProfiler();
#else
            this.Profiler = new Profiler();
#endif
            this.Components.Add(Profiler);

            game.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
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
        /// Initialize components that work on Windows.
        /// </summary>
        private void InitializeForWindows()
        {
#if WINDOWS
            GearsetResources.Font = GearsetResources.Content.Load<SpriteFont>("Default");
            GearsetResources.FontTiny = GearsetResources.Content.Load<SpriteFont>("Tiny");
            GearsetResources.FontAlert = GearsetResources.Content.Load<SpriteFont>("Alert");

            GearsetResources.GameWindow = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(game.Window.Handle);
            GearsetResources.Mouse = new MouseComponent();

            // Add the game assembly to the compiler references.
            ReflectionHelper.CompilerParameters.ReferencedAssemblies.Add(GearsetResources.Game.GetType().Assembly.Location);

            DataSamplerManager = new DataSamplerManager();
            this.Components.Add(DataSamplerManager);

            this.Alerter = new Alerter();
            this.Components.Add(Alerter);

            var plotter = new Plotter();
            Plotter = plotter;
            this.Components.Add(plotter);

            // Create the components of the console.
            this.Picker = new Picker();
            this.Components.Add(Picker);

            GearsetResources.AboutWindow = new AboutWindow();
            GearsetResources.AboutWindow.DataContext = GearsetResources.AboutViewModel;

            this.Widget = new Widget();
            this.Components.Add(Widget);

            this.Inspector = new InspectorManager();
            this.Components.Add(Inspector);

            // Asynchronously check if there's a new version available
            ThreadPool.QueueUserWorkItem(CheckNewVersion);
            
            this.Finder = new Finder();
            this.Components.Add(Finder);

            this.Logger = new LoggerManager();
            this.Components.Add(Logger);

            this.Bender = new Bender();
            this.Components.Add(Bender);

            //this.Persistor = new Persistor();
            //this.Components.Add(persistor);

            Inspector.Inspect("Gearset Settings", Settings, false);
            Inspector.Inspect("Game", GearsetResources.Game, false);

            GearsetResources.GameWindow.Resize += new EventHandler(GameWindow_Resize);
            GearsetResources.GameWindow.GotFocus += new EventHandler(GameWindow_GotFocus);
            GearsetResources.GameWindow.Activated += new EventHandler(GameWindow_Activated);
#endif
        }

        void GameWindow_Activated(object sender, EventArgs e)
        {
            //if (Inspector.Window.IsVisible)
            //{
            //    //Inspector.Window.WindowState = WindowState.Minimized;
            //    Inspector.Window.WindowState = WindowState.Normal;
            //    //Inspector.Window.Topmost = false;
            //    Console.WriteLine("Activated");
            //}
            //if (Logger.Window.IsVisible)
            //    Logger.Window.Show();
        }

        void GameWindow_GotFocus(object sender, EventArgs e)
        {
#if WINDOWS
            if (Inspector.Window.IsVisible)
            {
                Inspector.Window.Topmost = true;
                Inspector.Window.Topmost = false;
            }
            if (Inspector.Window.WasHiddenByGameMinimize && !Inspector.Window.IsVisible)
                Inspector.Window.Show();

            if (Logger.Window.IsVisible)
            {
                Logger.Window.Topmost = true;
                Logger.Window.Topmost = false;
            }
            if (Logger.Window.WasHiddenByGameMinimize && !Logger.Window.IsVisible)
                Logger.Window.Show();

            if (Finder.Window.IsVisible)
            {
                Finder.Window.Topmost = true;
                Finder.Window.Topmost = false;
            }
            if (Finder.Window.WasHiddenByGameMinimize && !Finder.Window.IsVisible)
                Finder.Window.Show();

            if (Bender.Window.IsVisible)
            {
                Bender.Window.Topmost = true;
                Bender.Window.Topmost = false;
            }
            if (Bender.Window.WasHiddenByGameMinimize && !Bender.Window.IsVisible)
                Bender.Window.Show();

            Inspector.Window.WasHiddenByGameMinimize = false;
            Logger.Window.WasHiddenByGameMinimize = false;
            Finder.Window.WasHiddenByGameMinimize = false;
            Bender.Window.WasHiddenByGameMinimize = false;

            Widget.Window.Topmost = true;
            Widget.Window.Topmost = false;
            GearsetResources.GameWindow.TopMost = true;
            GearsetResources.GameWindow.TopMost = false;
#endif
        }

        void GameWindow_Resize(object sender, EventArgs e)
        {
#if WINDOWS
            if (GearsetResources.GameWindow.WindowState == System.Windows.Forms.FormWindowState.Minimized)
            {
                if (Inspector.Window.IsVisible)
                {
                    Inspector.Window.Hide();
                    Inspector.Window.WasHiddenByGameMinimize = true;
                }
                if (Logger.Window.IsVisible)
                {
                    Logger.Window.Hide();
                    Logger.Window.WasHiddenByGameMinimize = true;
                }
                if (Finder.Window.IsVisible)
                {
                    Finder.Window.Hide();
                    Finder.Window.WasHiddenByGameMinimize = true;
                }
                if (Bender.Window.IsVisible)
                {
                    Bender.Window.Hide();
                    Bender.Window.WasHiddenByGameMinimize = true;
                }
            }
#endif
        }

        /// <summary>
        /// Check if the latest version posted on The Complot site is different
        /// from our current version.
        /// </summary>
        private void CheckNewVersion(Object state)
        {
#if WINDOWS
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
                    bool newVersionAvailable = false;
                    bool currentVersionIsDev = false;
                    for (int i = 0; i < latestVersionNumbers.Length; i++)
                    {
                        int current, latest;
                        if (int.TryParse(currentVersionNumbers[i], out current) && int.TryParse(latestVersionNumbers[i], out latest))
                        {
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
             
                    }
                    if (newVersionAvailable)
                    {
                        Inspector.AddNotice("New Gearset version available", "http://www.thecomplot.com/gearsetdownload.html", "Get it now");
                        
                    }
                    else if (currentVersionIsDev)
                    {
                        Inspector.AddNotice("Unreleased version, do not distribute", "http://www.thecomplot.com/gearsetdownload.html", "Get latest release");
                    }
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
#if WINDOWS
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
            if (!Enabled) return;
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
            if (!Enabled) return;
            DataSamplerManager.AddSample(plotName, value, historyLength);
            Plotter.ShowPlot(plotName);
        } 
        #endregion

        #region Logger
        /// <summary>
        /// Los a message to the specified stream.
        /// </summary>
        /// <param name="streamName">Name of the Stream to log the message to</param>
        /// <param name="message">Message to log</param>
        public void Log(String streamName, String content)
        {
#if WINDOWS
            if (!Enabled) return;
            Logger.Log(streamName, content);
#endif
        }

        /// <summary>
        /// Logs the specified message in the default stream.
        /// </summary>
        /// <param name="content">The message to log.</param>
        public void Log(String content)
        {
#if WINDOWS
            if (!Enabled) return;
            Logger.Log(content);
#endif
        }

        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The first format parameter</param>
        public void Log(String streamName, String format, Object arg0) 
        {
#if WINDOWS
            if (!Enabled) return;
            Logger.Log(streamName, format, arg0);
#endif
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
#if WINDOWS
            if (!Enabled) return;
            Logger.Log(streamName, format, arg0, arg1);
#endif
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
#if WINDOWS
            if (!Enabled) return;
            Logger.Log(streamName, format, arg0, arg1, arg2);
#endif
        }
        /// <summary>
        /// Logs a formatted string to the specified stream.
        /// </summary>
        /// <param name="streamName">Stream to log to</param>
        /// <param name="format">The format string</param>
        /// <param name="arg0">The format parameters</param>
        public void Log(String streamName, String format, params Object[] args)
        {
#if WINDOWS
            if (!Enabled) return;
            Logger.Log(streamName, format, args);
#endif
        }

        /// <summary>
        /// Shows a dialog asking for a filename and saves the log to the specified file.
        /// </summary>
        public void SaveLogToFile()
        {
#if WINDOWS
            Logger.SaveLogToFile();
#endif
        }

        /// <summary>
        /// Saves the log to the specified file.
        /// </summary>
        /// <param name="filename">Name of the file to save the log (usually ending in .log)</param>
        public void SaveLogToFile(string filename)
        {
#if WINDOWS
            Logger.SaveLogToFile(filename);
#endif 
        }
        #endregion

        #region Marks
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector3 position, Color color)
        {
            if (!Enabled) return;
            this.Marker.ShowMark(key, position, color);
        }
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector3 position)
        {
            if (!Enabled) return;
            this.Marker.ShowMark(key, position);
        }
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector2 position, Color color)
        {
            if (!Enabled) return;
            this.Marker.ShowMark(key, position, color);
        }
        /// <summary>
        /// This is an experimental feature.
        /// </summary>
        public void ShowMark(String key, Vector2 position)
        {
            if (!Enabled) return;
            this.Marker.ShowMark(key, position);
        } 
        #endregion

        #region Alert
        /// <summary>
        /// Shows huge text on the center of the screen which fades
        /// out quickly.
        /// </summary>
        public void Alert(String message)
        {
            if (!Enabled) return;
            this.Alerter.Alert(message);
        } 
        #endregion

        #region Line Drawing
        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector3 v1, Vector3 v2)
        {
            if (!Enabled) return;
            LineDrawer.ShowLine(key, v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector3 v1, Vector3 v2, Color color)
        {
            if (!Enabled) return;
            LineDrawer.ShowLine(key, v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector3 v1, Vector3 v2)
        {
            if (!Enabled) return;
            LineDrawer.ShowLineOnce(v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector3 v1, Vector3 v2, Color color)
        {
            if (!Enabled) return;
            LineDrawer.ShowLineOnce(v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector2 v1, Vector2 v2)
        {
            if (!Enabled) return;
            LineDrawer.ShowLine(key, v1, v2, Color.White);
        }

        /// <summary>
        /// Draws a line between two points.
        /// </summary>
        public void ShowLine(String key, Vector2 v1, Vector2 v2, Color color)
        {
            if (!Enabled) return;
            LineDrawer.ShowLine(key, v1, v2, color);
        }

        /// <summary>
        /// Draws a line between two points for one frame.
        /// </summary>
        public void ShowLineOnce(Vector2 v1, Vector2 v2)
        {
            if (!Enabled) return;
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
            if (!Enabled) return;
            BoxDrawer.ShowBox(key, box);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowBox(String key, Vector3 min, Vector3 max)
        {
            if (!Enabled) return;
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
            if (!Enabled) return;
            BoxDrawer.ShowBox(key, box, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowBox(String key, Vector3 min, Vector3 max, Color color)
        {
            if (!Enabled) return;
            BoxDrawer.ShowBox(key, min, max, color);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="box">The BoundingBox to draw</param>
        /// </summary>
        public void ShowBoxOnce(BoundingBox box)
        {
            if (!Enabled) return;
            BoxDrawer.ShowBoxOnce(box);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowBoxOnce(Vector3 min, Vector3 max)
        {
            if (!Enabled) return;
            BoxDrawer.ShowBoxOnce(min, max);
        }

        /// <summary>
        /// Shows an axis aligned bounding box for one frame.
        /// <param name="box">The BoundingBox to draw</param>
        /// <param name="color">The color that will be used to draw the box</param>
        /// </summary>
        public void ShowBoxOnce(BoundingBox box, Color color)
        {
            if (!Enabled) return;
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
            if (!Enabled) return;
            BoxDrawer.ShowBoxOnce(min, max, color);
        }
        #endregion

        #region Sphere Drawing
        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphere(String key, BoundingSphere sphere)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphere(key, sphere);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphere(String key, Vector3 center, float radius)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphere(key, center, radius);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphere(String key, BoundingSphere sphere, Color color)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphere(key, sphere, color);
        }

        /// <summary>
        /// Shows a sphere on the screen.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphere(String key, Vector3 center, float radius, Color color)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphere(key, center, radius, color);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphereOnce(BoundingSphere sphere)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(sphere);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphereOnce(Vector3 center, float radius)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(center, radius);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphereOnce(BoundingSphere sphere, Color color)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(sphere, color);
        }

        /// <summary>
        /// Shows a sphere on the screen for one frame.
        /// <param name="min">Minimum values of the box in each axis</param>
        /// <param name="max">Maximum values of the box in each axis</param>
        /// </summary>
        public void ShowSphereOnce(Vector3 center, float radius, Color color)
        {
            if (!Enabled) return;
            SphereDrawer.ShowSphereOnce(center, radius, color);
        }
        #endregion

        #region Label Drawing
        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        public void ShowLabel(String name, Vector2 position)
        {
            if (!Enabled) return;
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
            if (!Enabled) return;
            Labeler.ShowLabel(name, position, text, color);
        }

        /// <summary>
        /// Shows a label at the specified position (the text will be the label's name).
        /// </summary>
        /// <param name="name">Name of the label as well of the text to show. Subsequent calls with the same name will modify this label</param>
        /// <param name="position">Position where the label will be shown</param>
        public void ShowLabel(String name, Vector3 position)
        {
            if (!Enabled) return;
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
#if WINDOWS
            if (!Enabled) return;
            Inspector.Inspect(name, o);
#endif
        }

        /// <summary>
        /// Sends an object to the Inspector window.
        /// </summary>
        public void Inspect(String name, Object o, bool autoExpand)
        {
#if WINDOWS
            if (!Enabled) return;
            Inspector.Inspect(name, o, autoExpand);
#endif
        }

        /// <summary>
        /// Removes an object from the Inspector window.
        /// </summary>
        public void RemoveInspect(Object o)
        {
#if WINDOWS
            if (!Enabled) return;
            Inspector.RemoveInspect(o);
#endif
        }

        /// <summary>
        /// Clears the Inspector Window.
        /// </summary>
        public void ClearInspector()
        {
#if WINDOWS
            if (!Enabled) return;
            Inspector.ClearInspectedObjects();
#endif
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
#if WINDOWS
            if (!Enabled) return;
            Finder.Config.SearchFunction = searchFunction;
#endif
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
        /// <param name="barIndex">index of bar</param>
        /// <param name="markerName">name of marker.</param>
        /// <param name="color">color</param>
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
            if (!Enabled) return;
            this.Show("Gearset.Update Count", UpdateCount);
            this.Show("Gearset.Draw Count", DrawCount);

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
#if WINDOWS
            GearsetResources.Mouse.Update(gameTime);
#endif
            foreach (Gear gear in Components)
            {
                UpdateRecursively(gear, gameTime);
            }
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

            if (!Settings.Enabled || GearsetResources.GlobalAlpha <= 0) return;

            if (GearsetResources.Effect.GraphicsDevice.IsDisposed)
                RecreateGraphicResources();

            if (GearsetResources.Game.IsMouseVisible == false)
            {
                Vector2 pos = GearsetResources.Mouse.Position;
                Show("Position", pos);
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
            GearsetResources.Effect.Techniques[0].Passes[0].Apply();
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
            GearsetResources.Effect2D.Techniques[0].Passes[0].Apply();
            GearsetResources.CurrentRenderPass = RenderPass.ScreenSpacePass;
            foreach (Gear component in Components)
                if (component.Visible)
                    DrawRecursively(component, gameTime);

            // 2D (Screen Space)
            GearsetResources.Effect2D.Alpha = GearsetResources.GlobalAlpha;
            GearsetResources.Effect2D.World = GearsetResources.Transform2D;
            GearsetResources.Effect2D.Techniques[0].Passes[0].Apply();
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
#if WINDOWS
            GearsetSettings.Save();
#endif
        }

        private void BackgroundSave(Object state)
        {
            GearsetSettings.Save();
        }
    }
}


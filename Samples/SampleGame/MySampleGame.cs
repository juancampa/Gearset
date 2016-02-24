//Uncomment this compiler directive to easily remove GEAREST for testing purposes
//#undef USE_GEARSET

using System;
using System.Collections.Generic;
using System.Threading;
using Gearset;
using Gearset.Components.Profiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SampleGame.Gearset;

namespace SampleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MySampleGame : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }

        //Game Components
        FpsCounter _fpsCounter;
        
        KeyboardState _keyboardState;
        KeyboardState _previousKeyboardState;

        Matrix _worldMatrix;
        Matrix _viewMatrix;
        Matrix _projectionMatrix;

        //Finder test! A custom 'scene graph' used by finder
        readonly List<GameObject> _gameObjects = new List<GameObject>(); 

        public MySampleGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
			Graphics.IsFullScreen = false;
			Graphics.ApplyChanges ();

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Initialise Gearset with the full UI experience.
            //GS.Initialize(this);
            //GS.Initialize(this, createUI: true);

            //Initialise Gearset in 'headless' mode with no 'external' UI - (overlays are still available).
            //We want to monitor managed memory allocations from our app and the UIs generate a fair amount of garbage which would distort the profiling.
            GS.Initialize(this, createUI: true);

            _fpsCounter = new FpsCounter(this);
            Components.Add(_fpsCounter);
            
            IsMouseVisible = true;

            base.Initialize();

            #if USE_GEARSET
                GearsetSettings.Instance.MemoryMonitorConfig.Visible = true;
                GearsetSettings.Instance.MemoryMonitorConfig.MemoryGraphConfig.Visible = true;
                GearsetSettings.Instance.MemoryMonitorConfig.MemoryGraphConfig.Position = new Vector2(100, 50);
                GearsetSettings.Instance.MemoryMonitorConfig.MemoryGraphConfig.Size = new Vector2(400, 75);
            #endif

            GS.Action(()=>GS.GearsetComponent.Console.Inspect("Profiler", new ProfilerInpectorSettings(GS.GearsetComponent.Console.Profiler)));

            GS.Log("I am a log message - tra la la");

            //Finder!
            //You can double click finder result items to add them to the Inspector window!
            //Comment this to use the default search query for finder (by default it's GameComponent based).
            GS.Action(ConfigureFinder);

            //Quick Actions
            GS.AddQuickAction("QuickAction1", () => { /* Pass in a delegate here to do something */ });
            GS.AddQuickAction("QuickAction2", () => { /* Pass in a delegate here to do something */ });

            //Command Console
            //Comes with 3 built in commands (help, cls, echo).
            
            //Some examples of things you can do...
            GS.RegisterCommand("helloworld", "runs a test command", (host, command, args) => 
            {
                host.EchoWarning("Hello World");
            });

            GS.RegisterCommand("echotest", "echotest message [warning|error]", (host, command, args) =>
            {
                if (args.Count < 1)
                    host.EchoError("You must specify a message");
                else if (args.Count == 1)
                    host.Echo(args[0]);
                else if (args.Count == 2)
                {
                    if (args[1] == "warning")
                        host.EchoWarning(args[0]);
                    else if (args[1] == "error")
                        host.EchoError(args[0]);
                    else
                        host.Echo(args[0]);
                }
                else if (args.Count > 2)
                {
                    host.Echo("Too many arguments specified");
                }
            });

            GS.RegisterCommand("fixedstep", "toggles Game.IsFixedStep", (host, command, args) =>
            {
                IsFixedTimeStep = !IsFixedTimeStep;
                host.Echo($"IsFixedStep: {IsFixedTimeStep}");
            });

            //Let's list all those registered commands now...
            GS.ExecuteCommand("help");

            //Call one of our custom commands...
            GS.ExecuteCommand("helloworld");

            //Set Gerset Matrices if you want to show ovelaid geometry (boxes, spheres, etc).
            var prj = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
            var halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            _worldMatrix = Matrix.Identity;
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = halfPixelOffset * prj;
        }
			
        void ConfigureFinder()
        {
            //This is an example of a 'scene graph' query you can use in finder

            //Add some objects to the scene graph...
            _gameObjects.Add(new PlayerGameObject { Name = "Player 1", Health = 100 });
            for (var i = 1; i <= 5; i++)
                _gameObjects.Add(new EnemyGameObject { Name = "Enemy " + i, Damage = i * 10});

            //Set the Finder function to query the _gameObjects collection
            GS.GearsetComponent.Console.SetFinderSearchFunction(queryString =>
            {
                var result = new FinderResult();
                var searchParams = queryString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Split the query
                if (searchParams.Length == 0)
                {
                    searchParams = new[] { String.Empty };
                }
                else
                {
                    // Ignore case.
                    for (var i = 0; i < searchParams.Length; i++)
                        searchParams[i] = searchParams[i].ToUpper();
                }

                foreach (var component in _gameObjects)
                {
                    var matches = true;
                    var type = component.GetType().ToString();

                    // Check if it matches all search params.
                    foreach (var t in searchParams)
                    {
                        if (component.ToString().ToUpper().Contains(t) || type.ToUpper().Contains(t))
                            continue;

                        matches = false;
                        break;
                    }

                    if (matches)
                        result.Add(new ObjectDescription(component, type));
                }
                return result;
            });
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            const string mark = "Update";

            try
            {
                base.Update(gameTime);

                //We must call StartFrame at the top of Update to indicate to the Profiler that a new frame has started.
                GS.StartFrame();
                GS.BeginMark(mark, FlatTheme.PeterRiver);

                _previousKeyboardState = _keyboardState;
                _keyboardState = Keyboard.GetState();

                // Allows the game to exit
				#if WINDOWS
	                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
	                {
	                    Exit();
	                    return;
	                }
				#endif

                if (_keyboardState.IsKeyUp(Keys.Escape) && _previousKeyboardState.IsKeyDown(Keys.Escape))
                {
                    Exit();
                    return;
                }

                #if USE_GEARSET
                    //Test for CPU / GPU bound
                    if (GS.GearsetComponent.Console.Profiler.DoUpdate() == false)
                    {
                        return;
                    }
                #endif

                //PLOT test
                GS.Plot("FPS", _fpsCounter.Fps);
//                GS.Plot("Tick Memory K", _memoryMonitor.TickMemoryK);

                var mouseState = Mouse.GetState();
                var mousePos2 = new Vector2(mouseState.X, mouseState.Y);
                var mousePos3 = new Vector3(mousePos2, 0);

                //Label test
                GS.ShowLabel("I follow the mouse pointer!", mousePos2);

                //Line Test
                //Draw a line but then use the same key to reference it a second time and alter the postion / color
//                GS.ShowLine("TestLine", new Vector2(mouseState.X, mouseState.Y + 20), new Vector2(mouseState.X + 200, mouseState.Y + 20), Color.Green);
//                GS.ShowLine("TestLine", new Vector2(mouseState.X, mouseState.Y - 20), new Vector2(mouseState.X + 200, mouseState.Y - 20), Color.Violet);
                //Other lines...
                GS.ShowLineOnce(new Vector2(mouseState.X, mouseState.Y + 25), new Vector2(mouseState.X + 200, mouseState.Y + 25), Color.Pink);
                GS.ShowLineOnce(new Vector2(mouseState.X, mouseState.Y + 35), new Vector2(mouseState.X + 200, mouseState.Y + 35), Color.Red);

                //ALERT test - press SPACE for an alert message!
                if (_keyboardState.IsKeyUp(Keys.Space) && _previousKeyboardState.IsKeyDown(Keys.Space))
                    GS.Alert("I am an alert message");

                Thread.Sleep(1);//Let's trick the update into taking some time so that we can see some profile info

                //Update Gearset matrixes for 3d geometry
                GS.SetMatrices(ref _worldMatrix, ref _viewMatrix, ref _projectionMatrix);

                //Geometry tests...
//                GS.ShowSphere("TestSphere", mousePos3, 50, Color.Azure);
                GS.ShowSphereOnce(mousePos3, 50, Color.Azure);
//                GS.ShowBox("TestBox", new Vector3(mouseState.X + 50, mouseState.Y + 50, 0), new Vector3(mouseState.X + 100, mouseState.Y + 100, 0), Color.Blue);
                GS.ShowBoxOnce(new Vector3(mouseState.X + 100, mouseState.Y + 100, 0), new Vector3(mouseState.X + 150, mouseState.Y + 150, 0), Color.Red);
            }
            finally
            {
                //Must call EndMark
                GS.EndMark(mark);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(48,48,48,255));
            
            GS.BeginMark("Draw", FlatTheme.Pomegrantate);

            GS.BeginMark("Draw Background", FlatTheme.Pumpkin);
            Thread.Sleep(2); //Let's trick the update into taking some time so that we can see some profile info
            GS.EndMark("Draw Background");

            //Test nesting
            GS.BeginMark("Draw Sprites", FlatTheme.Sunflower);
            Thread.Sleep(1); //Let's trick the update into taking some time so that we can see some profile info
            GS.BeginMark("Draw Sprites", FlatTheme.Sunflower);
            Thread.Sleep(1); //Let's trick the update into taking some time so that we can see some profile info
            GS.EndMark("Draw Sprites");
            GS.EndMark("Draw Sprites");

			GS.BeginMark("Draw Particles", FlatTheme.Amethyst);
            Thread.Sleep(2); //Let's trick the update into taking some time so that we can see some profile info
            GS.EndMark("Draw Particles");

            GS.BeginMark("base.Draw", FlatTheme.Nephritis);
            base.Draw(gameTime);
            GS.EndMark("base.Draw");

            GS.EndMark("Draw");
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            GS.Shutdown(this);
        }
    }
}

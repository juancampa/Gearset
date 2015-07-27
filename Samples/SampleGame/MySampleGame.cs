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
        MemoryMonitor _memoryMonitor;

        KeyboardState _keyboardState;
        KeyboardState _previousKeyboardState;

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
            GS.Initialize(this);
            _fpsCounter = new FpsCounter(this);
            _memoryMonitor = new MemoryMonitor(this);
            Components.Add(_fpsCounter);
            Components.Add(_memoryMonitor);
            IsMouseVisible = true;

            base.Initialize();

            GS.Action(()=>GS.GearsetComponent.Console.Inspect("Profiler", new ProfilerInpectorSettings(GS.GearsetComponent.Console.Profiler)));

            GS.Log("I am a log message - tra la la");

            //Finder!
            //You can double click finder result items to add them to the Inspector window!
            //Comment this to use the default search query for finder (by default it's GameComponent based).
            GS.Action(ConfigureFinder);
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

                //NOTE: You may need to set Gerset Matrices if you have custom World, View or Projection matrices.
                //GS.SetMatrices(ref World, ref View, ref Projection);

                //PLOT test
                GS.Plot("FPS", _fpsCounter.Fps);
                GS.Plot("Total Memory K", _memoryMonitor.TotalMemoryK, 240);
                GS.Plot("Tick Memory K", _memoryMonitor.TickMemoryK);

                //Label test
                var mouseState = Mouse.GetState();
                GS.ShowLabel("I follow the mouse pointer!", new Vector2(mouseState.X, mouseState.Y));
                
                //ALERT test - press SPACE for an alert message!
                if (_keyboardState.IsKeyUp(Keys.Space) && _previousKeyboardState.IsKeyDown(Keys.Space))
                    GS.Alert("I am an alert message");

                Thread.Sleep(1);//Let's trick the update into taking some time so that we can see some profile info

                base.Update(gameTime);
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
    }
}

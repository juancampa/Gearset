//#undef USE_GEARSET
using System.Threading;
using Gearset;
using Gearset.Components.Profiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProfilerTestGame.Gearset;

namespace ProfilerTestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        //Gearset
        FpsCounter _fpsCounter;
        MemoryMonitor _memoryMonitor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            GS.Initialize(this);
            _fpsCounter = new FpsCounter(this);
            _memoryMonitor = new MemoryMonitor(this);
            Components.Add(_fpsCounter);
            Components.Add(_memoryMonitor);

            base.Initialize();

            GS.Action(()=>GS.GearsetComponent.Console.Inspect("Profiler", new ProfilerInpectorSettings(GS.GearsetComponent.Console.Profiler)));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //We must call StartFrame at the top of Update to indicate to the TimeRuler that a new frame has started.
            GS.StartFrame();
            GS.BeginMark("Update", FlatTheme.PeterRiver);

            //Some nice plots...
            //GS.Plot("FPS", _fpsCounter.Fps);
            //GS.Plot("Total Memory K", _memoryMonitor.TotalMemoryK, 240);
            //GS.Plot("Tick Memory K", _memoryMonitor.TickMemoryK);


            #if USE_GEARSET
                //Test for CPU / GPU bound
                if (GS.GearsetComponent.Console.Profiler.DoUpdate() == false)
                {
                    GS.EndMark("Update");
                    return;
                }   
            #endif

            Thread.Sleep(1);//Let's trick the update into taking some time so that we can see some profile info
            
            base.Update(gameTime);
            GS.EndMark("Update");
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            GS.BeginMark("Draw", FlatTheme.Pomegrantate);

            GS.BeginMark("Draw Background", FlatTheme.Pumpkin);
            Thread.Sleep(3); //Let's trick the update into taking some time so that we can see some profile info
            GS.EndMark("Draw Background");

            //Test nesting
            GS.BeginMark("Draw Sprites", FlatTheme.Sunflower);
            Thread.Sleep(2); //Let's trick the update into taking some time so that we can see some profile info
            GS.BeginMark("Draw Sprites", FlatTheme.Sunflower);
            Thread.Sleep(2); //Let's trick the update into taking some time so that we can see some profile info
            GS.EndMark("Draw Sprites");
            GS.EndMark("Draw Sprites");

            GS.BeginMark("Draw Particles", FlatTheme.Sunflower);
            Thread.Sleep(2); //Let's trick the update into taking some time so that we can see some profile info
            GS.EndMark("Draw Particles");

            GS.BeginMark("base.Draw", FlatTheme.Nephritis);
            base.Draw(gameTime);
            GS.EndMark("base.Draw");

            GS.EndMark("Draw");
        }
    }
}

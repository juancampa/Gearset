using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Gearset;

namespace ReleaseTest
{
    public static class Debug
    {
        public static GearConsole GS { get; set; }
    }

    

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ReleaseTestGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private class Heroe
        {
            public String hola;
            public String chao
            {
                get;
                set;
            }
        }

        List<Tester> testers = new List<Tester>();

        public static Matrix World;
        public static Matrix View;
        public static Matrix Projection;

        public ReleaseTestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            World = Matrix.Identity;
            View = Matrix.CreateLookAt(new Vector3(0, 5, -5), Vector3.Zero, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight, 0.01f, 100f);

            Debug.GS = new GearConsole(this);
            Debug.GS.Initialize();
            IsMouseVisible = true;

            Debug.GS.Inspect("A hero", new Heroe());

            testers.Add(new TesterLabels(this));

            foreach (Tester tester in testers)
            {
                tester.Enabled = tester.Visible = false;
                Components.Add(tester);
            }

            testers[0].Enabled = testers[0].Visible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

            // TODO: Add your update logic here

            

            base.Update(gameTime);

            // ...
            Debug.GS.SetMatrices(ref World, ref View, ref Projection);
            Debug.GS.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            // ...
            Debug.GS.Draw(gameTime);
        }
    }
}

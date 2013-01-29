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
using Complot.Dronengine;
using Complot.Dronengine.Graphics;
using Complot.Dronengine.Debug;
using Complot.Dronengine.Input;
using System.Collections;
using Gearset;

namespace GearsetTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private Curve SomeShittyCurve;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 655;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        public class TextureLibrary : List<Texture2D>
        {
        }

        private List<String> CollectionTest = new List<string>();
        private Random random;
        private float elapsed;

        public double Plottable;

        private TextureLibrary a;
        public float uiRotation;

        public InspectorTestCases InspectorTestCases = new InspectorTestCases();

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            random = new Random();
            CpuParticleSystem.DefaultRenderPassBits = (int)RenderPassFlags.Particles;
            Sprite.DefaultRenderPassBits = (int)RenderPassFlags.UI;

            Engine.Initialize();
            
            Screen.Manager.Push(new MainScreen());

            a = new TextureLibrary();
            XdtkWrapper.Inspect("Texture Library", a);

            var list = a;
            for (int i = 0; i < 100; i++)
            {
                if ((i%10) == 0) continue;
                if (i < 100)
                {
                    String name = "Textures\\" + (i % 10);
                    Texture2D texture = Content.Load<Texture2D>(name);
                    texture.Name = name;
                    list.Add(texture);
                }
                else
                {
                    //list.Add("SomeText " + random.Next(100));
                }
            }

            Components.Add(new Compo(this));
            Components.Add(new ICompo());

            //XdtkWrapper.Console.SetFinderSearchFunction(SearchFunction);
            //a = list;
            //XdtkWrapper.Console.ShowBox("The 1x1 box", Vector3.One * -.5f, Vector3.One * .5f, Color.Yellow);
            //XdtkWrapper.Console.ShowSphere("The 1x1 sphere", Vector3.One, .5f, Color.Yellow);
            //XdtkWrapper.Console.ShowVector3("The 1x1 vector", Vector3.One, Vector3.One, Color.Red);

            //XdtkWrapper.AddAction("Reset values", () => { });
            //XdtkWrapper.AddAction("Make remote request", () => { });
            //XdtkWrapper.AddAction("Validate positions now", () => { });

            //XdtkWrapper.Show("Once1", "Value");
            //XdtkWrapper.Show("Once2", "Value");
            //XdtkWrapper.Show("Once2.SomethingIn1", "Value1");
            //XdtkWrapper.Show("Once2.SomethingIn2", "Value1");
            //XdtkWrapper.Show("Once2.SomethingIn2.AnotherLevel", "Vaasasdlue");

            //for (int i = 0; i < 10000; i++)
            //{
            //    Console.WriteLine("Product key is:" + new Hid(id1) + new Hid(id2) + new Hid(id3));
            //}

            base.Initialize();
        }

        private FinderResult SearchFunction(String queryString)
        {
            FinderResult list = new FinderResult();
            foreach (var item in a)
            {
                if (item.Name.Contains(queryString) || item.GetType().Name.Contains(queryString))
                {
                    list.Add(new ObjectDescription(item, item.GetType().Name));
                }
            }

            return list;
        }

        // Rotates the string the given amount keeping it alphanumeric.
        // alphanum count: 10 + 26 + 26 = 62
        public String RotateString(String str)
        {
            String result = String.Empty;
            int rot = 17;
            for (int i = 0; i < str.Length; i++)
            {
                int c = (str[i] - 48 + rot);
                rot += str[i];
                c = c % 62 + 48;
                if (c > 57)
                    c += 7;
                if (c > 90)
                    c += 6;
                result += (char)c;
            }
            return result;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsed += dt;

            Engine.DebugManager.TimeRuler.StartFrame();
            Engine.Update(gameTime);

            if (InputManager.Keyboard.IsKeyJustDown(Keys.P))
            {
                Screen.Manager.Push(new MainScreen());
            }

            Plottable = Math.Sin(elapsed) + random.NextDouble() * 0.2 + (random.NextDouble() < 0.08 ? random.NextDouble() * 10 - 5: 0);

            //XdtkWrapper.ShowMark("Vortex", Vector2.Zero, Color.OrangeRed);
            Matrix rotating = Matrix.CreateRotationX(elapsed) * Matrix.CreateTranslation(Vector3.One);

            XdtkWrapper.Transform2D = Matrix.CreateScale(-1, 1, 1) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateTranslation(new Vector3(Engine.BackBufferSize * new Vector2(.5f, 1f), 0));

            //XdtkWrapper.Console.ShowVector3("The test vector", -Vector3.UnitX * 3, a.TestVector3, Color.White);
            //XdtkWrapper.Console.ShowVector2("The test vector", this.GetBackBufferSize() * .5f, a.TestVector2, Color.Red);
            //XdtkWrapper.Console.Transform3Drawer.ShowTransform("Rotating", rotating);
            //XdtkWrapper.Console.ShowSphere("esfera", rotating.Up, 1, Color.Yellow);
            //XdtkWrapper.Console.ShowSphere("esfera2", rotating.Up , 1.1f, Color.Orange);
            //XdtkWrapper.Console.ShowSphere("esfera3", rotating.Up , 1.2f, Color.Purple);
            //XdtkWrapper.Console.ShowBox("caja", new BoundingBox(Vector3.One, Vector3.One * 2), Color.LightPink);

            Vector3 t = new Vector3(2, 0, 0);

            for (int i = 0; i < 100; i++)
            {
                XdtkWrapper.ShowVector2(random.Next().ToString(), Vector2.Zero, Vector2.One * 100, Color.Blue);
            }
            XdtkWrapper.ShowLine("Hola", new Vector2(0), new Vector2(100), Color.Red);
            XdtkWrapper.ShowVector2("Chao", Vector2.Zero, new Vector2(45, 85), Color.Blue);
            //XdtkWrapper.Console.ShowVector3Once(Vector3.Zero + t, Vector3.One + t, Color.Red);
            //XdtkWrapper.Console.Transform3Drawer.ShowTransformOnce(rotating);
            XdtkWrapper.ShowSphereOnce(rotating.Up + t, 1f, Color.Yellow);
            //XdtkWrapper.Console.ShowSphereOnce(rotating.Up + t, 1.1f, Color.Orange);
            //XdtkWrapper.Console.ShowSphereOnce(rotating.Up + t, 1.2f, Color.Purple);
            //XdtkWrapper.Console.ShowBoxOnce(new BoundingBox(Vector3.One + t, Vector3.One * 2 + t), Color.LightPink);

            //XdtkWrapper.Console.Vector2Drawer.ShowVector2("Mouse", InputManager.Mouse.Position, InputManager.Mouse.PreviousPosition, Color.Red);
            //XdtkWrapper.Show("EveryFrame1", "Value");
            //XdtkWrapper.Show("EveryFrame2", "Value");
            //XdtkWrapper.Show("EveryFrame2.SomethingIn1", "ValueMid1");
            //XdtkWrapper.Show("EveryFrame2.SomethingIn2", "ValueMid2");
            //XdtkWrapper.Show("EveryFrame2.SomethingIn2.DOWWWN", "ValueDown");

            //XdtkWrapper.Console.Labeler.ShowLabel("LabelTest", -Vector3.UnitX * 3 + a.TestVector3, a.TestVector3.Length().ToString());
            float v = elapsed * 6.1f;
            XdtkWrapper.Plot("Math.Sin", (float)Math.Sin(v) * 0.4f + 0.4f);
            //XdtkWrapper.Plot("elapsedTime", (float)-v);
            
            //XdtkWrapper.Plot("Math.Tan", (float)Math.Tan(v), 300);
            //XdtkWrapper.Plot("Math.Cos2", (float)Math.Cos(v), 100);
            //XdtkWrapper.Plot("Drop Chance", InputManager.Mouse.Position.X, 200);
            //XdtkWrapper.Plot("Mouse.Y", InputManager.Mouse.Position.Y, 120);
            //XdtkWrapper.Plot("Vortex Intensity", (float)random.NextDouble() * .2f + .45f, 200);

            //if (DotMath.PulsesBetween(elapsed - dt, elapsed, 1) > 0)
            //{
            //    XdtkWrapper.Alert("Warning");
            //}

            int toss = random.Next(40) + 0;
            switch (toss)
            {
                case 0:
                    XdtkWrapper.Log("Character", String.Format("User input (X:{0}, Y:{1})", random.NextFloat() * 100, random.NextFloat() * -40));
                    break;
                case 1:
                    XdtkWrapper.Log("Simulation", String.Format("Stepping simulation to keep up. {0} -> {1}!", random.Next(100), random.Next(100)));
                    XdtkWrapper.Log("Simulation", String.Format("Stepping simulation to keep up. {0} -> {1}!", random.Next(100), random.Next(100)));
                    XdtkWrapper.Log("Simulation", String.Format("Stepping simulation to keep up. {0} -> {1}!", random.Next(100), random.Next(100)));
                    XdtkWrapper.Log("Simulation", String.Format("Stepping simulation to keep up. {0} -> {1}!", random.Next(100), random.Next(100)));
                    XdtkWrapper.Log("Simulation", String.Format("Stepping simulation to keep up. {0} -> {1}! HALT!", random.Next(100), random.Next(100)));
                    XdtkWrapper.Log("Character", String.Format("User input X:{0}, Y:{1}", random.NextFloat() * 100, random.NextFloat() * -40));
                    break;
                case 2:
                    XdtkWrapper.Log("Character", String.Format("User breaked from jail. Reducing speed."));
                    XdtkWrapper.Log("Game states", String.Format("Changing states {0} -> {1}", random.Next(10), random.Next(10)));
                    break;
                case 3:
                    XdtkWrapper.Log("AI Manager", String.Format("Character didn't react. This shoudn't happen, check speed clamping, a case is missing."));
                    break;
                case 4:
                    XdtkWrapper.Log("Physics controller", String.Format("Increasing Y velocity, no z speed was found."));
                    break;
                case 5:
                    XdtkWrapper.Log("Camera", String.Format("Colliding with wall, FAIL."));
                    XdtkWrapper.Log("Camera", String.Format("Correcting position:"));
                    XdtkWrapper.Log("Camera", String.Format("Nothing good found, position: (X:{0},Y:{1},Z:{2}). Saving log to file.", random.NextFloat() * 4, random.NextFloat() * 40, random.NextFloat() * 4));
                    break;
                default:
                    break;
            }
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Engine.Draw(gameTime);
            base.Draw(gameTime);
        }
    }

    class Compo : GameComponent
    {
        public Compo(Game game)
            : base(game)
        {

        }
    }

    class ICompo : IGameComponent
    {
        public void Initialize()
        {
        }
    }

    public class InspectorTestCases
    {
        public float PublicProperty { get; set; }
        public float PublicGetPrivateSet { get; private set; }
        public float PrivateGetPublicSet { private get; set; }
        private float PrivateProperty { get; set; }
    }
}

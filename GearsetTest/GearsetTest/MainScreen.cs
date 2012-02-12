using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Complot.Dronengine;
using Complot.Dronengine.Graphics;
using Complot.Dronengine.Scene;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Complot.Dronengine.Debug;
using Complot.Dronengine.Graphics._3D.Particles.SpaceWarps;
using Complot.Dronengine.Collision;

namespace GearsetTest
{
    [Flags]
    public enum RenderPassFlags
    {
        Particles = (1 << 0),
        UI =        (1 << 1),
    }

    class VortexComponent : GameComponent
    {

        public VortexComponent(Game game)
            : base(game)
        {

        }
        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            SpaceWarp.Enabled = Enabled;
        }
        public VortexSpaceWarp SpaceWarp;
        public SphereCollider CoreCollider;
    }

    class ParticlesComponent : GameComponent
    {
        public ParticlesComponent(Game game)
            : base(game)
        {

        }
        public ParticleSettings Settings;
        public ParticleSystem System;
        public BasicEffect BasicEffect;
        public List<DirectionalEmitter> Emitters = new List<DirectionalEmitter>();

        public Range<float> Velocity;
        public Range<float> Spread;

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Emitters.Count; i++)
            {
                Emitters[i].Velocity = Velocity;
                Emitters[i].Spread = Spread;
            }
        }
    }

    public class Sprite2 : Sprite, IShowable
    {
        public Curve XPosition { get; private set; }
        public Curve YPosition { get; private set; }

        public bool TestMode { get; set; }


        public Sprite2(Entity parent, String name)
            : base(parent, name)
        {
            XPosition = new Curve();
            YPosition = new Curve();

            XdtkWrapper.AddCurve("Sprite2.X", XPosition);
            XdtkWrapper.AddCurve("Sprite2.Y", YPosition);

            TestMode = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (TestMode)
            {
                float t = XdtkWrapper.BenderNeedlePosition;
                Position = new Vector2(XPosition.Evaluate(t), YPosition.Evaluate(t));
            }
            base.Update(gameTime);
        }
    }

    public class MainScreen : Screen
    {
        public MainScreen()
            : base("MainScreen", ScreenUnloadPolicy.NeverUnload)
        {
        }

        protected override void OnLoad()
        {
            PerspectiveCamera camera = new PerspectiveCamera(this, "Camera", MathHelper.PiOver2);
            camera.Transform.Position.Z = 20f;
            camera.Transform.Position.Y = 16f;
            //camera.LookAt(Vector3.Zero);

            new KeyboardCameraController(camera, "KeyboardController");

            Camera.Manager.ActiveCamera = camera;

            // Vortex
            VortexSpaceWarp vortex = new VortexSpaceWarp(this, "Vortex");
            vortex.AxialPulldown = 0;
            vortex.Intensity = 0;
            collider = new SphereCollider(new Dummy3D(this, "Dummy3D"));

            VortexComponent v = new VortexComponent(Engine.Game);
            Engine.Game.Components.Add(v);
            v.SpaceWarp = vortex;
            v.CoreCollider = collider;

            ParticlesComponent particles = new ParticlesComponent(Engine.Game);
            Engine.Game.Components.Add(particles);

            ParticleSettings settings = new ParticleSettings();
            settings.StretchWithSpeed = true;
            settings.Size.Begin.Min = 0.06f;
            settings.Size.Begin.Max = 0.08f;
            settings.Size.End.Min = 0.03f;
            settings.Size.End.Max = 0.03f;
            settings.MaxParticles = 10000;
            settings.Damping = 0.98f;
            settings.StretchWithSpeed = false;
            //settings.Color.Begin = new Range<Color>(Color.Red, Color.Orange);
            //settings.Color.End = new Range<Color>(Color.Yellow, Color.White);
            settings.Color.Begin = new Range<Color>(Color.White, Color.White);
            settings.Color.End = new Range<Color>(Color.White, Color.White);
            settings.StretchWithSpeed = false;

            Sprite2 cursor = new Sprite2(this, "cursor");
            XdtkWrapper.Inspect("Cursor", cursor);

            BasicEffect effect = new BasicEffect(Engine.Game.GraphicsDevice);
            effect.TextureEnabled = false;
            effect.DiffuseColor = Vector3.One;
            effect.VertexColorEnabled = true;
            effect.EmissiveColor = new Vector3(0f, 0f, 0);
            effect.DiffuseColor = Vector3.One;

            CpuParticleSystem particleSystem = new CpuParticleSystem(this, "ParticleSystem", settings);
            particleSystem.Effect = effect;
            
            particles.Settings = settings;
            particles.BasicEffect = effect;
            particles.System = particleSystem;

            int count = 13;
            for (int i = 0; i < count; i++)
            {
                ////DotMath.Remap(i, 0, count / 3f, 0, 1f), DotMath.Remap(i, count / 3f, count * 2 / 3f, 0, 1), DotMath.Remap(i, count * 2 / 3f, count, 0, 1f));
                //Vector3 gradient1 = new Vector3(1.0f, 0.2f, 0.2f);
                //Vector3 gradient2 = new Vector3(0.2f, 0.8f, 0.2f);
                //Vector3 gradient3 = new Vector3(0.2f, 0.2f, 0.8f);
                //float normali = i/(float)count;
                //effect.EmissiveColor = normali < 0.5f ? Vector3.Lerp(gradient1, gradient2, Easing.EaseIn(normali * 2f, EasingType.Quadratic)) : Vector3.Lerp(gradient2, gradient3, (normali - 0.5f) * 2);
                
                

                // Emitter position
                float off = MathHelper.PiOver2;
                Vector3 position = new Vector3((float)Math.Sin(i / (float)count * MathHelper.TwoPi) * 5, 0, (float)Math.Cos(i / (float)count * MathHelper.TwoPi) * 5);
                Vector3 velocity = new Vector3((float)Math.Sin(i / (float)count * MathHelper.TwoPi + off), 0, (float)Math.Cos(i / (float)count * MathHelper.TwoPi + off));
                DirectionalEmitter emitter = new DirectionalEmitter(particleSystem, "Emitter" + i, particleSystem, 40);
                emitter.Position = position;
                emitter.Direction = velocity;
                emitter.Velocity = new Range<float>(0, 1);
                emitter.Spread = new Range<float>(0, 0);

                particles.Emitters.Add(emitter);
            
                // Add to system.
                particleSystem.AddModifier(vortex);
                particleSystem.AddCollider(collider);
            }

            base.OnLoad();
            param = new VortexParameters();
            //XdtkWrapper.Inspect("Vortex Params", param);
        }

        private VortexParameters param;
        private SphereCollider collider;

        public class VortexParameters
        {
            public Range<float> Spread = new Range<float>();
            public Range<float> Velocity = new Range<float>();
            public Texture2D Texture;
        }

        public override void Update(GameTime gameTime)
        {
            //XdtkWrapper.Show("Camera.Position", Camera.Manager.ActiveCamera.Position);
            XdtkWrapper.ShowSphereOnce(collider.BoundingSphere, Color.Orange);

            //foreach (var system in ChildrenByType<CpuParticleSystem>())
            //{
            //    BasicEffect effect = system.Effect as BasicEffect;
            //    if (effect.Texture != param.Texture)
            //    {
            //        effect.Texture = param.Texture;
            //        effect.TextureEnabled = true;
            //    }
            //    foreach (var item in system.ChildrenByType<DirectionalEmitter>())
            //    {
            //        item.Spread = param.Spread;
            //        item.Velocity = param.Velocity;
            //    }
            //}
            
            base.Update(gameTime);
        }
        protected override void Initialize(out ISceneManager2D scene2D, out ISceneManager3D scene3D, out Complot.Dronengine.Graphics.Renderer renderer)
        {
            scene2D = new DummyScene2D();
            scene3D = new DummyScene3D();
            renderer = new Renderer(scene2D, scene3D);

            ClearColorPass clear = new ClearColorPass();
            clear.ClearColor = new Vector4(200, 200, 60, 255);

            Scene3DTraversalRenderPass particles = new Scene3DTraversalRenderPass((int)RenderPassFlags.Particles, SceneTraversalMode.BackToFront)
            {
                BlendState = BlendState.Additive,
                DepthStencilState = DepthStencilState.DepthRead,
            };

            WireframePass wireframe = new WireframePass(0x0FFFFFFF, scene3D, SceneTraversalMode.BackToFront)
            {
                Enabled = false,
            };

            SpriteBatchRenderPass spriteBatchRenderPass = new SpriteBatchRenderPass((int)RenderPassFlags.UI, SceneTraversalMode.BackToFront);

            renderer.AddPass(clear);
            renderer.AddPass(particles);
            renderer.AddPass(wireframe);
            renderer.AddPass(spriteBatchRenderPass);
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using DotEngine;
//using DotEngine.Graphics;
//using DotEngine.Scene;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;
//using DotEngine.Debug;
//using DotEngine.Graphics._3D.Particles.SpaceWarps;
//using DotEngine.Collision;

//namespace GearsetTest
//{
//    [Flags]
//    public enum RenderPassFlags
//    {
//        Particles = 0x1,
//    }

//    class VortexComponent : GameComponent
//    {
//        protected override void OnEnabledChanged(object sender, EventArgs args)
//        {
//            SpaceWarp.Enabled = Enabled;
//        }
//        public VortexSpaceWarp SpaceWarp;
//        public SphereCollider CoreCollider;
//    }

//    class ParticlesComponent : GameComponent
//    {
//        public ParticleSettings Settings;
//        public ParticleSystem System;
//        public BasicEffect Effect;
//        public List<DirectionalEmitter> Emitters;
//    }

//    public class MainScreen : Screen
//    {
//        public MainScreen()
//            : base("MainScreen", ScreenUnloadPolicy.NeverUnload)
//        {
//        }

//        protected override void OnLoad()
//        {
//            PerspectiveCamera camera = new PerspectiveCamera(this, "Camera", MathHelper.PiOver2);
//            camera.Transform.Position.Z = 10f;
//            new KeyboardCameraController(camera, "KeyboardController");

//            Camera.Manager.ActiveCamera = camera;

//            // Vortex
//            VortexSpaceWarp vortex = new VortexSpaceWarp(this, "Vortex");
//            collider = new SphereCollider(new Dummy3D(this, "Dummy3D"));

//            VortexComponent v = new VortexComponent();
//            v.SpaceWarp = vortex;
//            v.CoreCollider = collider;
//            Engine.Game.Components.Add(v);

//            ParticlesComponent particles = new ParticlesComponent();

//            ParticleSettings settings = new ParticleSettings();
//            settings.StretchWithSpeed = true;
//            settings.Size.Begin.Min = 0.06f;
//            settings.Size.Begin.Max = 0.08f;
//            settings.Size.End.Min = 0.03f;
//            settings.Size.End.Max = 0.03f;
//            settings.MaxParticles = 10000;
//            settings.Damping = 0.98f;

//            particles.Settings = settings;
            
//            int count = 13;
//            for (int i = 0; i < count; i++)
//            {
//                BasicEffect effect = new BasicEffect(Engine.Game.GraphicsDevice);
//                effect.TextureEnabled = false;
//                effect.DiffuseColor = Vector3.One;
//                effect.VertexColorEnabled = false;
//                effect.EmissiveColor = Vector3.One;

//                effect.DiffuseColor = Vector3.Zero;

//                //DotMath.Remap(i, 0, count / 3f, 0, 1f), DotMath.Remap(i, count / 3f, count * 2 / 3f, 0, 1), DotMath.Remap(i, count * 2 / 3f, count, 0, 1f));
//                Vector3 gradient1 = new Vector3(1.0f, 0.2f, 0.2f);
//                Vector3 gradient2 = new Vector3(0.2f, 0.8f, 0.2f);
//                Vector3 gradient3 = new Vector3(0.2f, 0.2f, 0.8f);
//                float normali = i/(float)count;
//                effect.EmissiveColor = normali < 0.5f ? Vector3.Lerp(gradient1, gradient2, Easing.EaseIn(normali * 2f, EasingType.Quadratic)) : Vector3.Lerp(gradient2, gradient3, (normali - 0.5f) * 2);
                
//                CpuParticleSystem particleSystem = new CpuParticleSystem(this, "ParticleSystem" + i, settings);
//                particleSystem.Effect = effect;

//                // Emitter position
//                float off = MathHelper.PiOver2;
//                Vector3 position = new Vector3((float)Math.Sin(i / (float)count * MathHelper.TwoPi) * 5, 0, (float)Math.Cos(i / (float)count * MathHelper.TwoPi) * 5);
//                Vector3 velocity = new Vector3((float)Math.Sin(i / (float)count * MathHelper.TwoPi + off), 0, (float)Math.Cos(i / (float)count * MathHelper.TwoPi + off));
//                DirectionalEmitter emitter = new DirectionalEmitter(particleSystem, "Emitter" + i, particleSystem, 40);
//                emitter.Position = position;
//                emitter.Direction = velocity;
//                emitter.Velocity = new Range<float>(0, 1);
//                emitter.Spread = new Range<float>(0, 0);
            
//                // Add to system.
//                particleSystem.AddModifier(vortex);
//                particleSystem.AddCollider(collider);
//            }

//            base.OnLoad();
//            param = new VortexParameters();
//            XdtkWrapper.Inspect("Vortex Params", param);
//        }

//        private VortexParameters param;
//        private SphereCollider collider;

//        public class VortexParameters
//        {
//            public Range<float> Spread = new Range<float>();
//            public Range<float> Velocity = new Range<float>();
//            public Texture2D Texture;
//        }

//        public override void Update(GameTime gameTime)
//        {
//            XdtkWrapper.Show("Camera.Position", Camera.Manager.ActiveCamera.Position);
//            XdtkWrapper.ShowSphereOnce(collider.BoundingSphere, Color.Orange);

//            foreach (var system in ChildrenByType<CpuParticleSystem>())
//            {
//                BasicEffect effect = system.Effect as BasicEffect;
//                if (effect.Texture != param.Texture)
//                {
//                    effect.Texture = param.Texture;
//                    effect.TextureEnabled = true;
//                }
//                foreach (var item in system.ChildrenByType<DirectionalEmitter>())
//                {
//                    item.Spread = param.Spread;
//                    item.Velocity = param.Velocity;
//                }
//            }
            
//            base.Update(gameTime);
//        }
//        protected override void Initialize(out ISceneManager2D scene2D, out ISceneManager3D scene3D, out DotEngine.Graphics.Renderer renderer)
//        {
//            scene2D = new DummyScene2D();
//            scene3D = new DummyScene3D();
//            renderer = new Renderer(scene2D, scene3D);

//            ClearColorPass clear = new ClearColorPass();

//            Scene3DTraversalRenderPass particles = new Scene3DTraversalRenderPass((int)RenderPassFlags.Particles, SceneTraversalMode.BackToFront)
//            {
//                BlendState = BlendState.Additive,
//                DepthStencilState = DepthStencilState.DepthRead,
//            };

//            WireframePass wireframe = new WireframePass(0x0FFFFFFF, scene3D, SceneTraversalMode.BackToFront);

//            renderer.AddPass(clear);
//            renderer.AddPass(particles);
//            renderer.AddPass(wireframe);
//        }
//    }
//}

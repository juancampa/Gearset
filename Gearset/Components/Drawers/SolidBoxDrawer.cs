using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    internal class SolidBoxDrawer : Gear
    {
        private const int MaxBoxes = 500;
        int boxCount = 0;
        private VertexPositionColorTexture[] Vertices;
        public Texture2D noiseTexture;
        private SamplerState wrapSamplerState;
        //private bool inspected;

        public SolidBoxDrawer()
            : base (new GearConfig())
        {
            Vertices = new VertexPositionColorTexture[MaxBoxes * 6];

            // Generate a texture of gray noise.
            noiseTexture = new Texture2D(GearsetResources.Game.GraphicsDevice, 128, 128);
            Random random = new Random();
            int textureSize = 128 * 128;
            Color[] noise = new Color[textureSize];
            for (int i = 0; i < textureSize; i++)
            {
                byte shade = (byte)random.Next(100, 150);
                noise[i].R = shade;
                noise[i].G = shade;
                noise[i].B = shade;
                noise[i].A = 255;
            }
            noiseTexture.SetData<Color>(noise);

            wrapSamplerState = new SamplerState();
            wrapSamplerState.AddressU = TextureAddressMode.Wrap;
            wrapSamplerState.AddressV = TextureAddressMode.Wrap;
            wrapSamplerState.Filter = TextureFilter.Point;
        }

        public override void Update(GameTime gameTime)
        {
            //if (!inspected)
            //    GearsetResources.Console.Inspect("SOlidBoxDrawer", this);
            //inspected = true;

            boxCount = 0;
            base.Update(gameTime);
        }

        public void ShowBoxOnce(Vector2 min, Vector2 max)
        {
            if (boxCount >= MaxBoxes)
                return;

            Vector3 tl = new Vector3(min, 0);
            Vector3 tr = new Vector3(max.X, min.Y, 0);
            Vector3 br = new Vector3(max, 0);
            Vector3 bl = new Vector3(min.X, max.Y, 0);

            Color color = new Color(32, 32, 32, 127);

            int i = boxCount * 6;
            Vertices[i + 0] = new VertexPositionColorTexture(tl, color, new Vector2(0, 0));
            Vertices[i + 1] = new VertexPositionColorTexture(tr, color, new Vector2(1, 0));
            Vertices[i + 2] = new VertexPositionColorTexture(br, color, new Vector2(1, 1));
            Vertices[i + 3] = new VertexPositionColorTexture(tl, color, new Vector2(0, 0));
            Vertices[i + 4] = new VertexPositionColorTexture(br, color, new Vector2(1, 1));
            Vertices[i + 5] = new VertexPositionColorTexture(bl, color, new Vector2(0, 1));

            boxCount++;
        }

        public void ShowGradientBoxOnce(Vector2 min, Vector2 max, Color top, Color bottom)
        {
            if (boxCount >= MaxBoxes)
                return;

            Vector3 tl = new Vector3(min, 0);
            Vector3 tr = new Vector3(max.X, min.Y, 0);
            Vector3 br = new Vector3(max, 0);
            Vector3 bl = new Vector3(min.X, max.Y, 0);

            int i = boxCount * 6;

            Vector2 tiling = (max - min) * 1 / 128f;
            Vertices[i + 0] = new VertexPositionColorTexture(tl, top, new Vector2(0, 0));
            Vertices[i + 1] = new VertexPositionColorTexture(tr, top, new Vector2(tiling.X, 0));
            Vertices[i + 2] = new VertexPositionColorTexture(br, bottom, new Vector2(tiling.X, tiling.Y));
            Vertices[i + 3] = new VertexPositionColorTexture(tl, top, new Vector2(0, 0));
            Vertices[i + 4] = new VertexPositionColorTexture(br, bottom, new Vector2(tiling.X, tiling.Y));
            Vertices[i + 5] = new VertexPositionColorTexture(bl, bottom, new Vector2(0, tiling.Y));

            boxCount++;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GearsetResources.CurrentRenderPass == RenderPass.ScreenSpacePass && boxCount > 0)
            {
                GearsetResources.Effect2D.Texture = noiseTexture;
                GearsetResources.Effect2D.TextureEnabled = true;
                GearsetResources.Effect2D.CurrentTechnique.Passes[0].Apply();
                GearsetResources.Game.GraphicsDevice.SamplerStates[0] = wrapSamplerState;
                GearsetResources.Device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, Vertices, 0, boxCount * 2);
                GearsetResources.Effect2D.Texture = null;
                GearsetResources.Effect2D.TextureEnabled = false;
                GearsetResources.Effect2D.CurrentTechnique.Passes[0].Apply();

                boxCount = 0;
            }
        }
    }
}

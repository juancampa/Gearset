using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components.Profiler
{
    //TODO: Added this with a much bigger buffer and no noise texture - think we need a better solution going forward.
    internal class TempBoxDrawer : Gear
    {
        private const int MaxBoxes = 6000;
        private readonly VertexPositionColor[] Vertices;
        private int boxCount;
        public Texture2D noiseTexture;

        public TempBoxDrawer() : base (new GearConfig())
        {
            Vertices = new VertexPositionColor[MaxBoxes * 6];
        }

        public void ShowBoxOnce(Vector2 min, Vector2 max)
        {
            if (boxCount >= MaxBoxes)
                return;

            var tl = new Vector3(min, 0);
            var tr = new Vector3(max.X, min.Y, 0);
            var br = new Vector3(max, 0);
            var bl = new Vector3(min.X, max.Y, 0);

            var color = new Color(32, 32, 32, 127);

            var i = boxCount * 6;
            Vertices[i + 0] = new VertexPositionColor(tl, color);
            Vertices[i + 1] = new VertexPositionColor(tr, color);
            Vertices[i + 2] = new VertexPositionColor(br, color);
            Vertices[i + 3] = new VertexPositionColor(tl, color);
            Vertices[i + 4] = new VertexPositionColor(br, color);
            Vertices[i + 5] = new VertexPositionColor(bl, color);

            boxCount += 6;
        }

        public void ShowGradientBoxOnce(Vector2 min, Vector2 max, Color top, Color bottom)
        {
            if (boxCount >= MaxBoxes)
                return;

            var tl = new Vector3(min, 0);
            var tr = new Vector3(max.X, min.Y, 0);
            var br = new Vector3(max, 0);
            var bl = new Vector3(min.X, max.Y, 0);

            var i = boxCount * 6;

            Vertices[i + 0] = new VertexPositionColor(tl, top);;
            Vertices[i + 1] = new VertexPositionColor(tr, top);
            Vertices[i + 2] = new VertexPositionColor(br, bottom);
            Vertices[i + 3] = new VertexPositionColor(tl, top);
            Vertices[i + 4] = new VertexPositionColor(br, bottom);
            Vertices[i + 5] = new VertexPositionColor(bl, bottom);

            boxCount += 6;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (GearsetResources.CurrentRenderPass == RenderPass.ScreenSpacePass && boxCount > 0)
            {
                GearsetResources.Effect2D.Texture = null;
                GearsetResources.Effect2D.TextureEnabled = false;
                GearsetResources.Effect2D.Techniques[0].Passes[0].Apply();
                GearsetResources.Device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices, 0, boxCount * 2);

                boxCount = 0;
            }
        }
    }
}

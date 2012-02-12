#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Gearset
{
    class BoundingBoxHelper
    {
        private static BasicEffect effect;
        private static Random random = new Random();
        private static VertexPositionColor[] vertices;

        public static void Initialize()
        {
            effect = GearsetResources.Effect;
            vertices = new VertexPositionColor[24];
        }

        private static void InitializeVertices(BoundingBox box, Color color)
        {
            Vector3[] corners = box.GetCorners();
            
            
            vertices[0].Position = corners[0];
            vertices[1].Position = corners[1];
            vertices[2].Position = corners[1];
            vertices[3].Position = corners[2];
            vertices[4].Position = corners[2];
            vertices[5].Position = corners[3];
            vertices[6].Position = corners[3];
            vertices[7].Position = corners[0];

            vertices[8].Position = corners[4];
            vertices[9].Position = corners[5];
            vertices[10].Position = corners[5];
            vertices[11].Position = corners[6];
            vertices[12].Position = corners[6];
            vertices[13].Position = corners[7];
            vertices[14].Position = corners[7];
            vertices[15].Position = corners[4];

            vertices[16].Position = corners[4];
            vertices[17].Position = corners[0];
            vertices[18].Position = corners[5];
            vertices[19].Position = corners[1];
            vertices[20].Position = corners[6];
            vertices[21].Position = corners[2];
            vertices[22].Position = corners[7];
            vertices[23].Position = corners[3];

            for (int i=0; i<24 ; i++)
            {
                vertices[i].Color = color;
                
            }
        }

        #region Draw
        /// <summary>
        /// Draws White a specified Bounding Box.
        /// </summary>
        public static void DrawBoundingBox(BoundingBox box)
        {
            DrawBoundingBox(box, Color.White);
        }

        /// <summary>
        /// Draws with the specified color, the specified BBox.
        /// </summary>
        public static void DrawBoundingBox(BoundingBox box, Color color)
        {
            InitializeVertices(box, color);
            GearsetResources.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 12);
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{

    /// <summary>   
    /// Draw lines in 3D space, can draw line lists and line strips.
    /// </summary>
    public class InternalLineDrawer : Gear
    {
        /// <summary>
        /// Maps an id to an index in the dictionary.
        /// </summary>
        private Dictionary<String, int> persistentLine3DTable = new Dictionary<String, int>();

        /// <summary>
        /// Maps an id to an index in the dictionary.
        /// </summary>
        private Dictionary<String, int> persistentLine2DTable = new Dictionary<String, int>();

        private VertexPositionColor[] persistentVertices3D;
        private VertexPositionColor[] persistentVertices2D;
        private VertexPositionColor[] singleFrameVertices2D;
        private VertexPositionColor[] singleFrameVertices3D;

        /// <summary>
        /// Number of lines to be drawn on this frame. There
        /// will be twice as vertices in the singleFrameVertices array.
        /// </summary>
        private int singleFrameLine2DCount = 0;

        /// <summary>
        /// Number of lines to be drawn on this frame. There
        /// will be twice as vertices in the singleFrameVertices array.
        /// </summary>
        private int singleFrameLine3DCount = 0;

        /// <summary>
        /// Number of lines to be drawn on this frame. There
        /// will be twice as vertices in the singleFrameVertices array.
        /// </summary>
        private int persistentLine3DCount = 0;

        /// <summary>
        /// Number of lines to be drawn on this frame. There
        /// will be twice as vertices in the singleFrameVertices array.
        /// </summary>
        private int persistentLine2DCount = 0;

        /// <summary>
        /// Sets a maximun of lines we can draw.
        /// </summary>
        private const int MaxLineCount = 10000;   // Implies a 32k per buffer.

        /// <summary>
        /// When a persistent line is deleted it's index will be
        /// stored here so the next one can take it.
        /// </summary>
        private Queue<int> freeSpots3D;

        /// <summary>
        /// When a persistent line is deleted it's index will be
        /// stored here so the next one can take it.
        /// </summary>
        private Queue<int> freeSpots2D;

        /// <summary>
        /// Gets or sets the config for the Line Drawer.
        /// </summary>
        public virtual LineDrawerConfig Config { get { return config; } }

        /// <summary>
        /// Defines the way that coordinates will be interpreted in 2D space. Defaults to Screen space.
        /// </summary>
        public CoordinateSpace CoordinateSpace;

        private LineDrawerConfig config;

        #region Constructor
        public InternalLineDrawer()
            : this(new LineDrawerConfig())
        { }
        public InternalLineDrawer(LineDrawerConfig config)
            : base(config)
        {
            this.config = config;
            config.Cleared += new EventHandler(Config_Cleared);

            this.persistentVertices3D = new VertexPositionColor[MaxLineCount * 2];
            this.persistentVertices2D = new VertexPositionColor[MaxLineCount * 2];
            this.singleFrameVertices2D = new VertexPositionColor[MaxLineCount * 2];
            this.singleFrameVertices3D = new VertexPositionColor[MaxLineCount * 2];

            CoordinateSpace = Components.CoordinateSpace.ScreenSpace;

            freeSpots3D = new Queue<int>();
            freeSpots2D = new Queue<int>();
        }

        public void Clear()
        {
            for (int i = 0; i < persistentVertices3D.Length; i++)
            {
                persistentVertices3D[i].Position = Vector3.Zero;
                persistentVertices3D[i].Color = new Color(persistentVertices3D[i].Color.R, persistentVertices3D[i].Color.G, persistentVertices3D[i].Color.B, 1);
            }

            for (int i = 0; i < persistentVertices2D.Length; i++)
            {
                persistentVertices2D[i].Position = Vector3.Zero;
                persistentVertices2D[i].Color = new Color(persistentVertices2D[i].Color.R, persistentVertices2D[i].Color.G, persistentVertices2D[i].Color.B, 1);
            }
        }

        void Config_Cleared(object sender, EventArgs e)
        {
            Clear();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            // Make space for new frame data.
            singleFrameLine2DCount = 0;
            singleFrameLine3DCount = 0;

            base.Update(gameTime);
        }

        #region ShowLine (public methods)
        /// <summary>
        /// Draws a line and keeps drawing it, its values can be changed
        /// by calling ShowLine again with the same key. If you want to
        /// make more efficient subsequent calls, get the returned index (
        /// and call it again but with the index overload.
        /// </summary>
        public int ShowLine(String key, Vector3 v1, Vector3 v2, Color color)
        {
            int index = 0;

            // Do we already know this line?
            if (persistentLine3DTable.ContainsKey(key))
            {
                index = persistentLine3DTable[key];
                ShowLine(index, v1, v2, color);
            }
            else // We don't know this line, assign a new index to it.
            {
                // Can we take a spot left by other line?
                if (freeSpots3D.Count > 0)
                {
                    index = freeSpots3D.Dequeue();
                    persistentLine3DTable.Add(key, index);
                    ShowLine(index, v1, v2, color);
                }
                else if (persistentLine3DCount + 1 < MaxLineCount)
                {
                    index = (persistentLine3DCount++) * 2;
                    persistentLine3DTable.Add(key, index);
                    ShowLine(index, v1, v2, color);
                }
            }
            return index;
        }

        /// <summary>
        /// Draws a line and keeps drawing it, its values can be changed
        /// by calling ShowLine again with the same key. If you want to
        /// make more efficient subsequent calls, get the returned index (
        /// and call it again but with the index overload.
        /// </summary>
        public int ShowLine(String key, Vector2 v1, Vector2 v2, Color color)
        {
            int index = 0;
            // Do we already know this line?
            if (persistentLine2DTable.ContainsKey(key))
            {
                index = persistentLine2DTable[key];
                ShowLine(index, v1, v2, color);
            }
            else // We don't know this line, assign a new index to it.
            {
                // Can we take a spot left by other line?
                if (freeSpots2D.Count > 0)
                {
                    index = freeSpots2D.Dequeue();
                    persistentLine2DTable.Add(key, index);
                    ShowLine(index, v1, v2, color);
                }
                else if (persistentLine2DCount + 1 < MaxLineCount)
                {
                    index = (persistentLine2DCount++) * 2;
                    persistentLine2DTable.Add(key, index);
                    ShowLine(index, v1, v2, color);
                }
            }
            return index;
        }

        /// <summary>
        /// Use this method only once, then if you want to update the line
        /// use the (int, Vector3, Vector3, Color) overload with the index
        /// returned by this method.
        /// </summary>
        internal int ShowLine(Vector3 v1, Vector3 v2, Color color)
        {
            int index;
            // Can we take a spot left by other line?
            if (freeSpots3D.Count > 0)
                index = freeSpots3D.Dequeue();
            else
            {
                if ((persistentLine3DCount + 1) * 2 + 1 >= persistentVertices3D.Length)
                    return 0;
                else
                    index = (persistentLine3DCount++) * 2;
            }

            ShowLine(index, v1, v2, color);
            return index;
        }

        /// <summary>
        /// Use this method only once, then if you want to update the line
        /// use the (int, Vector2, Vector2, Color) overload with the index
        /// returned by this method.
        /// </summary>
        internal int ShowLine(Vector2 v1, Vector2 v2, Color color)
        {
            int index;
            // Can we take a spot left by other line?
            if (freeSpots2D.Count > 0)
                index = freeSpots2D.Dequeue();
            else
            {
                if ((persistentLine2DCount + 1) * 2 + 1 >= persistentVertices2D.Length)
                    return 0;
                else
                    index = (persistentLine2DCount++) * 2;
            }

            ShowLine(index, v1, v2, color);
            return index;
        }
        
        /// <summary>
        /// Only use this method if you know you have a valid index. You
        /// can get a valid index by calling the other overlaods.
        /// </summary>
        internal void ShowLine(int index, Vector3 v1, Vector3 v2, Color color)
        {
            if (index + 1 >= persistentVertices3D.Length)
                return;
            persistentVertices3D[index + 0].Position = v1;
            persistentVertices3D[index + 0].Color = color;
            persistentVertices3D[index + 1].Position = v2;
            persistentVertices3D[index + 1].Color = color;
        }

        /// <summary>
        /// Only use this method if you know you have a valid index. You
        /// can get a valid index by calling the other overlaods.
        /// </summary>
        internal void ShowLine(int index, Vector2 v1, Vector2 v2, Color color)
        {
            if (index + 1 >= persistentVertices2D.Length)
                return;
            persistentVertices2D[index + 0].Position = new Vector3(v1, 0);
            persistentVertices2D[index + 0].Color = color;
            persistentVertices2D[index + 1].Position = new Vector3(v2, 0);
            persistentVertices2D[index + 1].Color = color;
        }

        /// <summary>
        /// Draws a line for one frame.
        /// </summary>
        public void ShowLineOnce(Vector3 v1, Vector3 v2, Color color)
        {
            if (!Visible || GearsetResources.GlobalAlpha <= 0)
                return;
            int index = (singleFrameLine3DCount++) * 2;
            if (index + 1 >= singleFrameVertices3D.Length)
                return;
            singleFrameVertices3D[index + 0].Position = v1;
            singleFrameVertices3D[index + 0].Color = color;
            singleFrameVertices3D[index + 1].Position = v2;
            singleFrameVertices3D[index + 1].Color = color;
        }

        /// <summary>
        /// Draws a line for one frame.
        /// </summary>
        public void ShowLineOnce(Vector2 v1, Vector2 v2, Color color)
        {
            if (!Visible || GearsetResources.GlobalAlpha <= 0)
                return;
            int index = (singleFrameLine2DCount++) * 2;
            if (index + 1 >= singleFrameVertices2D.Length)
                return;
            singleFrameVertices2D[index + 0].Position = new Vector3(v1, 0);
            singleFrameVertices2D[index + 0].Color = color;
            singleFrameVertices2D[index + 1].Position = new Vector3(v2, 0);
            singleFrameVertices2D[index + 1].Color = color;
        }

        /// <summary>
        /// If a line with the specified key existe, remove it. Else, do nothing.
        /// </summary>
        public void DeleteLine3(String key)
        {
            if (persistentLine3DTable.ContainsKey(key))
            {
                freeSpots3D.Enqueue(persistentLine3DTable[key]);
                persistentLine3DTable.Remove(key);
            }
        }

        /// <summary>
        /// If a line with the specified key existe, remove it. Else, do nothing.
        /// </summary>
        public void DeleteLine3(int index)
        {
            // Make the current line invisible
            persistentVertices3D[index].Color = Color.Transparent;
            persistentVertices3D[index + 1].Color = Color.Transparent;
            
            // Let someone else take that index.
            freeSpots3D.Enqueue(index);
        }

        /// <summary>
        /// If a line with the specified key exists, remove it. Else, do nothing.
        /// </summary>
        public void DeleteLine2(String key)
        {
            if (persistentLine2DTable.ContainsKey(key))
            {
                freeSpots2D.Enqueue(persistentLine2DTable[key]);
                persistentLine2DTable.Remove(key);
            }
        }

        /// <summary>
        /// If a line with the specified key exists, remove it. Else, do nothing.
        /// </summary>
        public void DeleteLine2(int index)
        {
            // Make the current line invisible
            persistentVertices2D[index].Color = Color.Transparent;
            persistentVertices2D[index + 1].Color = Color.Transparent;

            // Let someone else take that index.
            freeSpots2D.Enqueue(index);
        }
        #endregion

        #region Draw
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Only draw if we're doing a BasicEffectPass pass
            if (GearsetResources.CurrentRenderPass == RenderPass.BasicEffectPass)
            {
                // If there are no lines, don't draw anything.
                if (persistentLine3DCount > 0)
                    GearsetResources.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, persistentVertices3D, 0, persistentLine3DCount);

                if (singleFrameLine3DCount > 0)
                {
                    GearsetResources.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, singleFrameVertices3D, 0, singleFrameLine3DCount);
                    singleFrameLine3DCount = 0;
                }
            }

            RenderPass valid2DPass = (CoordinateSpace == CoordinateSpace.ScreenSpace ? RenderPass.ScreenSpacePass : RenderPass.GameSpacePass);
            if (GearsetResources.CurrentRenderPass ==  valid2DPass)
            {
                // If there are no lines, don't draw anything.
                if (persistentLine2DCount > 0)
                    GearsetResources.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, persistentVertices2D, 0, persistentLine2DCount);

                if (singleFrameLine2DCount > 0)
                {
                    GearsetResources.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, singleFrameVertices2D, 0, singleFrameLine2DCount);
                    singleFrameLine2DCount = 0;
                }
            }
        }

        #endregion
    }

    public enum CoordinateSpace
    {
        /// <summary>
        /// The geometry will be interpreted as being in screen space.
        /// </summary>
        ScreenSpace,

        /// <summary>
        /// The geometry will be interpreted as being in game space, thus the Transform2D matrix will be applied.
        /// </summary>
        GameSpace,
    }
}

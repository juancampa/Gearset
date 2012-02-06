using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gearset.Components;

namespace Gearset.UI
{
    /// <summary>
    /// A box.
    /// </summary>
    public class LayoutBox
    {
        /// <summary>
        /// Gets or sets the parent of this LayoutBox.
        /// </summary>
        public LayoutBox Parent { get; set; }

        public event RefEventHandler<Vector2> MouseDown;
        public event RefEventHandler<Vector2> MouseUp;
        public event RefEventHandler<Vector2> Click;

        public bool IsMouseOver { get; internal set; }


        // For now the argument means the delta change.
        public event RefEventHandler<Vector2> Dragged;

        /// <summary>
        /// Gets or sets the position of this LayoutBox.
        /// </summary>
        /// <value>The position</value>
        public Vector2 Position { get { return position; } set { position = value; OnPositionChanged(); } }
        private Vector2 position;
        public Vector2 Size { get { return size; } set { size = new Vector2(Math.Max(0, value.X), Math.Max(0, value.Y)); OnSizeChanged(); } }
        private Vector2 size;

        public float Left { get { return position.X; } set { position.X = value; } }
        public float Right { get { return position.X + size.X; } set { position.X = value - size.X; } }
        public float Top { get { return position.Y; } set { position.Y = value; } }
        public float Bottom { get { return position.X + size.Y; } set { position.Y = value - size.Y; } }

        public Vector2 Center { get { return position + size * .5f; } set { position = value - size * .5f; } }
        public Vector2 TopLeft { get { return new Vector2(Position.X, position.Y); } set { position = value; } }
        public Vector2 TopRight { get { return new Vector2(Position.X + Size.X, position.Y); } set { position = new Vector2(value.X - size.X, value.Y); } }
        public Vector2 BottomRight { get { return position + size; } set { position = value - size; } }
        public Vector2 BottomLeft { get { return new Vector2(Position.X, position.Y + Size.Y); } set { position = new Vector2(value.X, value.Y - size.Y); } }

        public float Width { get { return size.X; } set { size.X = Math.Max(value, 0); } }
        public float Height { get { return size.Y; } set { size.Y = Math.Max(value, 0); } }

        /// <summary>
        /// Returns the area where the elements of this UI box must be drawn.
        /// </summary>
        public Rectangle DrawArea { get { return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y); } }

        public LayoutBox(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;

            UIManager.Boxes.Add(this);
        }

        public LayoutBox(Vector2 position)
        {
            Position = position;
            Size = new Vector2(100);    // Some default size.

            UIManager.Boxes.Add(this);
        }

        protected virtual void OnPositionChanged() { }
        protected virtual void OnSizeChanged() { }

        public Vector2 GetScreenPosition()
        {
            Vector2 screenPosition = Position;
            var current = Parent;
            while (current != null)
            {
                screenPosition += current.Position;
                current = current.Parent;
            }
            return screenPosition;
        }

        /// <summary>
        /// Helper methods, draws the border of the box. Must be called every frame.
        /// </summary>
        public void DrawCrossLines(Color color)
        {
            DrawCrossLines(color, GearsetResources.Console.LineDrawer);
        }
        public void DrawCrossLines(Color color, InternalLineDrawer lineDrawer)
        {
            lineDrawer.ShowLineOnce(TopLeft, BottomRight, color);
            lineDrawer.ShowLineOnce(TopRight, BottomLeft, color);
        }

        /// <summary>
        /// Helper methods, draws the border of the box. Must be called every frame.
        /// </summary>
        public void DrawBorderLines(Color color)
        {
            DrawBorderLines(color, GearsetResources.Console.LineDrawer);
        }
        /// <summary>
        /// Helper methods, draws the border of the box. Must be called every frame.
        /// TODO: Move this to a UI debug drawer or something similar
        /// </summary>
        internal void DrawBorderLines(Color color, InternalLineDrawer lineDrawer)
        {
            Vector2 screenPos = GetScreenPosition();
            Vector2 tl = screenPos;
            Vector2 tr = new Vector2(screenPos.X + size.X, screenPos.Y);
            Vector2 br = new Vector2(screenPos.X + size.X, screenPos.Y + size.Y);
            Vector2 bl = new Vector2(screenPos.X, screenPos.Y + size.Y);
            lineDrawer.ShowLineOnce(tl, tr, color);
            lineDrawer.ShowLineOnce(tr, br, color);
            lineDrawer.ShowLineOnce(br, bl, color);
            lineDrawer.ShowLineOnce(bl, tl, color);
        }

        /// <summary>
        /// Returns true if the passed point is contained in this box.
        /// </summary>
        internal bool Contains(Vector2 point)
        {
            Vector2 min = GetScreenPosition();
            Vector2 max = min + Size;

            return
                point.X >= min.X && point.X <= max.X &&
                point.Y >= min.Y && point.Y <= max.Y;
        }

        /// <summary>
        /// Returns the passed world point in local point of this LayoutBox.
        /// </summary>
        public Vector2 WorldToLocal(Vector2 point)
        {
            var current = Parent;
            while (current != null)
            {
                point -= current.Position;
                current = current.Parent;
            }
            return point;
        }

        #region Event raisers
        /// <summary>
        /// Only to be called by the MouseRouter
        /// </summary>
        internal void RaiseMouseDown(Vector2 position)
        {
            if (MouseDown != null)
                MouseDown(this, ref position);
        }

        /// <summary>
        /// Only to be called by the MouseRouter
        /// </summary>
        internal void RaiseMouseUp(Vector2 position)
        {
            if (MouseUp != null)
                MouseUp(this, ref position);
        }

        /// <summary>
        /// Only to be called by the MouseRouter
        /// </summary>
        internal void RaiseClick(Vector2 position)
        {
            if (Click != null)
                Click(this, ref position);
        }

        /// <summary>
        /// Only to be called by the MouseRouter
        /// </summary>
        internal void RaiseDragged(Vector2 delta)
        {
            if (Dragged != null)
                Dragged(this, ref delta);
        } 
        #endregion

    }
}

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

namespace Gearset.UI
{
    /// <summary>
    /// Routes the mouse events in to the appopiate box in the box list.
    /// </summary>
    public class MouseRouter
    {
        private MouseState prevState;
        private MouseState state;

        //private UIManager parent;

        private List<LayoutBox> downCollidingSet;
        private List<LayoutBox> upCollidingSet;
        private List<LayoutBox> overCollidingSet;

        private bool HaveFocus { get { return true; } }// return parent.Game.IsActive; } }

        /// <summary>
        /// True if the mouse was just pressed, last one frame true.
        /// </summary>
        public bool IsLeftJustDown
        {
            get { return (state.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released && HaveFocus); }
        }

        /// <summary>
        /// True if the mouse was just released, last one frame true.
        /// </summary>
        public bool IsLeftJustUp
        {
            get { return (state.LeftButton == ButtonState.Released && prevState.LeftButton == ButtonState.Pressed && HaveFocus); }
        }

        public MouseRouter()
        {
            //this.parent = parent;
            state = new MouseState();
            downCollidingSet = new List<LayoutBox>();
            upCollidingSet = new List<LayoutBox>();
            overCollidingSet = new List<LayoutBox>();
        }

        public void Update()
        {
            prevState = state;
            state = Mouse.GetState();
            Vector2 position = new Vector2(state.X, state.Y);
            Vector2 prevPosition = new Vector2(prevState.X, prevState.Y);

            // Check mouse down.
            if (IsLeftJustDown)
            {
                FindCollidingBoxes(downCollidingSet, position);
                foreach (var item in downCollidingSet)
                    item.RaiseMouseDown(item.WorldToLocal(position));
            }
            // Check mouse up, click.
            else if (IsLeftJustUp)
            {
                FindCollidingBoxes(upCollidingSet, position);
                foreach (var item in upCollidingSet)
                {
                    Vector2 local = item.WorldToLocal(position);
                    item.RaiseMouseUp(local);
                    if (downCollidingSet.Contains(item))
                        item.RaiseClick(local);
                }

                // Forget the down list.
                downCollidingSet.Clear();
            }

            if (state.LeftButton == ButtonState.Released)
            {
                foreach (var box in UIManager.Boxes)
                {
                    if (box.Contains(position))
                        box.IsMouseOver = true;
                    else
                        box.IsMouseOver = false;
                }
            }

            // Check drag.
            if (position != prevPosition)
            {
                foreach (var item in downCollidingSet)
                {
                    item.RaiseDragged(position - prevPosition);
                }
            }
        }

        private void FindCollidingBoxes(List<LayoutBox> collidingSet, Vector2 position)
        {
            collidingSet.Clear();
            foreach (var box in UIManager.Boxes)
            {
                if (box.Contains(position))
                    collidingSet.Add(box);
            }
        }
    }
}

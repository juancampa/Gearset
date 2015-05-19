using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Gearset.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MouseComponent : Gear
    {
        MouseState state;
        MouseState prevState;

        /// <summary>
        /// Can last only one frame true
        /// </summary>
        private bool justClicked;
        /// <summary>
        /// Can last only one frame true
        /// </summary>
        private bool justDragging;

        /// <summary>
        /// Remains true while the left mouse button is pressed.
        /// </summary>
        private bool mouseDown;

        /// <summary>
        /// How long the mouse was down.
        /// </summary>
        private int mouseDownTime;

        /// <summary>
        /// The position where the left button became pressed.
        /// </summary>
        private Vector2 mouseDownPosition;

        private bool dragging;

        public Vector2 Position
        {
            get { return new Vector2(state.X, state.Y); }
        }

        private bool HaveFocus { get { return Game.IsActive; } }

        /// <summary>
        /// The distance the (down left) mouse can move without being
        /// considered a drag (still a click). Should be zero for PC
        /// games and some higher value for tablets.
        /// </summary>
        public float ClickThreshold { get; set; }

        /// <summary>
        /// Set to true if the component should force the mouse to 
        /// stay inside the client logger bounds.
        /// </summary>
        public bool KeepMouseInWindow { get; set; }

        /// <summary>
        /// Get the movement the mouse have made since it started dragging.
        /// If the mouse is not dragging it will return Vector2.Zero.
        /// </summary>
        public Vector2 DragOffset
        {
            get
            {
                if (dragging)
                    return Position - mouseDownPosition;
                else
                    return Vector2.Zero;
            }

        }

        #region Constructor
        public MouseComponent()
            : base(new GearConfig())
        {
            state = Mouse.GetState();
        } 
        #endregion


        #region Update
        public override void Update(GameTime gameTime)
        {
            prevState = state;
            state = Mouse.GetState();
            justClicked = false;
            justDragging = false;
            if (mouseDown)
            {
                if (Vector2.Distance(Position, mouseDownPosition) > ClickThreshold && !dragging)
                {
                    justDragging = true;
                    dragging = true;
                }
                mouseDownTime += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (IsLeftJustUp())
            {
                if (!dragging)
                    justClicked = true;
                dragging = false;
                mouseDown = false;
            }

            if (IsLeftJustDown())
            {
                mouseDownPosition = Position;
                mouseDown = true;
                mouseDownTime = 0;
            }

            // Keep the mouse inside
            if (KeepMouseInWindow)
            {       
#if WINDOWS 
				Rectangle rect = Game.Window.ClientBounds; // Rectangle to clip (in screen coordinates)
                System.Windows.Forms.Cursor.Clip = new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
#endif
            }
            else
            {
#if WINDOWS 
                System.Windows.Forms.Cursor.Clip = new System.Drawing.Rectangle(int.MinValue / 2, int.MinValue / 2, int.MaxValue, int.MaxValue);
#endif
            }
            base.Update(gameTime);
        }

        #endregion

        #region Is Left Just Up/Down/Click
        /// <summary>
        /// True if the mouse was just pressed, last one frame true.
        /// </summary>
        public bool IsLeftJustDown()
        {
            return (state.LeftButton == ButtonState.Pressed && prevState.LeftButton == ButtonState.Released && HaveFocus);
        }

        /// <summary>
        /// True if the mouse was just released, last one frame true.
        /// </summary>
        public bool IsLeftJustUp()
        {
            return (state.LeftButton == ButtonState.Released && prevState.LeftButton == ButtonState.Pressed && HaveFocus);
        }

        /// <summary>
        /// True if mouse did a released-pressed-released cycle
        /// without moving the ClickThreshold.
        /// </summary>
        public bool IsLeftClick()
        {
            return justClicked && HaveFocus;
        } 
        #endregion

        #region Is Right Just Up/Down/Click
        /// <summary>
        /// True if the mouse was just pressed, last one frame true.
        /// </summary>
        public bool IsRightJustDown()
        {
            return (state.RightButton == ButtonState.Pressed && prevState.RightButton == ButtonState.Released && HaveFocus);
        }

        /// <summary>
        /// True if the mouse was just released, last one frame true.
        /// </summary>
        public bool IsRightJustUp()
        {
            return (state.RightButton == ButtonState.Released && prevState.RightButton == ButtonState.Pressed && HaveFocus);
        }
        #endregion


        #region Dragging
        /// <summary>
        /// The mouse mave moved the ClickThreshold since it was pressed.
        /// </summary>
        /// <returns></returns>
        public bool IsDragging()
        {
            return dragging && HaveFocus;
        }

        /// <summary>
        /// The mouse just moved the threshold, this will only be true
        /// for one frame.
        /// </summary>
        public bool IsJustDragging()
        {
            return justDragging && HaveFocus;
        } 
        #endregion



    }
}
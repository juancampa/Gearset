using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DebugConsole.Components
{
    /// <summary>
    /// Shows important alerts on the screen.
    /// </summary>
    public class SliderManager : DebugComponent
    {
        /// <summary>
        /// The position for the next slider created.
        /// </summary>
        private Vector2 nextPosition;
        private SpriteBatch spriteBatch;
        private Texture2D whiteTexture;
        private List<Slider> sliders;

        private MouseState mouse, prevMouse;

        
        #region Constructors
        public SliderManager()
            : base()
        {
            this.nextPosition = new Vector2(Resources.Device.Viewport.Width - 200, Resources.Device.Viewport.Height - 300);
            this.spriteBatch = Resources.SpriteBatch;
#if XBOX
            this.whiteTexture = Resources.Content.Load<Texture2D>("white_Xbox360");
#else
            this.whiteTexture = Resources.Content.Load<Texture2D>("white");
#endif
            //this.whiteTexture = Resources.Content.Load<Texture2D>("white");
            this.sliders = new List<Slider>();
            Resources.Console.AddGlobal("slider", this);
        }
        #endregion

        public void Create(String variableName, float min, float max)
        {
            this.sliders.Add(new Slider(nextPosition, min, max, variableName));
            this.nextPosition.Y += 20;
            mouse = Mouse.GetState();
        }

        public void Create(String variableName)
        {
            this.sliders.Add(new Slider(nextPosition, variableName));
            this.nextPosition.Y += 20;
            mouse = Mouse.GetState();
        }

        #region Update
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            prevMouse = mouse;
            mouse = Mouse.GetState();
            foreach (Slider s in sliders)
            {
                s.Update(mouse, prevMouse);
            }
        }
        #endregion

        #region Draw
        public override void  Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            foreach (Slider s in sliders)
            {
                spriteBatch.DrawString(Resources.FontTiny, s.ToString(), s.position - Vector2.UnitY * 14f, Color.LightGray);
                spriteBatch.Draw(whiteTexture, new Rectangle(s.sliderRect.X - 1, s.sliderRect.Y - 1, s.sliderRect.Width + 2, s.sliderRect.Height + 2), Color.Black);
                spriteBatch.Draw(whiteTexture, s.sliderRect, Color.Gray);
                spriteBatch.Draw(whiteTexture, s.handleRect, Color.Orange);
            }
            spriteBatch.End();
        }
        #endregion
    }
}

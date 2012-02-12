using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DebugConsole.Components
{
    class Slider
    {
        public Vector2 position;

        /// <summary>
        /// Min and max define how much is 0 and how much is 1.
        /// </summary>
        public float min, max;

        /// <summary>
        /// Normalized, in the range [0,1]
        /// </summary>
        public float value;
        public int width;
        public String variableName;
        public Rectangle sliderRect, handleRect;

        public bool sliding = false;

        #region Constructors
        public Slider(Vector2 position, float min, float max, String variableName)
        {
            this.position = position;
            this.width = 100;
            this.max = max;
            this.min = min;
            this.variableName = variableName;
            this.value = Normalize(GetCurrentValue());
            this.sliderRect = new Rectangle((int)position.X, (int)position.Y, width + 5, 5);
            
        }
        /// <summary>
        /// A slider will be created with max=value+5 and min=value-5;
        /// </summary>
        /// <param name="position"></param>
        /// <param name="variableName"></param>
        public Slider(Vector2 position, String variableName)
        {
            this.position = position;
            this.width = 100;
            this.variableName = variableName;
            float val = GetCurrentValue();
            this.max = val + 5;
            this.min = val - 5;
            this.value = Normalize(val);
            this.sliderRect = new Rectangle((int)position.X, (int)position.Y, width + 5, 5);
            
        }
        #endregion

        private float Normalize(float v)
        {
            return (v - min) / (max - min);
        }

        private float Denormalize(float v)
        {
            return v * (max - min) + min;
        }

        /// <summary>
        /// Returns the value of the float that correspond to this slider.
        /// </summary>
        /// <returns></returns>
        private float GetCurrentValue()
        {
            Object val;
            // A boxing will occur here.
            Resources.Console.PyExecute(variableName, out val);
            return (float)val;
        }

        public void Update(MouseState mouse, MouseState prevMouse)
        {
            if (handleRect.Contains(mouse.X, mouse.Y) && mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                sliding = true;
            }
            if (sliding)
            {
                if (mouse.LeftButton == ButtonState.Released)
                    sliding = false;
                value = MathHelper.Clamp((mouse.X - (int)position.X) / (float)width, 0, 1);
                float realvalue = min + value * (max - min);
                //Console.WriteLine(variableName + " = " + realvalue.ToString());
                Resources.Console.PyExecute(variableName + " = " + realvalue.ToString().Replace(',', '.'));
            }
            int xpos = (int)(position.X + MathHelper.Lerp(0, width, value));
            int ypos = (int)position.Y;
            handleRect = new Rectangle(xpos, ypos, 5, 5);
        }

        public override string ToString()
        {
            return variableName + "(" + Denormalize(value) + ")";
        }
    }
}

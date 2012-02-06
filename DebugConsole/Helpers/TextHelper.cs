using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gearset
{
    /// <summary>
    /// Contains some helper methods for handling text.
    /// </summary>
    public static class TextHelper
    {
        #region DrawText
        /// <summary>
        /// A Helper method to draw some text. No SpriteBatch.Begin() is
        /// called so this must be done by the calling method.
        /// </summary>
        private static void DrawText(String text, Vector2 drawPosition)
        {
            Color color = new Color(1, 1, 1, GearsetResources.GlobalAlpha);
            Color shadowColor = new Color(0, 0, 0, GearsetResources.GlobalAlpha);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(-1, 0), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(0, 1), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(0, -1), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(1, 0), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition, color);
        }
        #endregion

        #region FormatForDebug
        public static string FormatForDebug(Object val)
        {
            String output;
            if (val is Vector3)
            {
                Vector3 v = (Vector3)val;
                output = String.Format("({0:F4}, {1:F4}, {2:F4})",
                    v.X, v.Y, v.Z);
            }
            else if (val is Vector2)
            {
                Vector2 v = (Vector2)val;
                output = String.Format("({0:F4}, {1:F4})",
                    v.X, v.Y);
            }
            else
            {
                if (val == null)
                    output = "<null>";
                else
                    output = val.ToString();
            }
            return output;
        }
        #endregion
    }
}

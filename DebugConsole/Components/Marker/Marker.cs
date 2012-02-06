using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Places marks on 3D space with labels on 2D space
    /// </summary>
    public class Marker : Gear
    {
        private Dictionary<String, DebugMark> markTable = new Dictionary<String, DebugMark>();
        private SpriteBatch spriteBatch;
        private BasicEffect effect;
        private Texture2D markTexture;

        #region Constructor
        public Marker()
            : base(new GearConfig())
        {
            this.spriteBatch = GearsetResources.SpriteBatch;
            //this.effect = Resources.effect;
            this.effect = GearsetResources.Effect;
#if XBOX
            this.markTexture = GearsetResources.Content.Load<Texture2D>("mark_Xbox360");
#elif WINDOWS_PHONE
            this.markTexture = GearsetResources.Content.Load<Texture2D>("mark_wp");
#else
            this.markTexture = GearsetResources.Content.Load<Texture2D>("mark");
#endif
        }
        #endregion

        #region ShowMark (public methods)
        public void ShowMark(String key, Vector3 position, Color color)
        {
            if (!markTable.ContainsKey(key))
                this.AddMark(key, position, color, false);
            else
                this.SetMark(key, position, color);

        }

        public void ShowMark(String key, Vector3 position)
        {
            if (!markTable.ContainsKey(key))
                this.AddMark(key, position, Color.Yellow, false);
            else
                this.SetMark(key, position);

        }

        /// <summary>
        /// ScreenSpace Mark.
        /// </summary>
        public void ShowMark(String key, Vector2 screenPosition, Color color)
        {
            Vector3 position = new Vector3(screenPosition, 0);
            if (!markTable.ContainsKey(key))
                this.AddMark(key, position, color, true);
            else
                this.SetMark(key, position, color);

        }

        /// <summary>
        /// ScreenSpace Mark.
        /// </summary>
        public void ShowMark(String key, Vector2 screenPosition)
        {
            Vector3 position = new Vector3(screenPosition, 0);
            if (!markTable.ContainsKey(key))
                this.AddMark(key, position, Color.Yellow, true);
            else
                this.SetMark(key, position);

        }
        #endregion

        #region AddMark/SetMark (private methods)
        private void AddMark(String key, Vector3 position, Color color, bool screenSpace)
        {
            Vector2 textSize = GearsetResources.FontTiny.MeasureString(key);
            RenderTarget2D renderTarget;

            
            renderTarget = new RenderTarget2D(GearsetResources.Device, (int)Math.Ceiling(textSize.X), (int)Math.Ceiling(textSize.Y));

            // Save the current render targets.
            RenderTargetBinding[] savedRenderTargets = GearsetResources.Device.GetRenderTargets();
            
            // Draw the label to a special renderTarget and then extract the texture.
            GearsetResources.Device.SetRenderTarget(renderTarget);
            GearsetResources.Device.Clear(new Color(0.0f, 0.0f, 0.0f, 0.4f));
            spriteBatch.Begin();
            spriteBatch.DrawString(GearsetResources.FontTiny, key, Vector2.One, Color.Black);
            spriteBatch.DrawString(GearsetResources.FontTiny, key, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Restore the render targets.
            GearsetResources.Device.SetRenderTargets(savedRenderTargets);

            // Finally add the mark.
            markTable.Add(key, new DebugMark(position, color, renderTarget, screenSpace));
        }

        private void SetMark(String key, Vector3 position, Color color)
        {
            ((DebugMark)markTable[key]).MoveTo(position);
            ((DebugMark)markTable[key]).color = color;
        }

        private void SetMark(String key, Vector3 position)
        {
            ((DebugMark)markTable[key]).MoveTo(position);
        }
        #endregion

        #region Draw
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch (GearsetResources.CurrentRenderPass)
            {
                case RenderPass.BasicEffectPass: DrawMarks();
                    break;
                case RenderPass.SpriteBatchPass: DrawMarksLabels();
                    break;
            }
            base.Draw(gameTime);
        }

        private void DrawMarks()
        {
            // Draw the marks.
            foreach (KeyValuePair<String, DebugMark> m in markTable)
            {
                DebugMark mark = m.Value as DebugMark;
                if (mark.ScreenSpace == true) continue;
                switch (((DebugMark)m.Value).Type)
                {
                    case MarkType.Cross:
                        GearsetResources.Device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, ((DebugMark)m.Value).mark, 0, 3);
                        break;
                }
            }
        }

        private void DrawMarksLabels()
        {
            // The marks' labels
            foreach (KeyValuePair<String, DebugMark> m in markTable)
            {
                DebugMark mark = m.Value as DebugMark;
                if (mark.ScreenSpace == true)
                {
                    if (mark.ScreenSpace == false) continue;
                    spriteBatch.Draw(markTexture, mark.ScreenSpacePosition - new Vector2((int)(markTexture.Width * .5f), (int)(markTexture.Height * .5f)), mark.color);
                    spriteBatch.Draw(mark.label, mark.ScreenSpacePosition + new Vector2((int)(markTexture.Width * .5f), 0), Color.White);
                }
                else
                {
                    Texture2D texture = mark.label;
                    Vector3 position = Vector3.Transform(mark.mark[0].Position, Matrix.Identity * (GearsetResources.View * GearsetResources.Projection));
                    if (position.Z < 0) continue;
                    Rectangle dest = new Rectangle((int)(((position.X / position.Z + 1) / 2) * GearsetResources.Device.Viewport.Width),
                                                    (int)(((-position.Y / position.Z + 1) / 2) * GearsetResources.Device.Viewport.Height),
                                                    (int)(texture.Width),
                                                    (int)(texture.Height));
                    //Color color = new Color((byte)(255 - position.Z * 10), (byte)(255 - position.Z * 10), (byte)(255 - position.Z * 10));
                    spriteBatch.Draw(texture, dest, Color.White * GearsetResources.GlobalAlpha);
                }
            }
        }
        #endregion
    }
}

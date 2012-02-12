using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gearset.Components
{
    /// <summary>
    /// Shows important alerts on the screen
    /// </summary>
    public class Alerter : Gear
    {
        private List<AlertItem> alerts = new List<AlertItem>();
        private Vector2 alertPosition;
        private SpriteBatch spriteBatch;

        private float textHeight;

        /// <summary>
        /// We use this list to delete elements that are not
        /// being shown anymore.
        /// </summary>
        LinkedList<AlertItem> toRemove = new LinkedList<AlertItem>();

        #region Constructors
        public Alerter()
            : base(GearsetSettings.Instance.AlerterConfig)
        {
            this.spriteBatch = GearsetResources.SpriteBatch;
            textHeight = GearsetResources.FontAlert.MeasureString("M").Y;
            OnResolutionChanged();
        }
        #endregion

        public override void OnResolutionChanged()
        {
            this.alertPosition = new Vector2(GearsetResources.Device.Viewport.Width / 2, GearsetResources.Device.Viewport.Height / 2 - textHeight * 1.5f);
        }


        #region Alerts
        public void Alert(String message)
        {
            alerts.Add(new AlertItem(message, alertPosition, 40));
            this.alertPosition.Y += 48;
            if (this.alertPosition.Y >= 300) this.alertPosition.Y = 150;
        }
        #endregion

        #region Update
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            foreach (AlertItem i in alerts)
            {
                if (i.remainingTime > 0) i.remainingTime--;
                else if (i.remainingTime == 0) toRemove.AddLast(i);
            }
            foreach (AlertItem i in toRemove)
            {
                alerts.Remove(i);
            }
            toRemove.Clear();
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void  Draw(GameTime gameTime)
        {
            // Only draw if we're doing a spriteBatch passs
            if (GearsetResources.CurrentRenderPass != RenderPass.SpriteBatchPass) return;
            foreach (AlertItem i in alerts)
            {
                float textWidth = GearsetResources.FontAlert.MeasureString(i.Text).X;
                Vector2 origin = new Vector2(textWidth * .5f, textHeight * .5f);
                byte alpha = (byte)MathHelper.Clamp((i.remainingTime / 7f) * 255, 0, 255);
                Color forecolor = new Color(alpha, alpha, alpha, alpha);
                Color backcolor = new Color(0, 0, 0, (byte)MathHelper.Clamp((i.remainingTime / 60f) * 255, 0, 255));
                spriteBatch.DrawString(GearsetResources.FontAlert, ((AlertItem)i).Text, ((AlertItem)i).position + new Vector2(-2, 2), backcolor, 0, origin, Vector2.One, SpriteEffects.None, 0);
                spriteBatch.DrawString(GearsetResources.FontAlert, ((AlertItem)i).Text, ((AlertItem)i).position + new Vector2(0, 0), forecolor, 0, origin, Vector2.One, SpriteEffects.None, 0);
            }
        }
        #endregion
    }
}

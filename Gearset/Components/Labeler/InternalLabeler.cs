using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.Components;
using Microsoft.Xna.Framework.Graphics;
using Gearset;
using Gearset.Extensions;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Shows labels of text in 2D positions or 3d (TODO)s unprojected positions.
    /// </summary>
    public class InternalLabeler : Gear
    {
        private Dictionary<String, Label> persistentLabels;
        private Dictionary<String, Label2D> persistent2DLabels;
        private Dictionary<String, Label3D> persistent3DLabels;

        private class Label
        {
            public String Name;
            public StringBuilder _stringBuilder;
            public Vector2 Position;
            public Color Color;

            public Label(String name, Vector2 position, Color color)
            {
                Name = name;
                _stringBuilder = new StringBuilder(64);
                Position = position;
                Color = color;
            }
        }

        private class Label2D
        {
            public String Name;
            public String Text;
            public Vector2 Position;
            public Color Color;

            public Label2D(String name, Vector2 position, String text, Color color)
            {
                Name = name;
                Text = text;
                Position = position;
                Color = color;
            }
        }

        private class Label3D
        {
            public String Name;
            public String Text;
            public Vector3 Position;
            public Color Color;

            public Label3D(String name, Vector3 position, String text, Color color)
            {
                Name = name;
                Text = text;
                Position = position;
                Color = color;
            }
        }

        public LabelerConfig Config { get { return GearsetSettings.Instance.LabelerConfig; } }

        public InternalLabeler()
            : this (new LabelerConfig())
        {
        }

        public InternalLabeler(LabelerConfig config)
            : base(config)
        {
            persistentLabels = new Dictionary<string, Label>();
            persistent2DLabels = new Dictionary<string, Label2D>();
            persistent3DLabels = new Dictionary<string, Label3D>();
            Config.DefaultColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);
            Config.Cleared += new EventHandler(Config_Cleared);
        }

        void Config_Cleared(object sender, EventArgs e)
        {
            persistentLabels.Clear();
            persistent2DLabels.Clear();
            persistent3DLabels.Clear();
        }

        #region Persistent 2D Labels
        /// <summary>
        /// Shows a label in the specified position. or changes a label position.
        /// </summary>
        public void ShowLabel(String name, Vector2 position)
        {
            Label2D label;
            if (!persistent2DLabels.TryGetValue(name, out label))
            {
                label = new Label2D(name, position, name, Config.DefaultColor);
                persistent2DLabels.Add(name, label);
            }
            else
            {
                label.Position = position;
            }
        }

        /// <summary>
        /// Creates a label, or changes its values.
        /// </summary>
        public void ShowLabel(String name, Vector2 position, String text)
        {
            Label2D label;
            if (!persistent2DLabels.TryGetValue(name, out label))
            {
                label = new Label2D(name, position, text, Config.DefaultColor);
                persistent2DLabels.Add(name, label);
            }
            else
            {
                label.Text = text;
                label.Position = position;
            }
        }

        /// <summary>
        /// Creates a label or changes its values
        /// </summary>
        public void ShowLabel(String name, Vector2 position, String text, Color color)
        {
            Label2D label;
            if (!persistent2DLabels.TryGetValue(name, out label))
            {
                label = new Label2D(name, position, text, color);
                persistent2DLabels.Add(name, label);
            }
            else
            {
                label.Text = text;
                label.Position = position;
                label.Color = color;
            }
        }

        public void ShowLabelEx(String name, Vector2 position, StringBuilder stringBuilder, Color color)
        {
            Label label;
            if (!persistentLabels.TryGetValue(name, out label))
            {
                label = new Label(name, position, color);
                persistentLabels.Add(name, label);
            }
            else
            {
                label.Position = position;
                label.Color = color;
            }

            label._stringBuilder.SetText(stringBuilder);
        }
        #endregion

        #region Persistent 3D Labels
        /// <summary>
        /// Shows a label in the specified position. or changes a label position.
        /// </summary>
        public void ShowLabel(String name, Vector3 position)
        {
            Label3D label;
            if (!persistent3DLabels.TryGetValue(name, out label))
            {
                label = new Label3D(name, position, name, Config.DefaultColor);
                persistent3DLabels.Add(name, label);
            }
            else
            {
                label.Position = position;
            }
        }

        /// <summary>
        /// Creates a label, or changes its values.
        /// </summary>
        public void ShowLabel(String name, Vector3 position, String text)
        {
            Label3D label;
            if (!persistent3DLabels.TryGetValue(name, out label))
            {
                label = new Label3D(name, position, text, Config.DefaultColor);
                persistent3DLabels.Add(name, label);
            }
            else
            {
                label.Text = text;
                label.Position = position;
            }
        }

        /// <summary>
        /// Creates a label or changes its values
        /// </summary>
        public void ShowLabel(String name, Vector3 position, String text, Color color)
        {
            Label3D label;
            if (!persistent3DLabels.TryGetValue(name, out label))
            {
                label = new Label3D(name, position, text, color);
                persistent3DLabels.Add(name, label);
            }
            else
            {
                label.Text = text;
                label.Position = position;
                label.Color = color;
            }
        }
        #endregion

        /// <summary>
        /// Remove a persistent label if exists.
        /// </summary>
        public void HideLabel(String name)
        {
            persistent2DLabels.Remove(name);
        }

        public void HideLabelEx(String name)
        {
            persistentLabels.Remove(name);
        }

        public override void Draw(GameTime gameTime)
        {
            if (GearsetResources.CurrentRenderPass != RenderPass.SpriteBatchPass)
                return;

            // Draw persistent labels.
            foreach (Label label in persistentLabels.Values)
            {
                GearsetResources.SpriteBatch.DrawString(GearsetResources.FontTiny, label._stringBuilder, label.Position + Vector2.One, Color.Black * GearsetResources.GlobalAlpha);
                GearsetResources.SpriteBatch.DrawString(GearsetResources.FontTiny, label._stringBuilder, label.Position, label.Color * GearsetResources.GlobalAlpha);
            }

            foreach (Label2D label in persistent2DLabels.Values)
            {
                GearsetResources.SpriteBatch.DrawString(GearsetResources.FontTiny, label.Text, label.Position + Vector2.One, Color.Black * GearsetResources.GlobalAlpha);
                GearsetResources.SpriteBatch.DrawString(GearsetResources.FontTiny, label.Text, label.Position, label.Color * GearsetResources.GlobalAlpha);
            }

            // Draw persistent labels.
            Matrix transform;
            Matrix.Multiply(ref GearsetResources.World, ref GearsetResources.View, out transform);
            Matrix.Multiply(ref transform, ref GearsetResources.Projection, out transform);
            Vector2 screenSize = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth, Game.GraphicsDevice.PresentationParameters.BackBufferHeight);
            foreach (Label3D label in persistent3DLabels.Values)
            {
                Vector3 projected;
                Vector3.Transform(ref label.Position, ref transform, out projected);
                
                Vector2 screenPos = new Vector2(projected.X, -projected.Y) / projected.Z;
                screenPos = screenPos * .5f + Vector2.One * .5f;
                screenPos *= screenSize;
                GearsetResources.SpriteBatch.DrawString(GearsetResources.FontTiny, label.Text, screenPos + Vector2.One, Color.Black * GearsetResources.GlobalAlpha);
                GearsetResources.SpriteBatch.DrawString(GearsetResources.FontTiny, label.Text, screenPos, label.Color * GearsetResources.GlobalAlpha);
            }
            base.Draw(gameTime);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gearset.Components
{
    /// <summary>
    /// Displays a hierarchy of values that need to be traced.
    /// </summary>
    public class TreeView : Gear
    {
        private TreeViewNode root;
        private int iterationCount;

        Texture2D closedTexture;
        Texture2D openedTexture;

        Vector2 Position;
        MouseState prevMouse;

        public TreeViewConfig Config { get { return GearsetSettings.Instance.TreeViewConfig; } }

        #region Constructor
        public TreeView()
            : base(GearsetSettings.Instance.TreeViewConfig)
        {
            Config.Cleared += new EventHandler(Config_Cleared);
            Config.Filter = String.Empty;

            root = new TreeViewNode("root");
#if XBOX
            this.openedTexture = GearsetResources.Content.Load<Texture2D>("close_Xbox360");
            this.closedTexture = GearsetResources.Content.Load<Texture2D>("open_Xbox360");
#elif WINDOWS_PHONE
            this.openedTexture = GearsetResources.Content.Load<Texture2D>("close_wp");
            this.closedTexture = GearsetResources.Content.Load<Texture2D>("open_wp");
#else
            this.openedTexture = GearsetResources.Content.Load<Texture2D>("close");
            this.closedTexture = GearsetResources.Content.Load<Texture2D>("open");
#endif
            this.Position = new Vector2(5, 20);
        }

        void Config_Cleared(object sender, EventArgs e)
        {
            Clear();
        }
        #endregion

        public void Clear()
        {
            root.Nodes.Clear();
        }

        #region Set
        /// <summary>
        /// Sets the value of a specified key, if the key is not present
        /// in the tree, then is added.
        /// </summary>
        /// <param name="key"></param>
        public void Set(String key, Object value)
        {
            bool foundKey = false;
            TreeViewNode currentNode = root;
            String remainingName = key;
            while (!foundKey)
            {
                iterationCount++;
                String subName;
                bool foundSubKey = false;

                int dotIndex = remainingName.IndexOf('.');
                if (dotIndex < 0 || dotIndex >= remainingName.Length)
                {
                    subName = remainingName;
                    remainingName = "";
                    foundKey = true;
                }
                else
                {
                    subName = remainingName.Remove(dotIndex,remainingName.Length - dotIndex);
                    remainingName = remainingName.Substring(subName.Length + 1);
                }

                foreach (TreeViewNode node in currentNode.Nodes)
                {
                    if (node.Name == subName)
                    {
                        currentNode = node;
                        foundSubKey = true;     // A part of the key was found.
                        break;
                    }
                }
                if (!foundSubKey)
                {
                    // If there's no subKey, we create it.
                    TreeViewNode newNode = new TreeViewNode(subName);
                    currentNode.Nodes.Add(newNode);
                    currentNode = newNode;
                    foundSubKey = true;     // A part of the key was found (created).
                }
            }
            currentNode.Value = value;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
#if XBOX360
            return;
#endif
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed)
            {
                LeftClick(new Vector2(mouse.X, mouse.Y));
            }
            prevMouse = mouse;
        }
        #endregion

        #region LeftClick
        public void LeftClick(Vector2 clickPos)
        {
            int position = 20;
            foreach (TreeViewNode node in root.Nodes)
            {
                if (Config.Filter == String.Empty || node.FilterName.Contains(Config.Filter))
                    position = CheckLeftRecursively(node, position, 1, clickPos);
            }
        }

        private int CheckLeftRecursively(TreeViewNode node, int position, int level, Vector2 clickPos)
        {
            int newPosition = position + 12;

            Rectangle rect = new Rectangle((int)this.Position.X + level * 11 - 12,(int)this.Position.Y + position, closedTexture.Width, closedTexture.Height);
            if (rect.Contains((int)clickPos.X, (int)clickPos.Y))
            {
                node.Toggle();
                return newPosition;
            }
            if (node.Open)
            {
                foreach (TreeViewNode n in node.Nodes)
                {
                    newPosition = CheckLeftRecursively(n, newPosition, level + 1, clickPos);
                }
            }
            return newPosition;
        }

        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            // Only draw if we're doing a spriteBatch pass
            if (GearsetResources.CurrentRenderPass != RenderPass.SpriteBatchPass) return;

            int position = 20;
            foreach (TreeViewNode node in root.Nodes)
            {
                if (Config.Filter == String.Empty || node.FilterName.Contains(Config.Filter))
                    position = DrawRecursively(node, position, 1);
            }
        }

        private int DrawRecursively(TreeViewNode node, int position, int level)
        {
            int newPosition = position + 12;
            DrawNode(node, position, level);
            if (node.Open)
            {
                foreach (TreeViewNode n in node.Nodes)
                {
                    newPosition = DrawRecursively(n, newPosition, level + 1);
                }
            }
            return newPosition;
        }

        
        private void DrawText(String text, Vector2 drawPosition)
        {
            Color color = new Color(1, 1, 1, GearsetResources.GlobalAlpha) * GearsetResources.GlobalAlpha;
            Color shadowColor = new Color(0, 0, 0, GearsetResources.GlobalAlpha) * GearsetResources.GlobalAlpha;
            
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(-1, 0), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(0, 1), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(0, -1), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition + new Vector2(1, 0), shadowColor);
            GearsetResources.SpriteBatch.DrawString(GearsetResources.Font, text, drawPosition, color);

            Vector2 textSize = GearsetResources.Font.MeasureString(text);
            textSize.Y = 12;
            textSize.X += 6;
            drawPosition.X -= 3;
            GearsetResources.Console.SolidBoxDrawer.ShowBoxOnce(drawPosition, drawPosition + textSize);
        }

        private void DrawNode(TreeViewNode node, int position, int level)
        {
            Vector2 drawPosition = new Vector2(this.Position.X + level * 11, this.Position.Y + position);
            if (node.Nodes.Count == 0)
            {
                DrawText(node.Name + ": " + TextHelper.FormatForDebug(node.Value), drawPosition);
            }
            else
            {
                Color color = new Color(1, 1, 1, GearsetResources.GlobalAlpha) * GearsetResources.GlobalAlpha;
                if (!node.Open)
                    GearsetResources.SpriteBatch.Draw(closedTexture, drawPosition - new Vector2(12, 0), color);
                else
                    GearsetResources.SpriteBatch.Draw(openedTexture, drawPosition - new Vector2(12, 0), color);
                DrawText(node.Name, drawPosition);
            }
            
            
        }
        #endregion
    }
}

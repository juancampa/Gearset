using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.UI
{
    /// <summary>
    /// A Window, with a title bar to show a title and drag it. 
    /// Also a scale nob at the bottom-right corner
    /// </summary>
    public class Window : LayoutBox
    {
        private LayoutBox titleBar;
        private LayoutBox scaleNob;

        /// <summary>
        /// Gets the title bar LayoutBox
        /// </summary>
        public LayoutBox TitleBar { get { return titleBar; } }

        /// <summary>
        /// Gets the scale nob LayoutBox.
        /// </summary>
        public LayoutBox ScaleNob { get { return scaleNob; } }

        /// <summary>
        /// Defines a hight for the title bar.
        /// </summary>
        public float TitleBarSize { get { return titleBar.Height; } set { titleBar.Height = value; UpdateLayout(); } }

        public Window(Vector2 position, Vector2 clientSize)
            : base(position, clientSize)
        {

            titleBar = new LayoutBox(position) { Parent = this };
            scaleNob = new LayoutBox(position, new Vector2(6)) { Parent = this };

            scaleNob.Dragged += new RefEventHandler<Vector2>(scaleNob_Dragged);
            titleBar.Dragged += new RefEventHandler<Vector2>(titleBar_Dragged);
            this.Dragged += new RefEventHandler<Vector2>(titleBar_Dragged);

            TitleBarSize = 20;
            //UpdateLayout();
        }

        void titleBar_Dragged(object sender, ref Vector2 delta)
        {
            Position += delta;
            UpdateLayout();
        }

        void scaleNob_Dragged(object sender, ref Vector2 delta)
        {
            Size += delta;
            UpdateLayout();
        }

        protected override void OnPositionChanged()
        {
            UpdateLayout();
        }

        protected override void OnSizeChanged()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Positions (and sizes) the title bar and the scale nob.
        /// </summary>
        public void UpdateLayout()
        {
            if (titleBar == null || scaleNob == null)
                return;

            titleBar.Top = -TitleBarSize;
            titleBar.Left = 0;
            titleBar.Width = Width;
            titleBar.Height = TitleBarSize;

            scaleNob.Position = this.Size;
        }
    }
}

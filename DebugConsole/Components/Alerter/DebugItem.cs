using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gearset.Components
{
    internal class AlertItem
    {
        internal String Text;
        internal Vector2 position;
        internal int remainingTime = -1;

        internal AlertItem(String text, Vector2 position)
        {
            this.Text = text;
            this.position = position;
        }
        internal AlertItem(String text, Vector2 position, int time)
        {
            this.Text = text;
            this.position = position;
            this.remainingTime = time;
        }
    }
}

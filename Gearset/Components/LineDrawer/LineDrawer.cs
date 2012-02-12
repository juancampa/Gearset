using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{

    /// <summary>   
    /// Draw lines in 2D/3D space, can draw line lists and line strips.
    /// </summary>
    public class LineDrawer : InternalLineDrawer
    {
        public LineDrawer()
            : base(GearsetSettings.Instance.LineDrawerConfig)
        {
            CoordinateSpace = Components.CoordinateSpace.GameSpace;
        }
    }
}

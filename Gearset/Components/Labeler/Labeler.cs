using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    /// <summary>
    /// Shows labels of text in 2D positions or 3d (TODO)s unprojected positions.
    /// </summary>
    public class Labeler : InternalLabeler
    {
        public Labeler()
            : base(GearsetSettings.Instance.LabelerConfig)
        {
        }
    }
}

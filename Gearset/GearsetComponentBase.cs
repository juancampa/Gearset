using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset
{
    /// <summary>
    /// Base class for Gearset game components.
    /// </summary>
    public class GearsetComponentBase : DrawableGameComponent
    {
        public GearsetComponentBase(Game game)
            : base(game)
        {
        }
    }
}

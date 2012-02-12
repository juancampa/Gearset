using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    [Serializable]
    public class LabelerConfig : GearConfig
    {
        /// <summary>
        /// Gets or sets the default color of the labels shown.
        /// </summary>
        [Inspector(FriendlyName="Default label color")]
        public Color DefaultColor { get; set; }

        /// <summary>
        /// Raised when the user request labels to be cleared.
        /// </summary>
        [field:NonSerialized]
        public event EventHandler Cleared;

        /// <summary>
        /// Clears all lines
        /// </summary>
        public void Clear()
        {
            if (Cleared != null)
                Cleared(this, EventArgs.Empty);
        }
    }
}

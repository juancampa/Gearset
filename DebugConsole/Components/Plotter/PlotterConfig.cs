using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gearset.Components
{
    [Serializable]
    public class PlotterConfig : GearConfig
    {
        /// <summary>
        /// Gets or sets the default size.
        /// </summary>
        /// <value>The default size</value>
        [Inspector(FriendlyName="Default size of new plots")]
        public Vector2 DefaultSize { get; set; }

        [Inspector(FriendlyName = "Default history length")]
        public int DefaultHistoryLength { 
            get { return GearsetSettings.Instance.DataSamplerConfig.DefaultHistoryLength; }
            set { GearsetSettings.Instance.DataSamplerConfig.DefaultHistoryLength = value; }
        }

        /// <summary>
        /// Raised when the user request plots to be cleared.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler Cleared;

        [InspectorIgnore]
        public List<String> HiddenPlots { get; internal set; }

        /// <summary>
        /// Clears all lines
        /// </summary>
        public void Clear()
        {
            if (Cleared != null)
                Cleared(this, EventArgs.Empty);
        }

        public PlotterConfig()
        {
            DefaultSize = new Vector2(100, 60);
            HiddenPlots = new List<string>();
        }
    }
}

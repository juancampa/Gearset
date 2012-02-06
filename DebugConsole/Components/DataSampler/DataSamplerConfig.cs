using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components
{
    /// <summary>
    /// This whole class is ignored by the inspector.
    /// </summary>
    [Serializable]
    public class DataSamplerConfig : GearConfig
    {
        public int DefaultHistoryLength { get; set; }

        public DataSamplerConfig()
        {
            DefaultHistoryLength = 60;
        }
    }
}

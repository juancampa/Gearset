using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components
{
    [Serializable]
    public class InspectorConfig : GearConfig
    {
        [InspectorIgnoreAttribute]
        public double Top { get; internal set; }
        [InspectorIgnoreAttribute]
        public double Left { get; internal set; }
        [InspectorIgnoreAttribute]
        public double Width { get; internal set; }
        [InspectorIgnoreAttribute]
        public double Height { get; internal set; }

        [InspectorIgnoreAttribute]
        public String SearchText
        {
            get { return searchText; }
            set { if (value == null) return; searchText = value; OnPropertyChanged("SearchText"); if (SearchTextChanged != null) SearchTextChanged(this, EventArgs.Empty); }
        }
        private String searchText;

        [InspectorIgnoreAttribute]
        public bool ModifiedOnly { get { return modifiedOnly; } set { modifiedOnly = value; if (ModifiedOnlyChanged != null) ModifiedOnlyChanged(this, EventArgs.Empty); } }
        [NonSerialized]
        private bool modifiedOnly;

        [field:NonSerialized]
        public event EventHandler ModifiedOnlyChanged;

        [field: NonSerialized]
        public event EventHandler SearchTextChanged;

        public InspectorConfig()
        {
            // Defaults
            Width = 430;
            Height = 600;
            searchText = String.Empty;
        }
    }
}

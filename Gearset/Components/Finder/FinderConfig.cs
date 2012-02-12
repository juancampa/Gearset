using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gearset.Components
{
    [Serializable]
    public class FinderConfig : GearConfig
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
            set { searchText = value; OnPropertyChanged("SearchText"); if (SearchTextChanged != null) SearchTextChanged(this, EventArgs.Empty); }
        }
        private String searchText;

        /// <summary>
        /// The function that will be called everytime the query string changes.
        /// </summary>
        public SearchFunction SearchFunction
        {
            get { return searchFunction; }
            set { searchFunction = value; OnPropertyChanged("SearchFunction"); }
        }
        [NonSerialized]
        private SearchFunction searchFunction;

        [field: NonSerialized]
        public event EventHandler SearchTextChanged;

        public FinderConfig()
        {
            // Defaults
            Width = 400;
            Height = 430;
            searchText = String.Empty;
        }
    }
}

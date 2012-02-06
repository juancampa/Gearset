using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Gearset.Components
{
    [Serializable]
    public class GearConfig : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event EventHandler<BooleanChangedEventArgs> EnabledChanged;
        [field: NonSerialized]
        public event EventHandler<BooleanChangedEventArgs> VisibleChanged;
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private bool enabled;
        private bool visible;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Gear"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [InspectorIgnoreAttribute]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value; 
                if (EnabledChanged != null)
                    EnabledChanged(this, new BooleanChangedEventArgs(value));
                OnPropertyChanged("Enabled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Gear"/> is visible.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [InspectorIgnoreAttribute]
        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value; 
                if (VisibleChanged != null)
                    VisibleChanged(this, new BooleanChangedEventArgs(visible));
                OnPropertyChanged("Visible");
            }
        }

        public GearConfig()
        {
            Enabled = true;
            Visible = true;
        }

        /// <summary>
        /// Call this method when a data-bound property changes so the UI gets notified.
        /// </summary>
        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class BooleanChangedEventArgs : EventArgs
    {
        public bool NewValue { get; set; }
        public BooleanChangedEventArgs(bool newValue)
        {
            NewValue = newValue;
        }
    }
}

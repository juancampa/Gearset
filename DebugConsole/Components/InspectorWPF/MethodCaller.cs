using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms.Integration;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace Gearset.Components.InspectorWPF
{
    public abstract class MethodCaller : INotifyPropertyChanged
    {
        public String Name { get; set; }
        public bool CallAutomatically { get; set; }
        protected MethodInfo methodInfo;

        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// The TreeViewNode which holds this node.
        /// </summary>
        public ItemsControl UIContainer
        {
            get
            {
                return _UIContainer;
            }
            set
            {
                if (value != null)
                {
                    value.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
                    value.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(value_MouseDoubleClick);
                    _UIContainer = value;
                }
            }
        }

        void value_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsReady)
                CallMethod();
        }

        /// <summary>
        /// Determines if all parameters have been set so the method
        /// can be invoked.
        /// </summary>
        public bool IsReady
        {
            get
            {
                bool isReady = true;
                foreach (var par in Parameters)
                    if (par.Parameter == null)
                        isReady = false;
                return isReady;
            }
        }

        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            foreach (var v in Parameters)
            {
                if (v.UIContainer == null)
                {
                    v.UIContainer = (ItemsControl)UIContainer.ItemContainerGenerator.ContainerFromItem(v);
                }
            }
        }
        private ItemsControl _UIContainer;

        protected MethodCaller(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            this.Parameters = new ObservableCollection<MethodParamContainer>();
            foreach (var par in methodInfo.GetParameters())
            {
                MethodParamContainer parameter = new MethodParamContainer() { ParameterInfo = par };
                parameter.PropertyChanged += new PropertyChangedEventHandler(parameter_PropertyChanged);
                Parameters.Add(parameter);
            }

        }

        void parameter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ParameterName")
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsReady"));
            }
        }

        /// <summary>
        /// Parameters for the invocation of the method. On a especific
        /// implementation of a Method caller, the CallMethod method
        /// should use a delegate to speed things up, and of course
        /// not use this list to pass parameters but still use it to expose
        /// it to the UI.
        /// </summary>
        public ObservableCollection<MethodParamContainer> Parameters { get; private set; }

        /// <summary>
        /// Calls the method 
        /// </summary>
        public abstract void CallMethod();
        /// <summary>
        /// If the methodCaller should be called every
        /// frame, this can be done in this 
        /// </summary>
        public virtual void Update()
        {
            if (CallAutomatically)
                CallMethod();
        }
    }
}

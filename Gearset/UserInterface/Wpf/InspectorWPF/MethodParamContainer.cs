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

namespace Gearset.Components.InspectorWPF
{
    public class MethodParamContainer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                    TreeViewItem node = value as TreeViewItem;
                    node.AllowDrop = true;
                    node.Drop += new System.Windows.DragEventHandler(node_Drop);
                    _UIContainer = value;
                }
            }
        }

        public String ParameterName
        {
            get { return (Parameter != null) ? Parameter.Name : "(no parameter set)"; }
        }

        public String ParameterType
        {
            get { return ParameterInfo.ParameterType.Name; }
        }

        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private ItemsControl _UIContainer;

        public ParameterInfo ParameterInfo { get; set; }
        public virtual InspectorNode Parameter
        {
            get { return parameter; }
            set { parameter = value; OnPropertyChanged("ParameterName"); }
        }
        private InspectorNode parameter;

        void node_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ParameterInfo.ParameterType))
            {
                Parameter = e.Data.GetData(ParameterInfo.ParameterType) as InspectorNode;
            }
        }
    }

    public class MethodParamContainer<T>
    {
        public T RealParameter;
        public Object Parameter
        {
            get { return RealParameter; }
            set { RealParameter = (T)value; }
        }
    }
}

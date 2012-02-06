using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gearset.Components
{
    public class CurveTreeNode : INotifyPropertyChanged
    {
        public virtual String Name { get; set; }

        protected bool areParentsVisible;
        public virtual bool AreParentsVisible {
            get { return areParentsVisible; } 
            set 
            { 
                var prevValue = areParentsVisible;
                areParentsVisible = value;
                if (prevValue != areParentsVisible)
                {
                    foreach (var child in Children)
                    {
                        child.AreParentsVisible = this.areParentsVisible && this.isVisible;
                    }
                    OnPropertyChanged("AreParentsVisible");
                }
            } 
        }

        protected bool isVisible;
        public virtual bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                foreach (var child in Children)
                {
                    child.AreParentsVisible = this.areParentsVisible && this.isVisible;
                }
                OnPropertyChanged("IsVisible");
            }
        }

        private bool isExpanded;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
                if (isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                if (isSelected)
                    IsExpanded = true;
                OnPropertyChanged("IsSelected");
            }
        }


        public CurveTreeNode Parent { get; private set; }
        private ObservableCollection<CurveTreeNode> children;
        public ObservableCollection<CurveTreeNode> Children { get { return children; } set { children = value; OnPropertyChanged("Children"); } }

        public CurveTreeNode(String name, CurveTreeNode parent)
            : this(parent)
        {
            this.Name = name;
        }

        /// <summary>
        /// Only to be used by subclasses that override the name property.
        /// </summary>
        protected CurveTreeNode(CurveTreeNode parent)
        {
            Parent = parent;
            Children = new ObservableCollection<CurveTreeNode>();
            isVisible = true;
            areParentsVisible = true;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}

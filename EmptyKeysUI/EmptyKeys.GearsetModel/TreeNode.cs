using System;
using System.Collections;
using System.Collections.Generic;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// Implements inspector tree data item
    /// </summary>
    public class TreeNode : BindableBase, ITreeDataItem
    {
        public string Name { get; set; }

        public bool IsEnabled { get { return true; } }        

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }

        public bool HasChildren { get { return Children.Count > 0; } }

        private List<TreeNode> _children;
        public List<TreeNode> Children
        {
            get { return _children; }
            set { SetProperty(ref _children, value); }
        }

        private List<TreeNode> _methods;
        public List<TreeNode> Methods
        {
            get { return _methods; }
            set { SetProperty(ref _methods, value); }
        }

        public TreeNode()
        {
            Name = "Tree Item " + Guid.NewGuid();

            Children = new List<TreeNode>();
            Methods = new List<TreeNode>();
        }

        /// <summary>
        /// Gets the children for tree view item
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetChildren()
        {
            return Children;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

using System.Collections.Generic;
using EmptyKeys.UserInterface.Mvvm;

namespace EmptyKeys.GearsetModel
{
    /// <summary>
    /// MVVM View Model for the Inspector Window.
    /// </summary>
    public class InspectorWindowViewModel : WindowViewModel
    {
        private List<TreeNode> _treeItems;
        public List<TreeNode> TreeItems
        {
            get { return _treeItems; }
            set { SetProperty(ref _treeItems, value); }
        }

        public InspectorWindowViewModel()
        {
            Title = "Inspector";

            TreeItems = new List<TreeNode>();
        }
    }
}

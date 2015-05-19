using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class VisualItemBase : UserControl
    {
        /// <summary>
        /// The InspectorTreeNode that this float spinner affetcs.
        /// </summary>
        public InspectorNode TreeNode
        {
            get
            {
                return (InspectorNode)GetValue(TreeNodeProperty);
            }
            set
            {
                SetValue(TreeNodeProperty, value);
            }
        }

        /// <summary>
        /// This will make the TreeNode stop calling UpdateUI on this
        /// node if we're expanded. Ussually used for the genericItem
        /// because the ToString() representation of a variable is 
        /// the same as watching it's children updating.
        /// </summary>
        public bool UpdateIfExpanded { get; set; }

        /// <summary>
        /// Defines a way to move the focus out of the
        /// textbox when enter is pressed.
        /// </summary>
        protected static readonly TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);

        /// <summary>
        /// Registers a dependency property as backing store for the FloatValue property
        /// </summary>
        public static readonly DependencyProperty TreeNodeProperty =
            DependencyProperty.Register("TreeNode", typeof(InspectorNode), typeof(VisualItemBase),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, TreeNodeChangedCallback));

        public static void TreeNodeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualItemBase item = d as VisualItemBase;
            if (item != null && item.TreeNode != null)
            {
                item.TreeNode.VisualItem = item;
                item.OnTreeNodeChanged();
            }
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public VisualItemBase()
        {
        }

        public virtual void OnTreeNodeChanged()
        {
        }

        /// <summary>
        /// Override this method and add logic to update
        /// the control to reflect the new value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void UpdateUI(Object value)
        {
            // Do nothing
        }

        /// <summary>
        /// Override this method and add logic to update
        /// the value of the variable of the treenode.
        /// </summary>
        public virtual void UpdateVariable()
        {
            // Do nothing
        }

    }
}

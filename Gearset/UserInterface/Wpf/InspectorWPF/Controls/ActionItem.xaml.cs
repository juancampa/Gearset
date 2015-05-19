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
    public partial class ActionItem : VisualItemBase
    {
        public ActionItem()
        {
            InitializeComponent();
            Button1.Click += new RoutedEventHandler(Button1_Click);
        }

         public void Button1_Click(object sender, RoutedEventArgs e)
        {
            // This will call the Setter that in turn will
            // call the method.
            if (TreeNode == null || TreeNode.Parent == null)
            {
                GearsetResources.Console.Log("Gearset", "Internal Gearset error, could not call method.");
                return;
            }
            try
            {
                if (TreeNode.Method.IsStatic)
                {
                    TreeNode.Method.Invoke(null, new Object[] {TreeNode.Parent.Property});
                }
                else
                {
                    Object returnValue = TreeNode.Method.Invoke(TreeNode.Parent.Property, null);
                    if (returnValue != null)
                        GearsetResources.Console.Inspect(this.TreeNode.Parent.Name + '.' + this.TreeNode.Name + "()", returnValue);
                }
            }
            catch (Exception ex)
            {
                 GearsetResources.Console.Log("Gearset", "Method threw {0}: {1}", ex.GetType(), ex.Message);
            }
        }

        public override void UpdateUI(Object value)
        {
        }

        public override void OnTreeNodeChanged()
        {
            Button1.Content = TreeNode.FriendlyName;
        }

    }
}

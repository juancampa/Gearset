using System.Windows;
using Microsoft.Xna.Framework;

namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class CurveItem : VisualItemBase
    {
        public CurveItem()
        {
            InitializeComponent();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            var curve = TreeNode.Property as Curve ?? new Curve();
            GearsetResources.Console.AddCurve(TreeNode.Name, curve);
            GearsetResources.Console.Bender.Show();
            GearsetResources.Console.Bender.Focus();
        }
    }
}

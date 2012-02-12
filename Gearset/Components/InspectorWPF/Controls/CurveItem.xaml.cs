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

        public override void UpdateUI(Object value)
        {
        }

        public override void UpdateVariable()
        {
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            Curve curve = this.TreeNode.Property as Curve;
            if (curve == null)
            {
                curve = new Curve();
            }
            GearsetResources.Console.AddCurve(this.TreeNode.Name, curve);
            GearsetResources.Console.Bender.Window.Show();
            GearsetResources.Console.Bender.Window.Focus();
        }

    }
}

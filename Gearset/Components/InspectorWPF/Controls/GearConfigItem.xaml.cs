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
    /// 
    /// </summary>
    public partial class GearConfigItem : VisualItemBase
    {
        bool isEventFake = false;
        public GearConfigItem()
        {
            InitializeComponent();
        }

        public override void UpdateUI(Object value)
        {
            GearConfig config = value as GearConfig;
            if (config != null)
            {
                isEventFake = true;
                ToggleButton.IsChecked = config.Enabled;
                isEventFake = false;
            }
        }

         public void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!isEventFake)
                UpdateVariable();
        }

         public void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!isEventFake)
                UpdateVariable();
        }

        public override void UpdateVariable()
        {
            ((GearConfig)TreeNode.Property).Enabled = ToggleButton.IsChecked.Value;
            ((GearConfig)TreeNode.Property).Visible = ToggleButton.IsChecked.Value;
        }

    }
}

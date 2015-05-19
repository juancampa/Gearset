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
    public partial class BoolButton : VisualItemBase
    {
        bool isEventFake = false;

        public BoolButton()
        {
            InitializeComponent();

            ToggleButton.Checked += new RoutedEventHandler(ToggleButton_Checked);
            ToggleButton.Unchecked += new RoutedEventHandler(ToggleButton_Unchecked);
            
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

        public override void UpdateUI(Object value)
        {
            isEventFake = true;
            ToggleButton.IsChecked = (bool)value;
            isEventFake = false;
        }

        public override void UpdateVariable()
        {
            TreeNode.Property = ToggleButton.IsChecked;
        }


    }
}

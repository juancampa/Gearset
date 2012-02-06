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
    public partial class GenericItem : VisualItemBase
    {
        public GenericItem()
        {
            InitializeComponent();
        }

        public override void UpdateUI(Object value)
        {
            if (value != null)
            {
                String text = value.ToString();
                this.TextBlock1.Text = text;
                this.TextBlock1.ToolTip = text;
            }
            else
            {
                TextBlock1.Text = "(null)";
            }
        }

    }
}

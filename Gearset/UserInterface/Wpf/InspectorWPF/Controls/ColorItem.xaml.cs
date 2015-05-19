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
using Microsoft.Xna.Framework.Graphics;
using System.IO;


namespace Gearset.Components.InspectorWPF
{
    /// <summary>
    /// Interaction logic for Spinner.xaml
    /// </summary>
    public partial class ColorItem : VisualItemBase
    {
        SolidColorBrush brush;
        public ColorItem()
        {
            InitializeComponent();
            brush = new SolidColorBrush(Colors.Transparent);
            FrontRect.Fill = brush;

            UpdateIfExpanded = true;
        }

        public override void UpdateUI(Object value)
        {
            Microsoft.Xna.Framework.Color color = (Microsoft.Xna.Framework.Color)value;

            brush.Color = Color.FromArgb(color.A, color.R, color.G, color.B);
            
        }

    }
}

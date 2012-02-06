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
    public partial class GenericItemOnList : UserControl
    {
        public GenericItemOnList()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(GenericItemOnList_DataContextChanged);
        }

        void GenericItemOnList_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBlock1.Text = (DataContext != null) ? DataContext.ToString() : "(null)";
        }
    }
}

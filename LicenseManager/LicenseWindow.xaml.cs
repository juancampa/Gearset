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
using System.Windows.Forms.Integration;

namespace Gearset.Component
{
    /// <summary>
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public event EventHandler LicenseChanged;
        public LicenseWindow()
        {
            InitializeComponent();
            ElementHost.EnableModelessKeyboardInterop(this);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

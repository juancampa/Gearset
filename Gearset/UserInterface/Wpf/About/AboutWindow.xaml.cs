using System.Windows;
using System.Windows.Forms.Integration;

namespace Gearset.UserInterface.Wpf.About
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            ElementHost.EnableModelessKeyboardInterop(this);
        }
    }
}

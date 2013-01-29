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
using Gearset;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace Gearset.Components
{
    /// <summary>
    /// Widget that is shown above the Game's titlebar.
    /// </summary>
    public partial class WidgetWindow : Window
    {
        public event EventHandler Event;
        public WidgetWindow()
        {
            WindowStyle = System.Windows.WindowStyle.ToolWindow;
            InitializeComponent();
            //System.Windows.Interop.WindowInteropHelper wih = new System.Windows.Interop.WindowInteropHelper(this);
            //wih.Owner = GearsetResources.GameWindow.Handle;

            this.Loaded += new RoutedEventHandler(XdtkWidget_Loaded);

        }

        #region Don't show in alt-tab
        public void XdtkWidget_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...    
        }
        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...    
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);
        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;        // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }
            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }
            return result;
        }
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);
        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }
        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            // Execute the action.
            Button button = (Button)e.Source;
            Action action = ((ActionItem)button.DataContext).Action;
            if (action != null)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    button.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 20, 20));
                    button.ToolTip = ex.Message;
                }
            }
        }



        public void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = e.Source as MenuItem;
            if (((String)item.Header) == "About")
            {
                GearsetResources.AboutWindow.Show();
                e.Handled = true;
            }
            else if (((String)item.Header) == "Support/Feature Request")
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://www.thecomplot.com/lib/support"));
                e.Handled = true;
            }
        }

    }
}

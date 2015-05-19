#if WPF
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Gearset.UserInterface.Wpf.About;

    namespace Gearset.UserInterface.Wpf.Widget
    {
        /// <summary>
        /// Widget that is shown above the Game's titlebar.
        /// </summary>
        public partial class WidgetWindow : IWindow
        {
            readonly AboutWindow _aboutWindow;

            public ListView ButtonList { get { return buttonList; } }

            public bool WasHiddenByGameMinimize { get; set; }

            public WidgetWindow()
            {
                WindowStyle = WindowStyle.ToolWindow;
            
                InitializeComponent();

                #if WINDOWS
                    Loaded += XdtkWidget_Loaded;
                #endif

                var versionFull = typeof(GearConsole).Assembly.GetName().Version.ToString();
          
                var productName = string.Empty;
                var copyright = string.Empty;
                var attributes = typeof(GearConsole).Assembly.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    var assemblyProduct = attribute as AssemblyProductAttribute;
                    if (assemblyProduct != null)
                        productName = assemblyProduct.Product;

                    var assemblyCopyright = attribute as AssemblyCopyrightAttribute;
                    if (assemblyCopyright != null)
                        copyright = assemblyCopyright.Copyright;
                }

                _aboutWindow = new AboutWindow
                {
                    DataContext = new AboutViewModel(productName + " v" + versionFull, copyright)
                };
                
            }

            #region Don't show in alt-tab
            #if WINDOWS
                public void XdtkWidget_Loaded(object sender, RoutedEventArgs e)
                {
                    var wndHelper = new WindowInteropHelper(this);
                    var exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
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
                    int error;
                    IntPtr result;        // Win32 SetWindowLong doesn't clear error on success
                    SetLastError(0);
                    if (IntPtr.Size == 4)
                    {
                        // use SetWindowLong
                        var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
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
            #endif
            #endregion

            public void Button_Click(object sender, RoutedEventArgs e)
            {
                // Execute the action.
                var button = (Button)e.Source;
                var action = ((WidgetViewModel.ActionItem)button.DataContext).Action;
                if (action == null)
                    return;

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

            public void ContextMenu_Click(object sender, RoutedEventArgs e)
            {
                var item = e.Source as MenuItem;
                if (item == null) 
                    return;

                switch (((String)item.Header))
                {
                    case "About":
                        _aboutWindow.Show();
                        e.Handled = true;
                        break;

                    case "Support/Feature Request":
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://www.thecomplot.com/lib/support"));
                        e.Handled = true;
                        break;
                }
            }
        }
    }
#endif
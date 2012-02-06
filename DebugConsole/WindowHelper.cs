using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace Gearset
{
    public class WindowHelper
    {
        public static void EnsureOnScreen(Window window)
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            //Rect boundsWpf = window.Bounds;
            Rectangle bounds = new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
            bool isVisible = false;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].WorkingArea.IntersectsWith(bounds))
                {
                    isVisible = true;
                    break;
                }
            }

            if (!isVisible)
            {
                window.Left = 0;
                window.Top = 0;
            }

        }
    }
}

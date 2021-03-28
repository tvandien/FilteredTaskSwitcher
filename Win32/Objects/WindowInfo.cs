using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FilteredTaskSwitcher.Win32.Objects
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string WindowDescription { get; set; }
        public System.Diagnostics.Process Process { get; set; }
        public Rectangle WindowRectangle { get; set; }
        public bool UnderCursor { get; set; }
        public bool IsVisible { get; set; }

        public bool IsRealWindow(IntPtr shellWindow)
        {
            if (Handle == shellWindow) return false;
            if (!IsVisible) return false;
            if (WindowDescription.Length == 0) return false;

            return true;
        }
    }
}

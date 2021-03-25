using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace FilteredTaskSwitcher.Win32
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string WindowDescription { get; set; }
        public System.Diagnostics.Process Process { get; set; }
        public Rectangle WindowRectangle { get; set; }
        public bool UnderMouse { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FilteredTaskSwitcher.Win32.Objects
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PSIZE
    {
        public int x;
        public int y;
    }
}

using FilteredTaskSwitcher.Win32.API;
using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilteredTaskSwitcher.Classes
{
    public class FilteredTaskSwitcherProcessName : IFilteredTaskSwitcher
    {
        public int InitialSelectedIndex { get; protected set; }

        private readonly List<string> processFilter;        

        public FilteredTaskSwitcherProcessName(List<string> processFilter)
        {
            this.processFilter = processFilter;
        }

        public void UpdatedFilteredWindowList(List<WindowInfo> windows, bool isReverseHotkey)
        {
            Process.RefreshWindowList(windows, processFilter, out IntPtr firstWindow);

            int direction = isReverseHotkey ? -1 : 1;
            InitialSelectedIndex = firstWindow == windows[0].Handle ? (windows.Count + direction) % windows.Count : 0;
        }
    }
}

using FilteredTaskSwitcher.Win32.API;
using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FilteredTaskSwitcher.Classes
{
    public class FilteredTaskSwitcherProcessName : IFilteredTaskSwitcher
    {
        public int InitialSelectedIndex { get; protected set; }

        private readonly string[] processFilter;        

        public FilteredTaskSwitcherProcessName(params string[] processFilter)
        {
            this.processFilter = processFilter;
        }

        public void UpdatedFilteredWindowList(List<WindowInfo> windows, bool isReverseHotkey)
        {
            Process.UpdateWindowList(windows, out IntPtr firstWindow);
            FilterWindowList(windows);

            int direction = isReverseHotkey ? -1 : 1;
            InitialSelectedIndex = firstWindow == windows[0].Handle ? (windows.Count + direction) % windows.Count : 0;
        }

        private void FilterWindowList(List<WindowInfo> windows)
        {
            if (processFilter == null || !processFilter.Any())
                return;

            windows.RemoveAll(a => !processFilter.Contains(a.Process.ProcessName));
        }
    }
}

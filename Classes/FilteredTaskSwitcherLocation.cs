using FilteredTaskSwitcher.Win32.API;
using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilteredTaskSwitcher.Classes
{
    public class FilteredTaskSwitcherLocation : IFilteredTaskSwitcher
    {
        public int InitialSelectedIndex { get; protected set; }

        public void UpdatedFilteredWindowList(List<WindowInfo> windows, bool isReverseHotkey)
        {
            Process.UpdateWindowList(windows, out IntPtr firstWindow);
            FilterWindowList(windows);

            if (!windows.Any())
                return;

            int direction = isReverseHotkey ? -1 : 1;
            InitialSelectedIndex = firstWindow == windows[0].Handle ? (windows.Count + direction) % windows.Count : 0;
        }

        private void FilterWindowList(List<WindowInfo> windows)
        {
            windows.RemoveAll(a => !a.UnderCursor);
        }
    }
}

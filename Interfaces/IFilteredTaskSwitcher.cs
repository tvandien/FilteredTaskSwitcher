using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilteredTaskSwitcher.Classes
{
    public interface IFilteredTaskSwitcher
    {
        void UpdatedFilteredWindowList(List<WindowInfo> windows, bool isReverseHotkey);

        int InitialSelectedIndex { get; }
    }
}

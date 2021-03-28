using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FilteredTaskSwitcher.Classes
{
    public class FilteredTaskSwitcherLocation : IFilteredTaskSwitcher
    {
        public int InitialSelectedIndex => throw new NotImplementedException();

        public bool IsReverseHotkey => throw new NotImplementedException();

        public List<WindowInfo> GetFilteredWindows()
        {
            throw new NotImplementedException();
        }

        public void UpdatedFilteredWindowList(List<WindowInfo> windows, bool isReverseHotkey)
        {
            throw new NotImplementedException();
        }
    }
}

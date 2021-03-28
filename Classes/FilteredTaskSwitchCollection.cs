using FilteredTaskSwitcher.Win32;
using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace FilteredTaskSwitcher.Classes
{
    public class HotkeyInfo
    {
        public bool IsReverseHotkey { get; set; }
        public IFilteredTaskSwitcher TaskSwitcher { get; set; }

        public HotkeyInfo(IFilteredTaskSwitcher taskSwitcher, bool isReverseHotkey)
        {
            TaskSwitcher = taskSwitcher;
            IsReverseHotkey = isReverseHotkey;
        }
    }

    public class FilteredTaskSwitchCollection
    {
        private readonly Dictionary<int, HotkeyInfo> hotkeyHandleToHotkeyInfo = new Dictionary<int, HotkeyInfo>();

        public void AddTaskSwitcher(IFilteredTaskSwitcher taskSwitcher, RegisteredHotkey registeredHotkey)
        {
            hotkeyHandleToHotkeyInfo.Add(registeredHotkey.HotkeyHandle, new HotkeyInfo(taskSwitcher, false));
            hotkeyHandleToHotkeyInfo.Add(registeredHotkey.ReverseHotkeyHandle, new HotkeyInfo(taskSwitcher, true));
        }

        public void GetWindowInfoListForHotkeyHandle(int hotkeyHandle, List<WindowInfo> windows, out int initialSelectedIndex)
        {
            var hotkeyInfo = hotkeyHandleToHotkeyInfo[hotkeyHandle] ?? throw new Exception($"Unknown hotkey handle: {hotkeyHandle}");
            var taskSwitcher = hotkeyInfo.TaskSwitcher;

            taskSwitcher.UpdatedFilteredWindowList(windows, hotkeyInfo.IsReverseHotkey);
            initialSelectedIndex = taskSwitcher.InitialSelectedIndex;
        }

        public int GetDirectionForHotkeyHandle(int hotkeyHandle)
        {
            var hotkeyInfo = hotkeyHandleToHotkeyInfo[hotkeyHandle] ?? throw new Exception($"Unknown hotkey handle: {hotkeyHandle}");

            return hotkeyInfo.IsReverseHotkey ? -1 : 1;
        }
    }
}

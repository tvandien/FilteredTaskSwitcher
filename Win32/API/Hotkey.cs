using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FilteredTaskSwitcher.Win32.API
{
    public class Hotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static readonly VirtualKeyboardModifiers startModifier = VirtualKeyboardModifiers.ALT;
        private static readonly VirtualKeyboardModifiers reverseModifier = VirtualKeyboardModifiers.SHIFT;
        private static readonly List<int> hotkeyHandles = new List<int>();

        public static RegisteredHotkey RegisterHotkey(IntPtr handle, VirtualKeyboard hotkey)
        {
            var registeredHotkey = new RegisteredHotkey() {
                HotkeyHandle = GetNewHotkeyHandle(),
                ReverseHotkeyHandle = GetNewHotkeyHandle()
            };

            RegisterHotKey(handle, registeredHotkey.HotkeyHandle, (int)startModifier, (int)hotkey);
            RegisterHotKey(handle, registeredHotkey.ReverseHotkeyHandle, (int)startModifier | (int)reverseModifier, (int)hotkey);

            return registeredHotkey;
        }

        public static void UnregisterHotkeys(IntPtr handle)
        {
            foreach (var hotkeyHandle in hotkeyHandles)
            {
                UnregisterHotKey(handle, hotkeyHandle);
            }
        }

        private static int GetNewHotkeyHandle()
        {
            var newHotkey = hotkeyHandles.Any() ? hotkeyHandles.Max() + 1 : 1;

            hotkeyHandles.Add(newHotkey);

            return newHotkey;
        }
    }
}

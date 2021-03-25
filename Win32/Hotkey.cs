using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FilteredTaskSwitcher.Win32
{
    public class Hotkey
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void RegisterHotkeys(IntPtr handle)
        {
            RegisterHotKey(handle, (int)HotkeyHandles.BROWSER_SWITCH_PRESSED, (int)VirtualKeyboardModifiers.ALT, (int)VirtualKeyboard.F13);
            RegisterHotKey(handle, (int)HotkeyHandles.SHIFT_BROWSER_SWITCH_PRESSED, (int)(VirtualKeyboardModifiers.ALT | VirtualKeyboardModifiers.SHIFT), (int)VirtualKeyboard.F13);
            RegisterHotKey(handle, (int)HotkeyHandles.DEV_SWITCH_PRESSED, (int)VirtualKeyboardModifiers.ALT, (int)VirtualKeyboard.F14);
            RegisterHotKey(handle, (int)HotkeyHandles.SHIFT_DEV_SWITCH_PRESSED, (int)(VirtualKeyboardModifiers.ALT | VirtualKeyboardModifiers.SHIFT), (int)VirtualKeyboard.F14);
        }

        public static void UnregisterHotkeys(IntPtr handle)
        {
            Win32.Hotkey.UnregisterHotKey(handle, (int)HotkeyHandles.BROWSER_SWITCH_PRESSED);
            Win32.Hotkey.UnregisterHotKey(handle, (int)HotkeyHandles.SHIFT_BROWSER_SWITCH_PRESSED);
            Win32.Hotkey.UnregisterHotKey(handle, (int)HotkeyHandles.DEV_SWITCH_PRESSED);
            Win32.Hotkey.UnregisterHotKey(handle, (int)HotkeyHandles.SHIFT_DEV_SWITCH_PRESSED);
        }
    }
}

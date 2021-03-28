using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FilteredTaskSwitcher.Win32.API
{
    public class Process
    {

        [DllImport("user32.dll")]
        private static extern ushort GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);
        private delegate bool EnumWindowsProc(IntPtr handle, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr handle, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr handle, out Rectangle rectangle);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr handle, out uint lpdwProcessId);

        public static void UpdateWindowList(List<WindowInfo> windows, out IntPtr firstWindow)
        {
            windows.Clear();

            IntPtr shellWindow = GetShellWindow();
            GetCursorPos(out Point mousePosition);
            var processes = System.Diagnostics.Process.GetProcesses();

            IntPtr _firstWindow = IntPtr.Zero;

            EnumWindows(delegate (IntPtr handle, int lParam)
            {
                var windowInfo = GetWindowInfoFromHandle(handle, processes, mousePosition);
                if (!windowInfo.IsRealWindow(shellWindow)) return true;

                if (_firstWindow == IntPtr.Zero)
                    _firstWindow = handle;

                windows.Add(windowInfo);
                return true;

            }, 0);

            firstWindow = _firstWindow;
        }

        private static WindowInfo GetWindowInfoFromHandle(IntPtr handle, System.Diagnostics.Process[] processes, Point mousePosition)
        {
            int length = GetWindowTextLength(handle);

            StringBuilder windowText = new StringBuilder(length);
            GetWindowText(handle, windowText, length + 1);

            GetWindowThreadProcessId(handle, out uint pid);
            var process = processes.First(a => a.Id == pid);

            GetWindowRect(handle, out Rectangle windowRectangle);
            var underCursor = PointInRectangle(mousePosition, windowRectangle);

            return new WindowInfo()
            {
                Handle = handle,
                Process = process,
                WindowDescription = windowText.ToString(),
                UnderCursor = underCursor,
                WindowRectangle = windowRectangle,
                IsVisible = IsWindowVisible(handle)
            };
        }

        public static void SetFocus(IntPtr handle)
        {
            ShowWindow(handle, 1);
            SetForegroundWindow(handle);
        }

        public static bool IsKeyPressed(VirtualKeyboard keycode)
        {
            // GetAsyncKeyState returns a ushort. 
            // The most significant bit of this ushort reflects the state of the button.
            ushort keyState = GetAsyncKeyState((int)keycode);
            ushort mostSignificantBitForUshort = 32768;
            return (keyState & mostSignificantBitForUshort) > 0;
        }

        private static bool PointInRectangle(Point p, Rectangle r)
        {
            return p.X >= r.X &&
                p.X <= r.X + r.Width &&
                p.Y >= r.Y &&
                p.Y <= r.Y + r.Height;
        }
    }
}

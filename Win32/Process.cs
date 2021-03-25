using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace FilteredTaskSwitcher.Win32
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

        public static void RefreshWindowList(List<WindowInfo> windows, List<string> filter, out IntPtr firstWindow)
        {
            windows.Clear();

            IntPtr shellWindow = GetShellWindow();
            GetCursorPos(out Point mousePosition);
            var processes = System.Diagnostics.Process.GetProcesses();

            IntPtr _firstWindow = IntPtr.Zero;

            EnumWindows(delegate (IntPtr handle, int lParam)
            {
                if (handle == shellWindow) return true;
                if (!IsWindowVisible(handle)) return true;

                int length = GetWindowTextLength(handle);
                if (length == 0) return true;

                StringBuilder windowText = new StringBuilder(length);
                GetWindowText(handle, windowText, length + 1);

                GetWindowThreadProcessId(handle, out uint pid);

                var process = processes.First(a => a.Id == pid);
                var fileName = process.ProcessName;

                if (_firstWindow == IntPtr.Zero)
                    _firstWindow = handle;

                if (filter.Any() && !filter.Contains(fileName)) return true;

                GetWindowRect(handle, out Rectangle windowRectangle);

                windows.Add(new WindowInfo()
                {
                    Handle = handle,
                    Process = process,
                    WindowDescription = windowText.ToString(),
                    WindowRectangle = windowRectangle,
                    UnderMouse = PointInRectangle(mousePosition, windowRectangle)
                });
                return true;

            }, 0);

            firstWindow = _firstWindow;
        }

        public static void SetFocus(IntPtr handle)
        {
            ShowWindow(handle, 1);
            SetForegroundWindow(handle);
        }

        public static ushort GetAsyncKeyState(VirtualKeyboard virtualKeyboard)
        {
            return GetAsyncKeyState((int)virtualKeyboard);
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

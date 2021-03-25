using FilteredTaskSwitcher.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilteredTaskSwitcher
{
    public partial class AltTabUI : Form
    {
        private readonly List<WindowInfo> windows = new List<WindowInfo>();
        private readonly System.Timers.Timer timer = new System.Timers.Timer();

        private readonly List<string> BrowserFilter = new List<string>() { "firefox", "chrome" };
        private readonly List<string> DevFilter = new List<string>() { "devenv", "idea64" };

        private const int WM_HOTKEY = 0x0312;

        private const int previewSize = 200;
        private const int previewTextHeight = 32;
        private const int previewSpacing = 10;

        private delegate void HideFormCallback();
        private bool FirstRender = true;
        private int SelectedIndex = 0;

        public AltTabUI()
        {
            InitializeComponent();
            HideForm();

            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 50;

            Hotkey.RegisterHotkeys(Handle);
        }

        private void AltUp()
        {
            Reset();

            HideForm();

            if (SelectedIndex >= 0 && SelectedIndex < windows.Count)
            {
                Win32.Process.SetFocus(windows[SelectedIndex].Handle);
            }
        }

        private void Reset()
        {
            FirstRender = true;
            Win32.DWM.UnregisterThumbnails();
        }

        private void UpdateList(int direction, List<string> filter)
        {
            timer.Start();

            if (FirstRender)
            {
                Win32.Process.RefreshWindowList(windows, filter, out IntPtr firstWindow);

                if (!windows.Any())
                {
                    return;
                }

                // If the first window in the filter is the window with focus, select the second entry in the list. 
                // Otherwise, select the first.
                SelectedIndex = firstWindow == windows[0].Handle ? (windows.Count + direction) % windows.Count : 0;

                ShowList();
                ShowPreviews();

                FirstRender = false;
            }
            else
            {
                SelectedIndex = (windows.Count + SelectedIndex + direction) % windows.Count;
                lbWindows.SelectedIndex = SelectedIndex;
            }

            DrawSelection(SelectedIndex);
        }

        private void ShowPreviews()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                var rectangle = GetRectForIndex(i);
                Win32.DWM.RegisterThumbnails(window.Handle, Handle, rectangle);
            }
        }

        private void DrawSelection(int index)
        {
            Refresh();

            Graphics graphics = Graphics.FromHwnd(Handle);

            graphics.DrawRectangle(Pens.LightGray, GetRectForIndex(index).ToBoundingRectangle());

            for (int i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                var rectangle = GetRectForIndex(i);
                try
                {
                    Icon icon = Icon.ExtractAssociatedIcon(window.Process.MainModule.FileName);
                    graphics.DrawIcon(
                        icon,
                        new Rectangle(
                            rectangle.Left,
                            rectangle.Top - previewTextHeight,
                            previewTextHeight,
                            previewTextHeight));
                } catch (Exception)
                {
                    // This happens for elevated processes. If you want to see icons for these processes, try running this app as admin.
                }

                graphics.DrawString(
                    window.WindowDescription,
                    new Font("Segou UI", 11),
                    Brushes.LightGray,
                    new Rectangle(
                        rectangle.Left + previewTextHeight,
                        rectangle.Top - previewTextHeight,
                        rectangle.Width - previewTextHeight,
                        previewTextHeight)
                    );
            }
        }

        private Win32.Rect GetRectForIndex(int index)
        {
            int previewsPerRow = (Size.Width - previewSpacing) / (previewSize + previewSpacing);
            int x = index % previewsPerRow;
            int y = index / previewsPerRow;
            int pixelsPerPreviewWidth = previewSize + previewSpacing;
            int pixelsPerPreviewHeight = previewSize + previewTextHeight + previewSpacing;

            return new Win32.Rect(
                previewSpacing + x * pixelsPerPreviewWidth,
                previewTextHeight + previewSpacing + y * pixelsPerPreviewHeight,
                previewSpacing + x * pixelsPerPreviewWidth + previewSize,
                previewTextHeight + previewSpacing + y * pixelsPerPreviewHeight + previewSize);
        }

        private int GetIndexForMouseCoordinates(int x, int y)
        {
            int previewsPerRow = (Size.Width - previewSpacing) / (previewSize + previewSpacing);

            int xIndex = x - previewSpacing;
            xIndex /= (previewSize + previewSpacing);

            int yIndex = y - previewSpacing;
            yIndex /= (previewSize + previewSpacing + previewTextHeight);

            return xIndex + yIndex * previewsPerRow;
            
        }

        private void ShowList()
        {
            lbWindows.Items.Clear();
            foreach (var window in windows)
            {
                lbWindows.Items.Add(window.WindowDescription);
            }

            lbWindows.SelectedIndex = SelectedIndex;

            ShowForm();
            Location = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - (Size.Width / 2), Screen.PrimaryScreen.Bounds.Height / 2 - (Size.Height / 2));
            Win32.Process.SetFocus(this.Handle);
        }

        private void ShowForm()
        {
            Show();
        }

        private void HideForm()
        {
            if (InvokeRequired)
            {
                Invoke(new HideFormCallback(HideForm));
            }
            else
            {
                Hide();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                switch ((HotkeyHandles)m.WParam.ToInt32())
                {
                    case HotkeyHandles.BROWSER_SWITCH_PRESSED:
                        UpdateList(1, (BrowserFilter));
                        break;
                    case HotkeyHandles.SHIFT_BROWSER_SWITCH_PRESSED:
                        UpdateList(-1, (BrowserFilter));
                        break;
                    case HotkeyHandles.DEV_SWITCH_PRESSED:
                        UpdateList(1, (DevFilter));
                        break;
                    case HotkeyHandles.SHIFT_DEV_SWITCH_PRESSED:
                        UpdateList(-1, (DevFilter));
                        break;
                    default:
                        throw new Exception("Unknown hotkey handle triggered");
                }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ushort keyState = Win32.Process.GetAsyncKeyState(VirtualKeyboard.ALT);

            // This silly api call uses the most significant bit to indicate if a key is pressed, 
            // and the least significant bit for state changes. So yeah. 1 is okay, 0 is okay.
            if (keyState < 2)
            {
                timer.Stop();
                AltUp();
            }
        }

        private void AltTabUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hotkey.UnregisterHotkeys(Handle);
        }

        private void LbWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedIndex = lbWindows.SelectedIndex;
            DrawSelection(SelectedIndex);
        }

        private void AltTabUI_Shown(object sender, EventArgs e)
        {
            HideForm();
        }

        private void AltTabUI_MouseClick(object sender, MouseEventArgs e)
        {
            int index = GetIndexForMouseCoordinates(e.X, e.Y);

            if (index == -1 || index >= windows.Count)
            {
                return;
            }

            SelectedIndex = index;
            lbWindows.SelectedIndex = index;

            DrawSelection(index);
        }
    }
}

using FilteredTaskSwitcher.Classes;
using FilteredTaskSwitcher.Win32.API;
using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilteredTaskSwitcher.Forms
{
    public partial class TaskSwitch : Form
    {
        private readonly List<WindowInfo> windows = new List<WindowInfo>();
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private readonly FilteredTaskSwitchCollection taskSwitchers = new FilteredTaskSwitchCollection();

        private const int WM_HOTKEY = 0x0312;

        private const int previewSize = 200;
        private const int previewTextHeight = 32;
        private const int previewSpacing = 10;

        private delegate void HideFormCallback();
        private bool InitialKeypress = true;
        private int SelectedIndex = 0;

        public TaskSwitch()
        {
            InitializeComponent();
            HideForm();
            timer = GetTimer();
            InitializeTaskSwitchers();
        }

        private void InitializeTaskSwitchers()
        {
            var browserHotkeys = Hotkey.RegisterHotkey(Handle, VirtualKeyboard.F13);
            var devHotkeys = Hotkey.RegisterHotkey(Handle, VirtualKeyboard.F14);

            taskSwitchers.AddTaskSwitcher(
                new FilteredTaskSwitcherProcessName(new List<string>() { "firefox", "chrome" }), 
                browserHotkeys
                );

            taskSwitchers.AddTaskSwitcher(
                new FilteredTaskSwitcherProcessName(new List<string>() { "devenv", "idea64" }),
                devHotkeys
                );
        }

        private System.Timers.Timer GetTimer()
        {
            var timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 50;

            return timer;
        }

        private void AltUp()
        {
            Reset();

            HideForm();

            if (SelectedIndex >= 0 && SelectedIndex < windows.Count)
            {
                Process.SetFocus(windows[SelectedIndex].Handle);
            }
        }

        private void Reset()
        {
            InitialKeypress = true;
            DWM.UnregisterThumbnails();
        }

        private void UpdateSelection(int direction)
        {
            if (InitialKeypress)
            {
                timer.Start();

                if (!windows.Any())
                {
                    return;
                }

                ShowList();
                DrawPreviews();
                Refresh();

                InitialKeypress = false;
            }
            else
            {
                SelectedIndex = (windows.Count + SelectedIndex + direction) % windows.Count;
                lbWindows.SelectedIndex = SelectedIndex;
            }
        }

        private void DrawPreviews()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                var rectangle = GetRectForIndex(i);
                DWM.RegisterThumbnails(window.Handle, Handle, rectangle);
            }
        }

        private void DrawSelection(int index)
        {
            Graphics graphics = Graphics.FromHwnd(Handle);

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
                }
                catch (Exception)
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

                Pen pen = (index == i) ? Pens.LightGray : Pens.Black;
                graphics.DrawRectangle(pen, GetRectForIndex(i).ToBoundingRectangle());
            }
        }

        private Rect GetRectForIndex(int index)
        {
            int previewsPerRow = (Size.Width - previewSpacing) / (previewSize + previewSpacing);
            int x = index % previewsPerRow;
            int y = index / previewsPerRow;
            int pixelsPerPreviewWidth = previewSize + previewSpacing;
            int pixelsPerPreviewHeight = previewSize + previewTextHeight + previewSpacing;

            return new Rect(
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
            Process.SetFocus(this.Handle);
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
                if (InitialKeypress)
                {
                    taskSwitchers.GetWindowInfoListForHotkeyHandle(m.WParam.ToInt32(), windows, out SelectedIndex);
                }

                int direction = taskSwitchers.GetDirectionForHotkeyHandle(m.WParam.ToInt32());
                UpdateSelection(direction);
                DrawSelection(SelectedIndex);
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Process.IsKeyPressed(VirtualKeyboard.ALT))
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

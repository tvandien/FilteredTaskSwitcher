using FilteredTaskSwitcher.Forms;
using FilteredTaskSwitcher.Win32.API;
using FilteredTaskSwitcher.Win32.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilteredTaskSwitcher.Classes
{
    public class WindowInfoRenderer
    {
        private readonly TaskSwitch TaskSwitchForm;
        private readonly System.Timers.Timer timer = new System.Timers.Timer();
        private readonly List<WindowInfo> windows = new List<WindowInfo>();
        private readonly FilteredTaskSwitchCollection taskSwitchers = new FilteredTaskSwitchCollection();

        private const int previewSize = 200;
        private const int previewTextHeight = 32;
        private const int previewSpacing = 10;

        private bool InitialKeypress = true;
        private int SelectedIndex = 0;

        public WindowInfoRenderer(TaskSwitch form)
        {
            ConfigureTimer();
            TaskSwitchForm = form;
        }

        public void AddTaskSwitcher(IFilteredTaskSwitcher taskSwitcher, RegisteredHotkey registeredHotkey)
        {
            taskSwitchers.AddTaskSwitcher(taskSwitcher, registeredHotkey);
        }

        public void HandleHotkey(int hotkeyHandle)
        {
            if (InitialKeypress)
            {
                taskSwitchers.GetWindowInfoListForHotkeyHandle(hotkeyHandle, windows, out SelectedIndex);
            }

            int direction = taskSwitchers.GetDirectionForHotkeyHandle(hotkeyHandle);
            UpdateSelection(direction);
            DrawSelection(SelectedIndex);
        }

        public void UpdateSelectedIndexFromListbox()
        {
            SelectedIndex = TaskSwitchForm.lbWindows.SelectedIndex;
            DrawSelection(SelectedIndex);
        }

        public void SetSelectedIndexFromClick(int mouseX, int mouseY)
        {
            int index = GetIndexForMouseCoordinates(mouseX, mouseY);

            if (index == -1 || index >= windows.Count)
            {
                return;
            }

            SelectedIndex = index;
            TaskSwitchForm.lbWindows.SelectedIndex = index;

            DrawSelection(index);
        }

        private void ConfigureTimer()
        {
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 50;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Process.IsKeyPressed(VirtualKeyboard.ALT))
            {
                timer.Stop();
                HotkeyReleased();
            }
        }

        private void HotkeyReleased()
        {
            Reset();

            TaskSwitchForm.HideForm();

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
                TaskSwitchForm.Refresh();

                InitialKeypress = false;
            }
            else
            {
                SelectedIndex = (windows.Count + SelectedIndex + direction) % windows.Count;
                TaskSwitchForm.lbWindows.SelectedIndex = SelectedIndex;
            }
        }

        private void DrawPreviews()
        {
            for (int i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                var rectangle = GetRectForIndex(i);
                DWM.RegisterThumbnails(window.Handle, TaskSwitchForm.Handle, rectangle);
            }
        }

        private void DrawSelection(int index)
        {
            Graphics graphics = Graphics.FromHwnd(TaskSwitchForm.Handle);

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
            int previewsPerRow = (TaskSwitchForm.Size.Width - previewSpacing) / (previewSize + previewSpacing);
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
            int previewsPerRow = (TaskSwitchForm.Size.Width - previewSpacing) / (previewSize + previewSpacing);

            int xIndex = x - previewSpacing;
            xIndex /= (previewSize + previewSpacing);

            int yIndex = y - previewSpacing;
            yIndex /= (previewSize + previewSpacing + previewTextHeight);

            return xIndex + yIndex * previewsPerRow;
        }

        private void ShowList()
        {
            TaskSwitchForm.lbWindows.Items.Clear();
            foreach (var window in windows)
            {
                TaskSwitchForm.lbWindows.Items.Add(window.WindowDescription);
            }

            TaskSwitchForm.lbWindows.SelectedIndex = SelectedIndex;

            TaskSwitchForm.ShowForm();
            var center = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
            TaskSwitchForm.Location = new Point(center.X - (TaskSwitchForm.Size.Width / 2), center.Y - (TaskSwitchForm.Size.Height / 2));
            Process.SetFocus(TaskSwitchForm.Handle);
        }
    }
}

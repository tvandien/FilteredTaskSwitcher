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
        private readonly WindowInfoRenderer windowInfoRenderer;

        private const int WM_HOTKEY = 0x0312;

        private delegate void HideFormCallback();

        public TaskSwitch()
        {
            InitializeComponent();
            HideForm();
            windowInfoRenderer = new WindowInfoRenderer(this);
            InitializeTaskSwitchers();
        }

        private void InitializeTaskSwitchers()
        {
            var browserHotkeys = Hotkey.RegisterHotkey(Handle, VirtualKeyboard.F13);
            var devHotkeys = Hotkey.RegisterHotkey(Handle, VirtualKeyboard.F14);
            var locationHotkeys = Hotkey.RegisterHotkey(Handle, VirtualKeyboard.F15);

            windowInfoRenderer.AddTaskSwitcher(
                new FilteredTaskSwitcherProcessName("firefox", "chrome"), 
                browserHotkeys
                );

            windowInfoRenderer.AddTaskSwitcher(
                new FilteredTaskSwitcherProcessName("devenv", "idea64"),
                devHotkeys
                );

            windowInfoRenderer.AddTaskSwitcher(
                new FilteredTaskSwitcherLocation(),
                locationHotkeys
                );
        }

        public void ShowForm()
        {
            Show();
        }

        public void HideForm()
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
                windowInfoRenderer.HandleHotkey(m.WParam.ToInt32());
            }
        }

        private void AltTabUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hotkey.UnregisterHotkeys(Handle);
        }

        private void LbWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            windowInfoRenderer.UpdateSelectedIndexFromListbox();
        }

        private void AltTabUI_Shown(object sender, EventArgs e)
        {
            HideForm();
        }

        private void AltTabUI_MouseClick(object sender, MouseEventArgs e)
        {
            windowInfoRenderer.SetSelectedIndexFromClick(e.X, e.Y);
        }
    }
}

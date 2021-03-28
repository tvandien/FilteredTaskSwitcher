
namespace FilteredTaskSwitcher.Forms
{
    partial class TaskSwitch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbWindows = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lbWindows
            // 
            this.lbWindows.FormattingEnabled = true;
            this.lbWindows.ItemHeight = 15;
            this.lbWindows.Location = new System.Drawing.Point(13, 654);
            this.lbWindows.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lbWindows.Name = "lbWindows";
            this.lbWindows.Size = new System.Drawing.Size(824, 184);
            this.lbWindows.TabIndex = 0;
            this.lbWindows.SelectedIndexChanged += new System.EventHandler(this.LbWindows_SelectedIndexChanged);
            // 
            // AltTabUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(850, 850);
            this.ControlBox = false;
            this.Controls.Add(this.lbWindows);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AltTabUI";
            this.Opacity = 0.8D;
            this.Text = "AltTabConfig";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AltTabUI_FormClosing);
            this.Shown += new System.EventHandler(this.AltTabUI_Shown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AltTabUI_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbWindows;
    }
}
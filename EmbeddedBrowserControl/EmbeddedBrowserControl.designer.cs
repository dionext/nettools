namespace EmbeddedBrowser
{
    partial class EmbeddedBrowserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmbeddedBrowserControl));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printToPdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDevToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeDevToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copySourceToClipBoardAsyncMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCurrentCookiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGlobalCookiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browserTabControl = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTabToolStripMenuItem,
            this.closeTabToolStripMenuItem,
            this.printToolStripMenuItem,
            this.printToPdfToolStripMenuItem,
            this.showDevToolsMenuItem,
            this.closeDevToolsMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // newTabToolStripMenuItem
            // 
            resources.ApplyResources(this.newTabToolStripMenuItem, "newTabToolStripMenuItem");
            this.newTabToolStripMenuItem.Name = "newTabToolStripMenuItem";
            this.newTabToolStripMenuItem.Click += new System.EventHandler(this.NewTabToolStripMenuItemClick);
            // 
            // closeTabToolStripMenuItem
            // 
            resources.ApplyResources(this.closeTabToolStripMenuItem, "closeTabToolStripMenuItem");
            this.closeTabToolStripMenuItem.Name = "closeTabToolStripMenuItem";
            this.closeTabToolStripMenuItem.Click += new System.EventHandler(this.CloseTabToolStripMenuItemClick);
            // 
            // printToolStripMenuItem
            // 
            resources.ApplyResources(this.printToolStripMenuItem, "printToolStripMenuItem");
            this.printToolStripMenuItem.Name = "printToolStripMenuItem";
            this.printToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItemClick);
            // 
            // printToPdfToolStripMenuItem
            // 
            resources.ApplyResources(this.printToPdfToolStripMenuItem, "printToPdfToolStripMenuItem");
            this.printToPdfToolStripMenuItem.Name = "printToPdfToolStripMenuItem";
            this.printToPdfToolStripMenuItem.Click += new System.EventHandler(this.PrintToPdfToolStripMenuItemClick);
            // 
            // showDevToolsMenuItem
            // 
            resources.ApplyResources(this.showDevToolsMenuItem, "showDevToolsMenuItem");
            this.showDevToolsMenuItem.Name = "showDevToolsMenuItem";
            this.showDevToolsMenuItem.Click += new System.EventHandler(this.ShowDevToolsMenuItemClick);
            // 
            // closeDevToolsMenuItem
            // 
            resources.ApplyResources(this.closeDevToolsMenuItem, "closeDevToolsMenuItem");
            this.closeDevToolsMenuItem.Name = "closeDevToolsMenuItem";
            this.closeDevToolsMenuItem.Click += new System.EventHandler(this.CloseDevToolsMenuItemClick);
            // 
            // editToolStripMenuItem
            // 
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            this.findMenuItem,
            this.toolStripMenuItem2,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.deleteMenuItem,
            this.selectAllMenuItem,
            this.toolStripSeparator1,
            this.copySourceToClipBoardAsyncMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            // 
            // undoMenuItem
            // 
            resources.ApplyResources(this.undoMenuItem, "undoMenuItem");
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.Click += new System.EventHandler(this.UndoMenuItemClick);
            // 
            // redoMenuItem
            // 
            resources.ApplyResources(this.redoMenuItem, "redoMenuItem");
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItemClick);
            // 
            // findMenuItem
            // 
            resources.ApplyResources(this.findMenuItem, "findMenuItem");
            this.findMenuItem.Name = "findMenuItem";
            this.findMenuItem.Click += new System.EventHandler(this.FindMenuItemClick);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // cutMenuItem
            // 
            resources.ApplyResources(this.cutMenuItem, "cutMenuItem");
            this.cutMenuItem.Name = "cutMenuItem";
            this.cutMenuItem.Click += new System.EventHandler(this.CutMenuItemClick);
            // 
            // copyMenuItem
            // 
            resources.ApplyResources(this.copyMenuItem, "copyMenuItem");
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.Click += new System.EventHandler(this.CopyMenuItemClick);
            // 
            // pasteMenuItem
            // 
            resources.ApplyResources(this.pasteMenuItem, "pasteMenuItem");
            this.pasteMenuItem.Name = "pasteMenuItem";
            this.pasteMenuItem.Click += new System.EventHandler(this.PasteMenuItemClick);
            // 
            // deleteMenuItem
            // 
            resources.ApplyResources(this.deleteMenuItem, "deleteMenuItem");
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Click += new System.EventHandler(this.DeleteMenuItemClick);
            // 
            // selectAllMenuItem
            // 
            resources.ApplyResources(this.selectAllMenuItem, "selectAllMenuItem");
            this.selectAllMenuItem.Name = "selectAllMenuItem";
            this.selectAllMenuItem.Click += new System.EventHandler(this.SelectAllMenuItemClick);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // copySourceToClipBoardAsyncMenuItem
            // 
            resources.ApplyResources(this.copySourceToClipBoardAsyncMenuItem, "copySourceToClipBoardAsyncMenuItem");
            this.copySourceToClipBoardAsyncMenuItem.Name = "copySourceToClipBoardAsyncMenuItem";
            this.copySourceToClipBoardAsyncMenuItem.Click += new System.EventHandler(this.CopySourceToClipBoardAsyncClick);
            // 
            // viewToolStripMenuItem
            // 
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.deleteCurrentCookiesToolStripMenuItem,
            this.deleteGlobalCookiesToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            // 
            // zoomInToolStripMenuItem
            // 
            resources.ApplyResources(this.zoomInToolStripMenuItem, "zoomInToolStripMenuItem");
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.ZoomInToolStripMenuItemClick);
            // 
            // zoomOutToolStripMenuItem
            // 
            resources.ApplyResources(this.zoomOutToolStripMenuItem, "zoomOutToolStripMenuItem");
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.ZoomOutToolStripMenuItemClick);
            // 
            // deleteCurrentCookiesToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteCurrentCookiesToolStripMenuItem, "deleteCurrentCookiesToolStripMenuItem");
            this.deleteCurrentCookiesToolStripMenuItem.Name = "deleteCurrentCookiesToolStripMenuItem";
            this.deleteCurrentCookiesToolStripMenuItem.Click += new System.EventHandler(this.deleteCurrentCookiesToolStripMenuItem_Click);
            // 
            // deleteGlobalCookiesToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteGlobalCookiesToolStripMenuItem, "deleteGlobalCookiesToolStripMenuItem");
            this.deleteGlobalCookiesToolStripMenuItem.Name = "deleteGlobalCookiesToolStripMenuItem";
            this.deleteGlobalCookiesToolStripMenuItem.Click += new System.EventHandler(this.deleteGlobalCookiesToolStripMenuItem_Click);
            // 
            // browserTabControl
            // 
            resources.ApplyResources(this.browserTabControl, "browserTabControl");
            this.browserTabControl.Name = "browserTabControl";
            this.browserTabControl.SelectedIndex = 0;
            // 
            // CefBrowserControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.browserTabControl);
            this.Controls.Add(this.menuStrip1);
            this.Name = "CefBrowserControl";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem showDevToolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem copySourceToClipBoardAsyncMenuItem;
        private System.Windows.Forms.TabControl browserTabControl;
        private System.Windows.Forms.ToolStripMenuItem newTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeDevToolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printToPdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCurrentCookiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteGlobalCookiesToolStripMenuItem;
    }
}
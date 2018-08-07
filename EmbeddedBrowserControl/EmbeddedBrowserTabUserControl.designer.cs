namespace EmbeddedBrowser
{
    partial class EmbeddedBrowserTabUserControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmbeddedBrowserTabUserControl));
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.findTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.findCloseButton = new System.Windows.Forms.ToolStripButton();
            this.statusLabel = new System.Windows.Forms.Label();
            this.outputLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.urlTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.browserPanel = new System.Windows.Forms.Panel();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.goButton = new System.Windows.Forms.ToolStripButton();
            this.zoomInButton = new System.Windows.Forms.ToolStripButton();
            this.zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.loginButton = new System.Windows.Forms.ToolStripButton();
            this.passwordButton = new System.Windows.Forms.ToolStripButton();
            this.findPreviousButton = new System.Windows.Forms.ToolStripButton();
            this.findNextButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findTextBox,
            this.findPreviousButton,
            this.findNextButton,
            this.findCloseButton});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // findTextBox
            // 
            this.findTextBox.Name = "findTextBox";
            resources.ApplyResources(this.findTextBox, "findTextBox");
            this.findTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindTextBoxKeyDown);
            // 
            // findCloseButton
            // 
            this.findCloseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.findCloseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.findCloseButton, "findCloseButton");
            this.findCloseButton.Name = "findCloseButton";
            this.findCloseButton.Click += new System.EventHandler(this.FindCloseButtonClick);
            // 
            // statusLabel
            // 
            resources.ApplyResources(this.statusLabel, "statusLabel");
            this.statusLabel.Name = "statusLabel";
            // 
            // outputLabel
            // 
            resources.ApplyResources(this.outputLabel, "outputLabel");
            this.outputLabel.Name = "outputLabel";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backButton,
            this.forwardButton,
            this.urlTextBox,
            this.goButton,
            this.zoomInButton,
            this.zoomOutButton,
            this.loginButton,
            this.passwordButton});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Stretch = true;
            this.toolStrip1.Layout += new System.Windows.Forms.LayoutEventHandler(this.HandleToolStripLayout);
            // 
            // urlTextBox
            // 
            resources.ApplyResources(this.urlTextBox, "urlTextBox");
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UrlTextBoxKeyUp);
            // 
            // browserPanel
            // 
            resources.ApplyResources(this.browserPanel, "browserPanel");
            this.browserPanel.Name = "browserPanel";
            // 
            // backButton
            // 
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.Image = global::EmbeddedBrowser.Properties.Resources.nav_left_green;
            this.backButton.Name = "backButton";
            this.backButton.Click += new System.EventHandler(this.BackButtonClick);
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.forwardButton, "forwardButton");
            this.forwardButton.Image = global::EmbeddedBrowser.Properties.Resources.nav_right_green;
            this.forwardButton.Name = "forwardButton";
            this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
            // 
            // goButton
            // 
            this.goButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.goButton.Image = global::EmbeddedBrowser.Properties.Resources.nav_plain_green;
            resources.ApplyResources(this.goButton, "goButton");
            this.goButton.Name = "goButton";
            this.goButton.Click += new System.EventHandler(this.GoButtonClick);
            // 
            // zoomInButton
            // 
            this.zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomInButton.Image = global::EmbeddedBrowser.Properties.Resources.zoominbox;
            resources.ApplyResources(this.zoomInButton, "zoomInButton");
            this.zoomInButton.Name = "zoomInButton";
            this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
            // 
            // zoomOutButton
            // 
            this.zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomOutButton.Image = global::EmbeddedBrowser.Properties.Resources.zoomoutbox;
            resources.ApplyResources(this.zoomOutButton, "zoomOutButton");
            this.zoomOutButton.Name = "zoomOutButton";
            this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
            // 
            // loginButton
            // 
            this.loginButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.loginButton, "loginButton");
            this.loginButton.Name = "loginButton";
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // passwordButton
            // 
            this.passwordButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.passwordButton.Image = global::EmbeddedBrowser.Properties.Resources.password;
            resources.ApplyResources(this.passwordButton, "passwordButton");
            this.passwordButton.Name = "passwordButton";
            this.passwordButton.Click += new System.EventHandler(this.passwordButton_Click);
            // 
            // findPreviousButton
            // 
            this.findPreviousButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findPreviousButton.Image = global::EmbeddedBrowser.Properties.Resources.nav_left_green;
            resources.ApplyResources(this.findPreviousButton, "findPreviousButton");
            this.findPreviousButton.Name = "findPreviousButton";
            this.findPreviousButton.Click += new System.EventHandler(this.FindPreviousButtonClick);
            // 
            // findNextButton
            // 
            this.findNextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.findNextButton.Image = global::EmbeddedBrowser.Properties.Resources.nav_right_green;
            resources.ApplyResources(this.findNextButton, "findNextButton");
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Click += new System.EventHandler(this.FindNextButtonClick);
            // 
            // CefBrowserTabUserControl
            // 
            this.Controls.Add(this.browserPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.toolStrip2);
            this.Name = "CefBrowserTabUserControl";
            resources.ApplyResources(this, "$this");
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripTextBox urlTextBox;
        private System.Windows.Forms.ToolStripButton goButton;
        private System.Windows.Forms.Label outputLabel;

        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton findPreviousButton;
        private System.Windows.Forms.ToolStripTextBox findTextBox;
        private System.Windows.Forms.ToolStripButton findNextButton;
        private System.Windows.Forms.ToolStripButton findCloseButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Panel browserPanel;
        private System.Windows.Forms.ToolStripButton loginButton;
        private System.Windows.Forms.ToolStripButton passwordButton;
        private System.Windows.Forms.ToolStripButton zoomInButton;
        private System.Windows.Forms.ToolStripButton zoomOutButton;
    }
}
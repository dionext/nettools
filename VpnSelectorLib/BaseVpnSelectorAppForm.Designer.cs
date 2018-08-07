namespace Dionext
{
    partial class BaseVpnSelectorAppForm
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
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // JustAppMainAppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 382);
            this.DocPanelBounds = new System.Drawing.Rectangle(19, 19, 643, 429);
            this.Name = "MainAppForm";
            this.Text = "MainAppForm";
            this.Load += new System.EventHandler(this.JustAppMainAppForm_Load);
            this.Controls.SetChildIndex(this.dockPanel, 0);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
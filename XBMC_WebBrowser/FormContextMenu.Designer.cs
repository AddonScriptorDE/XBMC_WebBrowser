namespace XBMC_WebBrowser
{
    partial class FormContextMenu
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
            this.listBoxMenu = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxMenu
            // 
            this.listBoxMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxMenu.BackColor = System.Drawing.Color.White;
            this.listBoxMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxMenu.FormattingEnabled = true;
            this.listBoxMenu.ItemHeight = 55;
            this.listBoxMenu.Location = new System.Drawing.Point(12, 12);
            this.listBoxMenu.Name = "listBoxMenu";
            this.listBoxMenu.Size = new System.Drawing.Size(476, 554);
            this.listBoxMenu.TabIndex = 1;
            this.listBoxMenu.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxMenu_KeyDown);
            // 
            // FormContextMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.listBoxMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormContextMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormContextMenu";
            this.TransparencyKey = System.Drawing.Color.Snow;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox listBoxMenu;
    }
}
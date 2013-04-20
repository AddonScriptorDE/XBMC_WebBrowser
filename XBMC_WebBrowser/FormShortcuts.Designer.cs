namespace XBMC_WebBrowser
{
    partial class FormShortcuts
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
            this.listBoxFavs = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBoxFavs
            // 
            this.listBoxFavs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxFavs.BackColor = System.Drawing.Color.White;
            this.listBoxFavs.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxFavs.FormattingEnabled = true;
            this.listBoxFavs.ItemHeight = 55;
            this.listBoxFavs.Location = new System.Drawing.Point(12, 12);
            this.listBoxFavs.Name = "listBoxFavs";
            this.listBoxFavs.Size = new System.Drawing.Size(476, 554);
            this.listBoxFavs.TabIndex = 1;
            this.listBoxFavs.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxFavs_KeyDown);
            // 
            // FormShortcuts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.ClientSize = new System.Drawing.Size(500, 600);
            this.Controls.Add(this.listBoxFavs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormShortcuts";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormShortcuts";
            this.TransparencyKey = System.Drawing.Color.Snow;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox listBoxFavs;
    }
}
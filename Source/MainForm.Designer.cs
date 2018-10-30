namespace RTSPRedirect
{
    partial class MainForm
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
            this.lblEnterRtspAddress = new System.Windows.Forms.Label();
            this.btnGetRedirectUrl = new System.Windows.Forms.Button();
            this.tbxRtspAddress = new System.Windows.Forms.TextBox();
            this.tbxRedirectUri = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.chbxPlayback = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEnterRtspAddress
            // 
            this.lblEnterRtspAddress.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.lblEnterRtspAddress, 3);
            this.lblEnterRtspAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEnterRtspAddress.Location = new System.Drawing.Point(3, 0);
            this.lblEnterRtspAddress.Name = "lblEnterRtspAddress";
            this.lblEnterRtspAddress.Size = new System.Drawing.Size(649, 13);
            this.lblEnterRtspAddress.TabIndex = 0;
            this.lblEnterRtspAddress.Text = "Enter RTSP Address:";
            // 
            // btnGetRedirectUrl
            // 
            this.btnGetRedirectUrl.Location = new System.Drawing.Point(269, 120);
            this.btnGetRedirectUrl.Name = "btnGetRedirectUrl";
            this.btnGetRedirectUrl.Size = new System.Drawing.Size(117, 23);
            this.btnGetRedirectUrl.TabIndex = 1;
            this.btnGetRedirectUrl.Text = "Get Redirect URL";
            this.btnGetRedirectUrl.UseVisualStyleBackColor = true;
            this.btnGetRedirectUrl.Click += new System.EventHandler(this.GetRedirectUrlButton_Click);
            // 
            // tbxRtspAddress
            // 
            this.tableLayoutPanel.SetColumnSpan(this.tbxRtspAddress, 3);
            this.tbxRtspAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxRtspAddress.Location = new System.Drawing.Point(3, 16);
            this.tbxRtspAddress.Multiline = true;
            this.tbxRtspAddress.Name = "tbxRtspAddress";
            this.tbxRtspAddress.Size = new System.Drawing.Size(649, 98);
            this.tbxRtspAddress.TabIndex = 2;
            // 
            // tbxRedirectUri
            // 
            this.tbxRedirectUri.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel.SetColumnSpan(this.tbxRedirectUri, 3);
            this.tbxRedirectUri.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbxRedirectUri.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxRedirectUri.Location = new System.Drawing.Point(3, 149);
            this.tbxRedirectUri.Multiline = true;
            this.tbxRedirectUri.Name = "tbxRedirectUri";
            this.tbxRedirectUri.ReadOnly = true;
            this.tbxRedirectUri.Size = new System.Drawing.Size(649, 98);
            this.tbxRedirectUri.TabIndex = 4;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.lblEnterRtspAddress, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.tbxRedirectUri, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.tbxRtspAddress, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.btnGetRedirectUrl, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.chbxPlayback, 0, 2);
            this.tableLayoutPanel.Location = new System.Drawing.Point(14, 12);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 4;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(655, 250);
            this.tableLayoutPanel.TabIndex = 5;
            // 
            // chbxPlayback
            // 
            this.chbxPlayback.AutoSize = true;
            this.chbxPlayback.Location = new System.Drawing.Point(3, 120);
            this.chbxPlayback.Name = "chbxPlayback";
            this.chbxPlayback.Size = new System.Drawing.Size(70, 17);
            this.chbxPlayback.TabIndex = 5;
            this.chbxPlayback.Text = "Playback";
            this.chbxPlayback.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 275);
            this.Controls.Add(this.tableLayoutPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "RTSP Redirect";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblEnterRtspAddress;
        private System.Windows.Forms.Button btnGetRedirectUrl;
        private System.Windows.Forms.TextBox tbxRtspAddress;
        public System.Windows.Forms.TextBox tbxRedirectUri;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.CheckBox chbxPlayback;
    }
}


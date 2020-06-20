namespace Luma.UI
{
    partial class frmMain
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
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.prgDownloadProgress = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lstDownloadLog = new System.Windows.Forms.ListBox();
            this.chkAutoScroll = new System.Windows.Forms.CheckBox();
            this.rdDownloadPlaylist = new System.Windows.Forms.RadioButton();
            this.rdDownloadVideo = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lblUrl
            // 
            this.lblUrl.AutoSize = true;
            this.lblUrl.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUrl.Location = new System.Drawing.Point(8, 9);
            this.lblUrl.Name = "lblUrl";
            this.lblUrl.Size = new System.Drawing.Size(40, 20);
            this.lblUrl.TabIndex = 0;
            this.lblUrl.Text = "URL:";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(54, 9);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(436, 20);
            this.txtUrl.TabIndex = 3;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(496, 7);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(87, 23);
            this.btnDownload.TabIndex = 4;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // prgDownloadProgress
            // 
            this.prgDownloadProgress.Location = new System.Drawing.Point(12, 280);
            this.prgDownloadProgress.MarqueeAnimationSpeed = 10;
            this.prgDownloadProgress.Name = "prgDownloadProgress";
            this.prgDownloadProgress.Size = new System.Drawing.Size(571, 23);
            this.prgDownloadProgress.Step = 1;
            this.prgDownloadProgress.TabIndex = 5;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(12, 309);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(282, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(301, 309);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(282, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(493, 37);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(58, 13);
            this.lblProgress.TabIndex = 13;
            this.lblProgress.Text = "lblProgress";
            // 
            // lstDownloadLog
            // 
            this.lstDownloadLog.BackColor = System.Drawing.Color.Beige;
            this.lstDownloadLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstDownloadLog.FormattingEnabled = true;
            this.lstDownloadLog.Location = new System.Drawing.Point(12, 58);
            this.lstDownloadLog.Name = "lstDownloadLog";
            this.lstDownloadLog.Size = new System.Drawing.Size(571, 212);
            this.lstDownloadLog.TabIndex = 14;
            this.lstDownloadLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstDownloadLog_DrawItem);
            this.lstDownloadLog.DoubleClick += new System.EventHandler(this.lstDownloadLog_DoubleClick);
            // 
            // chkAutoScroll
            // 
            this.chkAutoScroll.AutoSize = true;
            this.chkAutoScroll.Checked = true;
            this.chkAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoScroll.Location = new System.Drawing.Point(133, 36);
            this.chkAutoScroll.Name = "chkAutoScroll";
            this.chkAutoScroll.Size = new System.Drawing.Size(77, 17);
            this.chkAutoScroll.TabIndex = 15;
            this.chkAutoScroll.Text = "Auto Scroll";
            this.chkAutoScroll.UseVisualStyleBackColor = true;
            // 
            // rdDownloadPlaylist
            // 
            this.rdDownloadPlaylist.AutoSize = true;
            this.rdDownloadPlaylist.Checked = true;
            this.rdDownloadPlaylist.Location = new System.Drawing.Point(12, 35);
            this.rdDownloadPlaylist.Name = "rdDownloadPlaylist";
            this.rdDownloadPlaylist.Size = new System.Drawing.Size(57, 17);
            this.rdDownloadPlaylist.TabIndex = 12;
            this.rdDownloadPlaylist.TabStop = true;
            this.rdDownloadPlaylist.Text = "Playlist";
            this.rdDownloadPlaylist.UseVisualStyleBackColor = true;
            // 
            // rdDownloadVideo
            // 
            this.rdDownloadVideo.AutoSize = true;
            this.rdDownloadVideo.Location = new System.Drawing.Point(75, 35);
            this.rdDownloadVideo.Name = "rdDownloadVideo";
            this.rdDownloadVideo.Size = new System.Drawing.Size(52, 17);
            this.rdDownloadVideo.TabIndex = 11;
            this.rdDownloadVideo.Text = "Video";
            this.rdDownloadVideo.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 340);
            this.Controls.Add(this.chkAutoScroll);
            this.Controls.Add(this.lstDownloadLog);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.rdDownloadPlaylist);
            this.Controls.Add(this.rdDownloadVideo);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.prgDownloadProgress);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.lblUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Luma Youtube Playlist Downloader (Powered by YoutubeExplode)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUrl;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ProgressBar prgDownloadProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ListBox lstDownloadLog;
        private System.Windows.Forms.CheckBox chkAutoScroll;
        private System.Windows.Forms.RadioButton rdDownloadPlaylist;
        private System.Windows.Forms.RadioButton rdDownloadVideo;
    }
}


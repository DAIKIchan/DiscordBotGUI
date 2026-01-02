namespace DiscordBotGUI
{
    partial class VersionInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VersionInfo));
            this.LblProductName = new System.Windows.Forms.Label();
            this.LblVersion = new System.Windows.Forms.Label();
            this.GroupBox = new System.Windows.Forms.GroupBox();
            this.RtbComponentVersions = new System.Windows.Forms.RichTextBox();
            this.LnkLicenseStatus = new System.Windows.Forms.LinkLabel();
            this.GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblProductName
            // 
            this.LblProductName.AutoSize = true;
            this.LblProductName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblProductName.ForeColor = System.Drawing.Color.White;
            this.LblProductName.Location = new System.Drawing.Point(13, 9);
            this.LblProductName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblProductName.Name = "LblProductName";
            this.LblProductName.Size = new System.Drawing.Size(104, 21);
            this.LblProductName.TabIndex = 46;
            this.LblProductName.Text = "情報取得中...";
            // 
            // LblVersion
            // 
            this.LblVersion.AutoSize = true;
            this.LblVersion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblVersion.ForeColor = System.Drawing.Color.White;
            this.LblVersion.Location = new System.Drawing.Point(13, 49);
            this.LblVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblVersion.Name = "LblVersion";
            this.LblVersion.Size = new System.Drawing.Size(104, 21);
            this.LblVersion.TabIndex = 47;
            this.LblVersion.Text = "情報取得中...";
            // 
            // GroupBox
            // 
            this.GroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox.Controls.Add(this.RtbComponentVersions);
            this.GroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox.ForeColor = System.Drawing.Color.White;
            this.GroupBox.Location = new System.Drawing.Point(12, 144);
            this.GroupBox.Name = "GroupBox";
            this.GroupBox.Size = new System.Drawing.Size(460, 405);
            this.GroupBox.TabIndex = 49;
            this.GroupBox.TabStop = false;
            this.GroupBox.Text = "コンポーネント詳細";
            // 
            // RtbComponentVersions
            // 
            this.RtbComponentVersions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RtbComponentVersions.BackColor = System.Drawing.Color.DarkBlue;
            this.RtbComponentVersions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RtbComponentVersions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RtbComponentVersions.ForeColor = System.Drawing.Color.White;
            this.RtbComponentVersions.Location = new System.Drawing.Point(6, 22);
            this.RtbComponentVersions.Name = "RtbComponentVersions";
            this.RtbComponentVersions.ReadOnly = true;
            this.RtbComponentVersions.Size = new System.Drawing.Size(448, 377);
            this.RtbComponentVersions.TabIndex = 0;
            this.RtbComponentVersions.Text = "";
            this.RtbComponentVersions.WordWrap = false;
            // 
            // LnkLicenseStatus
            // 
            this.LnkLicenseStatus.AutoSize = true;
            this.LnkLicenseStatus.BackColor = System.Drawing.Color.DarkBlue;
            this.LnkLicenseStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LnkLicenseStatus.LinkColor = System.Drawing.Color.White;
            this.LnkLicenseStatus.Location = new System.Drawing.Point(13, 89);
            this.LnkLicenseStatus.Name = "LnkLicenseStatus";
            this.LnkLicenseStatus.Size = new System.Drawing.Size(104, 21);
            this.LnkLicenseStatus.TabIndex = 51;
            this.LnkLicenseStatus.TabStop = true;
            this.LnkLicenseStatus.Text = "情報取得中...";
            this.LnkLicenseStatus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LnkLicenseStatus_LinkClicked);
            // 
            // VersionInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.DarkBlue;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.LnkLicenseStatus);
            this.Controls.Add(this.GroupBox);
            this.Controls.Add(this.LblVersion);
            this.Controls.Add(this.LblProductName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "VersionInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "製品バージョン詳細";
            this.Activated += new System.EventHandler(this.VersionInfo_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VersionInfo_FormClosing);
            this.Load += new System.EventHandler(this.VersionInfo_Load);
            this.GroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblProductName;
        private System.Windows.Forms.Label LblVersion;
        private System.Windows.Forms.GroupBox GroupBox;
        private System.Windows.Forms.RichTextBox RtbComponentVersions;
        private System.Windows.Forms.LinkLabel LnkLicenseStatus;
    }
}
namespace DiscordBotGUI
{
    partial class LicenseInfoSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseInfoSetting));
            this.LicenseTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LblRegister = new System.Windows.Forms.Label();
            this.BtnDisplay = new System.Windows.Forms.Button();
            this.LblLicenseKey = new System.Windows.Forms.Label();
            this.LblLicenseFile = new System.Windows.Forms.Label();
            this.TxtLicenseKey = new System.Windows.Forms.TextBox();
            this.BtnActivate = new System.Windows.Forms.Button();
            this.BtnRegister = new System.Windows.Forms.Button();
            this.TxtLicenseFilePath = new System.Windows.Forms.TextBox();
            this.BtnSelectLicenseFile = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.BtnGenerateAuthRequest = new System.Windows.Forms.Button();
            this.TxtHwId = new System.Windows.Forms.TextBox();
            this.LblHwId = new System.Windows.Forms.Label();
            this.TabPagePlugins = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.ClbPlugins = new System.Windows.Forms.CheckedListBox();
            this.LicenseTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.TabPagePlugins.SuspendLayout();
            this.SuspendLayout();
            // 
            // LicenseTabControl
            // 
            this.LicenseTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LicenseTabControl.Controls.Add(this.tabPage1);
            this.LicenseTabControl.Controls.Add(this.tabPage3);
            this.LicenseTabControl.Controls.Add(this.TabPagePlugins);
            this.LicenseTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LicenseTabControl.Location = new System.Drawing.Point(12, 12);
            this.LicenseTabControl.Name = "LicenseTabControl";
            this.LicenseTabControl.SelectedIndex = 0;
            this.LicenseTabControl.Size = new System.Drawing.Size(360, 237);
            this.LicenseTabControl.TabIndex = 57;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.LblRegister);
            this.tabPage1.Controls.Add(this.BtnDisplay);
            this.tabPage1.Controls.Add(this.LblLicenseKey);
            this.tabPage1.Controls.Add(this.LblLicenseFile);
            this.tabPage1.Controls.Add(this.TxtLicenseKey);
            this.tabPage1.Controls.Add(this.BtnActivate);
            this.tabPage1.Controls.Add(this.BtnRegister);
            this.tabPage1.Controls.Add(this.TxtLicenseFilePath);
            this.tabPage1.Controls.Add(this.BtnSelectLicenseFile);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(352, 209);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ライセンス認証";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LblRegister
            // 
            this.LblRegister.AutoSize = true;
            this.LblRegister.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRegister.ForeColor = System.Drawing.Color.Red;
            this.LblRegister.Location = new System.Drawing.Point(7, 97);
            this.LblRegister.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblRegister.Name = "LblRegister";
            this.LblRegister.Size = new System.Drawing.Size(239, 21);
            this.LblRegister.TabIndex = 61;
            this.LblRegister.Text = "まだライセンス認証されていません!!";
            // 
            // BtnDisplay
            // 
            this.BtnDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnDisplay.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BtnDisplay.Location = new System.Drawing.Point(286, 183);
            this.BtnDisplay.Name = "BtnDisplay";
            this.BtnDisplay.Size = new System.Drawing.Size(60, 20);
            this.BtnDisplay.TabIndex = 60;
            this.BtnDisplay.Text = "表示";
            this.BtnDisplay.UseVisualStyleBackColor = true;
            this.BtnDisplay.Click += new System.EventHandler(this.BtnDisplay_Click);
            // 
            // LblLicenseKey
            // 
            this.LblLicenseKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblLicenseKey.AutoSize = true;
            this.LblLicenseKey.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblLicenseKey.Location = new System.Drawing.Point(7, 159);
            this.LblLicenseKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblLicenseKey.Name = "LblLicenseKey";
            this.LblLicenseKey.Size = new System.Drawing.Size(149, 21);
            this.LblLicenseKey.TabIndex = 59;
            this.LblLicenseKey.Text = "ライセンスキーを入力";
            // 
            // LblLicenseFile
            // 
            this.LblLicenseFile.AutoSize = true;
            this.LblLicenseFile.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblLicenseFile.Location = new System.Drawing.Point(7, 3);
            this.LblLicenseFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblLicenseFile.Name = "LblLicenseFile";
            this.LblLicenseFile.Size = new System.Drawing.Size(168, 21);
            this.LblLicenseFile.TabIndex = 45;
            this.LblLicenseFile.Text = "ライセンスファイルを選択";
            // 
            // TxtLicenseKey
            // 
            this.TxtLicenseKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLicenseKey.Enabled = false;
            this.TxtLicenseKey.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TxtLicenseKey.Location = new System.Drawing.Point(6, 183);
            this.TxtLicenseKey.Name = "TxtLicenseKey";
            this.TxtLicenseKey.PasswordChar = '*';
            this.TxtLicenseKey.Size = new System.Drawing.Size(274, 20);
            this.TxtLicenseKey.TabIndex = 58;
            this.TxtLicenseKey.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtLicenseKey_KeyPress);
            // 
            // BtnActivate
            // 
            this.BtnActivate.Enabled = false;
            this.BtnActivate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnActivate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnActivate.ForeColor = System.Drawing.Color.DarkTurquoise;
            this.BtnActivate.Location = new System.Drawing.Point(130, 53);
            this.BtnActivate.Name = "BtnActivate";
            this.BtnActivate.Size = new System.Drawing.Size(116, 29);
            this.BtnActivate.TabIndex = 56;
            this.BtnActivate.Text = "アクティベート";
            this.BtnActivate.UseVisualStyleBackColor = true;
            this.BtnActivate.Click += new System.EventHandler(this.BtnActivate_Click);
            // 
            // BtnRegister
            // 
            this.BtnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnRegister.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRegister.ForeColor = System.Drawing.Color.Blue;
            this.BtnRegister.Location = new System.Drawing.Point(6, 53);
            this.BtnRegister.Name = "BtnRegister";
            this.BtnRegister.Size = new System.Drawing.Size(116, 29);
            this.BtnRegister.TabIndex = 55;
            this.BtnRegister.Text = "ライセンス認証";
            this.BtnRegister.UseVisualStyleBackColor = true;
            this.BtnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // TxtLicenseFilePath
            // 
            this.TxtLicenseFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLicenseFilePath.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TxtLicenseFilePath.Location = new System.Drawing.Point(6, 27);
            this.TxtLicenseFilePath.Name = "TxtLicenseFilePath";
            this.TxtLicenseFilePath.Size = new System.Drawing.Size(291, 20);
            this.TxtLicenseFilePath.TabIndex = 46;
            // 
            // BtnSelectLicenseFile
            // 
            this.BtnSelectLicenseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnSelectLicenseFile.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BtnSelectLicenseFile.Location = new System.Drawing.Point(303, 27);
            this.BtnSelectLicenseFile.Name = "BtnSelectLicenseFile";
            this.BtnSelectLicenseFile.Size = new System.Drawing.Size(43, 20);
            this.BtnSelectLicenseFile.TabIndex = 47;
            this.BtnSelectLicenseFile.Text = "参照";
            this.BtnSelectLicenseFile.UseVisualStyleBackColor = true;
            this.BtnSelectLicenseFile.Click += new System.EventHandler(this.BtnSelectLicenseFile_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.BtnGenerateAuthRequest);
            this.tabPage3.Controls.Add(this.TxtHwId);
            this.tabPage3.Controls.Add(this.LblHwId);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(352, 209);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "認証キー発行";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // BtnGenerateAuthRequest
            // 
            this.BtnGenerateAuthRequest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnGenerateAuthRequest.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnGenerateAuthRequest.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnGenerateAuthRequest.ForeColor = System.Drawing.Color.Blue;
            this.BtnGenerateAuthRequest.Location = new System.Drawing.Point(6, 174);
            this.BtnGenerateAuthRequest.Name = "BtnGenerateAuthRequest";
            this.BtnGenerateAuthRequest.Size = new System.Drawing.Size(116, 29);
            this.BtnGenerateAuthRequest.TabIndex = 56;
            this.BtnGenerateAuthRequest.Text = "認証キー発行";
            this.BtnGenerateAuthRequest.UseVisualStyleBackColor = true;
            this.BtnGenerateAuthRequest.Click += new System.EventHandler(this.BtnGenerateAuthRequest_Click);
            // 
            // TxtHwId
            // 
            this.TxtHwId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtHwId.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.TxtHwId.Location = new System.Drawing.Point(6, 27);
            this.TxtHwId.Name = "TxtHwId";
            this.TxtHwId.ReadOnly = true;
            this.TxtHwId.Size = new System.Drawing.Size(340, 20);
            this.TxtHwId.TabIndex = 47;
            // 
            // LblHwId
            // 
            this.LblHwId.AutoSize = true;
            this.LblHwId.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblHwId.Location = new System.Drawing.Point(7, 3);
            this.LblHwId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblHwId.Name = "LblHwId";
            this.LblHwId.Size = new System.Drawing.Size(101, 21);
            this.LblHwId.TabIndex = 46;
            this.LblHwId.Text = "ハードウェアID";
            // 
            // TabPagePlugins
            // 
            this.TabPagePlugins.Controls.Add(this.label1);
            this.TabPagePlugins.Controls.Add(this.ClbPlugins);
            this.TabPagePlugins.Location = new System.Drawing.Point(4, 24);
            this.TabPagePlugins.Name = "TabPagePlugins";
            this.TabPagePlugins.Padding = new System.Windows.Forms.Padding(3);
            this.TabPagePlugins.Size = new System.Drawing.Size(352, 209);
            this.TabPagePlugins.TabIndex = 3;
            this.TabPagePlugins.Text = "プラグイン一覧";
            this.TabPagePlugins.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 21);
            this.label1.TabIndex = 47;
            this.label1.Text = "認証済みプラグインDLL";
            // 
            // ClbPlugins
            // 
            this.ClbPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClbPlugins.FormattingEnabled = true;
            this.ClbPlugins.HorizontalScrollbar = true;
            this.ClbPlugins.IntegralHeight = false;
            this.ClbPlugins.Location = new System.Drawing.Point(6, 32);
            this.ClbPlugins.Name = "ClbPlugins";
            this.ClbPlugins.Size = new System.Drawing.Size(340, 171);
            this.ClbPlugins.Sorted = true;
            this.ClbPlugins.TabIndex = 0;
            this.ClbPlugins.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ClbPlugins_ItemCheck);
            this.ClbPlugins.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ClbPlugins_MouseMove);
            // 
            // LicenseInfoSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.LicenseTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "LicenseInfoSetting";
            this.Text = "ライセンス認証プロパティ";
            this.Activated += new System.EventHandler(this.LicenseInfoSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LicenseInfoSetting_FormClosing);
            this.Load += new System.EventHandler(this.LicenseInfoSetting_Load);
            this.LicenseTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.TabPagePlugins.ResumeLayout(false);
            this.TabPagePlugins.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl LicenseTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label LblLicenseFile;
        private System.Windows.Forms.Button BtnRegister;
        private System.Windows.Forms.TextBox TxtLicenseFilePath;
        private System.Windows.Forms.Button BtnSelectLicenseFile;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label LblHwId;
        private System.Windows.Forms.TextBox TxtHwId;
        private System.Windows.Forms.Button BtnGenerateAuthRequest;
        private System.Windows.Forms.Button BtnActivate;
        private System.Windows.Forms.TextBox TxtLicenseKey;
        private System.Windows.Forms.Label LblLicenseKey;
        private System.Windows.Forms.TabPage TabPagePlugins;
        private System.Windows.Forms.CheckedListBox ClbPlugins;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnDisplay;
        private System.Windows.Forms.Label LblRegister;
    }
}
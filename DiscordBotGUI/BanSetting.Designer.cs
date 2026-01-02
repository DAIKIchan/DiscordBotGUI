namespace DiscordBotGUI
{
    partial class BanSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BanSetting));
            this.BanTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LblRolePermission = new System.Windows.Forms.Label();
            this.LblBanServer = new System.Windows.Forms.Label();
            this.CmbGuilds = new System.Windows.Forms.ComboBox();
            this.ChkListBoxRoles = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.BtnUnban = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.LstBannedUsers = new System.Windows.Forms.ListBox();
            this.NumDeleteDelayMs = new System.Windows.Forms.NumericUpDown();
            this.ChkAutoDeleteEnabled = new System.Windows.Forms.CheckBox();
            this.NumTimeoutMinutes = new System.Windows.Forms.NumericUpDown();
            this.LblTimeoutMinutes = new System.Windows.Forms.Label();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BanTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).BeginInit();
            this.SuspendLayout();
            // 
            // BanTabControl
            // 
            this.BanTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BanTabControl.Controls.Add(this.tabPage1);
            this.BanTabControl.Controls.Add(this.tabPage2);
            this.BanTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BanTabControl.Location = new System.Drawing.Point(12, 12);
            this.BanTabControl.Name = "BanTabControl";
            this.BanTabControl.SelectedIndex = 0;
            this.BanTabControl.Size = new System.Drawing.Size(460, 367);
            this.BanTabControl.TabIndex = 59;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Navy;
            this.tabPage1.Controls.Add(this.LblRolePermission);
            this.tabPage1.Controls.Add(this.LblBanServer);
            this.tabPage1.Controls.Add(this.CmbGuilds);
            this.tabPage1.Controls.Add(this.ChkListBoxRoles);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(452, 339);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ユーザBAN設定";
            // 
            // LblRolePermission
            // 
            this.LblRolePermission.AutoSize = true;
            this.LblRolePermission.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRolePermission.ForeColor = System.Drawing.Color.White;
            this.LblRolePermission.Location = new System.Drawing.Point(7, 71);
            this.LblRolePermission.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblRolePermission.Name = "LblRolePermission";
            this.LblRolePermission.Size = new System.Drawing.Size(158, 21);
            this.LblRolePermission.TabIndex = 46;
            this.LblRolePermission.Text = "許可するロールを選択";
            // 
            // LblBanServer
            // 
            this.LblBanServer.AutoSize = true;
            this.LblBanServer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblBanServer.ForeColor = System.Drawing.Color.White;
            this.LblBanServer.Location = new System.Drawing.Point(7, 3);
            this.LblBanServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblBanServer.Name = "LblBanServer";
            this.LblBanServer.Size = new System.Drawing.Size(160, 21);
            this.LblBanServer.TabIndex = 45;
            this.LblBanServer.Text = "許可するサーバを選択";
            // 
            // CmbGuilds
            // 
            this.CmbGuilds.BackColor = System.Drawing.Color.Navy;
            this.CmbGuilds.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmbGuilds.ForeColor = System.Drawing.Color.White;
            this.CmbGuilds.FormattingEnabled = true;
            this.CmbGuilds.Location = new System.Drawing.Point(6, 27);
            this.CmbGuilds.Name = "CmbGuilds";
            this.CmbGuilds.Size = new System.Drawing.Size(169, 23);
            this.CmbGuilds.TabIndex = 9;
            this.CmbGuilds.SelectedIndexChanged += new System.EventHandler(this.CmbGuilds_SelectedIndexChanged);
            // 
            // ChkListBoxRoles
            // 
            this.ChkListBoxRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkListBoxRoles.BackColor = System.Drawing.Color.Navy;
            this.ChkListBoxRoles.ForeColor = System.Drawing.Color.White;
            this.ChkListBoxRoles.FormattingEnabled = true;
            this.ChkListBoxRoles.HorizontalScrollbar = true;
            this.ChkListBoxRoles.Location = new System.Drawing.Point(6, 95);
            this.ChkListBoxRoles.Name = "ChkListBoxRoles";
            this.ChkListBoxRoles.Size = new System.Drawing.Size(440, 238);
            this.ChkListBoxRoles.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Navy;
            this.tabPage2.Controls.Add(this.BtnUnban);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.LstBannedUsers);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(452, 339);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ユーザBAN解除";
            // 
            // BtnUnban
            // 
            this.BtnUnban.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnUnban.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnUnban.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnUnban.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnUnban.ForeColor = System.Drawing.Color.Transparent;
            this.BtnUnban.Location = new System.Drawing.Point(6, 308);
            this.BtnUnban.Name = "BtnUnban";
            this.BtnUnban.Size = new System.Drawing.Size(77, 25);
            this.BtnUnban.TabIndex = 88;
            this.BtnUnban.Text = "BAN解除";
            this.BtnUnban.UseVisualStyleBackColor = false;
            this.BtnUnban.Click += new System.EventHandler(this.BtnUnban_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(7, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 21);
            this.label1.TabIndex = 48;
            this.label1.Text = "BAN解除するユーザを選択";
            // 
            // LstBannedUsers
            // 
            this.LstBannedUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LstBannedUsers.BackColor = System.Drawing.Color.Navy;
            this.LstBannedUsers.ForeColor = System.Drawing.Color.White;
            this.LstBannedUsers.FormattingEnabled = true;
            this.LstBannedUsers.ItemHeight = 15;
            this.LstBannedUsers.Location = new System.Drawing.Point(6, 27);
            this.LstBannedUsers.Name = "LstBannedUsers";
            this.LstBannedUsers.Size = new System.Drawing.Size(440, 274);
            this.LstBannedUsers.TabIndex = 47;
            // 
            // NumDeleteDelayMs
            // 
            this.NumDeleteDelayMs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumDeleteDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteDelayMs.Location = new System.Drawing.Point(12, 416);
            this.NumDeleteDelayMs.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.NumDeleteDelayMs.Name = "NumDeleteDelayMs";
            this.NumDeleteDelayMs.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteDelayMs.TabIndex = 84;
            this.NumDeleteDelayMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ChkAutoDeleteEnabled
            // 
            this.ChkAutoDeleteEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ChkAutoDeleteEnabled.AutoSize = true;
            this.ChkAutoDeleteEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkAutoDeleteEnabled.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkAutoDeleteEnabled.ForeColor = System.Drawing.Color.Black;
            this.ChkAutoDeleteEnabled.Location = new System.Drawing.Point(12, 385);
            this.ChkAutoDeleteEnabled.Name = "ChkAutoDeleteEnabled";
            this.ChkAutoDeleteEnabled.Size = new System.Drawing.Size(283, 25);
            this.ChkAutoDeleteEnabled.TabIndex = 83;
            this.ChkAutoDeleteEnabled.Text = "プロンプトMSGの自動削除を有効[ms]";
            this.ChkAutoDeleteEnabled.UseVisualStyleBackColor = true;
            this.ChkAutoDeleteEnabled.CheckedChanged += new System.EventHandler(this.ChkAutoDeleteEnabled_CheckedChanged);
            // 
            // NumTimeoutMinutes
            // 
            this.NumTimeoutMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumTimeoutMinutes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumTimeoutMinutes.Location = new System.Drawing.Point(12, 479);
            this.NumTimeoutMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.NumTimeoutMinutes.Name = "NumTimeoutMinutes";
            this.NumTimeoutMinutes.Size = new System.Drawing.Size(169, 23);
            this.NumTimeoutMinutes.TabIndex = 86;
            this.NumTimeoutMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LblTimeoutMinutes
            // 
            this.LblTimeoutMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblTimeoutMinutes.AutoSize = true;
            this.LblTimeoutMinutes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTimeoutMinutes.Location = new System.Drawing.Point(13, 455);
            this.LblTimeoutMinutes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblTimeoutMinutes.Name = "LblTimeoutMinutes";
            this.LblTimeoutMinutes.Size = new System.Drawing.Size(323, 21);
            this.LblTimeoutMinutes.TabIndex = 85;
            this.LblTimeoutMinutes.Text = "対話中のプロンプトMSGのタイムアウト時間[分]";
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(12, 524);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(53, 25);
            this.BtnSave.TabIndex = 87;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BanSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.NumTimeoutMinutes);
            this.Controls.Add(this.LblTimeoutMinutes);
            this.Controls.Add(this.NumDeleteDelayMs);
            this.Controls.Add(this.ChkAutoDeleteEnabled);
            this.Controls.Add(this.BanTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "BanSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ユーザBANプロパティ";
            this.Activated += new System.EventHandler(this.BanSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BanSetting_FormClosing);
            this.Load += new System.EventHandler(this.BanSetting_Load);
            this.BanTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl BanTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label LblRolePermission;
        private System.Windows.Forms.Label LblBanServer;
        private System.Windows.Forms.ComboBox CmbGuilds;
        private System.Windows.Forms.CheckedListBox ChkListBoxRoles;
        private System.Windows.Forms.NumericUpDown NumDeleteDelayMs;
        private System.Windows.Forms.CheckBox ChkAutoDeleteEnabled;
        private System.Windows.Forms.NumericUpDown NumTimeoutMinutes;
        private System.Windows.Forms.Label LblTimeoutMinutes;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListBox LstBannedUsers;
        private System.Windows.Forms.Button BtnUnban;
        private System.Windows.Forms.Label label1;
    }
}
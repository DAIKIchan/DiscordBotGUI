namespace DiscordBotGUI
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.LblToken = new System.Windows.Forms.Label();
            this.TxtToken = new System.Windows.Forms.TextBox();
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.TxtLog = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.SettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MessageBulkDeletionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UserBanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UserKickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RoleSettingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WeatherInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.JoiningLeavingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProductToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UpdateCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnReloadAllPlugins = new System.Windows.Forms.Button();
            this.BtnReleaseAllPlugins = new System.Windows.Forms.Button();
            this.BtnReloadAllCommand = new System.Windows.Forms.Button();
            this.licenseCheckTimer = new System.Windows.Forms.Timer(this.components);
            this.tESTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LblToken
            // 
            this.LblToken.AutoSize = true;
            this.LblToken.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblToken.Location = new System.Drawing.Point(12, 25);
            this.LblToken.Name = "LblToken";
            this.LblToken.Size = new System.Drawing.Size(89, 21);
            this.LblToken.TabIndex = 0;
            this.LblToken.Text = "BOTトークン";
            // 
            // TxtToken
            // 
            this.TxtToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtToken.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtToken.Location = new System.Drawing.Point(107, 25);
            this.TxtToken.Name = "TxtToken";
            this.TxtToken.PasswordChar = '●';
            this.TxtToken.Size = new System.Drawing.Size(765, 23);
            this.TxtToken.TabIndex = 1;
            // 
            // BtnStart
            // 
            this.BtnStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnStart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnStart.ForeColor = System.Drawing.Color.Green;
            this.BtnStart.Location = new System.Drawing.Point(12, 69);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(75, 23);
            this.BtnStart.TabIndex = 2;
            this.BtnStart.Text = "起動";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnStop.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnStop.ForeColor = System.Drawing.Color.Red;
            this.BtnStop.Location = new System.Drawing.Point(93, 69);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(75, 23);
            this.BtnStop.TabIndex = 3;
            this.BtnStop.Text = "停止";
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // TxtLog
            // 
            this.TxtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLog.BackColor = System.Drawing.Color.Black;
            this.TxtLog.ForeColor = System.Drawing.Color.Transparent;
            this.TxtLog.Location = new System.Drawing.Point(12, 98);
            this.TxtLog.Name = "TxtLog";
            this.TxtLog.ReadOnly = true;
            this.TxtLog.Size = new System.Drawing.Size(860, 351);
            this.TxtLog.TabIndex = 4;
            this.TxtLog.Text = "";
            this.TxtLog.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingToolStripMenuItem,
            this.EditToolStripMenuItem,
            this.PluginToolStripMenuItem,
            this.ProductToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // SettingToolStripMenuItem
            // 
            this.SettingToolStripMenuItem.Name = "SettingToolStripMenuItem";
            this.SettingToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.SettingToolStripMenuItem.Text = "設定";
            this.SettingToolStripMenuItem.Click += new System.EventHandler(this.SettingToolStripMenuItem_Click);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.EditToolStripMenuItem.Text = "編集";
            this.EditToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItem_Click);
            // 
            // PluginToolStripMenuItem
            // 
            this.PluginToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.QAToolStripMenuItem,
            this.MessageBulkDeletionToolStripMenuItem,
            this.UserBanToolStripMenuItem,
            this.UserKickToolStripMenuItem,
            this.RoleSettingToolStripMenuItem,
            this.WeatherInfoToolStripMenuItem,
            this.JoiningLeavingToolStripMenuItem});
            this.PluginToolStripMenuItem.Name = "PluginToolStripMenuItem";
            this.PluginToolStripMenuItem.Size = new System.Drawing.Size(79, 21);
            this.PluginToolStripMenuItem.Text = "プラグイン";
            // 
            // QAToolStripMenuItem
            // 
            this.QAToolStripMenuItem.Enabled = false;
            this.QAToolStripMenuItem.Name = "QAToolStripMenuItem";
            this.QAToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.QAToolStripMenuItem.Text = "アンケートQA";
            this.QAToolStripMenuItem.Click += new System.EventHandler(this.QAToolStripMenuItem_Click);
            // 
            // MessageBulkDeletionToolStripMenuItem
            // 
            this.MessageBulkDeletionToolStripMenuItem.Enabled = false;
            this.MessageBulkDeletionToolStripMenuItem.Name = "MessageBulkDeletionToolStripMenuItem";
            this.MessageBulkDeletionToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.MessageBulkDeletionToolStripMenuItem.Text = "メッセージ一括削除";
            this.MessageBulkDeletionToolStripMenuItem.Click += new System.EventHandler(this.MessageBulkDeletionToolStripMenuItem_Click);
            // 
            // UserBanToolStripMenuItem
            // 
            this.UserBanToolStripMenuItem.Enabled = false;
            this.UserBanToolStripMenuItem.Name = "UserBanToolStripMenuItem";
            this.UserBanToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.UserBanToolStripMenuItem.Text = "ユーザBAN";
            this.UserBanToolStripMenuItem.Click += new System.EventHandler(this.UserBanToolStripMenuItem_Click);
            // 
            // UserKickToolStripMenuItem
            // 
            this.UserKickToolStripMenuItem.Enabled = false;
            this.UserKickToolStripMenuItem.Name = "UserKickToolStripMenuItem";
            this.UserKickToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.UserKickToolStripMenuItem.Text = "ユーザKICK";
            this.UserKickToolStripMenuItem.Click += new System.EventHandler(this.UserKickToolStripMenuItem_Click);
            // 
            // RoleSettingToolStripMenuItem
            // 
            this.RoleSettingToolStripMenuItem.Enabled = false;
            this.RoleSettingToolStripMenuItem.Name = "RoleSettingToolStripMenuItem";
            this.RoleSettingToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.RoleSettingToolStripMenuItem.Text = "ロール自動付与";
            this.RoleSettingToolStripMenuItem.Click += new System.EventHandler(this.RoleSettingToolStripMenuItem_Click);
            // 
            // WeatherInfoToolStripMenuItem
            // 
            this.WeatherInfoToolStripMenuItem.Enabled = false;
            this.WeatherInfoToolStripMenuItem.Name = "WeatherInfoToolStripMenuItem";
            this.WeatherInfoToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.WeatherInfoToolStripMenuItem.Text = "天気予報";
            this.WeatherInfoToolStripMenuItem.Click += new System.EventHandler(this.WeatherInfoToolStripMenuItem_Click);
            // 
            // JoiningLeavingToolStripMenuItem
            // 
            this.JoiningLeavingToolStripMenuItem.Enabled = false;
            this.JoiningLeavingToolStripMenuItem.Name = "JoiningLeavingToolStripMenuItem";
            this.JoiningLeavingToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.JoiningLeavingToolStripMenuItem.Text = "ユーザ入退出ログ";
            this.JoiningLeavingToolStripMenuItem.Click += new System.EventHandler(this.JoiningLeavingToolStripMenuItem_Click);
            // 
            // ProductToolStripMenuItem
            // 
            this.ProductToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VersionToolStripMenuItem,
            this.LicenseToolStripMenuItem,
            this.UpdateCheckToolStripMenuItem,
            this.tESTToolStripMenuItem});
            this.ProductToolStripMenuItem.Name = "ProductToolStripMenuItem";
            this.ProductToolStripMenuItem.Size = new System.Drawing.Size(84, 21);
            this.ProductToolStripMenuItem.Text = "製品情報";
            // 
            // VersionToolStripMenuItem
            // 
            this.VersionToolStripMenuItem.Name = "VersionToolStripMenuItem";
            this.VersionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.VersionToolStripMenuItem.Text = "Version情報";
            this.VersionToolStripMenuItem.Click += new System.EventHandler(this.VersionToolStripMenuItem_Click);
            // 
            // LicenseToolStripMenuItem
            // 
            this.LicenseToolStripMenuItem.Name = "LicenseToolStripMenuItem";
            this.LicenseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.LicenseToolStripMenuItem.Text = "ライセンス認証";
            this.LicenseToolStripMenuItem.Click += new System.EventHandler(this.LicenseToolStripMenuItem_Click);
            // 
            // UpdateCheckToolStripMenuItem
            // 
            this.UpdateCheckToolStripMenuItem.Name = "UpdateCheckToolStripMenuItem";
            this.UpdateCheckToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.UpdateCheckToolStripMenuItem.Text = "アップデート確認";
            this.UpdateCheckToolStripMenuItem.Click += new System.EventHandler(this.UpdateCheckToolStripMenuItem_Click);
            // 
            // BtnReloadAllPlugins
            // 
            this.BtnReloadAllPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnReloadAllPlugins.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnReloadAllPlugins.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnReloadAllPlugins.ForeColor = System.Drawing.Color.RoyalBlue;
            this.BtnReloadAllPlugins.Location = new System.Drawing.Point(389, 69);
            this.BtnReloadAllPlugins.Name = "BtnReloadAllPlugins";
            this.BtnReloadAllPlugins.Size = new System.Drawing.Size(157, 23);
            this.BtnReloadAllPlugins.TabIndex = 6;
            this.BtnReloadAllPlugins.Text = "プラグインDLLリロード";
            this.BtnReloadAllPlugins.UseVisualStyleBackColor = true;
            this.BtnReloadAllPlugins.Click += new System.EventHandler(this.BtnReloadAllPlugins_Click);
            // 
            // BtnReleaseAllPlugins
            // 
            this.BtnReleaseAllPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnReleaseAllPlugins.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnReleaseAllPlugins.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnReleaseAllPlugins.ForeColor = System.Drawing.Color.Olive;
            this.BtnReleaseAllPlugins.Location = new System.Drawing.Point(552, 69);
            this.BtnReleaseAllPlugins.Name = "BtnReleaseAllPlugins";
            this.BtnReleaseAllPlugins.Size = new System.Drawing.Size(157, 23);
            this.BtnReleaseAllPlugins.TabIndex = 7;
            this.BtnReleaseAllPlugins.Text = "プラグインDLLアンロード";
            this.BtnReleaseAllPlugins.UseVisualStyleBackColor = true;
            this.BtnReleaseAllPlugins.Click += new System.EventHandler(this.BtnReleaseAllPlugins_Click);
            // 
            // BtnReloadAllCommand
            // 
            this.BtnReloadAllCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnReloadAllCommand.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnReloadAllCommand.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnReloadAllCommand.ForeColor = System.Drawing.Color.Coral;
            this.BtnReloadAllCommand.Location = new System.Drawing.Point(715, 69);
            this.BtnReloadAllCommand.Name = "BtnReloadAllCommand";
            this.BtnReloadAllCommand.Size = new System.Drawing.Size(157, 23);
            this.BtnReloadAllCommand.TabIndex = 8;
            this.BtnReloadAllCommand.Text = "登録コマンドリロード";
            this.BtnReloadAllCommand.UseVisualStyleBackColor = true;
            this.BtnReloadAllCommand.Click += new System.EventHandler(this.BtnReloadAllCommand_Click);
            // 
            // licenseCheckTimer
            // 
            this.licenseCheckTimer.Enabled = true;
            this.licenseCheckTimer.Interval = 60000;
            this.licenseCheckTimer.Tick += new System.EventHandler(this.licenseCheckTimer_Tick);
            // 
            // tESTToolStripMenuItem
            // 
            this.tESTToolStripMenuItem.Name = "tESTToolStripMenuItem";
            this.tESTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tESTToolStripMenuItem.Text = "TEST";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.BtnReloadAllCommand);
            this.Controls.Add(this.BtnReleaseAllPlugins);
            this.Controls.Add(this.BtnReloadAllPlugins);
            this.Controls.Add(this.TxtLog);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.TxtToken);
            this.Controls.Add(this.LblToken);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 300);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DiscordBOTコンソール(GUI版)";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblToken;
        private System.Windows.Forms.TextBox TxtToken;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.RichTextBox TxtLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem SettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem EditToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProductToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem VersionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LicenseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PluginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem JoiningLeavingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem QAToolStripMenuItem;
        private System.Windows.Forms.Button BtnReloadAllPlugins;
        private System.Windows.Forms.Button BtnReleaseAllPlugins;
        private System.Windows.Forms.Button BtnReloadAllCommand;
        private System.Windows.Forms.ToolStripMenuItem WeatherInfoToolStripMenuItem;
        private System.Windows.Forms.Timer licenseCheckTimer;
        private System.Windows.Forms.ToolStripMenuItem UserKickToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MessageBulkDeletionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UserBanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RoleSettingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UpdateCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tESTToolStripMenuItem;
    }
}


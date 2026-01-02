namespace DiscordBotGUI
{
    partial class KickSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KickSetting));
            this.ChkListBoxRoles = new System.Windows.Forms.CheckedListBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LblRolePermission = new System.Windows.Forms.Label();
            this.LblKickServer = new System.Windows.Forms.Label();
            this.CmbGuilds = new System.Windows.Forms.ComboBox();
            this.KickTabControl = new System.Windows.Forms.TabControl();
            this.NumDeleteDelayMs = new System.Windows.Forms.NumericUpDown();
            this.ChkAutoDeleteEnabled = new System.Windows.Forms.CheckBox();
            this.LblTimeoutMinutes = new System.Windows.Forms.Label();
            this.NumTimeoutMinutes = new System.Windows.Forms.NumericUpDown();
            this.tabPage1.SuspendLayout();
            this.KickTabControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).BeginInit();
            this.SuspendLayout();
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
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(12, 524);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(53, 25);
            this.BtnSave.TabIndex = 8;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Navy;
            this.tabPage1.Controls.Add(this.LblRolePermission);
            this.tabPage1.Controls.Add(this.LblKickServer);
            this.tabPage1.Controls.Add(this.CmbGuilds);
            this.tabPage1.Controls.Add(this.ChkListBoxRoles);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(452, 339);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ユーザKICK設定";
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
            // LblKickServer
            // 
            this.LblKickServer.AutoSize = true;
            this.LblKickServer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblKickServer.ForeColor = System.Drawing.Color.White;
            this.LblKickServer.Location = new System.Drawing.Point(7, 3);
            this.LblKickServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblKickServer.Name = "LblKickServer";
            this.LblKickServer.Size = new System.Drawing.Size(160, 21);
            this.LblKickServer.TabIndex = 45;
            this.LblKickServer.Text = "許可するサーバを選択";
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
            // KickTabControl
            // 
            this.KickTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.KickTabControl.Controls.Add(this.tabPage1);
            this.KickTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KickTabControl.Location = new System.Drawing.Point(12, 12);
            this.KickTabControl.Name = "KickTabControl";
            this.KickTabControl.SelectedIndex = 0;
            this.KickTabControl.Size = new System.Drawing.Size(460, 367);
            this.KickTabControl.TabIndex = 58;
            // 
            // NumDeleteDelayMs
            // 
            this.NumDeleteDelayMs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumDeleteDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteDelayMs.Location = new System.Drawing.Point(11, 416);
            this.NumDeleteDelayMs.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.NumDeleteDelayMs.Name = "NumDeleteDelayMs";
            this.NumDeleteDelayMs.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteDelayMs.TabIndex = 82;
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
            this.ChkAutoDeleteEnabled.TabIndex = 81;
            this.ChkAutoDeleteEnabled.Text = "プロンプトMSGの自動削除を有効[ms]";
            this.ChkAutoDeleteEnabled.UseVisualStyleBackColor = true;
            this.ChkAutoDeleteEnabled.CheckedChanged += new System.EventHandler(this.ChkAutoDeleteEnabled_CheckedChanged);
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
            this.LblTimeoutMinutes.TabIndex = 83;
            this.LblTimeoutMinutes.Text = "対話中のプロンプトMSGのタイムアウト時間[分]";
            // 
            // NumTimeoutMinutes
            // 
            this.NumTimeoutMinutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NumTimeoutMinutes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumTimeoutMinutes.Location = new System.Drawing.Point(11, 479);
            this.NumTimeoutMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.NumTimeoutMinutes.Name = "NumTimeoutMinutes";
            this.NumTimeoutMinutes.Size = new System.Drawing.Size(169, 23);
            this.NumTimeoutMinutes.TabIndex = 84;
            this.NumTimeoutMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // KickSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.NumTimeoutMinutes);
            this.Controls.Add(this.LblTimeoutMinutes);
            this.Controls.Add(this.NumDeleteDelayMs);
            this.Controls.Add(this.ChkAutoDeleteEnabled);
            this.Controls.Add(this.KickTabControl);
            this.Controls.Add(this.BtnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "KickSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ユーザKICKプロパティ";
            this.Activated += new System.EventHandler(this.KickSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KickSetting_FormClosing);
            this.Load += new System.EventHandler(this.KickSetting_Load);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.KickTabControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox ChkListBoxRoles;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label LblKickServer;
        private System.Windows.Forms.ComboBox CmbGuilds;
        private System.Windows.Forms.TabControl KickTabControl;
        private System.Windows.Forms.Label LblRolePermission;
        private System.Windows.Forms.NumericUpDown NumDeleteDelayMs;
        private System.Windows.Forms.CheckBox ChkAutoDeleteEnabled;
        private System.Windows.Forms.Label LblTimeoutMinutes;
        private System.Windows.Forms.NumericUpDown NumTimeoutMinutes;
    }
}
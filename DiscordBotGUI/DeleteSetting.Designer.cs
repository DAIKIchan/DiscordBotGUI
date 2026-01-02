namespace DiscordBotGUI
{
    partial class DeleteSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteSetting));
            this.DeleteTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LblRolePermission = new System.Windows.Forms.Label();
            this.LblDeleteServer = new System.Windows.Forms.Label();
            this.CmbGuilds = new System.Windows.Forms.ComboBox();
            this.ChkListBoxRoles = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.NumTimeoutMinutes = new System.Windows.Forms.NumericUpDown();
            this.LblTimeoutMinutes = new System.Windows.Forms.Label();
            this.NumDeleteDelayMs = new System.Windows.Forms.NumericUpDown();
            this.ChkAutoDeleteEnabled = new System.Windows.Forms.CheckBox();
            this.NumDeleteItems = new System.Windows.Forms.NumericUpDown();
            this.ChkDeleteItems = new System.Windows.Forms.CheckBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.DeleteTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteItems)).BeginInit();
            this.SuspendLayout();
            // 
            // DeleteTabControl
            // 
            this.DeleteTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteTabControl.Controls.Add(this.tabPage1);
            this.DeleteTabControl.Controls.Add(this.tabPage2);
            this.DeleteTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteTabControl.Location = new System.Drawing.Point(12, 12);
            this.DeleteTabControl.Name = "DeleteTabControl";
            this.DeleteTabControl.SelectedIndex = 0;
            this.DeleteTabControl.Size = new System.Drawing.Size(460, 491);
            this.DeleteTabControl.TabIndex = 59;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.LblRolePermission);
            this.tabPage1.Controls.Add(this.LblDeleteServer);
            this.tabPage1.Controls.Add(this.CmbGuilds);
            this.tabPage1.Controls.Add(this.ChkListBoxRoles);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(452, 463);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ロール許可設定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LblRolePermission
            // 
            this.LblRolePermission.AutoSize = true;
            this.LblRolePermission.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRolePermission.Location = new System.Drawing.Point(7, 67);
            this.LblRolePermission.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblRolePermission.Name = "LblRolePermission";
            this.LblRolePermission.Size = new System.Drawing.Size(158, 21);
            this.LblRolePermission.TabIndex = 46;
            this.LblRolePermission.Text = "許可するロールを選択";
            // 
            // LblDeleteServer
            // 
            this.LblDeleteServer.AutoSize = true;
            this.LblDeleteServer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblDeleteServer.Location = new System.Drawing.Point(7, 3);
            this.LblDeleteServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblDeleteServer.Name = "LblDeleteServer";
            this.LblDeleteServer.Size = new System.Drawing.Size(160, 21);
            this.LblDeleteServer.TabIndex = 45;
            this.LblDeleteServer.Text = "許可するサーバを選択";
            // 
            // CmbGuilds
            // 
            this.CmbGuilds.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.ChkListBoxRoles.Location = new System.Drawing.Point(6, 91);
            this.ChkListBoxRoles.Name = "ChkListBoxRoles";
            this.ChkListBoxRoles.Size = new System.Drawing.Size(440, 364);
            this.ChkListBoxRoles.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.NumTimeoutMinutes);
            this.tabPage2.Controls.Add(this.LblTimeoutMinutes);
            this.tabPage2.Controls.Add(this.NumDeleteDelayMs);
            this.tabPage2.Controls.Add(this.ChkAutoDeleteEnabled);
            this.tabPage2.Controls.Add(this.NumDeleteItems);
            this.tabPage2.Controls.Add(this.ChkDeleteItems);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(452, 463);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "メッセージ削除設定";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // NumTimeoutMinutes
            // 
            this.NumTimeoutMinutes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumTimeoutMinutes.Location = new System.Drawing.Point(6, 170);
            this.NumTimeoutMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.NumTimeoutMinutes.Name = "NumTimeoutMinutes";
            this.NumTimeoutMinutes.Size = new System.Drawing.Size(170, 23);
            this.NumTimeoutMinutes.TabIndex = 89;
            this.NumTimeoutMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LblTimeoutMinutes
            // 
            this.LblTimeoutMinutes.AutoSize = true;
            this.LblTimeoutMinutes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblTimeoutMinutes.Location = new System.Drawing.Point(7, 146);
            this.LblTimeoutMinutes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblTimeoutMinutes.Name = "LblTimeoutMinutes";
            this.LblTimeoutMinutes.Size = new System.Drawing.Size(323, 21);
            this.LblTimeoutMinutes.TabIndex = 88;
            this.LblTimeoutMinutes.Text = "対話中のプロンプトMSGのタイムアウト時間[分]";
            // 
            // NumDeleteDelayMs
            // 
            this.NumDeleteDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteDelayMs.Location = new System.Drawing.Point(6, 107);
            this.NumDeleteDelayMs.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.NumDeleteDelayMs.Name = "NumDeleteDelayMs";
            this.NumDeleteDelayMs.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteDelayMs.TabIndex = 87;
            this.NumDeleteDelayMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ChkAutoDeleteEnabled
            // 
            this.ChkAutoDeleteEnabled.AutoSize = true;
            this.ChkAutoDeleteEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkAutoDeleteEnabled.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkAutoDeleteEnabled.ForeColor = System.Drawing.Color.Black;
            this.ChkAutoDeleteEnabled.Location = new System.Drawing.Point(6, 76);
            this.ChkAutoDeleteEnabled.Name = "ChkAutoDeleteEnabled";
            this.ChkAutoDeleteEnabled.Size = new System.Drawing.Size(283, 25);
            this.ChkAutoDeleteEnabled.TabIndex = 86;
            this.ChkAutoDeleteEnabled.Text = "プロンプトMSGの自動削除を有効[ms]";
            this.ChkAutoDeleteEnabled.UseVisualStyleBackColor = true;
            this.ChkAutoDeleteEnabled.CheckedChanged += new System.EventHandler(this.ChkAutoDeleteEnabled_CheckedChanged);
            // 
            // NumDeleteItems
            // 
            this.NumDeleteItems.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteItems.Location = new System.Drawing.Point(6, 37);
            this.NumDeleteItems.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.NumDeleteItems.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NumDeleteItems.Name = "NumDeleteItems";
            this.NumDeleteItems.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteItems.TabIndex = 85;
            this.NumDeleteItems.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumDeleteItems.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // ChkDeleteItems
            // 
            this.ChkDeleteItems.AutoSize = true;
            this.ChkDeleteItems.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkDeleteItems.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkDeleteItems.ForeColor = System.Drawing.Color.Black;
            this.ChkDeleteItems.Location = new System.Drawing.Point(6, 6);
            this.ChkDeleteItems.Name = "ChkDeleteItems";
            this.ChkDeleteItems.Size = new System.Drawing.Size(266, 25);
            this.ChkDeleteItems.TabIndex = 82;
            this.ChkDeleteItems.Text = "メッセージ削除件数指定を有効[件]";
            this.ChkDeleteItems.UseVisualStyleBackColor = true;
            this.ChkDeleteItems.CheckedChanged += new System.EventHandler(this.ChkDeleteItems_CheckedChanged);
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(12, 524);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(53, 25);
            this.BtnSave.TabIndex = 60;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // DeleteSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 561);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.DeleteTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "DeleteSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "メッセージ一括削除プロパティ";
            this.Activated += new System.EventHandler(this.DeleteSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DeleteSetting_FormClosing);
            this.Load += new System.EventHandler(this.DeleteSetting_Load);
            this.DeleteTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl DeleteTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label LblRolePermission;
        private System.Windows.Forms.Label LblDeleteServer;
        private System.Windows.Forms.ComboBox CmbGuilds;
        private System.Windows.Forms.CheckedListBox ChkListBoxRoles;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox ChkDeleteItems;
        private System.Windows.Forms.NumericUpDown NumDeleteItems;
        private System.Windows.Forms.CheckBox ChkAutoDeleteEnabled;
        private System.Windows.Forms.NumericUpDown NumDeleteDelayMs;
        private System.Windows.Forms.Label LblTimeoutMinutes;
        private System.Windows.Forms.NumericUpDown NumTimeoutMinutes;
        private System.Windows.Forms.Button BtnSave;
    }
}
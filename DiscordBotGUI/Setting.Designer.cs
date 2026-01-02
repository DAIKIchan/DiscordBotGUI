namespace DiscordBotGUI
{
    partial class Setting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setting));
            this.LblToken = new System.Windows.Forms.Label();
            this.TxtConfig = new System.Windows.Forms.TextBox();
            this.BtnOpenCmd = new System.Windows.Forms.Button();
            this.FormSetting = new System.Windows.Forms.CheckBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.ChkDeleteCommandMessage = new System.Windows.Forms.CheckBox();
            this.ChkDebugLog = new System.Windows.Forms.CheckBox();
            this.GrpLogSettings = new System.Windows.Forms.GroupBox();
            this.ChkWriteLogFile = new System.Windows.Forms.CheckBox();
            this.LblUITaskDelayMs = new System.Windows.Forms.Label();
            this.NumUITaskDelayMs = new System.Windows.Forms.NumericUpDown();
            this.LblLogBatchSize = new System.Windows.Forms.Label();
            this.NumLogBatchSize = new System.Windows.Forms.NumericUpDown();
            this.LblLogFileSize = new System.Windows.Forms.Label();
            this.NumLogFileSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtLogFolder = new System.Windows.Forms.TextBox();
            this.BtnOpenFolder = new System.Windows.Forms.Button();
            this.GrpLogSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumUITaskDelayMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumLogBatchSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumLogFileSize)).BeginInit();
            this.SuspendLayout();
            // 
            // LblToken
            // 
            this.LblToken.AutoSize = true;
            this.LblToken.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblToken.ForeColor = System.Drawing.Color.Transparent;
            this.LblToken.Location = new System.Drawing.Point(12, 9);
            this.LblToken.Name = "LblToken";
            this.LblToken.Size = new System.Drawing.Size(181, 21);
            this.LblToken.TabIndex = 1;
            this.LblToken.Text = "コマンドConfigフォルダパス";
            // 
            // TxtConfig
            // 
            this.TxtConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtConfig.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtConfig.Location = new System.Drawing.Point(12, 33);
            this.TxtConfig.Name = "TxtConfig";
            this.TxtConfig.Size = new System.Drawing.Size(806, 23);
            this.TxtConfig.TabIndex = 2;
            // 
            // BtnOpenCmd
            // 
            this.BtnOpenCmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOpenCmd.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnOpenCmd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnOpenCmd.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnOpenCmd.ForeColor = System.Drawing.Color.Transparent;
            this.BtnOpenCmd.Location = new System.Drawing.Point(824, 33);
            this.BtnOpenCmd.Name = "BtnOpenCmd";
            this.BtnOpenCmd.Size = new System.Drawing.Size(48, 23);
            this.BtnOpenCmd.TabIndex = 3;
            this.BtnOpenCmd.Text = "参照";
            this.BtnOpenCmd.UseVisualStyleBackColor = false;
            this.BtnOpenCmd.Click += new System.EventHandler(this.BtnOpenCmd_Click);
            // 
            // FormSetting
            // 
            this.FormSetting.AutoSize = true;
            this.FormSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.FormSetting.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormSetting.ForeColor = System.Drawing.Color.Transparent;
            this.FormSetting.Location = new System.Drawing.Point(12, 62);
            this.FormSetting.Name = "FormSetting";
            this.FormSetting.Size = new System.Drawing.Size(191, 25);
            this.FormSetting.TabIndex = 17;
            this.FormSetting.Text = "前回フォーム位置を記憶";
            this.FormSetting.UseVisualStyleBackColor = true;
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.ForeColor = System.Drawing.Color.Transparent;
            this.BtnSave.Location = new System.Drawing.Point(12, 418);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(62, 31);
            this.BtnSave.TabIndex = 18;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = false;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // ChkDeleteCommandMessage
            // 
            this.ChkDeleteCommandMessage.AutoSize = true;
            this.ChkDeleteCommandMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkDeleteCommandMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkDeleteCommandMessage.ForeColor = System.Drawing.Color.Transparent;
            this.ChkDeleteCommandMessage.Location = new System.Drawing.Point(12, 93);
            this.ChkDeleteCommandMessage.Name = "ChkDeleteCommandMessage";
            this.ChkDeleteCommandMessage.Size = new System.Drawing.Size(283, 25);
            this.ChkDeleteCommandMessage.TabIndex = 19;
            this.ChkDeleteCommandMessage.Text = "入力されたコマンドの自動削除を有効 ";
            this.ChkDeleteCommandMessage.UseVisualStyleBackColor = true;
            // 
            // ChkDebugLog
            // 
            this.ChkDebugLog.AutoSize = true;
            this.ChkDebugLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkDebugLog.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkDebugLog.ForeColor = System.Drawing.Color.Transparent;
            this.ChkDebugLog.Location = new System.Drawing.Point(6, 18);
            this.ChkDebugLog.Name = "ChkDebugLog";
            this.ChkDebugLog.Size = new System.Drawing.Size(267, 25);
            this.ChkDebugLog.TabIndex = 20;
            this.ChkDebugLog.Text = "コンソールのデバッグログ出力を有効 ";
            this.ChkDebugLog.UseVisualStyleBackColor = true;
            // 
            // GrpLogSettings
            // 
            this.GrpLogSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GrpLogSettings.Controls.Add(this.BtnOpenFolder);
            this.GrpLogSettings.Controls.Add(this.TxtLogFolder);
            this.GrpLogSettings.Controls.Add(this.label1);
            this.GrpLogSettings.Controls.Add(this.NumLogFileSize);
            this.GrpLogSettings.Controls.Add(this.LblLogFileSize);
            this.GrpLogSettings.Controls.Add(this.ChkWriteLogFile);
            this.GrpLogSettings.Controls.Add(this.LblUITaskDelayMs);
            this.GrpLogSettings.Controls.Add(this.NumUITaskDelayMs);
            this.GrpLogSettings.Controls.Add(this.LblLogBatchSize);
            this.GrpLogSettings.Controls.Add(this.NumLogBatchSize);
            this.GrpLogSettings.Controls.Add(this.ChkDebugLog);
            this.GrpLogSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GrpLogSettings.ForeColor = System.Drawing.Color.White;
            this.GrpLogSettings.Location = new System.Drawing.Point(12, 124);
            this.GrpLogSettings.Name = "GrpLogSettings";
            this.GrpLogSettings.Size = new System.Drawing.Size(860, 288);
            this.GrpLogSettings.TabIndex = 21;
            this.GrpLogSettings.TabStop = false;
            this.GrpLogSettings.Text = "コンソールログ設定";
            // 
            // ChkWriteLogFile
            // 
            this.ChkWriteLogFile.AutoSize = true;
            this.ChkWriteLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkWriteLogFile.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkWriteLogFile.ForeColor = System.Drawing.Color.Transparent;
            this.ChkWriteLogFile.Location = new System.Drawing.Point(6, 138);
            this.ChkWriteLogFile.Name = "ChkWriteLogFile";
            this.ChkWriteLogFile.Size = new System.Drawing.Size(179, 25);
            this.ChkWriteLogFile.TabIndex = 78;
            this.ChkWriteLogFile.Text = "ログファイル出力を有効";
            this.ChkWriteLogFile.UseVisualStyleBackColor = true;
            this.ChkWriteLogFile.CheckedChanged += new System.EventHandler(this.ChkWriteLogFile_CheckedChanged);
            // 
            // LblUITaskDelayMs
            // 
            this.LblUITaskDelayMs.AutoSize = true;
            this.LblUITaskDelayMs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblUITaskDelayMs.Location = new System.Drawing.Point(6, 98);
            this.LblUITaskDelayMs.Name = "LblUITaskDelayMs";
            this.LblUITaskDelayMs.Size = new System.Drawing.Size(124, 21);
            this.LblUITaskDelayMs.TabIndex = 77;
            this.LblUITaskDelayMs.Text = "UI待機時間[ms]";
            // 
            // NumUITaskDelayMs
            // 
            this.NumUITaskDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumUITaskDelayMs.Location = new System.Drawing.Point(136, 98);
            this.NumUITaskDelayMs.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.NumUITaskDelayMs.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumUITaskDelayMs.Name = "NumUITaskDelayMs";
            this.NumUITaskDelayMs.Size = new System.Drawing.Size(81, 23);
            this.NumUITaskDelayMs.TabIndex = 76;
            this.NumUITaskDelayMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumUITaskDelayMs.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // LblLogBatchSize
            // 
            this.LblLogBatchSize.AutoSize = true;
            this.LblLogBatchSize.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblLogBatchSize.Location = new System.Drawing.Point(6, 58);
            this.LblLogBatchSize.Name = "LblLogBatchSize";
            this.LblLogBatchSize.Size = new System.Drawing.Size(112, 21);
            this.LblLogBatchSize.TabIndex = 75;
            this.LblLogBatchSize.Text = "ログバッチサイズ";
            // 
            // NumLogBatchSize
            // 
            this.NumLogBatchSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumLogBatchSize.Location = new System.Drawing.Point(124, 58);
            this.NumLogBatchSize.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NumLogBatchSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumLogBatchSize.Name = "NumLogBatchSize";
            this.NumLogBatchSize.Size = new System.Drawing.Size(81, 23);
            this.NumLogBatchSize.TabIndex = 74;
            this.NumLogBatchSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumLogBatchSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // LblLogFileSize
            // 
            this.LblLogFileSize.AutoSize = true;
            this.LblLogFileSize.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblLogFileSize.Location = new System.Drawing.Point(6, 178);
            this.LblLogFileSize.Name = "LblLogFileSize";
            this.LblLogFileSize.Size = new System.Drawing.Size(156, 21);
            this.LblLogFileSize.TabIndex = 79;
            this.LblLogFileSize.Text = "ログファイル出力サイズ";
            // 
            // NumLogFileSize
            // 
            this.NumLogFileSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumLogFileSize.Location = new System.Drawing.Point(168, 178);
            this.NumLogFileSize.Maximum = new decimal(new int[] {
            5120,
            0,
            0,
            0});
            this.NumLogFileSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumLogFileSize.Name = "NumLogFileSize";
            this.NumLogFileSize.Size = new System.Drawing.Size(81, 23);
            this.NumLogFileSize.TabIndex = 80;
            this.NumLogFileSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NumLogFileSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(6, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 21);
            this.label1.TabIndex = 81;
            this.label1.Text = "ログフォルダパス";
            // 
            // TxtLogFolder
            // 
            this.TxtLogFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtLogFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtLogFolder.Location = new System.Drawing.Point(6, 259);
            this.TxtLogFolder.Name = "TxtLogFolder";
            this.TxtLogFolder.Size = new System.Drawing.Size(794, 23);
            this.TxtLogFolder.TabIndex = 82;
            // 
            // BtnOpenFolder
            // 
            this.BtnOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOpenFolder.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnOpenFolder.ForeColor = System.Drawing.Color.Transparent;
            this.BtnOpenFolder.Location = new System.Drawing.Point(806, 259);
            this.BtnOpenFolder.Name = "BtnOpenFolder";
            this.BtnOpenFolder.Size = new System.Drawing.Size(48, 23);
            this.BtnOpenFolder.TabIndex = 83;
            this.BtnOpenFolder.Text = "参照";
            this.BtnOpenFolder.UseVisualStyleBackColor = false;
            this.BtnOpenFolder.Click += new System.EventHandler(this.BtnOpenFolder_Click);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkBlue;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.GrpLogSettings);
            this.Controls.Add(this.ChkDeleteCommandMessage);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.FormSetting);
            this.Controls.Add(this.BtnOpenCmd);
            this.Controls.Add(this.TxtConfig);
            this.Controls.Add(this.LblToken);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 500);
            this.Name = "Setting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "設定プロパティ";
            this.Activated += new System.EventHandler(this.Setting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Setting_FormClosing);
            this.Load += new System.EventHandler(this.Setting_Load);
            this.GrpLogSettings.ResumeLayout(false);
            this.GrpLogSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumUITaskDelayMs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumLogBatchSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumLogFileSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblToken;
        private System.Windows.Forms.TextBox TxtConfig;
        private System.Windows.Forms.Button BtnOpenCmd;
        private System.Windows.Forms.CheckBox FormSetting;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.CheckBox ChkDeleteCommandMessage;
        private System.Windows.Forms.CheckBox ChkDebugLog;
        private System.Windows.Forms.GroupBox GrpLogSettings;
        private System.Windows.Forms.Label LblUITaskDelayMs;
        private System.Windows.Forms.NumericUpDown NumUITaskDelayMs;
        private System.Windows.Forms.Label LblLogBatchSize;
        private System.Windows.Forms.NumericUpDown NumLogBatchSize;
        private System.Windows.Forms.CheckBox ChkWriteLogFile;
        private System.Windows.Forms.NumericUpDown NumLogFileSize;
        private System.Windows.Forms.Label LblLogFileSize;
        private System.Windows.Forms.Button BtnOpenFolder;
        private System.Windows.Forms.TextBox TxtLogFolder;
        private System.Windows.Forms.Label label1;
    }
}
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
            this.BtnStart = new System.Windows.Forms.Button();
            this.FormSetting = new System.Windows.Forms.CheckBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.ChkDeleteCommandMessage = new System.Windows.Forms.CheckBox();
            this.ChkDebugLog = new System.Windows.Forms.CheckBox();
            this.GrpLogSettings = new System.Windows.Forms.GroupBox();
            this.LblUITaskDelayMs = new System.Windows.Forms.Label();
            this.NumUITaskDelayMs = new System.Windows.Forms.NumericUpDown();
            this.LblLogBatchSize = new System.Windows.Forms.Label();
            this.NumLogBatchSize = new System.Windows.Forms.NumericUpDown();
            this.ChkWriteLogFile = new System.Windows.Forms.CheckBox();
            this.GrpLogSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumUITaskDelayMs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumLogBatchSize)).BeginInit();
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
            // BtnStart
            // 
            this.BtnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnStart.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnStart.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnStart.ForeColor = System.Drawing.Color.Transparent;
            this.BtnStart.Location = new System.Drawing.Point(824, 33);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(48, 23);
            this.BtnStart.TabIndex = 3;
            this.BtnStart.Text = "参照";
            this.BtnStart.UseVisualStyleBackColor = false;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
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
            // ChkWriteLogFile
            // 
            this.ChkWriteLogFile.AutoSize = true;
            this.ChkWriteLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkWriteLogFile.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkWriteLogFile.ForeColor = System.Drawing.Color.Transparent;
            this.ChkWriteLogFile.Location = new System.Drawing.Point(6, 137);
            this.ChkWriteLogFile.Name = "ChkWriteLogFile";
            this.ChkWriteLogFile.Size = new System.Drawing.Size(179, 25);
            this.ChkWriteLogFile.TabIndex = 78;
            this.ChkWriteLogFile.Text = "ログファイル出力を有効";
            this.ChkWriteLogFile.UseVisualStyleBackColor = true;
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
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.TxtConfig);
            this.Controls.Add(this.LblToken);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 250);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblToken;
        private System.Windows.Forms.TextBox TxtConfig;
        private System.Windows.Forms.Button BtnStart;
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
    }
}
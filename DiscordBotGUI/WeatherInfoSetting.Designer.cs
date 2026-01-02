namespace DiscordBotGUI
{
    partial class WeatherSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeatherSetting));
            this.NumCacheDuration = new System.Windows.Forms.NumericUpDown();
            this.LblCacheDuration = new System.Windows.Forms.Label();
            this.BtnSave = new System.Windows.Forms.Button();
            this.BtnClearCache = new System.Windows.Forms.Button();
            this.TxtApiKey = new System.Windows.Forms.TextBox();
            this.LblApiKey = new System.Windows.Forms.Label();
            this.NumDeleteDelayMs = new System.Windows.Forms.NumericUpDown();
            this.ChkAutoDeleteEnabled = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NumCacheDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).BeginInit();
            this.SuspendLayout();
            // 
            // NumCacheDuration
            // 
            this.NumCacheDuration.BackColor = System.Drawing.SystemColors.Window;
            this.NumCacheDuration.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumCacheDuration.Location = new System.Drawing.Point(309, 79);
            this.NumCacheDuration.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.NumCacheDuration.Name = "NumCacheDuration";
            this.NumCacheDuration.Size = new System.Drawing.Size(63, 23);
            this.NumCacheDuration.TabIndex = 71;
            this.NumCacheDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LblCacheDuration
            // 
            this.LblCacheDuration.AutoSize = true;
            this.LblCacheDuration.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCacheDuration.ForeColor = System.Drawing.Color.Transparent;
            this.LblCacheDuration.Location = new System.Drawing.Point(12, 79);
            this.LblCacheDuration.Name = "LblCacheDuration";
            this.LblCacheDuration.Size = new System.Drawing.Size(291, 21);
            this.LblCacheDuration.TabIndex = 74;
            this.LblCacheDuration.Text = "取得したデータのキャッシュ時間を設定[分]";
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.ForeColor = System.Drawing.Color.Transparent;
            this.BtnSave.Location = new System.Drawing.Point(12, 318);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(65, 31);
            this.BtnSave.TabIndex = 75;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = false;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // BtnClearCache
            // 
            this.BtnClearCache.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnClearCache.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnClearCache.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnClearCache.ForeColor = System.Drawing.Color.Yellow;
            this.BtnClearCache.Location = new System.Drawing.Point(12, 103);
            this.BtnClearCache.Name = "BtnClearCache";
            this.BtnClearCache.Size = new System.Drawing.Size(158, 31);
            this.BtnClearCache.TabIndex = 76;
            this.BtnClearCache.Text = "キャッシュデータ削除";
            this.BtnClearCache.UseVisualStyleBackColor = false;
            this.BtnClearCache.Click += new System.EventHandler(this.BtnClearCache_Click);
            // 
            // TxtApiKey
            // 
            this.TxtApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtApiKey.BackColor = System.Drawing.SystemColors.Control;
            this.TxtApiKey.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtApiKey.ForeColor = System.Drawing.Color.Black;
            this.TxtApiKey.Location = new System.Drawing.Point(12, 33);
            this.TxtApiKey.Name = "TxtApiKey";
            this.TxtApiKey.Size = new System.Drawing.Size(360, 23);
            this.TxtApiKey.TabIndex = 77;
            // 
            // LblApiKey
            // 
            this.LblApiKey.AutoSize = true;
            this.LblApiKey.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblApiKey.ForeColor = System.Drawing.Color.Transparent;
            this.LblApiKey.Location = new System.Drawing.Point(12, 9);
            this.LblApiKey.Name = "LblApiKey";
            this.LblApiKey.Size = new System.Drawing.Size(108, 21);
            this.LblApiKey.TabIndex = 78;
            this.LblApiKey.Text = "APIキーを入力";
            // 
            // NumDeleteDelayMs
            // 
            this.NumDeleteDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteDelayMs.Location = new System.Drawing.Point(12, 180);
            this.NumDeleteDelayMs.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.NumDeleteDelayMs.Name = "NumDeleteDelayMs";
            this.NumDeleteDelayMs.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteDelayMs.TabIndex = 80;
            this.NumDeleteDelayMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ChkAutoDeleteEnabled
            // 
            this.ChkAutoDeleteEnabled.AutoSize = true;
            this.ChkAutoDeleteEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkAutoDeleteEnabled.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkAutoDeleteEnabled.ForeColor = System.Drawing.Color.Transparent;
            this.ChkAutoDeleteEnabled.Location = new System.Drawing.Point(12, 149);
            this.ChkAutoDeleteEnabled.Name = "ChkAutoDeleteEnabled";
            this.ChkAutoDeleteEnabled.Size = new System.Drawing.Size(283, 25);
            this.ChkAutoDeleteEnabled.TabIndex = 79;
            this.ChkAutoDeleteEnabled.Text = "プロンプトMSGの自動削除を有効[ms]";
            this.ChkAutoDeleteEnabled.UseVisualStyleBackColor = true;
            this.ChkAutoDeleteEnabled.CheckedChanged += new System.EventHandler(this.ChkAutoDeleteEnabled_CheckedChanged);
            // 
            // WeatherInfoSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkBlue;
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.NumDeleteDelayMs);
            this.Controls.Add(this.ChkAutoDeleteEnabled);
            this.Controls.Add(this.LblApiKey);
            this.Controls.Add(this.TxtApiKey);
            this.Controls.Add(this.BtnClearCache);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.LblCacheDuration);
            this.Controls.Add(this.NumCacheDuration);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "WeatherInfoSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "天気予報設定";
            this.Activated += new System.EventHandler(this.WeatherInfoSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WeatherInfoSetting_FormClosing);
            this.Load += new System.EventHandler(this.WeatherInfoSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NumCacheDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NumCacheDuration;
        private System.Windows.Forms.Label LblCacheDuration;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.Button BtnClearCache;
        private System.Windows.Forms.TextBox TxtApiKey;
        private System.Windows.Forms.Label LblApiKey;
        private System.Windows.Forms.NumericUpDown NumDeleteDelayMs;
        private System.Windows.Forms.CheckBox ChkAutoDeleteEnabled;
    }
}
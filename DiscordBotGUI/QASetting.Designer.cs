namespace DiscordBotGUI
{
    partial class QASetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QASetting));
            this.PnlColorPreview = new System.Windows.Forms.Panel();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.BtnSelectColor = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.NumTimeoutMinutes = new System.Windows.Forms.NumericUpDown();
            this.TxtEmbedColor = new System.Windows.Forms.TextBox();
            this.LblEmbedColor = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ChkAllowMultipleVotes = new System.Windows.Forms.CheckBox();
            this.ChkAutoDeleteEnabled = new System.Windows.Forms.CheckBox();
            this.NumDeleteDelayMs = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).BeginInit();
            this.SuspendLayout();
            // 
            // PnlColorPreview
            // 
            this.PnlColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlColorPreview.Location = new System.Drawing.Point(71, 33);
            this.PnlColorPreview.Name = "PnlColorPreview";
            this.PnlColorPreview.Size = new System.Drawing.Size(53, 23);
            this.PnlColorPreview.TabIndex = 67;
            // 
            // BtnSelectColor
            // 
            this.BtnSelectColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSelectColor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSelectColor.Location = new System.Drawing.Point(12, 33);
            this.BtnSelectColor.Name = "BtnSelectColor";
            this.BtnSelectColor.Size = new System.Drawing.Size(53, 23);
            this.BtnSelectColor.TabIndex = 68;
            this.BtnSelectColor.Text = "カラー";
            this.BtnSelectColor.UseVisualStyleBackColor = true;
            this.BtnSelectColor.Click += new System.EventHandler(this.BtnSelectColor_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(12, 224);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(53, 25);
            this.BtnSave.TabIndex = 69;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // NumTimeoutMinutes
            // 
            this.NumTimeoutMinutes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumTimeoutMinutes.Location = new System.Drawing.Point(283, 79);
            this.NumTimeoutMinutes.Maximum = new decimal(new int[] {
            43200,
            0,
            0,
            0});
            this.NumTimeoutMinutes.Name = "NumTimeoutMinutes";
            this.NumTimeoutMinutes.Size = new System.Drawing.Size(59, 23);
            this.NumTimeoutMinutes.TabIndex = 70;
            this.NumTimeoutMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // TxtEmbedColor
            // 
            this.TxtEmbedColor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmbedColor.Location = new System.Drawing.Point(130, 33);
            this.TxtEmbedColor.Name = "TxtEmbedColor";
            this.TxtEmbedColor.Size = new System.Drawing.Size(114, 23);
            this.TxtEmbedColor.TabIndex = 71;
            // 
            // LblEmbedColor
            // 
            this.LblEmbedColor.AutoSize = true;
            this.LblEmbedColor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblEmbedColor.Location = new System.Drawing.Point(12, 9);
            this.LblEmbedColor.Name = "LblEmbedColor";
            this.LblEmbedColor.Size = new System.Drawing.Size(232, 21);
            this.LblEmbedColor.TabIndex = 72;
            this.LblEmbedColor.Text = "埋め込みメッセージの枠色を設定";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 21);
            this.label1.TabIndex = 73;
            this.label1.Text = "アンケートの投票制限時間を設定[分]";
            // 
            // ChkAllowMultipleVotes
            // 
            this.ChkAllowMultipleVotes.AutoSize = true;
            this.ChkAllowMultipleVotes.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ChkAllowMultipleVotes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkAllowMultipleVotes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ChkAllowMultipleVotes.Location = new System.Drawing.Point(12, 119);
            this.ChkAllowMultipleVotes.Name = "ChkAllowMultipleVotes";
            this.ChkAllowMultipleVotes.Size = new System.Drawing.Size(181, 25);
            this.ChkAllowMultipleVotes.TabIndex = 74;
            this.ChkAllowMultipleVotes.Text = "1人1投票制限を有効 ";
            this.ChkAllowMultipleVotes.UseVisualStyleBackColor = true;
            // 
            // ChkAutoDeleteEnabled
            // 
            this.ChkAutoDeleteEnabled.AutoSize = true;
            this.ChkAutoDeleteEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ChkAutoDeleteEnabled.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkAutoDeleteEnabled.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ChkAutoDeleteEnabled.Location = new System.Drawing.Point(12, 150);
            this.ChkAutoDeleteEnabled.Name = "ChkAutoDeleteEnabled";
            this.ChkAutoDeleteEnabled.Size = new System.Drawing.Size(284, 25);
            this.ChkAutoDeleteEnabled.TabIndex = 75;
            this.ChkAutoDeleteEnabled.Text = "プロンプトMSGの自動削除を有効[ms]";
            this.ChkAutoDeleteEnabled.UseVisualStyleBackColor = true;
            this.ChkAutoDeleteEnabled.CheckedChanged += new System.EventHandler(this.ChkAutoDeleteEnabled_CheckedChanged);
            // 
            // NumDeleteDelayMs
            // 
            this.NumDeleteDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteDelayMs.Location = new System.Drawing.Point(302, 152);
            this.NumDeleteDelayMs.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.NumDeleteDelayMs.Name = "NumDeleteDelayMs";
            this.NumDeleteDelayMs.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteDelayMs.TabIndex = 76;
            this.NumDeleteDelayMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // QASetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.NumDeleteDelayMs);
            this.Controls.Add(this.ChkAutoDeleteEnabled);
            this.Controls.Add(this.ChkAllowMultipleVotes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblEmbedColor);
            this.Controls.Add(this.TxtEmbedColor);
            this.Controls.Add(this.NumTimeoutMinutes);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.BtnSelectColor);
            this.Controls.Add(this.PnlColorPreview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "QASetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "アンケートQA設定";
            this.Activated += new System.EventHandler(this.QASetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QASetting_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.NumTimeoutMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel PnlColorPreview;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button BtnSelectColor;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.NumericUpDown NumTimeoutMinutes;
        private System.Windows.Forms.TextBox TxtEmbedColor;
        private System.Windows.Forms.Label LblEmbedColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ChkAllowMultipleVotes;
        private System.Windows.Forms.CheckBox ChkAutoDeleteEnabled;
        private System.Windows.Forms.NumericUpDown NumDeleteDelayMs;
    }
}
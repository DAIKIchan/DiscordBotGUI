namespace DiscordBotGUI
{
    partial class JoiningLeavingSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoiningLeavingSetting));
            this.TxtChannelId = new System.Windows.Forms.TextBox();
            this.LblChannelId = new System.Windows.Forms.Label();
            this.BtnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TxtChannelId
            // 
            this.TxtChannelId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtChannelId.BackColor = System.Drawing.SystemColors.Control;
            this.TxtChannelId.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtChannelId.ForeColor = System.Drawing.Color.Black;
            this.TxtChannelId.Location = new System.Drawing.Point(12, 33);
            this.TxtChannelId.Name = "TxtChannelId";
            this.TxtChannelId.Size = new System.Drawing.Size(260, 23);
            this.TxtChannelId.TabIndex = 2;
            // 
            // LblChannelId
            // 
            this.LblChannelId.AutoSize = true;
            this.LblChannelId.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblChannelId.ForeColor = System.Drawing.Color.Transparent;
            this.LblChannelId.Location = new System.Drawing.Point(12, 9);
            this.LblChannelId.Name = "LblChannelId";
            this.LblChannelId.Size = new System.Drawing.Size(200, 21);
            this.LblChannelId.TabIndex = 3;
            this.LblChannelId.Text = "入退室ログ送信チャンネルID";
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.ForeColor = System.Drawing.Color.Transparent;
            this.BtnSave.Location = new System.Drawing.Point(12, 218);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(62, 31);
            this.BtnSave.TabIndex = 8;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = false;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // JoiningLeavingSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkBlue;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.LblChannelId);
            this.Controls.Add(this.TxtChannelId);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(250, 200);
            this.Name = "JoiningLeavingSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "入退出設定";
            this.Activated += new System.EventHandler(this.JoiningLeavingSetting_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JoiningLeavingSetting_FormClosing);
            this.Load += new System.EventHandler(this.JoiningLeavingSetting_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtChannelId;
        private System.Windows.Forms.Label LblChannelId;
        private System.Windows.Forms.Button BtnSave;
    }
}
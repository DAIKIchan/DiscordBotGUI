namespace DiscordBotGUI
{
    partial class Edit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Edit));
            this.label1 = new System.Windows.Forms.Label();
            this.TxtSendMessage = new System.Windows.Forms.TextBox();
            this.EditTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.PnlColorPreview = new System.Windows.Forms.Panel();
            this.TxtEmbedColorHex = new System.Windows.Forms.TextBox();
            this.BtnSelectColor = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TxtEmbedTitle = new System.Windows.Forms.TextBox();
            this.TxtEmbedDescription = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.BtnAdd = new System.Windows.Forms.Button();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.CmdListBox = new System.Windows.Forms.ListBox();
            this.BtnSave = new System.Windows.Forms.Button();
            this.TxtCommandName = new System.Windows.Forms.TextBox();
            this.LblToken = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.EditTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "送信内容";
            // 
            // TxtSendMessage
            // 
            this.TxtSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSendMessage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtSendMessage.Location = new System.Drawing.Point(6, 27);
            this.TxtSendMessage.Multiline = true;
            this.TxtSendMessage.Name = "TxtSendMessage";
            this.TxtSendMessage.Size = new System.Drawing.Size(840, 289);
            this.TxtSendMessage.TabIndex = 5;
            // 
            // EditTabControl
            // 
            this.EditTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EditTabControl.Controls.Add(this.tabPage1);
            this.EditTabControl.Controls.Add(this.tabPage3);
            this.EditTabControl.Controls.Add(this.tabPage2);
            this.EditTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditTabControl.Location = new System.Drawing.Point(12, 12);
            this.EditTabControl.Name = "EditTabControl";
            this.EditTabControl.SelectedIndex = 0;
            this.EditTabControl.Size = new System.Drawing.Size(860, 350);
            this.EditTabControl.TabIndex = 57;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.TxtSendMessage);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(852, 322);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "メッセージ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.PnlColorPreview);
            this.tabPage3.Controls.Add(this.TxtEmbedColorHex);
            this.tabPage3.Controls.Add(this.BtnSelectColor);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.TxtEmbedTitle);
            this.tabPage3.Controls.Add(this.TxtEmbedDescription);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(852, 322);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "埋め込みメッセージ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // PnlColorPreview
            // 
            this.PnlColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlColorPreview.Location = new System.Drawing.Point(65, 50);
            this.PnlColorPreview.Name = "PnlColorPreview";
            this.PnlColorPreview.Size = new System.Drawing.Size(53, 23);
            this.PnlColorPreview.TabIndex = 66;
            // 
            // TxtEmbedColorHex
            // 
            this.TxtEmbedColorHex.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmbedColorHex.Location = new System.Drawing.Point(124, 50);
            this.TxtEmbedColorHex.Name = "TxtEmbedColorHex";
            this.TxtEmbedColorHex.ReadOnly = true;
            this.TxtEmbedColorHex.Size = new System.Drawing.Size(89, 23);
            this.TxtEmbedColorHex.TabIndex = 60;
            this.TxtEmbedColorHex.TextChanged += new System.EventHandler(this.TxtEmbedColorHex_TextChanged);
            // 
            // BtnSelectColor
            // 
            this.BtnSelectColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSelectColor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSelectColor.Location = new System.Drawing.Point(6, 50);
            this.BtnSelectColor.Name = "BtnSelectColor";
            this.BtnSelectColor.Size = new System.Drawing.Size(53, 23);
            this.BtnSelectColor.TabIndex = 60;
            this.BtnSelectColor.Text = "カラー";
            this.BtnSelectColor.UseVisualStyleBackColor = true;
            this.BtnSelectColor.Click += new System.EventHandler(this.BtnSelectColor_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 21);
            this.label2.TabIndex = 65;
            this.label2.Text = "タイトル名";
            // 
            // TxtEmbedTitle
            // 
            this.TxtEmbedTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtEmbedTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmbedTitle.Location = new System.Drawing.Point(89, 4);
            this.TxtEmbedTitle.Name = "TxtEmbedTitle";
            this.TxtEmbedTitle.Size = new System.Drawing.Size(757, 23);
            this.TxtEmbedTitle.TabIndex = 64;
            // 
            // TxtEmbedDescription
            // 
            this.TxtEmbedDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtEmbedDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmbedDescription.Location = new System.Drawing.Point(6, 116);
            this.TxtEmbedDescription.Multiline = true;
            this.TxtEmbedDescription.Name = "TxtEmbedDescription";
            this.TxtEmbedDescription.Size = new System.Drawing.Size(840, 201);
            this.TxtEmbedDescription.TabIndex = 63;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 21);
            this.label5.TabIndex = 62;
            this.label5.Text = "送信内容";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.BtnAdd);
            this.tabPage2.Controls.Add(this.BtnDelete);
            this.tabPage2.Controls.Add(this.CmdListBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(852, 322);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "コマンド一覧";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // BtnAdd
            // 
            this.BtnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnAdd.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnAdd.Location = new System.Drawing.Point(6, 6);
            this.BtnAdd.Name = "BtnAdd";
            this.BtnAdd.Size = new System.Drawing.Size(118, 34);
            this.BtnAdd.TabIndex = 61;
            this.BtnAdd.Text = "コマンドの追加";
            this.BtnAdd.UseVisualStyleBackColor = true;
            this.BtnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // BtnDelete
            // 
            this.BtnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDelete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnDelete.Location = new System.Drawing.Point(130, 6);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(118, 34);
            this.BtnDelete.TabIndex = 60;
            this.BtnDelete.Text = "コマンドの削除";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // CmdListBox
            // 
            this.CmdListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CmdListBox.BackColor = System.Drawing.Color.Black;
            this.CmdListBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmdListBox.ForeColor = System.Drawing.Color.Transparent;
            this.CmdListBox.FormattingEnabled = true;
            this.CmdListBox.ItemHeight = 17;
            this.CmdListBox.Location = new System.Drawing.Point(6, 46);
            this.CmdListBox.Name = "CmdListBox";
            this.CmdListBox.Size = new System.Drawing.Size(840, 259);
            this.CmdListBox.TabIndex = 0;
            this.CmdListBox.SelectedIndexChanged += new System.EventHandler(this.CmdListBox_SelectedIndexChanged);
            this.CmdListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CmdListBox_MouseDoubleClick);
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(16, 424);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(53, 25);
            this.BtnSave.TabIndex = 7;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // TxtCommandName
            // 
            this.TxtCommandName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TxtCommandName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtCommandName.Location = new System.Drawing.Point(95, 364);
            this.TxtCommandName.Name = "TxtCommandName";
            this.TxtCommandName.Size = new System.Drawing.Size(273, 23);
            this.TxtCommandName.TabIndex = 59;
            // 
            // LblToken
            // 
            this.LblToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LblToken.AutoSize = true;
            this.LblToken.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblToken.Location = new System.Drawing.Point(12, 365);
            this.LblToken.Name = "LblToken";
            this.LblToken.Size = new System.Drawing.Size(77, 21);
            this.LblToken.TabIndex = 58;
            this.LblToken.Text = "コマンド名";
            // 
            // Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.TxtCommandName);
            this.Controls.Add(this.LblToken);
            this.Controls.Add(this.EditTabControl);
            this.Controls.Add(this.BtnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "Edit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "編集プロパティ";
            this.Activated += new System.EventHandler(this.Edit_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Edit_FormClosing);
            this.Load += new System.EventHandler(this.Edit_Load);
            this.EditTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtSendMessage;
        private System.Windows.Forms.TabControl EditTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox TxtCommandName;
        private System.Windows.Forms.Label LblToken;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TxtEmbedTitle;
        private System.Windows.Forms.TextBox TxtEmbedDescription;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TxtEmbedColorHex;
        private System.Windows.Forms.Button BtnSelectColor;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel PnlColorPreview;
        private System.Windows.Forms.ListBox CmdListBox;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnAdd;
    }
}
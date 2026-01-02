namespace DiscordBotGUI
{
    partial class RoleSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RoleSetting));
            this.BtnSave = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.LblRoleList = new System.Windows.Forms.Label();
            this.CmbRoleList = new System.Windows.Forms.ComboBox();
            this.LblRole = new System.Windows.Forms.Label();
            this.TxtRoleName = new System.Windows.Forms.TextBox();
            this.BtnCreateRole = new System.Windows.Forms.Button();
            this.pnlScroll = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ClbVoice = new System.Windows.Forms.CheckedListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ClbText = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ClbMembership = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ClbGeneral = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ChkListBoxRoles = new System.Windows.Forms.CheckedListBox();
            this.LblRolePermission = new System.Windows.Forms.Label();
            this.LblRoleServer = new System.Windows.Forms.Label();
            this.CmbGuilds = new System.Windows.Forms.ComboBox();
            this.RoleTabControl = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.DgvPanels = new System.Windows.Forms.DataGridView();
            this.ChkGlobalPermanent = new System.Windows.Forms.CheckBox();
            this.ColEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColMsgid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColRoleinfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.BtnDeleteRole = new System.Windows.Forms.Button();
            this.BtnRoleSave = new System.Windows.Forms.Button();
            this.ChkSelectAll = new System.Windows.Forms.CheckBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.LblEmbedColor = new System.Windows.Forms.Label();
            this.TxtEmbedColor = new System.Windows.Forms.TextBox();
            this.BtnSelectColor = new System.Windows.Forms.Button();
            this.PnlColorPreview = new System.Windows.Forms.Panel();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.NumDeleteDelayMs = new System.Windows.Forms.NumericUpDown();
            this.ChkAutoDeleteEnabled = new System.Windows.Forms.CheckBox();
            this.tabPage3.SuspendLayout();
            this.pnlScroll.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.RoleTabControl.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvPanels)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSave.Location = new System.Drawing.Point(12, 424);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(53, 25);
            this.BtnSave.TabIndex = 88;
            this.BtnSave.Text = "保存";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.BackColor = System.Drawing.Color.Navy;
            this.tabPage3.Controls.Add(this.ChkSelectAll);
            this.tabPage3.Controls.Add(this.BtnRoleSave);
            this.tabPage3.Controls.Add(this.BtnDeleteRole);
            this.tabPage3.Controls.Add(this.LblRoleList);
            this.tabPage3.Controls.Add(this.CmbRoleList);
            this.tabPage3.Controls.Add(this.LblRole);
            this.tabPage3.Controls.Add(this.TxtRoleName);
            this.tabPage3.Controls.Add(this.BtnCreateRole);
            this.tabPage3.Controls.Add(this.pnlScroll);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(852, 378);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "ロール権限設定";
            // 
            // LblRoleList
            // 
            this.LblRoleList.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRoleList.ForeColor = System.Drawing.Color.White;
            this.LblRoleList.Location = new System.Drawing.Point(6, 63);
            this.LblRoleList.Name = "LblRoleList";
            this.LblRoleList.Size = new System.Drawing.Size(84, 21);
            this.LblRoleList.TabIndex = 91;
            this.LblRoleList.Text = "ロール一覧";
            // 
            // CmbRoleList
            // 
            this.CmbRoleList.BackColor = System.Drawing.Color.Navy;
            this.CmbRoleList.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CmbRoleList.ForeColor = System.Drawing.Color.White;
            this.CmbRoleList.FormattingEnabled = true;
            this.CmbRoleList.Location = new System.Drawing.Point(6, 87);
            this.CmbRoleList.Name = "CmbRoleList";
            this.CmbRoleList.Size = new System.Drawing.Size(169, 23);
            this.CmbRoleList.TabIndex = 90;
            this.CmbRoleList.SelectedIndexChanged += new System.EventHandler(this.CmbRoleList_SelectedIndexChanged);
            // 
            // LblRole
            // 
            this.LblRole.BackColor = System.Drawing.Color.Transparent;
            this.LblRole.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRole.ForeColor = System.Drawing.Color.White;
            this.LblRole.Location = new System.Drawing.Point(6, 3);
            this.LblRole.Name = "LblRole";
            this.LblRole.Size = new System.Drawing.Size(67, 21);
            this.LblRole.TabIndex = 61;
            this.LblRole.Text = "ロール名";
            // 
            // TxtRoleName
            // 
            this.TxtRoleName.BackColor = System.Drawing.Color.Navy;
            this.TxtRoleName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtRoleName.ForeColor = System.Drawing.Color.White;
            this.TxtRoleName.Location = new System.Drawing.Point(6, 27);
            this.TxtRoleName.Name = "TxtRoleName";
            this.TxtRoleName.Size = new System.Drawing.Size(273, 23);
            this.TxtRoleName.TabIndex = 60;
            // 
            // BtnCreateRole
            // 
            this.BtnCreateRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnCreateRole.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnCreateRole.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnCreateRole.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnCreateRole.ForeColor = System.Drawing.Color.White;
            this.BtnCreateRole.Location = new System.Drawing.Point(6, 347);
            this.BtnCreateRole.Name = "BtnCreateRole";
            this.BtnCreateRole.Size = new System.Drawing.Size(90, 25);
            this.BtnCreateRole.TabIndex = 89;
            this.BtnCreateRole.Text = "ロール作成";
            this.BtnCreateRole.UseVisualStyleBackColor = false;
            this.BtnCreateRole.Click += new System.EventHandler(this.BtnCreateRole_Click);
            // 
            // pnlScroll
            // 
            this.pnlScroll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlScroll.AutoScroll = true;
            this.pnlScroll.Controls.Add(this.groupBox4);
            this.pnlScroll.Controls.Add(this.groupBox3);
            this.pnlScroll.Controls.Add(this.groupBox2);
            this.pnlScroll.Controls.Add(this.groupBox1);
            this.pnlScroll.Location = new System.Drawing.Point(6, 116);
            this.pnlScroll.Name = "pnlScroll";
            this.pnlScroll.Size = new System.Drawing.Size(840, 225);
            this.pnlScroll.TabIndex = 1;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.ClbVoice);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(3, 549);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(817, 176);
            this.groupBox4.TabIndex = 91;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ボイス・イベント";
            // 
            // ClbVoice
            // 
            this.ClbVoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClbVoice.BackColor = System.Drawing.Color.Navy;
            this.ClbVoice.ForeColor = System.Drawing.Color.White;
            this.ClbVoice.FormattingEnabled = true;
            this.ClbVoice.Location = new System.Drawing.Point(6, 22);
            this.ClbVoice.Name = "ClbVoice";
            this.ClbVoice.Size = new System.Drawing.Size(805, 148);
            this.ClbVoice.TabIndex = 0;
            this.ClbVoice.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Clb_ItemCheck);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.ClbText);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(3, 367);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(817, 176);
            this.groupBox3.TabIndex = 90;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "テキスト権限";
            // 
            // ClbText
            // 
            this.ClbText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClbText.BackColor = System.Drawing.Color.Navy;
            this.ClbText.ForeColor = System.Drawing.Color.White;
            this.ClbText.FormattingEnabled = true;
            this.ClbText.Location = new System.Drawing.Point(6, 22);
            this.ClbText.Name = "ClbText";
            this.ClbText.Size = new System.Drawing.Size(805, 148);
            this.ClbText.TabIndex = 0;
            this.ClbText.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Clb_ItemCheck);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ClbMembership);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(3, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(817, 176);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "メンバー管理";
            // 
            // ClbMembership
            // 
            this.ClbMembership.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClbMembership.BackColor = System.Drawing.Color.Navy;
            this.ClbMembership.ForeColor = System.Drawing.Color.White;
            this.ClbMembership.FormattingEnabled = true;
            this.ClbMembership.Location = new System.Drawing.Point(6, 22);
            this.ClbMembership.Name = "ClbMembership";
            this.ClbMembership.Size = new System.Drawing.Size(805, 148);
            this.ClbMembership.TabIndex = 0;
            this.ClbMembership.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Clb_ItemCheck);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ClbGeneral);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(817, 176);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一般権限";
            // 
            // ClbGeneral
            // 
            this.ClbGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClbGeneral.BackColor = System.Drawing.Color.Navy;
            this.ClbGeneral.ForeColor = System.Drawing.Color.White;
            this.ClbGeneral.FormattingEnabled = true;
            this.ClbGeneral.Location = new System.Drawing.Point(6, 22);
            this.ClbGeneral.Name = "ClbGeneral";
            this.ClbGeneral.Size = new System.Drawing.Size(805, 148);
            this.ClbGeneral.TabIndex = 0;
            this.ClbGeneral.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.Clb_ItemCheck);
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.Navy;
            this.tabPage2.Controls.Add(this.ChkListBoxRoles);
            this.tabPage2.Controls.Add(this.LblRolePermission);
            this.tabPage2.Controls.Add(this.LblRoleServer);
            this.tabPage2.Controls.Add(this.CmbGuilds);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(852, 378);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ロール許可設定";
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
            this.ChkListBoxRoles.Location = new System.Drawing.Point(6, 98);
            this.ChkListBoxRoles.Name = "ChkListBoxRoles";
            this.ChkListBoxRoles.Size = new System.Drawing.Size(840, 274);
            this.ChkListBoxRoles.TabIndex = 49;
            // 
            // LblRolePermission
            // 
            this.LblRolePermission.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRolePermission.ForeColor = System.Drawing.Color.White;
            this.LblRolePermission.Location = new System.Drawing.Point(7, 74);
            this.LblRolePermission.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblRolePermission.Name = "LblRolePermission";
            this.LblRolePermission.Size = new System.Drawing.Size(158, 21);
            this.LblRolePermission.TabIndex = 48;
            this.LblRolePermission.Text = "許可するロールを選択";
            // 
            // LblRoleServer
            // 
            this.LblRoleServer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblRoleServer.ForeColor = System.Drawing.Color.White;
            this.LblRoleServer.Location = new System.Drawing.Point(7, 3);
            this.LblRoleServer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblRoleServer.Name = "LblRoleServer";
            this.LblRoleServer.Size = new System.Drawing.Size(99, 21);
            this.LblRoleServer.TabIndex = 47;
            this.LblRoleServer.Text = "サーバを選択";
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
            this.CmbGuilds.TabIndex = 46;
            this.CmbGuilds.SelectedIndexChanged += new System.EventHandler(this.CmbGuilds_SelectedIndexChanged);
            // 
            // RoleTabControl
            // 
            this.RoleTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RoleTabControl.Controls.Add(this.tabPage2);
            this.RoleTabControl.Controls.Add(this.tabPage3);
            this.RoleTabControl.Controls.Add(this.tabPage5);
            this.RoleTabControl.Controls.Add(this.tabPage1);
            this.RoleTabControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RoleTabControl.Location = new System.Drawing.Point(12, 12);
            this.RoleTabControl.Name = "RoleTabControl";
            this.RoleTabControl.SelectedIndex = 0;
            this.RoleTabControl.Size = new System.Drawing.Size(860, 406);
            this.RoleTabControl.TabIndex = 85;
            // 
            // tabPage5
            // 
            this.tabPage5.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage5.Controls.Add(this.DgvPanels);
            this.tabPage5.Controls.Add(this.ChkGlobalPermanent);
            this.tabPage5.Location = new System.Drawing.Point(4, 24);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(852, 378);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "ロール情報";
            // 
            // DgvPanels
            // 
            this.DgvPanels.AllowUserToAddRows = false;
            this.DgvPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvPanels.BackgroundColor = System.Drawing.Color.Navy;
            this.DgvPanels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvPanels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColEnabled,
            this.ColMsgid,
            this.ColRoleinfo,
            this.ColDelete});
            this.DgvPanels.Location = new System.Drawing.Point(6, 37);
            this.DgvPanels.Name = "DgvPanels";
            this.DgvPanels.RowTemplate.Height = 21;
            this.DgvPanels.Size = new System.Drawing.Size(840, 335);
            this.DgvPanels.TabIndex = 0;
            this.DgvPanels.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvPanels_CellContentClick);
            // 
            // ChkGlobalPermanent
            // 
            this.ChkGlobalPermanent.AutoSize = true;
            this.ChkGlobalPermanent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkGlobalPermanent.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkGlobalPermanent.ForeColor = System.Drawing.Color.Black;
            this.ChkGlobalPermanent.Location = new System.Drawing.Point(6, 6);
            this.ChkGlobalPermanent.Name = "ChkGlobalPermanent";
            this.ChkGlobalPermanent.Size = new System.Drawing.Size(246, 25);
            this.ChkGlobalPermanent.TabIndex = 84;
            this.ChkGlobalPermanent.Text = "ロール自動付与の永続化を有効";
            this.ChkGlobalPermanent.UseVisualStyleBackColor = true;
            // 
            // ColEnabled
            // 
            this.ColEnabled.FillWeight = 50F;
            this.ColEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ColEnabled.HeaderText = "有効";
            this.ColEnabled.Name = "ColEnabled";
            this.ColEnabled.Width = 50;
            // 
            // ColMsgid
            // 
            this.ColMsgid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColMsgid.FillWeight = 200F;
            this.ColMsgid.HeaderText = "メッセージID";
            this.ColMsgid.MinimumWidth = 100;
            this.ColMsgid.Name = "ColMsgid";
            // 
            // ColRoleinfo
            // 
            this.ColRoleinfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColRoleinfo.FillWeight = 200F;
            this.ColRoleinfo.HeaderText = "ロール情報";
            this.ColRoleinfo.MinimumWidth = 100;
            this.ColRoleinfo.Name = "ColRoleinfo";
            // 
            // ColDelete
            // 
            this.ColDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ColDelete.HeaderText = "操作";
            this.ColDelete.Name = "ColDelete";
            // 
            // BtnDeleteRole
            // 
            this.BtnDeleteRole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnDeleteRole.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnDeleteRole.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnDeleteRole.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnDeleteRole.ForeColor = System.Drawing.Color.Yellow;
            this.BtnDeleteRole.Location = new System.Drawing.Point(198, 347);
            this.BtnDeleteRole.Name = "BtnDeleteRole";
            this.BtnDeleteRole.Size = new System.Drawing.Size(90, 25);
            this.BtnDeleteRole.TabIndex = 92;
            this.BtnDeleteRole.Text = "ロール削除";
            this.BtnDeleteRole.UseVisualStyleBackColor = false;
            this.BtnDeleteRole.Click += new System.EventHandler(this.BtnDeleteRole_Click);
            // 
            // BtnRoleSave
            // 
            this.BtnRoleSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnRoleSave.BackColor = System.Drawing.Color.RoyalBlue;
            this.BtnRoleSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnRoleSave.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnRoleSave.ForeColor = System.Drawing.Color.White;
            this.BtnRoleSave.Location = new System.Drawing.Point(102, 347);
            this.BtnRoleSave.Name = "BtnRoleSave";
            this.BtnRoleSave.Size = new System.Drawing.Size(90, 25);
            this.BtnRoleSave.TabIndex = 93;
            this.BtnRoleSave.Text = "ロール保存";
            this.BtnRoleSave.UseVisualStyleBackColor = false;
            this.BtnRoleSave.Click += new System.EventHandler(this.BtnRoleSave_Click);
            // 
            // ChkSelectAll
            // 
            this.ChkSelectAll.AutoSize = true;
            this.ChkSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChkSelectAll.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkSelectAll.ForeColor = System.Drawing.Color.White;
            this.ChkSelectAll.Location = new System.Drawing.Point(181, 85);
            this.ChkSelectAll.Name = "ChkSelectAll";
            this.ChkSelectAll.Size = new System.Drawing.Size(111, 25);
            this.ChkSelectAll.TabIndex = 94;
            this.ChkSelectAll.Text = "権限全選択";
            this.ChkSelectAll.UseVisualStyleBackColor = true;
            this.ChkSelectAll.CheckedChanged += new System.EventHandler(this.ChkSelectAll_CheckedChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.NumDeleteDelayMs);
            this.tabPage1.Controls.Add(this.ChkAutoDeleteEnabled);
            this.tabPage1.Controls.Add(this.LblEmbedColor);
            this.tabPage1.Controls.Add(this.TxtEmbedColor);
            this.tabPage1.Controls.Add(this.BtnSelectColor);
            this.tabPage1.Controls.Add(this.PnlColorPreview);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(852, 402);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "詳細設定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // LblEmbedColor
            // 
            this.LblEmbedColor.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblEmbedColor.Location = new System.Drawing.Point(6, 3);
            this.LblEmbedColor.Name = "LblEmbedColor";
            this.LblEmbedColor.Size = new System.Drawing.Size(232, 21);
            this.LblEmbedColor.TabIndex = 76;
            this.LblEmbedColor.Text = "埋め込みメッセージの枠色を設定";
            // 
            // TxtEmbedColor
            // 
            this.TxtEmbedColor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtEmbedColor.Location = new System.Drawing.Point(124, 27);
            this.TxtEmbedColor.Name = "TxtEmbedColor";
            this.TxtEmbedColor.Size = new System.Drawing.Size(114, 23);
            this.TxtEmbedColor.TabIndex = 75;
            // 
            // BtnSelectColor
            // 
            this.BtnSelectColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtnSelectColor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnSelectColor.Location = new System.Drawing.Point(6, 27);
            this.BtnSelectColor.Name = "BtnSelectColor";
            this.BtnSelectColor.Size = new System.Drawing.Size(53, 23);
            this.BtnSelectColor.TabIndex = 74;
            this.BtnSelectColor.Text = "カラー";
            this.BtnSelectColor.UseVisualStyleBackColor = true;
            this.BtnSelectColor.Click += new System.EventHandler(this.BtnSelectColor_Click);
            // 
            // PnlColorPreview
            // 
            this.PnlColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PnlColorPreview.Location = new System.Drawing.Point(65, 27);
            this.PnlColorPreview.Name = "PnlColorPreview";
            this.PnlColorPreview.Size = new System.Drawing.Size(53, 23);
            this.PnlColorPreview.TabIndex = 73;
            // 
            // NumDeleteDelayMs
            // 
            this.NumDeleteDelayMs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumDeleteDelayMs.Location = new System.Drawing.Point(6, 102);
            this.NumDeleteDelayMs.Maximum = new decimal(new int[] {
            300000,
            0,
            0,
            0});
            this.NumDeleteDelayMs.Name = "NumDeleteDelayMs";
            this.NumDeleteDelayMs.Size = new System.Drawing.Size(170, 23);
            this.NumDeleteDelayMs.TabIndex = 78;
            this.NumDeleteDelayMs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ChkAutoDeleteEnabled
            // 
            this.ChkAutoDeleteEnabled.AutoSize = true;
            this.ChkAutoDeleteEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ChkAutoDeleteEnabled.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkAutoDeleteEnabled.ForeColor = System.Drawing.SystemColors.ControlText;
            this.ChkAutoDeleteEnabled.Location = new System.Drawing.Point(6, 71);
            this.ChkAutoDeleteEnabled.Name = "ChkAutoDeleteEnabled";
            this.ChkAutoDeleteEnabled.Size = new System.Drawing.Size(284, 25);
            this.ChkAutoDeleteEnabled.TabIndex = 77;
            this.ChkAutoDeleteEnabled.Text = "プロンプトMSGの自動削除を有効[ms]";
            this.ChkAutoDeleteEnabled.UseVisualStyleBackColor = true;
            this.ChkAutoDeleteEnabled.CheckedChanged += new System.EventHandler(this.ChkAutoDeleteEnabled_CheckedChanged);
            // 
            // RoleSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.RoleTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.Name = "RoleSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ロール自動付与プロパティ";
            this.Load += new System.EventHandler(this.RoleSetting_Load);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.pnlScroll.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.RoleTabControl.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DgvPanels)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumDeleteDelayMs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label LblRole;
        private System.Windows.Forms.TextBox TxtRoleName;
        private System.Windows.Forms.Button BtnCreateRole;
        private System.Windows.Forms.Panel pnlScroll;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckedListBox ClbVoice;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckedListBox ClbText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox ClbMembership;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox ClbGeneral;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckedListBox ChkListBoxRoles;
        private System.Windows.Forms.Label LblRolePermission;
        private System.Windows.Forms.Label LblRoleServer;
        private System.Windows.Forms.ComboBox CmbGuilds;
        private System.Windows.Forms.TabControl RoleTabControl;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.DataGridView DgvPanels;
        private System.Windows.Forms.CheckBox ChkGlobalPermanent;
        private System.Windows.Forms.Label LblRoleList;
        private System.Windows.Forms.ComboBox CmbRoleList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColMsgid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColRoleinfo;
        private System.Windows.Forms.DataGridViewButtonColumn ColDelete;
        private System.Windows.Forms.Button BtnDeleteRole;
        private System.Windows.Forms.Button BtnRoleSave;
        private System.Windows.Forms.CheckBox ChkSelectAll;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label LblEmbedColor;
        private System.Windows.Forms.TextBox TxtEmbedColor;
        private System.Windows.Forms.Button BtnSelectColor;
        private System.Windows.Forms.Panel PnlColorPreview;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.NumericUpDown NumDeleteDelayMs;
        private System.Windows.Forms.CheckBox ChkAutoDeleteEnabled;
    }
}
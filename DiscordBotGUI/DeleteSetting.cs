using Discord.WebSocket;
using DiscordBot.Core;
using Discord;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class DeleteSetting : Form
    {
        private DiscordSocketClient _client;
        //ILoggerを保持するためのプライベートフィールド
        private readonly ILogger _logger;
        private bool isActivated = false;
        //Role ID と Role Name を保持するためのヘルパークラス
        private class RoleDisplayItem
        {
            public ulong RoleId { get; set; }
            public string RoleName { get; set; }
            public override string ToString() => RoleName;
        }
        //サーバー ID とサーバー名を保持するためのヘルパークラスを追加
        private class GuildDisplayItem
        {
            public ulong GuildId { get; set; }
            public string GuildName { get; set; }
            public override string ToString() => GuildName;
        }
        public DeleteSetting(ILogger logger, DiscordSocketClient client)
        {
            InitializeComponent();
            //logger と client をフィールドに保存
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[DeleteSetting] Loggerインスタンスはnullにできません!!");
            _client = client;
            _logger.Log("-------------------- [DeleteSetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //コントロールの有効/無効状態を更新
        private void UpdateControlsState()
        {
            _logger?.Log("[INFO] UpdateControlsStateメソッドを開始!!", (int)LogType.Debug);
            // チェックボックスの状態を対応する NumericUpDown の Enabled プロパティに直接設定
            bool autoDeleteEnabled = ChkAutoDeleteEnabled.Checked;
            NumDeleteDelayMs.Enabled = autoDeleteEnabled;

            bool bulkDeleteEnabled = ChkDeleteItems.Checked;
            NumDeleteItems.Enabled = bulkDeleteEnabled;

            _logger?.Log($"[INFO] ChkAutoDeleteEnabled：[{autoDeleteEnabled}], NumDeleteDelayMs.Enabled設定完了!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] ChkDeleteItems：[{bulkDeleteEnabled}], NumDeleteItems.Enabled設定完了!!", (int)LogType.Debug);
            //レジストリから値を読み込み、UIに設定
            int delayMs = RegistryHelper.LoadDeleteDeleteDelayMs();
            int items = RegistryHelper.LoadBulkDeleteCount();
            _logger?.Log($"[INFO] プロンプトMSG自動削除時間：[{delayMs}]ms", (int)LogType.Debug);
            _logger?.Log($"[INFO] メッセージ削除件数：[{items}]個", (int)LogType.Debug);
            NumDeleteDelayMs.Value = delayMs;
            NumDeleteItems.Value = items;
            if (ChkAutoDeleteEnabled.Checked == false)
            {
                NumDeleteDelayMs.Value = 0;
                _logger?.Log("[INFO] プロンプトMSG自動削除が無効のため、NumDeleteDelayMs.Valueを0に設定!!", (int)LogType.Debug);
            }
            if (ChkDeleteItems.Checked == false)
            {
                NumDeleteItems.Value = 100;
                _logger?.Log("[INFO] メッセージ削除件数指定が無効のため、NumDeleteItems.Valueを100に設定!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] UpdateControlsStateメソッドを終了!!", (int)LogType.Debug);
        }
        //フォームロード時の処理
        private void DeleteSetting_Load(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] DeleteSetting_Loadイベントを開始!!", (int)LogType.Debug);
            _logger?.Log("[INFO] LoadGuildsToComboBoxメソッドを呼び出し!!", (int)LogType.Debug);
            LoadGuildsToComboBox();
            _logger?.Log("[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
            LoadServerRoles();
            //メッセージ削除件数指定フラグの読み込み
            _logger?.Log("[INFO] メッセージ削除件数指定フラグ設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.ChkDeleteItems != null)
            {
                bool usecustomcount = RegistryHelper.LoadDeleteUseCustomCount();
                ChkDeleteItems.Checked = usecustomcount;
                int bulkdeletecount = RegistryHelper.LoadBulkDeleteCount();
                NumDeleteItems.Value = bulkdeletecount;
                _logger?.Log($"[INFO] ChkDeleteItems (UseCustomCount) を [{usecustomcount}] に設定!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] NumDeleteItems (BulkDeleteCount) を [{bulkdeletecount}] に設定!!", (int)LogType.Debug);
            }
            //プロンプトMSG自動削除フラグ の読み込み
            _logger?.Log("[INFO] プロンプトMSG自動削除設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.ChkAutoDeleteEnabled != null)
            {
                bool shouldDelete = RegistryHelper.LoadDeleteShouldDelete();
                ChkAutoDeleteEnabled.Checked = shouldDelete;
                int delayMs = RegistryHelper.LoadDeleteDeleteDelayMs();
                NumDeleteDelayMs.Value = delayMs;
                _logger?.Log($"[INFO] ChkAutoDeleteEnabled (ShouldDelete) を [{shouldDelete}] に設定!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] NumDeleteDelayMs (DeleteDelayMs) を [{delayMs}]ms に設定!!", (int)LogType.Debug);
            }

            //対話中のプロンプトMSGタイムアウト時間の読み込み
            _logger?.Log("[INFO] 対話中プロンプトMSGタイムアウト設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.NumTimeoutMinutes != null)
            {
                int timeoutMinutesDelete = RegistryHelper.LoadDeleteTimeoutMinutes();
                NumTimeoutMinutes.Value = timeoutMinutesDelete;
                _logger?.Log($"[INFO] NumTimeoutMinutes (TimeoutMinutes) を [{timeoutMinutesDelete} 分]に設定!!", (int)LogType.Debug);
            }
            //コントロールの有効/無効
            _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] DeleteSetting_Loadイベントを終了!!", (int)LogType.Debug);
        }
        //Botが参加しているサーバーの一覧を ComboBox に読み込み
        private void LoadGuildsToComboBox()
        {
            _logger?.Log("[INFO] LoadGuildsToComboBoxメソッドを開始!!", (int)LogType.Debug);
            if (_client == null || _client.ConnectionState != ConnectionState.Connected)
            {
                _logger?.Log($"[ERROR] BOTが未接続のため、サーバ一覧の取得に失敗しました!! ConnectionState：[{_client?.ConnectionState.ToString() ?? "NULL"}]", (int)LogType.DebugError);
                MessageBox.Show("BOTが接続されていません!!\nサーバ覧を取得できません!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log("[INFO] LoadGuildsToComboBoxメソッドを終了!!", (int)LogType.Debug);
                return;
            }

            CmbGuilds.Items.Clear();
            _logger?.Log("[INFO] ComboBox (CmbGuilds) の既存アイテムをクリアしました!!", (int)LogType.Debug);
            //すべてのギルドを取得し、ComboBoxに追加
            var guilds = _client.Guilds.OrderBy(g => g.Name).ToList();
            _logger?.Log($"[INFO] BOTが参加しているギルドの総数：[{guilds.Count} 件]", (int)LogType.Debug);
            foreach (var guild in guilds)
            {
                CmbGuilds.Items.Add(new GuildDisplayItem { GuildId = guild.Id, GuildName = guild.Name });
                _logger?.Log($"[INFO] ギルドを追加：ID=[{guild.Id}], Name=[{guild.Name}]", (int)LogType.Debug);
            }
            _logger?.Log($"[INFO] ギルド一覧の読み込みが完了しました!! ComboBoxのアイテム数：[{CmbGuilds.Items.Count}]", (int)LogType.Debug);
            //最初の項目を選択
            /*if (CmbGuilds.Items.Count > 0)
            {
                CmbGuilds.SelectedIndex = 0;
            }*/
            _logger?.Log("[INFO] LoadGuildsToComboBoxメソッドを終了!!", (int)LogType.Debug);
        }
        //選択されているサーバーのロール一覧を CheckedListBox に読み込み (KickSettingからコピー)
        private void LoadServerRoles()
        {
            _logger?.Log("[INFO] LoadServerRolesメソッドを開始!!", (int)LogType.Debug);
            if (_client == null || _client.ConnectionState != ConnectionState.Connected)
            {
                _logger?.Log("[WARNING] BOTが未接続のため、ロールの読み込みをスキップします!!", (int)LogType.DebugError);
                _logger?.Log("[INFO] LoadServerRolesメソッドを終了!!", (int)LogType.Debug);
                return;
            }

            //ComboBoxから選択中のギルドIDを取得
            if (!(CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild))
            {
                _logger?.Log("[INFO] CmbGuildsでギルドが選択されていません!! ロールリストをクリア!!", (int)LogType.Debug);
                ChkListBoxRoles.Items.Clear();
                _logger?.Log("[INFO] LoadServerRolesメソッドを終了!!", (int)LogType.Debug);
                return;
            }
            _logger?.Log($"[INFO] 選択されたギルド：ID=[{selectedGuild.GuildId}], Name=[{selectedGuild.GuildName}]", (int)LogType.Debug);

            //選択されたサーバーIDを使って、特定のサーバーを取得
            SocketGuild guild = _client.GetGuild(selectedGuild.GuildId);
            if (guild == null)
            {
                _logger?.Log("[ERROR] ギルドが見つからなかった為、ロールリストをクリア!! (Botが退出した可能性)", (int)LogType.DebugError);
                MessageBox.Show("選択されたサーバーが見つかりませんでした!! Botがそのサーバーから退出した可能性があります!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ChkListBoxRoles.Items.Clear();
                _logger?.Log("[INFO] LoadServerRolesメソッドを終了!!", (int)LogType.Debug);
                return;
            }
            _logger?.Log($"[INFO] ギルドオブジェクトの取得に成功しました。ロール総数：[{guild.Roles.Count}]", (int)LogType.Debug);
            //@everyone 以外のロールを取得
            var roles = guild.Roles
                .Where(r => !r.IsEveryone)
                .OrderByDescending(r => r.Position)
                .ToList();
            _logger?.Log($"[INFO] @everyoneを除外し、Position順にソートされたロール数：[{roles.Count}]", (int)LogType.Debug);
            //ChkListBoxRoles にロールをバインド
            ChkListBoxRoles.Items.Clear();
            int addedCount = 0;
            foreach (var role in roles)
            {
                ChkListBoxRoles.Items.Add(new RoleDisplayItem { RoleId = role.Id, RoleName = role.Name });
                addedCount++;
            }
            _logger?.Log($"[SUCCESS] CheckedListBoxに [{addedCount} 個] のロールをバインドしました!!", (int)LogType.Debug);
            //ロールを読み込んだ後、現在の設定を適用
            _logger?.Log("[INFO] LoadCurrentSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            LoadCurrentSettings(selectedGuild.GuildId);
            _logger?.Log("[INFO] LoadServerRolesメソッドを終了!!", (int)LogType.Debug);
        }
        //LoadCurrentSettings にサーバーIDを引数として追加
        private void LoadCurrentSettings(ulong guildId)
        {
            _logger?.Log($"[INFO] LoadCurrentSettingsを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] 対象ギルドID：[{guildId}]", (int)LogType.Debug);
            //サーバーIDに対応する設定を読み込む (RegistryHelper.LoadDeleteAllowedRoleIds()は後で作成)
            List<ulong> allowedRoleIds = RegistryHelper.LoadDeleteAllowedRoleIds(guildId);
            _logger?.Log($"[INFO] レジストリから許可されたロールIDを [{allowedRoleIds.Count} 件] 読み込みました!!", (int)LogType.Debug);
            int checkedCount = 0;
            //保存済みの設定を CheckedListBox に反映
            for (int i = 0; i < ChkListBoxRoles.Items.Count; i++)
            {
                if (ChkListBoxRoles.Items[i] is RoleDisplayItem item)
                {
                    //以前チェックされていたロールか確認
                    bool isChecked = allowedRoleIds.Contains(item.RoleId);
                    ChkListBoxRoles.SetItemChecked(i, isChecked);
                    if (isChecked)
                    {
                        checkedCount++;
                        _logger?.Log($"[INFO] ロール：[{item.RoleName}], ID：[{item.RoleId}] をチェックONに設定!!", (int)LogType.Debug);
                    }
                    else
                    {
                        _logger?.Log($"[INFO] ロール：[{item.RoleName}] をチェックOFFに設定!!", (int)LogType.Debug);
                    }
                }
            }
            _logger?.Log($"[INFO] CheckedListBoxの設定を完了しました!! チェックされたロール数：[{checkedCount} 件]", (int)LogType.Debug);
            _logger?.Log("[INFO] LoadCurrentSettingsメソッドを終了!!", (int)LogType.Debug);
        }
        //保存ボタンクリック時
        private void BtnSave_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);
            //各設定値の読み込み
            bool shouldDelete = RegistryHelper.LoadDeleteShouldDelete();
            int delayMs = RegistryHelper.LoadDeleteDeleteDelayMs();
            int timeoutMinutes = RegistryHelper.LoadDeleteTimeoutMinutes();
            bool usecustomcount = RegistryHelper.LoadDeleteUseCustomCount();
            int bulkdeletecount = RegistryHelper.LoadBulkDeleteCount();
            _logger?.Log($"[INFO] 既存の設定値：ShouldDelete=[{shouldDelete}], DelayMs=[{delayMs}], TimeoutMin=[{timeoutMinutes}], UseCustomCount=[{usecustomcount}], BulkCount=[{bulkdeletecount}]", (int)LogType.Debug);
            if (!(CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild))
            {
                _logger?.Log("[WARNING] ギルドが選択されていません!! グローバル設定の変更のみを確認!!", (int)LogType.Debug);
                //ギルドが選択されていない場合、グローバル設定のみを比較し、変更があれば保存
                bool settingChanged =
                    shouldDelete != ChkAutoDeleteEnabled.Checked ||
                    delayMs != (int)NumDeleteDelayMs.Value ||
                    timeoutMinutes != (int)NumTimeoutMinutes.Value ||
                    usecustomcount != ChkDeleteItems.Checked ||
                    bulkdeletecount != (int)NumDeleteItems.Value;
                if (settingChanged)
                {
                    _logger?.Log("[INFO] ギルドは選択されていませんが、グローバル設定に変更がありました!! 保存処理を実行します!!", (int)LogType.Debug);
                    //レジストリにプロンプトMSG自動削除フラグ保存
                    RegistryHelper.SaveDeleteShouldDelete(ChkAutoDeleteEnabled.Checked);
                    //レジストリにメッセージ一括削除フラグ保存
                    RegistryHelper.SaveDeleteUseCustomCount(ChkDeleteItems.Checked);
                    //レジストリにプロンプトMSG自動削除時間を保存
                    RegistryHelper.SaveDeleteDeleteDelayMs((int)NumDeleteDelayMs.Value);
                    //レジストリに対話中のプロンプトMSGタイムアウト時間を保存
                    RegistryHelper.SaveDeleteTimeoutMinutes((int)NumTimeoutMinutes.Value);
                    //レジストリにメッセージ一括削除時間を保存
                    RegistryHelper.SaveBulkDeleteCount((int)NumDeleteItems.Value);
                    _logger?.Log("[SUCCESS] メッセージ削除件数指定フラグ/メッセージ削除件数/プロンプトMSG自動削除フラグ/プロンプトMSG自動削除時間/プロンプトMSGタイムアウト設定を保存しました!!", (int)LogType.Debug);
                    MessageBox.Show($"設定を保存しました!!", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _logger?.Log("[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }
                _logger?.Log("[INFO] ギルドが選択されておらず、グローバル設定にも変更がありませんでした!!", (int)LogType.Debug);
                MessageBox.Show("保存するサーバを選択してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log("[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }
            else
            {
                _logger?.Log($"[INFO] ギルド：[{selectedGuild.GuildName}], ID：[{selectedGuild.GuildId}] が選択されています!! ロール設定とグローバル設定を保存!!", (int)LogType.Debug);
                List<ulong> roleIdsToSave = new List<ulong>();
                foreach (RoleDisplayItem item in ChkListBoxRoles.CheckedItems)
                {
                    roleIdsToSave.Add(item.RoleId);
                }
                _logger?.Log($"[INFO] CheckedListBoxから [{roleIdsToSave.Count} 個] のロールIDを抽出しました!!", (int)LogType.Debug);
                // レジストリにサーバーIDと紐づけて保存
                RegistryHelper.SaveDeleteAllowedRoleIds(selectedGuild.GuildId, roleIdsToSave);
                _logger?.Log("[SUCCESS] サーバー固有の許可ロールIDを保存しました!!", (int)LogType.Debug);
                // レジストリにプロンプトMSG自動削除フラグ保存
                RegistryHelper.SaveDeleteShouldDelete(ChkAutoDeleteEnabled.Checked);
                //レジストリにメッセージ一括削除フラグ保存
                RegistryHelper.SaveDeleteUseCustomCount(ChkDeleteItems.Checked);
                // レジストリにプロンプトMSG自動削除時間を保存
                RegistryHelper.SaveDeleteDeleteDelayMs((int)NumDeleteDelayMs.Value);
                //レジストリに対話中のプロンプトMSGタイムアウト時間を保存
                RegistryHelper.SaveDeleteTimeoutMinutes((int)NumTimeoutMinutes.Value);
                //レジストリにメッセージ一括削除時間を保存
                RegistryHelper.SaveBulkDeleteCount((int)NumDeleteItems.Value);
                _logger?.Log("[SUCCESS] メッセージ削除件数指定フラグ/メッセージ削除件数/プロンプトMSG自動削除フラグ/プロンプトMSG自動削除時間/プロンプトMSGタイムアウト設定を保存しました!!", (int)LogType.Debug);
                MessageBox.Show($"サーバ：[{selectedGuild.GuildName}] のロール設定＆その他設定を保存しました!!", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            _logger?.Log("[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //コンボボックス内のサーバ選択時の処理
        private void CmbGuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] CmbGuilds_SelectedIndexChangedイベントを開始!!", (int)LogType.Debug);
            //選択されたギルドアイテムを取得(ログ)
            if (CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild)
            {
                _logger?.Log($"[INFO] ギルド選択変更を検出!! 選択されたギルド：[{selectedGuild.GuildName}], ID：[{selectedGuild.GuildId}]", (int)LogType.Debug);
            }
            else
            {
                _logger?.Log("[WARNING] 選択されたギルドアイテムが null または不正な型です!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
            LoadServerRoles();
            _logger?.Log("[INFO] CmbGuilds_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
        }
        //プロンプトMSGの自動削除チェックボックス変更時
        private void ChkAutoDeleteEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを終了!!", (int)LogType.Debug);
        }
        //メッセージ削除件数指定チェックボックス変更時
        private void ChkDeleteItems_CheckedChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] ChkDeleteItems_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] ChkDeleteItems_CheckedChangedイベントを終了!!", (int)LogType.Debug);
        }
        //フォーム設定の保存
        private void SaveFormSettings()
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
            if (Properties.Settings.Default.FormSetting == true)
            {
                _logger.Log($"[INFO] フォームの終了位置記憶処理開始!!", (int)LogType.Debug);
                //フォームの幅と高さを取得
                int width = this.Width;
                int height = this.Height;
                //フォームの位置を取得
                Point formPosition = this.Location;
                Properties.Settings.Default.DeleteSetting_FormX = width;
                Properties.Settings.Default.DeleteSetting_FormY = height;
                Properties.Settings.Default.DeleteSetting_PositionX = formPosition.X;
                Properties.Settings.Default.DeleteSetting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート処理
        private void DeleteSetting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] DeleteSetting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.DeleteSetting_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.DeleteSetting_FormX;
                int formY = Properties.Settings.Default.DeleteSetting_FormY;
                int positionX = Properties.Settings.Default.DeleteSetting_PositionX;
                int positionY = Properties.Settings.Default.DeleteSetting_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.DeleteSetting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.DeleteSetting_Init = false;
                        _logger.Log($"[INFO] フォームの初期化フラグを[{Properties.Settings.Default.DeleteSetting_Init}]に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.Save();
                        isActivated = true;
                    }
                    else
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの位置を変更
                        this.Location = new Point(positionX, positionY);
                        _logger.Log($"[INFO] フォーム座標：X軸[{positionX}],Y軸[{positionY}]", (int)LogType.Debug);
                        isActivated = true;
                    }
                }
                else
                {
                    //フォームの初期サイズを設定
                    this.Size = new System.Drawing.Size(formX, formY);
                    _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                    //フォームの起動位置を画面の中央に設定
                    this.StartPosition = FormStartPosition.CenterScreen;
                    _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                    isActivated = true;
                }
                _logger.Log($"[INFO] フォームアクティベート処理完了!!", (int)LogType.Debug);
                //メソッド終了ログ
                _logger.Log($"[INFO] DeleteSetting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクロージング処理
        private void DeleteSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] DeleteSetting_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] DeleteSetting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [DeleteSetting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

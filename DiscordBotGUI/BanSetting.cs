using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class BanSetting : Form
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
        //BAN済みユーザーと情報を保持するためのヘルパークラス
        private class BannedUserDisplayItem
        {
            public ulong UserId { get; set; }
            public string Username { get; set; }
            public string BanReason { get; set; }
            //表示フォーマットを定義
            public override string ToString() => $"{Username} ({UserId}) - 理由：{BanReason}";
        }
        public BanSetting(ILogger logger, DiscordSocketClient client)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[BanSetting] Loggerインスタンスはnullにできません!!");
            _client = client;
            _logger?.Log("-------------------- [BanSetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //コントロールの有効/無効状態を更新
        private void UpdateControlsState()
        {
            _logger?.Log("[INFO] UpdateControlsStateメソッドを開始!!", (int)LogType.Debug);
            //チェックボックスの状態を NumericUpDown の Enabled プロパティに直接設定
            bool isEnabled = ChkAutoDeleteEnabled.Checked;
            NumDeleteDelayMs.Enabled = isEnabled;
            _logger?.Log($"[INFO] ChkAutoDeleteEnabled：[{isEnabled}], NumDeleteDelayMs.Enabled設定完了!!", (int)LogType.Debug);
            int delayMs = RegistryHelper.LoadBanDeleteDelayMs();
            _logger?.Log($"[INFO] プロンプトMSG自動削除時間：[{delayMs}]ms", (int)LogType.Debug);
            NumDeleteDelayMs.Value = delayMs;
            if (ChkAutoDeleteEnabled.Checked == false)
            {
                NumDeleteDelayMs.Value = 0;
                _logger?.Log("[INFO] プロンプトMSG自動削除が無効のため、NumDeleteDelayMs.Valueを0に設定!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] UpdateControlsStateメソッドを終了!!", (int)LogType.Debug);
        }
        //Botが参加しているサーバーの一覧を ComboBox に読み込み
        private void LoadGuildsToComboBox()
        {
            _logger?.Log("[INFO] LoadGuildsToComboBoxメソッドを開始!!", (int)LogType.Debug);
            if (_client == null || _client.ConnectionState != ConnectionState.Connected)
            {
                _logger?.Log($"[ERROR] BOTが未接続のため、サーバ一覧の取得に失敗しました!! ConnectionState：[{_client?.ConnectionState.ToString() ?? "NULL"}]", (int)LogType.DebugError);
                MessageBox.Show("BOTが接続されていません!!\nサーバ一覧を取得できません!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        //選択されているサーバーのBAN済みユーザー一覧を ListBox に読み込み
        private async Task LoadBannedUsers()
        {
            _logger?.Log("[INFO] LoadBannedUsersを開始!!", (int)LogType.Debug);
            //ListBoxがデザイナーで追加されていることを前提とする
            if (LstBannedUsers == null)
            {
                _logger?.Log("[ERROR] LstBannedUsersコントロールが見つかりません!!", (int)LogType.DebugError);
                _logger?.Log("[INFO] LoadBannedUsersを終了!!", (int)LogType.Debug);
                return;
            }

            //ComboBoxから選択中のギルドIDを取得
            if (!(CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild))
            {
                LstBannedUsers.Items.Clear();
                _logger?.Log("[WARNING] CmbGuildsでギルドが選択されていません。", (int)LogType.Debug);
                _logger?.Log("[INFO] LoadBannedUsersを終了!!", (int)LogType.Debug);
                return;
            }
            _logger?.Log($"[INFO] 選択されたギルドID：[{selectedGuild.GuildId}]", (int)LogType.Debug);
            //選択されたサーバーIDを使って、特定のサーバーを取得
            SocketGuild guild = _client.GetGuild(selectedGuild.GuildId);
            if (guild == null)
            {
                LstBannedUsers.Items.Clear();
                _logger?.Log($"[ERROR] ギルドID：[{selectedGuild.GuildId}] に対応するサーバがクライアント側で見つかりませんでした!!", (int)LogType.DebugError);
                _logger?.Log("[INFO] LoadBannedUsersを終了!!", (int)LogType.Debug);
                return;
            }

            LstBannedUsers.Items.Clear();
            _logger?.Log("[INFO] BANリスト読み込み前のリストボックスをクリア!!", (int)LogType.Debug);
            try
            {
                _logger?.Log("[INFO] サーバAPIからBANリストの非同期取得を開始!! (GetBansAsync)", (int)LogType.Debug);
                //サーバーのBANリストを非同期で取得
                var bans = await guild.GetBansAsync().FlattenAsync();
                _logger?.Log($"[SUCCESS] BANリストを取得しました!! 合計BANユーザ数：[{bans.Count()}]", (int)LogType.Debug);
                //ListBox にユーザー情報を追加
                foreach (var ban in bans.OrderBy(b => b.User.Username))
                {
                    LstBannedUsers.Items.Add(new BannedUserDisplayItem
                    {
                        UserId = ban.User.Id,
                        Username = ban.User.Username,
                        //理由がない場合は「理由なし」と表示
                        BanReason = ban.Reason ?? "理由なし"
                    });
                }
                _logger?.Log($"[INFO] ListBoxに [{LstBannedUsers.Items.Count} 件] のBANユーザを追加!!", (int)LogType.Debug);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] BANユーザ覧の取得中に例外が発生しました!!\n例外：{ex.Message}", (int)LogType.Error);
                //権限エラーの場合などにメッセージボックスを表示
                if (ex.Message.Contains("Forbidden"))
                {
                    _logger?.Log("[ERROR] 権限不足エラー (Forbidden) を検出しました!!", (int)LogType.DebugError);
                    MessageBox.Show("BANユーザ一覧の取得に失敗しました!!\nBotに必要な権限 (Ban Membersなど) があるか確認してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            _logger?.Log("[INFO] LoadBannedUsersを終了!!", (int)LogType.Debug);
        }
        //選択されているサーバーのロール一覧を CheckedListBox に読み込み
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
                MessageBox.Show("選択されたサーバが見つかりませんでした!! Botがそのサーバから退出した可能性があります!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //サーバーIDに対応する設定を読み込む
            List<ulong> allowedRoleIds = RegistryHelper.LoadBanAllowedRoleIds(guildId);
            _logger?.Log($"[INFO] 許可されたロールIDを [{allowedRoleIds.Count} 件] 読み込みました!!", (int)LogType.Debug);
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
        //フォームロード時の処理
        private void BanSetting_Load(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BanSetting_Loadイベントを開始!!", (int)LogType.Debug);
            _logger?.Log("[INFO] LoadGuildsToComboBoxメソッドを呼び出し!!", (int)LogType.Debug);
            LoadGuildsToComboBox();
            _logger?.Log("[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
            LoadServerRoles();
            //プロンプトMSG自動削除フラグ の読み込み
            _logger?.Log("[INFO] プロンプトMSG自動削除設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.ChkAutoDeleteEnabled != null)
            {
                bool shouldDelete = RegistryHelper.LoadBanShouldDelete();
                ChkAutoDeleteEnabled.Checked = shouldDelete;
                int delayMs = RegistryHelper.LoadBanDeleteDelayMs();
                NumDeleteDelayMs.Value = delayMs;
                _logger?.Log($"[INFO] ChkAutoDeleteEnabled (ShouldDelete) を [{shouldDelete}] に設定!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] NumDeleteDelayMs (DeleteDelayMs) を [{delayMs}]ms に設定!!", (int)LogType.Debug);
            }
            //対話中のプロンプトMSGタイムアウト時間の読み込み
            _logger?.Log("[INFO] 対話中プロンプトMSGタイムアウト設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.NumTimeoutMinutes != null)
            {
                int timeoutMinutesBan = RegistryHelper.LoadBanTimeoutMinutes();
                NumTimeoutMinutes.Value = timeoutMinutesBan;
                _logger?.Log($"[INFO] NumTimeoutMinutes (TimeoutMinutes) を [{timeoutMinutesBan} 分]に設定!!", (int)LogType.Debug);
            }
            //コントロールの有効/無効
            _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] BanSetting_Loadイベントを終了!!", (int)LogType.Debug);
        }
        //保存ボタンクリック時
        private void BtnSave_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);
            bool shouldDelete = RegistryHelper.LoadBanShouldDelete();
            int delayMs = RegistryHelper.LoadBanDeleteDelayMs();
            int timeoutMinutesBan = RegistryHelper.LoadBanTimeoutMinutes();
            _logger?.Log($"[INFO] 既存の設定値：ShouldDelete=[{shouldDelete}], DelayMs=[{delayMs}], TimeoutMin=[{timeoutMinutesBan}]", (int)LogType.Debug);
            if (!(CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild))
            {
                _logger?.Log("[WARNING] ギルドが選択されていません!! グローバル設定の変更のみを確認!!", (int)LogType.Debug);
                //ギルドが選択されていない場合、グローバル設定のみを比較し、変更があれば保存
                bool settingChanged =
                    shouldDelete != ChkAutoDeleteEnabled.Checked ||
                    delayMs != (int)NumDeleteDelayMs.Value ||
                    timeoutMinutesBan != (int)NumTimeoutMinutes.Value;
                if (settingChanged)
                {
                    _logger?.Log("[INFO] ギルドは選択されていませんが、グローバル設定に変更がありました!! 保存処理を実行します!!", (int)LogType.Debug);
                    //レジストリにプロンプトMSG自動削除フラグ保存
                    RegistryHelper.SaveBanShouldDelete(ChkAutoDeleteEnabled.Checked);
                    //レジストリにプロンプトMSG自動削除時間を保存
                    RegistryHelper.SaveBanDeleteDelayMs((int)NumDeleteDelayMs.Value);
                    //レジストリに対話中のプロンプトMSGタイムアウト時間を保存
                    RegistryHelper.SaveBanTimeoutMinutes((int)NumTimeoutMinutes.Value);
                    _logger?.Log("[SUCCESS] プロンプトMSG自動削除フラグ/プロンプトMSG自動削除時間/プロンプトMSGタイムアウト設定を保存しました!!", (int)LogType.Debug);
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
                RegistryHelper.SaveBanAllowedRoleIds(selectedGuild.GuildId, roleIdsToSave);
                _logger?.Log("[SUCCESS] サーバー固有の許可ロールIDを保存しました!!", (int)LogType.Debug);
                //レジストリにプロンプトMSG自動削除フラグ保存
                RegistryHelper.SaveBanShouldDelete(ChkAutoDeleteEnabled.Checked);
                //レジストリにプロンプトMSG自動削除時間を保存
                RegistryHelper.SaveBanDeleteDelayMs((int)NumDeleteDelayMs.Value);
                //レジストリに対話中のプロンプトMSGタイムアウト時間を保存
                RegistryHelper.SaveBanTimeoutMinutes((int)NumTimeoutMinutes.Value);
                _logger?.Log("[SUCCESS] プロンプトMSG自動削除フラグ/プロンプトMSG自動削除時間/プロンプトMSGタイムアウト設定を保存しました!!", (int)LogType.Debug);
                MessageBox.Show($"サーバ：[{selectedGuild.GuildName}] のロール設定＆その他設定を保存しました!!", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            _logger?.Log("[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //サーバ選択時
        private async void CmbGuilds_SelectedIndexChanged(object sender, EventArgs e)
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
            //ロール一覧の更新
            _logger?.Log("[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
            LoadServerRoles();
            //サーバー切り替え時にBANユーザーリストも更新
            _logger?.Log("[INFO] LoadBannedUsersメソッドを非同期で呼び出し!!", (int)LogType.Debug);
            await LoadBannedUsers();
            _logger?.Log("[INFO] CmbGuilds_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
        }
        //BAN解除ボタンクリック時
        private async void BtnUnban_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnUnban_Clickイベントを開始!!", (int)LogType.Debug);
            //ギルド選択の検証
            if (!(CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild))
            {
                _logger?.Log("[WARNING] BAN解除対象のサーバが選択されていません!!", (int)LogType.Debug);
                MessageBox.Show("BANを解除するサーバを選択してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log("[INFO] BtnUnban_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }
            //ユーザー選択の検証
            if (!(LstBannedUsers.SelectedItem is BannedUserDisplayItem selectedUser))
            {
                _logger?.Log("[WARNING] BAN解除対象のユーザが選択されていません!!", (int)LogType.Debug);
                MessageBox.Show("BANを解除するユーザを選択してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log("[INFO] BtnUnban_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }
            _logger?.Log($"[INFO] 解除対象：サーバ=[{selectedGuild.GuildName}], サーバID：[{selectedGuild.GuildId}], ユーザ=[{selectedUser.Username}], ユーザID：[{selectedUser.UserId}]", (int)LogType.Debug);
            //確認メッセージ
            DialogResult result = MessageBox.Show(
                $"対象サーバ：[{selectedGuild.GuildName}]\nBAN解除ユーザ：[{selectedUser.Username}]\n\nBANを解除しますか?",
                "BAN解除の確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.No)
            {
                _logger?.Log("[INFO] ユーザがBAN解除処理をキャンセルしました!!", (int)LogType.Debug);
                _logger?.Log("[INFO] BtnUnban_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }
            //ギルドオブジェクトの取得
            SocketGuild guild = _client.GetGuild(selectedGuild.GuildId);
            if (guild == null)
            {
                _logger?.Log("[ERROR] 選択されたサーバが見つかりません!!", (int)LogType.DebugError);
                MessageBox.Show("サーバが見つかりませんでした!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log("[INFO] BtnUnban_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            try
            {
                _logger?.Log($"[INFO] サーバAPIに対し、ユーザ：[{selectedUser.UserId}] のBAN解除 (RemoveBanAsync) を実行します!!", (int)LogType.Debug);
                //Unban 処理を実行
                await guild.RemoveBanAsync(selectedUser.UserId);
                _logger.Log($"[SUCCESS] サーバ：[{selectedGuild.GuildName}] でユーザ：[{selectedUser.Username} ({selectedUser.UserId})] のBANを解除しました!!", (int)LogType.Debug);
                MessageBox.Show($"ユーザ：[{selectedUser.Username}] のBANを解除しました!!", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //成功後、リストを再読み込みして更新
                _logger?.Log("[INFO] LoadBannedUsersメソッドを非同期で呼び出し!!", (int)LogType.Debug);
                await LoadBannedUsers();
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] BAN解除中にエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"BAN解除に失敗しました!!\nBotに適切な権限があるか、またはユーザIDが正しいか確認してください!!\nエラー：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _logger?.Log("[INFO] BtnUnban_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //プロンプトMSGの自動削除有効フラグ変更時
        private void ChkAutoDeleteEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを終了!!", (int)LogType.Debug);
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
                Properties.Settings.Default.BanSetting_FormX = width;
                Properties.Settings.Default.BanSetting_FormY = height;
                Properties.Settings.Default.BanSetting_PositionX = formPosition.X;
                Properties.Settings.Default.BanSetting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void BanSetting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] BanSetting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.BanSetting_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.BanSetting_FormX;
                int formY = Properties.Settings.Default.BanSetting_FormY;
                int positionX = Properties.Settings.Default.BanSetting_PositionX;
                int positionY = Properties.Settings.Default.BanSetting_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.BanSetting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.BanSetting_Init = false;
                        _logger.Log($"[INFO] フォームの初期化フラグを[{Properties.Settings.Default.BanSetting_Init}]に設定!!", (int)LogType.Debug);
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
                _logger.Log($"[INFO] BanSetting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void BanSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] BanSetting_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] BanSetting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [BanSetting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

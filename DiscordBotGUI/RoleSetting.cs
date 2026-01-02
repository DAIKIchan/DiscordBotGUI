using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class RoleSetting : Form
    {
        private DiscordSocketClient _client;
        private ulong _targetGuildId;
        private List<RolePanelData> _settingsList;
        //全選択処理中かどうかを判定するフラグ
        private bool _isChangingCheckAll = false;
        //現在編集(選択中)のロールIDを保持
        private ulong _editingRoleId = 0;
        //編集開始時の名前(旧名用)
        private string _originalRoleName = "";
        private bool isActivated = false;
        private readonly ILogger _logger;
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
        public RoleSetting(ILogger logger, DiscordSocketClient client)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[RoleSetting] Loggerインスタンスはnullにできません!!");
            _client = client;
            _logger.Log("-------------------- [RoleSetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
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
                CmbRoleList.Items.Clear();
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
                CmbRoleList.Items.Clear();
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
            CmbRoleList.Items.Clear();
            int addedCount = 0;
            foreach (var role in roles)
            {
                //共通の表示用アイテムを作成
                var displayItem = new RoleDisplayItem { RoleId = role.Id, RoleName = role.Name };
                //1．管理用チェックリストに追加
                ChkListBoxRoles.Items.Add(displayItem);

                //2．ロール一覧コンボボックスに追加
                CmbRoleList.Items.Add(displayItem);
                //ChkListBoxRoles.Items.Add(new RoleDisplayItem { RoleId = role.Id, RoleName = role.Name });
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
            List<ulong> allowedRoleIds = RegistryHelper.LoadRoleAllowedRoleIds(guildId);
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
        private void RefreshGrid()
        {
            _logger?.Log($"[INFO] RefreshGridメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] 現在の設定リスト数：[{_settingsList?.Count ?? 0}]", (int)LogType.Debug);
            try
            {
                //描画更新の中断(パフォーマンス向上とチラつき防止)
                DgvPanels.SuspendLayout();

                //既存の行をクリア
                DgvPanels.Rows.Clear();
                _logger?.Log($"[INFO] DataGridViewの行をクリアしました!!", (int)LogType.Debug);

                if (_settingsList == null || _settingsList.Count == 0)
                {
                    _logger?.Log($"[INFO] 設定リストが空のため、グリッド更新をスキップします!!", (int)LogType.Debug);
                    return;
                }

                int addCount = 0;
                foreach (var panel in _settingsList)
                {
                    //ロール名のリストをカンマ区切りで作成
                    string roleInfoText = (panel.RoleMaps != null && panel.RoleMaps.Count > 0)
                        ? string.Join(", ", panel.RoleMaps.Select(m => $"{m.EmoteName} {m.RoleName}"))
                        : "なし";

                    //列の順番通りに値をセット
                    //0:有効無効(CheckBox), 1:メッセージID(String), 2:ロール情報(String), 3:削除(Button)
                    int rowIndex = DgvPanels.Rows.Add(
                        panel.IsEnabled,
                        panel.MessageId.ToString(),
                        roleInfoText,
                        "削除"
                    );

                    addCount++;
                    _logger?.Log($"[INFO] 行を追加：Index=[{rowIndex}], MessageID=[{panel.MessageId}]", (int)LogType.Debug);
                }

                _logger?.Log($"[INFO] グリッドの描画が完了しました!! 追加行数：[{addCount}]", (int)LogType.Debug);
            }
            catch (Exception ex)
            {
                //UI操作中の例外(スレッド競合など)をキャッチ
                _logger?.Log($"[ERROR] RefreshGrid処理中に例外が発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
            }
            finally
            {
                //描画更新の再開
                DgvPanels.ResumeLayout();
                _logger?.Log($"[INFO] RefreshGridメソッドを終了!!", (int)LogType.Debug);
            }
        }
        //フォームロード時
        private void RoleSetting_Load(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] RoleSetting_Loadイベントを開始!!", (int)LogType.Debug);

            try
            {
                //1．サーバー・ロール情報の読み込み
                _logger?.Log($"[INFO] LoadGuildsToComboBoxメソッドを呼び出し!!", (int)LogType.Debug);
                LoadGuildsToComboBox();
                _logger?.Log($"[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
                LoadServerRoles();

                //2．レジストリ(永続化フラグ)の読み込み
                bool isPermanent = RegistryHelper.LoadRoleIsPermanent();
                ChkGlobalPermanent.Checked = isPermanent;
                _logger?.Log($"[INFO] ロール自動付与永続化フラグ：[{isPermanent}]", (int)LogType.Debug);

                //3．JSONファイルからパネル設定の読み込み
                _logger?.Log($"[INFO] JSON設定ファイルの読み込みを開始：Path=[{RegistryHelper.RoleJsonPath}]", (int)LogType.Debug);
                if (File.Exists(RegistryHelper.RoleJsonPath))
                {
                    try
                    {
                        string json = File.ReadAllText(RegistryHelper.RoleJsonPath);
                        _settingsList = JsonConvert.DeserializeObject<List<RolePanelData>>(json) ?? new List<RolePanelData>();
                        _logger?.Log($"[INFO] JSON読み込み成功!! 取得パネル数：[{_settingsList.Count}]", (int)LogType.Debug);
                    }
                    catch (Exception jsonEx)
                    {
                        _logger?.Log($"[ERROR] JSONデシリアライズ中にエラーが発生しました!! リセットします!!\n例外：{jsonEx.Message}", (int)LogType.DebugError);
                        _settingsList = new List<RolePanelData>();
                    }
                }
                else
                {
                    _logger?.Log($"[INFO] JSONファイルが存在しません!! 新規リストを作成します!!", (int)LogType.Debug);
                    _settingsList = new List<RolePanelData>();
                }

                //4．色設定の読み込みと反映
                _logger?.Log($"[INFO] 埋め込み色の読み込みを開始!!", (int)LogType.Debug);
                uint colorValue = RegistryHelper.LoadRoleEmbedColor();

                //uint(RGB) から System.Drawing.Color を作成
                System.Drawing.Color loadedColor = System.Drawing.Color.FromArgb(255,
                    (byte)((colorValue >> 16) & 0xFF),
                    (byte)((colorValue >> 8) & 0xFF),
                    (byte)(colorValue & 0xFF));

                PnlColorPreview.BackColor = loadedColor;
                TxtEmbedColor.Text = GetColorDisplayString(loadedColor);
                TxtEmbedColor.ReadOnly = true;
                _logger?.Log($"[INFO] 色設定を反映：[#{colorValue:X6}] -> UI：[{TxtEmbedColor.Text}]", (int)LogType.Debug);

                //5．プロンプトMSG自動削除フラグの読み込み
                _logger?.Log($"[INFO] 自動削除設定の読み込みを開始!!", (int)LogType.Debug);
                if (this.ChkAutoDeleteEnabled != null)
                {
                    bool shouldDelete = RegistryHelper.LoadRoleShouldDelete();
                    ChkAutoDeleteEnabled.Checked = shouldDelete;

                    int delayMs = RegistryHelper.LoadRoleDeleteDelayMs();
                    //NumericUpDownの範囲チェック（念のため）
                    NumDeleteDelayMs.Value = Math.Max(NumDeleteDelayMs.Minimum, Math.Min(NumDeleteDelayMs.Maximum, delayMs));

                    _logger?.Log($"[INFO] 自動削除：[{shouldDelete}], 遅延時間：[{delayMs}]ms", (int)LogType.Debug);
                }

                //6．グリッドと権限リストの初期化
                _logger?.Log($"[INFO] RefreshGridメソッドを呼び出し!!", (int)LogType.Debug);
                RefreshGrid();
                _logger?.Log($"[INFO] InitPermissionListsメソッドを呼び出し!!", (int)LogType.Debug);
                InitPermissionLists();

                //7．UIコントロールの状態更新
                _logger?.Log($"[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateControlsState();
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] RoleSetting_Load中に致命的なエラーが発生しました!!\n例外：{ex.StackTrace}", (int)LogType.DebugError);
                MessageBox.Show("設定の読み込み中にエラーが発生しました!! ログを確認してください!!", "初期化エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _logger?.Log($"[INFO] RoleSetting_Loadイベントを終了!!", (int)LogType.Debug);
            }
        }
        //コントロールの有効/無効状態を更新
        private void UpdateControlsState()
        {
            _logger?.Log("[INFO] UpdateControlsStateメソッドを開始!!", (int)LogType.Debug);
            //チェックボックスの状態を NumericUpDown の Enabled プロパティに直接設定
            bool isEnabled = ChkAutoDeleteEnabled.Checked;
            NumDeleteDelayMs.Enabled = isEnabled;
            //チェックボックスの状態を NumericUpDown の Enabled プロパティに直接設定
            _logger?.Log($"[INFO] ChkAutoDeleteEnabled.Checked：[{isEnabled}] に基づき、NumDeleteDelayMs.Enabledを設定!!", (int)LogType.Debug);
            int delayMs = RegistryHelper.LoadRoleDeleteDelayMs();
            _logger?.Log($"[INFO] プロンプトMSG自動削除時間：[{delayMs}]ms", (int)LogType.Debug);
            NumDeleteDelayMs.Value = delayMs;
            if (ChkAutoDeleteEnabled.Checked == false)
            {
                NumDeleteDelayMs.Value = 0;
                _logger?.Log("[INFO] ChkAutoDeleteEnabledがfalseのため、NumDeleteDelayMs.Valueを0に設定しました!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] UpdateControlsStateメソッドを終了!!", (int)LogType.Debug);
        }

        private void InitPermissionLists()
        {
            _logger?.Log($"[INFO] InitPermissionListsメソッドを開始!!", (int)LogType.Debug);
            //1．一般権限
            string[] GeneralPerms = {
                "絵文字・スタンプの管理", "監査ログを表示", "管理者",
                "サーバーインサイトの表示", "サーバーの管理", "チャンネルの管理",
                "チャンネルを見る", "ウェブフックの管理", "ロールの管理"
            };
            _logger?.Log($"[INFO] 一般権限リストの項目数：[{GeneralPerms.Length}]", (int)LogType.Debug);
            //2．メンバー管理
            string[] MembershipPerms = {
                "招待の作成", "ニックネームの管理", "ニックネームの変更",
                "メンバーをBAN", "メンバーをキック", "メンバーをタイムアウト"
            };
            _logger?.Log($"[INFO] メンバー管理権限リストの項目数：[{MembershipPerms.Length}]", (int)LogType.Debug);
            //3．テキスト権限
            string[] TextPerms = {
                "アプリコマンドを使用", "エクスプレッションを作成", "外部のアプリを使用",
                "外部の絵文字を使用", "外部のスタンプを使用", "公開スレッドの作成",
                "全員宛てメンション", "スレッドでメッセージ送信", "スレッドの管理",
                "低速モードを回避", "投票の作成", "非公開スレッドの作成",
                "ファイルを添付", "ボイスメッセージを送信", "埋め込みリンク",
                "メッセージの管理", "メッセージを送信", "メッセージをピン止め",
                "メッセージ履歴の閲覧", "読み上げメッセージ送信", "リアクションの追加"
            };
            _logger?.Log($"[INFO] テキスト権限リストの項目数：[{TextPerms.Length}]", (int)LogType.Debug);
            //4．ボイス・イベント
            string[] VoicePerms = {
                "イベントの管理", "イベントを作成", "音声検出を使用",
                "外部のサウンドの使用", "サウンドボードを使用", "スピーカー参加リクエスト",
                "接続", "発言", "Webカメラ",
                "ボイスチャンネルステータスを使用", "優先スピーカー", "メンバーのスピーカーをミュート",
                "メンバーをミュート", "メンバーを移動", "ユーザーアクティビティ"
            };
            _logger?.Log($"[INFO] ボイス・イベント権限リストの項目数：[{VoicePerms.Length}]", (int)LogType.Debug);
            //名前からEnumに変換して一括追加する汎用メソッドを使用
            _logger?.Log($"[INFO] AddPermissionsToBoxメソッドを呼び出し!!", (int)LogType.Debug);
            AddPermissionsToBox(ClbGeneral, GeneralPerms);
            _logger?.Log($"[INFO] AddPermissionsToBoxメソッドを呼び出し!!", (int)LogType.Debug);
            AddPermissionsToBox(ClbMembership, MembershipPerms);
            _logger?.Log($"[INFO] AddPermissionsToBoxメソッドを呼び出し!!", (int)LogType.Debug);
            AddPermissionsToBox(ClbText, TextPerms);
            _logger?.Log($"[INFO] AddPermissionsToBoxメソッドを呼び出し!!", (int)LogType.Debug);
            AddPermissionsToBox(ClbVoice, VoicePerms);
            _logger?.Log($"[INFO] InitPermissionListsメソッドを終了!!", (int)LogType.Debug);
        }

        private void AddPermissionsToBox(CheckedListBox clb, string[] jpNames)
        {
            _logger?.Log($"[INFO] AddPermissionsToBoxメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] 対象BOX：[{clb.Name}], 処理予定数：[{jpNames.Length}]", (int)LogType.Debug);
            int successCount = 0;
            int failCount = 0;

            //描画のチラつきを抑えるための更新一時停止
            clb.BeginUpdate();
            try
            {
                clb.Items.Clear();

                foreach (var name in jpNames)
                {
                    //文字列からEnumを取得(リフレクションの簡易版)
                    var perm = PermissionHelper.GetPermission(name);

                    if (perm != null)
                    {
                        clb.Items.Add(name);
                        successCount++;
                    }
                    else
                    {
                        //名前が間違っている、またはHelperに定義がない場合
                        //重要度を少し上げ、どのリストで失敗したかを出力
                        _logger?.Log($"[ERROR] 権限マッピング失敗：BOX=[{clb.Name}], 権限名='{name}' が PermissionHelper で定義されていません!!", (int)LogType.DebugError);
                        failCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Log($"[ERROR] AddPermissionsToBox内で予期せぬエラーが発生!!\n例外：{ex.Message}", (int)LogType.DebugError);
            }
            finally
            {
                //描画の再開
                clb.EndUpdate();
                _logger?.Log($"[INFO] 対象BOX：[{clb.Name}], 成功：[{successCount}], 失敗：[{failCount}]", (int)LogType.Debug);
                _logger?.Log($"[INFO] AddPermissionsToBoxメソッドを終了!!", (int)LogType.Debug);
            }
        }
        //保存ボタン
        private void BtnSave_Click(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);

            try
            {
                //1．保存確認
                var result = MessageBox.Show("設定を保存しますか？\n\n※ ロール情報リストが再読み込みされる為、永続化フラグが[有効]ではない場合初期化される可能性があります!!", "保存確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    _logger?.Log($"[INFO] ユーザにより保存操作がキャンセルされました!!", (int)LogType.Debug);
                    return;
                }

                //2．ギルド選択の確認(サーバ固有設定用)
                var selectedGuild = CmbGuilds.SelectedItem as GuildDisplayItem;

                //3．サーバー固有の許可ロール保存
                if (selectedGuild != null)
                {
                    _logger?.Log($"[INFO] ギルド：[{selectedGuild.GuildName}], ID：[{selectedGuild.GuildId}] のロール設定を保存します!!", (int)LogType.Debug);

                    List<ulong> roleIdsToSave = new List<ulong>();
                    foreach (RoleDisplayItem item in ChkListBoxRoles.CheckedItems)
                    {
                        roleIdsToSave.Add(item.RoleId);
                    }

                    RegistryHelper.SaveRoleAllowedRoleIds(selectedGuild.GuildId, roleIdsToSave);
                    _logger?.Log($"[SUCCESS] サーバ固有の許可ロールIDを [{roleIdsToSave.Count} 件] 保存しました!!", (int)LogType.Debug);
                }
                else
                {
                    _logger?.Log($"[WARNING] ギルドが選択されていないため、サーバ固有のロール設定保存をスキップします!!", (int)LogType.Debug);
                }

                //4．レジストリへの保存(全体の設定：永続化フラグ)
                bool isPermanent = ChkGlobalPermanent.Checked;
                RegistryHelper.SaveRoleIsPermanent(isPermanent);
                _logger?.Log($"[INFO] ロール自動付与永続化フラグ：[{isPermanent}]", (int)LogType.Debug);

                //5．DataGridView の状態をリストに反映
                if (_settingsList != null)
                {
                    _logger?.Log($"[INFO] DataGridViewから設定リストへの同期を開始!! 行数：[{DgvPanels.Rows.Count}]", (int)LogType.Debug);
                    for (int i = 0; i < DgvPanels.Rows.Count; i++)
                    {
                        if (i < _settingsList.Count)
                        {
                            _settingsList[i].IsEnabled = (bool)DgvPanels.Rows[i].Cells["ColEnabled"].Value;
                        }
                    }

                    //JSONファイルとして保存
                    string folder = RegistryHelper.BaseDirectory;
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    string json = JsonConvert.SerializeObject(_settingsList, Formatting.Indented);
                    File.WriteAllText(RegistryHelper.RoleJsonPath, json);
                    _logger?.Log($"[SUCCESS] パネル設定リストをJSONに保存しました!! 件数：[{_settingsList.Count}]", (int)LogType.Debug);

                    RegistryHelper.NotifySettingsChanged();
                }

                //6．プロンプトMSG自動削除設定
                if (this.ChkAutoDeleteEnabled != null)
                {
                    bool shouldDelete = ChkAutoDeleteEnabled.Checked;
                    int delayMs = (int)NumDeleteDelayMs.Value;
                    RegistryHelper.SaveRoleShouldDelete(shouldDelete);
                    RegistryHelper.SaveRoleDeleteDelayMs(delayMs);
                    _logger?.Log($"[SUCCESS] 自動削除設定を保存：ShouldDelete=[{shouldDelete}], Delay=[{delayMs} ms]", (int)LogType.Debug);
                }

                //7．枠色保存
                SaveEmbedColor();

                _logger?.Log($"[SUCCESS] 全ての保存処理が正常に完了しました!!", (int)LogType.Debug);

                string msg = selectedGuild != null
                    ? $"サーバ：[{selectedGuild.GuildName}] の設定を保存しました!!"
                    : "共通設定を保存しました!!";
                MessageBox.Show(msg, "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] 保存処理中に致命的なエラーが発生!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"保存中にエラーが発生しました!!\n例外：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _logger?.Log($"[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
            }
        }

        private void CmbGuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] CmbGuilds_SelectedIndexChangedイベントを開始!!", (int)LogType.Debug);
            //選択されたギルドアイテムを取得(ログ)
            if (CmbGuilds.SelectedItem is GuildDisplayItem selectedGuild)
            {
                _targetGuildId = selectedGuild.GuildId;
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
        //ロール作成ボタンクリック時
        private async void BtnCreateRole_Click(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] BtnCreateRole_Clickイベントを開始!!", (int)LogType.Debug);
            //サーバーが選択されているかチェック
            if (_targetGuildId == 0)
            {
                _logger?.Log($"[WARNING] ロール作成中止：対象サーバが選択されていません!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnCreateRole_Clickイベントを終了!!", (int)LogType.Debug);
                MessageBox.Show("対象のサーバを [ロール許可設定] タブから選択してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string roleName = TxtRoleName.Text.Trim();
            if (string.IsNullOrEmpty(roleName))
            {
                _logger?.Log($"[WARNING] ロール作成中止：ロール名が未入力です!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnCreateRole_Clickイベントを終了!!", (int)LogType.Debug);
                MessageBox.Show("ロール名を入力してください!!", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //権限ビットフラグの集約
            _logger?.Log($"[INFO] 選択された権限の集約を開始!! 対象ロール名：[{roleName}]", (int)LogType.Debug);
            ulong permissionsRaw = 0;
            CheckedListBox[] allBoxes = { ClbGeneral, ClbMembership, ClbText, ClbVoice };
            int checkedCount = 0;
            foreach (var box in allBoxes)
            {
                foreach (var item in box.CheckedItems)
                {
                    //Core のヘルパーを使って日本語名から Enum に戻す
                    string permName = item.ToString();
                    var perm = PermissionHelper.GetPermission(permName);
                    if (perm.HasValue)
                    {
                        permissionsRaw |= (ulong)perm.Value;
                        checkedCount++;
                    }
                    else
                    {
                        _logger?.Log($"[ERROR] 権限取得失敗：'{permName}' に対応するEnum値が見つかりません!!", (int)LogType.DebugError);
                    }
                }
            }
            _logger?.Log($"[INFO] 権限集約完了!! チェック数：[{checkedCount}], 最終ビットフラグ：[{permissionsRaw}]", (int)LogType.Debug);
            var permissions = new GuildPermissions(permissionsRaw);

            try
            {
                var guild = _client.GetGuild(_targetGuildId);
                if (guild == null)
                {
                    _logger?.Log($"[FATAL] サーバ取得エラー：ID=[{_targetGuildId}] のサーバが見つかりません!!", (int)LogType.DebugError);
                    throw new Exception("サーバが見つかりません!!");
                }
                _logger?.Log($"[INFO] Discord APIへロール作成をリクエストします!! サーバ：[{guild.Name}]", (int)LogType.Debug);
                //ロール作成 (色はデフォルト、ホイスト・メンション可否は任意の設定を反映)
                var newRole = await guild.CreateRoleAsync(
                    name: roleName,
                    permissions: permissions,
                    //色選択を追加
                    color: null,
                    //メンバーリストで分けて表示
                    isHoisted: true,
                    isMentionable: false
                );
                _logger?.Log($"[SUCCESS] ロール作成成功!! 名前：[{newRole.Name}], ID：[{newRole.Id}]", (int)LogType.UserMessage);
                MessageBox.Show($"ロール[{newRole.Name}]の作成に成功しました!!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //入力のリセット
                _logger?.Log($"[INFO] 入力フォームをリセットし、ロール一覧を再読み込みします!!", (int)LogType.Debug);
                TxtRoleName.Clear();
                foreach (var box in allBoxes)
                {
                    for (int i = 0; i < box.Items.Count; i++) box.SetItemChecked(i, false);
                }
                _logger?.Log("[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
                LoadServerRoles();
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] ロール作成中に例外が発生しました!!\n例外：{ex.Message}\nスタックトレース：{ex.StackTrace}", (int)LogType.DebugError);
                MessageBox.Show($"作成に失敗しました!!\n例外：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _logger?.Log($"[INFO] BtnCreateRole_Clickイベントを終了!!", (int)LogType.Debug);
            }
        }
        //データグリッドビューの「削除」ボタンクリック時
        private void DgvPanels_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //1．ヘッダーがクリックされた場合は無視
            if (e.RowIndex < 0) return;
            _logger?.Log($"[INFO] DgvPanels_CellContentClickイベントを開始!!", (int)LogType.Debug);
            //2．クリックされた列が「削除ボタン列」であるか確認
            //"DeleteBtn" は、列作成時に設定した Name(名前)プロパティに合わせる
            if (DgvPanels.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                _logger?.Log($"[INFO] 削除ボタンがクリックされました!! Index：[{e.RowIndex}]", (int)LogType.Debug);
                //削除対象の情報をログ用に取得(MessageIdなどを特定)
                var targetData = (e.RowIndex < _settingsList?.Count) ? _settingsList[e.RowIndex] : null;
                string targetInfo = targetData != null ? $"MessageID：[{targetData.MessageId}]" : "不明なデータ";
                //削除確認のメッセージを表示
                var result = MessageBox.Show($"このロールパネル情報を削除しますか？\n({targetInfo})", "削除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _logger?.Log($"[INFO] 削除処理を実行します!! 対象：[{targetInfo}]", (int)LogType.Debug);
                    //リストから該当するインデックスのデータを削除
                    _settingsList.RemoveAt(e.RowIndex);
                    try
                    {
                        //リストから削除
                        if (_settingsList != null && e.RowIndex < _settingsList.Count)
                        {
                            _settingsList.RemoveAt(e.RowIndex);
                            _logger?.Log($"[INFO] メモリ上のリストから要素を削除しました!!", (int)LogType.Debug);
                        }
                        //JSONファイルとして即時保存
                        string json = JsonConvert.SerializeObject(_settingsList, Formatting.Indented);
                        File.WriteAllText(RegistryHelper.RoleJsonPath, json);
                        _logger?.Log($"[SUCCESS] JSONファイルを更新しました!! Path：[{RegistryHelper.RoleJsonPath}]", (int)LogType.Debug);
                        //プラグインに通知
                        RegistryHelper.NotifySettingsChanged();
                        _logger?.Log($"[INFO] 設定変更を本体に通知しました!!", (int)LogType.Debug);
                        MessageBox.Show($"削除に成功しました!!", "削除完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        _logger?.Log($"[ERROR] 削除処理またはファイル保存中に例外が発生しました!!\n例外：{ex.Message}", (int)LogType.Error);
                        MessageBox.Show($"保存に失敗しました!!\n例外：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    //グリッドを再描画
                    _logger?.Log($"[INFO] RefreshGridメソッドを呼び出し!!", (int)LogType.Debug);
                    RefreshGrid();
                }
                else
                {
                    _logger?.Log($"[INFO] ユーザーにより削除がキャンセルされました!!", (int)LogType.Debug);
                }
                _logger?.Log($"[INFO] DgvPanels_CellContentClickイベントを終了!!", (int)LogType.Debug);
            }
        }
        //ロール削除ボタンクリック時
        private async void BtnDeleteRole_Click(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] BtnDeleteRole_Clickイベントを開始!!", (int)LogType.Debug);
            //1．選択チェック
            if (!(CmbRoleList.SelectedItem is RoleDisplayItem selectedRole))
            {
                _logger?.Log($"[WARNING] ロール削除中止：削除対象が選択されていません!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnDeleteRole_Clickイベントを終了!!", (int)LogType.Debug);
                MessageBox.Show("削除するロールを選択してください。", "通知", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //2．削除確認
            var confirm = MessageBox.Show(
                $"ロール「{selectedRole.RoleName}」を削除してもよろしいですか？",
                "削除確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                _logger?.Log($"[INFO] ユーザによりロール削除がキャンセルされました!! ロール：[{selectedRole.RoleName}]", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnDeleteRole_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            try
            {
                _logger?.Log($"[INFO] Discord APIへロール削除をリクエストします!! 対象：[{selectedRole.RoleName}], ID：[{selectedRole.RoleId}]", (int)LogType.Debug);
                //3．Discordからロールを取得して削除
                var guild = _client.GetGuild(_targetGuildId);
                if (guild == null)
                {
                    _logger?.Log($"[ERROR] サーバが見つかりません!! ID：[{_targetGuildId}]", (int)LogType.Error);
                    throw new Exception("サーバ情報を取得できませんでした!!");
                }
                var role = guild?.GetRole(selectedRole.RoleId);

                if (role == null)
                {
                    _logger?.Log($"[WARNING] 削除対象ロールが既にサーバ上に存在しません!! ID：[{selectedRole.RoleId}]", (int)LogType.Debug);
                    MessageBox.Show("指定されたロールがサーバに見つかりませんでした!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //削除実行
                    await role.DeleteAsync();

                    _logger?.Log($"[SUCCESS] ロールを削除しました!! ロール名：[{selectedRole.RoleName}], ID：[{selectedRole.RoleId}]", (int)LogType.Debug);
                    MessageBox.Show($"ロール [{selectedRole.RoleName}] を削除しました!!", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Discord.Net.HttpException ex) when (ex.HttpCode == System.Net.HttpStatusCode.Forbidden)
            {
                //Botの役職順位がターゲットより低い、または権限がない場合
                _logger?.Log($"[ERROR] ロール削除権限がありません!! (403 Forbidden) ボットの順位が対象ロールより下である可能性があります!!\n例外：{ex.Message}", (int)LogType.Error);
                MessageBox.Show("ロールを削除する権限がありません。\nBotの役職順位が対象ロールより上にあるか確認してください。", "権限エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] ロール削除中に予期せぬエラーが発生しました!!\n例外：{ex.Message}\nスタックトレース：{ex.StackTrace}", (int)LogType.DebugError);
                MessageBox.Show($"エラーが発生しました!!\n例外：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //4．UIを最新状態に更新(再読み込み)
                _logger?.Log($"[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
                LoadServerRoles();
                _logger?.Log($"[INFO] BtnDeleteRole_Clickイベントを終了!!", (int)LogType.Debug);
            }
        }
        //コンボボックスでロール選択時
        private void CmbRoleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] CmbRoleList_SelectedIndexChangedイベントを開始!!", (int)LogType.Debug);
            //1．選択されたアイテムの型チェック
            if (!(CmbRoleList.SelectedItem is RoleDisplayItem selectedItem))
            {
                _editingRoleId = 0;
                _originalRoleName = "";
                _logger?.Log($"[WARNING] 選択されたアイテムがRoleDisplayItemではありません!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] CmbRoleList_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
                return;
            }
            _editingRoleId = selectedItem.RoleId;
            _originalRoleName = selectedItem.RoleName;
            //2．サーバ(ギルド)とロールを取得
            var guild = _client.GetGuild(_targetGuildId);
            if (guild == null)
            {
                _logger?.Log($"[ERROR] ギルドID：[{_targetGuildId}] の取得に失敗しました!!", (int)LogType.DebugError);
                _logger?.Log($"[INFO] CmbRoleList_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
                return;
            }
            var role = guild?.GetRole(selectedItem.RoleId);

            if (role == null)
            {
                _logger?.Log($"[WARNING] ロールID：[{selectedItem.RoleId}] がサーバ上に見つかりません!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] CmbRoleList_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
                return;
            }
            _logger?.Log($"[INFO] ロール [{role.Name}] の権限解析を開始!! フラグ値：[{role.Permissions.RawValue}]", (int)LogType.Debug);
            //3．UIの更新を一時停止(チラつき防止)
            ClbGeneral.BeginUpdate();
            ClbMembership.BeginUpdate();
            ClbText.BeginUpdate();
            ClbVoice.BeginUpdate();

            try
            {
                //4．全ての CheckedListBox をループで処理
                CheckedListBox[] allBoxes = { ClbGeneral, ClbMembership, ClbText, ClbVoice };
                int matchCount = 0;
                int failCount = 0;
                foreach (var box in allBoxes)
                {
                    for (int i = 0; i < box.Items.Count; i++)
                    {
                        //リストにある日本語名を取得
                        string jpName = box.Items[i].ToString();

                        //PermissionHelper を使って Enum に変換
                        var perm = PermissionHelper.GetPermission(jpName);

                        if (perm.HasValue)
                        {
                            //ロールがその権限を持っているかチェック
                            //role.Permissions.Has() を使用
                            bool hasPermission = role.Permissions.Has(perm.Value);
                            box.SetItemChecked(i, hasPermission);
                            if (hasPermission) matchCount++;
                        }
                        else
                        {
                            //辞書にない場合は念のためチェックを外す
                            _logger?.Log($"[WARNING] 権限名 '{jpName}' をEnumに変換できませんでした!!", (int)LogType.DebugError);
                            box.SetItemChecked(i, false);
                            failCount++;
                        }
                    }
                }
                _logger?.Log($"[INFO] UI反映完了!! 保持権限数：[{matchCount}], 判定不能：[{failCount}]", (int)LogType.Debug);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[ERROR] 権限反映中に例外が発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
            }
            finally
            {
                //5．UIの更新を再開
                ClbGeneral.EndUpdate();
                ClbMembership.EndUpdate();
                ClbText.EndUpdate();
                ClbVoice.EndUpdate();
                _logger?.Log($"[SUCCESS] ロール [{role.Name}] の権限をUIに同期しました!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] CmbRoleList_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //ロール保存ボタンクリック時
        private async void BtnRoleSave_Click(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] BtnRoleSave_Clickイベントを開始!!", (int)LogType.Debug);
            //1．ロールが選択されているか確認
            if (_editingRoleId == 0)
            {
                _logger?.Log($"[WARNING] 更新中止：編集対象のロールIDが0です!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnRoleSave_Clickイベントを終了!!", (int)LogType.Debug);
                MessageBox.Show("更新対象のロールを一覧から選択してください!!", "通知", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //2．コンボボックスに入力されている「新しい名前」を取得
            string newRoleName = CmbRoleList.Text.Trim();
            if (string.IsNullOrEmpty(newRoleName))
            {
                _logger?.Log($"[WARNING] 更新中止：新しいロール名が入力されていません!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnRoleSave_Clickイベントを終了!!", (int)LogType.Debug);
                MessageBox.Show("ロール名が空です!! 名前を入力してください!!", "通知", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //3．更新確認
            var confirm = MessageBox.Show(
                $"ロール [{_originalRoleName}] の内容を更新しますか？\n旧ロール名：[{_originalRoleName}]\n新ロール名：[{newRoleName}]",
                "保存確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                _logger?.Log($"[INFO] ユーザにより保存操作がキャンセルされました!! 対象：[{_originalRoleName}]", (int)LogType.Debug);
                _logger?.Log($"[INFO] BtnRoleSave_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            try
            {
                var guild = _client.GetGuild(_targetGuildId);
                if (guild == null)
                {
                    _logger?.Log($"[ERROR] ギルドID：[{_targetGuildId}] が見つかりません!!", (int)LogType.DebugError);
                    throw new Exception("サーバ情報が取得できませんでした。");
                }
                var role = guild?.GetRole(_editingRoleId);

                if (role == null)
                {
                    _logger?.Log($"[ERROR] 更新対象のロールID：[{_editingRoleId}] がサーバ上に存在しません!!", (int)LogType.DebugError);
                    MessageBox.Show("ロールが見つかりませんでした!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //4．権限ビットの計算
                _logger?.Log($"[INFO] 権限ビットの再計算を開始します!!", (int)LogType.Debug);
                ulong rawPermissions = 0;
                int checkedCount = 0;
                CheckedListBox[] allBoxes = { ClbGeneral, ClbMembership, ClbText, ClbVoice };
                foreach (var box in allBoxes)
                {
                    foreach (var item in box.CheckedItems)
                    {
                        var perm = PermissionHelper.GetPermission(item.ToString());
                        if (perm.HasValue)
                        {
                            rawPermissions |= (ulong)perm.Value;
                            checkedCount++;
                        }
                    }
                }
                _logger?.Log($"[INFO] 計算完了!! 新名称：[{newRoleName}], 権限ビット：[{rawPermissions}], 項目数：[{checkedCount}]", (int)LogType.Debug);
                //5．Discordのロールを更新(名前と権限を同時に送信)
                _logger?.Log($"[INFO] Discord APIへModifyAsyncを送信します!! ID：[{role.Id}]", (int)LogType.Debug);
                await role.ModifyAsync(x =>
                {
                    //コンボボックスのテキストを名前に反映
                    x.Name = newRoleName;
                    x.Permissions = new GuildPermissions(rawPermissions);
                });
                _logger?.Log($"[SUCCESS] ロールを更新しました!! 旧：[{_originalRoleName}] -> 新：[{newRoleName}], ID：[{role.Id}]", (int)LogType.UserMessage);
                MessageBox.Show($"ロール [{newRoleName}] の情報を保存しました!!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //6．UIをリロードしてリストを最新にする
                _logger?.Log($"[INFO] 編集ステートをリセットし、UIをリロードします!!", (int)LogType.Debug);
                //リセット
                _editingRoleId = 0;
                _originalRoleName = "";
                _logger?.Log($"[INFO] LoadServerRolesメソッドを呼び出し!!", (int)LogType.Debug);
                LoadServerRoles();
            }
            catch (Discord.Net.HttpException ex) when (ex.HttpCode == System.Net.HttpStatusCode.Forbidden)
            {
                _logger?.Log($"[ERROR] 権限エラー(403 Forbidden)：ボットの権限が不足しているか、役職順位が対象ロールより下です!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show("権限不足のため保存できませんでした!!\nBotの役職をこのロールより上に移動させてください!!", "権限エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] ロール保存中に致命的な例外が発生しました!!\n例外：{ex.Message}\nスタックトレース：{ex.StackTrace}", (int)LogType.DebugError);
                MessageBox.Show($"保存中にエラーが発生しました!!\n例外：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _logger?.Log($"[INFO] BtnRoleSave_Clickイベントを終了!!", (int)LogType.Debug);
            }
        }
        //権限全選択チェックボックス
        private void ChkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            //プログラムからの変更の場合は何もしない
            if (_isChangingCheckAll) return;
            _logger?.Log($"[INFO] ChkSelectAll_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            bool isChecked = ChkSelectAll.Checked;
            _logger?.Log($"[INFO] 状態：[{(isChecked ? "全選択" : "全解除")}]", (int)LogType.Debug);
            _isChangingCheckAll = true;

            try
            {
                CheckedListBox[] allBoxes = { ClbGeneral, ClbMembership, ClbText, ClbVoice };
                int totalUpdatedItems = 0;

                foreach (var clb in allBoxes)
                {
                    //描画のチラつきを抑える
                    clb.BeginUpdate();
                    try
                    {
                        int itemCount = clb.Items.Count;
                        for (int i = 0; i < itemCount; i++)
                        {
                            clb.SetItemChecked(i, isChecked);
                        }
                        totalUpdatedItems += itemCount;
                    }
                    finally
                    {
                        clb.EndUpdate();
                    }
                }

                _logger?.Log($"[INFO] 一括操作が完了しました!! 対象ボックス数：[{allBoxes.Length}], 総項目数：[{totalUpdatedItems}]", (int)LogType.Debug);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[ERROR] 全選択/解除処理中に例外が発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
            }
            finally
            {
                //最後に必ずフラグを戻す
                _isChangingCheckAll = false;
                _logger?.Log($"[INFO] 操作フラグ _isChangingCheckAll を解除しました!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] ChkSelectAll_CheckedChangedイベントを終了!!", (int)LogType.Debug);
            }
        }
        private void SyncSelectAllCheckBox()
        {
            //全選択ボタン自体の処理中ならスキップ
            if (_isChangingCheckAll) return;

            CheckedListBox[] allBoxes = { ClbGeneral, ClbMembership, ClbText, ClbVoice };
            bool allChecked = true;

            foreach (var clb in allBoxes)
            {
                //1つでもチェックされていない項目があれば false
                //以下の判定は「現在の全項目の数」と「チェックされている数」を比較
                if (clb.CheckedItems.Count != clb.Items.Count)
                {
                    allChecked = false;
                    break;
                }
            }
            //ChkSelectAll_CheckedChangedを誘発させない
            _isChangingCheckAll = true;
            ChkSelectAll.Checked = allChecked;
            _isChangingCheckAll = false;
        }

        private void Clb_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.BeginInvoke(new Action(() => {
                SyncSelectAllCheckBox();
            }));
        }
        //16進数コードと色名を結合した文字列を生成するヘルパーメソッド
        private string GetColorDisplayString(System.Drawing.Color color)
        {
            _logger?.Log("[INFO] GetColorDisplayStringヘルパーメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] 処理対象の色：[A={color.A}, R={color.R}, G={color.G}, B={color.B}]", (int)LogType.Debug);
            string hexString = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            _logger?.Log($"[INFO] 生成された16進数文字列：[{hexString}]", (int)LogType.Debug);
            //Web標準色名やシステム色名であれば、名前を併記する
            //デフォルト
            string colorName = "RGB値";
            _logger?.Log($"[INFO] IsNamedColorチェック：[{color.IsNamedColor}]", (int)LogType.Debug);
            if (color.IsNamedColor)
            {
                //IsKnownColor または IsSystemColor が true の場合、Nameを使用
                colorName = color.Name;
            }
            //最終的な表示文字列
            string result = $"{hexString} ({colorName})";
            _logger?.Log($"[INFO] 返却値：[{result}]", (int)LogType.Debug);
            _logger?.Log($"[INFO] GetColorDisplayStringを終了!!", (int)LogType.Debug);
            //例：#FF0000 (Red) または #1ABC9C (RGB値)
            return result;
        }

        private void BtnSelectColor_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnSelectColor_Clickイベントを開始!!", (int)LogType.Debug);
            //TxtEmbedColorから16進数コードのみを抽出してColorDialogに設定
            try
            {
                //TxtEmbedColorの内容を取得
                string currentText = TxtEmbedColor.Text.Trim();

                //最初に見つかった6桁の16進数コードを抽出
                Match match = Regex.Match(currentText, @"#?([0-9A-F]{6})", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string cleanHex = match.Groups[1].Value;
                    colorDialog.Color = ColorTranslator.FromHtml("#" + cleanHex);
                    _logger?.Log($"[INFO] TxtEmbedColorから既存の16進数 [{cleanHex}] を抽出し、ColorDialogに設定しました!!", (int)LogType.Debug);
                }
                else
                {
                    _logger?.Log($"[INFO] TxtEmbedColor [{currentText}] から有効な16進数コードを抽出できませんでした!! デフォルト色が使用されます!!", (int)LogType.Debug);
                }
            }
            catch (Exception ex)
            {
                //パース失敗時 (ColorTranslator.FromHtmlで無効な値など) はログに残すが、ColorDialogのデフォルト色が使われるため処理は続行
                _logger?.Log($"[WARNING] 既存色の設定中に例外発生!! (デフォルト色が使用されます)\n{ex.Message}", (int)LogType.DebugError);
                //パース失敗時は ColorDialog のデフォルト色が使われる
            }

            _logger?.Log("[INFO] ColorDialogを表示!!", (int)LogType.Debug);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                //選択された色をプレビューとTextBoxに反映
                System.Drawing.Color selectedColor = colorDialog.Color;
                PnlColorPreview.BackColor = selectedColor;

                //16進数と色名を併記して表示
                string colorDisplayString = GetColorDisplayString(selectedColor);
                TxtEmbedColor.Text = colorDisplayString;
                _logger?.Log($"[SUCCESS] ユーザーが色を選択しました!! 設定色：[{selectedColor.ToArgb():X8}] ({colorDisplayString})", (int)LogType.Debug);
            }
            else
            {
                _logger?.Log("[INFO] ユーザーがColorDialogをキャンセルしました!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] BtnSelectColor_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //埋め込みメッセージ枠色保存メソッド
        private void SaveEmbedColor()
        {
            _logger?.Log($"[INFO] SaveEmbedColorメソッドを開始!!", (int)LogType.Debug);
            string colorText = TxtEmbedColor.Text;
            //TxtEmbedColor から HEXコードを抽出
            Match match = Regex.Match(TxtEmbedColor.Text, @"#?([0-9A-F]{6})", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                try
                {
                    string hex = match.Groups[1].Value;
                    System.Drawing.Color c = ColorTranslator.FromHtml("#" + hex);

                    //Discord APIで扱いやすいように下位24bit(RGB)のuintに変換
                    //(R: 16bitシフト, G: 8bitシフト, B: そのまま)
                    uint colorValue = (uint)((c.R << 16) | (c.G << 8) | c.B);

                    //レジストリへの保存実行
                    RegistryHelper.SaveRoleEmbedColor(colorValue);

                    _logger?.Log($"[SUCCESS] 埋め込みカラーを保存しました!! HEX：[#{hex}], uint：[{colorValue}]", (int)LogType.Debug);
                }
                catch (Exception ex)
                {
                    _logger?.Log($"[ERROR] 色コードの変換または保存中に例外が発生しました!! 入力値：[{colorText}]\n例外：{ex.Message}", (int)LogType.DebugError);
                }
            }
            else
            {
                //正規表現にマッチしなかった場合（無効な形式など）
                _logger?.Log($"[WARNING] UI上の色コード形式が不正なため、保存をスキップしました!! 入力値：[{colorText}]", (int)LogType.DebugError);
            }
            _logger?.Log($"[INFO] SaveEmbedColorメソッドを終了!!", (int)LogType.Debug);
        }
        //チェックボックスの状態が変更されたときに実行されるイベントハンドラ
        private void ChkAutoDeleteEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを終了!!", (int)LogType.Debug);
        }
    }
}

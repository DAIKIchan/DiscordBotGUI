using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Plugin.Role
{
    public class RolePlugin : IPluginEventHandler, ICommandProvider, ICommandHandlerProvider
    {
        //コマンド側からアクセスするための静的インスタンス
        public static RolePlugin Instance { get; private set; }
        public string PluginName => "ロールパネルプラグイン(RolePlugin)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Role.dll";

        private ILogger _logger;
        private DiscordSocketClient _client;
        private DiscordBot_Role _RoleCommand;
        //メモリ上で管理する「現在有効な全パネル」のリスト（一時的＋永続の両方）
        private List<RolePanelData> _activePanels = new List<RolePanelData>();

        // ICommandHandlerProvider の契約
        public IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            if (_RoleCommand != null)
            {
                yield return _RoleCommand;
            }
        }
        public IEnumerable<ICommand> GetCommands() => Enumerable.Empty<ICommand>();
        //購読解除用のハンドラをメソッドとして定義
        private void HandleDataRegistered(ulong messageId, object data)
        {
            if (data is RolePanelData roleData)
            {
                RegisterPanel(messageId, roleData.RoleMaps, roleData.IsPermanent);
            }
        }
        //BOT本体からの初期化メソッド
        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            Instance = this;
            _logger = logger;
            _client = client;
            if (_RoleCommand == null)
            {
                _RoleCommand = new DiscordBot_Role(logger, client);
            }
            //1．レジストリから永続化が有効かどうかを確認
            bool isPermanentSettingEnabled = RegistryHelper.LoadRoleIsPermanent();

            if (isPermanentSettingEnabled)
            {
                //永続化が有効な場合のみ JSON からロード
                try
                {
                    var savedData = RoleStorageHelper.Load();
                    if (savedData != null && savedData.Count > 0)
                    {
                        _activePanels.AddRange(savedData);
                        _logger.Log($"[{PluginName}] 永続化有効：[{savedData.Count} 件] のパネルをロードしました!!", (int)LogType.Success);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"[{PluginName}] ロード失敗!!\n例外：{ex.Message}", (int)LogType.Error);
                }
            }
            else
            {
                _logger.Log($"[{PluginName}] 永続化無効：JSONの読み込みをスキップしました!!", (int)LogType.Success);
            }
            //Coreからの通知を待ち受ける設定
            DiscordBot.Core.RegistryHelper.OnDataRegistered += (messageId, data) =>
            {
                //届いたデータが「自分のプラグイン用(RolePanelData)」か確認
                if (data is RolePanelData roleData)
                {
                    _logger.Log($"[{PluginName}] 本体からの登録通知を受信しました!! ※MsgID：[{messageId}]", (int)LogType.Success);

                    //既存の RegisterPanel メソッドを呼び出して監視リスト追加と保存を行う
                    RegisterPanel(messageId, roleData.RoleMaps, roleData.IsPermanent);
                }
            };
            //設定変更通知を受け取ったらリストをリロードする
            RegistryHelper.OnSettingsChanged += () =>
            {
                _logger.Log($"[{PluginName}] 設定変更通知を受信!! リストをリロードします!!", (int)LogType.Success);

                //JSONから最新の状態を読み直してメモリを更新
                var refreshedData = RoleStorageHelper.Load();
                if (refreshedData != null)
                {
                    _activePanels = refreshedData;
                }
            };
            //リアクションイベントを購読
            _client.ReactionAdded += OnReactionAdded;
            //_client.ReactionRemoved += OnReactionRemoved;

            _logger.Log($"[{PluginName}] ロールパネルプラグインを初期化しました!!", (int)LogType.Success);
            _logger.Log($"[{PluginName}] コマンド [!{_RoleCommand.CommandName}] を登録しました!!", (int)LogType.Success);
        }
        public void Uninitialize()
        {
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            if (_client != null)
            {
                _client.ReactionAdded -= OnReactionAdded;
                //_client.ReactionRemoved -= OnReactionRemoved;
                RegistryHelper.OnDataRegistered -= HandleDataRegistered;
            }
            _RoleCommand = null;
            _logger = null;
            _client = null;
        }
        //新しいパネルを監視リストに登録
        public void RegisterPanel(ulong messageId, List<RoleEmoteMapItem> maps, bool isPermanent)
        {
            //添付ファイルの RolePanelData クラスを使用してデータを作成
            var newData = new RolePanelData
            {
                MessageId = messageId,
                RoleMaps = maps,
                IsPermanent = isPermanent,
                IsEnabled = true,
            };

            //まずメモリ上の管理リストに追加（これにより一時的パネルもこのセッション中は動作する）
            _activePanels.Add(newData);

            if (isPermanent)
            {
                // 永続化対象のみを抽出して JSON に保存
                var permanentList = _activePanels.Where(p => p.IsPermanent).ToList();
                RoleStorageHelper.Save(permanentList);
                _logger.Log($"[{PluginName}] メッセージID:[{messageId}] を永続パネルとしてJSONに保存しました。", (int)LogType.Debug);
            }
            else
            {
                _logger.Log($"[{PluginName}] メッセージID:[{messageId}] を一時パネルとして登録しました。", (int)LogType.Debug);
            }
        }
        // --- ロール付与ロジック ---
        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            // BOT自身のリアクションは処理しない
            if (reaction.UserId == _client.CurrentUser.Id) return;

            // 管理リスト内に該当するメッセージIDがあるか確認
            var panel = _activePanels.FirstOrDefault(p => p.MessageId == cachedMessage.Id);
            if (panel == null || !panel.IsEnabled) return;

            // リアクションされた絵文字がロールに対応しているか確認
            var map = panel.RoleMaps.FirstOrDefault(m => m.EmoteName == reaction.Emote.Name);
            if (map == null) return;

            try
            {
                // メッセージ、チャンネル、ギルド、ユーザーの各オブジェクトを取得
                var message = await cachedMessage.GetOrDownloadAsync();
                var channel = await cachedChannel.GetOrDownloadAsync() as SocketGuildChannel;
                var guild = channel?.Guild;
                if (guild == null || message == null) return;

                var role = guild.GetRole(map.RoleId);
                var user = reaction.User.Value as SocketGuildUser ?? guild.GetUser(reaction.UserId);

                if (role != null && user != null)
                {
                    string statusText = "";
                    Color embedColor;

                    // --- トグル（切り替え）ロジック ---
                    if (user.Roles.Any(r => r.Id == role.Id))
                    {
                        await user.RemoveRoleAsync(role);
                        statusText = $"ロール **【{role.Name}】** を解除しました。";
                        embedColor = Color.Orange;
                        _logger.Log($"[{PluginName}] ロール解除: {user.Username}", (int)LogType.Debug);
                    }
                    else
                    {
                        await user.AddRoleAsync(role);
                        statusText = $"ロール **【{role.Name}】** を付与しました。";
                        embedColor = Color.Green;
                        _logger.Log($"[{PluginName}] ロール付与: {user.Username}", (int)LogType.Debug);
                    }

                    // --- ★埋め込みメッセージで通知を送信 ---
                    var embed = new EmbedBuilder()
                        .WithAuthor(user)
                        .WithDescription($"{user.Mention} さん、{statusText}")
                        .WithColor(embedColor)
                        .Build();

                    // チャンネルに通知を送信
                    var notification = await message.Channel.SendMessageAsync(embed: embed);

                    // --- リアクションの自動削除 ---
                    // ユーザーがクリックしたリアクションを即座に消すことで、UIを常にクリーンに保つ
                    await message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    //5秒後に通知メッセージを自動削除
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000);
                        try { await notification.DeleteAsync(); } catch { /* 既に削除されている場合は無視 */ }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"[{PluginName}] ロール付与処理中にエラーが発生しました: {ex.Message}", (int)LogType.DebugError);
            }
        }
    }
}

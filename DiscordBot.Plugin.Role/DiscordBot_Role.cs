using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBot.Plugin.Role
{
    //ICommandHandler インターフェースを実装
    public class DiscordBot_Role : ICommandHandler
    {
        private ILogger _logger;
        private DiscordSocketClient _client;
        private IUser _commandExecutor;
        private static readonly Regex HeaderRegex = new Regex("\"([^\"]*)\"", RegexOptions.Compiled);

        //ICommandHandler インターフェースの実装
        public string CommandName => "role";
        public string PluginName => "ロールパネル(DiscordBot_Role)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Role.dll";

        public bool IsFinished => true;
        public ulong LastPromptMessageId { get; set; } = 0;
        public ulong LastPromptChannelId { get; set; } = 0;
        public int TimeoutMinutes => 0;
        public ulong FinalTimeoutMessageId { get; set; } = 0;
        //コンストラクタ群
        public DiscordBot_Role(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = null;
            _commandExecutor = null;
            _logger?.Log("----------------------------------------------------", (int)LogType.Success);
            _logger?.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);

        }
        public DiscordBot_Role(ILogger logger, DiscordSocketClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = client;
            _commandExecutor = null;
            _logger?.Log("----------------------------------------------------", (int)LogType.Success);
            _logger?.Log($"[{PluginName}(DLLログ)] ILoggerとDiscordClientの初期化に成功!!", (int)LogType.Success);
        }
        public DiscordBot_Role(ILogger logger, DiscordSocketClient client, IUser commandExecutor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = client;
            _commandExecutor = commandExecutor;
            _logger?.Log("----------------------------------------------------", (int)LogType.Success);
            _logger?.Log($"[{PluginName}(DLLログ)] ILogger、DiscordClient、IUserの初期化に成功!!", (int)LogType.Success);

        }
        //ICommandHandler 必須メンバー：対話中の入力処理(ロールパネルではテキスト入力は無視)
        public Task<MessageResponse> ExecuteInteractiveAsync(string userInput)
        {
            return Task.FromResult(new MessageResponse { ShouldDelete = true, DeleteDelayMs = 100, TimeoutMinutes = 0 });
        }
        //コマンド実行メソッド(ICommandHandlerの契約)
        public async Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを開始!!", (int)LogType.Debug);
            //設定の読み込み
            bool isPermanent = await Task.Run(() => RegistryHelper.LoadRoleIsPermanent());
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadRoleShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadRoleDeleteDelayMs());
            Discord.Color embedColor = await Task.Run(() => RegistryHelper.LoadRoleEmbedColor());
            string timestamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            //ギルド(サーバー)チャンネルでの実行を確認
            if (!(message.Channel is SocketGuildChannel guildChannel))
            {
                _logger.Log($"[{PluginName}(DLLログ, ERROR)] ギルド外で実行されました!! 処理を中止します!!", (int)LogType.DebugError);
                return new MessageResponse
                {
                    Embed = new EmbedBuilder()
                    .WithDescription("このコマンドはサーバ内でのみ使用できます!!")
                    .WithColor(Color.DarkRed).Build()
                };
            }
            SocketGuild guild = guildChannel.Guild;
            SocketGuildUser sender = message.Author as SocketGuildUser;
            ulong guildId = guild.Id;
            //--- 権限チェックロジック ---
            //管理者権限を持つユーザーはチェックをスキップ
            if (sender != null && sender.GuildPermissions.Administrator)
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者は管理者権限を所持しています!! ロールチェックをスキップします!!", (int)LogType.Debug);
            }
            else
            {
                //レジストリから「ロールパネル作成」を許可されたロールIDリストを取得
                List<ulong> allowedRoleIds = await Task.Run(() => RegistryHelper.LoadRoleAllowedRoleIds(guildId));

                if (allowedRoleIds.Any())
                {
                    bool hasAllowedRole = sender != null && sender.Roles.Any(r => allowedRoleIds.Contains(r.Id));
                    if (!hasAllowedRole)
                    {
                        _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 権限不足：実行者は許可されたロールを持っていません!!", (int)LogType.DebugError);
                        return new MessageResponse
                        {
                            Embed = new EmbedBuilder()
                                .WithTitle("🚫 権限エラー")
                                .WithDescription("このコマンドを実行するための権限を持っていません!!")
                                .WithColor(Color.DarkRed)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{message.Author.Username}]\n{timestamp}")
                                //.WithCurrentTimestamp()
                                .Build(),
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs
                        };
                    }
                }
            }

            string commandPrefix = $"!{CommandName}";
            string args = fullCommandText.Length > commandPrefix.Length
                ? fullCommandText.Substring(commandPrefix.Length).Trim()
                : string.Empty;

            //1．引数がない場合、またはパイプがない場合のエラーチェック
            var parts = args.Split(new[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                var inputEmbed = new EmbedBuilder()
                    .WithTitle("🚫 入力形式エラー")
                    .WithDescription("**ロール選択肢と本文は'|'で区切ってください!!**\n使用例：`!role \"タイトル\" \"内容\" | ロール名1 ロール名2`")
                    .WithColor(Color.DarkRed).Build();
                return new MessageResponse
                {
                    Embed = inputEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            string headerPart = parts[0].Trim();
            string optionsPart = parts[1].Trim();

            string title = "✨ ロール付与パネル";
            string description = "リアクションを押すと対応するロールが付与 / 解除されます!!";

            //2．タイトルと内容の解析
            try
            {
                var matches = HeaderRegex.Matches(headerPart);
                if (matches.Count >= 1) { title = matches[0].Groups[1].Value; }
                if (matches.Count >= 2) { description = matches[1].Groups[1].Value; }
            }
            catch (Exception ex)
            {
                _logger.Log($"[{PluginName}(DLLログ, ERROR)] タイトル・内容の解析エラー!!\n例外：{ex.Message}", (int)LogType.DebugError);
            }

            //3．ロール選択肢の解析
            string[] roleInputs = optionsPart.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (roleInputs.Length < 1 || roleInputs.Length > ReactionEmojis.MaxOptions)
            {
                var selectionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 ロール数エラー")
                    .WithDescription($"ロールは **1個以上**、最大 **{ReactionEmojis.MaxOptions} 個**まで指定してください!!")
                    .WithColor(Color.DarkRed).Build();
                return new MessageResponse
                {
                    Embed = selectionEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //4．ロールの検証と情報収集
            List<IEmote> reactions = new List<IEmote>();
            //ロールIDとエモート名をマッピングするためのヘルパーリスト
            List<RoleEmoteMapItem> roleMaps = new List<RoleEmoteMapItem>();
            //見つからなかったロールを保存するリスト
            List<string> missingRoles = new List<string>();
            string roleListText = "";

            for (int i = 0; i < roleInputs.Length; i++)
            {
                string roleInput = roleInputs[i].Trim();
                IEmote emote = ReactionEmojis.Numbers[i];

                //探し方を1行に集約
                SocketRole role = ulong.TryParse(roleInput, out ulong id)
                    ? guild.GetRole(id)
                    : guild.Roles.FirstOrDefault(r => r.Name.Equals(roleInput, StringComparison.OrdinalIgnoreCase));

                if (role == null)
                {
                    //エラーですぐ返さず、リストに記録して次へ
                    missingRoles.Add(roleInput);
                    continue;
                }

                //見つかった場合の処理
                reactions.Add(emote);
                roleMaps.Add(new RoleEmoteMapItem { RoleId = role.Id, EmoteName = emote.Name, RoleName = role.Name });
                roleListText += $"{emote.Name} **{role.Name}**\n";
            }

            //ループ終了後、見つからないロールが1つでもあればまとめて報告
            if (missingRoles.Count > 0)
            {
                //リストの中身を改行でつなげる
                string missingListText = string.Join("\n", missingRoles.Select(r => $"・`{r}`"));

                var notFoundEmbed = new EmbedBuilder()
                    .WithTitle("🚫 ロール検索エラー")
                    .WithDescription($"以下の指定されたロールが見つかりませんでした!!\n\n{missingListText}")
                    .WithColor(Color.DarkRed).Build();

                return new MessageResponse
                {
                    Embed = notFoundEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }
            //5．Embedの構築
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(embedColor)
                //.WithCurrentTimestamp()
                .WithFooter($"リアクションでロール付与 / 解除 | 作成者：[{message.Author.Username}]\n{timestamp}");

            embed.AddField("付与可能ロール", roleListText);

            //6．状態の保存(RolePlugin側でリアクションを処理するために必要)
            //送信用データの準備
            var panelData = new RolePanelData
            {
                MessageId = 0,
                RoleMaps = roleMaps,
                IsPermanent = isPermanent,
                IsEnabled = true,
            };
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] コマンド成功!! ロール付与パネルを作成します!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] パネル作成!! 永続化状態：[{isPermanent}]", (int)LogType.Debug);
            return new MessageResponse
            {
                Embed = embed.Build(),
                Reactions = reactions,
                ShouldDelete = false,
                DeleteDelayMs = 0,
                CustomData = panelData,
            };
        }
    }
}

using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Plugin.Ban
{
    internal class DiscordBot_Ban : ICommandHandler
    {
        //ICommandHandler インターフェースの実装
        public string CommandName => "ban";
        public string PluginName => "ユーザBAN(DiscordBot_Ban)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Ban.dll";

        private enum State { Initial, ReasonInput, Confirmation }
        private State _currentState = State.Initial;

        //処理に必要なデータ
        private IUser _targetUser = null;
        private string _banReason = null;
        //コマンド実行者(セッションオーナー)の情報
        private readonly IUser _commandExecutor;

        //Botが送信したプロンプトメッセージのID
        public ulong LastPromptMessageId { get; set; } = 0;
        public ulong LastPromptChannelId { get; set; } = 0;
        public ulong FinalTimeoutMessageId { get; set; } = 0;

        //依存関係
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;

        //対話が終了したかどうか
        public bool IsFinished => _currentState == State.Initial;

        //DLLロード用のコンストラクタ(Bot本体の初期化時に使用)
        public DiscordBot_Ban(ILogger logger, DiscordSocketClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = client;
            //この時点では実行者が不明なので null をセット
            _commandExecutor = null;
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);
        }
        //コマンド実行時のコンストラクタ(Bot本体がセッション開始時に使用)
        public DiscordBot_Ban(ILogger logger, DiscordSocketClient client, IUser commandExecutor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = client;
            _commandExecutor = commandExecutor;
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);
        }

        //1．最初のコマンド実行時 (!ban @メンション) の処理
        public async Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] fullCommandText：[{fullCommandText}]", (int)LogType.Debug);
            //自動削除、遅延時間、タイムアウト時間の読み込み
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadBanShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadBanDeleteDelayMs());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadBanTimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 設定読み込み：ShouldDelete=[{shouldDelete}], Timeout=[{timeoutMinutes} 分]", (int)LogType.Debug);
            //サーバー情報と権限チェック
            if (!(message.Channel is SocketGuildChannel guildChannel))
            {
                _logger?.Log($"[{PluginName}(DLLログ, WARNING)] DMからの実行を検出!! サーバチャンネルでのみ実行可能です!!", (int)LogType.DebugError);
                var permissionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 入力エラー")
                    .WithDescription("このコマンドはサーバ内でのみ実行可能です!!")
                    .WithColor(Color.DarkRed)
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .WithCurrentTimestamp()
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = permissionEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            SocketGuildUser sender = message.Author as SocketGuildUser;
            ulong guildId = guildChannel.Guild.Id;
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] コマンド実行者：[{sender?.Username}], ID：[{sender?.Id}], ギルドID：[{guildId}]", (int)LogType.Debug);
            //サーバー管理者権限チェック
            if (sender != null && !sender.GuildPermissions.Administrator)
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者は管理者ではないため、カスタムロールチェックを実行します!!", (int)LogType.Debug);
                //カスタムロールチェックのロジック
                List<ulong> allowedRoleIds = await Task.Run(() => RegistryHelper.LoadBanAllowedRoleIds(guildId));
                bool hasAllowedRole = allowedRoleIds.Any() && sender.Roles.Any(r => allowedRoleIds.Contains(r.Id));
                if (allowedRoleIds.Any() && !hasAllowedRole)
                {
                    _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 権限不足エラー：必要なカスタムロールがありません!!", (int)LogType.DebugError);
                    var permissionEmbed = new EmbedBuilder()
                        .WithTitle("🚫 権限エラー")
                        .WithDescription("このコマンドを実行するための権限ロールを持っていません!!")
                        .WithColor(Color.DarkRed)
                        .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                        .WithCurrentTimestamp()
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = permissionEmbed,
                        ShouldDelete = shouldDelete,
                        DeleteDelayMs = deleteDelayMs
                    };
                }
                else if (allowedRoleIds.Any() && hasAllowedRole)
                {
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者は許可されたカスタムロールを所持しています!!", (int)LogType.Debug);
                }
            }
            else
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者は管理者権限を所持しています!! ロールチェックをスキップします!!", (int)LogType.Debug);
            }

            //コマンド形式、対象ユーザーの特定
            string[] parts = fullCommandText.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 入力エラー：BAN対象ユーザが指定されていません!!", (int)LogType.DebugError);
                _currentState = State.Initial;
                var permissionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 対象選択エラー")
                    .WithDescription("BAN対象のユーザを指定してください!!\n例：`!ban @ユーザ` or `!ban ユーザID`")
                    .WithColor(Color.DarkRed)
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .WithCurrentTimestamp()
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = permissionEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //メンション/IDからのユーザー特定
            _targetUser = message.MentionedUsers.FirstOrDefault();
            if (_targetUser == null && ulong.TryParse(parts[1], out ulong targetId))
            {
                _targetUser = await _client.Rest.GetGuildUserAsync(guildChannel.Guild.Id, targetId);
            }
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 特定されたターゲットユーザ：[{_targetUser?.Username ?? "未検出"}], [ID：[{_targetUser?.Id.ToString() ?? "N/A"}]", (int)LogType.Debug);
            //対象ユーザーの無効チェック(BOT自身、実行者、存在しないユーザー)
            if (_targetUser == null || _targetUser.Id == sender.Id || _targetUser.Id == _client.CurrentUser.Id)
            {
                _logger?.Log($"[{PluginName}(DLLログ, ERROR)] 対象選択エラー：無効なBAN対象ユーザ ※自身/BOT/未検出", (int)LogType.DebugError);
                _currentState = State.Initial;
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("🚫 対象選択エラー")
                    .WithDescription("BAN対象のユーザが無効です!!\nBOT自身、コマンド実行者、または存在しないユーザはBANできません!!")
                    .WithColor(Color.DarkRed)
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .WithCurrentTimestamp()
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = errorEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //権限チェック(BOTのロールと対象ユーザーのロール比較)
            IGuildUser targetGuildUser = _targetUser as IGuildUser;
            SocketGuildUser selfUser = guildChannel.Guild.CurrentUser;

            if (targetGuildUser != null && selfUser != null)
            {
                //BOTが対象ユーザーより低いロールしか持っていない、またはオーナーはBANできない
                if (targetGuildUser.Hierarchy >= selfUser.Hierarchy || targetGuildUser.Id == targetGuildUser.Guild.OwnerId)
                {
                    _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 権限エラー：BOTのロールが対象ユーザより低いか、対象がサーバオーナーです!! BOT階層：[{selfUser.Hierarchy}], 対象階層：[{targetGuildUser.Hierarchy}]", (int)LogType.DebugError);
                    _currentState = State.Initial;
                    var hierarchyEmbed = new EmbedBuilder()
                        .WithTitle("🚫 権限エラー")
                        .WithDescription("BOTのロールが対象ユーザより低いため、BANを実行できません!!")
                        .WithColor(Color.DarkRed)
                        .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                        .WithCurrentTimestamp()
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = hierarchyEmbed,
                        ShouldDelete = shouldDelete,
                        DeleteDelayMs = deleteDelayMs
                    };
                }
                else
                {
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] BOT権限チェックOK：BOT階層が対象ユーザ階層より上位です!!", (int)LogType.Debug);
                }
            }

            //状態遷移とプロンプトメッセージの生成
            _currentState = State.ReasonInput;
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 状態を [{_currentState}] (理由入力) へ遷移させ、プロンプトを返します!!", (int)LogType.Debug);
            var selectEmbed = new EmbedBuilder()
                .WithTitle("✅ BAN理由 - メンバーBAN操作")
                .WithDescription($"ユーザ：[**{_targetUser.GlobalName ?? _targetUser.Username}**]")
                .AddField("BAN対象ユーザID", _targetUser.Id, true)
                .AddField($"セッション実行者", $"[{_commandExecutor.Username}]", false)
                .WithColor(Color.DarkRed)
                .WithFooter("BAN理由を入力してください!!\n(例：永続的な違反、アカウント凍結)")
                .Build();
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
            return new MessageResponse
            {
                Embed = selectEmbed,
                ShouldDelete = false,
                DeleteDelayMs = 0,
                TimeoutMinutes = timeoutMinutes
            };
        }

        //2．対話継続中 (理由入力、確認) の処理
        public async Task<MessageResponse> ExecuteInteractiveAsync(string userInput)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザ入力：[{userInput}]", (int)LogType.Debug);
            //自動削除の有効/無効と遅延時間を読み込む
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadBanShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadBanDeleteDelayMs());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadBanTimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 設定読み込み：ShouldDelete=[{shouldDelete}], Timeout=[{timeoutMinutes} 分], 現在の状態：[{_currentState}]", (int)LogType.Debug);
            //エラーチェック
            if (_targetUser == null)
            {
                _logger?.Log($"[{PluginName}(DLLログ, ERROR)] セッションエラー：_targetUserがnullです!!", (int)LogType.DebugError);
                _currentState = State.Initial;
                var failureEmbed = new EmbedBuilder()
                    .WithTitle("❌ セッションエラー")
                    .WithDescription($"対話状態が無効になりました!! 再度 `!ban @メンション` or `!ban ユーザID` から開始してください!!")
                    .WithColor(Color.Red)
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = failureEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ターゲットユーザ：[{_targetUser.GlobalName ?? _targetUser.Username}], ID：[{_targetUser.Id}]", (int)LogType.Debug);
            switch (_currentState)
            {
                case State.ReasonInput:
                    //理由の入力
                    _banReason = userInput.Trim();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 理由入力状態!! 入力された理由：[{_banReason}]", (int)LogType.Debug);
                    if (string.IsNullOrWhiteSpace(_banReason))
                    {
                        _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 入力エラー：BAN理由が空白です!!", (int)LogType.DebugError);
                        var failureEmbed = new EmbedBuilder()
                            .WithTitle("❌ 入力エラー")
                            .WithDescription($"BAN理由を入力してください!!")
                            .WithColor(Color.Red)
                            .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                            .Build();
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                        return new MessageResponse
                        {
                            Embed = failureEmbed,
                            ShouldDelete = false,
                            DeleteDelayMs = 0,
                            TimeoutMinutes = timeoutMinutes
                        };
                    }

                    //状態を「確認待ち」に遷移
                    _currentState = State.Confirmation;
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 状態を [Confirmation] へ遷移!! 最終確認プロンプトを返します!!", (int)LogType.Debug);
                    //最終確認メッセージを埋め込みで作成
                    var confirmEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ 最終確認 - メンバーBAN操作")
                        .WithDescription($"ユーザ：[**{_targetUser.GlobalName ?? _targetUser.Username}**] さんをサーバからBANしますか？")
                        .AddField("BAN対象ユーザID", _targetUser.Id, true)
                        .AddField($"セッション実行者", $"[{_commandExecutor.Username}]", false)
                        .AddField("BAN理由", _banReason, false)
                        .WithColor(Color.LightOrange)
                        .WithFooter($"実行する場合は 'yes' を、中止する場合は 'no' を入力してください!!")
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = confirmEmbed,
                        ShouldDelete = false,
                        DeleteDelayMs = 0,
                        TimeoutMinutes = timeoutMinutes
                    };

                case State.Confirmation:
                    //確認の入力
                    string input = userInput.ToLowerInvariant().Trim();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 確認待ち状態!! ユーザ入力：[{input}]", (int)LogType.Debug);
                    if (input == "yes" || input == "y")
                    {
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザがBAN実行を承認しました!!", (int)LogType.Debug);
                        //ユーザをBANする処理を実行
                        IGuildUser targetGuildUser = _targetUser as IGuildUser;

                        if (targetGuildUser == null)
                        {
                            _logger?.Log($"[{PluginName}(DLLログ, ERROR)] BAN実行失敗：対象ユーザ情報 (IGuildUser) がサーバで見つかりませんでした!!", (int)LogType.DebugError);
                            _currentState = State.Initial;
                            //失敗メッセージを埋め込みで作成
                            var failureEmbedFallback = new EmbedBuilder()
                                .WithTitle("❌ BAN実行失敗")
                                .WithDescription("対象ユーザ情報がサーバで見つかりませんでした!!\nセッションを終了します!!")
                                .WithColor(Color.Red)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                                .Build();
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = failureEmbedFallback,
                                ShouldDelete = shouldDelete,
                                DeleteDelayMs = deleteDelayMs
                            };
                        }

                        try
                        {
                            //最終BAN処理(Discord API呼び出し)
                            //BANはサーバー単位の操作
                            //pruneDays: 0 を指定するとメッセージは削除されない(7日間に設定)
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] Discord API経由でBAN処理を呼び出します!! PruneDays：7, 理由：[{_banReason}]", (int)LogType.Debug);
                            await targetGuildUser.Guild.AddBanAsync(_targetUser, pruneDays: 7, reason: _banReason);

                            string targetUserName = _targetUser?.GlobalName ?? _targetUser?.Username ?? "不明なユーザ";

                            //成功ログ
                            _logger.Log($"[{PluginName}(DLLログ)] -------------------- [ユーザBAN処理] --------------------", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] ユーザ名：[{targetUserName}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] ユーザID：[{_targetUser.Id}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] BAN理由：[{_banReason}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] 上記の理由によりサーバからBANされました!!", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] -------------------- [ユーザBAN処理] --------------------", (int)LogType.Debug);

                            //成功埋め込みを作成
                            var successEmbed = new EmbedBuilder()
                                .WithTitle("✅ BAN実行成功")
                                .WithDescription($"ユーザ [**{_targetUser.GlobalName ?? _targetUser.Username}**] をBANしました!!")
                                .AddField("BAN理由", _banReason, false)
                                .WithColor(Color.Green)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                                .Build();

                            //状態を初期に戻し、セッション終了
                            _currentState = State.Initial;
                            this._targetUser = null;
                            this._banReason = null;
                            this.FinalTimeoutMessageId = 0;
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] BAN成功!! セッションを終了します!!", (int)LogType.Debug);
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = successEmbed,
                                ShouldDelete = false,
                                DeleteDelayMs = 0
                            };
                        }
                        catch (Exception ex)
                        {
                            string targetUserName = _targetUser?.GlobalName ?? _targetUser?.Username ?? "不明なユーザ";
                            //BAN処理失敗時のエラーメッセージ
                            _logger?.Log($"[{PluginName}(DLLログ, FATAL)] BAN処理中にエラーが発生しました!! 対象ユーザ：[{targetUserName}]\n例外：{ex.Message}", (int)LogType.DebugError);

                            var errorEmbed = new EmbedBuilder()
                                .WithTitle("❌ BAN失敗")
                                .WithDescription($"ユーザ [**{targetUserName}**] のBANに失敗しました!!\nBOTに適切な権限があるか、対象ユーザがBOTより高いロールを持っていないか確認してください!!\nエラーメッセージ：{ex.Message.Substring(0, Math.Min(ex.Message.Length, 100))}...")
                                .WithColor(Color.Red)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                                .Build();

                            _currentState = State.Initial;
                            this._targetUser = null;
                            this._banReason = null;
                            this.FinalTimeoutMessageId = 0;
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] BAN失敗。セッションを終了します。", (int)LogType.Debug);
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = errorEmbed,
                                ShouldDelete = shouldDelete,
                                DeleteDelayMs = deleteDelayMs
                            };
                        }
                    }
                    else if (input == "no" || input == "n")
                    {
                        //キャンセル
                        _currentState = State.Initial;
                        this.FinalTimeoutMessageId = 0;
                        this._targetUser = null;
                        this._banReason = null;
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザがBAN操作をキャンセルしました!!", (int)LogType.Debug);
                        var cancelEmbed = new EmbedBuilder()
                            .WithTitle("❌ 操作キャンセル")
                            .WithDescription("BAN操作はキャンセルされました!!")
                            .WithColor(Color.LightGrey)
                            .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                            .Build();
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                        return new MessageResponse
                        {
                            Embed = cancelEmbed,
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs
                        };
                    }
                    else
                    {
                        //無効な入力
                        _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 無効な入力：[{input}] 再度入力を促します!!", (int)LogType.DebugError);
                        var invalidEmbed = new EmbedBuilder()
                            .WithTitle("⚠️ 入力エラー")
                            .WithDescription("入力が認識できませんでした!!\n『yes』または『no』を入力してください!!")
                            .WithColor(Color.Orange)
                            .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                            .Build();
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                        return new MessageResponse
                        {
                            Embed = invalidEmbed,
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs,
                            TimeoutMinutes = timeoutMinutes,
                            IsTransient = true
                        };
                    }

                default:
                    //不正な状態遷移
                    _currentState = State.Initial;
                    this.LastPromptMessageId = 0;
                    this.LastPromptChannelId = 0;
                    _logger?.Log($"[{PluginName}(DLLログ, FATAL)] 不正な状態：[{_currentState}] に遷移しました!! セッションをリセットします!!", (int)LogType.DebugError);
                    var fraudEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ エラー")
                        .WithDescription("対話が不正な状態になりました!!\n再度 `!ban @メンション` or `!ban ユーザID` から開始してください!!")
                        .WithColor(Color.Orange)
                        .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = fraudEmbed,
                        ShouldDelete = shouldDelete,
                        DeleteDelayMs = deleteDelayMs
                    };
            }
        }
    }
}

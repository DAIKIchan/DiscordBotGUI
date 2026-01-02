using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using Discord.Net.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBot.Plugin.Kick
{
    public class DiscordBot_Kick : ICommandHandler
    {
        //ICommandHandler インターフェースの実装
        public string CommandName => "kick";
        public string PluginName => "ユーザKick(DiscordBot_Kick)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Kick.dll";

        private enum State { Initial, ReasonInput, Confirmation }
        private State _currentState = State.Initial;

        //処理に必要なデータ
        private IUser _targetUser = null;
        private string _kickReason = null;
        //コマンド実行者（セッションオーナー）の情報
        private readonly IUser _commandExecutor;

        //Botが送信したプロンプトメッセージのID
        public ulong LastPromptMessageId { get; set; } = 0;
        public ulong LastPromptChannelId { get; set; } = 0;
        //タイムアウト時に削除すべきメッセージのID
        public ulong FinalTimeoutMessageId { get; set; } = 0;

        //依存関係
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;

        //対話が終了したかどうかをDLLがBOT本体に伝えるフラグ
        public bool IsFinished => _currentState == State.Initial;

        //DLLロード用のコンストラクタを追加
        public DiscordBot_Kick(ILogger logger, DiscordSocketClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            //_logger = logger;
            _client = client;
            //この時点では実行者が不明なので null をセット
            _commandExecutor = null;
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);
        }
        //コンストラクタで依存関係を受け取る
        public DiscordBot_Kick(ILogger logger, DiscordSocketClient client, IUser commandExecutor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            //_logger = logger;
            _client = client;
            _commandExecutor = commandExecutor;
            //許可ロールを設定
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);
        }
        //1．最初のコマンド実行時(!kick @メンション)の処理
        public async Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] fullCommandText：[{fullCommandText}]", (int)LogType.Debug);
            //自動削除の有効/無効と遅延時間を読み込む
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadKickShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadKickDeleteDelayMs());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadKickTimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 設定読み込み：ShouldDelete=[{shouldDelete}], Timeout=[{timeoutMinutes} 分]", (int)LogType.Debug);
            //サーバー情報を取得(DMは許可しない)
            if (!(message.Channel is SocketGuildChannel guildChannel))
            {
                _logger?.Log($"[{PluginName}(DLLログ, WARNING)] DMからの実行を検出!! サーバチャンネルでのみ実行可能です!!", (int)LogType.DebugError);
                var permissionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 入力エラー")
                    .WithDescription("このコマンドはサーバ内でのみ実行可能です!!")
                    .WithColor(Color.DarkRed)
                    //実行者情報をフッターに追加
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .WithCurrentTimestamp()
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                //権限エラーの場合は早期にリターン
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
            //サーバー管理者権限を持つユーザーは、カスタムロールチェックをスキップする
            if (sender != null && sender.GuildPermissions.Administrator)
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者は管理者権限を所持しています!! ロールチェックをスキップします!!", (int)LogType.Debug);
            }
            else
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者は管理者ではないため、カスタムロールチェックを実行します!!", (int)LogType.Debug);
                //管理者ではない場合、カスタムロールによるチェックを行う
                //サーバーIDに基づいてレジストリから許可ロールIDリストを読み込む
                List<ulong> allowedRoleIds = await Task.Run(() => RegistryHelper.LoadKickAllowedRoleIds(guildId));

                //カスタム権限チェック
                //許可ロールが設定されている (Any())、かつ送信者がそのロールのいずれも持っていない場合にエラー
                bool hasAllowedRole = allowedRoleIds.Any() && sender != null && sender.Roles.Any(r => allowedRoleIds.Contains(r.Id));
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
                    //権限エラーの場合は早期にリターン
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

            //権限チェック後の既存の処理をここに続ける
            string[] parts = fullCommandText.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //1．コマンド形式の基本的なチェック
            if (parts.Length < 2)
            {
                _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 入力エラー：キック対象ユーザが指定されていません!!", (int)LogType.DebugError);
                _currentState = State.Initial;
                var permissionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 対象選択エラー")
                    .WithDescription("キック対象のユーザを指定してください!!\n例：`!kick @ユーザ` or `!kick ユーザID`")
                    .WithColor(Color.DarkRed)
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .WithCurrentTimestamp()
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                //権限エラーの場合は早期にリターン
                return new MessageResponse
                {
                    Embed = permissionEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //2．メンションリストから取得を試みる(最も簡単な方法)
            _targetUser = message.MentionedUsers.FirstOrDefault();

            //3．メンションで見つからなかった場合、第2引数をユーザーIDとして解析する
            if (_targetUser == null)
            {
                if (ulong.TryParse(parts[1], out ulong targetId))
                {
                    //ユーザーIDとして解析できた場合、クライアントからユーザーを取得する
                    //REST APIを使用してサーバーメンバーを確実かつ広範な型で取得
                    _targetUser = await _client.Rest.GetGuildUserAsync(guildChannel.Guild.Id, targetId);
                }
            }
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 特定されたターゲットユーザ：[{_targetUser?.Username ?? "未検出"}], ID：[{_targetUser?.Id.ToString() ?? "N/A"}]", (int)LogType.Debug);
            //4．対象ユーザーのチェック
            if (_targetUser == null || _targetUser.Id == sender.Id || _targetUser.Id == _client.CurrentUser.Id)
            {
                _logger?.Log($"[{PluginName}(DLLログ, ERROR)] 対象選択エラー：無効なキック対象ユーザ ※自身/BOT/未検出", (int)LogType.DebugError);
                _currentState = State.Initial;
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("🚫 対象選択エラー")
                    .WithDescription("キック対象のユーザが無効です!!\nBOT自身、コマンド実行者、または存在しないユーザはキックできません!!")
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
            //5．権限チェック (BOTのロールと対象ユーザーのロール比較)
            IGuildUser targetGuildUser = _targetUser as IGuildUser;
            SocketGuildUser selfUser = guildChannel.Guild.CurrentUser;

            if (targetGuildUser != null && selfUser != null)
            {
                //BOTが対象ユーザーより低いロールしか持っていない場合はキックできない
                //オーナーは必ずキックできない
                if (targetGuildUser.Hierarchy >= selfUser.Hierarchy || targetGuildUser.Id == targetGuildUser.Guild.OwnerId)
                {
                    _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 権限エラー：BOTのロールが対象ユーザより低いか、対象がサーバオーナです!! BOT階層：[{selfUser.Hierarchy}], 対象階層：[{targetGuildUser.Hierarchy}]", (int)LogType.DebugError);
                    _currentState = State.Initial;
                    var hierarchyEmbed = new EmbedBuilder()
                        .WithTitle("🚫 権限エラー")
                        .WithDescription("BOTのロールが対象ユーザより低いため、キックを実行できません!!")
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
                .WithTitle("✅ キック理由 - メンバーキック操作")
                .WithDescription($"ユーザ：[**{_targetUser.GlobalName ?? _targetUser.Username}**]")
                .AddField("キック対象ユーザID", _targetUser.Id, true)
                .AddField($"セッション実行者", $"[{_commandExecutor.Username}]", false)
                .WithColor(Color.DarkRed)
                .WithFooter("キック理由を入力してください!!\n(例：違反行為、荒らし)")
                .Build();
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
            //権限エラーの場合は早期にリターン
            return new MessageResponse
            {
                Embed = selectEmbed,
                ShouldDelete = false,
                DeleteDelayMs = 0,
                TimeoutMinutes = timeoutMinutes
            };
        }
        //2．対話継続中(理由入力、確認)の処理
        public async Task<MessageResponse> ExecuteInteractiveAsync(string userInput)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザ入力：[{userInput}]", (int)LogType.Debug);
            //自動削除の有効/無効と遅延時間を読み込む
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadKickShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadKickDeleteDelayMs());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadKickTimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 設定読み込み：ShouldDelete=[{shouldDelete}], Timeout=[{timeoutMinutes} 分], 現在の状態：[{_currentState}]", (int)LogType.Debug);
            if (_targetUser == null)
            {
                _logger?.Log($"[{PluginName}(DLLログ, ERROR)] セッションエラー：_targetUserがnullです!!", (int)LogType.DebugError);
                _currentState = State.Initial;
                var failureEmbed = new EmbedBuilder()
                    .WithTitle("❌ セッションエラー")
                    .WithDescription($"対話状態が無効になりました!再度 `!kick @メンション` or `!kick ユーザID` から開始してください!!")
                    .WithColor(Color.Red)
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = failureEmbed,
                    //自動削除
                    ShouldDelete = shouldDelete,
                    //プロンプトMSGの削除時間
                    DeleteDelayMs = deleteDelayMs
                };
            }
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ターゲットユーザ：[{_targetUser.GlobalName ?? _targetUser.Username}], ID：[{_targetUser.Id}]", (int)LogType.Debug);
            switch (_currentState)
            {
                case State.ReasonInput:
                    //理由の入力
                    _kickReason = userInput.Trim();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 理由入力状態!! 入力された理由：[{_kickReason}]", (int)LogType.Debug);
                    if (string.IsNullOrWhiteSpace(_kickReason))
                    {
                        _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 入力エラー：キック理由が空白です!!", (int)LogType.DebugError);
                        var failureEmbed = new EmbedBuilder()
                            .WithTitle("❌ 入力エラー")
                            .WithDescription($"キック理由を入力してください!!")
                            .WithColor(Color.Red)
                            .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                            .Build();
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                        return new MessageResponse
                        {
                            Embed = failureEmbed,
                            //自動削除
                            ShouldDelete = false,
                            //プロンプトMSGの削除時間
                            DeleteDelayMs = 0,
                            TimeoutMinutes = timeoutMinutes
                        };
                    }
                    //状態を「確認待ち」に遷移
                    _currentState = State.Confirmation;
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 状態を [Confirmation] へ遷移!! 最終確認プロンプトを返します!!", (int)LogType.Debug);
                    //最終確認メッセージを埋め込みで作成
                    var confirmEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ 最終確認 - メンバーキック操作")
                        .WithDescription($"ユーザ：[**{_targetUser.GlobalName ?? _targetUser.Username}**] さんをサーバからキックしますか？")
                        .AddField("キック対象ユーザID", _targetUser.Id, true)
                        .AddField($"セッション実行者", $"[{_commandExecutor.Username}]", false)
                        .AddField("キック理由", _kickReason, false)
                        .WithColor(Color.LightOrange)
                        .WithFooter($"実行する場合は 'yes' を、中止する場合は 'no' を入力してください!!")
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = confirmEmbed,
                        //自動削除を無効化
                        ShouldDelete = false,
                        //自動削除ナシ(メインフォームから削除)
                        DeleteDelayMs = 0,
                        TimeoutMinutes = timeoutMinutes
                    };

                case State.Confirmation:
                    //確認の入力
                    string input = userInput.ToLowerInvariant().Trim();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 確認待ち状態!! ユーザ入力：[{input}]", (int)LogType.Debug);
                    if (input == "yes" || input == "y")
                    {
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザがキック実行を承認しました!!", (int)LogType.Debug);
                        //ユーザーをキックする処理を実行
                        //_targetUserはExecuteInitialAsyncでDiscordサーバー内のユーザーとして取得済み
                        IGuildUser targetGuildUser = _targetUser as IGuildUser;

                        //ユーザーが見つからない、またはSocketGuildUserではない場合 (防御的にチェック)
                        if (targetGuildUser == null)
                        {
                            _logger?.Log($"[{PluginName}(DLLログ, ERROR)] キック実行失敗：対象ユーザ情報 (IGuildUser) がサーバで見つかりませんでした!!", (int)LogType.DebugError);
                            //状態をリセット（対話終了）
                            _currentState = State.Initial;
                            this.FinalTimeoutMessageId = 0;
                            this._targetUser = null;
                            this._kickReason = null;
                            //失敗メッセージを埋め込みで作成
                            var failureEmbedFallback = new EmbedBuilder()
                                .WithTitle("❌ キック実行失敗")
                                .WithDescription("対象ユーザ情報がサーバで見つかりませんでした!!\nセッションを終了します!!")
                                .WithColor(Color.Red)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                                .Build();
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = failureEmbedFallback,
                                //自動削除
                                ShouldDelete = shouldDelete,
                                //プロンプトMSGの削除時間
                                DeleteDelayMs = deleteDelayMs
                            };
                        }
                        //キック実行直前に、ユーザーがまだギルドに存在するかを再チェック
                        IGuildUser currentGuildUser = await targetGuildUser.Guild.GetUserAsync(_targetUser.Id);

                        if (currentGuildUser == null)
                        {
                            _logger?.Log($"[{PluginName}(DLLログ, WARNING)] ユーザは既にサーバに存在しないため、キックは不要です!!", (int)LogType.DebugError);
                            //セッションを終了
                            _currentState = State.Initial;
                            this.FinalTimeoutMessageId = 0;
                            this._targetUser = null;
                            this._kickReason = null;
                            //_targetUserがnullの場合に備えて、安全な名前を取得
                            string targetUserName = _targetUser?.GlobalName ?? _targetUser?.Username ?? "不明なユーザ";
                            var alreadyKickedEmbed = new EmbedBuilder()
                                .WithTitle("⚠️ キック失敗")
                                .WithDescription($"ユーザ：[**{targetUserName ?? _targetUser.Username}**] は既にサーバに存在しません!!")
                                .WithColor(Color.Orange)
                                //実行者情報をフッターに追加 (Request 1 と共通化)
                                .WithFooter(footer => footer.Text = $"セッション実行者: {_commandExecutor.Username}")
                                .Build();
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = alreadyKickedEmbed,
                                ShouldDelete = shouldDelete,
                                DeleteDelayMs = deleteDelayMs
                            };
                        }

                        try
                        {
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] Discord API経由でKick処理を呼び出します!! 理由：[{_kickReason}]", (int)LogType.Debug);
                            //最終キック処理 (Discord API呼び出し)
                            //キックはサーバー単位の操作であり、targetGuildUserはどのサーバーのユーザーかを知っている
                            await targetGuildUser.KickAsync(_kickReason);
                            //成功メッセージで安全な名前を取得
                            string targetUserName = _targetUser?.GlobalName ?? _targetUser?.Username ?? "不明なユーザ";
                            //成功ログ
                            _logger.Log($"[{PluginName}(DLLログ)] -------------------- [ユーザKICK処理] --------------------", (int)LogType.Debug);
                            //_logger.Log($"[{PluginName}(DLLログ)] ユーザ名：[{_targetUser.GlobalName ?? _targetUser.Username}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] ユーザ名：[{targetUserName}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] ユーザID：[{_targetUser.Id}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] KICK理由：[{_kickReason}]", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] 上記の理由によりサーバからKICKされました!!", (int)LogType.Debug);
                            _logger.Log($"[{PluginName}(DLLログ)] -------------------- [ユーザKICK処理] --------------------", (int)LogType.Debug);

                            //成功埋め込みを作成
                            var successEmbed = new EmbedBuilder()
                                .WithTitle("✅ キック実行成功")
                                .WithDescription($"ユーザ [**{_targetUser.GlobalName ?? _targetUser.Username}**] をキックしました!!")
                                .AddField("キック理由", _kickReason, false)
                                .WithColor(Color.Green)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                                .Build();

                            //状態を初期に戻し、BOT本体にセッション終了を伝える
                            _currentState = State.Initial;
                            this._targetUser = null;
                            this._kickReason = null;
                            this.FinalTimeoutMessageId = 0;
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] キック成功!! セッションを終了します!!", (int)LogType.Debug);
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = successEmbed,
                                //自動削除
                                ShouldDelete = false,
                                DeleteDelayMs = 0
                            };
                        }
                        catch (Exception ex)
                        {
                            string targetUserName = _targetUser?.GlobalName ?? _targetUser?.Username ?? "不明なユーザ";
                            //キック処理失敗時のエラーメッセージ
                            _logger.Log($"[{PluginName}(DLLログ, FATAL)] キック処理中にエラーが発生しました!! 対象ユーザ：[{targetUserName}]\n例外：{ex.Message}", (int)LogType.DebugError);

                            var errorEmbed = new EmbedBuilder()
                                .WithTitle("❌ キック失敗")
                                //.WithDescription($"ユーザー [**{_targetUser.GlobalName ?? _targetUser.Username}**] のキックに失敗しました!!\nBOTに適切な権限があるか、対象ユーザーがBOTより高いロールを持っていないか確認してください!!\nエラーメッセージ：{ex.Message.Substring(0, Math.Min(ex.Message.Length, 100))}...")
                                .WithDescription($"ユーザ [**{targetUserName}**] のキックに失敗しました!!\nBOTに適切な権限があるか、対象ユーザがBOTより高いロールを持っていないか確認してください!!\nエラーメッセージ：{ex.Message.Substring(0, Math.Min(ex.Message.Length, 100))}...")
                                .WithColor(Color.Red)
                                .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                                .Build();

                            _currentState = State.Initial;
                            //最終プロンプトのIDリセットはBOT本体のIsFinished後の処理に任せる
                            this._targetUser = null;
                            this._kickReason = null;
                            this.FinalTimeoutMessageId = 0;
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] キック失敗!! セッションを終了します!!", (int)LogType.Debug);
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                            return new MessageResponse
                            {
                                Embed = errorEmbed,
                                //自動削除
                                ShouldDelete = shouldDelete,
                                //プロンプトMSGの削除時間
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
                        this._kickReason = null;
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザがキック操作をキャンセルしました!!", (int)LogType.Debug);
                        //最終プロンプトのIDリセットはBOT本体のIsFinished後の処理に任せる
                        var cancelEmbed = new EmbedBuilder()
                            .WithTitle("❌ 操作キャンセル")
                            .WithDescription("キック操作はキャンセルされました!!")
                            .WithColor(Color.LightGrey)
                            .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                            .Build();
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                        return new MessageResponse
                        {
                            Embed = cancelEmbed,
                            //自動削除
                            ShouldDelete = shouldDelete,
                            //プロンプトMSGの削除時間
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
                            //自動削除
                            ShouldDelete = shouldDelete,
                            //プロンプトMSGの削除時間
                            DeleteDelayMs = deleteDelayMs,
                            TimeoutMinutes = timeoutMinutes,
                            IsTransient = true
                        };
                    }

                default:
                    _currentState = State.Initial;
                    this.LastPromptMessageId = 0;
                    this.LastPromptChannelId = 0;
                    _logger?.Log($"[{PluginName}(DLLログ, FATAL)] 不正な状態：[{_currentState}] に遷移しました!! セッションをリセットします!!", (int)LogType.DebugError);
                    //無効な入力メッセージを埋め込みで作成
                    var fraudEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ エラー")
                        .WithDescription("対話が不正な状態になりました!!\n再度 `!kick @メンション` or `!kick ユーザID` から開始してください!!")
                        .WithColor(Color.Orange)
                        .WithFooter(footer => footer.Text = $"セッション実行者：[{_commandExecutor.Username}]")
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = fraudEmbed,
                        //自動削除
                        ShouldDelete = shouldDelete,
                        //プロンプトMSGの削除時間
                        DeleteDelayMs = deleteDelayMs
                    };
            }
        }
    }
}
using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Plugin.Delete
{
    public class DiscordBot_Delete : ICommandHandler
    {
        //ICommandHandler インターフェースの実装
        public string CommandName => "del";
        public string PluginName => "メッセージ削除(DiscordBot_Delete)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Delete.dll";

        private enum State { Initial, Confirmation, Executed }
        private State _currentState = State.Initial;

        //削除するメッセージの件数
        private int _messagesToDelete = 0;
        //実行チャンネルID (ExecuteInteractiveAsyncで使用)
        private ulong _currentChannelId = 0;
        private ulong _commandExecutorId = 0;

        //Botが送信したプロンプトメッセージのID
        public ulong LastPromptMessageId { get; set; } = 0;
        public ulong LastPromptChannelId { get; set; } = 0;
        //タイムアウト時に削除すべきメッセージのID
        public ulong FinalTimeoutMessageId { get; set; } = 0;

        //コマンド実行者
        private readonly IUser _commandExecutor;
        //依存関係
        private ILogger _logger;
        private DiscordSocketClient _client;

        //対話が終了したかどうかをDLLがBOT本体に伝えるフラグ
        //Initial(初期状態) または Executed(完了状態) の場合に true
        public bool IsFinished => _currentState == State.Initial || _currentState == State.Executed;
        //テンプレートインスタンス生成用(2引数)
        public DiscordBot_Delete(ILogger logger, DiscordSocketClient client)
        {
            _logger = logger;
            _client = client;
            _commandExecutor = null;
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);
        }
        //コンストラクタで依存関係を受け取る
        public DiscordBot_Delete(ILogger logger, DiscordSocketClient client, IUser commandExecutor)
        {
            _logger = logger;
            _client = client;
            _commandExecutor = commandExecutor;
            //初期状態に戻しておく
            _currentState = State.Initial;
            _logger.Log("----------------------------------------------------", (int)LogType.Debug);
            _logger.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Debug);
            _logger.Log($"[{PluginName}(DLLログ)] 対話ステータスの初期化に成功!!", (int)LogType.Debug);
        }

        //1．初回コマンド受付処理 (ICommandHandler.ExecuteInitialAsync)
        public async Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] fullCommandText：[{fullCommandText}]", (int)LogType.Debug);
            //初期状態に戻す
            _currentState = State.Initial;
            this.LastPromptMessageId = 0;
            this.LastPromptChannelId = 0;
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 状態を [{_currentState}] にリセットしました!!", (int)LogType.Debug);
            //自動削除の有効/無効と遅延時間を読み込む
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadDeleteShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadDeleteDeleteDelayMs());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadDeleteTimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 読み込み設定：ShouldDelete=[{shouldDelete}], DeleteDelayMs=[{deleteDelayMs} ms], Timeout=[{timeoutMinutes} 分]", (int)LogType.Debug);
            //実行コンテキストの保存
            _commandExecutorId = message.Author.Id;
            _currentChannelId = message.Channel.Id;
            //状態がInitialでない場合は、対話処理(ExecuteInteractiveAsync)に処理を任せるためnullを返す
            if (_currentState != State.Initial)
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 状態がInitialではないため、対話処理に委譲します!! 現在の状態：[{_currentState}]", (int)LogType.Debug);
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return null;
            }
            //ユーザーの権限チェック
            var user = message.Author as SocketGuildUser;
            if (user == null)
            {
                _logger?.Log($"[{PluginName}(DLLログ, WARNING)] DMからの実行を検出!! エラー応答を返します!!", (int)LogType.DebugError);
                //DMからの実行は許可しない
                var dmEmbed = new EmbedBuilder()
                    .WithTitle("⚠️ エラー")
                    .WithDescription("このコマンドはDMでは実行できません!! サーバチャンネルで実行してください!!")
                    .WithColor(Color.Orange)
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = dmEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //サーバーオーナーはすべての権限チェックをスキップする
            var guild = user.Guild;
            bool isGuildOwner = guild.OwnerId == user.Id;
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 実行者：[{user.Username}], ID：[{user.Id}], サーバオーナー：[{isGuildOwner}]", (int)LogType.Debug);
            //サーバーオーナーではない場合のみ、権限とロールのチェックを行う
            if (!isGuildOwner)
            {
                //必要な権限チェック: メッセージの管理
                if (!user.GuildPermissions.ManageMessages)
                {
                    _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 権限不足エラー：ユーザーに「メッセージの管理」権限がありません!!", (int)LogType.DebugError);
                    var permissionEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ 権限不足")
                        .WithDescription("メッセージを削除するには、`!del`コマンド実行者に「**メッセージの管理**」権限が必要です!!")
                        .WithColor(Color.Orange)
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = permissionEmbed,
                        ShouldDelete = shouldDelete,
                        DeleteDelayMs = deleteDelayMs
                    };
                }
                //ロール権限チェックの追加
                //レジストリから必要なロールIDを取得（存在しない場合は0を返す）
                List<ulong> requiredRoleIds = await Task.Run(() => RegistryHelper.LoadDeleteAllowedRoleIds(user.Guild.Id));
                //ロールIDリストがnullではなく、かつ要素が含まれている場合にロールチェックを実行
                if (requiredRoleIds != null && requiredRoleIds.Any())
                {
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 必須ロールチェックを実行します!! 必須ロールID数：[{requiredRoleIds.Count}]", (int)LogType.Debug);
                    //ユーザーが必須ロールのいずれかを持っているかチェック
                    bool hasRequiredRole = user.Roles.Any(r => requiredRoleIds.Contains(r.Id));

                    if (!hasRequiredRole)
                    {
                        //ユーザーがどの必須ロールも持っていない場合のエラー処理
                        //エラーメッセージ用に、必要なロールの名前をすべて取得して表示する
                        var requiredRoleNames = requiredRoleIds
                            .Select(id => guild.GetRole(id))
                            .Where(role => role != null)
                            .Select(role => $"**{role.Name}**")
                            .ToList();

                        //表示用のロール名リスト (例: "**ロールA**、**ロールB**、**ロールC**")
                        string rolesList = requiredRoleNames.Any() ? string.Join("、", requiredRoleNames) : "指定されたロール";
                        _logger?.Log($"[{PluginName}(DLLログ, WARNING)] ロール権限不足エラー：ユーザーは必要なロール [{rolesList}] を持っていません!!", (int)LogType.DebugError);
                        var roleEmbed = new EmbedBuilder()
                            .WithTitle("⚠️ 権限不足")
                            .WithDescription($"このコマンドを実行するには、以下のいずれかのロールが必要です!!\n\n{rolesList}")
                            .WithColor(Color.Orange)
                            .WithFooter(footer => footer.Text = $"セッション実行者：[{user.Username}#{user.Discriminator}]")
                            .Build();
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                        return new MessageResponse
                        {
                            Embed = roleEmbed,
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs
                        };
                    }
                }
            }
            //状態がInitialでない場合は処理しない (通常BOT本体が制御する)
            if (_currentState != State.Initial)
            {
                _logger?.Log($"[{PluginName}(DLLログ, ERROR)] セッション開始後のStateチェックでエラー検出：[{_currentState}]", (int)LogType.DebugError);
                //エラー応答 (DLL側で状態リセット)
                var fraudEmbed = new EmbedBuilder()
                    .WithTitle("⚠️ エラー")
                    .WithDescription("対話セッションが既に開始されています!!")
                    .WithColor(Color.Orange)
                    //フッターを追加
                    .WithFooter(footer => footer.Text = $"セッション実行者：[{user.Username}#{user.Discriminator}]")
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = fraudEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }
            //1．レジストリ設定の読み込み
            //フラグがtrueならカスタム件数を、falseならデフォルト件数(100)を上限として使用
            bool useCustomCount = RegistryHelper.LoadDeleteUseCustomCount();
            //設定フォームで指定された件数 (RegistryHelper側で下限2のチェックがされていると仮定)
            int customDeleteCount = RegistryHelper.LoadBulkDeleteCount();
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 削除件数設定：useCustomCount=[{useCustomCount}], customDeleteCount=[{customDeleteCount}]", (int)LogType.Debug);
            //2．削除件数の上限とデフォルト値の決定
            //'上限件数' は、コマンド引数で指定できる最大値、かつ !del のみの場合の件数の基準となる
            int deleteLimit = useCustomCount ? customDeleteCount : 100;
            //!del のみの場合に使用される件数
            int defaultCount = deleteLimit;
            //3．引数の解析: !del もしくは !del 数値
            string[] args = fullCommandText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int requestedCount = 0;
            //Discordのバルク削除APIの最小件数
            const int minimumCount = 2;
            if (args.Length == 1)
            {
                //!del のみの場合
                _messagesToDelete = defaultCount;
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 引数なし, デフォルト件数：[{_messagesToDelete} 件] を適用!!", (int)LogType.Debug);
            }
            //!del 数値 の場合
            else if (args.Length == 2 && int.TryParse(args[1], out requestedCount))
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 引数あり, ユーザー指定件数：[{requestedCount} 件] を適用!!", (int)LogType.Debug);
                //ユーザー指定の件数と、設定で決められた上限件数(deleteLimit)の小さい方を採用する
                int actualCount = Math.Min(requestedCount, deleteLimit);
                //最小件数と上限件数のチェック (上限件数はdeleteLimitを使用)
                if (actualCount < minimumCount)
                {
                    _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 削除件数不足エラー：[{actualCount} 件] は最小件数：[{minimumCount} 件] 未満です!!", (int)LogType.DebugError);
                    //2件未満はDiscordのバルク削除APIの制約で実行できないためエラー
                    var errorEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ 入力エラー")
                        .WithDescription($"削除できるメッセージ件数は**{minimumCount}件~{deleteLimit}件**までです!!")
                        .WithColor(Color.Orange)
                        .Build();
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                    return new MessageResponse
                    {
                        Embed = errorEmbed,
                        ShouldDelete = shouldDelete,
                        DeleteDelayMs = deleteDelayMs
                    };
                }
                _messagesToDelete = actualCount;
                //ユーザ指定が上限を超えた場合、ログを出力(エラーではない)
                if (requestedCount > deleteLimit)
                {
                    _logger.Log($"[{PluginName}(DLLログ, INFO)] ユーザー指定件数({requestedCount}件) が設定上限({deleteLimit}件) を超えたため、上限値({deleteLimit}件) で実行します!!", (int)LogType.Debug);
                }
            }
            else
            {
                _logger?.Log($"[{PluginName}(DLLログ, WARNING)] 不正な引数形式：args.Length=[{args.Length}]", (int)LogType.DebugError);
                //不正な引数
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("⚠️ 入力エラー")
                    .WithDescription("コマンドの形式が不正です!! 『!del』または『!del 数値』で入力してください!!")
                    .WithColor(Color.Orange)
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = errorEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //2．確認プロンプトの作成
            _currentState = State.Confirmation;

            var confirmEmbed = new EmbedBuilder()
                .WithTitle("🗑️ メッセージ一括削除の確認")
                .WithDescription($"このチャンネルの**過去 [{_messagesToDelete} 件]**のメッセージを削除します!!\n実行しますか？ `yes` または `no` を入力してください!!")
                .WithColor(Color.Red)
                // フッターを追加
                .WithFooter(footer => footer.Text = $"セッション実行者：[{user.Username}#{user.Discriminator}] | タイムアウト：{timeoutMinutes} 分")
                .Build();

            _logger.Log($"[{PluginName}(DLLログ, INFO)] ユーザー[{message.Author.Id}]からの削除要求!! (件数：{_messagesToDelete})を確認状態へ移行!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
            return new MessageResponse
            {
                Embed = confirmEmbed,
                //自動削除
                ShouldDelete = false,
                //自動削除遅延
                DeleteDelayMs = 0,
                //BOT本体がタイマーを開始
                TimeoutMinutes = timeoutMinutes,
            };
        }

        //2．対話応答処理 (ICommandHandler.ExecuteInteractiveAsync)
        public async Task<MessageResponse> ExecuteInteractiveAsync(string userInput)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザー入力：[{userInput}]", (int)LogType.Debug);
            //レジストリの値を取得
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadDeleteShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadDeleteDeleteDelayMs());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadDeleteTimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 設定読み込み：ShouldDelete=[{shouldDelete}], Timeout=[{timeoutMinutes} 分]", (int)LogType.Debug);
            //IsFinishedがtrueの場合は nullを返す (BOT本体がセッションを終了させる)
            if (IsFinished)
            {
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] IsFinishedがtrueのため、nullを返してセッションを終了させます!!", (int)LogType.Debug);
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
                return null;
            }

            string content = userInput.ToLower().Trim();
            MessageResponse response;

            switch (_currentState)
            {
                case State.Confirmation:
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 現在の状態：[Confirmation]", (int)LogType.Debug);
                    if (content == "yes" || content == "y")
                    {
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザーが削除を実行を承認 ('yes') しました!!", (int)LogType.Debug);
                        //1．削除実行 (100件を超える削除に対応するためループ処理に変更)
                        var targetChannel = _client.GetChannel(_currentChannelId) as ITextChannel;

                        if (targetChannel == null)
                        {
                            _logger?.Log($"[{PluginName}(DLLログ, ERROR)] 削除ターゲットチャンネルが見つからないか、DMです!!", (int)LogType.DebugError);
                            _currentState = State.Initial;
                            response = new MessageResponse
                            {
                                ShouldDelete = shouldDelete,
                                DeleteDelayMs = deleteDelayMs,
                                Embed = new EmbedBuilder()
                                .WithTitle("⚠️ エラー")
                                .WithDescription("DMではメッセージ削除を実行できません!!")
                                .WithColor(Color.Orange).Build()
                            };
                            break;
                        }

                        int totalDeletedCount = 0;
                        //最後に取得したメッセージのIDを保持。このIDより古いメッセージを次のバッチで取得する
                        //初期値は 0 (最新) で、初回は GetMessagesAsync(limit) と同等
                        ulong? lastMessageId = 0;
                        int remainingToDelete = _messagesToDelete;
                        //Discord APIのバルク削除および取得の最大件数 (100)
                        const int MaxMessagesPerBatch = 100;
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] 削除処理を開始!! 目標件数：[{_messagesToDelete} 件]", (int)LogType.Debug);
                        //削除すべき件数が残っている限りループ
                        while (remainingToDelete > 0)
                        {
                            //バッチで取得する件数 (最大100件)
                            //削除の要求件数と、APIの最大件数(100)の小さい方を採用!! ただし最低2件必要
                            int fetchLimit = Math.Max(2, Math.Min(remainingToDelete, MaxMessagesPerBatch));
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ループ開始!! 残り：[{remainingToDelete} 件], 取得件数制限：[{fetchLimit} 件], カーソルID：[{lastMessageId}]", (int)LogType.Debug);
                            IEnumerable<IMessage> messages;

                            if (lastMessageId.HasValue && lastMessageId.Value > 0)
                            {
                                //lastMessageId より古いメッセージを取得
                                //GetMessagesAsync(ulong referenceId, Direction dir, int limit) のオーバーロードを想定
                                messages = await targetChannel.GetMessagesAsync(
                                    lastMessageId.Value,
                                    Direction.Before,
                                    fetchLimit
                                ).FlattenAsync();
                            }
                            else
                            {
                                //初回または lastMessageId が 0 の場合、最新から取得
                                messages = await targetChannel.GetMessagesAsync(fetchLimit).FlattenAsync();
                            }

                            //取得したメッセージをリスト化
                            var fetchedList = messages.ToList();

                            if (fetchedList.Count == 0)
                            {
                                _logger?.Log($"[{PluginName}(DLLログ, INFO)] メッセージ取得ループ終了：取得メッセージなし!!", (int)LogType.Debug);
                                //メッセージが見つからなかった場合、ループ終了
                                break;
                            }
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 取得件数：[{fetchedList.Count} 件]", (int)LogType.Debug);
                            //削除可能なメッセージにフィルタリング (Discord APIの制約: 2週間（14日）以上前のメッセージは一括削除できない)
                            var deletableMessages = fetchedList
                                .Where(m => (DateTimeOffset.UtcNow - m.CreatedAt).TotalDays < 14)
                                //残りの削除要求件数を超えないように制限
                                .Take(remainingToDelete)
                                .ToList();
                            int nonDeletableCount = fetchedList.Count - deletableMessages.Count;
                            if (nonDeletableCount > 0)
                            {
                                _logger?.Log($"[{PluginName}(DLLログ, INFO)] 14日制限により、[{nonDeletableCount} 件] のメッセージをスキップしました!!", (int)LogType.Debug);
                            }
                            if (deletableMessages.Count == 0)
                            {
                                _logger?.Log($"[{PluginName}(DLLログ, INFO)] メッセージ取得ループ終了：削除可能なメッセージがなくなりました!! ※14日制限に到達!!", (int)LogType.Debug);
                                //14日制限により、これ以上削除可能なメッセージがなかった場合、ループ終了
                                break;
                            }

                            //バルク削除の実行
                            await targetChannel.DeleteMessagesAsync(deletableMessages);

                            //削除件数を更新
                            int currentDeletedCount = deletableMessages.Count;
                            totalDeletedCount += currentDeletedCount;
                            remainingToDelete -= currentDeletedCount;

                            //次のループのためのカーソルを設定
                            //削除対象となったメッセージの最も古いID (IDが最も小さいもの) を次の起点とする
                            lastMessageId = deletableMessages.Min(m => m.Id);
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] Batch 削除完了：[{currentDeletedCount} 件], 合計削除数：[{totalDeletedCount} 件], 次のカーソルID：[{lastMessageId}]", (int)LogType.Debug);
                            //Discord APIのレートリミット対策として、わずかに待機
                            if (currentDeletedCount > 0)
                            {
                                //連続実行によるレートリミットを避けるため、安全のために待機
                                await Task.Delay(1000);
                            }
                        }

                        //TotalDeletedCount が 0 の場合の処理
                        if (totalDeletedCount == 0)
                        {
                            _currentState = State.Initial;
                            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 削除対象が0件でした!! セッションを終了します!!", (int)LogType.Debug);
                            response = new MessageResponse
                            {
                                ShouldDelete = shouldDelete,
                                DeleteDelayMs = deleteDelayMs,
                                Embed = new EmbedBuilder()
                                .WithTitle("ℹ️ 削除対象なし")
                                .WithDescription("指定された件数で、過去14日以内の削除可能なメッセージは見つかりませんでした!!")
                                .WithColor(Color.LightGrey).Build()
                            };
                            break;
                        }

                        //2．完了メッセージ
                        //BOT本体のIsFinishedがこれを捕らえ、セッション終了処理を行う
                        _currentState = State.Executed;

                        var successEmbed = new EmbedBuilder()
                            .WithTitle("✅ 削除完了")
                            .WithDescription($"**[{totalDeletedCount} 件]**のメッセージを削除しました!!\n要求件数：[{_messagesToDelete} 件]")
                            .WithColor(Color.Green)
                            .Build();

                        _logger.Log($"[{PluginName}(DLLログ)] ユーザー[{_commandExecutorId}]により、[{totalDeletedCount} 件]のメッセージを削除しました!! 要求：[{_messagesToDelete} 件]", (int)LogType.UserMessage);

                        response = new MessageResponse
                        {
                            Embed = successEmbed,
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs
                        };
                        break;
                    }
                    else if (content == "no" || content == "n")
                    {
                        //キャンセル
                        _currentState = State.Initial;
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] ユーザーがメッセージ削除をキャンセル ('no') しました!!", (int)LogType.Debug);
                        var cancelEmbed = new EmbedBuilder()
                            .WithTitle("❌ 削除キャンセル")
                            .WithDescription("メッセージの一括削除をキャンセルしました!!")
                            .WithColor(Color.DarkGrey)
                            .Build();

                        _logger.Log($"[{PluginName}(DLLログ)] ユーザー[{_commandExecutorId}]により、メッセージ削除をキャンセルしました!!", (int)LogType.UserMessage);

                        response = new MessageResponse
                        {
                            Embed = cancelEmbed,
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs
                        };
                        break;
                    }
                    else
                    {
                        //無効な入力
                        _logger?.Log($"[{PluginName}(DLLログ, INFO)] 無効な入力検出!! 再度入力を促します!!", (int)LogType.Debug);
                        var invalidEmbed = new EmbedBuilder()
                            .WithTitle("⚠️ 無効な入力")
                            .WithDescription("『yes』または『no』を入力してください!!")
                            .WithColor(Color.Orange)
                            .Build();

                        //IsTransient=true により、BOT本体は古いプロンプトメッセージを削除せず、新しいメッセージをプロンプトとして上書きし、タイマーはリセットしない!!
                        response = new MessageResponse
                        {
                            Embed = invalidEmbed,
                            IsTransient = true,
                            //プロンプト継続
                            TimeoutMinutes = timeoutMinutes,
                            ShouldDelete = shouldDelete,
                            DeleteDelayMs = deleteDelayMs
                        };
                        break;
                    }

                default:
                    //不正な状態遷移
                    _currentState = State.Initial;
                    this.LastPromptMessageId = 0;
                    this.LastPromptChannelId = 0;
                    //this.FinalTimeoutMessageId = 0;
                    _logger?.Log($"[{PluginName}(DLLログ, ERROR)] 不正な状態：[{_currentState}] に遷移しました!! セッションをリセットします!!", (int)LogType.Error);
                    var fraudEmbed = new EmbedBuilder()
                        .WithTitle("⚠️ エラー")
                        .WithDescription("対話が不正な状態になりました!!\n再度 `!del` コマンドから開始してください!!")
                        .WithColor(Color.Orange)
                        .Build();

                    response = new MessageResponse
                    {
                        Embed = fraudEmbed,
                        ShouldDelete = shouldDelete,
                        DeleteDelayMs = deleteDelayMs
                    };
                    break;
            }
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 応答ステータス：[{_currentState}]", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
            return response;
        }
    }
}

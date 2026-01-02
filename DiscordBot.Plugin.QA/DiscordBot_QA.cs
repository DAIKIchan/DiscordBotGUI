using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

//DiscordBot.Plugin.QA プロジェクトの名前空間
namespace DiscordBot.Plugin.QA
{
    public class DiscordBot_QA : ICommandHandler
    {
        private ILogger _logger;
        private DiscordSocketClient _client;
        private IUser _commandExecutor;
        //状態を管理するための列挙型とフィールド
        private enum State { Initial, Active }
        private State _currentState = State.Initial;
        //ICommandHandler インターフェースの実装
        public string CommandName => "qa";
        public string PluginName => "アンケートQA(DiscordBot_QA)";
        //DLLVersion
        public string PluginVersion => "1.1.0.1";
        //DLL名称
        public string PluginDLL => "DiscordBot.Plugin.QA.dll";
        //Initial 状態のときのみ true を返し、BOT本体に新しいコマンドとして処理させる
        public bool IsFinished => true;
        public ulong LastPromptMessageId { get; set; } = 0;
        public ulong LastPromptChannelId { get; set; } = 0;
        //タイムアウト設定はMessageResponseで渡すため、ここではプロパティのみ定義
        public int TimeoutMinutes => 0;
        //タイムアウト時に削除すべきメッセージのID
        public ulong FinalTimeoutMessageId { get; set; } = 0;
        //コンストラクタ: ILogger を受け取る
        public DiscordBot_QA(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = null;
            _commandExecutor = null;
            _logger?.Log("----------------------------------------------------", (int)LogType.Success);
            _logger?.Log($"[{PluginName}(DLLログ)] ILoggerの初期化に成功!!", (int)LogType.Success);
        }
        //DLL初期化時にフレームワークが要求する可能性があるコンストラクタ
        public DiscordBot_QA(ILogger logger, DiscordSocketClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = client;
            _commandExecutor = null;
            _logger?.Log("----------------------------------------------------", (int)LogType.Success);
            _logger?.Log($"[{PluginName}(DLLログ)] ILoggerとDiscordClientの初期化に成功!!", (int)LogType.Success);
        }
        //コンストラクタ3：ILogger, DiscordSocketClient, IUser を引数とする (フレームワークが優先的に探す可能性のある形式)
        public DiscordBot_QA(ILogger logger, DiscordSocketClient client, IUser commandExecutor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"[{PluginName}(DLLログ)] Loggerインスタンスはnullにできません!!");
            _client = client;
            _commandExecutor = commandExecutor;
            _logger?.Log("----------------------------------------------------", (int)LogType.Success);
            _logger?.Log($"[{PluginName}(DLLログ)] ILogger、DiscordClient、IUserの初期化に成功!!", (int)LogType.Success);
        }
        public void Uninitialize()
        {
            _logger?.Log($"[{PluginName}(DLLログ)] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            //QAPluginがDiscordクライアントのイベントを購読していた場合、ここで解除します。
            //例：_client.MessageReceived -= OnMessageReceivedHandler;
            //Initializeで設定したフィールドをnullクリア
            _logger = null;
            _client = null;
            _commandExecutor = null;
        }
        //ICommandHandler 必須メンバーの追加：対話中の入力処理
        //アンケートはリアクションでの投票であり、テキスト入力はエラーとして応答を返す
        public Task<MessageResponse> ExecuteInteractiveAsync(string userInput)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを開始!!", (int)LogType.Debug);
            //レジストリからタイムアウト時間を再取得
            int currentTimeout = DiscordBot.Core.RegistryHelper.LoadQATimeoutMinutes();
            _logger?.Log($"[{PluginName}(DLLログ, ERROR)] アンケートはリアクション投票のみをサポートするため、テキスト入力は無視し、エラー応答を返します!! (TimeoutMinutes：[{currentTimeout} 分]", (int)LogType.DebugError);
            //エラーを出しつつ、タイマーは継続させる
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInteractiveAsyncメソッドを終了!!", (int)LogType.Debug);
            return Task.FromResult(new MessageResponse
            {
                //Embed = errorEmbed,
                ShouldDelete = true,
                DeleteDelayMs = 100,
                //タイムアウト継続をフレームワークに伝える
                TimeoutMinutes = currentTimeout
            });
        }
        //コマンド実行メソッド(ICommandの契約)
        public async Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText)
        {
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] fullCommandText：[{fullCommandText}]", (int)LogType.Debug);
            //自動削除の有効/無効と遅延時間を読み込む
            bool shouldDelete = await Task.Run(() => RegistryHelper.LoadQAShouldDelete());
            int deleteDelayMs = await Task.Run(() => RegistryHelper.LoadQADeleteDelayMs());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 自動削除設定を読み込みました!! ShouldDelete=[{shouldDelete}], DeleteDelayMs=[{deleteDelayMs}]ms", (int)LogType.Debug);
            //コマンド名(!qa) の後に続く引数を抽出
            string commandPrefix = $"!{CommandName}";
            string args = fullCommandText.Length > commandPrefix.Length
                ? fullCommandText.Substring(commandPrefix.Length).Trim()
                : string.Empty;
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] コマンド引数を抽出しました：[{args}]", (int)LogType.Debug);
            //1．引数がない場合のエラーチェック
            if (string.IsNullOrWhiteSpace(args))
            {
                _logger.Log($"[{PluginName}(DLLログ, WARNING)] 引数の値が空です!! 応答を返して終了します!!", (int)LogType.DebugError);
                var inputEmbed = new EmbedBuilder()
                    .WithTitle("🚫 入力エラー")
                    .WithDescription("**アンケートの形式が正しくありません!!**\n使用例：`!qa \"タイトル\" \"本文\" | QA1 QA2 QA3`")
                    .WithColor(Color.DarkRed)
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = inputEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            //2．タイトル、メッセージ、選択肢の解析
            //パイプ記号 '|' で「タイトル・説明部分」と「選択肢部分」に分割
            var parts = args.Split(new[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);

            //パイプ記号がない、または選択肢部分がない場合
            if (parts.Length < 2)
            {
                _logger.Log($"[{PluginName}(DLLログ, WARNING)] 入力形式エラー!! パイプ記号 '|' が見つからないか、選択肢部分が空です!! 応答を返して終了します!!", (int)LogType.DebugError);
                var inputEmbed = new EmbedBuilder()
                    .WithTitle("🚫 入力エラー")
                    .WithDescription("**アンケートの選択肢と本文は'|'で区切ってください!!**\n使用例：`!qa \"タイトル\" \"本文\" | QA1 QA2 QA3`")
                    .WithColor(Color.DarkRed)
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = inputEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            string headerPart = parts[0].Trim();
            string optionsPart = parts[1].Trim();
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ヘッダー部：[{headerPart}], オプション部：[{optionsPart}]", (int)LogType.Debug);
            string title = "📊 アンケートQA";
            string description = "以下の選択肢からリアクションで投票してください!!";

            //引用符 "..." を解析してタイトルと説明を抽出
            try
            {
                var matches = Regex.Matches(headerPart, "\"([^\"]*)\"");
                if (matches.Count >= 1)
                {
                    title = matches[0].Groups[1].Value;
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 解析されたタイトル：[{title}]", (int)LogType.Debug);
                }
                if (matches.Count >= 2)
                {
                    description = matches[1].Groups[1].Value;
                    _logger?.Log($"[{PluginName}(DLLログ, INFO)] 解析された説明：[{description}]", (int)LogType.Debug);
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"[{PluginName}(DLLログ, ERROR)] タイトル・説明の解析エラー：{ex.Message}", (int)LogType.DebugError);
            }

            //3．選択肢の解析
            string[] options = optionsPart.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] 解析された選択肢の数：[{options.Length}]", (int)LogType.Debug);
            if (options.Length < 2)
            {
                _logger.Log($"[{PluginName}(DLLログ, WARNING)] 選択肢の数が不足しています!! 2つ以上必要です!! 応答を返して終了します!!", (int)LogType.DebugError);
                var selectionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 選択肢エラー")
                    .WithDescription("選択肢は**2つ以上**必要です!!")
                    .WithColor(Color.DarkRed)
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = selectionEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }

            if (options.Length > ReactionEmojis.MaxOptions)
            {
                _logger.Log($"[{PluginName}(DLLログ, WARNING)] 選択肢が多すぎます!! 最大：[{ReactionEmojis.MaxOptions} 個] までです!! 応答を返して終了します!!", (int)LogType.DebugError);
                var selectionEmbed = new EmbedBuilder()
                    .WithTitle("🚫 選択肢エラー")
                    .WithDescription($"選択肢が多すぎます!!\n最大 **{ReactionEmojis.MaxOptions} 個**までです!!")
                    .WithColor(Color.DarkRed)
                    .Build();
                _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
                return new MessageResponse
                {
                    Embed = selectionEmbed,
                    ShouldDelete = shouldDelete,
                    DeleteDelayMs = deleteDelayMs
                };
            }
            //4．設定値の読み込み
            Discord.Color embedColor = await Task.Run(() => RegistryHelper.LoadQAEmbedColor());
            int timeoutMinutes = await Task.Run(() => RegistryHelper.LoadQATimeoutMinutes());
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] Embed設定を読み込みました!! Color=[{embedColor}], Timeout=[{timeoutMinutes} 分]", (int)LogType.Debug);
            //制限時間表示用の文字列を作成
            string footerText = $"投票期限：{(timeoutMinutes > 0 ? $"{timeoutMinutes} 分後" : "期限なし")}";
            //1．Embedの構築
            var embed = new EmbedBuilder()
                .WithTitle("アンケートQA：" + title)
                .WithDescription(description)
                .WithColor(embedColor)
                .WithCurrentTimestamp()
                .WithFooter(footerText);

            string optionList = "";
            List<IEmote> reactions = new List<IEmote>();

            for (int i = 0; i < options.Length; i++)
            {
                IEmote emote = ReactionEmojis.Numbers[i];
                reactions.Add(emote);
                optionList += $"{emote.Name} **{options[i]}**\n";
            }
            embed.AddField("選択肢", optionList);
            //成功したら状態を Active に変更し、タイマーを起動させる
            _currentState = State.Active;
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] コマンド成功!! 状態：[{_currentState}] に変更し、応答を返します!!", (int)LogType.Debug);
            //2．応答構造体を返す
            _logger?.Log($"[{PluginName}(DLLログ, INFO)] ExecuteInitialAsyncメソッドを終了!!", (int)LogType.Debug);
            return new MessageResponse
            {
                Embed = embed.Build(),
                //BOT本体がこのリストを使用してメッセージにリアクションを付与する
                Reactions = reactions,
                //読み込んだ制限時間をBOT本体に渡す
                TimeoutMinutes = timeoutMinutes,
            };
        }
    }
}
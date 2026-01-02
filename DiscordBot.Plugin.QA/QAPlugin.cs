using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Plugin.QA
{
    //プラグインエントリポイント
    public class QAPlugin : IPluginEventHandler, ICommandProvider, ICommandHandlerProvider
    {
        public string PluginName => "アンケートQAプラグイン(QAPlugin)";
        public string PluginVersion => "1.1.0.1";
        public string PluginDLL => "DiscordBot.Plugin.QA.dll";

        private ILogger _logger;
        private DiscordSocketClient _client;
        //MainForm と同じ値を持つローカルな enum を定義
        //コマンドインスタンスを保持
        //private List<ICommand> _commandsToProvide;
        private DiscordBot_QA _QACommand;
        //public IReadOnlyList<ICommand> Commands => new List<ICommand> { _QACommand };

        //ICommandProvider の契約 (DiscordBot_QA は ICommandHandlerProvider側で提供するため、空のリストを返す)
        public IEnumerable<ICommand> GetCommands()
        {
            return Enumerable.Empty<ICommand>();
        }

        //ICommandHandlerProvider の契約 (QAコマンドを ICommandHandler として提供する)
        public IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            if (_QACommand != null)
            {
                // 💡 ICommandHandler のインスタンスを返します
                yield return _QACommand;
            }
        }
        //BOT本体からの初期化メソッド
        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            _logger = logger;
            //リアクションが追加されたときのイベントを購読
            _client = client;
            //_QACommandをインスタンス化
            if (_QACommand == null)
            {
                //DiscordBot_QAはICommandHandlerとしてインスタンス化
                _QACommand = new DiscordBot_QA(logger, client);
            }
            _client.ReactionAdded += OnReactionAdded;
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[DLL初期化ログ]", (int)LogType.Success);
            _logger.Log($"[{PluginName}] アンケートQAプラグインを初期化しました!!", (int)LogType.Success);
            //コアの LogType enumをキャストして使用
            _logger.Log($"[{PluginName}] コマンド [!{_QACommand.CommandName}] を登録しました!!", (int)LogType.Success);
        }
        //Uninitialize メソッドを追加
        public void Uninitialize()
        {
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            //QAPluginがDiscordクライアントのイベントを購読していた場合、ここで解除します。
            //例：_client.MessageReceived -= OnMessageReceivedHandler;
            if (_client != null)
            {
                //イベント購読の解除
                _client.ReactionAdded -= OnReactionAdded;
            }
            //Initializeで設定したフィールドをnullクリア
            _QACommand = null;
            _logger = null;
            _client = null;
        }
        //1ユーザー1投票の制限を実装するイベントハンドラ
        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            //1．BOT自身の操作、およびDMでの操作は最優先で無視
            if (reaction.UserId == _client.CurrentUser.Id || (reaction.User.IsSpecified && reaction.User.Value.IsBot))
            {
                return;
            }

            //2．メッセージを取得
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message == null) return;

            //3．アンケートメッセージであるかを確認
            //BOTが送信し、かつEmbedのタイトルに「アンケートQA」が含まれるか判定
            var embed = message.Embeds.FirstOrDefault();
            if (message.Author.Id != _client.CurrentUser.Id || embed == null || !(embed.Title?.Contains("アンケートQA") ?? false))
            {
                //ここでリターンすることで、!roleなどの他パネルでのリアクション時はログを出さない
                return;
            }

            //--- 「アンケートQA」パネルであることが確定した後の処理 ---

            //レジストリから設定を読み込む(対象メッセージと確定してから実行)
            bool allowMultipleVotes = false;
            try
            {
                allowMultipleVotes = await Task.Run(() => RegistryHelper.LoadAllowMultipleVotesSetting());
            }
            catch (Exception ex)
            {
                _logger.Log($"[{PluginName}(DLLログ)] 設定読み込みエラー!!\n{ex.Message}", (int)LogType.DebugError);
            }

            //ログ出力(アンケートQA対象時のみ出力されるようになる)
            string newEmoteName = GetReadableEmoteName(reaction.Emote.Name);
            _logger.Log($"[{PluginName}(DLLログ)] OnReactionAdded受信(QA対象)：ユーザーID[{reaction.UserId}], 絵文字[{newEmoteName}], 1人1票制限[{allowMultipleVotes}]", (int)LogType.Debug);

            //アンケートの選択肢として使われる数字絵文字のリストを取得
            var pollEmoteNames = ReactionEmojis.Numbers.Select(e => e.Name).ToList();

            //新しく追加されたリアクションが選択肢外であれば削除
            if (!pollEmoteNames.Contains(reaction.Emote.Name))
            {
                await message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                _logger.Log($"[{PluginName}(DLLログ)] ユーザー：[{reaction.UserId}] が選択肢外のリアクション：[{reaction.Emote.Name}] を追加したため、削除しました!!", (int)LogType.Debug);
                return;
            }

            //1人1票制限が有効（allowMultipleVotes == false）の場合のロジック
            //※RegistryHelperの命名が LoadAllowMultipleVotes なので、falseの時が制限ありの状態
            if (!allowMultipleVotes)
            {
                //複数投票が許可されているなら何もしない
                return;
            }

            IEmote existingVoteEmote = null;

            //メッセージに付いている全てのリアクションをチェックして、重複投票を探す
            foreach (var messageReaction in message.Reactions)
            {
                IEmote currentEmote = messageReaction.Key;

                //選択肢の絵文字であり、かつ新しく追加されたものとは別の絵文字か確認
                if (pollEmoteNames.Contains(currentEmote.Name) && currentEmote.Name != reaction.Emote.Name)
                {
                    var reactors = await message.GetReactionUsersAsync(currentEmote, 100).FlattenAsync();

                    if (reactors.Any(u => u.Id == reaction.UserId))
                    {
                        existingVoteEmote = currentEmote;
                        break;
                    }
                }
            }

            //既存の投票がある場合、古い方を削除して1票に保つ
            if (existingVoteEmote != null)
            {
                await message.RemoveReactionAsync(existingVoteEmote, reaction.UserId);

                string existingEmoteName = GetReadableEmoteName(existingVoteEmote.Name);
                _logger.Log($"[{PluginName}(DLLログ)] ユーザー：[{reaction.UserId}] の既存投票：[{existingEmoteName}] を削除しました!! (1人1票制限)", (int)LogType.Debug);
            }
        }
        private string GetReadableEmoteName(string emoteName)
        {
            //全角の数字
            switch (emoteName)
            {
                case "1️⃣": return "1";
                case "2️⃣": return "2";
                case "3️⃣": return "3";
                case "4️⃣": return "4";
                case "5️⃣": return "5";
                case "6️⃣": return "6";
                case "7️⃣": return "7";
                case "8️⃣": return "8";
                case "9️⃣": return "9";
                case "🔟": return "10";
                //数字絵文字以外はそのまま返す
                default: return emoteName;
            }
        }
    }
}
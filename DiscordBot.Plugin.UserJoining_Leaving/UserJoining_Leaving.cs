using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System.Threading.Tasks;

namespace DiscordBot.Plugin.UserJoining_Leaving
{
    internal class UserJoining_Leaving : IPluginEventHandler
    {
        private ILogger _logger;
        private ulong _logChannelId = 0;
        //Uninitialize でイベントを解除するために client を保持するフィールド
        private DiscordSocketClient _client;
        public string PluginName => "ユーザ入退出ログ(UserJoining_Leaving)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.UserJoining_Leaving.dll";

        //BOT本体から渡されたクライアントを使ってイベントを購読する
        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            _logger = logger;
            _client = client;
            string channelIdStr = RegistryHelper.ReadChannelId();

            if (ulong.TryParse(channelIdStr, out ulong channelId) && channelId != 0)
            {
                _logChannelId = channelId;

                _client.UserJoined += OnUserJoined;
                _client.UserLeft += OnUserLeft;
                _logger.Log("----------------------------------------------------", (int)LogType.Success);
                _logger.Log($"[DLL初期化ログ]", (int)LogType.Success);
                _logger.Log($"[{PluginName}] イベント購読を設定しました!!ログチャンネルID：{_logChannelId}", (int)LogType.Success);
                _logger.Log($"[{PluginName}] ログチャンネルID：{_logChannelId}", (int)LogType.Success);
            }
            else
            {
                //読み込み失敗または設定されていない場合のログ
                _logger.Log("----------------------------------------------------", (int)LogType.Error);
                _logger.Log($"[DLL初期化ログ]", (int)LogType.Error);
                _logger.Log($"[{PluginName}] 入退出ログチャンネルIDが読み込めませんでした!!", (int)LogType.Error);
                _logger.Log($"[{PluginName}] [プラグイン]⇒[入退室ログ]から設定を確認してください!!", (int)LogType.Error);
            }
        }
        //Uninitialize メソッド(IUninitializer の契約)
        public void Uninitialize()
        {
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            if (_client != null)
            {
                //Initialize で登録したイベントをここで解除
                _client.UserJoined -= OnUserJoined;
                _client.UserLeft -= OnUserLeft;
            }
            //参照のクリア
            _logger = null;
            _client = null;
        }

        //ユーザー参加処理(埋め込みメッセージを使用)
        private async Task OnUserJoined(SocketGuildUser user)
        {
            _logger?.Log($"[{PluginName}(INFO)] OnUserJoinedメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(INFO)] 参加ユーザー：[{user.Username}], ID：[{user.Id}], ギルド：[{user.Guild.Name}]", (int)LogType.Debug);
            var channel = user.Guild.GetTextChannel(_logChannelId);
            if (channel != null)
            {
                _logger?.Log($"[{PluginName}(INFO)] ログチャンネル：[{channel.Name}] に入室メッセージを送信します!!", (int)LogType.Debug);
                //埋め込みメッセージを作成
                var embed = new EmbedBuilder()
                    .WithTitle("✅ 入室ログ")
                    .WithDescription($"`ユーザネーム：{user.Username}`\n **{user.Guild.Name}** に [{user.Mention}] が入室しました!!")
                    //緑色
                    .WithColor(Color.Green)
                    //ユーザーのアバター
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    //現在時刻
                    .WithCurrentTimestamp()
                    .Build();

                //埋め込みメッセージとして送信(テキストなし、embed のみ)
                await channel.SendMessageAsync(text: null, isTTS: false, embed: embed);
                _logger?.Log($"[{PluginName}(INFO)] 入室ログメッセージを正常に送信しました!!", (int)LogType.Debug);
            }
            else
            {
                _logger?.Log($"[{PluginName}(WARNING)] ログチャンネルID：[{_logChannelId}] に対応するチャンネルがギルド：[{user.Guild.Name}] に見つかりませんでした!! メッセージは送信されません!!", (int)LogType.DebugError);
            }
            _logger?.Log($"[{PluginName}(INFO)] OnUserJoinedメソッドを終了!!", (int)LogType.Debug);
        }

        //ユーザー退出処理(埋め込みメッセージを使用)
        private async Task OnUserLeft(SocketGuild guild, SocketUser user)
        {
            _logger?.Log($"[{PluginName}(INFO)] OnUserLeftメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[{PluginName}(INFO)] 退出ユーザー：[{user.Username}], ID：[{user.Id}], ギルド：[{guild.Name}]", (int)LogType.Debug);
            var channel = guild.GetTextChannel(_logChannelId);
            if (channel != null)
            {
                _logger?.Log($"[{PluginName}(INFO)] ログチャンネル：[{channel.Name}] に退出メッセージを送信します!!", (int)LogType.Debug);
                //埋め込みメッセージを作成
                var embed = new EmbedBuilder()
                    .WithTitle("❌ 退出ログ")
                    .WithDescription($"**{guild.Name}** から `{user.Username}` が退出しました!!")
                    //赤色
                    .WithColor(Color.Red)
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithCurrentTimestamp()
                    .Build();

                //埋め込みメッセージとして送信(テキストなし、embed のみ)
                await channel.SendMessageAsync(text: null, isTTS: false, embed: embed);
                _logger?.Log($"[{PluginName}(INFO)] 退出ログメッセージを正常に送信しました!!", (int)LogType.Debug);
            }
            else
            {
                _logger?.Log($"[{PluginName}(WARNING)] ログチャンネルID：[{_logChannelId}] に対応するチャンネルがギルド：[{guild.Name}] に見つかりませんでした!! メッセージは送信されません!!", (int)LogType.DebugError);
            }
            _logger?.Log($"[{PluginName}(INFO)] OnUserLeftメソッドを終了!!", (int)LogType.Debug);
        }
    }
}

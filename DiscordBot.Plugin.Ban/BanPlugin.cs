using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Plugin.Ban
{
    public class BanPlugin : IPluginEventHandler, ICommandHandlerProvider
    {
        public string PluginName => "ユーザBANプラグイン(BanPlugin)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Ban.dll";

        private ILogger _logger;
        private DiscordSocketClient _client;

        //ICommandHandlerのインスタンスを保持するリスト
        private List<ICommandHandler> _handlersToProvide = new List<ICommandHandler>();
        private DiscordBot_Ban _banCommand;

        //初期化処理
        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            _logger = logger;
            _client = client;

            //_banCommandをインスタンス化
            if (_banCommand == null)
            {
                //DiscordBot_BanはICommandHandlerとしてインスタンス化
                _banCommand = new DiscordBot_Ban(logger, client);
            }
            _handlersToProvide.Add(_banCommand);

            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[DLL初期化ログ]", (int)LogType.Success);
            _logger.Log($"[{PluginName}] ユーザBANプラグインを初期化しました!!", (int)LogType.Success);
            _logger.Log($"[{PluginName}] コマンド [!{_banCommand.CommandName}] を登録しました!!", (int)LogType.Success);
        }

        //アンロード処理
        public void Uninitialize()
        {
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            _logger = null;
            _client = null;
            _banCommand = null;
            _handlersToProvide = null;
        }

        //ICommandHandlerProvider の実装
        public IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            return _handlersToProvide;
        }

        //IPluginEventHandler の実装 (ここでは処理がないものとして定義)
        //public Task OnReadyAsync() => Task.CompletedTask;
        //public Task OnMessageReceivedAsync(SocketMessage message) => Task.CompletedTask;
    }
}

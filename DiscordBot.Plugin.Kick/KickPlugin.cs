using Discord;
using Discord.WebSocket;
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace DiscordBot.Plugin.Kick
{
    // 💡 ICommandHandlerProvider (ステップ1で追加) を実装
    public class KickPlugin : IPluginEventHandler, ICommandHandlerProvider
    {
        public string PluginName => "ユーザKickプラグイン(KickPlugin)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Kick.dll";

        private ILogger _logger;
        private DiscordSocketClient _client;

        // ICommandHandlerのインスタンスを保持するリスト
        private List<ICommandHandler> _handlersToProvide;
        private DiscordBot_Kick _kickCommand;

        public KickPlugin()
        {
            _handlersToProvide = new List<ICommandHandler>();
        }

        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            _logger = logger;
            _client = client;
            //_kickCommandをインスタンス化
            if (_kickCommand == null)
            {
                //DiscordBot_KickはICommandHandlerとしてインスタンス化
                _kickCommand = new DiscordBot_Kick(logger, client);
            }
            _handlersToProvide.Add(_kickCommand);
            //ログメッセージの文字列補間エラーを修正
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[DLL初期化ログ]", (int)LogType.Success);
            _logger.Log($"[{PluginName}] ユーザKickプラグインを初期化しました!!", (int)LogType.Success);
            _logger.Log($"[{PluginName}] コマンド [!{_kickCommand.CommandName}] を登録しました!!", (int)LogType.Success);
        }

        public void Uninitialize()
        {
            //Initializeで設定したフィールドをnullクリア
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            _logger = null;
            _client = null;
            _kickCommand = null;
            _handlersToProvide = null;
        }

        //ICommandHandlerProvider の実装
        public IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            return _handlersToProvide ?? Enumerable.Empty<ICommandHandler>();
        }
    }
}
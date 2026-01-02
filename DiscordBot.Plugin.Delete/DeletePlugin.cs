using Discord;
using Discord.WebSocket;
//ICommandHandlerProvider, ILogger などが含まれると仮定
using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

//DiscordBot.Plugin.Delete プロジェクトの名前空間
namespace DiscordBot.Plugin.Delete
{
    //ICommandHandlerProvider と IPluginEventHandler を実装
    public class DeletePlugin : IPluginEventHandler, ICommandHandlerProvider
    {
        public string PluginName => "メッセージ削除プラグイン(DeletePlugin)";
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Delete.dll";
        private ILogger _logger;
        private DiscordSocketClient _client;

        //ICommandHandlerのインスタンスを保持するリスト
        private List<ICommandHandler> _handlersToProvide;
        private DiscordBot_Delete _deleteCommand;

        public DeletePlugin()
        {
            _handlersToProvide = new List<ICommandHandler>();
        }

        //初期化処理
        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            _logger = logger;
            _client = client;
            //_deleteCommandをインスタンス化
            if (_deleteCommand == null)
            {
                //DiscordBot_DeleteはICommandHandlerとしてインスタンス化
                _deleteCommand = new DiscordBot_Delete(logger, client);
            }
            _handlersToProvide.Add(_deleteCommand);
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[DLL初期化ログ]", (int)LogType.Success);
            _logger.Log($"[{PluginName}] メッセージ削除プラグインを初期化しました!!", (int)LogType.Success);
            _logger.Log($"[{PluginName}] コマンド [!{_deleteCommand.CommandName}] を登録しました!!", (int)LogType.Success);
        }
        //アンロード処理
        public void Uninitialize()
        {
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            _logger = null;
            _client = null;
            _deleteCommand = null;
            _handlersToProvide = null;
        }
        //ICommandHandlerProvider の実装
        public IEnumerable<ICommandHandler> GetCommandHandlers()
        {
            return _handlersToProvide;
        }
    }
}
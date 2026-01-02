using Discord.WebSocket;
using DiscordBot.Core;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DiscordBot.Plugin.Weather
{
    //IPluginEventHandler, ICommandProvider, ICacheClearProvider のみを実装
    public class WeatherPlugin : IPluginEventHandler, ICommandProvider, ICacheClearProvider
    {
        public string PluginName => "天気予報プラグイン(WeatherPlugin)";
        //バージョン更新
        public string PluginVersion => "1.1.0.0";
        public string PluginDLL => "DiscordBot.Plugin.Weather.dll";

        private ILogger _logger;
        private DiscordBot_Weather _weatherCommand;

        //ICommandProvider の実装
        public IReadOnlyList<ICommand> Commands => new List<ICommand> { _weatherCommand };
        //ICacheClearProvider の実装(メインアプリケーションから ICacheClearProvider として直接呼び出す)
        public int ClearCache()
        {
            if (_weatherCommand == null)
            {
                _logger.Log($"[{PluginName}] コマンドが初期化されていません!!キャッシュをクリアできません!!", (int)LogType.DebugError);
                return 0;
            }
            //DiscordBot_Weather.cs で実装する ClearCache() メソッドを呼び出す
            return _weatherCommand.ClearCache();
        }

        //IPluginEventHandler：プラグインの初期化処理
        public void Initialize(DiscordSocketClient client, ILogger logger)
        {
            _logger = logger;
            //_weatherCommandをインスタンス化
            if (_weatherCommand == null)
            {
                //DiscordBot_WeatherはICommandとしてインスタンス化
                _weatherCommand = new DiscordBot_Weather(_logger);
            }
            //ログの初期化
            _logger.Log("----------------------------------------------------", (int)LogType.Success);
            _logger.Log($"[DLL初期化ログ]", (int)LogType.Success);
            _logger.Log($"[{PluginName}] お天気情報プラグインを初期化しました!!", (int)LogType.Success);
            //コアの LogType enumをキャストして使用
            _logger.Log($"[{PluginName}] コマンド [!{_weatherCommand.CommandName}] を登録しました!!", (int)LogType.Success);
        }

        //IPluginEventHandler：プラグインの終了処理
        public void Uninitialize()
        {
            _logger.Log($"[{PluginName}] DLLプラグインのアンロードを実行しました!!", (int)LogType.Success);
            _logger = null;
            _weatherCommand = null;
        }

        //ICommandProvider の GetCommands の実装 (IReadOnlyList<ICommand> Commands プロパティを使用)
        public IEnumerable<ICommand> GetCommands()
        {
            return Commands;
        }
    }
}

using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    public interface IPluginEventHandler : IUninitializer
    {
        //プラグインの名前
        string PluginName { get; }
        //DLLVersion
        string PluginVersion { get; }
        //DLL名称
        string PluginDLL { get; }

        //BOT本体から Discord クライアントを受け取り、イベント購読を設定する
        void Initialize(DiscordSocketClient client, ILogger logger);
    }
}
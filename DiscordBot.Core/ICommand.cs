using Discord.WebSocket;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    //単発コマンドのインターフェース
    public interface ICommand
    {
        //コマンド名(例："qa")
        string CommandName { get; }

        //プラグインの名前
        string PluginName { get; }
        //DLLVersion
        string PluginVersion { get; }
        //DLL名称
        string PluginDLL { get; }

        //実行メソッド(対話状態のフラグは不要)
        Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText);
    }
}
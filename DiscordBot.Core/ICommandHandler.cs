using Discord.WebSocket;
using System.Threading.Tasks;
namespace DiscordBot.Core
{
    //対話型インターフェース
    public interface ICommandHandler : ICommand
    {
        //コマンド名(例："test")
        //string CommandName { get; }

        //プラグインの名前
        //string PluginName { get; }
        //DLLVersion
        //string PluginVersion { get; }
        //DLL名称
        //string PluginDLL { get; }

        //対話が終了したかどうかをDLLがBOT本体に伝えるフラグ
        bool IsFinished { get; }
        //BOTが送信したプロンプトメッセージのID (MainFormからの設定と取得用)
        ulong LastPromptMessageId { get; set; }
        //BOTが送信したプロンプトメッセージのチャンネルID
        ulong LastPromptChannelId { get; set; }
        //タイムアウト時に削除すべきメッセージのID
        //このプロパティは、対話プロンプトが更新されるたびに、BOT本体によってセットされる
        ulong FinalTimeoutMessageId { get; set; }

        //最初のコマンド実行時 (!command) の処理!!
        //Task<MessageResponse> ExecuteInitialAsync(SocketMessage message, string fullCommandText);

        //対話継続中、ユーザーからの追加の入力があった場合の処理!!
        Task<MessageResponse> ExecuteInteractiveAsync(string userInput);
    }
}
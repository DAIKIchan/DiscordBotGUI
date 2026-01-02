using System.Collections.Generic;

namespace DiscordBot.Core
{
    //対話型実行コマンド (ICommandHandler) をメインアプリケーションに提供するためのインターフェース
    public interface ICommandHandlerProvider
    {
        //このプラグインが提供する ICommandHandler のインスタンスリストを取得
        IEnumerable<ICommandHandler> GetCommandHandlers();
    }
}
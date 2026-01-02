using System.Collections.Generic;

namespace DiscordBot.Core
{
    //単発実行コマンド (ICommand) をメインアプリケーションに提供するためのインターフェース
    //IPluginEventHandler を実装するクラスが、このインターフェースも実装することでコマンドを提供
    public interface ICommandProvider
    {
        //このプラグインが提供する ICommand のインスタンスリストを取得
        IEnumerable<ICommand> GetCommands();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    //プラグインがデータキャッシュをクリアする機能を提供するためのインターフェース
    //設定フォームなどからプラグイン内部のキャッシュを操作するために使用される
    public interface ICacheClearProvider
    {
        //プラグイン内のキャッシュをすべてクリアします。
        int ClearCache();
    }
}

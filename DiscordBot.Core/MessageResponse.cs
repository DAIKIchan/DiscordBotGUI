using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot.Core
{
    //DLLからBOT本体へ渡す、メッセージの構造
    public class MessageResponse
    {
        //互換性維持: メインフォームで使用されている Text プロパティ
        //public string Text { get; set; }

        //プラグインで使用されている ResponseText プロパティ
        public string ResponseText { get; set; }

        //埋め込みメッセージ部分(なければ null)
        public Embed Embed { get; set; }
        //自動削除フラグとタイマー(ミリ秒)
        public bool ShouldDelete { get; set; } = false;
        public int DeleteDelayMs { get; set; } = 0;

        //メッセージに付与するリアクションのリスト (アンケート機能に必須)
        public List<IEmote> Reactions { get; set; }
        //制限時間（分）をBOT本体に通知するためのプロパティ
        public int TimeoutMinutes { get; set; } = 0;

        //応答を一時的なものとして表示するかどうか
        public bool IsEphemeral { get; set; }
        //一時的なメッセージであり、LastPromptMessageIdを更新すべきでない場合に true
        public bool IsTransient { get; set; } = false;
        //ロールパネル設定
        public object CustomData { get; set; }
        //IsFinishedはICommandHandlerのプロパティで管理するため、ここでは省略
    }
}

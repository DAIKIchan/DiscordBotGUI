using Discord;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Core
{
    public static class PermissionHelper
    {
        //権限の日本語名とEnumの対応表
        private static readonly Dictionary<string, GuildPermission> PermissionMap = new Dictionary<string, GuildPermission>
        {
            //1．一般権限
            { "管理者", GuildPermission.Administrator },
            { "監査ログを表示", GuildPermission.ViewAuditLog },
            { "サーバーの管理", GuildPermission.ManageGuild },
            { "ロールの管理", GuildPermission.ManageRoles },
            { "チャンネルの管理", GuildPermission.ManageChannels },
            { "絵文字・スタンプの管理", GuildPermission.ManageEmojisAndStickers },
            { "チャンネルを見る", GuildPermission.ViewChannel },
            { "サーバーインサイトの表示", GuildPermission.ViewGuildInsights },
            { "ウェブフックの管理", GuildPermission.ManageWebhooks },
            //2．メンバー管理
            { "メンバーをキック", GuildPermission.KickMembers },
            { "メンバーをBAN", GuildPermission.BanMembers },
            { "メンバーをタイムアウト", GuildPermission.ModerateMembers },
            { "招待の作成", GuildPermission.CreateInstantInvite },
            { "ニックネームの変更", GuildPermission.ChangeNickname },
            { "ニックネームの管理", GuildPermission.ManageNicknames },
            //3．テキスト権限
            { "メッセージを送信", GuildPermission.SendMessages },
            //暫定
            { "メッセージをピン止め", GuildPermission.ManageMessages },
            //暫定
            { "低速モードを回避", GuildPermission.ManageMessages },
            { "エクスプレッションを作成", GuildPermission.CreateGuildExpressions },
            { "ボイスメッセージを送信", GuildPermission.SendVoiceMessages },
            { "投票の作成", GuildPermission.SendPolls },
            { "外部のアプリを使用", GuildPermission.UseExternalApps },
            { "スレッドでメッセージ送信", GuildPermission.SendMessagesInThreads },
            { "公開スレッドの作成", GuildPermission.CreatePublicThreads },
            { "非公開スレッドの作成", GuildPermission.CreatePrivateThreads },
            { "埋め込みリンク", GuildPermission.EmbedLinks },
            { "ファイルを添付", GuildPermission.AttachFiles },
            { "リアクションの追加", GuildPermission.AddReactions },
            { "外部の絵文字を使用", GuildPermission.UseExternalEmojis },
            { "外部のスタンプを使用", GuildPermission.UseExternalStickers },
            { "全員宛てメンション", GuildPermission.MentionEveryone },
            { "メッセージの管理", GuildPermission.ManageMessages },
            { "スレッドの管理", GuildPermission.ManageThreads },
            { "メッセージ履歴の閲覧", GuildPermission.ReadMessageHistory },
            { "読み上げメッセージ送信", GuildPermission.SendTTSMessages },
            { "アプリコマンドを使用", GuildPermission.UseApplicationCommands },
            //4．ボイス・イベント
            { "接続", GuildPermission.Connect },
            { "発言", GuildPermission.Speak },
            { "サウンドボードを使用", GuildPermission.UseSoundboard },
            { "外部のサウンドの使用", GuildPermission.UseExternalSounds },
            { "ボイスチャンネルステータスを使用", GuildPermission.SetVoiceChannelStatus },
            { "ユーザーアクティビティ", GuildPermission.StartEmbeddedActivities },
            { "Webカメラ", GuildPermission.Stream },
            { "音声検出を使用", GuildPermission.UseVAD },
            { "優先スピーカー", GuildPermission.PrioritySpeaker },
            { "メンバーをミュート", GuildPermission.MuteMembers },
            { "メンバーのスピーカーをミュート", GuildPermission.DeafenMembers },
            { "メンバーを移動", GuildPermission.MoveMembers },
            { "スピーカー参加リクエスト", GuildPermission.RequestToSpeak },
            { "イベントを作成", GuildPermission.CreateEvents },
            { "イベントの管理", GuildPermission.ManageEvents }
        };
        //日本語名から GuildPermission を取得
        public static GuildPermission? GetPermission(string jpName)
        {
            if (PermissionMap.TryGetValue(jpName, out var perm))
                return perm;
            return null;
        }
        //全ての定義済み日本語名を取得
        public static List<string> GetAllJpNames() => PermissionMap.Keys.ToList();
    }
}

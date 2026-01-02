using Discord;
using System.Collections.Generic;

namespace DiscordBot.Core
{
    public static class ReactionEmojis
    {
        //Discordで利用可能な数字の絵文字 (最大10個)
        public static readonly IEmote[] Numbers = new IEmote[]
        {
            new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣"), new Emoji("4️⃣"), new Emoji("5️⃣"),
            new Emoji("6️⃣"), new Emoji("7️⃣"), new Emoji("8️⃣"), new Emoji("9️⃣"), new Emoji("🔟")
        };
        public const int MaxOptions = 10;
    }
}
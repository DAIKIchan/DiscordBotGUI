using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core
{
    //ロールと絵文字のペア
    public class RoleEmoteMapItem
    {
        public ulong RoleId { get; set; }
        public string EmoteName { get; set; }
        public string RoleName { get; set; }
    }

    //パネル全体のデータ
    public class RolePanelData
    {
        public ulong MessageId { get; set; }
        public List<RoleEmoteMapItem> RoleMaps { get; set; } = new List<RoleEmoteMapItem>();
        public bool IsPermanent { get; set; }
        //有効フラグ
        public bool IsEnabled { get; set; } = true;
    }
}

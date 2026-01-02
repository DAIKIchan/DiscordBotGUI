using System.Collections.Generic;
using Newtonsoft.Json;

namespace DiscordBot.Core
{
    public class LicenseInfo
    {
        [JsonProperty("LicenseKey")]
        public string LicenseKey { get; set; }

        [JsonProperty("HardwareId")]
        public string HardwareId { get; set; }

        [JsonProperty("AppId")]
        public string AppId { get; set; }

        [JsonProperty("DateTime")]
        public string ExpiryDateTime { get; set; }

        [JsonProperty("LicenseType")]
        public string LicenseType { get; set; }
        //使用を許可するプラグインのリスト
        [JsonProperty("AllowedPlugins")]
        public List<string> AllowedPlugins { get; set; } = new List<string>();
    }

    public class LicenseFileRoot
    {
        [JsonProperty("LicenseInfo")]
        public List<LicenseInfo> LicenseInfos { get; set; } = new List<LicenseInfo>();
    }
}
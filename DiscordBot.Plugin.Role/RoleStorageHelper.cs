using DiscordBot.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBot.Plugin.Role
{
    /*public class RolePanelData
    {
        public ulong MessageId { get; set; }
        public List<RoleEmoteMapItem> RoleMaps { get; set; }
        public bool IsPermanent { get; set; }
        public bool IsEnabled { get; set; } = true;
    }*/
    public static class RoleStorageHelper
    {
        //保存先ディレクトリを定義
        //private static readonly string DirectoryPath = @"C:\DiscordBotGUI";
        //ファイルパスを結合
        //private static readonly string FileName = "Roles.json";
        //private static string FilePath => Path.Combine(DirectoryPath, FileName);

        public static void Save(List<RolePanelData> data)
        {
            try
            {
                //フォルダが存在しない場合は作成する
                if (!Directory.Exists(RegistryHelper.BaseDirectory))
                {
                    Directory.CreateDirectory(RegistryHelper.BaseDirectory);
                }

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(RegistryHelper.RoleJsonPath, json);
            }
            catch (Exception ex)
            {
                //必要に応じてログ出力などを追加してください
                Console.WriteLine($"Save Error: {ex.Message}");
            }
        }

        public static List<RolePanelData> Load()
        {
            try
            {
                //フォルダまたはファイルが存在しない場合は空のリストを返す
                if (!Directory.Exists(RegistryHelper.BaseDirectory) || !File.Exists(RegistryHelper.RoleJsonPath))
                    return new List<RolePanelData>();

                string json = File.ReadAllText(RegistryHelper.RoleJsonPath);
                return JsonConvert.DeserializeObject<List<RolePanelData>>(json) ?? new List<RolePanelData>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load Error: {ex.Message}");
                return new List<RolePanelData>();
            }
        }
    }
}

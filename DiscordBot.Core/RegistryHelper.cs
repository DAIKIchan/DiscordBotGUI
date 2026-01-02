using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Core
{
    public static class RegistryHelper
    {
        //ロガーインスタンスを保持するための静的フィールドを追加
        private static ILogger _logger;
        //1．パスの統一管理
        public static string RoleJsonPath => @"C:\DiscordBotGUI\Roles.json";
        public static string BaseDirectory => @"C:\DiscordBotGUI";
        //データが登録された時に発行されるイベント
        public static event Action<ulong, object> OnDataRegistered;
        //レジストリキーのパス
        private const string BaseRegistryPath = @"Software\DiscordBotGUI";
        //---------- 入退室ログ用 ----------
        private const string JoiningLeavingSubKey = "JoiningLeaving";
        private const string ChannelKey = "JoiningLeavingChannelId";
        //---------- アンケートQA用 ----------
        private const string QASubKey = "QA";
        //プロンプトMSG自動削除デフォルト値：10秒 (10000ミリ秒)
        private const int DefaultQADeleteDelayMs = 10000;
        //自動削除有効/無効のデフォルト設定 (1=有効)
        private const uint DefaultQAShouldDelete = 1U;
        //Discordのデフォルトカラー(青)
        private const uint DefaultQAEmbedColorValue = 0x3498DB;
        private const int DefaultTimeoutMinutes = 60;
        //1人1票制限有効のデフォルト設定(0=不許可, 1=許可/一人一票)
        private const uint DefaultAllowMultipleVotes = 0U;
        //---------- 天気予報プラグイン用 ----------
        private const string WeatherSubKey = "Weather";
        //データキャッシュ時間(分)のデフォルト値 (例：30分)
        private const int DefaultWeatherCacheDurationMinutes = 30;
        //プロンプトMSG自動削除デフォルト値：10秒 (10000ミリ秒)
        private const int DefaultWeatherDeleteDelayMs = 10000;
        //自動削除有効/無効のデフォルト設定 (1=有効)
        private const uint DefaultWeatherShouldDelete = 1U;
        //APIキー設定
        private const string WeatherApiKeyKey = "WeatherApiKey";
        //APIキーのデフォルト値(初回起動時にレジストリに書き込まれる値)
        private const string DefaultWeatherApiKey = "";
        //---------- ユーザKICKプラグイン用 ----------
        private const string KickSubKey = "Kick";
        private const string AllowedRolesKey = "AllowedRolesKey";
        //プロンプトMSG自動削除デフォルト値：10秒 (10000ミリ秒)
        private const int DefaultKickDeleteDelayMs = 10000;
        //自動削除有効/無効のデフォルト設定 (1=有効)
        private const uint DefaultKickShouldDelete = 1U;
        //対話中の応答メッセージタイムアウト設定(分)
        private const int DefaultKickTimeoutMinutes = 3;
        //---------- ユーザBANプラグイン用 ----------
        private const string BanSubKey = "Ban";
        //プロンプトMSG自動削除デフォルト値：10秒 (10000ミリ秒)
        private const int DefaultBanDeleteDelayMs = 10000;
        //自動削除有効/無効のデフォルト設定 (1=有効)
        private const uint DefaultBanShouldDelete = 1U;
        //対話中の応答メッセージタイムアウト設定(分)
        private const int DefaultBanTimeoutMinutes = 3;
        //---------- メッセージ削除プラグイン用 ----------
        private const string DeleteSubKey = "Delete";
        //プロンプトMSG自動削除デフォルト値：10秒 (10000ミリ秒)
        private const int DefaultDeleteDeleteDelayMs = 10000;
        //自動削除有効/無効のデフォルト設定 (1=有効)
        private const uint DefaultDeleteShouldDelete = 1U;
        //対話中の応答メッセージタイムアウト設定(分)
        private const int DefaultDeleteTimeoutMinutes = 3;
        //一括削除件数指定フラグ (0=無効/デフォルト100件, 1=有効/カスタム件数使用)
        private const uint DefaultDeleteUseCustomCount = 0U;
        //デフォルトの最大削除件数 (Discord APIの1回あたりの最大値)
        private const int DefaultBulkDeleteCount = 100;
        //---------- ロール付与プラグイン ----------
        private const string RoleSubKey = "Role";
        //ロールの自動付与永続化フラグ
        private const uint DefaultRoleIsPermanent = 0U;
        //Discordのデフォルトカラー(青)
        private const uint DefaultRoleEmbedColorValue = 0x3498DB;
        //プロンプトMSG自動削除デフォルト値：10秒 (10000ミリ秒)
        private const int DefaultRoleDeleteDelayMs = 10000;
        //自動削除有効/無効のデフォルト設定 (0=無効)
        private const uint DefaultRoleShouldDelete = 1U;
        //対話中の応答メッセージタイムアウト設定(分)
        private const int DefaultRoleTimeoutMinutes = 3;
        //---------- 共通設定 ----------
        //BOT本体の設定用(新しいサフィックス)
        private const string MainBotSubKey = "MainBot";
        //コマンド削除設定のデフォルト値 (true = 削除する)
        private const bool DefaultDeleteCommandMessage = false;
        //デバッグログ設定用
        private const string DebugLogSubKey = "DebugLogEnabled";
        //デバッグログ出力のデフォルト値 (false = 出力しない)
        private const bool DefaultDebugLogEnabled = false;
        //ログパフォーマンス設定のデフォルト値
        //一度にUIスレッドに渡すログの最大数
        private const int DefaultLogBatchSize = 25;
        //待機時間を制御(ms)
        private const int DefaultUITaskDelayMs = 50;
        //ログファイル出力フラグキー
        private const string DebugLogFileSubKey = "DebugLogFileEnabled";
        //ログファイル出力フラグ値
        private const bool DefaultDebugLogFileEnabled = false;
        //ログファイル出力サイズ値(100MB)
        private const int DebugLogFileSize = 100;
        //---------- Licenseファイル情報 ----------
        private const string LicenseRawSubKey = "LicenseRaw";
        private const string IsActivatedKey = "IsActivated";
        private const uint DefaultActivationFlag = 0U;
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "ILogger instance cannot be null.");
            _logger.Log($"[RegistryHelper] ILoggerの初期化が完了しました!!Base Path: HKEY_CURRENT_USER\\{BaseRegistryPath}", 4);
            _logger.Log($"[RegistryHelper] Base Path：HKEY_CURRENT_USER\\{BaseRegistryPath}", 4);
        }
        //設定が更新されたことを知らせるイベント
        public static event Action OnSettingsChanged;

        //設定フォームからこれを呼ぶ
        public static void NotifySettingsChanged() => OnSettingsChanged?.Invoke();
        //DiscordBot_Roleの設定
        //Roleコマンドの許可ロール設定Loadメソッド
        public static List<ulong> LoadRoleAllowedRoleIds(ulong guildId)
        {
            string json = GetStringSetting($"{BaseRegistryPath}\\{RoleSubKey}\\{guildId}", "RoleAllowedRoleIds", "[]");
            return JsonConvert.DeserializeObject<List<ulong>>(json);
        }
        //Roleコマンドの許可ロール設定Saveメソッド
        public static void SaveRoleAllowedRoleIds(ulong guildId, List<ulong> roleIds)
        {
            string json = JsonConvert.SerializeObject(roleIds);
            SetStringSetting($"{BaseRegistryPath}\\{RoleSubKey}\\{guildId}", "RoleAllowedRoleIds", json);
        }
        //Roleコマンドのロール自動付与永続化の有効/無効をレジストリに保存
        public static void SaveRoleIsPermanent(bool isPermanent)
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = isPermanent ? 1U : 0U;

            //RoleSubKey ("Role") を使用し、キー名 "PermanentEnabled" で保存
            SetDwordSetting(fullPath, "PermanentEnabled", value);
        }
        //Roleコマンドのロール自動付与永続化の有効/無効をレジストリから読み込み
        public static bool LoadRoleIsPermanent()
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //RoleSubKey ("Role") を使用し、キー名 "PermanentEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "PermanentEnabled", DefaultRoleIsPermanent);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //[ロール自動付与]ロール付与埋め込みメッセージの色を取得(数値：uint)
        public static Discord.Color LoadRoleEmbedColor()
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            uint colorValue = GetDwordSetting(fullPath, "EmbedColor", DefaultRoleEmbedColorValue);

            //Discord.ColorはDiscordBot.CoreがDiscordライブラリに依存している前提
            return new Discord.Color(colorValue);
        }
        //[ロール自動付与]ロール付与埋め込みメッセージの色を保存(数値：uint)
        public static void SaveRoleEmbedColor(uint colorValue)
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            SetDwordSetting(fullPath, "EmbedColor", colorValue);
        }
        //Roleコマンドの応答メッセージ自動削除の有効/無効をレジストリに保存
        public static void SaveRoleShouldDelete(bool shouldDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = shouldDelete ? 1U : 0U;

            //RoleSubKey ("Role") を使用し、キー名 "ShouldDeleteEnabled" で保存
            SetDwordSetting(fullPath, "ShouldDeleteEnabled", value);
        }
        //Roleコマンドの応答メッセージ自動削除の有効/無効をレジストリから読み込み
        public static bool LoadRoleShouldDelete()
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //RoleSubKey ("Role") を使用し、キー名 "ShouldDeleteEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "ShouldDeleteEnabled", DefaultRoleShouldDelete);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //Roleコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリに保存
        public static void SaveRoleDeleteDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //RoleSubKey を使用
            SetDwordSetting(fullPath, "DeleteDelayMs", (uint)delayMs);
        }
        //Roleコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリから読み込み
        public static int LoadRoleDeleteDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //RoleSubKey を使用
            uint value = GetDwordSetting(fullPath, "DeleteDelayMs", DefaultRoleDeleteDelayMs);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //Roleコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリに保存
        public static void SaveRoleTimeoutMinutes(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //DeleteSubKey を使用
            SetDwordSetting(fullPath, "TimeoutMinutes", (uint)delayMs);
        }
        //Roleコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリから読み込み
        public static int LoadRoleTimeoutMinutes()
        {
            string fullPath = $"{BaseRegistryPath}\\{RoleSubKey}";
            //DeleteSubKey を使用
            uint value = GetDwordSetting(fullPath, "TimeoutMinutes", DefaultRoleTimeoutMinutes);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //DiscordBot_Deleteの設定
        //Delコマンドの許可ロール設定Loadメソッド
        public static List<ulong> LoadDeleteAllowedRoleIds(ulong guildId)
        {
            string json = GetStringSetting($"{BaseRegistryPath}\\{DeleteSubKey}\\{guildId}", "DeleteAllowedRoleIds", "[]");
            return JsonConvert.DeserializeObject<List<ulong>>(json);
        }
        //Delコマンドの許可ロール設定Saveメソッド
        public static void SaveDeleteAllowedRoleIds(ulong guildId, List<ulong> roleIds)
        {
            string json = JsonConvert.SerializeObject(roleIds);
            SetStringSetting($"{BaseRegistryPath}\\{DeleteSubKey}\\{guildId}", "DeleteAllowedRoleIds", json);
        }
        //Delコマンドの応答メッセージ自動削除の有効/無効をレジストリに保存
        public static void SaveDeleteShouldDelete(bool shouldDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = shouldDelete ? 1U : 0U;

            //DeleteSubKey ("Delete") を使用し、キー名 "ShouldDeleteEnabled" で保存
            SetDwordSetting(fullPath, "ShouldDeleteEnabled", value);
        }

        //Delコマンドの応答メッセージ自動削除の有効/無効をレジストリから読み込み
        public static bool LoadDeleteShouldDelete()
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey ("Delete") を使用し、キー名 "ShouldDeleteEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "ShouldDeleteEnabled", DefaultDeleteShouldDelete);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //Delコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリに保存
        public static void SaveDeleteDeleteDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey を使用
            SetDwordSetting(fullPath, "DeleteDelayMs", (uint)delayMs);
        }
        //Delコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリから読み込み
        public static int LoadDeleteDeleteDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey を使用
            uint value = GetDwordSetting(fullPath, "DeleteDelayMs", DefaultDeleteDeleteDelayMs);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //Delコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリに保存
        public static void SaveDeleteTimeoutMinutes(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey を使用
            SetDwordSetting(fullPath, "TimeoutMinutes", (uint)delayMs);
        }
        //Delコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリから読み込み
        public static int LoadDeleteTimeoutMinutes()
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey を使用
            uint value = GetDwordSetting(fullPath, "TimeoutMinutes", DefaultDeleteTimeoutMinutes);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //一括削除件数のカスタム設定利用フラグを読み込み
        public static bool LoadDeleteUseCustomCount()
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey ("Delete") を使用し、キー名 "DeleteUseCustomCount" から読み込み
            uint value = GetDwordSetting(fullPath, "DeleteUseCustomCount", DefaultDeleteUseCustomCount);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //一括削除件数のカスタム設定利用フラグを保存
        public static void SaveDeleteUseCustomCount(bool DeleteUseFlag)
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = DeleteUseFlag ? 1U : 0U;

            //DeleteSubKey ("Delete") を使用し、キー名 "DeleteUseCustomCount" で保存
            SetDwordSetting(fullPath, "DeleteUseCustomCount", value);
        }
        //カスタム一括削除件数を読み込み
        public static int LoadBulkDeleteCount()
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey を使用
            uint value = GetDwordSetting(fullPath, "BulkDeleteCount", (uint)DefaultBulkDeleteCount);
            //2未満にならないよう、かつintの範囲内であることを保証
            return (int)Math.Max(Math.Min(value, int.MaxValue), 2);
        }
        //カスタム一括削除件数を保存
        public static void SaveBulkDeleteCount(int value)
        {
            string fullPath = $"{BaseRegistryPath}\\{DeleteSubKey}";
            //DeleteSubKey を使用
            //2未満の値を保存しないように調整
            SetDwordSetting(fullPath, "BulkDeleteCount", (uint)Math.Max(value, 2));
        }
        //----- DiscordBot_Kickの設定 -----
        //キックコマンドの実行を許可するロールIDのリストをレジストリから読み込み
        public static List<ulong> LoadKickAllowedRoleIds(ulong guildId)
        {
            string json = GetStringSetting($"{BaseRegistryPath}\\{KickSubKey}\\{guildId}", AllowedRolesKey, "[]");
            return JsonConvert.DeserializeObject<List<ulong>>(json);
        }
        //キックコマンドの実行を許可するロールIDのリストをレジストリに保存
        public static void SaveKickAllowedRoleIds(ulong guildId, List<ulong> roleIds)
        {
            string json = JsonConvert.SerializeObject(roleIds);
            SetStringSetting($"{BaseRegistryPath}\\{KickSubKey}\\{guildId}", AllowedRolesKey, json);
        }
        //キックコマンドの応答メッセージ自動削除の有効/無効をレジストリに保存
        public static void SaveKickShouldDelete(bool shouldDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{KickSubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = shouldDelete ? 1U : 0U;

            //KickSubKey ("Kick") を使用し、キー名 "ShouldDeleteEnabled" で保存
            SetDwordSetting(fullPath, "ShouldDeleteEnabled", value);
        }
        //キックコマンドの応答メッセージ自動削除の有効/無効をレジストリから読み込み
        public static bool LoadKickShouldDelete()
        {
            string fullPath = $"{BaseRegistryPath}\\{KickSubKey}";
            //KickSubKey ("Kick") を使用し、キー名 "ShouldDeleteEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "ShouldDeleteEnabled", DefaultKickShouldDelete);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //キックコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリに保存
        public static void SaveKickDeleteDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{KickSubKey}";
            //KickSubKey を使用
            SetDwordSetting(fullPath, "DeleteDelayMs", (uint)delayMs);
        }
        //キックコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリから読み込み
        public static int LoadKickDeleteDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{KickSubKey}";
            //KickSubKey を使用
            uint value = GetDwordSetting(fullPath, "DeleteDelayMs", DefaultKickDeleteDelayMs);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //キックコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリに保存
        public static void SaveKickTimeoutMinutes(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{KickSubKey}";
            //KickSubKey を使用
            SetDwordSetting(fullPath, "TimeoutMinutes", (uint)delayMs);
        }
        //キックコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリから読み込み
        public static int LoadKickTimeoutMinutes()
        {
            string fullPath = $"{BaseRegistryPath}\\{KickSubKey}";
            //KickSubKey を使用
            uint value = GetDwordSetting(fullPath, "TimeoutMinutes", DefaultKickTimeoutMinutes);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //----- DiscordBot_Banの設定 -----
        //バンコマンドの実行を許可するロールIDのリストをレジストリから読み込み
        public static List<ulong> LoadBanAllowedRoleIds(ulong guildId)
        {
            string json = GetStringSetting($"{BaseRegistryPath}\\{BanSubKey}\\{guildId}", AllowedRolesKey, "[]");
            return JsonConvert.DeserializeObject<List<ulong>>(json);
        }
        //バンコマンドの実行を許可するロールIDのリストをレジストリに保存
        public static void SaveBanAllowedRoleIds(ulong guildId, List<ulong> roleIds)
        {
            string json = JsonConvert.SerializeObject(roleIds);
            SetStringSetting($"{BaseRegistryPath}\\{BanSubKey}\\{guildId}", AllowedRolesKey, json);
        }
        //バンコマンドの応答メッセージ自動削除の有効/無効をレジストリから読み込み
        public static bool LoadBanShouldDelete()
        {
            string fullPath = $"{BaseRegistryPath}\\{BanSubKey}";
            //KickSubKey ("Kick") を使用し、キー名 "ShouldDeleteEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "ShouldDeleteEnabled", DefaultBanShouldDelete);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //バンコマンドの応答メッセージ自動削除の有効/無効をレジストリに保存
        public static void SaveBanShouldDelete(bool shouldDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{BanSubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = shouldDelete ? 1U : 0U;

            //BanSubKey ("Ban") を使用し、キー名 "ShouldDeleteEnabled" で保存
            SetDwordSetting(fullPath, "ShouldDeleteEnabled", value);
        }
        //バンコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリから読み込み
        public static int LoadBanDeleteDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{BanSubKey}";
            //KickSubKey を使用
            uint value = GetDwordSetting(fullPath, "DeleteDelayMs", DefaultBanDeleteDelayMs);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //バンコマンドの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリに保存
        public static void SaveBanDeleteDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{BanSubKey}";
            //BanSubKey を使用
            SetDwordSetting(fullPath, "DeleteDelayMs", (uint)delayMs);
        }
        //バンコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリから読み込み
        public static int LoadBanTimeoutMinutes()
        {
            string fullPath = $"{BaseRegistryPath}\\{BanSubKey}";
            //BanSubKey を使用
            uint value = GetDwordSetting(fullPath, "TimeoutMinutes", DefaultBanTimeoutMinutes);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //バンコマンドの応答メッセージ対話タイムアウト設定(分)をレジストリに保存
        public static void SaveBanTimeoutMinutes(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{BanSubKey}";
            //BanSubKey を使用
            SetDwordSetting(fullPath, "TimeoutMinutes", (uint)delayMs);
        }
        //----- License設定 -----
        //ライセンスのアクティベートフラグの読み込み
        public static bool GetLicenseActivationFlag()
        {
            try
            {
                uint flag = GetDwordSetting(BaseRegistryPath + "\\" + MainBotSubKey, IsActivatedKey, DefaultActivationFlag);
                return flag == 1U;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        //ライセンスのアクティベートフラグの保存
        public static void SaveLicenseActivationFlag(bool isActive)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}"; // 共通設定キーを使用
            SetDwordSetting(fullPath, IsActivatedKey, isActive ? 1U : 0U);
        }
        //暗号化されたライセンスファイルの内容をレジストリに保存する
        public static void SaveEncryptedLicense(string encryptedContent)
        {
            string fullPath = $"{BaseRegistryPath}\\{LicenseRawSubKey}";
            SetStringSetting(fullPath, "EncryptedContent", encryptedContent);
        }

        //暗号化されたライセンスファイルの内容をレジストリから読み込む
        public static string LoadEncryptedLicense()
        {
            try
            {
                string fullPath = $"{BaseRegistryPath}\\{LicenseRawSubKey}";
                return GetStringSetting(fullPath, "EncryptedContent", string.Empty);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        //[入退室ログ]チャンネルIDをレジストリから読み込む
        public static string ReadChannelId()
        {
            string fullPath = $"{BaseRegistryPath}\\{JoiningLeavingSubKey}";
            //既存のロジックをGetStringSettingで置き換え
            return GetStringSetting(fullPath, ChannelKey, string.Empty);
        }

        //[入退室ログ]チャンネルIDをレジストリに書き込む
        public static void WriteChannelId(string channelId)
        {
            string fullPath = $"{BaseRegistryPath}\\{JoiningLeavingSubKey}";
            //既存のロジックをSetStringSettingで置き換え
            SetStringSetting(fullPath, ChannelKey, channelId);
        }
        //[アンケートQA]アンケート埋め込みメッセージの色を取得(数値：uint)
        public static Discord.Color LoadQAEmbedColor()
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            uint colorValue = GetDwordSetting(fullPath, "EmbedColor", DefaultQAEmbedColorValue);

            //Discord.ColorはDiscordBot.CoreがDiscordライブラリに依存している前提
            return new Discord.Color(colorValue);
        }
        //[アンケートQA]アンケート埋め込みメッセージの色を保存(数値：uint)
        public static void SaveQAEmbedColor(uint colorValue)
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            SetDwordSetting(fullPath, "EmbedColor", colorValue);
        }
        //[アンケートQA]アンケートの自動終了時間(分)を取得(数値：int)
        public static int LoadQATimeoutMinutes()
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            return (int)GetDwordSetting(fullPath, "TimeoutMinutes", (uint)DefaultTimeoutMinutes);
        }

        //[アンケートQA]アンケートの自動終了時間(分)を保存(数値：int)
        public static void SaveQATimeoutMinutes(int minutes)
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            SetDwordSetting(fullPath, "TimeoutMinutes", (uint)minutes);
        }
        //アンケートQAの応答メッセージ自動削除の有効/無効をレジストリに保存
        public static void SaveQAShouldDelete(bool shouldDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            //bool値を 1 または 0 の uint に変換
            uint value = shouldDelete ? 1U : 0U;

            //QASubKey ("QA") を使用し、キー名 "ShouldDeleteEnabled" で保存
            SetDwordSetting(fullPath, "ShouldDeleteEnabled", value);
        }

        //アンケートQAの応答メッセージ自動削除の有効/無効をレジストリから読み込み
        public static bool LoadQAShouldDelete()
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            //QASubKey ("QA") を使用し、キー名 "ShouldDeleteEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "ShouldDeleteEnabled", DefaultQAShouldDelete);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //アンケートQAの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリに保存
        public static void SaveQADeleteDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            //QASubKey を使用
            SetDwordSetting(fullPath, "DeleteDelayMs", (uint)delayMs);
        }
        //アンケートQAの応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリから読み込み
        public static int LoadQADeleteDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            // QASubKey を使用
            uint value = GetDwordSetting(fullPath, "DeleteDelayMs", DefaultQADeleteDelayMs);

            // uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //アンケートQAの複数投票の許可設定 (0=不許可, 1=許可) を取得
        public static bool LoadAllowMultipleVotesSetting()
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            uint value = GetDwordSetting(fullPath, "AllowMultipleVotes", DefaultAllowMultipleVotes);
            //値が 1 なら true (許可)
            return value == 1U;
        }

        //アンケートQAの1人1票制限有効の許可設定を保存
        public static void SaveAllowMultipleVotesSetting(bool allowMultiple)
        {
            string fullPath = $"{BaseRegistryPath}\\{QASubKey}";
            //true (許可) なら 1U, false (不許可) なら 0U を保存
            SetDwordSetting(fullPath, "AllowMultipleVotes", allowMultiple ? 1U : 0U);
        }
        //[共通設定]コマンドメッセージを自動で削除するかどうかを取得
        public static bool LoadDeleteCommandMessageSetting()
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            // boolean を DWORD (0または1) で保存するため、uintとして取得
            uint value = GetDwordSetting(fullPath, "DeleteCommandMessage", DefaultDeleteCommandMessage ? 1U : 0U);
            return value == 1U;
        }

        //[共通設定]コマンドメッセージの自動削除設定を保存
        public static void SaveDeleteCommandMessageSetting(bool doDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            // boolean を DWORD (0または1) で保存
            SetDwordSetting(fullPath, "DeleteCommandMessage", doDelete ? 1U : 0U);
        }
        //[共通設定]デバッグログが有効かどうかを取得
        public static bool LoadDebugLogEnabledSetting()
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            // boolean を DWORD (0または1) で保存するため、uintとして取得
            uint value = GetDwordSetting(fullPath, DebugLogSubKey, DefaultDebugLogEnabled ? 1U : 0U);
            return value == 1U;
        }

        //[共通設定]デバッグログが有効かどうかを保存
        public static void SaveDebugLogEnabledSetting(bool isEnabled)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            // boolean を DWORD (0または1) で保存
            SetDwordSetting(fullPath, DebugLogSubKey, isEnabled ? 1U : 0U);
        }
        //ログのバッチサイズをレジストリから読み込み
        public static int LoadLogBatchSize()
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            uint value = GetDwordSetting(fullPath, "LogBatchSize", DefaultLogBatchSize);

            //最小値を保証 (例えば 1)
            return (int)Math.Max(value, 1);
        }
        //ログのバッチサイズをレジストリに保存
        public static void SaveLogBatchSize(int size)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            //最小値を保証 (1未満は保存しない)
            SetDwordSetting(fullPath, "LogBatchSize", (uint)Math.Max(size, 1));
        }
        //UIスレッドへのログ処理依頼後の待機時間(ミリ秒)をレジストリから読み込み
        public static int LoadUITaskDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            uint value = GetDwordSetting(fullPath, "UITaskDelayMs", (uint)DefaultUITaskDelayMs);

            //最小値を保証 (例えば 10ms)
            return (int)Math.Max(value, 1);
        }
        //UIスレッドへのログ処理依頼後の待機時間(ミリ秒)をレジストリに保存
        public static void SaveUITaskDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            //最小値を保証 (1ms未満は保存しない)
            SetDwordSetting(fullPath, "UITaskDelayMs", (uint)Math.Max(delayMs, 1));
        }
        //[共通設定]デバッグログをファイル出力するかどうかを取得
        public static bool LoadDebugLogFileEnabled()
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            // boolean を DWORD (0または1) で保存するため、uintとして取得
            uint value = GetDwordSetting(fullPath, DebugLogFileSubKey, DefaultDebugLogFileEnabled ? 1U : 0U);
            return value == 1U;
        }

        //[共通設定]デバッグログをファイル出力するかどうかを保存
        public static void SaveDebugLogFileEnabled(bool isEnabled)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            // boolean を DWORD (0または1) で保存
            SetDwordSetting(fullPath, DebugLogFileSubKey, isEnabled ? 1U : 0U);
        }
        //デバッグログのファイル出力最大書き込みサイズをレジストリから読み込み
        public static int LoadDebugLogFileSize()
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            uint value = GetDwordSetting(fullPath, "DebugLogFileSize", (uint)DebugLogFileSize);

            //最小値を保証 (例えば 10ms)
            return (int)Math.Max(value, 1);
        }
        //デバッグログのファイル出力最大書き込みサイズをレジストリに保存
        public static void SaveDebugLogFileSize(int maxSizeMb)
        {
            string fullPath = $"{BaseRegistryPath}\\{MainBotSubKey}";
            //最小値を保証 (1ms未満は保存しない)
            SetDwordSetting(fullPath, "DebugLogFileSize", (uint)Math.Max(maxSizeMb, 1));
        }
        //--- 天気予報プラグイン設定 ---
        //[天気予報]天気予報のデータキャッシュ時間(分)を取得
        public static int LoadWeatherCacheDurationSetting()
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            //キャッシュ時間は分単位なので、uintとして取得
            uint value = GetDwordSetting(fullPath, "CacheDurationMinutes", (uint)DefaultWeatherCacheDurationMinutes);
            //0以下の値にならないように保護
            return (int)Math.Max(0, value);
        }
        //[天気予報]天気予報のデータキャッシュ時間(分)を保存
        public static void SaveWeatherCacheDurationSetting(int durationMinutes)
        {
            //最小値を0分（キャッシュなし）としたい場合はここでチェック
            if (durationMinutes < 0)
            {
                durationMinutes = 0;
            }
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            //int を DWORD で保存 (負の値は0として保存)
            SetDwordSetting(fullPath, "CacheDurationMinutes", (uint)Math.Max(0, durationMinutes));
        }
        //[天気予報]天気予報の応答メッセージ自動削除の有効/無効をレジストリに保存
        public static void SaveWeatherShouldDelete(bool shouldDelete)
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";

            //bool値を 1 または 0 の uint に変換
            uint value = shouldDelete ? 1U : 0U;
            //WeatherSubKey ("Weather") を使用し、キー名 "ShouldDeleteEnabled" で保存
            SetDwordSetting(fullPath, "ShouldDeleteEnabled", value);
        }
        //[天気予報]天気予報の応答メッセージ自動削除の有効/無効をレジストリから読み込み
        public static bool LoadWeatherShouldDelete()
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            //WeatherSubKey ("Weather") を使用し、キー名 "ShouldDeleteEnabled" から読み込み
            uint value = GetDwordSetting(fullPath, "ShouldDeleteEnabled", DefaultWeatherShouldDelete);

            //読み込んだ値が 1 であれば true、それ以外（主に 0）であれば false を返す
            return value == 1U;
        }
        //[天気予報]天気予報の応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリに保存
        public static void SaveWeatherDeleteDelayMs(int delayMs)
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            //WeatherSubKey を使用
            SetDwordSetting(fullPath, "DeleteDelayMs", (uint)delayMs);
        }
        //[天気予報]天気予報の応答メッセージ自動削除の遅延時間(ミリ秒)をレジストリから読み込み
        public static int LoadWeatherDeleteDelayMs()
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            //WeatherSubKey を使用
            uint value = GetDwordSetting(fullPath, "DeleteDelayMs", DefaultWeatherDeleteDelayMs);

            //uintからintに変換して返す (負の値は想定しない)
            return (int)value;
        }
        //OpenWeatherMap APIキーをレジストリから取得
        public static string LoadWeatherApiKeySetting()
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            // 文字列としてAPIキーを読み込む
            return GetStringSetting(fullPath, WeatherApiKeyKey, DefaultWeatherApiKey);
        }
        //OpenWeatherMap APIキーをレジストリに保存
        public static void SaveWeatherApiKeySetting(string apiKey)
        {
            string fullPath = $"{BaseRegistryPath}\\{WeatherSubKey}";
            // 文字列としてAPIキーを保存
            SetStringSetting(fullPath, WeatherApiKeyKey, apiKey);
        }
        //汎用レジストリ操作のコア実装(DWORD/UINT)
        private static uint GetDwordSetting(string subKeyPath, string key, uint defaultValue)
        {
            //Registry.GetValue は HKEY_CURRENT_USER を自動で補完
            object value = Registry.GetValue($"HKEY_CURRENT_USER\\{subKeyPath}", key, defaultValue);

            if (value is int intValue)
            {
                return (uint)intValue;
            }
            return defaultValue;
        }
        private static void SetDwordSetting(string subKeyPath, string key, uint value)
        {
            try
            {
                //Registry.SetValue はキーが存在しない場合、自動で作成
                Registry.SetValue($"HKEY_CURRENT_USER\\{subKeyPath}", key, (int)value, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registry Write Error (DWORD)：{ex.Message}");
            }
        }
        //汎用レジストリ操作のコア実装(String)
        private static string GetStringSetting(string subKeyPath, string key, string defaultValue)
        {
            object value = Registry.GetValue($"HKEY_CURRENT_USER\\{subKeyPath}", key, defaultValue);

            return value?.ToString() ?? defaultValue;
        }

        private static void SetStringSetting(string subKeyPath, string key, string value)
        {
            try
            {
                //Registry.SetValue はキーが存在しない場合、自動で作成
                Registry.SetValue($"HKEY_CURRENT_USER\\{subKeyPath}", key, value, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                //既存のコードに合わせてエラーログを出力
                Console.WriteLine($"Registry Write Error (STRING)：{ex.Message}");
            }
        }
        // ---------- 実行時データ共有用（メモリ保持） ----------
        // メッセージIDとプラグイン独自のデータを紐付けるための辞書
        private static readonly Dictionary<ulong, object> _runtimeCustomData
            = new Dictionary<ulong, object>();

        //メッセージIDに対して任意のカスタムデータを一時的に保持(メモリ上)
        public static void RegisterRuntimeData(ulong messageId, object data)
        {
            _runtimeCustomData[messageId] = data;
            OnDataRegistered?.Invoke(messageId, data);
        }
    }
}
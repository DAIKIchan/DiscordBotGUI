using Discord;
using Discord.WebSocket;
//クラスライブラリ
using DiscordBot.Core;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class MainForm : Form
    {
        //各フォームの初期化
        Setting setting = null;
        Edit edit = null;
        JoiningLeavingSetting joiningleaving = null;
        QASetting qasetting = null;
        WeatherSetting weathersetting = null;
        KickSetting kicksetting = null;
        DeleteSetting deletesetting = null;
        BanSetting bansetting = null;
        RoleSetting rolesetting = null;
        LicenseInfoSetting licenseinfosetting = null;
        VersionInfo versioninfo = null;
        //最終チェック日を保持するフィールドversioninfo
        private DateTime lastCheckDate = DateTime.MinValue;
        //タイマー経由の場合一度だけ警告表示するフラグ
        private bool licenseExpiredWarningShown = false;
        //フォームがアクティブになったときのフラグ
        private bool isActivated = false;
        //ライセンス認証状態フラグ
        private bool _isLicenseActive = false;
        private DiscordSocketClient _client;
        private bool IsBotRunning = false;
        private bool _isClosingProgrammatically = false;
        private bool _isReloading = false;
        //レジストリから読み込む為のデバッグログ設定フラグ
        private bool _isDebugLogEnabled = false;
        //ログファイル出力フラグ
        private bool _isLogFileEnabled = false;
        //設定ファイルでファイルパスまで入っている為
        private string ConfigDirectory
        {
            get
            {
                //1．保存先フォルダのパスを定義
                string folderPath = @"C:\DiscordBotGUI";

                //2．フォルダが存在しなければ作成する
                if (!Directory.Exists(folderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"フォルダ作成失敗!!\n例外：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                //3．フォルダパスとファイル名を結合して返す
                return Path.Combine(folderPath, "Commands.json");
            }
        }
        private Dictionary<string, CommandSetting> _commands;
        // キー: DiscordメッセージID (ulong), 値: CancellationTokenSource
        private readonly Dictionary<ulong, CancellationTokenSource> _qaPollTimeouts = new Dictionary<ulong, CancellationTokenSource>();
        //DLLと紐づいたコントロールの辞書
        private Dictionary<string, object> _pluginConfigControls;
        //DLLプラグインコマンドのリスト
        private List<ICommandHandler> _commandHandlers;
        //DLL単発コマンド ICommand 用のフィールド
        private List<ICommand> _dllCommands;
        //対話の状態管理用辞書(ユーザーID -> 現在対話中のDLLインスタンス)
        private Dictionary<ulong, ICommandHandler> _activeInteractions = new Dictionary<ulong, ICommandHandler>();
        private List<IPluginEventHandler> _pluginEventHandlers = new List<IPluginEventHandler>();
        private List<ICommandProvider> _commandProviders = new List<ICommandProvider>();
        private readonly Dictionary<ulong, System.Timers.Timer> _commandTimers = new Dictionary<ulong, System.Timers.Timer>();
        // 非同期ログ処理のためのフィールド
        private readonly ConcurrentQueue<(string Message, LogType Type)> _logQueue = new ConcurrentQueue<(string, LogType)>();
        private CancellationTokenSource _logCts;
        private Task _logProcessorTask;
        //ログ性能設定を保持するインスタンス変数
        private int _logBatchSize;
        private int _uiTaskDelayMs;
        //製品情報
        private const string PGUniqueVersion = "BOT-000001";
        string PGVersion = "[Ver1.2.6.36]";
        private readonly ILogger _mainForm;
        private readonly ILogger _appForm;
        //DLLがアクセスできるインターフェース
        private class LoggerAdapter : ILogger
        {
            private readonly MainForm _mainForm;

            //コンストラクタで MainForm インスタンスを受け取る
            public LoggerAdapter(MainForm form) => _mainForm = form;

            //ILogger インターフェースの実装
            public void Log(string message, int typeValue)
            {
                //受け取った int(DLL側の PluginLogType の値)を MainForm の LogType にキャストして使用する
                var logType = (MainForm.LogType)typeValue;
                _mainForm.Log(message, logType);
            }
        }
        public class LicenseFileRoot
        {
            //JSONのキーに合わせて "LicenseInfo" に変更
            //念のため、Json.NETの属性で明示的に指定
            [JsonProperty("LicenseInfo")]
            public List<LicenseInfo> LicenseInfo { get; set; }
        }

        public class LicenseInfo
        {
            [JsonProperty("DateTime")]
            //期限チェックに使用
            public string ExpiryDateTime { get; set; }

            public string LicenseKey { get; set; }
            public string HardwareId { get; set; }
            public string AppId { get; set; }
            //使用しないため省略可
            public string LicenseType { get; set; }
            public List<string> AllowedPlugins { get; set; }
        }

        //復号化ヘルパーメソッド
        public static class EncryptionHelper
        {
            private static readonly string encryptionKeyBase = "DiscordBotGUI-1234567890123456";

            private static byte[] GetEncryptionKeyBytes()
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    return sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKeyBase));
                }
            }

            public static string Decrypt(string cipherText)
            {
                byte[] key = GetEncryptionKeyBytes();
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.Mode = CipherMode.CBC;
                    // IVはLicenseInfoSettingのSaveEncryptedLicense時と同じ0バイト配列を想定
                    aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        public MainForm()
        {
            _mainForm = new AnonymousLogger(
                (message, typeValue) =>
                {
                    // ここに MainForm の UI を更新する処理を記述します
                    // 例: this.listBoxLog.Items.Add($"[{((LogType)typeValue).ToString()}] {message}");

                    // (デバッグ用) 実際にはUIスレッドで実行する必要があります
                    //Console.WriteLine($"[{this.Text}] [{(LogType)typeValue}] {message}");
                }
            );
            InitializeComponent();
            //フォームロード時にログ処理タスクを開始
            this.Load += (sender, e) => StartLogProcessor();
            //フォームクローズ時にログ処理タスクを終了
            this.FormClosing += (sender, e) => StopLogProcessor();
            //フォーム起動時に最終チェック日を今日に設定
            lastCheckDate = DateTime.Today;
            //有効期限チェックとUI制御を最優先で実行
            UpdateMainFormControls(false);
            UpdateReloadButtonsState(IsBotRunning);
            //依存 DLL の解決イベントハンドラを登録 (必須)
            AppDomain.CurrentDomain.AssemblyResolve += ResolvePluginDependency;
            //RegistryHelper の初期化
            RegistryHelper.Initialize(_mainForm);
            //Debugログ設定をレジストリから読み込み
            _isDebugLogEnabled = RegistryHelper.LoadDebugLogEnabledSetting();
            BtnStop.Enabled = false;
            //UpdateReloadButtonsState(IsBotRunning);
            //初期化：DLLファイル名と、対応するボタンコントロールを設定
            _pluginConfigControls = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                //アンケートQA
                {"DiscordBot.Plugin.QA.dll", QAToolStripMenuItem},
                //メッセージ一括削除
                {"DiscordBot.Plugin.Delete.dll", MessageBulkDeletionToolStripMenuItem},
                //ユーザBAN
                {"DiscordBot.Plugin.Ban.dll", UserBanToolStripMenuItem},
                //ユーザKICK
                {"DiscordBot.Plugin.Kick.dll", UserKickToolStripMenuItem},
                //ロール自動付与
                {"DiscordBot.Plugin.Role.dll", RoleSettingToolStripMenuItem},
                //天気予報
                {"DiscordBot.Plugin.Weather.dll", WeatherInfoToolStripMenuItem},
                //入退出ログ
                {"DiscordBot.Plugin.UserJoining_Leaving.dll", JoiningLeavingToolStripMenuItem},
            };
        }
        //設定値を更新するパブリックメソッド
        public void UpdateLogPerformanceSettings(int newBatchSize, int newDelayMs)
        {
            // 新しい値をインスタンス変数に反映
            _logBatchSize = newBatchSize;
            _uiTaskDelayMs = newDelayMs;

            // ログに反映されたことを出力
            Log($"[INFO] ログパフォーマンス設定を更新しました!!", LogType.Debug);
            Log($"[INFO] 新しいバッチサイズ：[{newBatchSize}]", LogType.Debug);
            Log($"[INFO] 新しいUI待機時間：[{newDelayMs} ms]", LogType.Debug);

            // 注意: ProcessLogQueue メソッド内でこれらの変数が参照されていれば、
            // 次のループから新しい値で動作します。
        }
        private void StartLogProcessor()
        {
            //タスク開始前に設定値を読み込む
            //一度にUIスレッドに渡すログの最大数
            _logBatchSize = DiscordBot.Core.RegistryHelper.LoadLogBatchSize();
            //待機時間を制御し、CPU負荷を軽減
            _uiTaskDelayMs = DiscordBot.Core.RegistryHelper.LoadUITaskDelayMs();
            _logCts = new CancellationTokenSource();
            _logProcessorTask = ProcessLogQueue(_logCts.Token);
            Log($"ログパフォーマンスStart：{_logProcessorTask}", LogType.Debug);
        }

        private void StopLogProcessor()
        {
            if (_logCts != null)
            {
                //タスクのキャンセルを要求
                _logCts.Cancel();
                _logCts.Dispose();
                _logCts = null;
                _logProcessorTask = null;
                Log($"ログパフォーマンスStop：null", LogType.Debug);
            }
        }
        //1分ごとに実行され、日付が変わった場合にライセンスを再検証
        private void licenseCheckTimer_Tick(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Today;


            //最終チェック日と現在の日付が異なるとき(日付が変わったとき)に実行
            if (currentDate > lastCheckDate)
            {
                //日付が変わった
                Log("[ライセンス] 日付変更を検知しました!!ライセンス有効期限の再検証を実行します!!", LogType.Debug);

                //1．UIを更新し、期限切れならメッセージボックスを表示、コントロールを無効化
                UpdateMainFormControls(true);

                // 2. 最終チェック日を更新
                lastCheckDate = currentDate;
            }
        }
        //ライセンスの有効期限をチェックし、期限切れかどうかを判定
        private bool CheckLicenseExpiry()
        {
            //ライセンス認証されていない場合は、有効期限切れとは判定しない(未認証で制御する)
            if (!DiscordBot.Core.RegistryHelper.GetLicenseActivationFlag())
            {
                return false;
            }

            try
            {
                //許可プラグインリストを取得するロジックを流用し、ライセンス情報を取得
                string encryptedContent = DiscordBot.Core.RegistryHelper.LoadEncryptedLicense();
                string decryptedJson = EncryptionHelper.Decrypt(encryptedContent);
                var licenseRoot = JsonConvert.DeserializeObject<LicenseFileRoot>(decryptedJson);

                //ライセンス情報が存在することを確認
                if (licenseRoot != null && licenseRoot.LicenseInfo != null && licenseRoot.LicenseInfo.Count > 0)
                {
                    var licenseInfo = licenseRoot.LicenseInfo[0];

                    //JSONの "DateTime" プロパティを DateTime型 にパース
                    if (DateTime.TryParse(licenseInfo.ExpiryDateTime, out DateTime expiryDate))
                    {
                        //有効期限と現在時刻を比較(日付のみの比較、または期限切れ直後からの判定)
                        //期限の日付の「翌日」になったら期限切れと判定
                        if (expiryDate.Date < DateTime.Now.Date)
                        {
                            Log($"[ライセンス] 有効期限が [{expiryDate.ToShortDateString()}] に切れています!!", LogType.Error);
                            //期限切れ
                            return true;
                        }
                        else
                        {
                            Log($"[ライセンス] 有効期限は [{expiryDate.ToShortDateString()}] まで有効です!!", LogType.Success);
                            //有効
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //復号化、JSONパースなどに失敗した場合(データ破損時)
                Log($"[ライセンス] ライセンスファイルが不正です!!\n{ex.Message}", LogType.DebugError);
                //データが読めない場合は、安全のため期限切れと同じ扱いにする
                return true;
            }
            //ライセンス認証済みだが、有効期限データがない、またはパースできない場合はエラーとして扱う
            return true;
        }
        //ライセンス認証および有効期限に基づいてメインフォームのUIコントロールを更新
        public void UpdateMainFormControls(bool isCalledByTimer = false)
        {
            try
            {
                //1．ライセンス認証状態を取得
                bool isLicenseActivated = DiscordBot.Core.RegistryHelper.GetLicenseActivationFlag();

                //2．有効期限切れをチェック
                bool isExpired = false;
                if (isLicenseActivated)
                {
                    //認証されている場合のみ、有効期限をチェック
                    isExpired = CheckLicenseExpiry();
                }

                //3．実行可能フラグを決定
                //isRunnable = 認証済み AND 期限切れではない
                bool isRunnable = isLicenseActivated && !isExpired;
                //実行可能フラグに基づくログ(デバッグ用)
                Log($"[ライセンス] ライセンス状態：認証済み＝[{isLicenseActivated}], 期限切れ＝[{isExpired}], 実行可能＝[{isRunnable}]", LogType.Debug);

                //4．UIコントロールの無効化/有効化
                //SettingToolStripMenuItem, EditToolStripMenuItem, TxtToken, BtnStart, BtnReloadAllPlugins, BtnReleaseAllPlugins, BtnReloadAllCommand
                //if (SettingToolStripMenuItem != null) SettingToolStripMenuItem.Enabled = isRunnable;
                if (EditToolStripMenuItem != null) EditToolStripMenuItem.Enabled = isRunnable;
                if (PluginToolStripMenuItem != null) PluginToolStripMenuItem.Enabled = isRunnable;
                if (TxtToken != null) TxtToken.Enabled = isRunnable;
                if (BtnStart != null) BtnStart.Enabled = isRunnable;
                if (BtnReloadAllPlugins != null) BtnReloadAllPlugins.Enabled = isRunnable;
                if (BtnReleaseAllPlugins != null) BtnReleaseAllPlugins.Enabled = isRunnable;
                if (BtnReloadAllCommand != null) BtnReloadAllCommand.Enabled = isRunnable;
                if (IsBotRunning)
                {
                    if (isRunnable)
                    {
                        if (BtnStart != null) BtnStart.Enabled = false;
                        if (BtnStop != null) BtnStop.Enabled = isRunnable;
                    }
                    else
                    {
                        if (BtnStart != null) BtnStart.Enabled = isRunnable;
                        if (BtnStop != null) BtnStop.Enabled = isRunnable;
                    }
                }
                else
                {
                    if (isRunnable)
                    {
                        if (BtnStart != null) BtnStart.Enabled = isRunnable;
                        if (BtnStop != null) BtnStop.Enabled = false;
                    }
                    else
                    {
                        if (BtnStart != null) BtnStart.Enabled = isRunnable;
                        if (BtnStop != null) BtnStop.Enabled = isRunnable;
                    }
                }
                //5．状態に基づくメッセージの表示
                if (isRunnable)
                {
                    //認証済みかつ期限内の場合
                    //認証成功メッセージを出す(メインフォームに戻った際のフィードバック)
                    //MessageBox.Show("ライセンスは有効です!!全ての機能が利用可能です!!", "認証成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //期限が有効に戻ったため、警告フラグをリセット
                    licenseExpiredWarningShown = false;
                    //Version情報フォーム用ライセンスフラグ
                    this._isLicenseActive = true;
                    Log("ライセンスは有効です!!未認証プラグインDLLを除く全ての機能が利用可能です!!", LogType.Success);
                }
                else if (!isLicenseActivated)
                {
                    //Version情報フォーム用ライセンスフラグ
                    this._isLicenseActive = false;
                    //未認証の場合
                    MessageBox.Show("ライセンス認証がされていません!!BOTの起動や設定の変更など、機能の一部が制限されます!!", "未認証", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //BOTが起動中なら停止する
                    if (IsBotRunning && BtnStop != null)
                    {
                        //BtnStop_Click イベントハンドラを直接呼び出す
                        BtnStop_Click(BtnStop, EventArgs.Empty);
                        Log("🔴 ライセンスが認証されていないため、BOTを自動停止しました!!", LogType.Error);
                    }
                    else
                    {
                        Log("🔴 ライセンスが認証されていません!!", LogType.Error);
                        Log($"[ライセンス] ライセンス状態：認証済み＝[Error], 期限切れ＝[Error], 実行可能＝[Error]", LogType.Error);
                    }
                }
                else if (isExpired)
                {
                    //認証済みだが期限切れの場合
                    //(1) タイマー経由ではない場合 (起動時や手動操作時) => 表示する(毎回)
                    //(2) タイマー経由の場合、かつ警告がまだ表示されていない場合 => 表示する(一度のみ)
                    bool shouldShowWarning = !isCalledByTimer || !licenseExpiredWarningShown;

                    if (shouldShowWarning)
                    {
                        //Version情報フォーム用ライセンスフラグ
                        this._isLicenseActive = false;
                        //タイマー経由で表示した場合にのみ、フラグを立てる
                        if (isCalledByTimer)
                        {
                            licenseExpiredWarningShown = true;
                            //BOTが起動中なら停止する
                            if (IsBotRunning && BtnStop != null)
                            {
                                //BtnStop_Click イベントハンドラを直接呼び出す
                                BtnStop_Click(BtnStop, EventArgs.Empty);
                                Log("🔴 ライセンス有効期限切れのため、BOTを自動停止しました!!", LogType.Error);
                            }
                        }
                        MessageBox.Show("ライセンスの有効期限が切れました!!\n機能の一部が制限されます!!\n再度ライセンスの購入または更新してください!!", "有効期限切れ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                Log("※" + ErrorMessage, LogType.Error);
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ライセンス情報をデコードし、許可されたプラグインリストを取得する
        private List<string> GetAllowedPlugins()
        {
            Log("[ライセンスチェック] 許可プラグインリストの取得を開始します!!", LogType.Debug);

            try
            {
                string encryptedContent = DiscordBot.Core.RegistryHelper.LoadEncryptedLicense();
                if (string.IsNullOrEmpty(encryptedContent))
                {
                    Log("[ライセンスチェック] 暗号化されたライセンスデータが存在しません!!", LogType.DebugError);
                    return new List<string>();
                }

                string decryptedJson = EncryptionHelper.Decrypt(encryptedContent);
                var licenseRoot = JsonConvert.DeserializeObject<LicenseFileRoot>(decryptedJson);

                //licenseRoot.LicenseInfo にアクセス
                if (licenseRoot != null && licenseRoot.LicenseInfo != null && licenseRoot.LicenseInfo.Count > 0)
                {
                    //licenseRoot.LicenseInfo[0] にアクセス
                    var licenseInfo = licenseRoot.LicenseInfo[0];

                    List<string> allowedPlugins = licenseInfo.AllowedPlugins;

                    if (allowedPlugins != null && allowedPlugins.Count > 0)
                    {
                        Log($"[ライセンスチェック] 復号化されたプラグイン数：[{allowedPlugins.Count} 個]", LogType.Debug);
                        return allowedPlugins;
                    }
                    else
                    {
                        Log("[ライセンスチェック] ライセンスファイル内のリストが空またはnullです!!", LogType.DebugError);
                        return new List<string>();
                    }
                }
                else
                {
                    //LicenseInfo リストがデシリアライズできなかった場合のログ
                    Log("[ライセンスチェック] ライセンス情報リストが抽出できませんでした!!", LogType.DebugError);
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                Log($"[ライセンスチェック] ファイル解析エラー!!\n{ex.Message}", LogType.DebugError);
                return new List<string>();
            }
        }
        //これは、ILoggerの実装をその場で行うための最小限のクラス
        private class AnonymousLogger : ILogger
        {
            private readonly Action<string, int> _logAction;

            public AnonymousLogger(Action<string, int> logAction)
            {
                _logAction = logAction;
            }

            public void Log(string message, int typeValue)
            {
                _logAction(message, typeValue);
            }
        }
        //登録コマンドリロード
        private void BtnReloadAllCommand_Click(object sender, EventArgs e)
        {
            //設定ファイル読み込み
            try
            {
                if (!File.Exists(ConfigDirectory))
                {
                    Log($"❌：Commands.json が見つかりません!!", LogType.Error);
                    _commands = new Dictionary<string, CommandSetting>();
                }
                else
                {
                    string json = File.ReadAllText(ConfigDirectory);
                    var rootConfig = JsonConvert.DeserializeObject<CommandSettings>(json);

                    if (rootConfig == null || rootConfig.Commands == null)
                    {
                        Log("❌：Commands.json の内容が不正です!! ※デシリアライズ失敗!!", LogType.Error);
                        _commands = new Dictionary<string, CommandSetting>();
                        return;
                    }
                    //Dictionary に変換
                    _commands = new Dictionary<string, CommandSetting>();
                    foreach (var cmd in rootConfig.Commands)
                    {
                        _commands[cmd.CommandName] = cmd;
                    }
                    Log($"〇：[{_commands.Count} 個]のコマンドを再読み込みしました!!", LogType.Success);
                }
            }
            catch (Exception ex)
            {
                Log("JSON読み込みエラー：" + ex.Message, LogType.Error);
                _commands = new Dictionary<string, CommandSetting>();
            }
        }
        //タイマー管理ロジックの追加
        private void StartCommandTimer(ulong messageId, ulong channelId, int minutes, ICommandHandler handler, ulong userId)
        {
            var cts = new CancellationTokenSource();

            //既存のタイマー管理辞書 (_qaPollTimeouts または _commandTimeouts) を使用
            if (_qaPollTimeouts.ContainsKey(messageId))
            {
                _qaPollTimeouts[messageId].Cancel();
                _qaPollTimeouts.Remove(messageId);
            }
            _qaPollTimeouts.Add(messageId, cts);

            Task.Run(async () =>
            {
                try
                {
                    //指定時間待機
                    await Task.Delay(TimeSpan.FromMinutes(minutes), cts.Token);

                    //タイムアウト発生時の処理へ
                    await OnCommandTimeout(messageId, minutes, channelId, handler, userId);
                }
                catch (TaskCanceledException)
                {
                    Log($"[Timer] タイマーキャンセル：[{messageId}]", LogType.Debug);
                }
                catch (Exception ex)
                {
                    Log($"[Timer] タイマーエラー!!\n{ex.Message}", LogType.DebugError);
                }
                finally
                {
                    if (_qaPollTimeouts.ContainsKey(messageId))
                    {
                        _qaPollTimeouts.Remove(messageId);
                        cts.Dispose();
                    }
                }
            });
        }
        //時間切れ時に実行されるタイムアウト処理ロジック
        private async Task OnCommandTimeout(ulong messageId, int timeoutMinutes, ulong channelId, ICommandHandler handler, ulong userId)
        {
            try
            {
                //デフォルトはタイマーから渡されたID
                ulong messageIdToDelete = messageId;
                switch (handler.CommandName.ToLower())
                {
                    //アンケートQA
                    case "qa":
                        
                        await OnQATimeoutLogic(messageId, channelId);
                        break;
                    //Kickコマンド
                    case "kick":
                        //セッションが既に削除されている場合は、通知処理をスキップ
                        if (!_activeInteractions.ContainsKey(userId))
                        {
                            return;
                        }
                        //ICommandHandlerとしてアクセス
                        ICommandHandler kickHandler = handler;

                        //プラグインの新しいプロパティを優先して削除対象IDとする
                        if (kickHandler != null && kickHandler.FinalTimeoutMessageId != 0)
                        {
                            // プラグインが保持しているIDを最優先で使用 (これが最終確認メッセージのIDであるはず)
                            messageIdToDelete = kickHandler.FinalTimeoutMessageId;
                        }
                        //1．プロンプトメッセージを削除
                        if (_client.GetChannel(channelId) is ITextChannel kickChannel)
                        {
                            bool deletionSucceeded = false;
                            try
                            {
                                //メッセージオブジェクトを取得せず、IDで直接削除を実行
                                await kickChannel.DeleteMessageAsync(messageIdToDelete);
                                Log($"[Kick Timer] タイムアウトによりプロンプトメッセージ ID：[{messageId}] を削除しました!!", LogType.Debug);
                                deletionSucceeded = true;
                            }
                            catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                            {
                                Log($"[Kick Timer] タイムアウト時のプロンプト削除をスキップしました!! (ID：[{messageIdToDelete}] は既に削除済み)", LogType.Debug);
                                deletionSucceeded = true;
                                //削除に失敗しても、後続の処理は継続させる
                            }
                            catch (Exception ex)
                            {
                                //削除権限エラーなどの場合はログに出力
                                Log($"[Kick Timer] タイムアウト時のプロンプト削除に失敗しました!! (ID：[{messageIdToDelete}])\nエラー：{ex.Message.Substring(0, Math.Min(ex.Message.Length, 150))}...", LogType.DebugError);
                            }
                            //2．クリーンアップ
                            // 削除が成功した場合/Unknown Messageの場合のみ、Handlerの状態をクリア
                            if (deletionSucceeded && kickHandler != null)
                            {
                                kickHandler.FinalTimeoutMessageId = 0;
                                //念のため、他のトラッキングIDもクリアしてセッション情報を完全に消す
                                kickHandler.LastPromptMessageId = 0;
                                kickHandler.LastPromptChannelId = 0;
                            }

                            //3．タイムアウト通知を送信
                            var timeoutEmbed = new EmbedBuilder()
                                .WithTitle("⚠️ Kick 操作タイムアウト")
                                .WithDescription($"<@{userId}>\n**キック操作が[{timeoutMinutes}]分間入力されなかったため、中断されました!!**\n\n再度操作を行うには `!kick @メンション` または `!kick ユーザID` を実行してください!!")
                                .WithColor(Discord.Color.Orange)
                                .Build();

                            //4．埋め込みメッセージとして送信
                            var notify = await kickChannel.SendMessageAsync(
                                //@メンションをDescriptionに含めたため、テキストは空でOK
                                text: $"",
                                embed: timeoutEmbed
                            );

                            //通知を10秒後に削除するタスクを開始
                            _ = Task.Run(async () =>
                            {
                                await Task.Delay(10000);
                                //DeleteAsyncの失敗は無視
                                try { await notify.DeleteAsync(); } catch { }
                            });
                        }

                        //5．対話セッションを強制終了
                        if (_activeInteractions.ContainsKey(userId))
                        {
                            _activeInteractions.Remove(userId);
                            Log($"[対話終了] タイムアウトにより、ユーザー[{userId}] のセッションを終了しました!!", LogType.UserMessage);
                        }
                        break;
                    //Banコマンドのタイムアウト処理
                    case "ban":
                        //セッションが既に削除されている場合は、通知処理をスキップ
                        if (!_activeInteractions.ContainsKey(userId))
                        {
                            return;
                        }
                        ICommandHandler banHandler = handler;

                        //プラグインの新しいプロパティを優先して削除対象IDとする(最終確認メッセージのID)
                        if (banHandler != null && banHandler.FinalTimeoutMessageId != 0)
                        {
                            messageIdToDelete = banHandler.FinalTimeoutMessageId;
                        }

                        //1．プロンプトメッセージを削除
                        if (_client.GetChannel(channelId) is ITextChannel banChannel)
                        {
                            bool deletionSucceeded = false;
                            try
                            {
                                //メッセージオブジェクトを取得せず、IDで直接削除を実行
                                await banChannel.DeleteMessageAsync(messageIdToDelete);
                                Log($"[Ban Timer] タイムアウトによりプロンプトメッセージ ID：[{messageIdToDelete}] を削除しました!!", LogType.Debug);
                                deletionSucceeded = true;
                            }
                            catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                            {
                                Log($"[Ban Timer] タイムアウト時のプロンプト削除をスキップしました!! (ID：[{messageIdToDelete}] は既に削除済み)", LogType.Debug);
                                deletionSucceeded = true;
                            }
                            catch (Exception ex)
                            {
                                //削除権限エラーなどの場合はログに出力
                                Log($"[Ban Timer] タイムアウト時のプロンプト削除に失敗しました!! (ID：[{messageIdToDelete}])\nエラー：{ex.Message.Substring(0, Math.Min(ex.Message.Length, 150))}...", LogType.DebugError);
                            }

                            //2．クリーンアップ
                            //削除が成功した場合/Unknown Messageの場合のみ、Handlerの状態をクリア
                            if (deletionSucceeded && banHandler != null)
                            {
                                banHandler.FinalTimeoutMessageId = 0;
                                //念のため、他のトラッキングIDもクリアしてセッション情報を完全に消す
                                banHandler.LastPromptMessageId = 0;
                                banHandler.LastPromptChannelId = 0;
                            }

                            //3．タイムアウト通知を送信
                            var timeoutEmbed = new EmbedBuilder()
                                .WithTitle("⚠️ Ban 操作タイムアウト")
                                //timeoutMinutes を使用して正確な時間を表示
                                .WithDescription($"<@{userId}>\n**Ban 操作が[{timeoutMinutes}]分間入力されなかったため、中断されました!!**\n\n再度操作を行うには `!ban @メンション` または `!ban ユーザID` を実行してください!!")
                                .WithColor(Discord.Color.Orange)
                                .Build();

                            //埋め込みメッセージとして送信
                            var notify = await banChannel.SendMessageAsync(
                                text: $"",
                                embed: timeoutEmbed
                            );

                            //通知を10秒後に削除するタスクを開始
                            _ = Task.Run(async () =>
                            {
                                await Task.Delay(10000);
                                //DeleteAsyncの失敗は無視
                                try { await notify.DeleteAsync(); } catch { }
                            });
                        }

                        //4．話セッションを強制終了
                        if (_activeInteractions.ContainsKey(userId))
                        {
                            _activeInteractions.Remove(userId);
                            Log($"[対話終了] タイムアウトにより、ユーザー[{userId}] のセッションを終了しました!!", LogType.UserMessage);
                        }
                        break;
                    //メッセージ削除コマンドを追加
                    case "del":
                        //タイムアウト処理を適用
                        if (!_activeInteractions.ContainsKey(userId))
                        {
                            return;
                        }
                        ICommandHandler deleteHandler = handler;

                        //プラグインの新しいプロパティを優先して削除対象IDとする(最終確認メッセージのID)
                        if (deleteHandler != null && deleteHandler.FinalTimeoutMessageId != 0)
                        {
                            messageIdToDelete = deleteHandler.FinalTimeoutMessageId;
                        }

                        //1．プロンプトメッセージを削除
                        if (_client.GetChannel(channelId) is ITextChannel deleteChannel)
                        {
                            bool deletionSucceeded = false;
                            try
                            {
                                //メッセージオブジェクトを取得せず、IDで直接削除を実行
                                await deleteChannel.DeleteMessageAsync(messageIdToDelete);
                                Log($"[Delete Timer] タイムアウトによりプロンプトメッセージ ID:{messageIdToDelete} を削除しました。", LogType.Debug);
                                deletionSucceeded = true;
                            }
                            catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                            {
                                Log($"[Delete Timer] タイムアウト時のプロンプト削除をスキップしました (ID：{messageIdToDelete} は既に削除済み)。", LogType.Debug);
                                deletionSucceeded = true;
                            }
                            catch (Exception ex)
                            {
                                //削除権限エラーなどの場合はログに出力
                                Log($"[Delete Timer] タイムアウト時のプロンプト削除に失敗しました!! (ID：{messageIdToDelete})\nエラー：{ex.Message.Substring(0, Math.Min(ex.Message.Length, 150))}...", LogType.DebugError);
                            }

                            //2．クリーンアップ
                            //削除が成功した場合/Unknown Messageの場合のみ、Handlerの状態をクリア
                            if (deletionSucceeded && deleteHandler != null)
                            {
                                deleteHandler.FinalTimeoutMessageId = 0;
                                //念のため、他のトラッキングIDもクリアしてセッション情報を完全に消す
                                deleteHandler.LastPromptMessageId = 0;
                                deleteHandler.LastPromptChannelId = 0;
                            }

                            //3．タイムアウト通知を送信
                            var timeoutEmbed = new EmbedBuilder()
                                .WithTitle("⚠️ メッセージ削除操作タイムアウト")
                                //timeoutMinutes を使用して正確な時間を表示
                                .WithDescription($"<@{userId}> **メッセージ削除操作が{timeoutMinutes}分間入力されなかったため、中断されました!!**\n\n再度操作を行うには `!del` または `!del 数値` を実行してください!!")
                                .WithColor(Discord.Color.Orange)
                                .Build();

                            //埋め込みメッセージとして送信
                            var notify = await deleteChannel.SendMessageAsync(
                                text: $"",
                                embed: timeoutEmbed
                            );

                            //通知を10秒後に削除するタスクを開始
                            _ = Task.Run(async () =>
                            {
                                await Task.Delay(10000);
                                //DeleteAsyncの失敗は無視
                                try { await notify.DeleteAsync(); } catch { }
                            });
                        }
                        //4．話セッションを強制終了
                        if (_activeInteractions.ContainsKey(userId))
                        {
                            _activeInteractions.Remove(userId);
                            Log($"[対話終了] タイムアウトにより、ユーザー[{userId}] のセッションを終了しました!!", LogType.UserMessage);
                        }
                        break;

                    default:
                        Log($"[Timer] 未対応のコマンド[{handler.CommandName}]がタイムアウトしました!!", LogType.Debug);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"[QA Timer] タイムアウト処理中にエラーが発生しました!!\n{ex.Message}", LogType.DebugError);
            }
        }
        //Timerストップ
        private void StopCommandTimer(ulong messageId)
        {
            //タイマー辞書から該当するメッセージIDのエントリを検索
            if (_qaPollTimeouts.TryGetValue(messageId, out CancellationTokenSource cts))
            {
                //実行中の Task.Delay をキャンセル
                cts.Cancel();
                //辞書から削除
                _qaPollTimeouts.Remove(messageId);
                //リソースを解放
                cts.Dispose();

                Log($"[Timer] メッセージID：[{messageId}] に紐づくタイマー (CTS) を明示的に停止しました!!", LogType.Debug);
            }
        }
        private async Task OnQATimeoutLogic(ulong messageId, ulong channelId)
        {
            //1．レジストリ設定の取得と制限モードの決定
            bool allowMultipleVotes = RegistryHelper.LoadAllowMultipleVotesSetting();
            //チャンネルとメッセージを取得
            if (_client.GetChannel(channelId) is ITextChannel channel)
            {
                IUserMessage message = await channel.GetMessageAsync(messageId) as IUserMessage;

                if (message == null) return;

                //投票結果の集計ロジック

                //最終集計に使用する投票結果のリスト/辞書
                var allVotes = new List<int>();
                var singleVotes = new Dictionary<ulong, int>();

                var pollEmotes = DiscordBot.Core.ReactionEmojis.Numbers;

                //選択肢の数字の若い順（1から）に処理
                for (int i = 0; i < pollEmotes.Length; i++)
                {
                    IEmote currentEmote = pollEmotes[i];
                    int optionNumber = i + 1;

                    //リアクションユーザーの最新のリストを取得
                    var reactors = await message.GetReactionUsersAsync(currentEmote, 100).FlattenAsync();

                    foreach (var user in reactors)
                    {
                        if (user.IsBot) continue;

                        if (allowMultipleVotes)
                        {
                            //1人1票制限が有効な場合：重複排除ロジックを適用
                            if (!singleVotes.ContainsKey(user.Id))
                            {
                                singleVotes.Add(user.Id, optionNumber);
                            }
                        }
                        else
                        {
                            //複数投票が許可されている場合: 全てのリアクションをそのまま集計対象として記録
                            allVotes.Add(optionNumber);
                        }
                    }
                }
                List<int> votesToCount = allowMultipleVotes
                    ? singleVotes.Values.ToList()
                    : allVotes;


                //最終集計結果: 選択肢の数字（1, 2, 3...）ごとの投票数
                var results = votesToCount
                    .GroupBy(option => option)
                    .ToDictionary(g => g.Key, g => g.Count());

                //総投票数 (votesToCount の要素数)
                int totalVotes = votesToCount.Count;


                //投票集計が完了した後で、リアクションを削除する
                await message.RemoveAllReactionsAsync();


                //2．メッセージを編集して結果を表示
                var embed = message.Embeds.FirstOrDefault()?.ToEmbedBuilder();
                if (embed != null)
                {
                    //結果を Embed に追加するために整形
                    var resultLines = new List<string>();
                    foreach (var result in results.OrderBy(r => r.Key))
                    {
                        int optionNumber = result.Key;
                        int count = result.Value;
                        IEmote emote = pollEmotes[optionNumber - 1];
                        resultLines.Add($"{emote.Name} [**{count}** 票]");
                    }

                    if (resultLines.Any())
                    {
                        string originalDescription = message.Embeds.FirstOrDefault().Description;
                        //1人1票制限の有無を明記
                        string voteMode = allowMultipleVotes ? "(1人1票制限あり)" : "(複数投票可)";
                        embed.WithDescription(originalDescription + $"\n\n**総投票数：[{totalVotes} 票] {voteMode}**");
                        string resultText = string.Join("\n", resultLines);
                        embed.AddField("📊 投票結果", resultText, false);
                    }
                    else
                    {
                        string originalDescription = message.Embeds.FirstOrDefault().Description;
                        embed.WithDescription(originalDescription + "\n\n**投票者はいませんでした!!**");
                        embed.AddField("📊 投票結果", "票は投じられませんでした!!", false);
                    }

                    await message.ModifyAsync(x => x.Embed = embed.Build());
                    Log($"[QA Timer] アンケートID：[{messageId}] の投票を終了し、結果を表示しました!! 総投票数：[{totalVotes} 票] (1人1票制限：{allowMultipleVotes})", LogType.Debug);
                }
            }
        }
        //UIスレッドで発生した未処理の例外を捕捉
        public class BotCommand
        {
            public string Command { get; set; }
            public string Response { get; set; }
        }

        public class CommandConfig
        {
            public List<BotCommand> Management { get; set; }
        }
        public class CommandSetting
        {
            public string CommandName { get; set; } = string.Empty;
            public string SendMessage { get; set; } = string.Empty;
            public string EmbedTitle { get; set; } = string.Empty;
            public string EmbedDescription { get; set; } = string.Empty;
            public string EmbedColorHex { get; set; } = "#FFFFFF";
        }

        public class CommandSettings
        {
            public List<CommandSetting> Commands { get; set; } = new List<CommandSetting>();
        }
        private System.Reflection.Assembly ResolvePluginDependency(object sender, ResolveEventArgs args)
        {
            //1．必要な DLL の名前を取得 (例："Newtonsoft.Json" -> "Newtonsoft.Json.dll")
            string assemblyName = new System.Reflection.AssemblyName(args.Name).Name + ".dll";

            //2．依存 DLL が存在する Plugins フォルダのパスを設定
            string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            //3．依存 DLL のフルパスを構築
            string assemblyPath = Path.Combine(pluginsPath, assemblyName);

            if (File.Exists(assemblyPath))
            {
                //4．依存 DLL が Plugins フォルダ内に見つかった場合
                try
                {
                    //DLLファイルをバイト配列として読み込む (Assembly.Load(byte[])の要件)
                    byte[] assemblyData = File.ReadAllBytes(assemblyPath);

                    //PDBファイルもあれば読み込む(デバッグ情報用)
                    string pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
                    byte[] pdbData = null;
                    if (File.Exists(pdbPath))
                    {
                        pdbData = File.ReadAllBytes(pdbPath);
                    }

                    //5．依存 DLL を手動でロードし、ランタイムに提供
                    return System.Reflection.Assembly.Load(assemblyData, pdbData);
                }
                catch (Exception ex)
                {
                    //ロード中のエラーをログに出力
                    Log($"[エラー] 依存DLL [{assemblyName}] のロードに失敗：{ex.Message}", LogType.Error);
                }
            }

            //6．見つからなかった場合は null を返すと、標準ローダーが他の場所（GACなど）を探します。
            return null;
        }
        //アンケートQAのDLL参照をアンロードするメソッド
        public void ReleasePluginReferences(string pluginDllFileName)
        {
            if (pluginDllFileName.Equals("DiscordBot.Plugin.QA.dll", StringComparison.OrdinalIgnoreCase))
            {
                lock (_qaPollTimeouts)
                {
                    if (_qaPollTimeouts.Count > 0)
                    {
                        Log($"[QATimer] {Environment.NewLine}アンケートタイムアウト追跡中のリソースをクリーンアップします!! [{_qaPollTimeouts.Count} 件]", LogType.Debug);

                        foreach (var cts in _qaPollTimeouts.Values)
                        {
                            try
                            {
                                // 実行中の処理をキャンセルし、リソースを破棄 (Dispose)
                                cts.Cancel();
                                cts.Dispose();
                            }
                            catch (Exception ex)
                            {
                                // ログを出力して続行
                                Log($"[QATimer Cleanup Error] リソース破棄中にエラーが発生しました：{ex.Message}", LogType.Error);
                            }
                        }
                        _qaPollTimeouts.Clear();
                        Log("アンケートタイムアウト追跡中のリソースのクリーンアップ完了!!", LogType.Debug);
                    }
                }
            }
            //1．解除対象のイベントハンドラを取得
            var handlersToRelease = _pluginEventHandlers
                .Where(h => h.PluginDLL.Equals(pluginDllFileName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!handlersToRelease.Any())
            {
                return;
            }

            //2．イベント購読の解除 (IUninitializer.Uninitialize() の実行)
            foreach (var handler in handlersToRelease)
            {
                //IUninitializerを実装しているかチェックして実行
                if (handler is IUninitializer uninitializer)
                {
                    uninitializer.Uninitialize();
                }
            }

            //3．_pluginEventHandlers から参照を削除(メモリ解放のため)
            _pluginEventHandlers.RemoveAll(h => h.PluginDLL.Equals(pluginDllFileName, StringComparison.OrdinalIgnoreCase));
        }
        //プラグインリロードメソッド
        private async void BtnReloadAllPlugins_Click(object sender, EventArgs e)
        {
            if (_isReloading)
            {
                return;
            }
            _isReloading = true;
            //BOT接続状態をリロード前に取得 (再接続が必要か判断するため)
            bool wasBotRunning = IsBotRunning;

            //トークンを事前に取得 (再接続に必要)
            string token = TxtToken.Text.Trim();

            try
            {
                //リロード前のDLL数をカウント
                int initialDllCount = _pluginEventHandlers.Select(h => h.PluginDLL).Distinct().Count();
                Log("----------------------------------------------------", LogType.Success);
                Log($"全プラグインリロード開始!!", LogType.Success);
                Log($"リロード対象：[{initialDllCount} 個]", LogType.Success);
                Log("----------------------------------------------------", LogType.Success);

                //1．まず全て解除（アンロード）する
                //この中でイベントのデタッチ、リストのクリア、そしてGCによる参照解放が行われます。
                BtnReleaseAllPlugins_Click(sender, e);

                //2．_client が存在する場合は完全に停止・破棄し、新しいインスタンスが作られるようにする
                //wasBotRunningに関わらず、もしあれば破棄する
                if (_client != null)
                {
                    Log("既存のBOT接続を破棄します...", LogType.Success);
                    await _client.StopAsync();
                    await _client.LogoutAsync();
                    _client.Dispose();
                    // StartAsyncで確実に新しいインスタンスが生成されるようにnullにする
                    _client = null;
                    Log("既存のBOT接続を破棄しました!!", LogType.Success);
                }

                //3．StartAsync を実行してプラグインを再ロードし、接続を試みる
                if (string.IsNullOrEmpty(token))
                {
                    Log("BOTトークンが空のため、再接続/起動をスキップしました!!", LogType.Error);
                    //接続しないため停止状態とする
                    IsBotRunning = false;
                    return;
                }

                if (wasBotRunning)
                {
                    Log("新しいプラグイン設定を反映させるため、再接続します!!", LogType.Success);
                }
                else
                {
                    Log("DLLをロードし、BOTを起動します!!", LogType.Success);
                }

                //StartAsyncを実行 (内部で LoadCommandPlugins() が呼ばれる)
                //これが接続ボタンを押したときと全く同じロードパスを強制し、動作を統一します。
                await StartAsync(token);
                Log("----------------------------------------------------", LogType.Success);
                Log("✅：BOTの再接続/起動が完了し、新しいプラグインが適用されました!!", LogType.Success);
                //ロード後のDLL数をカウント
                int finalDllCount = _pluginEventHandlers.Select(h => h.PluginDLL).Distinct().Count();
                Log($"リロードされたDLL数：[{finalDllCount} 個]", LogType.Success);
                Log("----------------------------------------------------", LogType.Success);
            }
            catch (Exception ex)
            {
                Log($"プラグインの再ロード中に致命的なエラーが発生しました!!/n{ex.Message}", LogType.Error);
                MessageBox.Show($"プラグインの再ロード中に致命的なエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isReloading = false;
            }
        }
        //プラグインアンロードメソッド
        private void BtnReleaseAllPlugins_Click(object sender, EventArgs e)
        {
            if (_isReloading)
            {
                //ログを出すと再度ループを引き起こす可能性があるため、サイレントに抜けます
                return;
            }

            _isReloading = true;
            try
            {
                //1．解除対象のDLLファイル名リストを作成
                //_pluginEventHandlersフィールドから情報を取得
                List<string> dllsToRelease = _pluginEventHandlers
                    .Select(h => h.PluginDLL)
                    .Distinct()
                    .ToList();

                if (!dllsToRelease.Any())
                {
                    Log("[警告]：現在ロードされているプラグインはありません!!", LogType.Error);
                    return;
                }
                Log("----------------------------------------------------", LogType.Success);
                Log($"全プラグインアンロード開始!!", LogType.Success);
                Log("----------------------------------------------------", LogType.Success);
                //2．各DLLに対して参照解放処理を実行
                foreach (string dllFileName in dllsToRelease)
                {
                    //ReleasePluginReferences は内部で _pluginEventHandlers から参照を削除
                    //Uninitialize() を呼び出してイベント購読を解除
                    ReleasePluginReferences(dllFileName);
                }

                //3．コマンドリストもクリア
                int releasedDllCount = dllsToRelease.Count;
                _dllCommands.Clear();
                _commandProviders.Clear();
                _commandHandlers.Clear();
                //_pluginEventHandlers は ReleasePluginReferences の内部で既にクリアされています
                Log("----------------------------------------------------", LogType.Success);
                Log($"✅：全プラグイン解除が完了しました!!", LogType.Success);
                Log($"アンロード成功：[{releasedDllCount} 個]", LogType.Success);
                Log("----------------------------------------------------", LogType.Success);
            }
            finally
            {
                _isReloading = false;
            }
        }
        //リロード/アンロードボタンの状態を更新するメソッド
        private void UpdateReloadButtonsState(bool isRunning)
        {
            //リロードボタンの無効化/有効化
            BtnReloadAllPlugins.Enabled = isRunning;

            //アンロードボタンの無効化/有効化
            BtnReleaseAllPlugins.Enabled = isRunning;
        }
        //フォームアクティベート時
        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.MainForm_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(Properties.Settings.Default.MainForm_FormX, Properties.Settings.Default.MainForm_FormY);
                        //フォームの起動位置を画面の中央に設定
                        var screen = Screen.PrimaryScreen.WorkingArea;
                        //フォームの中央位置を計算
                        int centerX = (screen.Width - this.Width) / 2;
                        int centerY = (screen.Height - this.Height) / 2;
                        //フォームの位置を中央に設定
                        this.Location = new Point(centerX, centerY);
                        Properties.Settings.Default.MainForm_Init = false;
                        Properties.Settings.Default.Save();
                        isActivated = true;
                    }
                    else
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(Properties.Settings.Default.MainForm_FormX, Properties.Settings.Default.MainForm_FormY);
                        //フォームの位置を変更
                        this.Location = new Point(Properties.Settings.Default.MainForm_PositionX, Properties.Settings.Default.MainForm_PositionY);
                        isActivated = true;
                    }
                }
                else
                {
                    //フォームの初期サイズを設定
                    this.Size = new System.Drawing.Size(Properties.Settings.Default.MainForm_FormX, Properties.Settings.Default.MainForm_FormY);
                    //フォームの起動位置を画面の中央に設定
                    var screen = Screen.PrimaryScreen.WorkingArea;
                    //フォームの中央位置を計算
                    int centerX = (screen.Width - this.Width) / 2;
                    int centerY = (screen.Height - this.Height) / 2;
                    //フォームの位置を中央に設定
                    this.Location = new Point(centerX, centerY);
                    isActivated = true;
                }
            }
        }
        //ログイン完了ログ
        private Task OnReady()
        {
            Log($"ログイン完了：{_client.CurrentUser}", LogType.Success);
            Log("〇：-------------------- BOT 起動完了!! --------------------", LogType.Bot);
            return Task.CompletedTask;
        }
        public async Task StartAsync(string token)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            });

            /*_client.Log += (msg) =>
            {
                Log("[Discord.Net] " + msg.ToString());
                return Task.CompletedTask;
            };*/
            _client.Log += (msg) => { Log(msg.ToString()); return Task.CompletedTask; };
            _client.Ready += OnReady;
            _client.MessageReceived += OnMessageReceived;

            //設定ファイル読み込み
            try
            {
                if (!File.Exists(ConfigDirectory))
                {
                    Log($"❌：Commands.json が見つかりません!!", LogType.Error);
                    _commands = new Dictionary<string, CommandSetting>();
                }
                else
                {
                    string json = File.ReadAllText(ConfigDirectory);
                    var rootConfig = JsonConvert.DeserializeObject<CommandSettings>(json);

                    if (rootConfig == null || rootConfig.Commands == null)
                    {
                        Log("❌：Commands.json の内容が不正です!! ※デシリアライズ失敗!!", LogType.Error);
                        _commands = new Dictionary<string, CommandSetting>();
                        return;
                    }
                    //Dictionary に変換
                    _commands = new Dictionary<string, CommandSetting>();
                    foreach (var cmd in rootConfig.Commands)
                    {
                        _commands[cmd.CommandName] = cmd;
                    }
                    Log($"〇：[{_commands.Count} 個]のコマンドを読み込みました!!", LogType.Success);
                }
                //LoadCommandPlugins();
            }
            catch (Exception ex)
            {
                Log("JSON読み込みエラー：" + ex.Message, LogType.Error);
                _commands = new Dictionary<string, CommandSetting>();
            }
            LoadCommandPlugins();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }
        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (IsBotRunning)
            {
                Log("BOTはすでに起動しています!!", LogType.Bot);
                return;
            }
            string token = TxtToken.Text.Trim();
            if (string.IsNullOrEmpty(token))
            {
                MessageBox.Show("BOTトークンを入力してください!!", "お知らせ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            IsBotRunning = true;
            BtnStart.Enabled = false;
            BtnStop.Enabled = true;
            UpdateReloadButtonsState(IsBotRunning);
            Log("BOTを起動します...", LogType.Bot);
            await StartAsync(token);
        }

        public async void BtnStop_Click(object sender, EventArgs e)
        {
            await StopBotLogicAsync();
        }
        //コア処理とUI更新を分離し、パブリックなメソッドとして公開
        public async Task StopBotLogicAsync()
        {
            if (!IsBotRunning || _client == null)
            {
                Log("BOTは起動していません!!", LogType.Bot);
                return;
            }

            //停止処理
            await _client.StopAsync();
            await _client.LogoutAsync();
            _client.Dispose();
            IsBotRunning = false;

            //UI更新処理
            if (_isLicenseActive)
            {
                BtnStop.Enabled = false;
                BtnStart.Enabled = true;
            }
            else
            {
                BtnStop.Enabled = false;
                BtnStart.Enabled = false;
            }
            UpdateButtonStates();
            //UpdateReloadButtonsState(IsBotRunning);
            Log("〇：-------------------- BOT 停止完了!! --------------------", LogType.Bot);
        }
        //BOTの実行状態とライセンス状態に基づいて、BtnStartとBtnStopのEnabled状態を更新
        private void UpdateButtonStates()
        {
            //BOTが稼働中か否かでボタンの状態を決定
            if (IsBotRunning)
            {
                //稼働中は停止ボタンのみ有効
                BtnStop.Enabled = true;
                BtnStart.Enabled = false;
            }
            //BOTが停止中の場合
            else
            {
                if (_isLicenseActive)
                {
                    //ライセンス認証済みなら、開始ボタンのみ有効
                    BtnStop.Enabled = false;
                    BtnStart.Enabled = true;
                }
                else
                {
                    //ライセンス未認証なら、両方無効
                    BtnStop.Enabled = false;
                    BtnStart.Enabled = false;
                }
            }
            UpdateReloadButtonsState(IsBotRunning);
        }
        //ライセンス認証状態を外部から設定するためのメソッド
        public void SetLicenseActiveState(bool isActive)
        {
            //プライベートフィールドの値を更新
            _isLicenseActive = isActive;
            UpdateButtonStates();
            //UpdateReloadButtonsState(IsBotRunning);
        }
        private async Task OnMessageReceived(SocketMessage message)
        {
            try
            {
                if (message.Author.IsBot) return;
                if (message.Channel == null) return;
                if (!(message.Channel is IMessageChannel channel)) return;
                string content = message.Content?.Trim();
                ulong authorId = message.Author.Id;
                //1．ユーザーが現在対話中かチェック
                if (_activeInteractions.TryGetValue(authorId, out ICommandHandler activeHandler))
                {
                    //対話中の場合: ユーザーのメッセージをDLLに渡し、応答を取得
                    MessageResponse response = await activeHandler.ExecuteInteractiveAsync(content);
                    try
                    {
                        //message はユーザーが入力したメッセージ
                        await message.DeleteAsync();
                        Log($"[INFO] ユーザー応答メッセージを削除しました!! ID：[{message.Id}]", LogType.Debug);
                    }
                    //Unknown Message (10008) エラーをキャッチして無視する
                    catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                    {
                        //ユーザーが先に削除した場合の処理。エラーではなくDebug/Infoログ
                        Log($"[INFO] ユーザー応答メッセージの削除をスキップしました!! ※ID：[{message.Id}] は既に削除済み!!", LogType.Debug);
                    }
                    catch (Exception ex)
                    {
                        //削除権限がないなどのエラーの場合
                        Log($"[ERROR] ユーザー応答メッセージの削除に失敗しました!!\n{ex.Message}", LogType.DebugError);
                    }
                    //BOT本体は応答をDiscordに送信する仲介役を担う
                    //await SendResponseAsync(message.Channel, response);
                    await HandleCommandResponse(channel, response, activeHandler, authorId);
                    //DLLが終了を宣言したら、BOT本体のセッション管理から削除
                    if (activeHandler.IsFinished)
                    {
                        if (activeHandler.LastPromptMessageId != 0 && activeHandler.LastPromptChannelId != 0)
                        {
                            try
                            {
                                var promptChannel = _client.GetChannel(activeHandler.LastPromptChannelId) as IMessageChannel;
                                if (promptChannel != null)
                                {
                                    await promptChannel.DeleteMessageAsync(activeHandler.LastPromptMessageId);
                                    Log($"[INFO] 対話終了時の最終プロンプトメッセージ ID：[{activeHandler.LastPromptMessageId}] を削除しました!!", LogType.Debug);
                                }
                            }
                            catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                            {
                                //UnknownMessageは無視できるエラーなので、INFOレベルでログを出す
                                Log($"[INFO] 対話終了時のプロンプト削除をスキップしました!! ※ID：[{activeHandler.LastPromptMessageId}] は既に削除済み!!", LogType.Debug);
                            }
                            catch (Exception ex)
                            {
                                Log($"[ERROR] 対話終了時の最終プロンプト削除に失敗しました!! BOTに『メッセージの管理』権限があるか確認してください!!", LogType.DebugError);
                                Log($"[ERROR] ID：[{activeHandler.LastPromptMessageId}]\n{ex.Message}", LogType.DebugError);
                            }
                            //メッセージ削除が試行された後、すべてのIDをクリアする
                            activeHandler.LastPromptMessageId = 0;
                            activeHandler.LastPromptChannelId = 0;
                            activeHandler.FinalTimeoutMessageId = 0;
                        }
                        _activeInteractions.Remove(authorId);
                        //対話終了時にタイマーが残っている可能性があるため、強制停止する
                        if (activeHandler.LastPromptMessageId != 0)
                        {
                            // 対話が正常終了した場合、LastPromptMessageId（最終プロンプトID）にタイマーが紐づいている
                            StopCommandTimer(activeHandler.LastPromptMessageId);
                            Log($"[Timer] セッション終了時クリーンアップにより、プロンプトID：[{activeHandler.LastPromptMessageId}] のタイマーを停止しました!!", LogType.Debug);
                        }

                        // ユーザーのアクティブセッションリストから削除
                        if (_activeInteractions.ContainsKey(authorId))
                        {
                            _activeInteractions.Remove(authorId);
                            // ...
                        }
                        //対話終了時のログ
                        Log($"[対話終了] DLLプラグインコマンド：[{message.Author.Username}] のセッションを終了しました!!", LogType.UserMessage);
                    }
                    else
                    {
                        //対話継続時のログ
                        Log($"[対話継続] DLLプラグインコマンド：[{message.Author.Username}] の応答を受信!!", LogType.UserMessage);
                    }
                    return;
                }
                //2．対話中でない場合、新規コマンドとして処理
                if (!content.StartsWith("!")) return;
                //例："!qa "タイトル" | ..."
                string fullCommand = content;
                string commandText = content.Substring(1).Trim();
                //スペースの位置でコマンド名を抽出する
                int firstSpaceIndex = commandText.IndexOf(' ');
                string commandName;
                if (firstSpaceIndex == -1)
                {
                    //引数がない場合 (例："qa")
                    //commandName = commandText.ToLower();
                    commandName = commandText;
                }
                else
                {
                    //引数がある場合 (例："qa "タイトル" | ...")
                    //commandName = commandText.Substring(0, firstSpaceIndex).ToLower();
                    commandName = commandText.Substring(0, firstSpaceIndex);
                }
                //JSONコマンド検索用のキー："!qa"
                string commandKey = "!" + commandName;
                //DLLコマンド検索用に、小文字のコマンド名を別に作成
                string commandNameLower = commandName.ToLower();
                //デバッグログの追加
                Log($"[INFO] 抽出されたコマンド名：[{commandName}] 長さ：[{commandName.Length}]", LogType.Debug);
                //DLLコマンドリストのサイズ確認
                //Log($"[DEBUG] DLLコマンド数：{_dllCommands.Count}", LogType.Normal);
                //2-1．対話型DLLコマンド (ICommandHandler) のチェック
                var handlerTemplate = _commandHandlers?
                    .FirstOrDefault(h => h.CommandName.Equals(commandNameLower, StringComparison.OrdinalIgnoreCase));

                if (handlerTemplate != null)
                {
                    //一旦、ここで新しいインスタンスを作成する処理を仮定します。
                    ICommandHandler newHandler;
                    try
                    {
                        // 既存のテンプレートインスタンスの型情報を取得
                        Type handlerType = handlerTemplate.GetType();

                        //新しいインスタンスを生成（コンストラクタ引数に合わせて修正してください）
                        // DiscordBot_Kickのコンストラクタが (ILogger, DiscordSocketClient) の形だと仮定して、
                        // 依存関係を渡して新しいインスタンスを作成します。
                        newHandler = (ICommandHandler)Activator.CreateInstance(handlerType, new LoggerAdapter(this), _client, message.Author);
                    }
                    catch (Exception ex)
                    {
                        Log($"[ERROR] DLLコマンド[{commandName}]の新規インスタンス作成に失敗しました!!\n{ex.Message}", LogType.Error);
                        return;
                    }
                    //レジストリ設定のチェック
                    bool deleteCommand = RegistryHelper.LoadDeleteCommandMessageSetting();
                    //最初の実行メソッドを呼び出し
                    MessageResponse response = await newHandler.ExecuteInitialAsync(message, content);

                    //最初の応答を送信
                    //await SendResponseAsync(message.Channel, response);
                    await HandleCommandResponse(channel, response, newHandler, authorId);
                    Log($"[実行] DLLプラグインコマンド：[!{newHandler.CommandName}] を [{message.Author.Username}] が開始!!", LogType.UserMessage);

                    //コマンドメッセージの削除実行
                    if (deleteCommand)
                    {
                        try
                        {
                            await message.DeleteAsync();
                            Log($"[INFO] 実行された ICommandHandler コマンドメッセージ ID：[{message.Id}] を削除しました!!", LogType.Debug);
                        }
                        catch (Exception ex)
                        {
                            Log($"[ERROR] ICommandHandler コマンドメッセージの削除に失敗!!\n{ex.Message}", LogType.DebugError);
                        }
                    }
                    //応答が終了でなければ、対話セッションを開始
                    if (!newHandler.IsFinished)
                    {
                        //新しい対話セッションとして登録(DLLインスタンスを保存)
                        _activeInteractions.Add(authorId, newHandler);
                    }
                    return;
                }
                //2-2．単発型DLLコマンド (ICommand) のチェック
                var commandExecutor = _dllCommands?
                    .FirstOrDefault(c => c.CommandName.Equals(commandNameLower, StringComparison.OrdinalIgnoreCase));

                if (commandExecutor != null)
                {
                    //レジストリ設定のチェック
                    bool deleteCommand = RegistryHelper.LoadDeleteCommandMessageSetting();
                    //Log($"[DEBUG] DLLコマンド [{commandName}] は見つかりませんでした!!", LogType.Debug);
                    //単発実行メソッドを呼び出し
                    MessageResponse response = await commandExecutor.ExecuteInitialAsync(message, fullCommand);

                    //応答を送信
                    //await SendResponseAsync(message.Channel, response);
                    await HandleCommandResponse(channel, response, null, authorId);
                    Log($"[実行] DLLプラグインコマンド：[!{commandExecutor.CommandName}] を [{message.Author.Username}] が実行!!", LogType.UserMessage);
                    //コマンドメッセージの削除実行
                    if (deleteCommand)
                    {
                        try
                        {
                            await message.DeleteAsync();
                            Log($"[INFO] 実行された ICommand コマンドメッセージ ID:{message.Id} を削除しました。", LogType.Debug);
                        }
                        catch (Exception ex)
                        {
                            Log($"[ERROR] ICommand コマンドメッセージの削除に失敗!!\n{ex.Message}", LogType.DebugError);
                        }
                    }
                    //実行が完了したので、ここで終了
                    return;
                }
                //2-3．DLL コマンドが見つからなかった場合、既存の JSON コマンド処理を実行
                //content は「!コマンド名」の形式なので、そのまま Dictionary のキーとして使用
                if (_commands.TryGetValue(commandKey, out CommandSetting setting))
                {
                    // 💡 レジストリ設定のチェック
                    bool deleteCommand = RegistryHelper.LoadDeleteCommandMessageSetting();
                    //メッセージコンテンツ、埋め込みタイトル、埋め込みディスクリプションのいずれかが有効かチェック
                    string textContent = setting.SendMessage;
                    bool useEmbed = !string.IsNullOrWhiteSpace(setting.EmbedTitle) || !string.IsNullOrWhiteSpace(setting.EmbedDescription);

                    //1．空のメッセージ送信を防ぐためのチェック
                    if (string.IsNullOrWhiteSpace(textContent) && !useEmbed)
                    {
                        await message.Channel.SendMessageAsync("❌：このコマンドには応答内容が設定されていません!!");
                        Log($"警告：[{content}] が実行されましたが応答が空です!!", LogType.Error);
                        return;
                    }

                    if (useEmbed)
                    {
                        //埋め込みメッセージを構築する
                        var embed = new EmbedBuilder();

                        if (!string.IsNullOrWhiteSpace(setting.EmbedTitle))
                        {
                            embed.WithTitle(setting.EmbedTitle);
                        }
                        if (!string.IsNullOrWhiteSpace(setting.EmbedDescription))
                        {
                            embed.WithDescription(setting.EmbedDescription);
                        }

                        //カラーコードを処理
                        try
                        {
                            System.Drawing.Color drawingColor = System.Drawing.ColorTranslator.FromHtml(setting.EmbedColorHex);

                            
                            embed.WithColor(new Discord.Color(drawingColor.R, drawingColor.G, drawingColor.B));
                        }
                        catch (Exception ex)
                        {
                            Log($"警告：コマンド [{content}] のカラーコード処理でエラーが発生しました!!\n{ex.Message}", LogType.Error);
                            embed.WithColor(Discord.Color.Blue);
                        }

                        //通常メッセージ(SendMessage) がある場合は、Embedの前に送信されるテキストとして使用
                        string text = string.IsNullOrWhiteSpace(textContent) ? null : textContent;

                        //2．埋め込みメッセージを送信
                        await message.Channel.SendMessageAsync(text, embed: embed.Build());

                        Log($"[{message.Author.Username}] が [{content}] (JSON - Embed) を実行!!", LogType.UserMessage);
                    }
                    else
                    {
                        //埋め込み情報がない場合は、通常のテキストメッセージとして送信
                        await message.Channel.SendMessageAsync(textContent);
                        Log($"[{message.Author.Username}] が [{content}] (JSON - Text) を実行!!", LogType.UserMessage);
                    }
                    //コマンドメッセージの削除実行
                    if (deleteCommand)
                    {
                        try
                        {
                            await message.DeleteAsync();
                            Log($"[INFO] 実行された JSON コマンドメッセージ ID：[{message.Id}] を削除しました!!", LogType.Debug);
                        }
                        catch (Exception ex)
                        {
                            Log($"[ERROR] JSON コマンドメッセージの削除に失敗!!\n{ex.Message}", LogType.DebugError);
                        }
                    }
                    return;
                }
                else
                {
                    //どちらのコマンドも見つからなかった場合
                    //埋め込みメッセージを作成
                    var unknownCommandEmbed = new EmbedBuilder()
                        .WithTitle("❌ 未知のコマンドです")
                        // Botが認識するプレフィックス（例: !）を使って説明を追加
                        .WithDescription($"['{content}'] に対応するコマンドが見つかりませんでした!!\nコマンドはすべて `!` から始まります!!\n\n利用可能なコマンドについては、`!help` コマンドを参照してください!!")
                        .WithColor(Discord.Color.Red) // エラーとして赤色を使用
                        .WithFooter(footer => footer.Text = $"実行ユーザー: {message.Author.Username}")
                        .WithTimestamp(DateTimeOffset.UtcNow)
                        .Build();

                    IUserMessage sentMessage = null;
                    try
                    {
                        //埋め込みメッセージを送信し、送信結果を取得
                        sentMessage = await message.Channel.SendMessageAsync(embed: unknownCommandEmbed);
                    }
                    catch (Exception ex)
                    {
                        Log($"警告：未知のコマンド応答の送信中にエラーが発生しました!!\n{ex.Message}", LogType.Error);
                        //送信に失敗した場合は削除処理もスキップ
                        return;
                    }

                    //10秒後の自動削除処理
                    //非同期実行
                    if (sentMessage != null)
                    {
                        _ = Task.Run(async () =>
                        {
                            //10,000ミリ秒 (10秒) 待機
                            await Task.Delay(10000);
                            try
                            {
                                await sentMessage.DeleteAsync();
                                //Log($"[Discord] 未知のコマンド応答メッセージの自動削除を実行しました。", LogType.Debug);
                            }
                            catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                            {
                                //メッセージがすでに存在しない場合は無視
                            }
                            catch (Exception ex)
                            {
                                //その他の削除エラー
                                Log($"警告：未知のコマンド応答の自動削除に失敗しました!!\n{ex.Message}", LogType.Error);
                            }
                        });
                    }
                    //await message.Channel.SendMessageAsync("❌ 未知のコマンドです!!");
                    Log($"未知のコマンド：[{content}] を [{message.Author.Username}] が実行!!", LogType.UserMessage);
                }
            }
            catch (Exception ex)
            {
                Log("MessageReceived例外：" + ex.Message, LogType.Error);
            }
        }
        //HandleCommandResponseメソッドにタイマー開始ロジックを追加
        private async Task HandleCommandResponse(IMessageChannel channel, MessageResponse response, ICommandHandler handler, ulong userId)
        {
            //1．null チェック
            if (response == null) return;

            //2．メッセージ内容の防御的なチェックを追加
            //    ResponseText が空 AND Embed が null の場合は送信しない
            if (string.IsNullOrWhiteSpace(response.ResponseText) && response.Embed == null)
            {
                //ここにログ出力ロジックを追加して、何が返されたか記録
                //例：Log("[WARNING] DLLコマンドからの応答は内容が空のためスキップされました。", LogType.Error);
                return;
            }

            IUserMessage sentMessage = null;
            try
            {
                //3．メッセージを送信
                sentMessage = await channel.SendMessageAsync(
                    //ResponseTextがnullでも、Embedがあれば送信は成功する
                    text: response.ResponseText,
                    embed: response.Embed,
                    isTTS: false
                );
            }
            catch (Exception ex)
            {
                //送信失敗時のログ
                Log($"コマンド応答の送信中にエラーが発生しました!!\n{ex.Message}", LogType.Error);
                //メッセージ送信に失敗した場合は、後続の処理
                return;
            }
            //4．リアクションを付与
            if (response.Reactions != null && response.Reactions.Count > 0)
            {
                //Discord API の制限 (最大 20 個) を考慮して、最初の 20 個まで追加
                var reactionsToAdd = response.Reactions.Take(20).ToArray();
                await sentMessage.AddReactionsAsync(reactionsToAdd);
            }
            //5．ロールパネル情報の登録
            if (response.CustomData != null)
            {
                DiscordBot.Core.RegistryHelper.RegisterRuntimeData(sentMessage.Id, response.CustomData);

                Log($"[Main] メッセージID {sentMessage.Id} のカスタムデータを登録・通知しました。", LogType.Debug);
            }
            //自動削除処理 (Kickコマンドの5秒削除に対応)
            if (response.ShouldDelete && response.DeleteDelayMs > 0)
            {
                //Task.Delay で指定時間待機した後、メッセージを削除するタスクを非同期で開始
                //メインスレッドをブロックしない
                _ = Task.Run(async () =>
                {
                    await Task.Delay(response.DeleteDelayMs);
                    try
                    {
                        await sentMessage.DeleteAsync();
                        //Log($"[Discord] 自動削除を実行しました (Delay: {response.DeleteDelayMs}ms)", LogType.Debug);
                    }
                    catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                    {
                        //メッセージがすでに存在しない場合 (ユーザーが手動で削除したなど) は無視
                    }
                    catch (Exception ex)
                    {
                        //その他の削除エラー
                        Log($"警告：メッセージの自動削除に失敗しました!!\n{ex.Message}", LogType.DebugError);
                    }
                });
            }
            //古いタイマー停止ロジックを独立させる（対話終了時も実行させる）
            //TimeoutMinutes > 0 の外側、かつハンドラーチェックの内側に配置します
            if (handler != null && handler.LastPromptMessageId != 0)
            {
                //LastPromptMessageId は、前回のプロンプトメッセージIDを保持している
                ulong oldPromptMessageId = handler.LastPromptMessageId;
                ulong oldPromptChannelId = handler.LastPromptChannelId;
                //メッセージ削除ロジックの追加
                //対話が継続している場合（IsFinished=false）に、古いプロンプトを削除する
                if (!handler.IsFinished)
                {
                    var channelToDelete = _client.GetChannel(oldPromptChannelId) as IMessageChannel;

                    if (channelToDelete != null)
                    {
                        try
                        {
                            //古いメッセージを削除
                            await channelToDelete.DeleteMessageAsync(oldPromptMessageId);
                            Log($"[INFO] 対話継続時の古いプロンプトメッセージ ID：[{oldPromptMessageId}] を削除しました!!", LogType.Debug);
                        }
                        //UnknownMessage エラーをキャッチして無視する
                        /*catch (Discord.Net.HttpException ex) when (ex.DiscordCode == Discord.DiscordErrorCode.UnknownMessage)
                        {
                            // ユーザーや他のBOTが先に削除した場合、ここで捕捉して成功ログとして扱う
                            Log($"[INFO] 対話継続時の古いプロンプト削除をスキップしました!! ※ID：[{oldPromptMessageId}] は既に削除済み!!", LogType.Debug);
                        }*/
                        catch (Exception ex)
                        {
                            //削除失敗時 (権限不足など) の警告ログ
                            Log($"[ERROR] 対話継続時の古いプロンプト削除に失敗しました!! ID：[{oldPromptMessageId}]\n{ex.Message}", LogType.DebugError);
                        }
                    }
                }
                //IsFinishedの条件を削除し、タイマーが存在する限り常に停止する
                StopCommandTimer(oldPromptMessageId);

                Log($"[Timer] 処理続行・または対話終了に伴い、古いプロンプトメッセージ ID：[{oldPromptMessageId}] に紐づくタイマーを停止しました!!", LogType.Debug);
            }
            //2．タイムアウト処理の開始
            //MessageResponseにTimeoutMinutesプロパティが存在
            if (response.TimeoutMinutes > 0)
            {
                if (handler != null)
                {
                    //対話が継続する場合（!handler.IsFinished） AND/OR 対話が終了したがタイマーが指定されている場合
                    if (!handler.IsFinished || (handler.IsFinished && response.TimeoutMinutes > 0))
                    {
                        //sentMessage がメッセージIDとチャンネルIDを提供します
                        StartCommandTimer(sentMessage.Id, sentMessage.Channel.Id, response.TimeoutMinutes, handler, userId);

                        //FinalTimeoutMessageId を設定（KICK/BANの途中、またはQAの単発タイマー）
                        handler.FinalTimeoutMessageId = sentMessage.Id;

                        Log($"[Timer] コマンド[{handler.CommandName}] のタイマーを [{response.TimeoutMinutes} 分] で開始しました!!", LogType.Debug);
                    }
                    else
                    {
                        //対話終了時は新しいタイマーは開始されない
                        Log($"[Timer] 対話終了のため、タイマーは開始されませんでした!! メッセージID：[{sentMessage.Id}]", LogType.Debug);
                    }
                }
                else
                {
                    //handlerがnull（ICommandの場合など）はタイマー処理をスキップ
                    Log($"[Timer] コマンドタイマーは開始されませんでした!! (handlerがnull) メッセージID：[{sentMessage.Id}]", LogType.Debug);
                }
            }
            //ICommandHandler インターフェースが LastPromptMessageId および LastPromptChannelId プロパティを公開している必要があります。
            if (handler != null && sentMessage != null)
            {
                if (!handler.IsFinished && !response.IsTransient)
                {
                    handler.LastPromptMessageId = sentMessage.Id;
                    handler.LastPromptChannelId = sentMessage.Channel.Id;

                    //ICommandHandler経由でアクセスするため、キャスト不要
                    //handler.FinalTimeoutMessageId = sentMessage.Id;

                    Log($"[INFO] LastPromptMessageId/ChannelId および FinalTimeoutMessageId を ICommandHandler [{handler.CommandName}] に設定しました!! MessageID：[{sentMessage.Id}]", LogType.Debug);
                }
                else
                {
                    //対話終了時のメッセージ（成功/失敗/キャンセル）を送信した場合
                    //MainForm.cs の cleanup ロジックが前のメッセージを削除できる
                    Log($"[INFO] 対話終了応答を送信しました!! LastPromptMessageId は前のプロンプトID：[{handler.LastPromptMessageId}] のまま保持します!!", LogType.Debug);
                }
            }
        }
        //ToolStripMenuItem(メニュー項目)用のヘルパーメソッド
        private void DisableControlSafe(ToolStripMenuItem menuItem)
        {
            if (menuItem == null) return;

            //ToolStripMenuItem は Controlではないため、Enabledを直接設定
            menuItem.Enabled = false;
            Log($"[設定メニュー] '{menuItem.Text}' をDLLエラーのため無効化しました!!", LogType.Error);
        }
        private void LoadCommandPlugins()
        {
            //リストを初期化
            _commandHandlers = new List<ICommandHandler>();
            //ICommandリストを初期化
            _dllCommands = new List<ICommand>();
            //フィールドをクリアし、インスタンス生成用のリスト宣言は削除
            _pluginEventHandlers.Clear();
            _commandProviders.Clear();
            //Plugins フォルダのパスを設定(実行ファイルと同じ場所の Plugins フォルダ)
            string pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

            if (!Directory.Exists(pluginPath))
            {
                Directory.CreateDirectory(pluginPath);
                Log("Plugins フォルダを作成しました!!", LogType.Normal);
                return;
            }
            //有効期限が切れているかチェック
            if (CheckLicenseExpiry())
            {
                Log("----------------------------------------------------", LogType.Error);
                Log("❌：ライセンス有効期限切れのため、プラグインのロードをスキップしました!!", LogType.Error);
                Log("----------------------------------------------------", LogType.Error);
                return;
            }
            // 💡 【変更点1】ライセンス認証状態の確認
            bool isActivated = DiscordBot.Core.RegistryHelper.GetLicenseActivationFlag();

            if (!isActivated)
            {
                Log("----------------------------------------------------", LogType.Error);
                Log("❌：ライセンス認証されていません!! プラグインのロードをスキップしました!!", LogType.Error);
                Log("----------------------------------------------------", LogType.Error);
                // Pluginsフォルダの準備は完了したので、ここで処理を終了
                return;
            }

            // 💡 【変更点2】認証済みの場合、許可されたプラグインリストを取得
            List<string> allowedPlugins = GetAllowedPlugins();
            if (allowedPlugins.Count == 0)
            {
                Log("----------------------------------------------------", LogType.Error);
                Log("❌：ライセンスファイルに許可されたプラグインが定義されていないか、ファイルが不正です!! プラグインのロードをスキップしました!!", LogType.Error);
                Log("----------------------------------------------------", LogType.Error);
                //return;
            }

            //ロガーアダプターを作成(BOT本体のLogメソッドをDLLに渡すため)
            var logger = new LoggerAdapter(this);
            int loadedCommandCount = 0;
            int initializedEventCount = 0;

            //ロードするDLLのファイル名を全て取得
            var allDllFiles = Directory.GetFiles(pluginPath, "*.dll");

            //初期化後にコマンドを収集するため、ハンドラとプロバイダーのリストを一時的に保持
            List<IPluginEventHandler> eventHandlers = new List<IPluginEventHandler>();
            List<ICommandProvider> commandProviders = new List<ICommandProvider>();
            //ICommandHandlerProvider の一時リストを追加
            List<ICommandHandlerProvider> commandHandlerProviders = new List<ICommandHandlerProvider>();
            foreach (string fullDllPath in allDllFiles)
            {
                string expectedDllFileName = Path.GetFileName(fullDllPath);
                //ライセンスによる許可DLLのチェック
                //allowedPluginsリスト内に、現在のDLLファイル名が存在するか確認 (大文字小文字を区別しない)
                if (!allowedPlugins.Any(p => p.Equals(expectedDllFileName, StringComparison.OrdinalIgnoreCase)))
                {
                    Log("----------------------------------------------------", LogType.Error);
                    Log($"[プラグインロードスキップ] ライセンスで許可されていません!!", LogType.Error);
                    Log($"  > DLL名：[{expectedDllFileName}]", LogType.Error);
                    //Log("----------------------------------------------------", LogType.Error);
                    //許可されていないDLLはスキップし、次のDLLへ
                    continue;
                }
                object configItem = null;
                Assembly assembly = null;

                //辞書に登録されているかチェックし、対応する設定項目を取得(無効化制限の要否チェック)
                _pluginConfigControls.TryGetValue(expectedDllFileName, out configItem);
                try
                {
                    //DLLとPDBファイルをバイト配列として読み込む (ファイルロックを回避)
                    byte[] assemblyData = File.ReadAllBytes(fullDllPath);

                    string pdbPath = Path.ChangeExtension(fullDllPath, ".pdb");
                    byte[] pdbData = null;
                    if (File.Exists(pdbPath))
                    {
                        pdbData = File.ReadAllBytes(pdbPath);
                    }

                    //1．DLL(アセンブリ)をバイト配列から読み込む
                    assembly = Assembly.Load(assemblyData, pdbData);

                    //ロード中に問題があった場合、assembly が null になる場合があるためチェック
                    if (assembly == null) continue;

                    //リフレクションでDLLバージョンを取得
                    string dllVersion = assembly.GetName().Version.ToString();

                    
                    
                    //6．IPluginEventHandler(イベント)のロードと初期化
                    var eventHandlerTypes = assembly.GetTypes()
                        .Where(t => typeof(IPluginEventHandler).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in eventHandlerTypes)
                    {
                        var handler = Activator.CreateInstance(type) as IPluginEventHandler;
                        if (handler != null)
                        {
                            //イベントハンドラを一時リストに保持
                            _pluginEventHandlers.Add(handler);

                            //ICommandProvider はここでコマンド登録をせず、一時リストに格納するだけ
                            if (handler is ICommandProvider commandProvider)
                            {
                                //ICommandProvider も保持
                                _commandProviders.Add(commandProvider);
                            }
                            //ICommandHandlerProvider も保持
                            if (handler is ICommandHandlerProvider commandHandlerProvider)
                            {
                                commandHandlerProviders.Add(commandHandlerProvider);
                            }
                        }
                    }
                }
                catch (ReflectionTypeLoadException typeLoadEx)
                {
                    //外部インターフェース不適合や依存関係の欠落によるエラー(DLLは存在する)
                    Log("----------------------------------------------------", LogType.Error);
                    Log($"[プラグインロード失敗]", LogType.Error);
                    //2．DLL名
                    Log($"  > DLL名：{expectedDllFileName}", LogType.Error);

                    //3．失敗の理由(型エラー)
                    Log($"  > 理由：型検証エラー ※インターフェースまたは依存関係の不一致", LogType.Error);

                    //LoaderExceptions プロパティ内の全てのエラーメッセージを出力
                    foreach (var loaderEx in typeLoadEx.LoaderExceptions)
                    {
                        Log($"  >> 内部エラー：{loaderEx.Message}", LogType.Error);
                    }
                    Log($"  > ロード処理をスキップしました!!", LogType.Error);

                    //制限が必要なDLLの場合のみ、設定項目を無効化
                    if (configItem is ToolStripMenuItem menuItem)
                    {
                        DisableControlSafe(menuItem);
                    }
                }
                catch (Exception ex)
                {
                    //その他の一般的なDLLロードまたはインスタンス生成エラー(DLLは存在する)
                    Log("----------------------------------------------------", LogType.Error);
                    Log($"[プラグインロード失敗]", LogType.Error);
                    Log($"  > DLL名：[{expectedDllFileName}]", LogType.Error);

                    //4．失敗の理由(エラー詳細)
                    Log($"  > 理由：一般ロードエラー ※DLL破損、ファイル見つからない等", LogType.Error);
                    Log($"  >> エラー詳細：{ex.Message}", LogType.Error);
                    Log($"  > ロード処理をスキップしました!!", LogType.Error);

                    //制限が必要なDLLの場合のみ、設定項目を無効化
                    if (configItem is ToolStripMenuItem menuItem)
                    {
                        DisableControlSafe(menuItem);
                    }
                }
            }
            //フェーズ2：IPluginEventHandler の初期化(Initialize)
            foreach (var handler in _pluginEventHandlers)
            {
                //Initialize を先に実行し、ロガーを設定
                if (_client != null)
                {
                    handler.Initialize(_client, logger);
                    initializedEventCount++;

                    //成功ログ表示とメニュー有効化
                    //PluginDLLをPluginから取得
                    string expectedDllFileName = Path.GetFileName(handler.PluginDLL);
                    object configItem = null;
                    _pluginConfigControls.TryGetValue(expectedDllFileName, out configItem);

                    Log("----------------------------------------------------", LogType.Success);
                    Log($"[イベントプラグイン初期化]", LogType.Success);
                    Log($"  > プラグイン名：[{handler.PluginName}]", LogType.Success);
                    Log($"  > DLL名：[{expectedDllFileName}]", LogType.Success);
                    Log($"  > DLLバージョン：[{handler.PluginVersion}]", LogType.Success);

                    if (configItem is ToolStripMenuItem menuItem)
                    {
                        menuItem.Enabled = true;
                        Log($"[プラグイン] 設定メニューを有効化：[{menuItem.Text}]", LogType.Success);
                    }
                }
            }
            //フェーズ3：ICommandProvider から ICommand を収集(ロガー設定後)
            foreach (var commandProvider in _commandProviders)
            {
                //ICommandProviderがIPluginEventHandlerも実装していることを利用して情報を取得
                if (commandProvider is IPluginEventHandler eventHandler)
                {
                    //1．DLLファイル名を取得し、対応する設定項目を取得
                    string expectedDllFileName = Path.GetFileName(eventHandler.PluginDLL);
                    object configItem = null;
                    _pluginConfigControls.TryGetValue(expectedDllFileName, out configItem);

                    //2．コマンド登録
                    foreach (var command in commandProvider.GetCommands())
                    {
                        _dllCommands.Add(command);
                        Log("----------------------------------------------------", LogType.Success);
                        Log($"[プラグインコマンド初期化]", LogType.Success);
                        Log($"  > プラグイン名：[{command.PluginName}]", LogType.Success);
                        Log($"  > ロードされたコマンド名：[!{command.CommandName}]", LogType.Success);
                        loadedCommandCount++;
                    }

                    //3．コマンド登録後にメニューを有効化(フェーズ3の処理)
                    /*if (configItem is ToolStripMenuItem menuItem)
                    {
                        //有効化
                        menuItem.Enabled = true;
                        Log($"[プラグイン] 設定メニューを有効化：{menuItem.Text}", LogType.Success);
                    }*/
                }
            }
            //フェーズ4：ICommandHandlerProvider から ICommandHandler を収集(ロガー設定後)
            foreach (var handlerProvider in commandHandlerProviders)
            {
                // ICommandHandlerProvider も IPluginEventHandler も実装していることを利用
                if (handlerProvider is IPluginEventHandler eventHandler)
                {
                    // 1．対話型コマンド登録
                    foreach (var handler in handlerProvider.GetCommandHandlers())
                    {
                        _commandHandlers.Add(handler);
                        Log("----------------------------------------------------", LogType.Success);
                        Log($"[対話型コマンド初期化]", LogType.Success);
                        Log($"  > プラグイン名：[{handler.PluginName}]", LogType.Success);
                        Log($"  > ロードされたコマンド名：[!{handler.CommandName}]", LogType.Success);
                        loadedCommandCount++;
                    }
                }
            }
            Log("----------------------------------------------------", LogType.Success);
            Log($"〇：合計[{loadedCommandCount} 個] プラグインコマンドのロードに成功!!", LogType.Success);
            Log($"〇：合計[{initializedEventCount} 個] プラグインDLLのロードに成功!!", LogType.Success);
            Log("----------------------------------------------------", LogType.Success);
        }
        //ログ種別により文字色変更
        /*private void Log(string message, LogType type = LogType.Normal)
        {
            //デバッグログのフィルタリングロジックを追加
            //LogType.Debug または LogType.DebugErrorであり、かつデバッグログが無効(_isDebugLogEnabled == false)の場合は処理を終了
            if ((type == LogType.Debug || type == LogType.DebugError) && !_isDebugLogEnabled)
            {
                return;
            }
            RichTextBox TxtLog = this.Controls.Find("TxtLog", true).FirstOrDefault() as RichTextBox ?? this.TxtLog as RichTextBox;
            if (TxtLog == null) return;
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(message, type)));
                return;
            }
            TxtLog.SelectionStart = TxtLog.TextLength;
            TxtLog.SelectionLength = 0;
            //色を設定
            switch (type)
            {
                case LogType.Error:
                    TxtLog.SelectionColor = System.Drawing.Color.Red;
                    break;
                case LogType.Success:
                    TxtLog.SelectionColor = System.Drawing.Color.Lime;
                    break;
                case LogType.UserMessage:
                    TxtLog.SelectionColor = System.Drawing.Color.Blue;
                    break;
                case LogType.Debug:
                    TxtLog.SelectionColor = System.Drawing.Color.Yellow;
                    break;
                case LogType.Bot:
                    TxtLog.SelectionColor = System.Drawing.Color.Magenta;
                    break;
                case LogType.DebugError:
                    TxtLog.SelectionColor = System.Drawing.Color.Orange;
                    break;
                default:
                    TxtLog.SelectionColor = System.Drawing.Color.White;
                    break;
            }

            TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            //色を元に戻す
            TxtLog.SelectionColor = TxtLog.ForeColor;
            TxtLog.ScrollToCaret();
        }*/
        private void Log(string message, LogType type = LogType.Normal)
        {
            // デバッグログのフィルタリングロジックは維持
            if ((type == LogType.Debug || type == LogType.DebugError) && !_isDebugLogEnabled)
            {
                return;
            }
            //キューに入れる処理
            _logQueue.Enqueue((message, type));
        }
        //UIスレッド上でRichTextBoxにログを安全に書き込む本体処理(デバッグログ種別：INFO, ERROR, SUCCESS, CRITICAL)
        private void WriteLogBatchToRichTextBox(List<(string Message, LogType Type)> batch)
        {
            //RichTextBox TxtLog = null;
            // ログの最大文字数を定義 (例: 10万文字)
            const int MAX_LOG_LENGTH = 100000;
            const int TRIM_LENGTH = 10000;
            // Findによる取得を試みる (これが失敗しやすい可能性がある)
            //TxtLog = this.Controls.Find("TxtLog", true).FirstOrDefault() as RichTextBox;
            RichTextBox TxtLog = this.Controls.Find("TxtLog", true).FirstOrDefault() as RichTextBox ?? this.TxtLog as RichTextBox;
            if (TxtLog == null)
            {
                TxtLog = this.TxtLog as RichTextBox;
            }

            //TxtLogがnullか、既に破棄されているかチェックを追加
            if (TxtLog == null || TxtLog.IsDisposed || batch.Count == 0) return;
            if (TxtLog == null || batch.Count == 0) return;

            //RichTextBoxの更新を一時停止し、チラつきやパフォーマンス低下を防ぐ
            //ロックと SelectionStart/SelectionColor の操作のみで処理
            TxtLog.SuspendLayout();

            try
            {
                foreach (var logEntry in batch)
                {
                    //1．カーソルを末尾に移動 (色の適用を確実にする)
                    TxtLog.SelectionStart = TxtLog.TextLength;
                    TxtLog.SelectionLength = 0;

                    //2．色を設定
                    switch (logEntry.Type)
                    {
                        case LogType.Error:
                            TxtLog.SelectionColor = System.Drawing.Color.Red;
                            break;
                        case LogType.Success:
                            TxtLog.SelectionColor = System.Drawing.Color.Lime;
                            break;
                        case LogType.UserMessage:
                            TxtLog.SelectionColor = System.Drawing.Color.Blue;
                            break;
                        case LogType.Debug:
                            TxtLog.SelectionColor = System.Drawing.Color.Yellow;
                            break;
                        case LogType.Bot:
                            TxtLog.SelectionColor = System.Drawing.Color.Magenta;
                            break;
                        case LogType.DebugError:
                            TxtLog.SelectionColor = System.Drawing.Color.Orange;
                            break;
                        default:
                            TxtLog.SelectionColor = System.Drawing.Color.White;
                            break;
                    }

                    //3．テキストを追加
                    TxtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {logEntry.Message}\r\n");

                    //4．色を元に戻す
                    TxtLog.SelectionColor = TxtLog.ForeColor;
                    //TxtLog.ScrollToCaret();
                }
                //ログが長すぎる場合に古いログを削除
                if (TxtLog.TextLength > MAX_LOG_LENGTH)
                {
                    // 古いログ (先頭から TRIM_LENGTH 分) を削除
                    TxtLog.Select(0, TRIM_LENGTH);
                    TxtLog.SelectedText = "";

                    // 削除後、キャレット位置を末尾に再設定
                    TxtLog.SelectionStart = TxtLog.TextLength;
                    TxtLog.SelectionLength = 0;
                }
                //バッチ処理の最後に一度だけスクロールを実行
                if (TxtLog.TextLength > 0)
                {
                    TxtLog.SelectionStart = TxtLog.TextLength;
                    TxtLog.ScrollToCaret();
                }
            }
            catch (Exception)
            {
                //ログ表示に失敗しても、UIメッセージループの停止を防ぐために例外を捕捉
            }
            finally
            {
                //更新を再開
                TxtLog.ResumeLayout();
            }
        }
        private void FlushLogQueueSynchronously()
        {
            //フォームまたはハンドルが破棄されている場合は処理しない
            if (this.IsDisposed || !this.IsHandleCreated) return;

            var batch = new List<(string Message, LogType Type)>();

            //キューに残っているログをすべて取り出す
            while (_logQueue.TryDequeue(out var logEntry))
            {
                batch.Add(logEntry);
            }

            if (batch.Count > 0)
            {
                //ログをUIスレッドで同期的に書き込む
                WriteLogBatchToRichTextBox(batch);
            }
        }
        //ログキューを監視し、メッセージをUIスレッドにディスパッチするバックグラウンドタスク
        private async Task ProcessLogQueue(CancellationToken token)
        {
            var batch = new List<(string Message, LogType Type)>();

            while (!token.IsCancellationRequested)
            {
                //ログを最大_logBatchSize分、キューから取り出す
                while (batch.Count < _logBatchSize && _logQueue.TryDequeue(out var logEntry))
                {
                    batch.Add(logEntry);
                }

                if (batch.Count > 0)
                {
                    var logsToSend = batch.ToList();
                    batch.Clear();

                    if (!this.IsDisposed && this.IsHandleCreated)
                    {
                        //BeginInvokeでUIスレッドに処理を依頼
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            try
                            {
                                WriteLogBatchToRichTextBox(logsToSend);
                            }
                            catch (Exception)
                            {
                                //ここでエラーが発生した場合、ログ表示は失敗するが、UIメッセージループが継続できるよう、例外を外部に投げないようにする
                            }
                        });
                    }
                }

                //待機時間を制御し、CPU負荷を軽減(_uiTaskDelayMsを参照)
                try
                {
                    await Task.Delay(batch.Count > 0 ? _uiTaskDelayMs : 100, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
        //ログ種別の列挙型
        public enum LogType
        {
            Normal = 0,
            Error = 1,
            Success = 2,
            UserMessage = 3,
            Debug = 4,
            Bot = 5,
            DebugError = 6,
        }
        //フォームクロージング処理
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //1．プログラムからの Close() 呼び出しの場合は、無条件で終了を許可し、処理をスキップ
            if (_isClosingProgrammatically)
            {
                return;
            }

            //2．BOTが稼働中かチェック
            if (IsBotRunning)
            {
                //稼働中の場合の警告メッセージ
                DialogResult warningResult = MessageBox.Show(
                    "DiscordBOTが現在稼働中です!!\nこのまま終了するとBOTが停止します!!\n本当に終了してもよろしいですか?",
                    $"{PGVersion} - BOT稼働中",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (warningResult == DialogResult.No)
                {
                    e.Cancel = true;
                    //ログの停止、フラッシュ、再起動処理を BeginInvoke でメッセージループの後にポストする
                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke((MethodInvoker)delegate
                        {
                            //フォームクローズイベントの処理が完了し、メッセージループが再開した後に実行される
                            StopLogProcessor();

                            StartLogProcessor();

                            //ログが復旧したことを確認するためのログ
                            Log("[INFO] フォーム終了がキャンセルされました!! ログ処理を再起動しました!!", LogType.Debug);
                        });
                    }
                    return; // フォームクローズをキャンセルし、ここで完全にメソッドを終了
                }
                if (warningResult == DialogResult.Yes)
                {
                    //フォームクローズをキャンセル
                    e.Cancel = true;
                    //BOT停止処理と最終確認メッセージを処理するメソッドを呼び出す
                    StopBotAndCloseForm();
                    //BtnStart.Enabled = true;
                    //BtnStop.Enabled = false;
                    return;
                }
            }

            //3．BOT非稼働中の場合の最終確認のメッセージボックス
            DialogResult finalResult = MessageBox.Show(
                "DiscordBOTコンソール(GUI版) を終了しますか?",
                $"{PGVersion}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (finalResult == DialogResult.No)
            {
                e.Cancel = true;
                //ログの停止、フラッシュ、再起動処理を BeginInvoke でメッセージループの後にポストする
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        //フォームクローズイベントの処理が完了し、メッセージループが再開した後に実行される
                        StopLogProcessor();

                        StartLogProcessor();
                        Log("[INFO] フォーム終了がキャンセルされました!! ログ処理を再起動しました!!", LogType.Debug);
                    });
                }
            }
            //BOT非稼働中かつ「はい」の場合
            else
            {
                //設定を保存して通常終了を許可
                SaveFormSettings();
            }
        }

        //BOT停止とフォーム終了を制御する非同期メソッド
        private async void StopBotAndCloseForm()
        {

            //1．BOT停止処理の実行（警告後にすぐに実行される）
            if (IsBotRunning)
            {
                try
                {
                    //BOT停止処理を待機
                    await _client.StopAsync();
                    await _client.LogoutAsync();

                    //クリーンアップ
                    _client?.Dispose();
                    IsBotRunning = false;
                    Log("〇：-------------------- BOT 停止完了!! --------------------", LogType.Bot);
                    if (this.IsHandleCreated)
                    {
                        this.BeginInvoke((MethodInvoker)delegate {
                            BtnStart.Enabled = true;
                            BtnStop.Enabled = false;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log($"BOT停止処理中にエラーが発生しました!!\n{ex.Message}", LogType.Error);
                }
            }
            //3．最終確認メッセージの表示
            DialogResult finalResult = MessageBox.Show(
                "DiscordBOTコンソール(GUI版) を終了しますか?",
                $"{PGVersion}",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (finalResult == DialogResult.Yes)
            {
                StopLogProcessor();
                FlushLogQueueSynchronously();
                //4．アプリ終了
                SaveFormSettings();

                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate {
                        _isClosingProgrammatically = true;
                        this.Close();
                    });
                }
                else
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            else
            {
                //フォーム終了がキャンセルされたため、停止していたログ処理を再開する
                StartLogProcessor();
                Log("[INFO] フォーム終了がキャンセルされました!! ログ処理を再起動しました!!", LogType.Debug);
            }
        }
        //フォーム設定の保存
        private void SaveFormSettings()
        {
            if (Properties.Settings.Default.FormSetting == true)
            {
                //フォームの幅と高さを取得
                int width = this.Width;
                int height = this.Height;
                //フォームの位置を取得
                Point formPosition = this.Location;
                Properties.Settings.Default.MainForm_FormX = width;
                Properties.Settings.Default.MainForm_FormY = height;
                Properties.Settings.Default.MainForm_PositionX = formPosition.X;
                Properties.Settings.Default.MainForm_PositionY = formPosition.Y;
                Properties.Settings.Default.Save();
            }
        }
        //_isDebugLogEnabled の再読み込みを実行
        private void Setting_DebugSettingChanged()
        {
            //UIスレッドで実行する必要があるため、InvokeRequiredを使用して安全に呼び出す
            if (InvokeRequired)
            {
                Invoke(new Action(Setting_DebugSettingChanged));
                return;
            }
            //1．レジストリから新しい設定値を直接読み込む
            bool newDebugState = RegistryHelper.LoadDebugLogEnabledSetting();
            bool oldDebugState = _isDebugLogEnabled;
            try
            {
                //2．ログメッセージの出力（古い値と新しい値を比較して、適切なログレベルで出力）
                if (!oldDebugState && newDebugState)
                {
                    //OFF から ON に変更された場合
                    //新しく ON になったので、デバッグフィルタの影響を受けずに LogType.Normal で通知
                    Log("[INFO] デバッグログ出力が有効になりました!!", (int)LogType.Normal);
                }
                else if (oldDebugState && !newDebugState)
                {
                    //ON から OFF に変更された場合
                    //まだデバッグフィルタが効いているうちに LogType.Normal で通知
                    Log("[INFO] デバッグログ出力が無効になりました!!", (int)LogType.Normal);
                }
                //デバッグログ設定をレジストリから再読み込み
                _isDebugLogEnabled = newDebugState;

                //ログに再読み込みが完了したことを出力
                Log("[INFO] デバッグログ設定を再読み込みしました!!", LogType.Debug);
            }
            catch (Exception ex)
            {
                Log($"[ERROR] デバッグログ設定の再読み込みに失敗しました!!\n{ex.Message}", LogType.Error);
            }
        }
        //設定フォーム
        private void SettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.setting == null || this.setting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.setting = new Setting(new LoggerAdapter(this));
                    //設定フォームのインスタンスに対してイベントを購読する
                    this.setting.DebugSettingChanged += Setting_DebugSettingChanged;
                    //イベントハンドラの登録
                    this.setting.LogPerformanceSettingChanged += UpdateLogPerformanceSettings;
                }
                if (!this.setting.Visible)
                {
                    this.setting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (setting.WindowState == FormWindowState.Minimized)
                    {
                        setting.WindowState = FormWindowState.Normal;
                    }
                    if (setting != null && !setting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        setting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //編集フォーム
        private void EditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.edit == null || this.edit.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.edit = new Edit(new LoggerAdapter(this));
                }
                if (!this.edit.Visible)
                {
                    this.edit.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (edit.WindowState == FormWindowState.Minimized)
                    {
                        edit.WindowState = FormWindowState.Normal;
                    }
                    if (edit != null && !edit.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        edit.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //入退出ログ設定フォーム
        private void JoiningLeavingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.joiningleaving == null || this.joiningleaving.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.joiningleaving = new JoiningLeavingSetting(new LoggerAdapter(this));
                }
                if (!this.joiningleaving.Visible)
                {
                    this.joiningleaving.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (joiningleaving.WindowState == FormWindowState.Minimized)
                    {
                        joiningleaving.WindowState = FormWindowState.Normal;
                    }
                    if (joiningleaving != null && !joiningleaving.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        joiningleaving.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //アンケートQA設定フォーム
        private void QAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.qasetting == null || this.qasetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.qasetting = new QASetting(new LoggerAdapter(this));
                }
                if (!this.qasetting.Visible)
                {
                    this.qasetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (qasetting.WindowState == FormWindowState.Minimized)
                    {
                        qasetting.WindowState = FormWindowState.Normal;
                    }
                    if (qasetting != null && !qasetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        qasetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //お天気プラグイン
        private void WeatherInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. ロード済みのプラグインリスト (_pluginEventHandlers) から
                //    ICacheClearProvider を実装したインスタンスを検索します。
                ICacheClearProvider cacheClearProvider = _pluginEventHandlers
                    .OfType<ICacheClearProvider>()
                    .FirstOrDefault();

                if (cacheClearProvider == null)
                {
                    // プラグインがロードされていないか、ICacheClearProviderを実装していません。
                    Log($"❌：お天気情報プラグイン (ICacheClearProvider) が見つかりません!!", LogType.Error);
                    MessageBox.Show("お天気情報プラグインのキャッシュクリア機能が見つかりませんでした!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //二重起動防止
                if (this.weathersetting == null || this.weathersetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.weathersetting = new WeatherSetting(new LoggerAdapter(this), cacheClearProvider);
                }
                if (!this.weathersetting.Visible)
                {
                    this.weathersetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (weathersetting.WindowState == FormWindowState.Minimized)
                    {
                        weathersetting.WindowState = FormWindowState.Normal;
                    }
                    if (weathersetting != null && !weathersetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        weathersetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //LicenseInfoSetting フォームが閉じられたときに実行
        private void LicenseInfoSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            // ライセンスフォームが閉じられたら、メインフォームのUIを更新
            UpdateMainFormControls(false);

            // イベント購読を解除し、メモリリークを防ぎます
            if (sender is LicenseInfoSetting settingForm)
            {
                settingForm.FormClosed -= LicenseInfoSetting_FormClosed;
            }
        }
        //ライセンス情報設定フォーム
        private void LicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.licenseinfosetting == null || this.licenseinfosetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.licenseinfosetting = new LicenseInfoSetting(new LoggerAdapter(this), PGUniqueVersion, this);
                    //フォームが閉じられたときのイベントを購読
                    //this.licenseinfosetting.FormClosed += LicenseInfoSetting_FormClosed;
                }
                if (!this.licenseinfosetting.Visible)
                {
                    this.licenseinfosetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (licenseinfosetting.WindowState == FormWindowState.Minimized)
                    {
                        licenseinfosetting.WindowState = FormWindowState.Normal;
                    }
                    if (licenseinfosetting != null && !licenseinfosetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        licenseinfosetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ユーザKICK設定フォーム
        private void UserKickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.kicksetting == null || this.kicksetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.kicksetting = new KickSetting(new LoggerAdapter(this), this._client);
                }
                if (!this.kicksetting.Visible)
                {
                    this.kicksetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (kicksetting.WindowState == FormWindowState.Minimized)
                    {
                        kicksetting.WindowState = FormWindowState.Normal;
                    }
                    if (kicksetting != null && !kicksetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        kicksetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Version情報フォーム
        private void VersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.versioninfo == null || this.versioninfo.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.versioninfo = new VersionInfo(new LoggerAdapter(this), this._isLicenseActive);
                }
                if (!this.versioninfo.Visible)
                {
                    this.versioninfo.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (versioninfo.WindowState == FormWindowState.Minimized)
                    {
                        versioninfo.WindowState = FormWindowState.Normal;
                    }
                    if (versioninfo != null && !versioninfo.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        versioninfo.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //メッセージ一括削除
        private void MessageBulkDeletionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.deletesetting == null || this.deletesetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.deletesetting = new DeleteSetting(new LoggerAdapter(this), this._client);
                }
                if (!this.deletesetting.Visible)
                {
                    this.deletesetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (deletesetting.WindowState == FormWindowState.Minimized)
                    {
                        deletesetting.WindowState = FormWindowState.Normal;
                    }
                    if (deletesetting != null && !deletesetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        deletesetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ユーザBAN設定フォーム
        private void UserBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.bansetting == null || this.bansetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.bansetting = new BanSetting(new LoggerAdapter(this), this._client);
                }
                if (!this.bansetting.Visible)
                {
                    this.bansetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (bansetting.WindowState == FormWindowState.Minimized)
                    {
                        bansetting.WindowState = FormWindowState.Normal;
                    }
                    if (bansetting != null && !bansetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        bansetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //ロール自動付与設定
        private void RoleSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //二重起動防止
                if (this.rolesetting == null || this.rolesetting.IsDisposed)
                {
                    //ヌル、または破棄されていたら
                    this.rolesetting = new RoleSetting(new LoggerAdapter(this), this._client);
                }
                if (!this.rolesetting.Visible)
                {
                    this.rolesetting.Show();
                }
                else
                {
                    //フォームが最小化されている時にボタンクリックするとノーマルサイズで前面に表示
                    if (rolesetting.WindowState == FormWindowState.Minimized)
                    {
                        rolesetting.WindowState = FormWindowState.Normal;
                    }
                    if (rolesetting != null && !rolesetting.IsDisposed)
                    {
                        //フォームが既に表示されている場合、アクティベート
                        rolesetting.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                string ErrorType = ex.GetType().Name;
                string ErrorMessage = ex.Message;
                MessageBox.Show("※" + ErrorMessage, ErrorType, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

using Discord;
using DiscordBot.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class LicenseInfoSetting : Form
    {
        private readonly string ExpectedAppId;
        //メインフォームのインスタンスを保持するフィールド
        private readonly MainForm _mainForm;
        //ToolTip コンポーネントのインスタンスをフィールドとして宣言
        private ToolTip dllToolTip;

        //3．現在ツールチップが表示されている項目インデックスを保持
        private int lastIndex = -1;
        public static class EncryptionHelper
        {
            //32バイトのキー(AES-256用)ハッシュ化します
            private static readonly string encryptionKeyBase = "DiscordBotGUI-1234567890123456";

            //実際の暗号化キーを生成するヘルパーメソッド
            private static byte[] GetEncryptionKeyBytes()
            {
                //SHA256ハッシュ関数は、入力に関わらず常に32バイト（256ビット）の出力を生成します。
                using (SHA256 sha256 = SHA256.Create())
                {
                    //ベース文字列をUTF8バイト配列に変換し、ハッシュ化
                    return sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKeyBase));
                }
            }

            public static string Encrypt(string plainText)
            {
                //ゼロパディングされたIVを使用するEncryptメソッドのコード
                using (Aes aesAlg = Aes.Create())
                {
                    //GetEncryptionKeyBytes() を呼び出して32バイトのキーを取得
                    aesAlg.Key = GetEncryptionKeyBytes();
                    aesAlg.IV = new byte[16];

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                            string encryptedBase64 = Convert.ToBase64String(msEncrypt.ToArray());
                            return AddLineBreaks(encryptedBase64, 256);
                        }
                    }
                }
            }

            //Decrypt メソッド
            public static string Decrypt(string cipherText)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    //GetEncryptionKeyBytes() を呼び出して32バイトのキーを取得
                    aesAlg.Key = GetEncryptionKeyBytes();
                    aesAlg.IV = new byte[16];
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    string cleanCipherText = cipherText.Replace("\n", "").Replace("\r", "");
                    byte[] cipherBytes = Convert.FromBase64String(cleanCipherText);

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

            private static string AddLineBreaks(string input, int lineLength)
            {
                //AddLineBreaks の実装
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < input.Length; i += lineLength)
                {
                    if (i + lineLength < input.Length)
                        sb.AppendLine(input.Substring(i, lineLength));
                    else
                        sb.Append(input.Substring(i));
                }
                return sb.ToString().TrimEnd();
            }
        }

        //ILoggerを保持するためのプライベートフィールド
        private static ILogger _logger;
        private const string PluginDictionaryFileName = "DLLPlugin.Dictionary";
        private const string PluginDictionaryExplanation = "DLLPluginExplanation.Dictionary";
        private List<string> _allowedPlugins = new List<string>();
        //DLLPlugin.Dictionaryの情報(DLLファイル名 => 表示名)を保持
        private Dictionary<string, string> _dllToDisplayNameMap = new Dictionary<string, string>();
        //DLL名と説明文をマッピングする辞書(表示名 => 説明文)を保持
        private Dictionary<string, string> _dllToExplanationMap = new Dictionary<string, string>();
        //アクティベートフラグ
        private bool isActivated = false;
        public LicenseInfoSetting(ILogger logger, string appUniqueVersion, MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            //APPID
            ExpectedAppId = appUniqueVersion;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[LicenseInfoSetting] Loggerインスタンスはnullにできません!!");
            _logger.Log("-------------------- [LicenseInfoSetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //フォームロードメソッド
        private void LicenseInfoSetting_Load(object sender, EventArgs e)
        {
            _logger.Log($"[INFO] LicenseInfoSetting_Loadイベントを開始!!", (int)LogType.Debug);
            //HWIDの取得
            string currentHwid = GetHardwareId();
            TxtHwId.Text = currentHwid;
            _logger.Log($"[INFO] 取得したHWIDをTxtHwIdに設定：[{currentHwid}]", (int)LogType.Debug);
            //ToolTip コンポーネントを初期化
            dllToolTip = new ToolTip();
            //ツールチップが表示されるまでの遅延時間を設定 (例: 500ミリ秒)
            dllToolTip.InitialDelay = 500;
            dllToolTip.ShowAlways = true;
            _logger.Log($"[INFO] ToolTipコンポーネントを初期化完了!!", (int)LogType.Debug);
            //CheckedListBox のイベントを購読
            ClbPlugins.MouseMove += ClbPlugins_MouseMove;
            //LoadDllPluginDictionary();
            //フォームの初期化後にコントロールの状態を更新
            _logger.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateActivationControlState();
            _logger.Log($"[INFO] LicenseInfoSetting_Loadイベントを終了!!", (int)LogType.Debug);
        }
        //DLLPlugin.Dictionaryロードメソッド
        private void LoadDllPluginDictionary()
        {
            _logger?.Log($"[INFO] LoadDllPluginDictionaryメソッドを開始!!", (int)LogType.Debug);
            string filePath1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginDictionaryFileName);
            string filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginDictionaryExplanation);
            _logger?.Log($"[INFO] ファイルパス1 (表示名)：[{filePath1}], ファイルパス2 (説明文)：[{filePath2}]", (int)LogType.Debug);
            if (File.Exists(filePath1))
            {
                _logger?.Log($"[INFO] DLLPlugin.Dictionaryファイル1 (表示名) が見つかりました!! 読み込みを開始!!", (int)LogType.Debug);
                try
                {
                    string json1 = File.ReadAllText(filePath1);
                    //_dllToDisplayNameMap にデシリアライズ
                    //キー：DLLファイル名, 値：表示名
                    _dllToDisplayNameMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(json1) ?? new Dictionary<string, string>();
                    _logger?.Log($"[INFO] _dllToDisplayNameMap に [{_dllToDisplayNameMap.Count} 件]の表示名をロードしました!!", (int)LogType.Debug);
                    //CheckedListBoxのアイテムをDLLPlugin.Dictionaryの「表示名」でロード
                    ClbPlugins.Items.Clear();
                    foreach (var displayName in _dllToDisplayNameMap.Values)
                    {
                        ClbPlugins.Items.Add(displayName);
                    }
                    _logger?.Log($"[INFO] ClbPluginsに [{_dllToDisplayNameMap.Count} 件]のアイテムを追加しました!!", (int)LogType.Debug);
                }
                catch (Exception ex)
                {
                    _logger?.Log($"[ERROR] DLLPlugin.Dictionary (表示名) の読み込み・デシリアライズに失敗しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                    _dllToDisplayNameMap = new Dictionary<string, string>();
                }
            }
            else
            {
                _logger?.Log("[ERROR] DLLPlugin.Dictionary (表示名) が見つかりません!! _dllToDisplayNameMapは空で初期化されます!!", (int)LogType.DebugError);
                _dllToDisplayNameMap = new Dictionary<string, string>();
            }
            if (File.Exists(filePath2))
            {
                _logger?.Log($"[INFO] DLLPlugin.Dictionaryファイル2 (説明文) が見つかりました!! 読み込みを開始!!", (int)LogType.Debug);
                try
                {
                    string json2 = File.ReadAllText(filePath2);
                    //_dllToExplanationMap にデシリアライズ
                    //キー：DLLプラグイン名, 値：説明文
                    _dllToExplanationMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(json2) ?? new Dictionary<string, string>();
                    _logger?.Log($"[INFO] _dllToExplanationMap に [{_dllToExplanationMap.Count} 件]の説明文をロードしました!!", (int)LogType.Debug);
                }
                catch (Exception ex)
                {
                    _logger?.Log($"[ERROR] DLLPlugin.Dictionary (説明文) の読み込み・デシリアライズに失敗しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                    _dllToExplanationMap = new Dictionary<string, string>();
                }
            }
            else
            {
                //ファイルが見つからなかった場合のログを追加
                _logger?.Log("[ERROR] DLLPlugin.Dictionary (説明文) が見つかりません!! _dllToExplanationMapは空で初期化されます!!", (int)LogType.DebugError);
                _dllToExplanationMap = new Dictionary<string, string>();
            }

            _logger?.Log($"[INFO] LoadDllPluginDictionaryメソッドを終了!!", (int)LogType.Debug);
        }
        //アクティベートボタンとテキストボックスの有効/無効
        private void UpdateActivationControlState()
        {
            _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを開始!!", (int)LogType.Debug);
            //レジストリに暗号化されたライセンス情報があるかチェック
            string encryptedContent = DiscordBot.Core.RegistryHelper.LoadEncryptedLicense();
            _logger?.Log($"[INFO] 暗号化コンテンツを取得しました!! 長さ：[{encryptedContent?.Length ?? 0}]", (int)LogType.Debug);
            //ライセンス情報が空でなければ（登録されていれば）true
            bool isLicenseRegistered = !string.IsNullOrEmpty(encryptedContent);
            _logger?.Log($"[INFO] ライセンス登録状態：isLicenseRegistered=[{isLicenseRegistered}]", (int)LogType.Debug);
            //アクティベーションフラグの確認
            bool isActivated = DiscordBot.Core.RegistryHelper.GetLicenseActivationFlag();
            _logger?.Log($"[INFO] アクティベートフラグ：isActivated=[{isActivated}]", (int)LogType.Debug);
            //有効期限切れフラグ
            bool isExpired = false;

            //TxtLicenseKeyとBtnActivateの有効/無効を設定
            //デザイナで配置されているコントロールの存在を前提とする
            TxtLicenseKey.Enabled = isLicenseRegistered;
            BtnActivate.Enabled = isLicenseRegistered;
            _logger?.Log($"[INFO] 初期コントロール状態設定：TxtLicenseKey.Enabled=[{isLicenseRegistered}], BtnActivate.Enabled=[{isLicenseRegistered}]", (int)LogType.Debug);
            //DLLPlugin.Dictionaryの読み込みが成功していることを前提とする
            if (_dllToDisplayNameMap.Count == 0)
            {
                _logger?.Log($"[INFO] _dllToDisplayNameMapが空です!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] LoadDllPluginDictionaryメソッドを呼び出し!!", (int)LogType.Debug);
                LoadDllPluginDictionary();
            }

            if (isLicenseRegistered)
            {
                _logger?.Log($"[INFO] ライセンスが登録されているため、復号化と検証を開始!!", (int)LogType.Debug);
                try
                {
                    //登録されているライセンスを復号化
                    string decryptedJson = EncryptionHelper.Decrypt(encryptedContent);
                    _logger?.Log($"[INFO] ライセンス情報の復号化に成功!!", (int)LogType.Debug);
                    var licenseRoot = JsonConvert.DeserializeObject<DiscordBot.Core.LicenseFileRoot>(decryptedJson);

                    if (licenseRoot != null && licenseRoot.LicenseInfos.Count > 0)
                    {
                        var licenseInfo = licenseRoot.LicenseInfos[0];
                        _logger?.Log($"[INFO] ライセンス情報 (LicenseInfos[0]) のデシリアライズに成功!!", (int)LogType.Debug);
                        //有効期限チェックのロジック
                        //JSONの "DateTime" キーに対応するプロパティから文字列を取得
                        string expiryDateString = licenseInfo.ExpiryDateTime;
                        _logger?.Log($"[INFO] 期限日文字列：[{expiryDateString}]", (int)LogType.Debug);
                        if (DateTime.TryParse(expiryDateString, out DateTime expiryDate))
                        {
                            //期限の日付の翌日になったら期限切れと判定
                            if (expiryDate.Date < DateTime.Now.Date)
                            {
                                isExpired = true;
                                TxtLicenseKey.Enabled = !isExpired;
                                BtnActivate.Enabled = !isExpired;
                                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(!isExpired);
                                _logger?.Log($"[ERROR] ライセンス有効期限が切れています!! isExpired=[{isExpired}]", (int)LogType.DebugError);
                                _logger?.Log($"[ERROR] 有効期限：[{expiryDate.ToShortDateString()}]", (int)LogType.DebugError);
                            }
                            else
                            {
                                _logger?.Log($"[SUCCESS] ライセンスは有効期限内です!! 期限日：[{expiryDate.ToShortDateString()}]", (int)LogType.Debug);
                            }
                        }
                        else
                        {
                            _logger?.Log($"[ERROR] 有効期限文字列：[{expiryDateString}] の解析に失敗!!", (int)LogType.DebugError);
                        }

                        //許可されたDLL名をクラスフィールドに保存
                        _allowedPlugins = licenseInfo.AllowedPlugins;
                        var allowedPlugins = licenseInfo.AllowedPlugins;
                        //プラグイン一覧タブに情報を表示（リストのアイテムをロード）
                        //DisplayAllowedPlugins(allowedPlugins);

                        //アクティベート状態に基づいてチェック状態を設定
                        //一次登録ではチェックは入らず、アクティベートでチェックが入る
                        _logger?.Log($"[INFO] SetPluginsCheckedStateメソッドを呼び出し!!", (int)LogType.Debug);
                        SetPluginsCheckedState(isActivated);

                        //アクティベート状態に基づいて CheckedListBox の文字色を設定
                        //未アクティベート時(チェックなし)は灰色、アクティベート時(チェックあり)は黒
                        ClbPlugins.ForeColor = isActivated ? SystemColors.WindowText : SystemColors.GrayText;
                        _logger?.Log($"[INFO] プラグイン一覧をロードしました!! アイテム数：[{_allowedPlugins.Count} 個], CheckedStateを設定しました!!", (int)LogType.Debug);
                    }
                    else
                    {
                        _logger?.Log($"[ERROR] ライセンスデータの復号化に成功しましたが、内容が無効です!! (nullまたはLicenseInfosが空)", (int)LogType.DebugError);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Log($"[CRITICAL] ライセンス情報ロード失敗!! (復号化またはデシリアライズエラー)\n例外：{ex.Message}", (int)LogType.DebugError);
                    //例外発生時は安全のため、期限切れと同じ扱いにする
                    isExpired = true;
                    TxtLicenseKey.Enabled = !isExpired;
                    BtnActivate.Enabled = !isExpired;
                    DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(!isExpired);
                    _logger?.Log($"[INFO] 例外ハンドリング完了：isExpiredをtrueに設定し、コントロールを無効化しました!!", (int)LogType.Debug);
                    //失敗時はプラグイン一覧をクリア
                    if (_allowedPlugins != null)
                    {
                        _allowedPlugins.Clear();
                        ClbPlugins.Items.Clear();
                    }
                }
            }
            //isLicenseRegistered == false
            else
            {
                _logger?.Log($"[INFO] ライセンスが登録されていません!! 未登録処理を実行します!!", (int)LogType.Debug);
                TxtLicenseKey.Enabled = !isExpired;
                BtnActivate.Enabled = !isExpired;
                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(!isExpired);
                //ライセンスが未登録の場合、プラグイン一覧もクリア
                if (_allowedPlugins != null)
                {
                    _allowedPlugins.Clear();
                    ClbPlugins.Items.Clear();
                }
            }

            //LblRegister のテキストと色を更新するロジック
            if (LblRegister != null)
            {
                //認証フラグが立っている AND 期限切れではない場合のみ「認証されています」
                if (isActivated && !isExpired)
                {
                    LblRegister.Text = "ライセンス認証されています!!";
                    LblRegister.ForeColor = System.Drawing.Color.Green;
                    _logger?.Log($"[INFO] LblRegisterを「認証されています!!」(Green)に設定!!", (int)LogType.Debug);
                }
                else
                {
                    //期限切れ OR 未認証 OR ライセンスファイル破損の場合、すべて「認証されていません!!」と表示
                    LblRegister.Text = "ライセンス認証されていません!!";
                    LblRegister.ForeColor = System.Drawing.Color.Red;
                    TxtLicenseKey.Enabled = !isExpired;
                    BtnActivate.Enabled = !isExpired;
                    DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(!isExpired);
                    _logger?.Log($"[INFO] LblRegisterを「認証されていません!!」(Red)に設定!! isActivated=[{isActivated}], isExpired=[{isExpired}]", (int)LogType.Debug);
                }
            }
            _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを終了!!", (int)LogType.Debug);
        }
        //認証リクエスト用のデータ構造
        private class AuthRequestInfo
        {
            [JsonProperty("HardwareId")]
            public string HardwareId { get; set; }

            [JsonProperty("AppId")]
            public string AppId { get; set; }

            [JsonProperty("RequestDateTime")]
            public string RequestDateTime { get; set; }
        }
        //マザーボードのシリアル番号のみを使用してHWIDを生成
        private string GetHardwareId()
        {
            _logger?.Log("[INFO] GetHardwareIdメソッドを開始!!", (int)LogType.Debug);
            try
            {
                _logger?.Log("[INFO] Win32_BaseBoardからSerialNumberの取得を試行!!", (int)LogType.Debug);
                //1．マザーボードのシリアル番号を取得
                _logger?.Log($"[INFO] GetManagementPropertyメソッドを呼び出し!!", (int)LogType.Debug);
                string motherBoardSerial = GetManagementProperty("Win32_BaseBoard", "SerialNumber");
                _logger?.Log($"[INFO] 取得された生シリアル番号：[{motherBoardSerial}]", (int)LogType.Debug);
                //2．値が取得できなかった場合のチェック
                if (string.IsNullOrWhiteSpace(motherBoardSerial) || motherBoardSerial.Contains("NO_MB_SERIAL"))
                {
                    _logger?.Log("[ERROR] シリアル番号が空または無効な値 ('NO_MB_SERIAL') のため、例外をスローします!!", (int)LogType.DebugError);
                    throw new InvalidOperationException("マザーボードのシリアル番号を取得できませんでした!!");
                }
                _logger?.Log("[INFO] シリアル番号のSHA256ハッシュ化を開始!!", (int)LogType.Debug);
                //3．SHA256でハッシュ化し、32バイトのHWIDとして利用(64文字の文字列)
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(motherBoardSerial));
                    string hwid = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    _logger?.Log($"[SUCCESS] HWIDの生成に成功しました!! 生成されたHWIDの長さ：[{hwid.Length}]", (int)LogType.Debug);
                    _logger?.Log("[INFO] GetHardwareIdメソッドを終了!! (正常終了)", (int)LogType.Debug);
                    return hwid;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HWID取得エラー!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log($"[ERROR] HWID取得エラー!!\n例外：{ex.Message}", (int)LogType.DebugError);

                //失敗時は固定のエラー文字列を返す (エラー判定用に "ERROR" を含む)
                string fallbackHwid = "HWID_GENERATION_ERROR_FALLBACK_FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
                _logger?.Log($"[CRITICAL] HWID取得に失敗したため、フォールバック値：[{fallbackHwid}] を返します!!", (int)LogType.DebugError);
                _logger?.Log("[INFO] GetHardwareIdメソッドを終了!! (異常終了)", (int)LogType.Debug);
                return fallbackHwid;
            }
        }
        //指定されたWMIクラスからプロパティ値を取得
        private static string GetManagementProperty(string className, string propertyName)
        {
            //メソッド開始ログ
            _logger?.Log($"[INFO] GetManagementPropertyメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] WMIクラス：[{className}], プロパティ：[{propertyName}]", (int)LogType.Debug);
            string result = string.Empty;
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT {propertyName} FROM {className}"))
            {
                _logger?.Log($"[INFO] WMIクエリ実行!! SELECT：[{propertyName}], FROM：[{className}]", (int)LogType.Debug);
                foreach (ManagementObject managementObject in searcher.Get())
                {
                    if (managementObject[propertyName] != null)
                    {
                        //値を取得し、トリムして保存
                        result = managementObject[propertyName].ToString().Trim();
                        _logger?.Log($"[INFO] WMIプロパティ値の取得に成功!! 結果(Trim済み)：[{result}]", (int)LogType.Debug);
                        break;
                    }
                    _logger?.Log($"[INFO] 現在のManagementObjectのプロパティ：[{propertyName}]はnullでした!! 次のオブジェクトをチェック!!", (int)LogType.Debug);
                }
            }
            //最終結果の判定
            string finalResult = string.IsNullOrWhiteSpace(result) ? "NO_MB_SERIAL" : result;
            _logger?.Log($"[INFO] 返却値：[{finalResult}]", (int)LogType.Debug);
            //最終結果とメソッド終了ログ
            _logger?.Log($"[INFO] GetManagementPropertyメソッドを終了!!", (int)LogType.Debug);
            return finalResult;
        }
        //フォルダ選択ダイアログ表示
        private string SelectLicenseFile(System.IntPtr parentHandle)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                //1．ファイルのみ選択可能にする
                dialog.IsFolderPicker = false;

                //2．初期ディレクトリ
                //ユーザーのデスクトップなど、アクセスしやすい場所を設定
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                //3．ネットワークフォルダを許可
                dialog.AllowNonFileSystemItems = true;

                //4．タイトルをライセンスファイル選択用に修正
                dialog.Title = "Licenseファイルを選択してください!!";

                //5．フィルタを .License ファイルに修正
                dialog.Filters.Clear();
                dialog.Filters.Add(new CommonFileDialogFilter("License File", "*.License"));
                dialog.Filters.Add(new CommonFileDialogFilter("すべてのファイル", "*.*"));

                if (dialog.ShowDialog(parentHandle) == CommonFileDialogResult.Ok)
                {
                    //選択したファイルの完全パスを返す
                    return dialog.FileName;
                }
            }
            return null;
        }
        //ファイル選択ボタン
        private void BtnSelectLicenseFile_Click(object sender, EventArgs e)
        {
            string filePath = SelectLicenseFile(this.Handle);

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                //選択されたファイルパスをTextBoxに設定
                TxtLicenseFilePath.Text = filePath;
            }
        }
        //ライセンス認証ボタン
        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            _logger?.Log($"[INFO] BtnRegister_Clickイベントを開始!!", (int)LogType.Debug);
            string filePath = TxtLicenseFilePath.Text.Trim();
            _logger?.Log($"[INFO] 入力されたライセンスファイルパス：[{filePath}]", (int)LogType.Debug);
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                _logger?.Log("[ERROR] ライセンスファイルのパスが無効、またはファイルが見つかりません!!", (int)LogType.DebugError);
                MessageBox.Show("ライセンスファイルのパスを確認してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateActivationControlState();
                _logger?.Log($"[INFO] BtnRegister_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            try
            {
                string encryptedContent = File.ReadAllText(filePath, Encoding.UTF8);
                _logger?.Log($"[INFO] ライセンスファイルを読み込みました!! コンテンツ長：[{encryptedContent.Length}]", (int)LogType.Debug);
                string currentHwid = GetHardwareId();
                _logger?.Log($"[INFO] 現在のHWID：[{currentHwid.Substring(0, 8)}...]", (int)LogType.Debug);
                //1．ライセンスファイルの内容を復号化
                _logger?.Log("[INFO] ライセンスコンテンツの復号化を開始!!", (int)LogType.Debug);
                string decryptedJson = EncryptionHelper.Decrypt(encryptedContent);
                _logger?.Log("[INFO] 復号化に成功しました!!", (int)LogType.Debug);
                //2．JSONを LicenseFileRoot オブジェクトにデシリアライズ
                _logger?.Log("[INFO] JSONデシリアライズを開始!!", (int)LogType.Debug);
                var licenseRoot = JsonConvert.DeserializeObject<DiscordBot.Core.LicenseFileRoot>(decryptedJson);

                if (licenseRoot == null || licenseRoot.LicenseInfos == null || licenseRoot.LicenseInfos.Count == 0)
                {
                    _logger?.Log("[ERROR] ライセンスファイルのデシリアライズは成功したが、JSON構造が不正です!!", (int)LogType.DebugError);
                    MessageBox.Show("ライセンスファイルが不正な形式です!!", "登録エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(false);
                    _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                    UpdateActivationControlState();
                    _logger?.Log($"[INFO] BtnRegister_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }

                //3．最初のライセンス情報を取り出して検証 (複数のライセンスが含まれる場合も最初のものを使用)
                var licenseInfo = licenseRoot.LicenseInfos[0];
                _logger?.Log("[INFO] ライセンス情報を取り出し、検証を開始!!", (int)LogType.Debug);
                bool isHwidMatch = licenseInfo.HardwareId.Equals(currentHwid, StringComparison.OrdinalIgnoreCase);
                bool isAppIdMatch = licenseInfo.AppId.Equals(ExpectedAppId, StringComparison.OrdinalIgnoreCase);
                _logger?.Log($"[INFO] 検証結果：HWID一致=[{isHwidMatch}], AppID一致=[{isAppIdMatch}]", (int)LogType.Debug);
                if (!isHwidMatch)
                {
                    _logger?.Log($"[ERROR] HWID不一致!! Current：[{currentHwid.Substring(0, 8)}...], File：[{licenseInfo.HardwareId.Substring(0, 8)}...]", (int)LogType.DebugError);
                    MessageBox.Show($"このライセンスは別のPC用に発行されています!!\n" + $"・現在のHWID：[{currentHwid.Substring(0, 8)}]\n" + $"・ファイル内のHWID：[{licenseInfo.HardwareId.Substring(0, 8)}]", "登録失敗(HWID不一致)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                    UpdateActivationControlState();
                    _logger?.Log($"[INFO] BtnRegister_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }

                if (!isAppIdMatch)
                {
                    _logger?.Log($"[ERROR] AppID不一致!! Expected：[{ExpectedAppId}], File：[{licenseInfo.AppId}]", (int)LogType.DebugError);
                    MessageBox.Show("このライセンスは別のアプリケーション用に発行されています!!", "登録失敗(AppID不一致)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                    UpdateActivationControlState();
                    _logger?.Log($"[INFO] BtnRegister_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }
                //有効期限のチェック(年月日での判断と表示)
                _logger?.Log($"[INFO] 有効期限チェックを開始!! ファイル内日時：[{licenseInfo.ExpiryDateTime}]", (int)LogType.Debug);
                if (DateTime.TryParse(licenseInfo.ExpiryDateTime, out DateTime expiryDate))
                {
                    //ライセンスファイルの日付部分(時刻を00:00:00に設定)
                    DateTime licenseExpiryDateOnly = expiryDate.Date;

                    //現在の日付部分を取得(時刻を00:00:00に設定)
                    DateTime todayUtcDateOnly = DateTime.Now.Date;
                    _logger?.Log($"[INFO] 期限日(DateOnly)：[{licenseExpiryDateOnly.ToShortDateString()}], 本日日付(DateOnly)：[{todayUtcDateOnly.ToShortDateString()}]", (int)LogType.Debug);
                    //期限切れの判断:：有効期限の日付が「今日(日付)」よりも過去である場合
                    if (licenseExpiryDateOnly < todayUtcDateOnly)
                    {
                        //日本語の年月日形式で表示(ローカルタイムに変換)
                        string displayDate = licenseExpiryDateOnly.ToLocalTime().ToString("yyyy年M月d日");
                        _logger?.Log($"[ERROR] ライセンス期限切れ!! 期限：[{displayDate}]", (int)LogType.DebugError);
                        MessageBox.Show($"このライセンスは既に有効期限が切れています!!\n期限：[{displayDate}]", "登録失敗(期限切れ)", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        //期限切れの場合はレジストリに保存せず、処理を終了
                        DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(false);
                        _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                        UpdateActivationControlState();
                        _logger?.Log($"[INFO] BtnRegister_Clickイベントを終了!!", (int)LogType.Debug);
                        return;
                    }
                    _logger?.Log("[SUCCESS] 有効期限チェックを通過しました!!", (int)LogType.Debug);
                }
                else
                {
                    _logger?.Log($"[WARNING] 有効期限文字列：[{licenseInfo.ExpiryDateTime}] の解析に失敗しましたが、登録処理を続行します!!", (int)LogType.Debug);
                }
                //4．レジストリに暗号化されたコンテンツを保存
                _logger?.Log("[INFO] 検証成功!! 暗号化されたコンテンツを保存!!", (int)LogType.Debug);
                DiscordBot.Core.RegistryHelper.SaveEncryptedLicense(encryptedContent);

                //5．アクティベーション状態をリセット(既存のアクティベーションを無効化)
                _logger?.Log("[INFO] アクティベーションフラグを false にリセット!!", (int)LogType.Debug);
                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(false);
                _logger?.Log($"[INFO] LoadDllPluginDictionaryメソッドを呼び出し!!", (int)LogType.Debug);
                LoadDllPluginDictionary();
                // TxtLicenseFilePath.Enabled = false;
                _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateActivationControlState();
                //ライセンスファイル登録成功時にプラグイン一覧を更新
                //DisplayAllowedPlugins(licenseInfo.AllowedPlugins);
                _logger?.Log("[SUCCESS] ライセンス情報のレジストリ保存が完了しました!! ユーザーにアクティベートを促します!!", (int)LogType.Debug);
                MessageBox.Show("ライセンス情報を保存しました!!\n次に、ライセンスキーを入力し、「アクティベート」ボタンをクリックしてください!!", "ライセンス登録完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //念のためnullチェック
                if (_mainForm != null)
                {
                    _logger?.Log("[INFO] _mainFormに対し、ライセンス状態リセットとBOT停止ロジック実行!!", (int)LogType.Debug);
                    _logger?.Log($"[INFO] _mainForm.SetLicenseActiveStateメソッドを呼び出し!!", (int)LogType.Debug);
                    _mainForm.SetLicenseActiveState(false);
                    //メインフォームの公開メソッドを非同期で呼び出す
                    _logger?.Log($"[INFO] _mainForm.StopBotLogicAsyncメソッドを呼び出し!!", (int)LogType.Debug);
                    await _mainForm.StopBotLogicAsync();
                }

            }
            catch (CryptographicException)
            {
                _logger?.Log("[FATAL] 復号化に失敗!! ライセンスファイルの暗号化キーが異なるか、ファイルが破損しています!!", (int)LogType.DebugError);
                MessageBox.Show("ライセンスファイルの暗号化キーが異なります!!またはファイルが破損しています!!", "登録エラー(復号化失敗)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(false);
                _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateActivationControlState();
            }
            catch (JsonException)
            {
                _logger?.Log("[FATAL] JSONデシリアライズ失敗!! ライセンスファイルの内容が不正な形式です!!", (int)LogType.DebugError);
                MessageBox.Show("ライセンスファイルの内容が不正な形式です!!(JSONデシリアライズ失敗)", "登録エラー(形式不正)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(false);
                _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateActivationControlState();
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] その他のエラー発生!!\n{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"ライセンスファイルの読み込みまたは保存中にエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(false);
                _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateActivationControlState();
            }
            _logger?.Log($"[INFO] BtnRegister_Clickイベントを終了!!", (int)LogType.Debug);
        }

        //CheckedListBoxに表示されているすべてのプラグインのチェック状態を一括で設定
        private void SetPluginsCheckedState(bool isChecked)
        {
            _logger?.Log($"[INFO] SetPluginsCheckedStateメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] isChecked=[{isChecked}]", (int)LogType.Debug);
            ClbPlugins.ItemCheck -= ClbPlugins_ItemCheck;
            _logger?.Log("[INFO] ClbPlugins_ItemCheckイベントを一時的に購読解除しました!!", (int)LogType.Debug);
            //値(表示名)をキーにする(kv => kv.Value)
            //キー(DLLファイル名)を値にする(kv => kv.Key)
            var displayNameToDllMap = _dllToDisplayNameMap.ToDictionary(kv => kv.Value, kv => kv.Key);
            _logger?.Log($"[INFO] DLL表示名からファイル名へのマップを作成しました!! アイテム数：[{displayNameToDllMap.Count}]", (int)LogType.Debug);
            for (int i = 0; i < ClbPlugins.Items.Count; i++)
            {
                //ClbPlugins.Items には DLLPlugin.Dictionary の「表示名」が入っている
                string displayPluginName = ClbPlugins.Items[i].ToString();
                bool shouldBeChecked = false;

                if (isChecked)
                {
                    //1．表示名から対応するDLLファイル名を取得
                    if (displayNameToDllMap.TryGetValue(displayPluginName, out string dllFileName))
                    {
                        //2．DLLファイル名がライセンスの許可リストに含まれているか確認
                        //_allowedPlugins にはライセンスファイルから読み込んだDLLファイル名が入っている
                        if (_allowedPlugins.Contains(dllFileName))
                        {
                            shouldBeChecked = true;
                            _logger?.Log($"[INFO] プラグイン名：[{displayPluginName}], DLL：[{dllFileName}] が許可リストに含まれているため、チェックをONに設定!!", (int)LogType.Debug);
                        }
                    }
                    else
                    {
                        //マップにエントリがない場合は警告
                        _logger?.Log($"[WARNING] CheckedListBoxのアイテム [{displayPluginName}] に対応するDLLファイル名がマップに見つかりませんでした!! チェックはOFFになります!!", (int)LogType.Debug);
                    }
                }
                //CheckedListBox の SetItemChecked メソッドを使用して状態を設定
                ClbPlugins.SetItemChecked(i, shouldBeChecked);
            }

            //ItemCheckイベントを再度有効化
            ClbPlugins.ItemCheck += ClbPlugins_ItemCheck;
            _logger?.Log("[INFO] ClbPlugins_ItemCheckイベントを再度購読しました!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] プラグイン一覧のチェック状態を [{(isChecked ? "ON" : "OFF")}] に設定しました!!", (int)LogType.Debug);
            _logger?.Log("[INFO] SetPluginsCheckedStateメソッドを終了!!", (int)LogType.Debug);
        }
        //認証キー作成ボタン
        private void BtnGenerateAuthRequest_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnGenerateAuthRequest_Clickイベントを開始!!", (int)LogType.Debug);
            try
            {
                string currentHwid = GetHardwareId();
                _logger?.Log($"[INFO] GetHardwareIdからHWIDを取得：[{currentHwid.Substring(0, 8)}...]", (int)LogType.Debug);
                //GetHardwareIdが失敗した場合は、エラーメッセージを含む文字列を返す
                if (currentHwid.Contains("ERROR"))
                {
                    _logger?.Log("[ERROR] HWID取得がエラー値を返しました!! ユーザーに警告します!!", (int)LogType.DebugError);
                    MessageBox.Show($"ハードウェアIDの取得に失敗しました!!\nアプリケーションを管理者権限で実行しているか確認してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _logger?.Log("[INFO] BtnGenerateAuthRequest_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }
                _logger?.Log("[INFO] 認証リクエストデータ構造の構築を開始!!", (int)LogType.Debug);
                //1．データ構造の構築
                var authRequest = new AuthRequestInfo
                {
                    //現在のPCのHWID
                    HardwareId = currentHwid,
                    //ハードコードされたAppID
                    AppId = ExpectedAppId,
                    //リクエスト日時
                    RequestDateTime = DateTime.Now.Date.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };
                _logger?.Log($"[INFO] AuthRequestInfo構築完了!! AppId=[{authRequest.AppId}], DateTime=[{authRequest.RequestDateTime}]", (int)LogType.Debug);
                //2．JSONへのシリアライズ
                _logger?.Log("[INFO] JSONへのシリアライズを開始!!", (int)LogType.Debug);
                string json = JsonConvert.SerializeObject(authRequest, Formatting.Indented);
                _logger?.Log($"[INFO] JSONシリアライズに成功しました!! シリアライズされたJSONの長さ：[{json.Length}]", (int)LogType.Debug);
                //3．JSON文字列の暗号化
                _logger?.Log("[INFO] JSON文字列の暗号化を開始!!", (int)LogType.Debug);
                string encryptedContent = EncryptionHelper.Encrypt(json);
                _logger?.Log($"[INFO] 暗号化に成功しました!! 暗号化コンテンツの長さ：[{encryptedContent.Length}]", (int)LogType.Debug);
                //4．ファイルパスの決定と保存ダイアログの表示
                string hwidPrefix = currentHwid.Length >= 8 ? currentHwid.Substring(0, 8) : currentHwid;
                string defaultFileName = $"AuthReq_{hwidPrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.Auth";

                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Auth Request File (*.Auth)|*.Auth";
                    saveDialog.Title = "認証リクエストファイルの保存場所を選択!!";
                    saveDialog.FileName = defaultFileName;
                    saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    _logger?.Log($"[INFO] SaveFileDialogを表示します!! 初期ファイル名：[{defaultFileName}]", (int)LogType.Debug);
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        //5．ファイルへの書き込み
                        _logger?.Log($"[INFO] ファイルへの書き込みを開始!! パス：[{saveDialog.FileName}]", (int)LogType.Debug);
                        File.WriteAllText(saveDialog.FileName, encryptedContent, Encoding.UTF8);
                        _logger?.Log("[SUCCESS] 認証リクエストファイルを正常に作成し、保存しました!!", (int)LogType.Success);
                        MessageBox.Show($"認証リクエストファイルを正常に作成しました!!\nパス：[{saveDialog.FileName}]\n\nこのファイルを開発者に提出してライセンスキーを発行してもらってください!!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        _logger?.Log("[WARNING] ユーザーが保存ダイアログをキャンセルしました!!", (int)LogType.Debug);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] 認証リクエストの生成中にエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"認証リクエストの生成中にエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _logger?.Log("[INFO] BtnGenerateAuthRequest_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //アクティベートボタンクリック時
        private void BtnActivate_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnActivate_Clickイベントを開始!!", (int)LogType.Debug);
            string licenseKey = TxtLicenseKey.Text.Trim();
            //処理結果を保持するためのフラグを定義
            bool activationSuccess = false;
            _logger?.Log($"[INFO] 入力されたライセンスキーの長さ：[{licenseKey.Length}]", (int)LogType.Debug);
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                _logger?.Log("[WARNING] ライセンスキーが未入力です!!", (int)LogType.Debug);
                MessageBox.Show("ライセンスキーが入力されていません!!", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger?.Log("[INFO] BtnActivate_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            //1．レジストリから暗号化されたライセンス情報を読み込む
            string encryptedContent = DiscordBot.Core.RegistryHelper.LoadEncryptedLicense();
            _logger?.Log($"[INFO] 暗号化コンテンツを取得しました!! 長さ：[{encryptedContent?.Length ?? 0}]", (int)LogType.Debug);
            if (string.IsNullOrEmpty(encryptedContent))
            {
                _logger?.Log("[ERROR] レジストリに暗号化コンテンツが見つかりません!! ライセンスファイル未登録!!", (int)LogType.DebugError);
                MessageBox.Show("まずライセンスファイルを登録してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log("[INFO] BtnActivate_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            try
            {
                _logger?.Log("[INFO] ライセンス情報の復号化とデシリアライズを開始!!", (int)LogType.Debug);
                //2．復号化してライセンス情報を取得
                string decryptedJson = EncryptionHelper.Decrypt(encryptedContent);
                var licenseRoot = JsonConvert.DeserializeObject<DiscordBot.Core.LicenseFileRoot>(decryptedJson);

                if (licenseRoot == null || licenseRoot.LicenseInfos == null || licenseRoot.LicenseInfos.Count == 0)
                {
                    _logger?.Log("[ERROR] 登録されているライセンスファイルの内容が不正な形式です!!", (int)LogType.DebugError);
                    MessageBox.Show("登録されているライセンスファイルが不正な形式です!!", "アクティベーション失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //finallyブロックで処理する為、ここで抜ける(アクティベートボタン有効にならない為終了MSGいらないかな。。。)
                    //_logger?.Log("[INFO] BtnActivate_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }
                _logger?.Log($"[INFO] ライセンス情報リストからキーの検証を開始!! リスト数：[{licenseRoot.LicenseInfos.Count}]", (int)LogType.Debug);
                //3．ライセンスキーの検証と情報の取得(有効期限チェックのためFirstOrDefaultを使用)
                var matchedInfo = licenseRoot.LicenseInfos.FirstOrDefault(info =>
                    info.LicenseKey.Equals(licenseKey, StringComparison.Ordinal));

                if (matchedInfo != null)
                {
                    _logger?.Log("[INFO] ライセンスキーが一致しました!! 有効期限のチェックを開始!!", (int)LogType.Debug);
                    //アクティベーション時の有効期限チェック
                    if (DateTime.TryParse(matchedInfo.ExpiryDateTime, out DateTime expiryDate))
                    {
                        //有効期限の日付部分を取得
                        DateTime licenseExpiryDateOnly = expiryDate.Date;
                        //現在の日付部分を取得
                        DateTime todayUtcDateOnly = DateTime.Now.Date.Date;
                        _logger?.Log($"[INFO] 期限日(DateOnly)：[{licenseExpiryDateOnly.ToShortDateString()}], 本日日付(DateOnly)：[{todayUtcDateOnly.ToShortDateString()}]", (int)LogType.Debug);
                        //期限切れの判断：有効期限の日付が「今日(UTC日付)」よりも過去である場合
                        if (licenseExpiryDateOnly < todayUtcDateOnly)
                        {
                            //日本語の年月日形式で表示
                            string displayDate = licenseExpiryDateOnly.ToLocalTime().ToString("yyyy年M月d日");
                            _logger?.Log($"[ERROR] ライセンス有効期限切れ!! 期限：[{displayDate}]", (int)LogType.DebugError);
                            MessageBox.Show($"このライセンスは有効期限が切れています!!\n期限：[{displayDate}]", "アクティベーション失敗!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            activationSuccess = false;
                        }
                        else
                        {
                            _logger?.Log("[SUCCESS] ライセンスキーは有効期限内です!!", (int)LogType.Debug);
                            activationSuccess = true;
                        }
                    }
                    else
                    {
                        //日付解析失敗
                        _logger?.Log($"[WARNING] 有効期限文字列：[{matchedInfo.ExpiryDateTime}] の解析に失敗しました!! アクティベーション失敗!!", (int)LogType.Debug);
                        activationSuccess = false;
                    }
                }
                else
                {
                    //キー不一致
                    _logger?.Log("[ERROR] 入力されたライセンスキーが登録されているライセンスファイルの内容と一致しません!!", (int)LogType.DebugError);
                    MessageBox.Show("入力されたライセンスキーが登録されているライセンスファイルの内容と一致しません!!", "アクティベーション失敗!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    activationSuccess = false;
                }

            }
            catch (CryptographicException)
            {
                _logger?.Log("[FATAL] 復号化失敗!! ライセンスデータのキーまたはデータが破損しています!!", (int)LogType.DebugError);
                MessageBox.Show("ライセンス情報の取得に失敗しました!!\nキーまたはデータが破損しています!!", "アクティベーション失敗!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                activationSuccess = false;
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] アクティベーション中に予期せぬエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"アクティベーション中に予期せぬエラーが発生しました!!\n{ex.Message}", "アクティベーション失敗!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                activationSuccess = false;
            }
            finally
            {
                _logger?.Log($"[INFO] finallyブロックを開始!! 最終的な activationSuccess=[{activationSuccess}]", (int)LogType.Debug);
                //成功/失敗/例外に関わらず、フラグを保存し、UIを更新する
                DiscordBot.Core.RegistryHelper.SaveLicenseActivationFlag(activationSuccess);
                //LoadDllPluginDictionary();
                _logger?.Log($"[INFO] UpdateActivationControlStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateActivationControlState();
                //メインフォームのライセンス状態をactivationSuccessで更新
                if (_mainForm != null)
                {
                    _logger?.Log($"[INFO] _mainForm.SetLicenseActiveStateメソッドを呼び出し!!", (int)LogType.Debug);
                    _mainForm.SetLicenseActiveState(activationSuccess);
                    //mainForm.UpdateMainFormControls();
                }
                if (activationSuccess)
                {
                    _logger?.Log("[INFO] ライセンスの認証とアクティベーションが完了しました!!", (int)LogType.Debug);
                    MessageBox.Show("ライセンスの認証とアクティベーションが完了しました!!\nDiscordBOTコンソール(GUI版)が利用可能になりました!!", "アクティベーション成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //成功後クリア
                    TxtLicenseKey.Text = string.Empty;
                }
                else
                {
                    _logger?.Log("[WARNING] ライセンスのアクティベーションに失敗しました!!", (int)LogType.Debug);
                }
                /*MainForm mainForm = this.Owner as MainForm;
                if (mainForm != null)
                {
                    //メインフォーム側で公開されているメソッドを呼び出す
                    mainForm.UpdateMainFormControls();
                }*/
                _logger?.Log("[INFO] BtnActivate_Clickイベントを終了!!", (int)LogType.Debug);
            }
        }
        //チェックボックスリストのチェック挙動(ユーザで変更できない!!)
        private void ClbPlugins_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            e.NewValue = e.CurrentValue;
        }
        //全角等の入力制限イベント
        private void TxtLicenseKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Backspace、Enter、Controlキーの組み合わせなどは許可
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            //IME入力モードでの文字入力をチェック
            if (IsJapaneseChar(e.KeyChar))
            {
                //日本語文字であれば入力をキャンセル
                e.Handled = true;
            }
        }
        //日本語文字かどうかを判定するメソッド
        private bool IsJapaneseChar(char c)
        {
            //Hiragana(U+3040 – U+309F)
            //Katakana(U+30A0 – U+30FF)
            //CJK Unified Ideographs(漢字など)：U+4E00 – U+9FFF
            //全角英数字/記号の範囲を追加(UFF00 - U9FFF)
            return (c >= '\u3040' && c <= '\u309F') ||
                   (c >= '\u30A0' && c <= '\u30FF') ||
                   (c >= '\u4E00' && c <= '\u9FFF') ||
                   (c >= '\uFF00' && c <= '\uFFEF');
        }

        private void BtnDisplay_Click(object sender, EventArgs e)
        {
            //現在のボタンのテキストに基づいて動作を決定
            if (BtnDisplay.Text == "表示")
            {
                //1．表示モードに切り替える
                if (TxtLicenseKey != null)
                {
                    //パスワード文字をnullに設定し、テキストを表示
                    TxtLicenseKey.PasswordChar = '\0';
                }

                //2．ボタンのテキストを「非表示」に変更
                BtnDisplay.Text = "非表示";
            }
            //現在「非表示」の場合
            else
            {
                //1．非表示モードに切り替える
                if (TxtLicenseKey != null)
                {
                    //パスワード文字を*に設定
                    TxtLicenseKey.PasswordChar = '*';
                }

                //2．ボタンのテキストを「表示」に変更
                BtnDisplay.Text = "表示";
            }
        }
        private void ClbPlugins_MouseMove(object sender, MouseEventArgs e)
        {
            CheckedListBox box = (CheckedListBox)sender;

            //マウスの位置から項目インデックスを取得
            int currentIndex = box.IndexFromPoint(e.Location);

            if (currentIndex != lastIndex)
            {
                lastIndex = currentIndex;

                //項目上にカーソルがある場合
                if (currentIndex >= 0 && currentIndex < box.Items.Count)
                {
                    // CheckedListBoxのアイテム (表示名) をキーとして取得
                    string displayNameKey = box.Items[currentIndex].ToString();

                    // 【修正箇所】dllDescriptions ではなく _dllToExplanationMap を使用
                    if (_dllToExplanationMap.ContainsKey(displayNameKey.Trim()))
                    {
                        string description = _dllToExplanationMap[displayNameKey.Trim()];

                        //以前表示されていたツールチップを非表示にし、新しい項目に対して表示
                        dllToolTip.Hide(box);
                        //5秒間表示
                        dllToolTip.Show(description, box, e.Location.X + 10, e.Location.Y + 10, 5000);
                    }
                    else
                    {
                        //説明文がない場合
                        dllToolTip.Hide(box);
                    }
                }
                else
                {
                    //リスト項目外にカーソルがある場合
                    dllToolTip.Hide(box);
                }
            }
        }
        //フォーム設定の保存
        private void SaveFormSettings()
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] SaveFormSettingsメソッドを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
            if (Properties.Settings.Default.FormSetting == true)
            {
                _logger.Log($"[INFO] フォームの終了位置記憶処理開始!!", (int)LogType.Debug);
                //フォームの幅と高さを取得
                int width = this.Width;
                int height = this.Height;
                //フォームの位置を取得
                Point formPosition = this.Location;
                Properties.Settings.Default.LicenseInfoSetting_FormX = width;
                Properties.Settings.Default.LicenseInfoSetting_FormY = height;
                Properties.Settings.Default.LicenseInfoSetting_PositionX = formPosition.X;
                Properties.Settings.Default.LicenseInfoSetting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsメソッドを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void LicenseInfoSetting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] LicenseInfoSetting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.LicenseInfoSetting_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.LicenseInfoSetting_FormX;
                int formY = Properties.Settings.Default.LicenseInfoSetting_FormY;
                int positionX = Properties.Settings.Default.LicenseInfoSetting_PositionX;
                int positionY = Properties.Settings.Default.LicenseInfoSetting_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.LicenseInfoSetting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.LicenseInfoSetting_Init = false;
                        Properties.Settings.Default.Save();
                        isActivated = true;
                    }
                    else
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの位置を変更
                        this.Location = new Point(positionX, positionY);
                        _logger.Log($"[INFO] フォーム座標：X軸[{positionX}],Y軸[{positionY}]", (int)LogType.Debug);
                        isActivated = true;
                    }
                }
                else
                {
                    //フォームの初期サイズを設定
                    this.Size = new System.Drawing.Size(formX, formY);
                    _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                    //フォームの起動位置を画面の中央に設定
                    this.StartPosition = FormStartPosition.CenterScreen;
                    _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                    isActivated = true;
                }
                _logger.Log($"[INFO] フォームアクティベート処理完了!!", (int)LogType.Debug);
                //メソッド終了ログ
                _logger.Log($"[INFO] LicenseInfoSetting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void LicenseInfoSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] LicenseInfoSetting_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] LicenseInfoSetting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [LicenseInfoSetting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

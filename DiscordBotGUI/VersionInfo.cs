using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace DiscordBotGUI
{
    public partial class VersionInfo : Form
    {
        private readonly ILogger _logger;
        //ライセンス認証フラグ
        private bool _isLicenseActive;
        //アクティベートフラグ
        private bool isActivated = false;

        public VersionInfo(ILogger logger, bool isLicenseActive)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[VersionInfo] Loggerインスタンスはnullにできません!!");
            this._isLicenseActive = isLicenseActive;
            _logger.Log("-------------------- [VersionInfo] フォームが初期化されました!! --------------------", (int)LogType.Debug);
            _logger.Log($"[INFO] ライセンス認証状態を取得：[{_isLicenseActive}]", (int)LogType.Debug);
        }
        //フォームロード時
        private void VersionInfo_Load(object sender, EventArgs e)
        {
            //イベント開始ログ
            _logger.Log($"[INFO] VersionInfo_Loadイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] DisplayVersionInfoメソッドを呼び出し!!", (int)LogType.Debug);
            DisplayVersionInfo();
            _logger.Log($"[INFO] VersionInfo_Loadイベントを終了!!", (int)LogType.Debug);
        }
        //情報取得と表示ロジック
        private void DisplayVersionInfo()
        {
            _logger.Log($"[INFO] DisplayVersionInfoメソッドを開始!!", (int)LogType.Debug);
            //1．製品本体の情報を取得(GUIアセンブリ)
            Assembly mainAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            _logger.Log($"[INFO] メインアセンブリ情報を取得!!", (int)LogType.Debug);
            string productName = AssemblyTitle(mainAssembly);
            string productVersion = mainAssembly.GetName().Version.ToString();
            _logger.Log($"[INFO] 製品名：[{productName}], バージョン：[{productVersion}]", (int)LogType.Debug);
            //2．コントロールに設定
            LblProductName.Text =$"製品名：[{productName}]";
            LblVersion.Text = $"バージョン：[{productVersion}]";
            _logger.Log($"[INFO] 製品名とバージョンをコントロールに設定!!", (int)LogType.Debug);
            //3．ライセンス認証状態 (RichTextBox を使用して色を制御)
            string baseText = "ライセンス認証状態：";
            string statusText;
            System.Drawing.Color statusColor;

            if (_isLicenseActive)
            {
                statusText = "認証済み ✔";
                statusColor = System.Drawing.Color.Lime;
                _logger.Log($"[INFO] ライセンス状態：[{statusText}]", (int)LogType.Debug);
            }
            else
            {
                statusText = "未認証 ✘";
                statusColor = System.Drawing.Color.Red;
                _logger.Log($"[ERROR] ライセンス状態：[{statusText}]", (int)LogType.DebugError);
            }
            //LinkLabel に全テキストを設定
            LnkLicenseStatus.Text = baseText + statusText;

            //1．リンクを設定する範囲を計算
            int startIndex = baseText.Length;
            int length = statusText.Length;
            _logger.Log($"[INFO] LinkAreaの開始インデックス：[{startIndex}], 長さ：[{length}]", (int)LogType.Debug);
            //2．LinkArea を設定
            LnkLicenseStatus.LinkArea = new LinkArea(startIndex, length);

            //3．リンク色を動的に設定
            LnkLicenseStatus.LinkColor = statusColor;

            //4．LinkLabel のその他の色設定
            //通常色に戻す
            LnkLicenseStatus.ForeColor = Color.White;
            //クリック時の色も同じに設定
            LnkLicenseStatus.ActiveLinkColor = statusColor;
            //一度クリックしても色が変わらないように設定
            LnkLicenseStatus.VisitedLinkColor = statusColor;
            _logger.Log($"[INFO] ライセンス状態のリンク色を [{statusColor.Name}] に設定!!", (int)LogType.Debug);
            //4．各DLLコンポーネントのバージョン情報
            string componentVersions = GetComponentVersions(mainAssembly);
            RtbComponentVersions.Text = componentVersions;
            _logger.Log($"[INFO] DLLコンポーネントのバージョン情報をRichTextBoxに設定!!", (int)LogType.Debug);
            _logger.Log($"[INFO] DisplayVersionInfoメソッドを終了!!", (int)LogType.Debug);
        }
        //指定されたアセンブリのタイトルを取得
        private string AssemblyTitle(Assembly assembly)
        {
            _logger.Log($"[INFO] AssemblyTitleメソッドを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] 対象アセンブリ：[{assembly.FullName}]", (int)LogType.Debug);
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            _logger.Log($"[INFO] AssemblyTitleAttributeの数：[{attributes.Length}]", (int)LogType.Debug);
            if (attributes.Length > 0)
            {
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                //タイトルが空でないかチェック
                if (titleAttribute.Title != "")
                {
                    _logger.Log($"[INFO] AssemblyTitleAttributeからタイトルを取得：[{titleAttribute.Title}]", (int)LogType.Debug);
                    _logger.Log($"[INFO] AssemblyTitleメソッドを終了!!", (int)LogType.Debug);
                    return titleAttribute.Title;
                }
            }
            //タイトルが見つからない場合はファイル名を返す
            string fallbackTitle = System.IO.Path.GetFileNameWithoutExtension(assembly.CodeBase);
            _logger.Log($"[ERROR] AssemblyTitleAttributeが見つからない、または空でした!! ファイル名からタイトルを取得：[{fallbackTitle}]", (int)LogType.DebugError);
            _logger.Log($"[INFO] AssemblyTitleメソッドを終了!!", (int)LogType.Debug);
            return fallbackTitle;
        }
        //プラグイン関連のDLLのバージョンを動的に取得
        private string GetComponentVersions(Assembly mainAssembly)
        {
            _logger.Log($"[INFO] GetComponentVersionsメソッドを開始!!", (int)LogType.Debug);
            var sb = new StringBuilder();
            string baseDir = Path.GetDirectoryName(mainAssembly.Location);
            _logger.Log($"[INFO] 実行ファイルディレクトリ：[{baseDir}]", (int)LogType.Debug);
            //プラグインフォルダのパスを定義
            string pluginDirPath = Path.Combine(baseDir, "Plugins");
            _logger.Log($"[INFO] Pluginsディレクトリパス：[{pluginDirPath}]", (int)LogType.Debug);
            //バージョンを取得すべきアセンブリのフルパスを格納するリスト
            var assemblyPaths = new List<string>();

            //1．DiscordBot.Core.dll
            string coreDllPath = Path.Combine(baseDir, "DiscordBot.Core.dll");
            if (File.Exists(coreDllPath))
            {
                assemblyPaths.Add(coreDllPath);
                _logger.Log($"[INFO] DiscordBot.Core.dll のパスを追加!!", (int)LogType.Debug);
            }
            else
            {
                _logger.Log($"[ERROR] DiscordBot.Core.dll が見つかりませんでした!!", (int)LogType.DebugError);
            }

            //2．DiscordBot.Plugin系
            try
            {
                if (Directory.Exists(pluginDirPath))
                {
                    _logger.Log($"[INFO] Pluginsディレクトリが存在します!! プラグイン検索を開始!!", (int)LogType.Debug);
                    //Pluginsフォルダ内にあるファイルを取得し、フルパスをリストに追加
                    var pluginFiles = Directory.GetFiles(pluginDirPath, "DiscordBot.Plugin.*.dll");
                    assemblyPaths.AddRange(pluginFiles);
                    _logger.Log($"[INFO] Pluginsディレクトリから [{pluginFiles.Length} 個] のプラグインDLLを見つけました!!", (int)LogType.Debug);
                }
                else
                {
                    _logger.Log($"[INFO] Pluginsディレクトリが見つかりませんでした!! プラグインはロードされません!!", (int)LogType.Debug);
                }
            }
            catch (Exception ex)
            {
                //フォルダアクセスやDLLが見つからないエラーは無視
                _logger.Log($"[ERROR] Pluginsディレクトリ検索中に例外が発生!!\n{ex.Message}", (int)LogType.DebugError);
            }

            //sb.AppendLine("コンポーネントバージョン一覧");
            sb.AppendLine("--------------------------------------------------");
            _logger.Log($"[INFO] 合計：[{assemblyPaths.Distinct().Count()} 個] のDLLのバージョン取得を開始!!", (int)LogType.Debug);
            //ファイル名でソートして処理
            foreach (string fullPath in assemblyPaths.Distinct().OrderBy(p => Path.GetFileName(p)))
            {
                string dllName = Path.GetFileName(fullPath);
                string componentName = Path.GetFileNameWithoutExtension(dllName);
                _logger.Log($"[INFO] 処理中のコンポーネント：[{componentName}]", (int)LogType.Debug);
                try
                {
                    Assembly assembly;

                    //既にAppDomainにロードされているかチェック
                    var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a =>
                        !a.IsDynamic &&
                        a.Location == fullPath);

                    if (loadedAssembly != null)
                    {
                        assembly = loadedAssembly;
                        _logger.Log($"[INFO] [{componentName}] は既にロード済みでした!! AppDomainから取得!!", (int)LogType.Debug);
                    }
                    else
                    {
                        //ロードされていない場合、Assembly.LoadFile で明示的に読み込む
                        assembly = Assembly.LoadFile(fullPath);
                        _logger.Log($"[INFO] [{componentName}] を Assembly.LoadFile で明示的に読み込み!!", (int)LogType.Debug);
                    }

                    var version = assembly.GetName().Version.ToString();
                    _logger.Log($"[SUCCESS] [{componentName}] のバージョンを取得：[{version}]", (int)LogType.Debug);
                    //左寄せで表示し、バージョンを整形
                    sb.AppendLine($"{componentName}：[{version}]");
                }
                catch (FileNotFoundException)
                {
                    //ファイルパスがリストにあっても、まれに取得失敗する場合があるため
                    string errorMsg = $"{componentName}\n(ファイルが見つかりません!!)";
                    sb.AppendLine(errorMsg);
                    _logger.Log($"[ERROR] [{componentName}] のバージョン取得中に FileNotFoundException が発生!!", (int)LogType.DebugError);
                }
                catch (Exception ex)
                {
                    //その他のバージョン取得エラー
                    string errorMsg = $"{componentName}\n(エラー：{ex.Message})";
                    sb.AppendLine(errorMsg);
                    _logger.Log($"[ERROR] [{componentName}] のバージョン取得中に予期せぬエラー!!\n{ex.Message}", (int)LogType.Error);
                }
            }
            sb.AppendLine("--------------------------------------------------");
            //イベント終了ログ
            _logger.Log($"[INFO] GetComponentVersionsメソッドを終了!!", (int)LogType.Debug);
            return sb.ToString();
        }
        //リンクラベルのクリック時(何もしない)
        private void LnkLicenseStatus_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            e.Link.Visited = false;
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
                Properties.Settings.Default.VersionInfo_FormX = width;
                Properties.Settings.Default.VersionInfo_FormY = height;
                Properties.Settings.Default.VersionInfo_PositionX = formPosition.X;
                Properties.Settings.Default.VersionInfo_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsメソッドを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void VersionInfo_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] VersionInfo_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.VersionInfo_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.VersionInfo_FormX;
                int formY = Properties.Settings.Default.VersionInfo_FormY;
                int positionX = Properties.Settings.Default.VersionInfo_PositionX;
                int positionY = Properties.Settings.Default.VersionInfo_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.VersionInfo_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.VersionInfo_Init = false;
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
                _logger.Log($"[INFO] VersionInfo_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void VersionInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] VersionInfo_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] VersionInfo_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [VersionInfo] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

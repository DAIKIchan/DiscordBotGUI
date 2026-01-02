using DiscordBot.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace DiscordBotGUI
{
    public partial class Setting : Form
    {
        //ILoggerを保持するためのプライベートフィールド
        private readonly ILogger _logger;
        //設定ファイルでファイルパスまで入っている為
        private string ConfigDirectory
        {
            get
            {
                return Path.Combine(Properties.Settings.Default.CmdConfigPath, "Commands.json");
            }
        }
        private bool isActivated = false;
        //比較用デバッグログフラグ
        private bool _debuglog = false;
        //デバッグログフラグ再読み込み用イベントデリゲートを定義(メインフォームで使用)
        public event Action DebugSettingChanged;
        //デリゲートの定義 (新しい設定値を引数として渡す)
        public delegate void LogPerformanceSettingUpdateHandler(int newBatchSize, int newDelayMs);
        //イベントの定義
        public event LogPerformanceSettingUpdateHandler LogPerformanceSettingChanged;
        readonly System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
        public Setting(ILogger logger)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[Setting] Loggerインスタンスはnullにできません!!");
            _debuglog = ChkDebugLog.Checked;
            //10秒間表示
            toolTip.AutoPopDelay = 10000;
            //0.2秒後に表示
            toolTip.InitialDelay = 200;
            //次に表示されるまでの待機時間
            toolTip.ReshowDelay = 1000;
            toolTip.IsBalloon = true;
            _logger.Log("-------------------- [Setting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //設定ボタン
        private void BtnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("設定を保存します!!", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (TxtConfig.Text != "")
                {
                    //コマンドコンフィグPath
                    Properties.Settings.Default.CmdConfigPath = TxtConfig.Text;
                    //Commands.jsonファイルが見つからない場合
                    if (!File.Exists(ConfigDirectory))
                    {
                        //ConfigDirectoryからファイル名を除いたディレクトリパスを取得
                        string directoryPath = Path.GetDirectoryName(ConfigDirectory);

                        //1．ディレクトリが存在するか確認し、存在しなければ作成する
                        if (!Directory.Exists(directoryPath))
                        {
                            try
                            {
                                Directory.CreateDirectory(directoryPath);
                                _logger.Log($"[INFO] ディレクトリ：[{directoryPath}]が存在しないので作成!!", (int)LogType.Debug);
                            }
                            catch (Exception ex)
                            {
                                //ディレクトリ作成失敗時の致命的なエラー
                                MessageBox.Show($"[{directoryPath}] の作成に失敗しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _logger.Log($"[INFO] ディレクトリ：[{directoryPath}]の作成に失敗しました!!\n{ex.Message}", (int)LogType.DebugError);
                                return;
                            }
                        }

                        //2．初期JSONコンテンツを作成する
                        //オブジェクトを定義してシリアライズ
                        var initialCommandsObject = new { Commands = new List<object>() };
                        string initialJsonContent = JsonConvert.SerializeObject(initialCommandsObject, Formatting.Indented);
                        _logger.Log($"[INFO] JSONオブジェクトを初期化!!", (int)LogType.Debug);
                        //3．ファイルを新規作成し、初期コンテンツを書き込む
                        try
                        {
                            File.WriteAllText(ConfigDirectory, initialJsonContent);
                            MessageBox.Show($"Commands.jsonファイルの作成が完了しました!!\n{ConfigDirectory}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _logger.Log($"[INFO] Commands.jsonファイルの作成が完了しました!!\n{ConfigDirectory}", (int)LogType.Debug);
                        }
                        catch (Exception ex)
                        {
                            //ファイル書き込みに失敗した場合の処理
                            MessageBox.Show($"Commands.jsonファイルの作成に失敗しました!!\n{ConfigDirectory}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _logger.Log($"[INFO] Commands.jsonファイルの作成に失敗しました!!\n{ConfigDirectory}", (int)LogType.DebugError);
                        }
                    }
                }
                else
                {
                    if (Properties.Settings.Default.CmdConfigPath != "")
                    {
                        //コマンドコンフィグPath
                        Properties.Settings.Default.CmdConfigPath = TxtConfig.Text;
                    }
                }
                //1．コマンドメッセージ削除設定を保存
                SaveCheckboxSetting("ChkDeleteCommandMessage", RegistryHelper.SaveDeleteCommandMessageSetting);
                _logger.Log($"[INFO] チェックボックス：[入力されたコマンドの自動削除を有効]の設定値を保存!!", (int)LogType.Debug);
                //2．デバッグログ出力設定を保存
                SaveCheckboxSetting("ChkDebugLog", RegistryHelper.SaveDebugLogEnabledSetting);
                _logger.Log($"[INFO] チェックボックス：[コンソールのデバッグログ出力を有効]の設定値を保存!!", (int)LogType.Debug);
                //ログ性能設定の保存
                if (Controls.Find("NumLogBatchSize", true).FirstOrDefault() is NumericUpDown numBatchSize)
                {
                    int batchSize = (int)numBatchSize.Value;
                    RegistryHelper.SaveLogBatchSize(batchSize);
                    _logger.Log($"[INFO] NumLogBatchSize：[ログバッチサイズ]の値[{batchSize}]を保存!!", (int)LogType.Debug);
                }
                if (Controls.Find("NumUITaskDelayMs", true).FirstOrDefault() is NumericUpDown numDelayMs)
                {
                    int delayMs = (int)numDelayMs.Value;
                    RegistryHelper.SaveUITaskDelayMs(delayMs);
                    _logger.Log($"[INFO] NumUITaskDelayMs：[UI待機時間(ms)]の値[{delayMs}]を保存!!", (int)LogType.Debug);
                }
                bool isLogFileEnabled = ChkWriteLogFile.Checked;
                RegistryHelper.SaveDebugLogFileEnabled(isLogFileEnabled);
                _logger?.Log($"[INFO] ログファイル出力設定を保存：[{isLogFileEnabled}]", (int)LogType.Debug);
                //Debugログ変更通知イベント
                if (_debuglog != ChkDebugLog.Checked)
                {
                    //nullチェックしてから呼び出す
                    DebugSettingChanged?.Invoke();
                    //比較用変数に変更した値を入れる
                    _debuglog = ChkDebugLog.Checked;
                }
                //パフォーマンス設定のイベント通知
                var newSettings = GetLogPerformanceSettings();
                if (newSettings.HasValue)
                {
                    //レジストリへの保存処理はここより上で行われている前提

                    //イベントを発火させ、新しい設定値をメインフォームに渡す
                    LogPerformanceSettingChanged?.Invoke(newSettings.Value.BatchSize, newSettings.Value.DelayMs);
                }
                _logger.Log($"[INFO] フォームの終了位置記憶フラグ：[{FormSetting.Checked}]", (int)LogType.Debug);
                //前回フォーム位置フラグ
                if (FormSetting.Checked == false)
                {
                    //フォーム前回位置記憶フラグ
                    Properties.Settings.Default.FormSetting = FormSetting.Checked;
                    //MainFormフォーム位置
                    Properties.Settings.Default.MainForm_FormX = 900;
                    Properties.Settings.Default.MainForm_FormY = 500;
                    Properties.Settings.Default.MainForm_PositionX = 0;
                    Properties.Settings.Default.MainForm_PositionY = 0;
                    Properties.Settings.Default.MainForm_Init = true;
                    //Settingフォーム位置
                    Properties.Settings.Default.Setting_FormX = 900;
                    Properties.Settings.Default.Setting_FormY = 500;
                    Properties.Settings.Default.Setting_PositionX = 0;
                    Properties.Settings.Default.Setting_PositionY = 0;
                    Properties.Settings.Default.Setting_Init = true;
                    //Editフォーム位置
                    Properties.Settings.Default.Edit_FormX = 900;
                    Properties.Settings.Default.Edit_FormY = 500;
                    Properties.Settings.Default.Edit_PositionX = 0;
                    Properties.Settings.Default.Edit_PositionY = 0;
                    Properties.Settings.Default.Edit_Init = true;
                    //QASettingフォーム位置
                    Properties.Settings.Default.QASetting_FormX = 500;
                    Properties.Settings.Default.QASetting_FormY = 300;
                    Properties.Settings.Default.QASetting_PositionX = 0;
                    Properties.Settings.Default.QASetting_PositionY = 0;
                    Properties.Settings.Default.QASetting_Init = true;
                    //DeleteSettingフォーム位置
                    Properties.Settings.Default.DeleteSetting_FormX = 500;
                    Properties.Settings.Default.DeleteSetting_FormY = 600;
                    Properties.Settings.Default.DeleteSetting_PositionX = 0;
                    Properties.Settings.Default.DeleteSetting_PositionY = 0;
                    Properties.Settings.Default.DeleteSetting_Init = true;
                    //BanSettingフォーム位置
                    Properties.Settings.Default.BanSetting_FormX = 500;
                    Properties.Settings.Default.BanSetting_FormY = 600;
                    Properties.Settings.Default.BanSetting_PositionX = 0;
                    Properties.Settings.Default.BanSetting_PositionY = 0;
                    Properties.Settings.Default.BanSetting_Init = true;
                    //KickSettingフォーム位置
                    Properties.Settings.Default.KickSetting_FormX = 500;
                    Properties.Settings.Default.KickSetting_FormY = 600;
                    Properties.Settings.Default.KickSetting_PositionX = 0;
                    Properties.Settings.Default.KickSetting_PositionY = 0;
                    Properties.Settings.Default.KickSetting_Init = true;
                    //WeatherInfoSettingフォーム位置
                    Properties.Settings.Default.WeatherInfoSetting_FormX = 400;
                    Properties.Settings.Default.WeatherInfoSetting_FormY = 400;
                    Properties.Settings.Default.WeatherInfoSetting_PositionX = 0;
                    Properties.Settings.Default.WeatherInfoSetting_PositionY = 0;
                    Properties.Settings.Default.WeatherInfoSetting_Init = true;
                    //JoiningLeavingSettingフォーム位置
                    Properties.Settings.Default.JoiningLeavingSetting_FormX = 300;
                    Properties.Settings.Default.JoiningLeavingSetting_FormY = 300;
                    Properties.Settings.Default.JoiningLeavingSetting_PositionX = 0;
                    Properties.Settings.Default.JoiningLeavingSetting_PositionY = 0;
                    Properties.Settings.Default.JoiningLeavingSetting_Init = true;
                    //VersionInfoフォーム位置
                    Properties.Settings.Default.VersionInfo_FormX = 500;
                    Properties.Settings.Default.VersionInfo_FormY = 600;
                    Properties.Settings.Default.VersionInfo_PositionX = 0;
                    Properties.Settings.Default.VersionInfo_PositionY = 0;
                    Properties.Settings.Default.VersionInfo_Init = true;
                    //LicenseInfoSettingフォーム位置
                    Properties.Settings.Default.LicenseInfoSetting_FormX = 400;
                    Properties.Settings.Default.LicenseInfoSetting_FormY = 400;
                    Properties.Settings.Default.LicenseInfoSetting_PositionX = 0;
                    Properties.Settings.Default.LicenseInfoSetting_PositionY = 0;
                    Properties.Settings.Default.LicenseInfoSetting_Init = true;
                    _logger.Log($"[INFO] 各フォームのサイズ＆座標を初期化完了!!", (int)LogType.Debug);
                }
                else
                {
                    Properties.Settings.Default.FormSetting = FormSetting.Checked;
                    
                }
                Properties.Settings.Default.Save();
                MessageBox.Show("設定を保存しました!!", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //フォルダ選択ダイアログ表示
        public static string SelectFolder(System.IntPtr parentHandle)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                //フォルダのみ選択可能にする
                dialog.IsFolderPicker = true;
                //初期ディレクトリ
                dialog.InitialDirectory = "C:\\";
                //ネットワークフォルダを許可
                dialog.AllowNonFileSystemItems = true;
                dialog.Title = "フォルダを選択してください!!";

                if (dialog.ShowDialog(parentHandle) == CommonFileDialogResult.Ok)
                {
                    //選択したフォルダのパスを返す
                    return dialog.FileName;
                }
            }
            return null;
        }
        //参照ボタンクリック時
        private void BtnStart_Click(object sender, EventArgs e)
        {
            string selectedPath = SelectFolder(this.Handle);
            if (!string.IsNullOrEmpty(selectedPath))
            {
                //選択したフォルダパスを表示
                TxtConfig.Text = selectedPath;
            }
        }
        //設定値を対応するチェックボックスに反映させるローカル関数
        void SetCheckboxState(string controlName, bool isChecked)
        {
            //コントロールを検索し、CheckBoxであればCheckedプロパティを設定する
            if (Controls.Find(controlName, true).FirstOrDefault() is System.Windows.Forms.CheckBox checkbox)
            {
                checkbox.Checked = isChecked;
            }
        }
        //レジストリ保存処理をするローカル関数
        private void SaveCheckboxSetting(string controlName, Action<bool> saveAction)
        {
            //コントロールを検索し、CheckBoxであればCheckedプロパティを取得
            if (Controls.Find(controlName, true).FirstOrDefault() is System.Windows.Forms.CheckBox checkbox)
            {
                //取得したチェック状態を引数として、渡された保存アクション (RegistryHelper.Save...Setting) を実行
                saveAction(checkbox.Checked);
            }
        }
        //NumericUpDown の値を読み込むメソッド
        private (int BatchSize, int DelayMs)? GetLogPerformanceSettings()
        {
            int batchSize = 0;
            int delayMs = 0;
            bool allFound = true;

            if (Controls.Find("NumLogBatchSize", true).FirstOrDefault() is NumericUpDown numBatchSize)
            {
                batchSize = (int)numBatchSize.Value;
            }
            else { allFound = false; }

            if (Controls.Find("NumUITaskDelayMs", true).FirstOrDefault() is NumericUpDown numDelayMs)
            {
                delayMs = (int)numDelayMs.Value;
            }
            else { allFound = false; }

            if (allFound)
            {
                // 全てのコントロールが見つかった場合、タプルを返す
                return (batchSize, delayMs);
            }
            else
            {
                //見つからなかった場合、nullを返す
                return null;
            }
        }
        //フォームロード時
        private void Setting_Load(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] Setting_Loadイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] フォームをロード中...", (int)LogType.Debug);
            //コマンドコンフィグパス
            TxtConfig.Text = Properties.Settings.Default.CmdConfigPath;
            //前回フォーム位置を記憶フラグ
            FormSetting.Checked = Properties.Settings.Default.FormSetting;
            //コマンドメッセージ削除設定フラグのロード
            bool shouldDelete = RegistryHelper.LoadDeleteCommandMessageSetting();
            //デバッグログの出力を有効フラグのロード
            bool debuglog = RegistryHelper.LoadDebugLogEnabledSetting();
            _debuglog = RegistryHelper.LoadDebugLogEnabledSetting();
            bool debuglogoutput = RegistryHelper.LoadDebugLogFileEnabled();
            //ログ性能設定のロード
            int logBatchSize = RegistryHelper.LoadLogBatchSize();
            int uiTaskDelayMs = RegistryHelper.LoadUITaskDelayMs();
            //ロードした設定値をチェックボックスにセット
            SetCheckboxState("ChkDeleteCommandMessage", shouldDelete);
            _logger.Log($"[INFO] チェックボックス：[入力されたコマンドの自動削除を有効]の設定値をロード!!", (int)LogType.Debug);
            SetCheckboxState("ChkDebugLog", debuglog);
            _logger.Log($"[INFO] チェックボックス：[コンソールのデバッグログ出力を有効]の設定値をロード!!", (int)LogType.Debug);
            SetCheckboxState("ChkWriteLogFile", debuglogoutput);
            _logger.Log($"[INFO] チェックボックス：[ログファイル出力を有効]の設定値をロード!!", (int)LogType.Debug);
            //ログ性能設定のコントロールへの反映
            if (Controls.Find("NumLogBatchSize", true).FirstOrDefault() is NumericUpDown numBatchSize)
            {
                numBatchSize.Value = logBatchSize;
                _logger.Log($"[INFO] NumLogBatchSize：[ログバッチサイズ]の値[{logBatchSize}]をロード!!", (int)LogType.Debug);
            }
            if (Controls.Find("NumUITaskDelayMs", true).FirstOrDefault() is NumericUpDown numDelayMs)
            {
                numDelayMs.Value = uiTaskDelayMs;
                _logger.Log($"[INFO] NumUITaskDelayMs：[UI待機時間(ms)]の値[{uiTaskDelayMs}]をロード!!", (int)LogType.Debug);
            }
            //チェックボックスに説明文を設定
            toolTip.SetToolTip(FormSetting, "前回閉じた座標でフォームが起動します!!");
            toolTip.SetToolTip(ChkDeleteCommandMessage, "送信したコマンドを検知し自動削除します!!");
            toolTip.SetToolTip(ChkDebugLog, "プログラムのデバッグログをメインコンソールに出力します!!\n※膨大なログでUIスレッドが固まるおそれがあります!!");
            toolTip.SetToolTip(NumLogBatchSize, "一度にUIに書き込むログの最大数です!!\n小さくするとUIのフリーズ時間が短くなります!!");
            toolTip.SetToolTip(NumUITaskDelayMs, "ログのバッチ処理後、UIスレッドに処理を返すまでの待機時間(ミリ秒)です!!\n長くするとCPU負荷が軽減されます!!");
            _logger.Log($"[INFO] フォームをロード完了!!", (int)LogType.Debug);
            //メソッド終了ログ
            _logger.Log($"[INFO] Setting_Loadイベントを終了!!", (int)LogType.Debug);
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
                Properties.Settings.Default.Setting_FormX = width;
                Properties.Settings.Default.Setting_FormY = height;
                Properties.Settings.Default.Setting_PositionX = formPosition.X;
                Properties.Settings.Default.Setting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsメソッドを終了!!", (int)LogType.Debug);
        }
        //フォームクローズ時
        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] Setting_FormClosingイベントを開始!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] Setting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [Setting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void Setting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] Setting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.Setting_Init}]", (int)LogType.Debug);
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.Setting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(Properties.Settings.Default.Setting_FormX, Properties.Settings.Default.Setting_FormY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{Properties.Settings.Default.Setting_FormX}],Y軸[{Properties.Settings.Default.Setting_FormY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.Setting_Init = false;
                        Properties.Settings.Default.Save();
                        isActivated = true;
                    }
                    else
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(Properties.Settings.Default.Setting_FormX, Properties.Settings.Default.Setting_FormY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{Properties.Settings.Default.Setting_FormX}],Y軸[{Properties.Settings.Default.Setting_FormY}]", (int)LogType.Debug);
                        //フォームの位置を変更
                        this.Location = new Point(Properties.Settings.Default.Setting_PositionX, Properties.Settings.Default.Setting_PositionY);
                        _logger.Log($"[INFO] フォーム座標：X軸[{Properties.Settings.Default.Setting_PositionX}],Y軸[{Properties.Settings.Default.Setting_PositionY}]", (int)LogType.Debug);
                        isActivated = true;
                    }
                }
                else
                {
                    //フォームの初期サイズを設定
                    this.Size = new System.Drawing.Size(Properties.Settings.Default.Setting_FormX, Properties.Settings.Default.Setting_FormY);
                    _logger.Log($"[INFO] フォームサイズ：X軸[{Properties.Settings.Default.Setting_FormX}],Y軸[{Properties.Settings.Default.Setting_FormY}]", (int)LogType.Debug);
                    //フォームの起動位置を画面の中央に設定
                    this.StartPosition = FormStartPosition.CenterScreen;
                    _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                    isActivated = true;
                }
                _logger.Log($"[INFO] フォームアクティベート処理完了!!", (int)LogType.Debug);
                //メソッド終了ログ
                _logger.Log($"[INFO] Setting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
    }
}

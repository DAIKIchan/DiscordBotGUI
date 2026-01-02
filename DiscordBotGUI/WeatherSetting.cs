using DiscordBot.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class WeatherSetting : Form
    {
        //ILoggerを保持するためのプライベートフィールド
        private readonly ILogger _logger;
        //キャッシュクリア機能を持つプラグインへの参照 (UIからはインターフェース経由でのみアクセス)
        private readonly ICacheClearProvider _cacheClearProvider;
        private bool isActivated = false;
        //キャッシュ時間用変数
        int currentDuration;
        //APIキー用変数
        string currentApiKey;
        readonly System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
        public WeatherSetting(ILogger logger, ICacheClearProvider cacheClearProvider)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[WeatherInfoSetting] Loggerインスタンスはnullにできません!!(2)");
            _cacheClearProvider = cacheClearProvider ?? throw new ArgumentNullException(nameof(cacheClearProvider), "[WeatherInfoSetting] CacheClearProviderインスタンスはnullにできません!!(2)");
            //ツールチップのカスタマイズ設定
            //10秒間表示
            toolTip.AutoPopDelay = 10000;
            //0.2秒後に表示
            toolTip.InitialDelay = 200;
            //次に表示されるまでの待機時間
            toolTip.ReshowDelay = 1000;
            toolTip.IsBalloon = true;
            _logger.Log("-------------------- [WeatherInfoSetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //コントロールの有効/無効状態を更新
        private void UpdateControlsState()
        {
            _logger?.Log("[INFO] UpdateControlsStateメソッドを開始!!", (int)LogType.Debug);
            //チェックボックスの状態を NumericUpDown の Enabled プロパティに直接設定
            bool isEnabled = ChkAutoDeleteEnabled.Checked;
            NumDeleteDelayMs.Enabled = isEnabled;
            _logger?.Log($"[INFO] ChkAutoDeleteEnabled：[{isEnabled}], NumDeleteDelayMs.Enabled設定完了!!", (int)LogType.Debug);
            int delayMs = RegistryHelper.LoadWeatherDeleteDelayMs();
            _logger?.Log($"[INFO] プロンプトMSG自動削除時間：[{delayMs}]ms", (int)LogType.Debug);
            NumDeleteDelayMs.Value = delayMs;
            if (ChkAutoDeleteEnabled.Checked == false)
            {
                NumDeleteDelayMs.Value = 0;
                _logger?.Log("[INFO] プロンプトMSG自動削除が無効のため、NumDeleteDelayMs.Valueを0に設定!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] UpdateControlsStateメソッドを終了!!", (int)LogType.Debug);
        }
        //現在の設定をレジストリから読み込み、UIに反映
        private void LoadCurrentSettings()
        {
            _logger?.Log($"[INFO] LoadCurrentSettingsメソッドを開始!!", (int)LogType.Debug);
            try
            {
                //レジストリヘルパーを介して現在のキャッシュ時間を取得
                _logger?.Log("[INFO] (1) キャッシュ時間の読み込みを開始!!", (int)LogType.Debug);
                currentDuration = RegistryHelper.LoadWeatherCacheDurationSetting();
                //NumericUpDownコントロールの範囲内にあるか確認し、値を設定する
                //レジストリの値が Maximum/Minimum の範囲外の場合は調整
                decimal clampedDuration = Math.Max(NumCacheDuration.Minimum, Math.Min(NumCacheDuration.Maximum, currentDuration));
                NumCacheDuration.Value = clampedDuration;
                _logger?.Log($"[INFO] 天気予報キャッシュ時間設定を読み込みました!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] 現在設定されているキャッシュ時間：[設定値=[{currentDuration} 分], UI設定値=[{clampedDuration} 分]", (int)LogType.Debug);

                //レジストリヘルパーを介してAPIkeyを取得し、TxtApiKeyに設定
                _logger?.Log("[INFO] (2) API Keyの読み込みを開始!!", (int)LogType.Debug);
                currentApiKey = RegistryHelper.LoadWeatherApiKeySetting();
                TxtApiKey.Text = currentApiKey;
                _logger?.Log($"[INFO] 天気予報APIキーを読み込みました!!", (int)LogType.Debug);

                //プロンプトMSG自動削除フラグ の読み込み
                _logger?.Log("[DEBUG] (3) 自動削除フラグと遅延時間の読み込みを開始!!", (int)LogType.Debug);
                bool shouldDelete = RegistryHelper.LoadWeatherShouldDelete();
                ChkAutoDeleteEnabled.Checked = shouldDelete;
                //プロンプトMSG自動削除するまでの時間設定
                int delayMs = RegistryHelper.LoadWeatherDeleteDelayMs();
                NumDeleteDelayMs.Value = delayMs;
                _logger?.Log($"[INFO] BOTによるプロンプトMSG自動削除フラグ：[{shouldDelete}]", (int)LogType.Debug);
                _logger?.Log($"[INFO] BOTによるプロンプトMSG自動削除時間：[{delayMs} ms / {delayMs / 1000} 秒]", (int)LogType.Debug);

                //コントロールの有効/無効
                _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateControlsState();
            }
            catch (Exception ex)
            {
                _logger?.Log($"[ERROR] 設定の読み込み中に予期せぬエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"設定の読み込み中にエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _logger?.Log("[INFO] LoadCurrentSettingsメソッドを終了!!", (int)LogType.Debug);
        }
        //フォームロード時
        private void WeatherInfoSetting_Load(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] LoadCurrentSettingsイベントを開始!!", (int)LogType.Debug);
            //チェックボックスに説明文を設定
            toolTip.SetToolTip(LblCacheDuration, "この時間(分)が経過するまで、天気情報は再取得されずキャッシュデータを使用します!!\n0分に設定すると、キャッシュデータを使用せず常に最新の情報を取得します!!\n\n※キャッシュデータを使用しない場合、毎回API接続しに行くことで動作やレスポンスが低下する可能性がありますが、問題ない場合は[0分]をオススメします!!");
            _logger?.Log("[INFO] LoadCurrentSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            LoadCurrentSettings();
            _logger?.Log("[INFO] LoadCurrentSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //保存ボタンクリック時の処理
        private void BtnSave_Click(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger?.Log($"[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);
            try
            {
                //UIから新しい値を取得 (decimal から int に変換)
                int newDuration = (int)NumCacheDuration.Value;
                _logger?.Log($"[INFO] UIから取得した新しいキャッシュ時間：[{newDuration} 分]", (int)LogType.Debug);
                //レジストリにキャッシュ時間を保存
                RegistryHelper.SaveWeatherCacheDurationSetting(newDuration);

                // APIキー設定を保存 (TxtApiKey が TextBox コントロールであると仮定)
                string apiKey = TxtApiKey.Text;
                RegistryHelper.SaveWeatherApiKeySetting(apiKey);
                _logger?.Log($"[INFO] APIキー設定を保存しました!! キーの長さ：[{apiKey.Length}]", (int)LogType.Debug);
                //プロンプトMSG自動削除フラグ＆数値を保存
                if (this.ChkAutoDeleteEnabled != null)
                {
                    //ShouldDelete の保存
                    bool shouldDelete = ChkAutoDeleteEnabled.Checked;
                    RegistryHelper.SaveWeatherShouldDelete(shouldDelete);

                    //DeleteDelayMs の保存
                    int delayMs = (int)NumDeleteDelayMs.Value;
                    RegistryHelper.SaveWeatherDeleteDelayMs(delayMs);
                    _logger?.Log($"[INFO] 自動削除設定を保存：ShouldDelete=[{shouldDelete}], DeleteDelayMs=[{delayMs}]ms", (int)LogType.Debug);
                }
                _logger?.Log($"[INFO] キャッシュデータの保存時間：[{newDuration} 分] を含むすべての設定を保存しました!!", (int)LogType.Debug);
                MessageBox.Show($"設定を保存しました!!", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                _logger?.Log($"[ERROR] 設定の保存中にエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"設定の保存中にエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _logger?.Log($"[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //キャッシュデータ削除ボタンクリック時
        private void BtnClearCache_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnClearCache_Clickイベントを開始!!", (int)LogType.Debug);
            try
            {
                //確認ダイアログ
                DialogResult confirmResult = MessageBox.Show(
                    "現在保持されている天気情報キャッシュデータを全て削除しますか？\n※ 削除後、次回コマンド実行時に最新の情報を取得します!!",
                    "キャッシュデータの削除確認",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                _logger?.Log($"[INFO] ユーザー確認結果：[{confirmResult}]", (int)LogType.Debug);
                if (confirmResult == DialogResult.Yes)
                {
                    _logger?.Log("[INFO] ユーザーが削除を承認しました!!", (int)LogType.Debug);
                    _logger?.Log("[INFO] ICacheClearProvider.ClearCacheメソッドを呼び出し!!", (int)LogType.Debug);
                    //ICacheClearProvider 経由でプラグインのキャッシュクリア機能を呼び出す
                    int clearedCount = _cacheClearProvider.ClearCache();
                    _logger?.Log($"[INFO] 全ての天気情報キャッシュをクリアしました!!", (int)LogType.Debug);
                    _logger?.Log($"[INFO] クリアされたキャッシュエントリ数：[{clearedCount} 個]", (int)LogType.Debug);

                    //完了メッセージを表示
                    MessageBox.Show($"[{clearedCount} 個]のキャッシュエントリを削除しました!!", "キャッシュ削除完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _logger?.Log("[INFO] ユーザーがキャッシュ削除をキャンセルしました!!", (int)LogType.Debug);
                }
            }
            catch (Exception ex)
            {
                _logger?.Log($"[ERROR] キャッシュデータの削除中に予期せぬエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"キャッシュデータの削除中にエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _logger?.Log("[INFO] BtnClearCache_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //プロンプトMSGの自動削除有効フラグ変更時
        private void ChkAutoDeleteEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            _logger?.Log("[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを終了!!", (int)LogType.Debug);
        }
        //フォーム設定の保存
        private void SaveFormSettings()
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
            if (Properties.Settings.Default.FormSetting == true)
            {
                _logger.Log($"[INFO] フォームの終了位置記憶処理開始!!", (int)LogType.Debug);
                //フォームの幅と高さを取得
                int width = this.Width;
                int height = this.Height;
                //フォームの位置を取得
                Point formPosition = this.Location;
                Properties.Settings.Default.WeatherInfoSetting_FormX = width;
                Properties.Settings.Default.WeatherInfoSetting_FormY = height;
                Properties.Settings.Default.WeatherInfoSetting_PositionX = formPosition.X;
                Properties.Settings.Default.WeatherInfoSetting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void WeatherInfoSetting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] WeatherInfoSetting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.WeatherInfoSetting_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.WeatherInfoSetting_FormX;
                int formY = Properties.Settings.Default.WeatherInfoSetting_FormY;
                int positionX = Properties.Settings.Default.WeatherInfoSetting_PositionX;
                int positionY = Properties.Settings.Default.WeatherInfoSetting_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.WeatherInfoSetting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.WeatherInfoSetting_Init = false;
                        _logger.Log($"[INFO] フォームの初期化フラグを[{Properties.Settings.Default.WeatherInfoSetting_Init}]に設定!!", (int)LogType.Debug);
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
                _logger.Log($"[INFO] WeatherInfoSetting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void WeatherInfoSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] WeatherInfoSetting_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] WeatherInfoSetting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [WeatherInfoSetting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

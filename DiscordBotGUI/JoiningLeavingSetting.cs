using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using DiscordBot.Core;

namespace DiscordBotGUI
{
    public partial class JoiningLeavingSetting : Form
    {
        //ILoggerを保持するためのプライベートフィールド
        private readonly ILogger _logger;
        private bool isActivated = false;
        public JoiningLeavingSetting(ILogger logger)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[JoiningLeavingSetting] Loggerインスタンスはnullにできません!!");
            _logger.Log("-------------------- [JoiningLeavingSetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //フォームロード時の処理
        private void JoiningLeavingSetting_Load(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] JoiningLeavingSetting_Loadイベントを開始!!", (int)LogType.Debug);
            //レジストリからチャンネルIDを読み込み、TextBoxに表示
            string loadedChannelId = RegistryHelper.ReadChannelId();
            TxtChannelId.Text = loadedChannelId;
            _logger?.Log($"[INFO] チャンネルIDを読み込みました：[{loadedChannelId}]", (int)LogType.Debug);
            _logger?.Log("[INFO] JoiningLeavingSetting_Loadイベントを終了!!", (int)LogType.Debug);
        }
        //保存ボタンクリック時の処理
        private void BtnSave_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);
            string channelId = TxtChannelId.Text.Trim();
            _logger?.Log($"[INFO] 入力されたチャンネルID：[{channelId}] (長さ：{channelId.Length})", (int)LogType.Debug);
            //簡易的な入力チェック (DiscordチャンネルIDは通常17桁以上の数字)
            if (string.IsNullOrWhiteSpace(channelId) || !Regex.IsMatch(channelId, @"^\d{17,}$"))
            {
                _logger?.Log($"[ERROR] チャンネルIDの入力が無効です!! 空か、または17桁以上の数字ではありません!!", (int)LogType.DebugError);
                MessageBox.Show("有効なDiscordチャンネルID ※17桁以上の数字 を入力してください!!", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _logger?.Log("[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
                return;
            }

            try
            {
                //レジストリにチャンネルIDを保存
                RegistryHelper.WriteChannelId(channelId);
                _logger?.Log($"[SUCCESS] チャンネルID：[{channelId}] の保存に成功!!", (int)LogType.Debug);

                MessageBox.Show("チャンネルIDを保存しました!!", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger?.Log($"[FATAL] チャンネルIDのレジストリ書き込み中にエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.Error);
                MessageBox.Show($"設定の保存中にエラーが発生しました!!\n{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _logger?.Log("[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
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
                Properties.Settings.Default.JoiningLeavingSetting_FormX = width;
                Properties.Settings.Default.JoiningLeavingSetting_FormY = height;
                Properties.Settings.Default.JoiningLeavingSetting_PositionX = formPosition.X;
                Properties.Settings.Default.JoiningLeavingSetting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void JoiningLeavingSetting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] JoiningLeavingSetting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.JoiningLeavingSetting_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.JoiningLeavingSetting_FormX;
                int formY = Properties.Settings.Default.JoiningLeavingSetting_FormY;
                int positionX = Properties.Settings.Default.JoiningLeavingSetting_PositionX;
                int positionY = Properties.Settings.Default.JoiningLeavingSetting_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.JoiningLeavingSetting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.JoiningLeavingSetting_Init = false;
                        _logger.Log($"[INFO] フォームの初期化フラグを[{Properties.Settings.Default.JoiningLeavingSetting_Init}]に設定!!", (int)LogType.Debug);
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
                _logger.Log($"[INFO] JoiningLeavingSetting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void JoiningLeavingSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] JoiningLeavingSetting_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] JoiningLeavingSetting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [JoiningLeavingSetting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

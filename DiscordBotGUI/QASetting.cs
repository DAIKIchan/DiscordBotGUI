using DiscordBot.Core;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class QASetting : Form
    {
        private bool isActivated = false;
        private readonly ILogger _logger;
        public QASetting(ILogger logger)
        {
            InitializeComponent();
            //_logger = logger;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[QASetting] Loggerインスタンスはnullにできません!!");
            _logger.Log("-------------------- [QASetting] フォームが初期化されました!! --------------------", (int)LogType.Debug);
            _logger.Log($"[INFO] LoadSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            LoadSettings();
        }

        //16進数コードと色名を結合した文字列を生成するヘルパーメソッド
        private string GetColorDisplayString(Color color)
        {
            _logger?.Log("[INFO] GetColorDisplayStringヘルパーメソッドを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] 処理対象の色：[A={color.A}, R={color.R}, G={color.G}, B={color.B}]", (int)LogType.Debug);
            string hexString = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            _logger?.Log($"[INFO] 生成された16進数文字列：[{hexString}]", (int)LogType.Debug);
            //Web標準色名やシステム色名であれば、名前を併記する
            //デフォルト
            string colorName = "RGB値";
            _logger?.Log($"[INFO] IsNamedColorチェック：[{color.IsNamedColor}]", (int)LogType.Debug);
            if (color.IsNamedColor)
            {
                //IsKnownColor または IsSystemColor が true の場合、Nameを使用
                colorName = color.Name;
            }
            //最終的な表示文字列
            string result = $"{hexString} ({colorName})";
            _logger?.Log($"[INFO] 返却値：[{result}]", (int)LogType.Debug);
            _logger?.Log($"[INFO] GetColorDisplayStringを終了!!", (int)LogType.Debug);
            //例：#FF0000 (Red) または #1ABC9C (RGB値)
            return result;
        }
        //コントロールの有効/無効状態を更新
        private void UpdateControlsState()
        {
            _logger?.Log("[INFO] UpdateControlsStateメソッドを開始!!", (int)LogType.Debug);
            //チェックボックスの状態を NumericUpDown の Enabled プロパティに直接設定
            bool isEnabled = ChkAutoDeleteEnabled.Checked;
            NumDeleteDelayMs.Enabled = isEnabled;
            //チェックボックスの状態を NumericUpDown の Enabled プロパティに直接設定
            _logger?.Log($"[INFO] ChkAutoDeleteEnabled.Checked：[{isEnabled}] に基づき、NumDeleteDelayMs.Enabledを設定!!", (int)LogType.Debug);
            int delayMs = RegistryHelper.LoadQADeleteDelayMs();
            _logger?.Log($"[INFO] プロンプトMSG自動削除時間：[{delayMs}]ms", (int)LogType.Debug);
            NumDeleteDelayMs.Value = delayMs;
            if (ChkAutoDeleteEnabled.Checked == false)
            {
                NumDeleteDelayMs.Value = 0;
                _logger?.Log("[INFO] ChkAutoDeleteEnabledがfalseのため、NumDeleteDelayMs.Valueを0に設定しました!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] UpdateControlsStateメソッドを終了!!", (int)LogType.Debug);
        }

        //フォーム起動時に設定を読み込む
        private void LoadSettings()
        {
            _logger?.Log("[INFO] LoadSettingsメソッドを開始!!", (int)LogType.Debug);
            //1．埋め込み色
            _logger?.Log("[INFO] 埋め込み色設定の読み込みを開始!!", (int)LogType.Debug);
            //Discord.Color 構造体が返されることを前提
            Discord.Color discordColor = RegistryHelper.LoadQAEmbedColor();
            Color loadedColor = Color.FromArgb(255, (byte)discordColor.R, (byte)discordColor.G, (byte)discordColor.B);
            _logger?.Log($"[INFO] 読み込んだDiscordカラー値 (R,G,B)：[{discordColor.R}, {discordColor.G}, {discordColor.B}]", (int)LogType.Debug);
            //UIコントロールがInitializeComponent()で初期化されていることを前提
            if (this.PnlColorPreview != null && this.TxtEmbedColor != null)
            {
                //プレビューとTextBoxに反映
                this.PnlColorPreview.BackColor = loadedColor;

                //16進数と色名を併記して表示
                string colorDisplayString = GetColorDisplayString(loadedColor);
                this.TxtEmbedColor.Text = colorDisplayString;

                //TxtEmbedColor が編集不可であることを前提に ReadOnly プロパティを設定
                this.TxtEmbedColor.ReadOnly = true;
                _logger?.Log($"[INFO] PnlColorPreviewを [{loadedColor.ToArgb():X8}] に、TxtEmbedColorを [{colorDisplayString}] に設定!!", (int)LogType.Debug);
            }

            //2．期限設定
            _logger?.Log("[INFO] QAアンケートタイムアウト設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.NumTimeoutMinutes != null)
            {
                int timeoutValue = RegistryHelper.LoadQATimeoutMinutes();
                //範囲外の値がレジストリにあった場合に備えてMin/Maxでクランプ
                decimal clampedValue = Math.Max(this.NumTimeoutMinutes.Minimum, Math.Min(this.NumTimeoutMinutes.Maximum, timeoutValue));
                this.NumTimeoutMinutes.Value = clampedValue;
                _logger?.Log($"[INFO] QAアンケートタイムアウト値：レジストリ値=[{timeoutValue}]分, 設定値=[{clampedValue}]分", (int)LogType.Debug);
            }
            //複数投票の許可設定の読み込み
            _logger?.Log("[INFO] 複数投票許可設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.ChkAllowMultipleVotes != null)
            {
                // true = 複数投票を許可 (0)
                bool allowMultiple = RegistryHelper.LoadAllowMultipleVotesSetting();
                this.ChkAllowMultipleVotes.Checked = allowMultiple;
                _logger?.Log($"[INFO] 複数投票許可 (ChkAllowMultipleVotes) を [{allowMultiple}] に設定!!", (int)LogType.Debug);
            }
            //プロンプトMSG自動削除フラグ の読み込み
            _logger?.Log("[INFO] プロンプトMSG自動削除設定の読み込みを開始!!", (int)LogType.Debug);
            if (this.ChkAutoDeleteEnabled != null)
            {
                bool shouldDelete = RegistryHelper.LoadQAShouldDelete();
                ChkAutoDeleteEnabled.Checked = shouldDelete;
                int delayMs = RegistryHelper.LoadQADeleteDelayMs();
                NumDeleteDelayMs.Value = delayMs;
                _logger?.Log($"[INFO] プロンプトMSG自動削除フラグ (ChkAutoDeleteEnabled) を [{shouldDelete}] に設定!!", (int)LogType.Debug);
                _logger?.Log($"[INFO] プロンプトMSG削除遅延時間 (NumDeleteDelayMs) を [{delayMs}]ms に設定!!", (int)LogType.Debug);
            }
            //コントロールの有効/無効
            _logger?.Log($"[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateControlsState();
            _logger?.Log("[INFO] LoadSettingsメソッドを終了!!", (int)LogType.Debug);
        }
        //カラー選択ボタン押下時
        private void BtnSelectColor_Click(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] BtnSelectColor_Clickイベントを開始!!", (int)LogType.Debug);
            //TxtEmbedColorから16進数コードのみを抽出してColorDialogに設定
            try
            {
                //TxtEmbedColorの内容を取得
                string currentText = TxtEmbedColor.Text.Trim();

                //最初に見つかった6桁の16進数コードを抽出
                Match match = Regex.Match(currentText, @"#?([0-9A-F]{6})", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string cleanHex = match.Groups[1].Value;
                    colorDialog.Color = ColorTranslator.FromHtml("#" + cleanHex);
                    _logger?.Log($"[INFO] TxtEmbedColorから既存の16進数 [{cleanHex}] を抽出し、ColorDialogに設定しました!!", (int)LogType.Debug);
                }
                else
                {
                    _logger?.Log($"[INFO] TxtEmbedColor [{currentText}] から有効な16進数コードを抽出できませんでした!! デフォルト色が使用されます!!", (int)LogType.Debug);
                }
            }
            catch (Exception ex)
            {
                //パース失敗時 (ColorTranslator.FromHtmlで無効な値など) はログに残すが、ColorDialogのデフォルト色が使われるため処理は続行
                _logger?.Log($"[WARNING] 既存色の設定中に例外発生!! (デフォルト色が使用されます)\n{ex.Message}", (int)LogType.DebugError);
                //パース失敗時は ColorDialog のデフォルト色が使われる
            }

            _logger?.Log("[INFO] ColorDialogを表示!!", (int)LogType.Debug);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                //選択された色をプレビューとTextBoxに反映
                Color selectedColor = colorDialog.Color;
                PnlColorPreview.BackColor = selectedColor;

                //16進数と色名を併記して表示
                string colorDisplayString = GetColorDisplayString(selectedColor);
                TxtEmbedColor.Text = colorDisplayString;
                _logger?.Log($"[SUCCESS] ユーザーが色を選択しました!! 設定色：[{selectedColor.ToArgb():X8}] ({colorDisplayString})", (int)LogType.Debug);
            }
            else
            {
                _logger?.Log("[INFO] ユーザーがColorDialogをキャンセルしました!!", (int)LogType.Debug);
            }
            _logger?.Log("[INFO] BtnSelectColor_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //保存ボタン押下時
        private void BtnSave_Click(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger?.Log($"[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);
            //1．埋め込み色を取得・保存
            try
            {
                _logger?.Log("[INFO] 埋め込み色設定の検証と抽出を開始!!", (int)LogType.Debug);
                //TxtEmbedColor の内容から16進数コードのみを抽出
                Match match = Regex.Match(TxtEmbedColor.Text.Trim(), @"#?([0-9A-F]{6})", RegexOptions.IgnoreCase);
                string hexCode = "";

                if (match.Success)
                {
                    hexCode = match.Groups[1].Value.ToUpper();
                    _logger?.Log($"[INFO] TxtEmbedColorから抽出されたHEXコード：[{hexCode}]", (int)LogType.Debug);
                }
                else
                {
                    _logger?.Log($"[WARNING] TxtEmbedColorから6桁のHEXコードを抽出できませんでした!! 入力値：[{TxtEmbedColor.Text.Trim()}]", (int)LogType.Debug);
                }
                if (string.IsNullOrEmpty(hexCode) || hexCode.Length != 6)
                {
                    MessageBox.Show("有効な色コードが設定されていません!!\nボタンで色を選択してください!!", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _logger?.Log("[INFO] 有効な色コードが設定されていません!! 処理を中断!!", (int)LogType.Debug);
                    _logger?.Log($"[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
                    return;
                }

                //抽出したHEXコードを元にColorオブジェクトを生成
                Color selectedColor = ColorTranslator.FromHtml("#" + hexCode);
                _logger?.Log($"[INFO] HEXコード [{hexCode}] からColorオブジェクトを生成!! RGB：[{selectedColor.R}, {selectedColor.G}, {selectedColor.B}]", (int)LogType.Debug);
                //Color.ToArgb() から下位24ビットのRGB値(uint)を抽出して保存
                uint colorValue = (uint)(selectedColor.R << 16 | selectedColor.G << 8 | selectedColor.B);
                RegistryHelper.SaveQAEmbedColor(colorValue);
                _logger?.Log($"[SUCCESS] 埋め込み色を保存しました!! 値：[{colorValue}]", (int)LogType.Debug);

                //2．期限を保存
                _logger?.Log("[INFO] 期限設定 (NumTimeoutMinutes) の保存を開始!!", (int)LogType.Debug);
                int timeoutMinutes = (int)NumTimeoutMinutes.Value;
                RegistryHelper.SaveQATimeoutMinutes(timeoutMinutes);
                _logger?.Log($"[SUCCESS] 期限 (NumTimeoutMinutes) を [{timeoutMinutes}]分で保存!!", (int)LogType.Debug);

                //3．複数投票の許可設定を保存
                _logger?.Log("[INFO] 複数投票許可設定の保存を開始!!", (int)LogType.Debug);
                if (this.ChkAllowMultipleVotes != null)
                {
                    bool allowMultiple = ChkAllowMultipleVotes.Checked;
                    RegistryHelper.SaveAllowMultipleVotesSetting(allowMultiple);
                    _logger?.Log($"[SUCCESS] 複数投票許可設定 (ChkAllowMultipleVotes) を [{allowMultiple}] で保存!!", (int)LogType.Debug);
                }

                //4．プロンプトMSG自動削除フラグ＆数値を保存
                _logger?.Log("[INFO] 自動削除設定の保存を開始!!", (int)LogType.Debug);
                if (this.ChkAutoDeleteEnabled != null)
                {
                    //ShouldDelete の保存
                    bool shouldDelete = ChkAutoDeleteEnabled.Checked;
                    RegistryHelper.SaveQAShouldDelete(shouldDelete);

                    //DeleteDelayMs の保存
                    int deleteDelayMs = (int)NumDeleteDelayMs.Value;
                    RegistryHelper.SaveQADeleteDelayMs(deleteDelayMs);

                    _logger?.Log($"[SUCCESS] 自動削除フラグを [{shouldDelete}]、遅延時間 (DeleteDelayMs) を [{deleteDelayMs}]ms で保存!!", (int)LogType.Debug);
                }

                MessageBox.Show("QAアンケート設定を保存しました!!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _logger?.Log("[INFO] QAアンケート設定を保存しました!!", (int)LogType.Debug);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定の保存中にエラーが発生しました：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger?.Log($"[ERROR] 設定の保存中にエラーが発生しました!!\n例外：{ex.Message}", (int)LogType.DebugError);
            }
            _logger?.Log($"[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //チェックボックスの状態が変更されたときに実行されるイベントハンドラ
        private void ChkAutoDeleteEnabled_CheckedChanged(object sender, EventArgs e)
        {
            _logger?.Log("[INFO] ChkAutoDeleteEnabled_CheckedChangedイベントを開始!!", (int)LogType.Debug);
            _logger?.Log($"[INFO] UpdateControlsStateメソッドを呼び出し!!", (int)LogType.Debug);
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
                Properties.Settings.Default.QASetting_FormX = width;
                Properties.Settings.Default.QASetting_FormY = height;
                Properties.Settings.Default.QASetting_PositionX = formPosition.X;
                Properties.Settings.Default.QASetting_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void QASetting_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] QASetting_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.QASetting_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.QASetting_FormX;
                int formY = Properties.Settings.Default.QASetting_FormY;
                int positionX = Properties.Settings.Default.QASetting_PositionX;
                int positionY = Properties.Settings.Default.QASetting_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.QASetting_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.QASetting_Init = false;
                        _logger.Log($"[INFO] フォームの初期化フラグを[{Properties.Settings.Default.QASetting_Init}]に設定!!", (int)LogType.Debug);
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
                _logger.Log($"[INFO] QASetting_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void QASetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] QASetting_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] QASetting_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [QASetting] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

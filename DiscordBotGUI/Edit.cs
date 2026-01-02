using DiscordBot.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    public partial class Edit : Form
    {
        private CommandSettings _settings;
        private Dictionary<string, CommandSetting> _commands;
        //ILoggerを保持するためのプライベートフィールド
        private readonly ILogger _logger;
        private bool isActivated = false;
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
        //通常メッセージ用フィールド追加
        public class CommandSetting
        {
            public string CommandName { get; set; } = string.Empty;
            public string SendMessage { get; set; } = string.Empty;
            //埋め込みメッセージ用フィールド追加
            public string EmbedTitle { get; set; } = string.Empty;
            public string EmbedDescription { get; set; } = string.Empty;
            public string EmbedColorHex { get; set; } = "#FFFFFF";
        }
        public class CommandSettings
        {
            public List<CommandSetting> Commands { get; set; } = new List<CommandSetting>();
        }
        public Edit(ILogger logger)
        {
            InitializeComponent();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "[Edit] Loggerインスタンスはnullにできません!!");
            _settings = ReadCommandSettings();
            _logger.Log("-------------------- [Edit] フォームが初期化されました!! --------------------", (int)LogType.Debug);
        }
        //JSON読み込みロジック
        private CommandSettings ReadCommandSettings()
        {
            if (!File.Exists(ConfigDirectory))
            {
                _logger.Log($"[ERROR] 設定ファイルが存在しません!!", (int)LogType.DebugError);
                //ファイルが存在しない場合は、空のリストを返す
                MessageBox.Show($"設定ファイルが存在しません!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new CommandSettings();
            }

            try
            {
                string json = File.ReadAllText(ConfigDirectory);

                //Newtonsoft.Json.JsonConvert.DeserializeObject を使用
                var settings = JsonConvert.DeserializeObject<CommandSettings>(json);

                return settings ?? new CommandSettings();
            }
            catch (Exception ex)
            {
                _logger.Log($"[ERROR] 設定ファイルの読み込みエラー!!\n{ex.Message}", (int)LogType.DebugError);
                MessageBox.Show($"設定ファイルの読み込みエラー：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new CommandSettings();
            }
        }
        //JSON書き込みロジック
        private void WriteCommandSettings(CommandSettings settings)
        {
            try
            {
                //1．フォルダの存在確認と作成
                string directory = Path.GetDirectoryName(ConfigDirectory);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //2．JSONファイル書き込み (読み込みを待たずに書き込む)
                string jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(ConfigDirectory, jsonString);

                MessageBox.Show("コマンド設定をJSONファイルに保存しました!!", "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //3．メモリ上のDictionaryを最新の状態に更新
                _commands = new Dictionary<string, CommandSetting>();
                foreach (var cmd in settings.Commands)
                {
                    _commands[cmd.CommandName] = cmd;
                }

                _logger.Log($"[INFO] コマンド設定をJSONファイルに保存しました!!", (int)LogType.Debug);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの保存エラー：{ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.Log($"[ERROR] 設定ファイルの保存エラー\n{ex.Message}", (int)LogType.DebugError);
            }
        }
        //ロードメソッド
        private void Edit_Load(object sender, EventArgs e)
        {
            //イベント開始ログ
            _logger.Log($"[INFO] Edit_Loadイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] UpdateListBoxメソッドを呼び出し!!", (int)LogType.Debug);
            //既存のコマンドを ListBox に追加し、ソート
            UpdateListBox(null);
            //ListBoxには CommandSetting オブジェクト全体が格納されるが、表示は CommandName フィールドの値
            CmdListBox.DisplayMember = "CommandName";
            _logger.Log($"[INFO] ListBoxの表示メンバーを [CommandName] に設定!!", (int)LogType.Debug);
            if (_settings.Commands.Count > 0)
            {
                //最初のコマンドを選択状態にする (SelectedIndexChanged が発火し、編集パネルに表示される)
                CmdListBox.SelectedIndex = 0;
                _logger.Log($"[INFO] コマンド数：[{_settings.Commands.Count} 件], インデックス0のコマンドを選択!!", (int)LogType.Debug);
                if (CmdListBox.SelectedItem is CommandSetting selectedSetting)
                {
                    _logger.Log($"[INFO] LoadSettingToEditPanelメソッドを呼び出し!!", (int)LogType.Debug);
                    LoadSettingToEditPanel(selectedSetting);
                }
            }
            else
            {
                _logger.Log($"[INFO] コマンドが存在しないため、ClearEditPanelメソッドを呼び出し!!", (int)LogType.Debug);
                ClearEditPanel();
            }
            _logger.Log($"[INFO] Edit_Loadイベントを終了!!", (int)LogType.Debug);
        }
        //カラーボタン押下時
        private void BtnSelectColor_Click(object sender, EventArgs e)
        {
            //イベント開始ログ
            _logger.Log($"[INFO] BtnSelectColor_Clickイベントを開始!!", (int)LogType.Debug);
            try
            {
                //TxtEmbedColorHex の値から Color 構造体に変換
                if (!string.IsNullOrWhiteSpace(TxtEmbedColorHex.Text))
                {
                    string currentHex = TxtEmbedColorHex.Text;
                    _logger.Log($"[INFO] 現在のHEXコード [{currentHex}] をColorDialogの初期色に設定!!", (int)LogType.Debug);
                    //HexコードをWinFormsのColorに変換
                    colorDialog1.Color = System.Drawing.ColorTranslator.FromHtml(currentHex);
                }
            }
            catch (Exception ex)
            {
                //変換失敗時（不正なHexコードなど）はデフォルトの黒を設定
                colorDialog1.Color = System.Drawing.Color.Black;
                _logger.Log($"[ERROR] HEXコードの変換に失敗したため、ColorDialogの初期色を黒に設定しました!!\n{ex.Message}", (int)LogType.Error);
            }
            //ColorDialog を表示
            _logger.Log($"[INFO] ColorDialogを表示!!", (int)LogType.Debug);
            //ColorDialog を表示
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                //ユーザーが色を選択し「OK」を押した場合
                _logger.Log($"[INFO] ColorDialogで「OK」が押下されました!!", (int)LogType.Debug);
                //1．選択された System.Drawing.Color を取得
                System.Drawing.Color selectedColor = colorDialog1.Color;

                //2．16進数カラーコード（#RRGGBB形式）に変換
                //ColorTranslator.ToHtml() を使用すると最も簡単に #RRGGBB 形式が得られる
                string hexColor = System.Drawing.ColorTranslator.ToHtml(selectedColor);
                _logger.Log($"[INFO] 選択された色 [{selectedColor}] をHEXコード [{hexColor}] に変換!!", (int)LogType.Debug);
                //3．テキストボックスに設定
                TxtEmbedColorHex.Text = hexColor;
                _logger.Log($"[INFO] 新しいカラーコード [{hexColor}] をテキストボックスに設定!!", (int)LogType.Debug);
            }
            else
            {
                _logger.Log($"[INFO] ColorDialogで「キャンセル」または閉じる操作が行われました!!", (int)LogType.Debug);
            }
            //イベント終了ログ
            _logger.Log($"[INFO] BtnSelectColor_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //カラーテキストコードが変更されるたびに色パネルを変えるメソッド
        private void TxtEmbedColorHex_TextChanged(object sender, EventArgs e)
        {
            //テキストが変更されるたびにプレビューを更新
            UpdateColorPreview();
        }
        //色プレビューを更新するヘルパーメソッド
        private void UpdateColorPreview()
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] UpdateColorPreviewメソッドを開始!!", (int)LogType.Debug);
            try
            {
                string hexText = TxtEmbedColorHex.Text;
                _logger.Log($"[INFO] TxtEmbedColorHexの値：[{hexText}]", (int)LogType.Debug);
                //TxtEmbedColorHex の値から Color 構造体に変換
                if (!string.IsNullOrWhiteSpace(hexText))
                {
                    //HexコードをWinFormsのColorに変換
                    System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(hexText);
                    //Panelの背景色を設定
                    PnlColorPreview.BackColor = color;
                    _logger.Log($"[INFO] HEXコード [{hexText}] を色に変換し、プレビューパネルを [{color}] に設定!!", (int)LogType.Debug);
                }
                else
                {
                    //テキストが空の場合はデフォルトの色（例：白）に戻す
                    PnlColorPreview.BackColor = System.Drawing.Color.White;
                    _logger.Log($"[INFO] HEXコードが空欄のため、プレビューパネルをデフォルトの [白] に設定!!", (int)LogType.Debug);
                }
            }
            catch (Exception ex)
            {
                //変換失敗時（不正なHexコードなど）は灰色を設定
                PnlColorPreview.BackColor = System.Drawing.Color.Gray;
                _logger.Log($"[ERROR] HEXコードの変換に失敗しました。パネルを [灰色] に設定!!\n{ex.Message}", (int)LogType.DebugError);
            }
            // メソッド終了ログ
            _logger.Log($"[INFO] UpdateColorPreviewメソッドを終了!!", (int)LogType.Debug);
        }
        //編集パネルの全入力をクリア
        private void ClearEditPanel()
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] ClearEditPanelメソッドを開始!!", (int)LogType.Debug);
            TxtCommandName.Text = string.Empty;
            TxtSendMessage.Text = string.Empty;
            TxtEmbedTitle.Text = string.Empty;
            TxtEmbedDescription.Text = string.Empty;
            TxtEmbedColorHex.Text = "#FFFFFF";
            _logger.Log($"[INFO] 編集パネルの全テキストボックスをクリアし、色を #FFFFFF (白) に初期化!!", (int)LogType.Debug);
            _logger.Log($"[INFO] UpdateColorPreviewメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateColorPreview();
            //メソッド終了ログ
            _logger.Log($"[INFO] ClearEditPanelメソッドを終了!!", (int)LogType.Debug);
        }
        //選択されたコマンドの設定を編集パネルに表示
        private void LoadSettingToEditPanel(CommandSetting setting)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] LoadSettingToEditPanelメソッドを開始!!", (int)LogType.Debug);
            if (setting == null)
            {
                _logger.Log($"[ERROR] 設定オブジェクトがnullのため、ClearEditPanelメソッドを呼び出し!!", (int)LogType.DebugError);
                ClearEditPanel();
                //処理終了
                _logger.Log($"[INFO] LoadSettingToEditPanelメソッドを終了!!", (int)LogType.Debug);
                return;
            }

            //フォームに値をセット
            _logger.Log($"[INFO] コマンド設定：[{setting.CommandName}] を編集パネルにロード中...", (int)LogType.Debug);
            TxtCommandName.Text = setting.CommandName;
            TxtSendMessage.Text = setting.SendMessage;
            TxtEmbedTitle.Text = setting.EmbedTitle;
            TxtEmbedDescription.Text = setting.EmbedDescription;
            TxtEmbedColorHex.Text = setting.EmbedColorHex;
            _logger.Log($"[INFO] UpdateColorPreviewメソッドを呼び出し!!", (int)LogType.Debug);
            UpdateColorPreview();
            _logger.Log($"[INFO] 各コントロールへの値セットとカラープレビューの更新が完了!!", (int)LogType.Debug);
            //コマンドの内容に基づいて編集タブを切り替える
            bool hasEmbedContent = !string.IsNullOrWhiteSpace(setting.EmbedTitle) || !string.IsNullOrWhiteSpace(setting.EmbedDescription);
            _logger.Log($"[INFO] 埋め込みコンテンツの有無をチェック中：hasEmbedContent = [{hasEmbedContent}]", (int)LogType.Debug);
            if (hasEmbedContent)
            {
                //埋め込みメッセージ編集タブへ(インデックス 1)
                EditTabControl.SelectedIndex = 1;
                _logger.Log($"[INFO] 埋め込みコンテンツがあるため、タブをインデックス1 に切り替えました!!", (int)LogType.Debug);
            }
            else
            {
                //通常メッセージ編集タブへ(インデックス 0)
                EditTabControl.SelectedIndex = 0;
                _logger.Log($"[INFO] 埋め込みコンテンツがないため、タブをインデックス0 に切り替えました!!", (int)LogType.Debug);
            }
            // メソッド終了ログ
            _logger.Log($"[INFO] LoadSettingToEditPanelメソッドを終了!!", (int)LogType.Debug);
        }
        //コマンドが変更された際に ListBox を再ソートし、選択を維持
        private void UpdateListBox(CommandSetting selectAfter)
        {
            _logger.Log($"[INFO] UpdateListBoxメソッドを開始!!", (int)LogType.Debug);
            //1．コマンドリストをカスタム比較子(NaturalCommandComparer)でソートする
            _logger.Log($"[INFO] コマンドリストをNaturalCommandComparerでソート中...", (int)LogType.Debug);
            var sortedList = _settings.Commands
                //CommandSettingオブジェクト自体を比較
                .OrderBy(c => c, new NaturalCommandComparer(_logger))
                .ToList();
            _logger.Log($"[INFO] ソート完了!! 項目数：[{sortedList.Count}]", (int)LogType.Debug);
            //2．ListBoxを一旦クリアして、ソート済みのリストを再追加
            _logger.Log($"[INFO] ListBoxをクリアし、ソート済みリストを再追加します!!", (int)LogType.Debug);
            CmdListBox.Items.Clear();
            foreach (var cmd in sortedList)
            {
                CmdListBox.Items.Add(cmd);
            }
            _logger.Log($"[INFO] ListBoxへの項目追加が完了!!", (int)LogType.Debug);
            //3．変更後のコマンドを選択状態にする
            if (selectAfter != null)
            {
                _logger.Log($"[INFO] 選択対象のコマンドを設定中：[{selectAfter}]", (int)LogType.Debug);
                CmdListBox.SelectedItem = selectAfter;
                _logger.Log($"[INFO] コマンドを選択状態にしました!!", (int)LogType.Debug);
            }
            else
            {
                _logger.Log($"[INFO] selectAfterがnullのため、項目選択はスキップしました!!", (int)LogType.Debug);
            }

            //メソッド終了ログ
            _logger.Log($"[INFO] UpdateListBoxメソッドを終了!!", (int)LogType.Debug);
        }
        //自然順ソートを実現するためのカスタムIComparerクラス
        public class NaturalStringComparer : IComparer<string>
        {
            private readonly ILogger _logger;
            //数字の塊を抽出するための正規表現
            private static readonly Regex _re = new Regex(@"(\d+)|(\D+)", RegexOptions.Compiled);
            //コンストラクタでロガーを受け取る
            public NaturalStringComparer(ILogger logger)
            {
                _logger = logger;
            }
            public int Compare(string a, string b)
            {
                //比較開始ログ
                _logger.Log($"[INFO] NaturalStringComparer.Compare('{a}', '{b}')メソッドを開始!!", (int)LogType.Debug);
                if (a == null || b == null)
                {
                    int nullCompare = StringComparer.Ordinal.Compare(a, b);
                    _logger.Log($"[INFO] NULL比較の結果：[{nullCompare}] a==null：[{a == null}], b==null：[{b == null}])", (int)LogType.Debug);
                    //ログ挿入
                    return nullCompare;
                }

                var aMatches = _re.Matches(a);
                var bMatches = _re.Matches(b);

                int i = 0;
                while (i < aMatches.Count && i < bMatches.Count)
                {
                    Match aMatch = aMatches[i];
                    Match bMatch = bMatches[i];

                    _logger.Log($"[INFO] パート：[{i}] a=[{aMatch.Value}], b=[{bMatch.Value}]", (int)LogType.Debug);

                    //両方が数字の塊の場合
                    if (aMatch.Groups[1].Success && bMatch.Groups[1].Success)
                    {
                        //数値として比較する
                        if (int.TryParse(aMatch.Value, out int aNum) && int.TryParse(bMatch.Value, out int bNum))
                        {
                            int numCompare = aNum.CompareTo(bNum);
                            if (numCompare != 0)
                            {
                                _logger.Log($"[INFO] パート：[{i}] 数値比較結果 [{numCompare}] で確定!!", (int)LogType.Debug);
                                return numCompare;
                            }
                        }
                    }
                    //両方が文字列の塊、または数字解析に失敗した場合
                    else
                    {
                        //文字列として比較する
                        int strCompare = StringComparer.OrdinalIgnoreCase.Compare(aMatch.Value, bMatch.Value);
                        if (strCompare != 0)
                        {
                            _logger.Log($"[INFO] パート：[{i}] 文字列比較結果 [{strCompare}] で確定!!", (int)LogType.Debug);
                            return strCompare;
                        }
                    }
                    i++;
                }

                //片方がもう一方の部分文字列である場合
                int finalCompare = a.Length.CompareTo(b.Length);
                _logger.Log($"[INFO] 全パート一致!! 長さ比較の結果：[{finalCompare}]", (int)LogType.Debug);
                _logger.Log($"[INFO] NaturalStringComparer.Compare('{a}', '{b}')メソッドを終了!!", (int)LogType.Debug);
                return finalCompare;
            }
        }
        //CommandSettingオブジェクトを比較するためのIComparer実装(OrderByで使用するため)
        public class NaturalCommandComparer : IComparer<CommandSetting>
        {
            private readonly NaturalStringComparer _stringComparer;
            private readonly ILogger _logger;

            //コンストラクタでロガーを受け取り、それをNaturalStringComparerに渡す
            public NaturalCommandComparer(ILogger logger)
            {
                _logger = logger;
                _stringComparer = new NaturalStringComparer(logger);
            }

            public int Compare(CommandSetting x, CommandSetting y)
            {
                _logger.Log($"[INFO] NaturalCommandComparer.Compareメソッドを開始!!", (int)LogType.Debug);
                if (x == null || y == null)
                {
                    _logger.Log($"[INFO] NaturalCommandComparer：nullの比較が行われました!!", (int)LogType.Debug);
                    return 0;
                }
                int result = _stringComparer.Compare(x.CommandName, y.CommandName);
                _logger.Log($"[INFO] NaturalCommandComparer：'{x.CommandName}', '{y.CommandName}' の比較結果：[{result}]", (int)LogType.Debug);
                _logger.Log($"[INFO] NaturalCommandComparer.Compareメソッドを終了!!", (int)LogType.Debug);
                return result;
            }
        }
        //コマンド保存ボタン
        private void BtnSave_Click(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] BtnSave_Clickイベントを開始!!", (int)LogType.Debug);
            //選択されているコマンドを取得
            if (!(CmdListBox.SelectedItem is CommandSetting selectedSetting))
            {
                _logger.Log($"[ERROR] コマンドが選択されていません!! 保存処理を中断します!!", (int)LogType.DebugError);
                MessageBox.Show("保存するコマンドを選択するか、新規作成してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string oldCommandName = selectedSetting.CommandName;
            string commandName = TxtCommandName.Text.Trim();
            _logger.Log($"[INFO] 選択されたコマンド名：[{oldCommandName}], 入力されたコマンド名：[{commandName}]", (int)LogType.Debug);
            if (string.IsNullOrWhiteSpace(commandName))
            {
                MessageBox.Show("コマンド名を入力してください!!", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //コマンド名の先頭に "!" がなければ追加する
            if (!commandName.StartsWith("!"))
            {
                commandName = "!" + commandName;
                //ユーザーに反映させるため、テキストボックスの値も更新する
                TxtCommandName.Text = commandName;
                _logger.Log($"[INFO] コマンド名にプレフィックスを自動追加!! [{commandName}]", (int)LogType.Debug);
            }

            //コマンド名が変更されたかどうかのフラグ
            bool nameChanged = !commandName.Equals(selectedSetting.CommandName, StringComparison.Ordinal);
            _logger.Log($"[INFO] コマンド名変更フラグ：[{nameChanged}]", (int)LogType.Debug);
            //コマンド名が変更された場合の重複チェック
            if (nameChanged)
            {
                _logger.Log($"[INFO] コマンド名変更のため、重複チェックを開始：[{commandName}]", (int)LogType.Debug);
                if (_settings.Commands.Any(c => c != selectedSetting && c.CommandName.Equals(commandName, StringComparison.Ordinal)))
                {
                    _logger.Log($"[ERROR] コマンド名：[{commandName}] は既に存在します!! 元のコマンド名に戻しました!!", (int)LogType.DebugError);
                    MessageBox.Show("そのコマンド名は既に存在します!!", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //元のコマンド名に戻す
                    TxtCommandName.Text = selectedSetting.CommandName;
                    return;
                }
            }

            //フォームの内容を現在選択されているオブジェクトに上書き
            selectedSetting.CommandName = commandName;
            selectedSetting.SendMessage = TxtSendMessage.Text;
            selectedSetting.EmbedTitle = TxtEmbedTitle.Text;
            selectedSetting.EmbedDescription = TxtEmbedDescription.Text;
            string inputColorText = TxtEmbedColorHex.Text;
            string finalHexColor = "#FFFFFF";
            _logger.Log($"[INFO] 編集内容をオブジェクトに適用中!! Send：[{selectedSetting.SendMessage.Length} 文字], EmbedTitle：[{selectedSetting.EmbedTitle}]", (int)LogType.Debug);
            try
            {
                //入力されたテキスト（色の名前やHexコードなど）を System.Drawing.Color に変換
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(inputColorText);

                //System.Drawing.Color から #RRGGBB 形式の文字列に変換
                finalHexColor = System.Drawing.ColorTranslator.ToHtml(color);
                _logger.Log($"[INFO] カラーコード変換成功：入力=[{inputColorText}] -> 出力=[{finalHexColor}]", (int)LogType.Debug);
            }
            catch
            {
                //変換に失敗した場合はデフォルト値(#FFFFFF)が finalHexColor に残る
                _logger.Log($"[INFO] カラーコードの変換に失敗!! デフォルト値 [#FFFFFF] を使用!!", (int)LogType.DebugError);
            }

            selectedSetting.EmbedColorHex = finalHexColor;
            _logger.Log($"[INFO] コマンド設定：[{commandName}] の変更を確定しました!!", (int)LogType.Debug);
            _logger.Log($"[INFO] WriteCommandSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            //JSONファイルに保存
            WriteCommandSettings(_settings);
            //コマンド名が変更された場合、ListBoxの表示を更新する
            if (nameChanged)
            {
                _logger.Log($"[INFO] コマンド名変更に伴い、ListBoxの再ソートと選択を更新!!", (int)LogType.Debug);
                _logger.Log($"[INFO] UpdateListBoxメソッドを呼び出し!!", (int)LogType.Debug);
                UpdateListBox(selectedSetting);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] BtnSave_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //リストボックスの項目をクリック時
        private void CmdListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] CmdListBox_SelectedIndexChangedイベントを開始!!", (int)LogType.Debug);
            //選択された項目を CommandSetting オブジェクトとして取得
            if (CmdListBox.SelectedItem is CommandSetting selectedSetting)
            {
                //選択された項目がある場合のログ
                _logger.Log($"[INFO] コマンドが選択されました!!：[{selectedSetting.CommandName}]", (int)LogType.Debug);
                //CommandName をテキストボックスに表示する前にログ出力
                _logger.Log($"[INFO] TxtCommandName にコマンド名を設定します!!: [{selectedSetting.CommandName}]", (int)LogType.Debug);
                TxtCommandName.Text = selectedSetting.CommandName;
            }
            else
            {
                //項目が選択されていない場合のログ
                _logger.Log($"[INFO] 選択されているコマンドがありません!!(選択解除または空リスト)", (int)LogType.Debug);
                //項目が選択されていない場合は、コマンド名テキストボックスをクリア
                TxtCommandName.Text = string.Empty;
                //ClearEditPanel() 呼び出しのログ
                _logger.Log($"[INFO] ClearEditPanelメソッドを呼び出し!!", (int)LogType.Debug);
                ClearEditPanel();
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] CmdListBox_SelectedIndexChangedイベントを終了!!", (int)LogType.Debug);
        }
        //リストボックスの項目をダブルクリック時
        private void CmdListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] CmdListBox_MouseDoubleClickイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] クリック座標：[X:{e.X}, Y:{e.Y}]", (int)LogType.Debug);
            //マウスが押された位置にある項目を取得
            int index = CmdListBox.IndexFromPoint(e.Location);

            if (index != ListBox.NoMatches && index < CmdListBox.Items.Count)
            {
                _logger.Log($"[INFO] 項目：[{index}] がダブルクリックされました!!", (int)LogType.Debug);
                //ダブルクリックされた項目を選択状態にする (SelectedIndexChanged も発火する)
                CmdListBox.SelectedIndex = index;
                _logger.Log($"[INFO] ListBox.SelectedIndex を [{index}] に設定しました!!", (int)LogType.Debug);
                //選択された項目を CommandSetting オブジェクトとして取得
                if (CmdListBox.SelectedItem is CommandSetting selectedSetting)
                {
                    //ClearEditPanel() 呼び出しのログ
                    _logger.Log($"[INFO] LoadSettingToEditPanelメソッドを呼び出し!!", (int)LogType.Debug);
                    //編集パネルへ内容を読み込む (タブ切り替えもここで行われる)
                    LoadSettingToEditPanel(selectedSetting);
                }
                else
                {
                    _logger.Log($"[ERROR] Index：[{index}] に対応する項目は CommandSetting オブジェクトではありませんでした!!", (int)LogType.DebugError);
                }
            }
            else
            {
                //項目がない部分をダブルクリックした場合のログ
                _logger.Log($"[INFO] 項目がない部分がダブルクリックされたので処理をスキップ!! [index：{index}]", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] CmdListBox_MouseDoubleClickイベントを終了!!", (int)LogType.Debug);
        }
        //コマンド追加ボタン
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] BtnAdd_Clickイベントを開始!!", (int)LogType.Debug);
            const string baseName = "!new_command";
            _logger.Log($"[INFO] ベースコマンド名：[{baseName}]", (int)LogType.Debug);
            _logger.Log($"[INFO] 既存コマンドから連番の最大値抽出を開始!!", (int)LogType.Debug);
            //1．既存のコマンドから連番部分を数値として抽出する
            int maxNumber = _settings.Commands
                //コマンド名を取得
                .Select(c => c.CommandName)
                //"!new_command_"で始まるものをフィルタ
                .Where(name => name.StartsWith(baseName + "_", StringComparison.OrdinalIgnoreCase))
                .Select(name => {
                    //"!"new_command_12" の "12" 部分を抜き出す
                    string numberPart = name.Substring(baseName.Length + 1);
                    if (int.TryParse(numberPart, out int number))
                    {
                        return number;
                    }
                    //解析できなかった場合は 0
                    return 0;
                })
                //リストが空の場合は 0 を返す
                .DefaultIfEmpty(0)
                //最大値を取得
                .Max();
            //最大連番の抽出結果ログ
            _logger.Log($"[INFO] 抽出された最大連番：[{maxNumber}]", (int)LogType.Debug);
            //2．新しいコマンド名を設定
            string newCommandName;
            if (maxNumber == 0 && !_settings.Commands.Any(c => c.CommandName.Equals(baseName, StringComparison.OrdinalIgnoreCase)))
            {
                //"!new_command" が存在しない、かつ連番もない場合は、まず連番なしで作成
                newCommandName = baseName;
                _logger.Log($"[INFO] 連番なしのベース名：[{baseName}] が存在しないため、新規コマンド名として採用!!", (int)LogType.Debug);
            }
            else
            {
                //最大連番の次の番号を使用
                newCommandName = $"{baseName}_{maxNumber + 1}";
                _logger.Log($"[INFO] 連番：[{maxNumber + 1}] を使用し、新規コマンド名：[{newCommandName}] を設定!!", (int)LogType.Debug);
            }

            //新しいコマンドを作成
            var newSetting = new CommandSetting { CommandName = newCommandName };
            _logger.Log($"[INFO] CommandSetting オブジェクトを作成しました!! CommandName：[{newCommandName}]", (int)LogType.Debug);
            //メモリ上の設定に追加
            _settings.Commands.Add(newSetting);
            _logger.Log($"[INFO] 設定コレクションに新しいコマンドを追加しました!!", (int)LogType.Debug);
            //UpdateListBox() 呼び出しのログ
            _logger.Log($"[INFO] UpdateListBoxメソッドを呼び出し!!", (int)LogType.Debug);
            //ListBoxを更新し、新しいコマンドを選択
            UpdateListBox(newSetting);

            //編集に備える
            TxtCommandName.Focus();
            TxtCommandName.SelectAll();
            _logger.Log($"[INFO] TxtCommandName にフォーカスし、テキストを全選択しました!!", (int)LogType.Debug);
            //メソッド終了ログ
            _logger.Log($"[INFO] BtnAdd_Clickイベントを終了!!", (int)LogType.Debug);
        }
        //コマンド削除ボタン
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] BtnDelete_Clickイベントを開始!!", (int)LogType.Debug);
            if (CmdListBox.SelectedItem is CommandSetting selectedSetting)
            {
                _logger.Log($"[INFO] 削除対象コマンド：[{selectedSetting.CommandName}]", (int)LogType.Debug);
                DialogResult result = MessageBox.Show(
                    $"コマンド '{selectedSetting.CommandName}' を本当に削除しますか？",
                    "コマンドの削除",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                _logger.Log($"[INFO] 削除確認ダイアログの結果：[{result}]", (int)LogType.Debug);
                if (result == DialogResult.Yes)
                {
                    _logger.Log($"[INFO] ユーザーが削除を承認しました!! コマンド：[{selectedSetting.CommandName}] の削除を実行します!!", (int)LogType.Debug);
                    //メモリ上の設定リストと ListBox から削除
                    _settings.Commands.Remove(selectedSetting);
                    CmdListBox.Items.Remove(selectedSetting);
                    _logger.Log($"[INFO] メモリ上の _settings.Commands と ListBox からコマンドを削除しました!!", (int)LogType.Debug);
                    _logger.Log($"[INFO] WriteCommandSettingsメソッドを呼び出し!!", (int)LogType.Debug);
                    //JSONファイルに保存
                    WriteCommandSettings(_settings);
                    _logger.Log($"[INFO] ClearEditPanelメソッドを呼び出し!!", (int)LogType.Debug);
                    ClearEditPanel();
                    _logger.Log($"[INFO] コマンド [{selectedSetting.CommandName}] の削除処理が完了しました!!", (int)LogType.Debug);
                }
                else
                {
                    _logger.Log($"[INFO] ユーザーが削除をキャンセルしました!!", (int)LogType.Debug);
                }
            }
            else
            {
                _logger.Log($"[ERROR] 削除するコマンドが選択されていません!!", (int)LogType.DebugError);
                MessageBox.Show("削除するコマンドを選択してください!!", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] BtnDelete_Clickイベントを終了!!", (int)LogType.Debug);
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
                Properties.Settings.Default.Edit_FormX = width;
                Properties.Settings.Default.Edit_FormY = height;
                Properties.Settings.Default.Edit_PositionX = formPosition.X;
                Properties.Settings.Default.Edit_PositionY = formPosition.Y;
                _logger.Log($"[INFO] フォームサイズ：X軸[{width}],Y軸[{height}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム座標：X軸[{formPosition.X}],Y軸[{formPosition.Y}]", (int)LogType.Debug);
                Properties.Settings.Default.Save();
                _logger.Log("[INFO] フォームのX,Y座標取得完了!!", (int)LogType.Debug);
            }
            //メソッド終了ログ
            _logger.Log($"[INFO] SaveFormSettingsイベントを終了!!", (int)LogType.Debug);
        }
        //フォームアクティベート時
        private void Edit_Activated(object sender, EventArgs e)
        {
            if (!isActivated)
            {
                //メソッド開始ログ
                _logger.Log($"[INFO] Edit_Activatedイベントを開始!!", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームアクティベート処理中...", (int)LogType.Debug);
                _logger.Log($"[INFO] フォーム初回起動：[{!isActivated}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの終了位置記憶設定：[{Properties.Settings.Default.FormSetting}]", (int)LogType.Debug);
                _logger.Log($"[INFO] フォームの初期化フラグ：[{Properties.Settings.Default.Edit_Init}]", (int)LogType.Debug);
                int formX = Properties.Settings.Default.Edit_FormX;
                int formY = Properties.Settings.Default.Edit_FormY;
                int positionX = Properties.Settings.Default.Edit_PositionX;
                int positionY = Properties.Settings.Default.Edit_PositionY;
                if (Properties.Settings.Default.FormSetting == true)
                {
                    if (Properties.Settings.Default.Edit_Init == true)
                    {
                        //フォームの初期サイズを設定
                        this.Size = new System.Drawing.Size(formX, formY);
                        _logger.Log($"[INFO] フォームサイズ：X軸[{formX}],Y軸[{formY}]", (int)LogType.Debug);
                        //フォームの起動位置を画面の中央に設定
                        this.StartPosition = FormStartPosition.CenterScreen;
                        _logger.Log($"[INFO] フォーム起動位置を画面中央に設定!!", (int)LogType.Debug);
                        Properties.Settings.Default.Edit_Init = false;
                        _logger.Log($"[INFO] フォームの初期化フラグを[{Properties.Settings.Default.Edit_Init}]に設定!!", (int)LogType.Debug);
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
                _logger.Log($"[INFO] Edit_Activatedイベントを終了!!", (int)LogType.Debug);
            }
        }
        //フォームクローズ時
        private void Edit_FormClosing(object sender, FormClosingEventArgs e)
        {
            //メソッド開始ログ
            _logger.Log($"[INFO] Edit_FormClosingイベントを開始!!", (int)LogType.Debug);
            _logger.Log($"[INFO] SaveFormSettingsメソッドを呼び出し!!", (int)LogType.Debug);
            SaveFormSettings();
            //メソッド終了ログ
            _logger.Log($"[INFO] Edit_FormClosingイベントを終了!!", (int)LogType.Debug);
            _logger.Log("-------------------- [Edit] フォーム終了処理完了!! --------------------", (int)LogType.Debug);
        }
    }
}

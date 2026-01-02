using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordBotGUI
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //グローバルな例外ハンドラを設定
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        //UIスレッドでの未処理例外をキャッチ
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show($"UIスレッドで未処理の例外が発生しました!!\n{e.Exception.Message}\n\nStackTrace:\n{e.Exception.StackTrace}", "致命的なエラー");
            // ログファイルへの書き込みも推奨
        }

        //バックグラウンドスレッドでの未処理例外をキャッチ
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"バックグラウンドスレッドで未処理の例外が発生しました!!\n{ex?.Message}\n\nStackTrace:\n{ex?.StackTrace}", "致命的なエラー");
            // ログファイルへの書き込みも推奨
        }
    }
}

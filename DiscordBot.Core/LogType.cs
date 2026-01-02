namespace DiscordBot.Core
{
    //MainForm と DLLプラグインで共有するためのログタイプ定義
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
}

namespace DiscordBot.Core
{
    public interface ILogger
    {
        //LogType の代わりに int を使用し、値だけを渡す
        void Log(string message, int typeValue);
    }
}
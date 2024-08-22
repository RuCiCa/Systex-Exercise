using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace WebApplication1.Common
{
    public static class Logger
    {
        private static readonly object lockObj = new object();
        private static readonly HashSet<string> loggedMessages = new HashSet<string>();
        private static readonly TimeSpan errorCacheDuration = TimeSpan.FromMinutes(5);
        private static readonly Dictionary<string, DateTime> messageTimestamps = new Dictionary<string, DateTime>();

        private static string GetLogFilePath()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return $"{date}.log";
        }
        private static string GetLogLevelFilePath(string levelStr)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return $"{date}-{levelStr}.log";
        }

        public static void Log(int level, string result, string message, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string levelStr = LevelConvert(level);
            string fileName = Path.GetFileName(filePath);
            string log = $"[{levelStr}] {date}, {fileName}-{memberName}-{lineNumber}, {result}, {message}";

            lock (lockObj)
            {
                try
                {
                    string logFilePath = GetLogFilePath();
                    string LevelPath = GetLogLevelFilePath(levelStr);
                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        writer.WriteLine(log);
                    }
                    using (StreamWriter writer = new StreamWriter(LevelPath, true))
                    {
                        writer.WriteLine(log);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private static string LevelConvert(int level)
        {
            string levelStr = "";
            switch (level)
            {
                case 0:
                    levelStr = "Trace";
                    break;
                default:
                    levelStr = "Debug";
                    break;
                case 2:
                    levelStr = "Info";
                    break;
                case 3:
                    levelStr = "Warn";
                    break;
                case 4:
                    levelStr = "Error";
                    break;
                case 5:
                    levelStr = "Fatal";
                    break;
            }
            return levelStr;
        }
    }
}

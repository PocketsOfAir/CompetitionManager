using System.Reflection;

namespace CompetitionManager.Util
{
    internal sealed class LoggingService
    {
        private static readonly LoggingService _instance = new();
        public static LoggingService Instance => _instance;

        private string FileName { get; }

        private LoggingService()
        {
            FileName = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration", $"Log_{DateTime.Now:yyyy-MM-dd HH.mm.ss.fff}.txt");
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
            File.AppendAllText(FileName, message);
            File.AppendAllText(FileName, "\n");
        }
    }
}

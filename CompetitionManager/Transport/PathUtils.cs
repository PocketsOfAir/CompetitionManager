using System.Reflection;

namespace CompetitionManager.Transport
{
    internal static class PathUtils
    {
        public static string GetOutputFilePath(string filename)
        {
            var directoryPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Output");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return Path.Join(directoryPath, filename);
        }

        public static string GetLogFilePath(string filename)
        {
            var directoryPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Logs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return Path.Join(directoryPath, filename);
        }

        public static string GetConfigFilePath(string filename)
        {
            var directoryPath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration");
            return Path.Join(directoryPath, filename);
        }
    }
}

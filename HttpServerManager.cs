using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public static class HttpServerManager
    {
        public static void StartServer()
        {
            int port = 8080;
            string baseDirectory = Path.Combine(Application.StartupPath, @"themes\superstar\_www");

            CleanUpDirectory(baseDirectory);
            HttpServer.StartServer(baseDirectory, port, Program.songListManager);
        }

        private static void CleanUpDirectory(string baseDirectory)
        {
            string[] directoriesToKeep = { "css", "fonts", "superstar-pic", "手機點歌" };

            var allDirectories = Directory.GetDirectories(baseDirectory);
            var allFiles = Directory.GetFiles(baseDirectory);

            var filesToKeep = allFiles
                .Where(file => file.EndsWith(".html"))
                .Select(file => Path.GetFileName(file))
                .ToArray();

            foreach (var dir in allDirectories)
            {
                var dirName = Path.GetFileName(dir);
                if (!directoriesToKeep.Contains(dirName))
                {
                    Directory.Delete(dir, true);
                }
            }

            foreach (var file in allFiles)
            {
                var fileName = Path.GetFileName(file);
                if (!filesToKeep.Contains(fileName))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
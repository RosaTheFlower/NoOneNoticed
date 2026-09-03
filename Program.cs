using NoOneNoticed.Export;
using NoOneNoticed.SystemMonitor;
using System.Diagnostics;

namespace NoOneNoticed
{
    internal class Program
    {
        /// <summary>
        /// Parses command-line arguments into monitor configuration values.
        /// Supported flags: --range (polling interval in ms), --ram (RAM warning threshold in MB), --path (folder to watch).
        /// </summary>
        /// <param name="args">Raw command-line arguments passed to the application.</param>
        /// <returns>A tuple containing the polling interval, RAM threshold, and path to watch.</returns>
        static (int pollingMs, long ramMb, string watchPath) ParseArgs(string[] args)
        {
            int pollingMs = 2000;
            long ramMb = 500;
            string watchPath = @"C:\";

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--range" when i + 1 < args.Length:
                        if (int.TryParse(args[i + 1], out var parsedInterval))
                            pollingMs = parsedInterval;
                        i++;
                        break;

                    case "--ram" when i + 1 < args.Length:
                        if (long.TryParse(args[i + 1], out var parsedRam))
                            ramMb = parsedRam;
                        i++;
                        break;

                    case "--path" when i + 1 < args.Length:
                        watchPath = args[i + 1];
                        i++;
                        break;
                }
            }

            return (pollingMs, ramMb, watchPath);
        }

        /// <summary>
        /// Application entry point. Starts the file and process watchers, listens for the stop key (ESC),
        /// then exports a report and optionally opens it if CTRL+ESC was pressed.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args)
        {
            var (pollingMs, ramMb, watchPath) = ParseArgs(args);
            var fileWatcher = new FileWatcher(watchPath);
            var processWatcher = new ProcessWatcher(pollingMs, ramMb);
            fileWatcher.Start();
            processWatcher.Start();

            Console.WriteLine("Monitoring... Press ESC to stop (SHIFT+ESC to stop and open the report).");

            bool openFile = false;
            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    openFile = keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift);
                    break;
                }
            }

            fileWatcher.Stop();
            processWatcher.Stop();

            var reportPath = ReportExporter.Export();
            Console.WriteLine($"Report saved at: {reportPath}");

            if (openFile)
            {
                OpenFile(reportPath);
            }
        }

        /// <summary>
        /// Opens the given file using the operating system's default associated application.
        /// </summary>
        /// <param name="path">Full path to the file to open.</param>
        static void OpenFile(string path)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
    }
}
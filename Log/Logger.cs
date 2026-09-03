using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoOneNoticed.Log
{
    /// <summary>
    /// Represents a single logged event, capturing when it happened, its severity,
    /// which watcher produced it, and a human-readable description.
    /// </summary>
    public class LogEntry
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public LogLevel Level { get; init; }
        public string Source { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }

    /// <summary>
    /// Severity levels used to classify logged events, from routine activity
    /// to internal monitoring failures.
    /// </summary>
    public enum LogLevel
    {
        Info = 1,
        Warning = 2,
        Danger = 3,
        Error = 4
    }

    /// <summary>
    /// Application-wide singleton responsible for recording log entries in memory
    /// and printing them to the console in real time, color-coded by severity.
    /// </summary>
    public class Logger
    {
        private static readonly Logger _instance = new Logger();

        /// <summary>
        /// The single shared instance of the logger.
        /// </summary>
        public static Logger Instance => _instance;

        private readonly List<LogEntry> _history = new();

        /// <summary>
        /// Read-only view of every log entry recorded since the application started.
        /// </summary>
        public IReadOnlyList<LogEntry> History => _history;

        private Logger() { }

        /// <summary>
        /// Records a new log entry: validates the input, stores it in the in-memory
        /// history, and writes it to the console.
        /// </summary>
        /// <param name="level">Severity of the event.</param>
        /// <param name="source">Component that generated the event (e.g. "FileWatcher").</param>
        /// <param name="message">Human-readable description of the event.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="source"/> or <paramref name="message"/> is null or whitespace.</exception>
        public void Log(LogLevel level, string source, string message)
        {

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentException("Source cannot be null or whitespace.", nameof(source));
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be null or whitespace.", nameof(message));
            }

            LogEntry newLogEntry = new LogEntry
            {
                Level = level,
                Source = source,
                Message = message
            };

            _history.Add(newLogEntry);
            WriteLog(newLogEntry);
        }

        /// <summary>
        /// Writes a log entry to the console, using a distinct color per severity level.
        /// </summary>
        private void WriteLog(LogEntry logEntry)
        {
            switch (logEntry.Level)
            {
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Danger:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ResetColor();
                    break;
            }

            Console.WriteLine($"[{logEntry.Timestamp}] [{logEntry.Level}] [{logEntry.Source}] - {logEntry.Message}");
            Console.ResetColor();
        }
    }
}
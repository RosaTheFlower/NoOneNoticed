using NoOneNoticed.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace NoOneNoticed.SystemMonitor
{
    /// <summary>
    /// Watches a directory (and its subdirectories) for file and folder creation,
    /// deletion, and rename events, logging each event through the Logger.
    /// </summary>
    public class FileWatcher
    {
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        /// Creates a new file watcher for the given root path.
        /// </summary>
        /// <param name="pathToWatch">Root directory to monitor, including all subdirectories.</param>
        public FileWatcher(string pathToWatch)
        {
            _watcher = new FileSystemWatcher(pathToWatch)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = false
            };

            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;
            _watcher.Error += OnError;
        }

        /// <summary>
        /// Starts raising file system events.
        /// </summary>
        public void Start() => _watcher.EnableRaisingEvents = true;

        /// <summary>
        /// Stops raising file system events.
        /// </summary>
        public void Stop() => _watcher.EnableRaisingEvents = false;

        /// <summary>
        /// Logs a file or folder creation event.
        /// </summary>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Logger.Instance.Log(LogLevel.Info, "FileWatcher", $"Created: {e.FullPath}");
        }

        /// <summary>
        /// Logs a file or folder deletion event.
        /// </summary>
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Logger.Instance.Log(LogLevel.Danger, "FileWatcher", $"Deleted: {e.FullPath}");
        }

        /// <summary>
        /// Logs a file or folder rename event.
        /// </summary>
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Logger.Instance.Log(LogLevel.Info, "FileWatcher", $"Renamed: {e.OldFullPath} -> {e.FullPath}");
        }

        /// <summary>
        /// Logs an internal watcher error, typically caused by an internal buffer
        /// overflow when too many file system events occur in a short period.
        /// </summary>
        private void OnError(object sender, ErrorEventArgs e)
        {
            Logger.Instance.Log(LogLevel.Error, "FileWatcher", $"Monitoring error: {e.GetException().Message}");
        }
    }
}
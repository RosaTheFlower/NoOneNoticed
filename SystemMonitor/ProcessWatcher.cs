using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NoOneNoticed.Log;

namespace NoOneNoticed.SystemMonitor
{
    /// <summary>
    /// Represents a point-in-time snapshot of a single process's resource usage,
    /// used to calculate deltas between polling cycles.
    /// </summary>
    public class ProcessSnapshot
    {
        public string Name { get; init; } = string.Empty;
        public TimeSpan CpuTimeAtLastCheck { get; init; }
        public DateTime CheckedAt { get; init; }
        public long RamBytes { get; init; }
    }

    /// <summary>
    /// Periodically polls running system processes to detect new/terminated processes
    /// and to flag processes exceeding CPU or RAM usage thresholds.
    /// </summary>
    public class ProcessWatcher
    {
        private Dictionary<int, ProcessSnapshot> _lastSnapshot = new();
        private readonly int _pollingIntervalMs;
        private readonly long _ramWarningThresholdBytes;
        private CancellationTokenSource? _cts;

        /// <summary>
        /// Creates a new process watcher.
        /// </summary>
        /// <param name="pollingIntervalMs">Interval, in milliseconds, between each process check.</param>
        /// <param name="ramWarningThresholdMb">RAM usage threshold, in megabytes, above which a warning is logged.</param>
        public ProcessWatcher(int pollingIntervalMs = 2000, long ramWarningThresholdMb = 500)
        {
            _pollingIntervalMs = pollingIntervalMs;
            _ramWarningThresholdBytes = ramWarningThresholdMb * 1024 * 1024;
        }

        /// <summary>
        /// Starts the polling loop in the background.
        /// </summary>
        public void Start()
        {
            _cts = new CancellationTokenSource();
            _ = PollingLoopAsync(_cts.Token);
        }

        /// <summary>
        /// Signals the polling loop to stop.
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// Runs the polling loop until cancellation is requested, checking processes
        /// on each cycle and waiting between checks.
        /// </summary>
        /// <param name="token">Token used to stop the loop.</param>
        private async Task PollingLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    CheckProcesses();
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(LogLevel.Error, "ProcessWatcher", $"Polling error: {ex.Message}");
                }

                try
                {
                    await Task.Delay(_pollingIntervalMs, token);
                }
                catch (TaskCanceledException)
                {
                    // Expected when Stop() is called.
                }
            }
        }

        /// <summary>
        /// Takes a fresh snapshot of all running processes, compares it against the
        /// previous snapshot to detect started/terminated processes and resource
        /// usage above the configured thresholds, then stores it as the new baseline.
        /// </summary>
        private void CheckProcesses()
        {
            var currentSnapshot = new Dictionary<int, ProcessSnapshot>();
            var now = DateTime.Now;

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    currentSnapshot[process.Id] = new ProcessSnapshot
                    {
                        Name = process.ProcessName,
                        CpuTimeAtLastCheck = process.TotalProcessorTime,
                        CheckedAt = now,
                        RamBytes = process.WorkingSet64
                    };
                }
                catch
                {
                    continue;
                }
            }

            // Detect newly started processes.
            foreach (var kvp in currentSnapshot)
            {
                int pid = kvp.Key;
                var current = kvp.Value;

                if (!_lastSnapshot.ContainsKey(pid))
                {
                    Logger.Instance.Log(LogLevel.Info, "ProcessWatcher", $"Process started: {current.Name} (PID {pid})");
                }
            }

            // Detect processes that have terminated.
            foreach (var kvp in _lastSnapshot)
            {
                int pid = kvp.Key;
                var previous = kvp.Value;

                if (!currentSnapshot.ContainsKey(pid))
                {
                    Logger.Instance.Log(LogLevel.Info, "ProcessWatcher", $"Process terminated: {previous.Name} (PID {pid})");
                }
            }

            // Check resource usage for processes still running.
            foreach (var kvp in currentSnapshot)
            {
                int pid = kvp.Key;
                var current = kvp.Value;

                if (_lastSnapshot.TryGetValue(pid, out var previous))
                {
                    double deltaCpuMs = (current.CpuTimeAtLastCheck - previous.CpuTimeAtLastCheck).TotalMilliseconds;
                    double deltaRealMs = (current.CheckedAt - previous.CheckedAt).TotalMilliseconds;

                    if (deltaRealMs > 0)
                    {
                        double cpuPercent = (deltaCpuMs / (deltaRealMs * Environment.ProcessorCount)) * 100;

                        if (cpuPercent > 50)
                        {
                            Logger.Instance.Log(LogLevel.Warning, "ProcessWatcher",
                                $"{current.Name} (PID {pid}) using {cpuPercent:F1}% CPU");
                        }
                    }

                    if (current.RamBytes > _ramWarningThresholdBytes)
                    {
                        Logger.Instance.Log(LogLevel.Warning, "ProcessWatcher",
                            $"{current.Name} (PID {pid}) using {current.RamBytes / (1024 * 1024)}MB RAM");
                    }
                }
            }

            _lastSnapshot = currentSnapshot;
        }
    }
}
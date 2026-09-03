using NoOneNoticed.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace NoOneNoticed.Export
{
    /// <summary>
    /// Generates a plain-text report from the logger's recorded history and
    /// saves it to the system's temporary folder.
    /// </summary>
    public class ReportExporter
    {
        /// <summary>
        /// Writes every recorded log entry to a timestamped report file in the
        /// system temp folder.
        /// </summary>
        /// <returns>The full path of the generated report file.</returns>
        public static string Export()
        {
            var folder = Path.GetTempPath();
            var fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var fullPath = Path.Combine(folder, fileName);

            var sb = new StringBuilder();
            sb.AppendLine("Monitoring Report");
            sb.AppendLine($"Created at: {DateTime.Now}");
            sb.AppendLine($"Entries: {Logger.Instance.History.Count}");
            sb.AppendLine();

            foreach (var entry in Logger.Instance.History)
            {
                sb.AppendLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Level}] [{entry.Source}] - {entry.Message}");
            }

            File.WriteAllText(fullPath, sb.ToString());

            return fullPath;
        }
    }
}
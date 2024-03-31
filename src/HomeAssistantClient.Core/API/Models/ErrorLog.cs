using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace HomeAssistantClient.Core.API.Models;

/// <summary>
/// Represents an error log entry.
/// </summary>
[PublicAPI]
public partial class ErrorLog
{
    /// <summary>
    /// Gets a Regex object representing a new log line.
    /// </summary>
    private static Regex _newLogLineRegex = LogLineRegex();

    /// <summary>
    /// Gets a Regex object representing an error line.
    /// </summary>
    private static Regex _logTypeError = LogTypeRegex();

    /// <summary>
    /// Gets a Regex object representing a warning line.
    /// </summary>
    private static Regex _logTypeWarning = LogTypeWarningRegex();

    /// <summary>
    /// Gets the full, raw log text.
    /// </summary>
    public string? RawLog { get; private set; }

    /// <summary>
    /// Gets the list of log entries.
    /// </summary>
    public List<string> LogEntries { get; private set; }

    /// <summary>
    /// Gets only log lines with a type of "ERROR".
    /// </summary>
    public List<string> Errors => LogEntries.Where(e => _logTypeError.IsMatch(e)).ToList();

    /// <summary>
    /// Gets only log lines with a type of "WARNING".
    /// </summary>
    public List<string> Warnings => LogEntries.Where(e => _logTypeWarning.IsMatch(e)).ToList();

    /// <summary>
    /// Gets the most recent <paramref name="count" /> of entries, sorted newest first.
    /// </summary>
    /// <param name="count">The number of entries to retrieve.</param>
    /// <returns>A list of log entries of the specified <paramref name="count" />, newest first.</returns>
    public List<string> this[int count] => LogEntries.AsEnumerable().Reverse().Take(count).ToList();

    private static readonly char[] Separator = ['\n'];

    /// <summary>
    /// Initializes a new instance of the error log object with the specified <paramref name="log" /> data.
    /// </summary>
    /// <param name="log">The raw log data to parse.</param>
    public ErrorLog(string? log)
    {
            RawLog = log;
            LogEntries = ParseLog(log);
        }

    /// <summary>
    /// Parses the raw log text and splits each entry out based on whether or not the line starts with a date.
    /// </summary>
    /// <param name="log">The log text to parse.</param>
    /// <returns>A parsed list of log entries.</returns>
    private static List<string> ParseLog(string? log)
    {
            List<string> entries = [];
            if (string.IsNullOrEmpty(log)) return entries;
            
            var currentLine = "";

            foreach (var line in log.Replace("\r", "").Split(Separator, StringSplitOptions.RemoveEmptyEntries))
            {
                // If the first part of the entry is a date-like thing (DDDD-DD-DD), then consider this a "new" entry.
                // Sample: 2015-12-20 11:02:50 homeassistant.components.recorder: Found unfinished sessions
                if (_newLogLineRegex.IsMatch(line) && !string.IsNullOrWhiteSpace(currentLine))
                {
                    entries.Add(currentLine.Trim());
                    currentLine = "";
                }

                currentLine += line + "\r\n";
            }
            entries.Add(currentLine.Trim());

            return entries;
        }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2}", RegexOptions.Compiled)]
    private static partial Regex LogLineRegex();
        
    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} ERROR", RegexOptions.Compiled)]
    private static partial Regex LogTypeRegex();
        
    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} WARNING", RegexOptions.Compiled)]
    private static partial Regex LogTypeWarningRegex();
}
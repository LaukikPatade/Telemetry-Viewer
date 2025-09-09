using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace Telemetry_demo_Avalonia.Utils
{
    /// <summary>
    /// Logging utility for the application with message box display and file logging
    /// </summary>
    public static class AppLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppPaths.ApplicationBaseDirectory, "app.log");
        private static readonly object LogLock = new object();

        /// <summary>
        /// Log levels for different types of messages
        /// </summary>
        public enum LogLevel
        {
            Info,
            Success,
            Warning,
            Error
        }

        /// <summary>
        /// Shows a message box with the specified message and level
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="level">The log level</param>
        /// <param name="title">Optional custom title</param>
        /// <returns>Task that completes when the message box is closed</returns>
        public static async Task ShowMessageAsync(string message, LogLevel level = LogLevel.Info, string? title = null)
        {
            var icon = level switch
            {
                LogLevel.Success => Icon.Success,
                LogLevel.Warning => Icon.Warning,
                LogLevel.Error => Icon.Error,
                _ => Icon.Info
            };

            var defaultTitle = level switch
            {
                LogLevel.Success => "Success",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                _ => "Information"
            };

            var box = MessageBoxManager.GetMessageBoxStandard(
                title ?? defaultTitle,
                message,
                ButtonEnum.Ok,
                icon
            );

            await box.ShowWindowAsync();
        }

        /// <summary>
        /// Shows a confirmation dialog with Yes/No buttons
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="title">Optional custom title</param>
        /// <returns>True if user clicked Yes, false if No</returns>
        public static async Task<bool> ShowConfirmationAsync(string message, string title = "Confirm")
        {
            var box = MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.YesNo,
                Icon.Question
            );

            var result = await box.ShowWindowAsync();
            return result == ButtonResult.Yes;
        }

        /// <summary>
        /// Logs a message to the application log file
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        /// <param name="exception">Optional exception to include</param>
        public static void LogToFile(string message, LogLevel level = LogLevel.Info, Exception? exception = null)
        {
            try
            {
                lock (LogLock)
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var levelText = level.ToString().ToUpper();
                    var logEntry = $"[{timestamp}] [{levelText}] {message}";
                    
                    if (exception != null)
                    {
                        logEntry += $"\nException: {exception.Message}\nStackTrace: {exception.StackTrace}";
                    }
                    
                    logEntry += "\n";
                    
                    File.AppendAllText(LogFilePath, logEntry, Encoding.UTF8);
                }
            }
            catch
            {
                // If logging fails, we don't want to crash the application
                // Could potentially show a fallback message box here
            }
        }

        /// <summary>
        /// Logs a message and optionally shows a message box
        /// </summary>
        /// <param name="message">The message to log and display</param>
        /// <param name="level">The log level</param>
        /// <param name="showMessageBox">Whether to show a message box</param>
        /// <param name="title">Optional custom title for message box</param>
        /// <param name="exception">Optional exception to include in log</param>
        public static async Task LogAsync(string message, LogLevel level = LogLevel.Info, bool showMessageBox = false, string? title = null, Exception? exception = null)
        {
            // Log to file
            LogToFile(message, level, exception);

            // Show message box if requested
            if (showMessageBox)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await ShowMessageAsync(message, level, title);
                });
            }
        }

        /// <summary>
        /// Logs an information message
        /// </summary>
        public static void LogInfo(string message, bool showMessageBox = false)
        {
            _ = LogAsync(message, LogLevel.Info, showMessageBox);
        }

        /// <summary>
        /// Logs a success message
        /// </summary>
        public static void LogSuccess(string message, bool showMessageBox = false)
        {
            _ = LogAsync(message, LogLevel.Success, showMessageBox);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public static void LogWarning(string message, bool showMessageBox = false)
        {
            _ = LogAsync(message, LogLevel.Warning, showMessageBox);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public static void LogError(string message, Exception? exception = null, bool showMessageBox = false)
        {
            _ = LogAsync(message, LogLevel.Error, showMessageBox, exception: exception);
        }

        /// <summary>
        /// Ensures the log file directory exists
        /// </summary>
        public static void EnsureLogDirectoryExists()
        {
            try
            {
                var logDir = Path.GetDirectoryName(LogFilePath);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
            }
            catch
            {
                // If we can't create the log directory, we'll continue without logging
            }
        }

        /// <summary>
        /// Clears the application log file
        /// </summary>
        public static void ClearLogFile()
        {
            try
            {
                lock (LogLock)
                {
                    if (File.Exists(LogFilePath))
                    {
                        File.Delete(LogFilePath);
                    }
                }
            }
            catch
            {
                // If we can't clear the log, continue without it
            }
        }
    }
} 
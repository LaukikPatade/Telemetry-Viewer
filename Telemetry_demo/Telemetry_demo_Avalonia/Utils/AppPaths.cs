using System;
using System.IO;

namespace Telemetry_demo_Avalonia.Utils
{
    /// <summary>
    /// Utility class for managing application paths that work across different deployment scenarios
    /// </summary>
    public static class AppPaths
    {
        /// <summary>
        /// Gets the base directory where the application executable is located
        /// </summary>
        public static string ApplicationBaseDirectory
        {
            get
            {
                // Try to get the directory of the executing assembly first
                var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(assemblyLocation))
                {
                    var assemblyDir = Path.GetDirectoryName(assemblyLocation);
                    if (!string.IsNullOrEmpty(assemblyDir))
                        return assemblyDir;
                }

                // Fallback to AppDomain base directory
                var appDomainBase = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(appDomainBase))
                    return appDomainBase;

                // Final fallback to current directory
                return Directory.GetCurrentDirectory();
            }
        }

        /// <summary>
        /// Gets the path to the configs.json file
        /// </summary>
        public static string ConfigsFile => Path.Combine(ApplicationBaseDirectory, "configs.json");

        /// <summary>
        /// Gets the logs directory path
        /// </summary>
        public static string LogsDirectory => Path.Combine(ApplicationBaseDirectory, "logs");

        /// <summary>
        /// Gets the path to a specific log directory for a connection
        /// </summary>
        /// <param name="connectionName">The connection name</param>
        /// <returns>The path to the connection's log directory</returns>
        public static string GetLogDirectory(string connectionName) => 
            Path.Combine(LogsDirectory, connectionName ?? "default");

        /// <summary>
        /// Ensures that the logs directory exists
        /// </summary>
        public static void EnsureLogsDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(LogsDirectory))
                {
                    Directory.CreateDirectory(LogsDirectory);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Failed to create logs directory: {LogsDirectory}", ex);
            }
        }

        /// <summary>
        /// Ensures that a specific connection's log directory exists
        /// </summary>
        /// <param name="connectionName">The connection name</param>
        public static void EnsureConnectionLogDirectoryExists(string connectionName)
        {
            try
            {
                var logDir = GetLogDirectory(connectionName);
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Failed to create connection log directory for: {connectionName}", ex);
            }
        }

        /// <summary>
        /// Ensures that the application base directory exists
        /// </summary>
        public static void EnsureApplicationDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(ApplicationBaseDirectory))
                {
                    Directory.CreateDirectory(ApplicationBaseDirectory);
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Failed to create application directory: {ApplicationBaseDirectory}", ex);
            }
        }

        /// <summary>
        /// Ensures all required application directories exist
        /// </summary>
        public static void EnsureAllDirectoriesExist()
        {
            EnsureApplicationDirectoryExists();
            EnsureLogsDirectoryExists();
            AppLogger.EnsureLogDirectoryExists();
        }
    }
} 
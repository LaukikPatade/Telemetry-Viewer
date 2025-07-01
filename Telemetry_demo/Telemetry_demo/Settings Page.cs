using System;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace Telemetry_demo
{
    public partial class SettingsPage : UserControl
    {
        private const string CONFIG_FILE = "settings.json";
        private string configPath;

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                // Load the current config path from settings
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TelemetryViewer"
                );
                configPath = Path.Combine(appDataPath, "configs");
                
                // Create directory if it doesn't exist
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }

                txtConfigPath.Text = configPath;

                // Load log path from settings.json if it exists
                string settingsPath = Path.Combine(appDataPath, "settings.json");
                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    dynamic settings = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                    string logPath = settings?.logPath;
                    if (!string.IsNullOrWhiteSpace(logPath))
                        txtLogPath.Text = logPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Browse button clicked");
            try
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Configuration Directory";
                    folderDialog.SelectedPath = txtConfigPath.Text;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        configPath = folderDialog.SelectedPath;
                        txtConfigPath.Text = configPath;

                        // Create directory if it doesn't exist
                        if (!Directory.Exists(configPath))
                        {
                            Directory.CreateDirectory(configPath);
                        }

                        // Save the new path
                        SaveSettings();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings()
        {
            try
            {
                // Save the config path to settings
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TelemetryViewer",
                    CONFIG_FILE
                );

                // Create directory if it doesn't exist
                string settingsDir = Path.GetDirectoryName(settingsPath);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }

                // Save the path
                File.WriteAllText(settingsPath, configPath);

                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseLog_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select Logs Folder";
                    folderDialog.SelectedPath = txtLogPath.Text;

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtLogPath.Text = folderDialog.SelectedPath;
                        SaveLogPath();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting logs folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveLogPath()
        {
            try
            {
                // Save the log path to settings
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TelemetryViewer",
                    "settings.json"
                );

                // Load existing settings if present
                string configPath = txtConfigPath.Text;
                string logPath = txtLogPath.Text;
                var settings = new { configPath, logPath };
                string json = JsonConvert.SerializeObject(settings);
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving logs folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void txtConfigPath_TextChanged(object sender, EventArgs e)
        //{
        //    Console.WriteLine("Config path changed: " + txtConfigPath.Text);
        //    try
        //    {
        //        // Update the configPath in ConfigManager
        //        string newPath = Path.Combine(txtConfigPath.Text, "config.json");
        //        typeof(ConfigManager)
        //            .GetField("configPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
        //            ?.SetValue(null, newPath);

        //        // Save the new path to settings
        //        configPath = txtConfigPath.Text;
        //        SaveSettings();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error updating config path: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
    }
} 
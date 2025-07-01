using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Telemetry_demo
{
    internal class ConfigManager
    {

        private static string configPath = "";
        public ConfigManager() { }

        public static void SaveConfig(InputConfig config)
        {
            Console.WriteLine("Saving Configuration");

            List<InputConfig> configs = new List<InputConfig>();

            if (File.Exists(configPath) && new FileInfo(configPath).Length > 0)
            {
                string existingJson = File.ReadAllText(configPath);
                configs = JsonConvert.DeserializeObject<List<InputConfig>>(existingJson) ?? new List<InputConfig>();
            }

            configs.Add(config);

            string jsonData = JsonConvert.SerializeObject(configs, Formatting.Indented);
            File.WriteAllText(configPath, jsonData);
        }

        public static void SetConfigPath()
        {
            string dirPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TelemetryViewer",
                "configs"
            );
            // Ensure the directory exists
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string configPath_ = Path.Combine(dirPath, "config.json");
     

            // Ensure the file exists
            if (!File.Exists(configPath_))
            {
                // Create an empty JSON array as the initial content
                File.WriteAllText(configPath_, "[]");
            }

            // If you want to store this path in a class variable, do so here
            configPath = configPath_;
        }
        public static List<InputConfig> LoadConfigs()
        {
            if (File.Exists(configPath) && new FileInfo(configPath).Length > 0)
            {
                string json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<List<InputConfig>>(json) ?? new List<InputConfig>();
            }

            return new List<InputConfig>(); // Return an empty list if file doesn't exist or is empty
        }
        public static InputConfig searchConfig(string inputName)
        {
            List<InputConfig> configs = LoadConfigs();
            foreach (InputConfig config in configs)
            {
                if (string.IsNullOrEmpty(inputName)) continue;
                if (inputName==config.InputName) return config;
            }

            return null; // Return an empty list if file doesn't exist or is empty
        }


    }
}

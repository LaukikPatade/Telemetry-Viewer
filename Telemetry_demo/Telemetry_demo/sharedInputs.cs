using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry_demo
{
    internal class sharedInputs
    {
        public static Dictionary<string, InputConfig> Configurations { get; set; } = new Dictionary<string, InputConfig>();
        public static List<InputConfig> configs { get; } = new List<InputConfig>();

        public static void AddConfiguration(InputConfig config)
        {
            configs.Add(config);
            Configurations[config.InputName] = config;
        }
    }
}

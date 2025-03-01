using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry_demo
{
    internal class InputConfig
    {
        public string InputName { get; set; }
        public int BaudRate { get; set; }
        public string ComPort { get; set; }
        public Dictionary<string, object> Config { get; set; }

        public InputConfig(string comPort, int baudRate, string inputName)
        {
            InputName = inputName;
            BaudRate = baudRate;
            ComPort = comPort;
            Config = new Dictionary<string, object>
        {
            { "com_port", comPort },
            { "baud_rate", baudRate },
            { "input_name", inputName }
        };
        }
    }
}

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
        public string ConnectionType { get; set; }
        public int BaudRate { get; set; }
        public string Port { get; set; }
        public string InputMode { get; set; }

        public inputStructure ChannelConfig { get; set; }


        public InputConfig(string port, int baudRate, string inputMode, string inputName, string connectionType)
        {
            ConnectionType = connectionType;
            InputName = inputName;
            BaudRate = baudRate;
            Port = port;
            InputMode = inputMode;
            ChannelConfig = new inputStructure();
        }
    }
}

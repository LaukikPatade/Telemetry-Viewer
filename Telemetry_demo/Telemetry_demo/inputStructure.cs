using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry_demo
{
    public class ChannelInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    internal class inputStructure
    {
        public List<ChannelInfo> Channels { get; set; }

        public inputStructure()
        {
            Channels = new List<ChannelInfo>();
        }

        public void AddColumn(ChannelInfo channel)
        {
            if (channel != null && !string.IsNullOrWhiteSpace(channel.Name))
            {
                Channels.Add(channel);
            }
        }
    }
}

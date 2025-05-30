using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry_demo
{
    internal class inputStructure
    {
        public List <String> Channels { get; set; }

        public inputStructure()
        {
            Channels=new List<String>();
        }

        public void AddColumn(string channel)
        {
            if (!string.IsNullOrWhiteSpace(channel))
            {
                Channels.Add(channel);
            }
        }
        
    }
}

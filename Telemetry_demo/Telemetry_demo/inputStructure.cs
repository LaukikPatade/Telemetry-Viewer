using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry_demo
{
    internal class inputStructure
    {
        public List <String> ColumnNames { get; set; }

        public inputStructure()
        {
            ColumnNames=new List<String>();
        }

        public void AddColumn(string columnName)
        {
            if (!string.IsNullOrWhiteSpace(columnName))
            {
                ColumnNames.Add(columnName);
            }
        }
        
    }
}

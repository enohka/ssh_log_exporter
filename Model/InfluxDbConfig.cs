using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hp_ssh_exporter.Model
{
    public class InfluxDbConfig
    {
        public string Host { get; set; } = String.Empty;
        public string Token { get; set; } = String.Empty;
        public string Bucket { get; set; } = String.Empty;
        public string Org { get; set; } = String.Empty;
    }
}

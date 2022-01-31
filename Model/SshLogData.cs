using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hp_ssh_exporter.Model
{
    public class SshLogData
    {
        public string? IpAddress { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public ulong Time { get; set; } = 0;
    }
}

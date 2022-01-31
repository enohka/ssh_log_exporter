using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hp_ssh_exporter.Model
{
    public class SshLogScraperConfig
    {
        public double ScrapeIntervall { get; set; } = 10;
        public string LogLocation { get; set; } = string.Empty;
    }
}

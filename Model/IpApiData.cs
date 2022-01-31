using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hp_ssh_exporter.Model
{
    public class IpApiData
    {
        public string Query { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public float Lat { get; set; } = 0;
        public float Lon { get; set; } = 0;
        public string Timezone { get; set; } = string.Empty;
        public string Isp { get; set; } = string.Empty;
        public string Org { get; set; } = string.Empty;
        public string As {get; set;} = string.Empty;
    }
}

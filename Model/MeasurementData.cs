using InfluxDB.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hp_ssh_exporter.Model
{
    [Measurement("ssh_connection")]
    public class MeasurementData
    {
        [Column("ip_address")]public string? IpAddress { get; set; }
        
        [Column("country")] public string? Country { get; set; }
        [Column("region_name")] public string? RegionName { get; set; }
        [Column("city")] public string? City { get; set; }
        [Column("lat")] public float? Lat { get; set; }
        [Column("lon")] public float? Lon { get; set; }

        [Column("username")] public string? Username { get; set; }
        [Column("password")] public string? Password { get; set; }
        
        [Column(IsTimestamp = true)] public ulong Time { get; set; }
    }
}

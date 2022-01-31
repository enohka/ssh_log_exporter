using hp_ssh_exporter.Model;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace hp_ssh_exporter.Service
{
    public class InfluxdbClientService
    {
        private readonly InfluxDbConfig _options;
        private readonly ILogger _logger;

        public InfluxdbClientService(ILogger<InfluxdbClientService> logger, IOptions<InfluxDbConfig> opt)
        {
            _options = opt.Value;
            _logger = logger; 
        }

        public void WriteMeasurement(MeasurementData data)
        {
            using (var client = InfluxDBClientFactory.Create(_options.Host, _options.Token))
            {
                client.SetLogLevel(InfluxDB.Client.Core.LogLevel.Body);
                using (var writeApi = client.GetWriteApi())
                {
                    _logger.LogInformation("Sending measurement to influxdb...");
                    writeApi.WriteMeasurement(_options.Bucket, _options.Org, WritePrecision.Ms, data);
                }
            }
            _logger.LogInformation("done");
        }
    }
}



using AutoMapper;
using hp_ssh_exporter.Extensions;
using hp_ssh_exporter.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace hp_ssh_exporter.Service
{
    public class SshLogScraperService : IHostedService, IDisposable
    {
        private readonly SshLogScraperConfig _options;
        private readonly InfluxdbClientService _influxdbClient;
        private readonly IpApiService _ipApiService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        private readonly Timer _timer;

        public SshLogScraperService(ILogger<SshLogScraperService> logger,
                                    IOptions<SshLogScraperConfig> opt,
                                    InfluxdbClientService clientService,
                                    IpApiService apiService,
                                    IMapper mapper)
        {
            _options = opt.Value;
            _influxdbClient = clientService;
            _ipApiService = apiService;
            _logger = logger;
            _mapper = mapper;

            _timer = new Timer(Worker, null, Timeout.Infinite, 0);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting SSH Log scraper service..");

            _timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        private async void Worker(object? state)
        {
            _timer.Change(Timeout.Infinite, 0);

            if (!Directory.Exists(_options.LogLocation))
            {
                _logger.LogWarning($"Scraping failed: File {_options.LogLocation} does not exist.");
                return;
            }

            _logger.LogInformation($"Scraping folder {_options.LogLocation}");

            try
            {
                if (!Directory.Exists(Path.Combine(_options.LogLocation, "scraped")))
                {
                    Directory.CreateDirectory(Path.Combine(_options.LogLocation, "scraped"));
                }

                var di = new DirectoryInfo(_options.LogLocation);

                foreach (var file in di.GetFiles())
                {
                    _logger.LogInformation($"Scraping {file.FullName}");

                    using (var sr = new StreamReader(file.FullName))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var sshLogData = JsonSerializer.Deserialize<SshLogData>(sr.ReadLine(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                            var ipApiData = await _ipApiService.GetIpDataAsync(sshLogData.IpAddress);

                            var measurementData = _mapper.MergeInto<MeasurementData>(ipApiData, sshLogData);

                            _influxdbClient.WriteMeasurement(measurementData);
                        }
                    }

                    file.MoveTo(Path.Combine(_options.LogLocation, "scraped", file.Name));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while scraping: {ex}");
            }
            finally
            {
                _timer.Change(TimeSpan.FromSeconds(_options.ScrapeIntervall), TimeSpan.FromMilliseconds(-1));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);

            _logger.LogInformation("Stopped SSH Log scraper service..");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

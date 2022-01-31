using hp_ssh_exporter.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hp_ssh_exporter.Service
{
    public class IpApiService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public IpApiService(ILogger<IpApiService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://ip-api.com/json/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ISC SSH Honeypot project");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<IpApiData> GetIpDataAsync(string ip)
        {
            _logger.LogInformation($"Quering IP Api for {ip}");

            var response = await _httpClient.GetAsync(ip);

            var rl = response.Headers.Where(x => x.Key == "X-Rl").FirstOrDefault().Value.FirstOrDefault();
            var ttl = response.Headers.Where(x => x.Key == "X-Ttl").FirstOrDefault().Value.FirstOrDefault();

            if (!string.IsNullOrEmpty(rl))
            {
                var remainingQuota = Int32.Parse(rl);

                if (remainingQuota <= 1)
                {
                    var waitingTime = string.IsNullOrEmpty(ttl) ? 60 : Int32.Parse(ttl);

                    _logger.LogWarning($"IP Api Quota hit - usually 45 calls/second. Sleeping for {waitingTime + 5} seconds to prevent blacklisting");

                    Thread.Sleep(TimeSpan.FromSeconds(waitingTime + 5));
                }
            }

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Success");

                try
                {
                    var data = await JsonSerializer.DeserializeAsync<IpApiData>(response.Content.ReadAsStream(), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    return data;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to parse JSON response from IP Api: {ex}");

                    return new IpApiData();
                }
            }
            else
            {
                _logger.LogWarning($"Response code from {response?.RequestMessage?.RequestUri} was: {response?.StatusCode}");

                return new IpApiData();
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}

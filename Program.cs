using hp_ssh_exporter.Model;
using hp_ssh_exporter.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = new HostBuilder()
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         config.AddEnvironmentVariables();

         if (args != null)
         {
             config.AddCommandLine(args);

             var tempConfig = config.Build();

             var configFile = tempConfig.GetValue<string>("config");

             config.AddJsonFile(configFile);
         }
     })
     .ConfigureServices((hostContext, services) =>
     {
         services.AddOptions();

         services.Configure<InfluxDbConfig>(hostContext.Configuration.GetSection("influxdb"));
         services.Configure<SshLogScraperConfig>(hostContext.Configuration.GetSection("sshlogscraper"));

         services.AddAutoMapper(config =>
         {
             config.CreateMap<IpApiData, MeasurementData>();
             config.CreateMap<SshLogData, MeasurementData>();
         });

         services.AddTransient<InfluxdbClientService>();
         services.AddSingleton<IpApiService>();
         services.AddSingleton<IHostedService, SshLogScraperService>();
     })
     .ConfigureLogging((hostingContext, logging) =>
      {
          logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
          logging.AddConsole();
      });

await builder.RunConsoleAsync();


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TSF_mustidisProj.Data;

namespace TSF_mustidisProj.Services
{
    public class MqttBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MqttBackgroundService> _logger;

        public MqttBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<MqttBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MQTT Background Service is starting");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mqttService = scope.ServiceProvider.GetRequiredService<IMqttService>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Connect to MQTT broker
                    await mqttService.ConnectAsync();
                    
                    // Set up message handler
                    mqttService.MessageReceived += async (feed, value) => {
                        _logger.LogInformation("Processing MQTT message: {Feed} = {Value}", feed, value);
                        // Additional processing if needed
                        //return Task.CompletedTask;
                    };
                    
                    // Subscribe to all feeds in the database
                    var feeds = dbContext.Feeds.ToList();
                    foreach (var feed in feeds)
                    {
                        await mqttService.SubscribeAsync(feed.Key);
                        _logger.LogInformation("Subscribed to feed: {Feed}", feed.Key);
                    }
                    
                    // Keep the service running
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    }
                    
                    // Disconnect when stopping
                    await mqttService.DisconnectAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MQTT Background Service");
            }
            
            _logger.LogInformation("MQTT Background Service is stopping");
        }
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Text;
using System.Threading.Tasks;
using TSF_mustidisProj.Data;
using TSF_mustidisProj.Models;
using TSF_mustidisProj.Services;
using Microsoft.EntityFrameworkCore;

namespace TSF_mustidisProj.Services
{
    public class AdafruitMqttService : IMqttService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdafruitMqttService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private IManagedMqttClient _mqttClient;
        private string _username;
        private string _key;
        private bool _isConnected;

        public event Func<string, string, Task> MessageReceived;

        public AdafruitMqttService(
            IConfiguration configuration,
            ILogger<AdafruitMqttService> logger,
            ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = dbContext;
            
            _username = _configuration["Adafruit:Username"];
            _key = _configuration["Adafruit:Key"];
            
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_key))
            {
                throw new ArgumentException("Adafruit credentials not configured properly");
            }
        }

        public async Task ConnectAsync()
        {
            if (_mqttClient != null && _isConnected)
            {
                _logger.LogInformation("Already connected to Adafruit IO");
                return;
            }

            try
            {
                // Create client options
                var options = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                        .WithClientId(Guid.NewGuid().ToString())
                        .WithTcpServer("io.adafruit.com", 1883)
                        .WithCredentials(_username, _key)
                        .WithCleanSession()
                        .Build())
                    .Build();

                // Create client
                var factory = new MqttFactory();
                _mqttClient = factory.CreateManagedMqttClient();

                // Set up handlers
                _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;
                _mqttClient.ConnectedAsync += HandleConnected;
                _mqttClient.DisconnectedAsync += HandleDisconnected;

                // Connect
                await _mqttClient.StartAsync(options);
                _logger.LogInformation("Connecting to Adafruit IO MQTT broker...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Adafruit IO MQTT broker");
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_mqttClient != null)
            {
                await _mqttClient.StopAsync();
                _mqttClient.Dispose();
                _mqttClient = null;
                _isConnected = false;
                _logger.LogInformation("Disconnected from Adafruit IO MQTT broker");
            }
        }

        public async Task PublishAsync(string feed, string message)
        {
            if (_mqttClient == null || !_isConnected)
            {
                _logger.LogWarning("Not connected to MQTT broker. Attempting to connect...");
                await ConnectAsync();
            }

            try
            {
                string topic = $"{_username}/feeds/{feed}";
                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(message)
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .WithRetainFlag()
                    .Build();
                
                var managedMessage = new ManagedMqttApplicationMessageBuilder()
            .WithApplicationMessage(applicationMessage)
            .Build();

                await _mqttClient.EnqueueAsync(managedMessage);
                _logger.LogInformation("Published message to {Topic}: {Message}", topic, message);
                
                // Update the feed's last value in the database
                await UpdateFeedValue(feed, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message to {Feed}", feed);
                throw;
            }
        }

        public async Task SubscribeAsync(string feed)
        {
            if (_mqttClient == null || !_isConnected)
            {
                _logger.LogWarning("Not connected to MQTT broker. Attempting to connect...");
                await ConnectAsync();
            }

            try
            {
                string topic = $"{_username}/feeds/{feed}";
                await _mqttClient.SubscribeAsync(topic);
                _logger.LogInformation("Subscribed to {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to subscribe to {Feed}", feed);
                throw;
            }
        }

        public async Task UnsubscribeAsync(string feed)
        {
            if (_mqttClient == null || !_isConnected)
            {
                _logger.LogWarning("Client is not connected. Cannot unsubscribe.");
                return;
            }

            try
            {
                string topic = $"{_username}/feeds/{feed}";
                await _mqttClient.UnsubscribeAsync(topic);
                _logger.LogInformation("Unsubscribed from {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unsubscribe from {Feed}", feed);
                throw;
            }
        }

        private async Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs args)
        {
            try
            {
                string topic = args.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
                
                _logger.LogInformation("Message received: {Topic} - {Payload}", topic, payload);
                
                // Extract feed name from topic (format: username/feeds/feedname)
                string[] topicParts = topic.Split('/');
                if (topicParts.Length >= 3)
                {
                    string feedName = topicParts[2];
                    
                    // Update feed value in database
                    await UpdateFeedValue(feedName, payload);
                    
                    // Invoke the event
                    if (MessageReceived != null)
                    {
                        await MessageReceived.Invoke(feedName, payload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling received message");
            }
        }

        private async Task UpdateFeedValue(string feedKey, string value)
        {
            try
            {
                // Find the feed by key
                //var feed = await _dbContext.Feeds.FindAsync(feedKey); ///line 211
                var feed = await _dbContext.Feeds.FirstOrDefaultAsync(f => f.Key == feedKey);

                if (feed != null)
                {
                    feed.LastValue = value;
                    feed.RecordedAt = DateTime.UtcNow;
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Updated feed {Feed} value to {Value}", feedKey, value);
                }
                else
                {
                    _logger.LogWarning("Feed {Feed} not found in database", feedKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feed value in database");
            }
        }

        private Task HandleConnected(MqttClientConnectedEventArgs args)
        {
            _isConnected = true;
            _logger.LogInformation("Connected to Adafruit IO MQTT broker");
            return Task.CompletedTask;
        }

        private Task HandleDisconnected(MqttClientDisconnectedEventArgs args)
        {
            _isConnected = false;
            _logger.LogWarning("Disconnected from Adafruit IO MQTT broker: {Reason}", args.Reason);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
        }
    }
}
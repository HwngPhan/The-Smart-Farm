using System;
using System.Threading.Tasks;

namespace TSF_mustidisProj.Services
{
    public interface IMqttService
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task PublishAsync(string feed, string message);
        Task SubscribeAsync(string feed);
        Task UnsubscribeAsync(string feed);
        
        event Func<string, string, Task> MessageReceived;
    }
}
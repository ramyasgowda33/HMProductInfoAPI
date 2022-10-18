using HMProductInfoAPI.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

namespace HMProductInfoAPI.Services
{
    public record Notification (string Message);
    public interface INotificationService
    {
        ValueTask PushAsync(Notification notification);
        
    }

    public class NotificationService : BackgroundService, INotificationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationService> _logger;
        private readonly Channel<Notification> _channel;

        public NotificationService(IServiceProvider serviceProvider,
            ILogger<NotificationService> logger)
           
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _channel = Channel.CreateUnbounded<Notification>();
        }
        public ValueTask PushAsync(Notification notification)
        {
            return _channel.Writer.WriteAsync(notification);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(true)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    var message = await _channel.Reader.ReadAsync(stoppingToken);

                    using var scope = _serviceProvider.CreateScope();

                    var hub = scope.ServiceProvider.GetRequiredService<IHubContext
                        <NotificationHub>>();

                    var payLoad = new { Message = message };

                    _logger.LogInformation($"Sending started : ProductCreatedEvent:{message} to All clients");

                    await hub.Clients.User("AllClients").SendAsync("Notify", payLoad, stoppingToken);

                    _logger.LogInformation($"Sending completed :ProductCreatedEvent:{message} to All clients");

                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Error in notification sevice");
                        
                }
            }
        }
    }
}

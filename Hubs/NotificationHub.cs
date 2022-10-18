using HMProductInfoAPI.DTO;
using Microsoft.AspNetCore.SignalR;

namespace HMProductInfoAPI.Hubs
{
    public class NotificationHub : Hub
    {
        public void BroadcastProductCreatedEvent(ProductDto productDto)
        {
            Clients.All.SendAsync("ReceiveProductCreatedEvent", productDto);
        }
    }
}

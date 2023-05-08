using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using System.Threading.Tasks;
using System;

namespace DrawAndGuess.Invokers
{
    public abstract class BaseSender
    {
        public IHubContext<GameHub> HubContext { get; }
        public BaseSender(IHubContext<GameHub> hubContext)
        {
            HubContext = hubContext;
        }

        public async Task SendToGroup(string groupName, string methodName, object payload = null!)
        {
            try
            {
                await HubContext.Clients.Group(groupName)
                    .SendAsync(methodName, payload ?? new object());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SendToClient(string connectionId, string methodName, object payload = null!)
        {
            try
            {
                await HubContext.Clients.Client(connectionId)
                    .SendAsync(methodName, payload ?? new object());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

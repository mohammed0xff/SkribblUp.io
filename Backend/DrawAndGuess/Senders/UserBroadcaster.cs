using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalRChat.Hubs;
using DrawAndGuess.Constants;
using DrawAndGuess.Models;
using DrawAndGuess.Entities;

namespace DrawAndGuess.Invokers
{
    /// <summary>
    /// This class is for ( user --> server --> user(s) ) comunication.
    /// </summary>
    public class UserBroadcaster : BaseSender
    {
        private readonly ILogger<UserBroadcaster> _logger;

        public UserBroadcaster(IHubContext<GameHub> hubContext)
            : base(hubContext)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<UserBroadcaster>();
        }

        public async Task SendMessage(User user, string message)
        {
            try
            {
                await SendToGroup(user.RoomName, ClientMethods.ReceiveMessage,
                    new Message
                    {
                        Username = user.UserName,
                        Content = message
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message for user {user.UserName} in room {user.RoomId}: {ex.Message}");
            }
        }

        /// sets the new pos of cursor as user just clicked.
        /// also to start a new path on the canvas.
        public async Task SendDrawingStarted(string roomName, MousePos pos)
        {
            try
            {
                await SendToGroup(roomName, ClientMethods.StartDrawing, pos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending drawing start message for room {roomName}: {ex.Message}");
            }
        }

        /// onMouseUp event didnt happen yet
        /// user still drawing ..
        public async Task SendMousePos(string roomName, MousePos pos)
        {
            try
            {
                await SendToGroup(roomName, ClientMethods.Draw, pos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending mouse position message for room {roomName}: {ex.Message}");
            }
        }

        public async Task ClearCanvasInRoom(string roomName)
        {
            try
            {
                await SendToGroup(roomName, ClientMethods.ClearCanvas);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending clear canvas message for room {roomName}: {ex.Message}");
            }
        }

        public async Task SendNewColorToRoom(string roomName, string color)
        {
            try
            {
                await SendToGroup(roomName, ClientMethods.ColorChanged, color);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending color changed message for room {roomName}: {ex.Message}");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DrawAndGuess.Constants;
using DrawAndGuess.Utils;
using DrawAndGuess.Entities;
using DrawAndGuess.Models;

namespace DrawAndGuess.Invokers
{
    /// <summary>
    /// This class is for ( game logic --> user/users ) comunication
    /// </summary>
    public class GameSender : BaseSender
    {
        private readonly ILogger<GameSender> _logger;
        public GameSender(IHubContext<GameHub> hubContext)
            : base(hubContext)
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            _logger = loggerFactory.CreateLogger<GameSender>();
        }

        private async Task SendUserAction(User user, string action)
        {
            try
            {
                await SendToGroup(user.RoomName, ClientMethods.UserAction,
                        new UserAction
                        {
                            User = user,
                            ActionType = action
                        }
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending user action for user : " +
                    $"{user.UserName} to room {user.RoomId} : {ex.Message}");
            }
        }

        public async Task UserJoinedRoom(User user)
            => await SendUserAction(user, UserActions.UserJoined);
        public async Task UserLeftRoom(User user)
            => await SendUserAction(user, UserActions.UserLeft);
        public async Task UserDiconnected(User user)
            => await SendUserAction(user, UserActions.UserDisconnected);
        public async Task NotifyRightGuess(User user)
            => await SendUserAction(user, UserActions.UserGuessed);
        public async Task NotifyArtistIsDrawing(User user)
            => await SendUserAction(user, UserActions.DrawingNow);
        public async Task NotifyArtistChoosingWord(User user)
            => await SendUserAction(user, UserActions.ChoosingWord);

        public async Task SendConnectedUsers(User user, IEnumerable<User> usersInRoom)
        {
            try
            {
                await SendToClient(user.ConnectionId, ClientMethods.UsersInRoom, usersInRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending connected users for user {user.UserName}: {ex.Message}");
            }
        }

        public async Task NotifyGameStateOnRoom(Room room)
        {
            try
            {
                await SendToGroup(room.Name, ClientMethods.GameAction, room.GameState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending game state message for room {room.Name}: {ex.Message}");
            }
        }

        public async Task GetWordFromArtist(User user)
        {
            try
            {
                var guessingWords = SecretWords.GetTreeRandomWords();
                await SendToClient(user.ConnectionId, ClientMethods.ChooseWord, guessingWords);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending word choices message to client {user.ConnectionId}: {ex.Message}");
            }
        }

        public async Task RevealWord(Room room)
        {
            try
            {
                if (room.SecretWord is null)
                {
                    ArgumentNullException exception = new(nameof(room.SecretWord));
                    throw exception;
                }

                await SendToGroup(room.Name, ClientMethods.RevealWord, room.SecretWord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error revealing word for room {room.Name}: {ex.Message}");
            }
        }

        public async Task SendHint(Room room)
        {
            try
            {
                if (room.HintWord is null)
                {
                    ArgumentNullException exception = new(nameof(room.HintWord));
                    throw exception;
                }

                await SendToGroup(room.Name, ClientMethods.WordHint, room.HintWord);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending hint for room {room.Name}: {ex.Message}");
            }
        }

        public async Task StartTimer(Room room, int timeInMinuites)
        {
            try
            {
                await SendToGroup(room.Name, ClientMethods.StartTimer, timeInMinuites);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting timer for room {room.Name}: {ex.Message}");
            }

        }

        public async Task StopTimer(Room room)
        {
            try
            {
                await SendToGroup(room.Name, ClientMethods.StopTimer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error stopping timer for room {room.Name}: {ex.Message}");
            }
        }

        public async Task UpdateUsersPoints(Room room, IEnumerable<User> usersInRoom)
        {
            try
            {
                await SendToGroup(room.Name, ClientMethods.UsersInRoom, usersInRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending users updated for room {room.Name}: {ex.Message}");
            }
        }

        public async Task NotifyNewTurn(Room room)
        {
            try
            {
                await SendToGroup(room.Name, ClientMethods.NewTurn);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error notifing for a new turn room : {room.Name}: {ex.Message}");
            }
        }

        public async Task NotifyAristToPickThePen(User artist)
        {
            try
            {
                await SendToClient(artist.ConnectionId, ClientMethods.PickThePen);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error notifing for a picking the pen arist : {artist.UserName}: {ex.Message}");
            }
        }
    }
}

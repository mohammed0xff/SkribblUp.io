using System;
using System.Threading.Tasks;
using DrawAndGuess.Commands.Interfaces;

namespace DrawAndGuess.Commands
{
    public class ClearCanvasCommand : ICommand
    {
        public string ConnectionId { get; }

        public ClearCanvasCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            var user = game.GetUserByConnectionId(ConnectionId);
            if (user == null)
            {
                throw new ArgumentException("user does not exist");
            }

            var room = game.GetRoomById(user.RoomId);
            if (room == null)
            {
                throw new ArgumentException("user is not in a room");
            }

            if (room.ArtistUserId != null && room.ArtistUserId == ConnectionId)
            {
                await game.Broadcaster.ClearCanvasInRoom(room.Name);
            }
        }
    }
}

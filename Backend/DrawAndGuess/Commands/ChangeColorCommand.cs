using System;
using System.Threading.Tasks;
using DrawAndGuess.Commands.Interfaces;

namespace DrawAndGuess.Commands
{
    public class ChangeColorCommand : ICommand
    {
        public string ConnectionId { get; }
        public string NewColor { get; init; }

        public ChangeColorCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            if(string.IsNullOrWhiteSpace(NewColor))
            {
                throw new ArgumentNullException(nameof(NewColor));
            }

            var user = game.GetUserByConnectionId(ConnectionId);
            if (user == null)
            {
                throw new ArgumentException("user does not exist");
            }

            if (user.RoomName == null)
            {
                throw new ArgumentException("user is not in a room");
            }

            await game.Broadcaster.SendNewColorToRoom(user.RoomName, NewColor);
        }
    }
}

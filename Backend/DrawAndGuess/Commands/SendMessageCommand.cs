using System;
using System.Threading.Tasks;
using DrawAndGuess.Commands.Interfaces;

namespace DrawAndGuess.Commands
{
    public class SendMessageCommand : ICommand
    {
        public string ConnectionId { get;}
        public string Message { get; init; }
        public SendMessageCommand(string connectionId)
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

            if (user.RoomId == null)
            {
                throw new ArgumentException("user is not in a room");
            }

            // send message only if it doesnt contain a right guess.
            if (! await game.CheckRightGuess(user, Message))
            {
                await game.Broadcaster.SendMessage(user, Message);
            }
        }
    }
}

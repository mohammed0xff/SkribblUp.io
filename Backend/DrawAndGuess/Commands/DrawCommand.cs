using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Models;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class DrawCommand : ICommand
    {
        public MousePos MousePos { get; init; }
        public string ConnectionId { get; init; }

        public DrawCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.Broadcaster.SendMousePos(
                game.GetUserByConnectionId(ConnectionId).RoomName, MousePos
                );
        }
    }
}

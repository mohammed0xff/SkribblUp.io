using DrawAndGuess.Commands.Interfaces;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class LeaveRoomCommand : ICommand
    {
        public string ConnectionId { get; init; }
        public LeaveRoomCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.LeaveRoom(ConnectionId);
        }
    }
}

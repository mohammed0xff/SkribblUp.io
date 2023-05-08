using DrawAndGuess.Commands.Interfaces;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class DisconnectCommand : ICommand
    {
        public string ConnectionId { get; init; }
        public DisconnectCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.UserDisconnected(ConnectionId);
        }
    }
}

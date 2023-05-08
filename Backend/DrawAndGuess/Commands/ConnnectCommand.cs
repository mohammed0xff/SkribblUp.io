using DrawAndGuess.Commands.Interfaces;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class ConnnectCommand : ICommand
    {
        public string ConnectionId { get; }
        public ConnnectCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }
        public async Task ExecuteAsync(Game game)
        {
            game.UserConnected(ConnectionId);
        }
    }
}

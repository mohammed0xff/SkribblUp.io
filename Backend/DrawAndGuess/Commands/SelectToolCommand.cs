using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Models;
using System.Drawing;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class SelectToolCommand : ICommand
    {
        public string ConnectionId { get; init; }
        public int ToolIndex { get; init; }
        public SelectToolCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }
        public async Task ExecuteAsync(Game game)
        {
            await game.Broadcaster.SendToolChangedAsync(
                game.GetUserByConnectionId(ConnectionId).RoomName, ToolIndex
                );
        }
    }
}

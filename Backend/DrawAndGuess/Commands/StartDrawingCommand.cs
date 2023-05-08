using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Models;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class StartDrawingCommand : ICommand
    {
        public MousePos mousePos; 
        public string ConnectionId { get; init; }
        public StartDrawingCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.Broadcaster.SendDrawingStarted(
                game.GetUserByConnectionId(ConnectionId).RoomName, mousePos
                );
        }
    }
}

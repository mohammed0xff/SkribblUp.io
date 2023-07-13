using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Models;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class FillColorCommand : ICommand
    {
        public MousePos mousePos; 
        public string ConnectionId { get; init; }
        public FillColorCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.Broadcaster.SendFill(
                game.GetUserByConnectionId(ConnectionId).RoomName, mousePos
                );
        }
    }
}

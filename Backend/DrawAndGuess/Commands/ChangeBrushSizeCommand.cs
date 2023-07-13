using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Entities;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    internal class ChangeBrushSizeCommand : ICommand
    {
        public int Size { get; init; }
        public string ConnectionId { get; init; }
        public ChangeBrushSizeCommand(string connectionId)
        {
            ConnectionId= connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.Broadcaster.SendBrushSizeChangedAsync(
                game.GetUserByConnectionId(ConnectionId).RoomName, Size
            );
        }
    }
}
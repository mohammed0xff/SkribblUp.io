using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Models;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class JoinRoomCommand : ICommand
    {
        public string ConnectionId { get; init; }
        public JoinRoomForm Form { get; set; }
        public JoinRoomCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public async Task ExecuteAsync(Game game)
        {
            await game.JoinRoom(Form, ConnectionId);
        }
    }
}

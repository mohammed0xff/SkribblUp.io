using DrawAndGuess.Commands.Interfaces;
using DrawAndGuess.Constants;
using System;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class GetUserInRoomCommand : ICommand
    {
        public GetUserInRoomCommand(string connectionId) { ConnectionId = connectionId; }
        public string ConnectionId { get; init; }
        public string RoomName { get; init; }
        public async Task ExecuteAsync(Game game)
        {
           var user = game.GetUserByConnectionId(ConnectionId);
           
            if(user == null) { throw new ArgumentNullException(nameof(user)); }
           
            await game.Broadcaster.SendToClient(ConnectionId, 
                    ClientMethods.UsersInRoom, 
                    game.GetUsersInRoom(user.RoomId));
        }
    }
}

using DrawAndGuess.Commands.Interfaces;
using System;
using System.Threading.Tasks;

namespace DrawAndGuess.Commands
{
    public class PickWordCommand : ICommand
    {
        public string ConnectionId { get; init; }
        public string Word { get; set; }
        public PickWordCommand(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public Task ExecuteAsync(Game game)
        {
            var artist = game.GetUserByConnectionId(ConnectionId);
            if (artist == null)
            {
                throw new ArgumentException("Artist does not exist");
            }

            var room = game.GetRoomById(artist.RoomId);
            if (room == null)
            {
                throw new ArgumentException("Room does not exist");
            }

            room.SetSecretWord(artist.ConnectionId, Word);
            
            return Task.CompletedTask;
        }
    }
}

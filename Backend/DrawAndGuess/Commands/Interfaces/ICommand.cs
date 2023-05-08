using System.Threading.Tasks;

namespace DrawAndGuess.Commands.Interfaces
{
    public interface ICommand 
    {
        public string ConnectionId { get; }
        public Task ExecuteAsync(Game game);
    }
}

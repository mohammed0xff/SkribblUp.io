using DrawAndGuess.Commands.Interfaces;

namespace DrawAndGuess.CommandProcessor
{
    public interface ICommandProcessor
    {
        void AddCommand(ICommand command);
    }
}

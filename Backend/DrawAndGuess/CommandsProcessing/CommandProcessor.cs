using System;
using System.Threading.Tasks;
using DrawAndGuess.Commands.Interfaces;
using Microsoft.Extensions.Logging;

namespace DrawAndGuess.CommandProcessor
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly Game _game;
        private readonly ILogger<CommandProcessor> _logger;
        private readonly TaskQueue _taskQueue;

        public CommandProcessor(Game game, ILogger<CommandProcessor> logger)
        {
            _game = game;
            _logger = logger;
            _taskQueue = new TaskQueue();
        }

        public void AddCommand(ICommand command)
        {
            _taskQueue.EnqueueTask(
                    () => ProcessCommand(command)
                );
        }

        private async Task ProcessCommand(ICommand command)
        {
            try
            {
               await command.ExecuteAsync(_game);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in executing command {command.GetType().Name} : {ex.Message}");
            }
        }
    }
}

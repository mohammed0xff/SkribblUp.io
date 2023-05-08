using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrawAndGuess.Utils
{
    public class GameTimer
    {
        /// <summary>
        /// Waits for a specified condition to turn true, up to a maximum amount of time.
        /// </summary>
        /// <param name="Condition">The condition to be checked at each interval.</param>
        /// <param name="maxTime">The maximum amount of time to wait, in ms.</param>
        /// <returns>A task that represents the completion of the wait operation.</returns>
        public GameTimer() { }
        public Task WaitTill(Func<bool> condition, int maxTime)
        {
            while (!condition() && maxTime > 0)
            {
                // wait a sec
                maxTime -= 1000;
                Thread.Sleep(1000);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Runs the specified action at a regular interval for a number of times or until canceled.
        /// </summary>
        /// <param name="action">The action to be executed at each interval.</param>
        /// <param name="msInterval">The time in milliseconds between each interval.</param>
        /// <param name="maxTimes">The maximum number of times the action can be executed.</param>
        /// <param name="cancelationtoken">cancellation token that can be used to cancel the interval.</param>
        public void SetInterval(
            Action action,
            int msInterval,
            int maxTimes,
            CancellationToken cancelationtoken
            )
        {
            Task.Run(() =>
            {
                do {
                    action();
                    Thread.Sleep(msInterval);
                } while (--maxTimes > 0 && !cancelationtoken.IsCancellationRequested);
            }, cancelationtoken);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawAndGuess.CommandProcessor
{
    public class TaskQueue
    {
        // Queue of tasks
        private readonly Queue<Func<Task>> _queue = new Queue<Func<Task>>();
        // Object used for thread synchronization
        private readonly object _lock = new object();
        // Completion source used to signal when new tasks are added to the queue.
        private TaskCompletionSource<bool> _completionSource = new TaskCompletionSource<bool>();

        public TaskQueue()
        {
            _ = ProcessQueueAsync();
        }

        /// <summary>
        /// Enqueues a new task to be processed
        /// </summary>
        /// <param name="taskFunc"></param>
        public void EnqueueTask(Func<Task> taskFunc)
        {
            lock (_lock) // Lock the queue before accessing it
            {
                _queue.Enqueue(taskFunc);
                // Signal that a new task is available
                // and continues the loop as it stops waiting.
                _completionSource.TrySetResult(true); 
            }
        }

        /// <summary>
        /// Processes tasks from the queue as they become available
        /// </summary>
        /// <returns></returns>
        private async Task ProcessQueueAsync()
        {
            while (true)
            {
                Func<Task> task = null!;
                lock (_lock) // Lock the queue before accessing it
                {
                    if (_queue.Count > 0)
                    {
                        // Dequeue the next task
                        task = _queue.Dequeue();
                    }
                    else // the queue is empty
                    {
                        // Create a new completion source to wait for new tasks
                        _completionSource = new TaskCompletionSource<bool>();
                    }
                }

                if (task != null) // If a task was dequeued
                {
                    // Execute the task
                    await task.Invoke();
                }
                else // the queue is empty
                {
                    // wait for a new task to be added to the queue
                    await _completionSource.Task;
                }
            }
        }
    }
}
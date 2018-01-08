using System;
using System.Collections.Generic;
using System.Threading;

namespace Reach.Execution
{
    public class ActionQueue : IActionQueue
    {
        // One to signal message availability, another for shutdown.
        private readonly AutoResetEvent[] waitHandles = 
        {
            new AutoResetEvent(false), // For message availability
            new AutoResetEvent(false)  // For shutdown event
        };

        private readonly object locker = new object();

        private enum HandleIndex
        {
            ActionAvailable, Shutdown
        }

        private readonly Queue<Action> actionQueue = new Queue<Action>();
        private readonly Thread worker;

        public event Action<Exception> OnException;

        // Event is emitted when sequent message from queue is consumed. 
        public event Action ActionConsumed;

        private void OnLogException(Exception ex) 
        {
            if (OnException != null) 
            {
                OnException(ex);
            }
        }

        private void OnActionConsumed()
        {
            if (ActionConsumed != null)
            {
                ActionConsumed();
            }
        }

        public ActionQueue()
        {
            worker = new Thread(Consume);
            // If not set as a background thread, this thread will hold 
            // the application open on shutdown
            worker.IsBackground = true;
            worker.Start();
        }

        public void Dispose()
        {
            lock (locker)
            {
                actionQueue.Clear();
            }
            waitHandles[(int)HandleIndex.Shutdown].Set(); // Shutdown requested
            worker.Join(); // Wait for thread to end.
        }

        public void Enqueue(Action action)
        {
            lock (locker)
            {
                actionQueue.Enqueue(action);
                waitHandles[(int)HandleIndex.ActionAvailable].Set(); // A message is available.
            }
        }

        private void Consume()
        {
            while (true)
            {
                Action action = null;
                lock (locker)
                {
                    if (actionQueue.Count > 0)
                        action = actionQueue.Dequeue();
                }
                if (action != null)
                {
                    try
                    {
                        action();
                        OnActionConsumed();
                    }
                    catch (Exception ex)
                    {
                        OnLogException(ex);
                    }
                    continue;
                }

                // No more messages, go into wait.
                int eventIndex = WaitHandle.WaitAny(waitHandles);

                if (eventIndex == 1)
                {
                    // Shutdown event.
                    break;
                }

                // Otherwise, pick up the next message.
            }
        }
    }
}

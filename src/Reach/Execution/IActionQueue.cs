using System;

namespace Reach.Execution
{
    public interface IActionQueue : IDisposable
    { 
        void Enqueue(Action action);
        event Action<Exception> OnException;
        event Action ActionConsumed;
    }
}
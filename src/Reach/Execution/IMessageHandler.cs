using System;
using System.Collections.Generic;
using Reach.Messages;
using Reach.Messages.Data;

namespace Reach.Execution
{
    public interface IMessageHandler : IDisposable
    {
        event ResultReadyEventHandler ResultReady;

        int? CurrentMessageId { get; }
        string SessionId { get; set; }
        ActionQueue MessageQueue { get; set; }

        string SerializeMessage(Message message);
        Message DeserializeMessage(string jsonString);

        void Execute(Message message);
        void ExecuteMessage(Message message, bool enqueue);
        IEnumerable<LibraryItem> GetAllLibraryItemsByCategory();
        IEnumerable<ExecutedNode> ExecutedNodes { get; }
    }
}
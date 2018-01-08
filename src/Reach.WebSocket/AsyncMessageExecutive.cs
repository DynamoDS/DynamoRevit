using Dynamo.Models;
using Reach.Execution;
using Reach.Logging;
using Reach.Messages;
using Reach.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reach.WebSocket
{
    public interface IAsyncMessageExecutive : IDisposable
    {
        void ExecuteMessage(Message message, bool enqueue = true);
        Message ExecuteMessage(string message, bool enqueue = true);
        Message ExecuteFile(byte[] file, bool enqueue = true);

        event Action<Response> ResponseReady;
        event Action<Message> MessageConsumed;
        event Action<Exception> ExceptionRaised;
        event Action<IAsyncMessageExecutive> Disposed;
    }

    public class AsyncMessageExecutive : IAsyncMessageExecutive
    {
        public event Action<IAsyncMessageExecutive> Disposed;
        public event Action<Response> ResponseReady;
        public event Action<Message> MessageConsumed;
        public event Action<Exception> ExceptionRaised;

        private readonly IMessageHandler messageHandler;
        private readonly IActionQueue actionQueue;
        private int counterId = 0;

        public AsyncMessageExecutive(DynamoModel dynamoModel, ILogger logger)
        {
            this.messageHandler = new MessageHandler(dynamoModel, logger);
            this.actionQueue = new ActionQueue();

            this.messageHandler.ResultReady += this.OnResponseReady;
            this.actionQueue.OnException += this.OnExceptionRaised;
        }

        public void Dispose()
        {
            messageHandler.Dispose();
            actionQueue.Dispose();

            OnDisposed();
        }

        private void OnDisposed()
        {
            if (Disposed != null) Disposed(this);
        }

        private void OnResponseReady(object sender, ResultReadyEventArgs e)
        {
            if (ResponseReady != null) ResponseReady(e.Response);
        }
        
        private void OnMessageConsumed(Message message)
        {
            if (MessageConsumed != null) MessageConsumed(message);
        }

        private void OnExceptionRaised(Exception e)
        {
            if (ExceptionRaised != null) ExceptionRaised(e);
        }

        public void ExecuteMessage(Message message, bool enqueue)
        {
            if (enqueue)
            {
                actionQueue.Enqueue(() => {
                    messageHandler.Execute(message);
                    OnMessageConsumed(message);
                });
            }
            else
            {
                messageHandler.Execute(message);
                OnMessageConsumed(message);
            }
        }

        public Message ExecuteMessage(string message, bool enqueue)
        {
            var msg = messageHandler.DeserializeMessage(message);
            if (msg.UniqueID == null)
            {
                msg.UniqueID = counterId++;
            }
            ExecuteMessage(msg, enqueue);
            return msg;
        }

        public Message ExecuteFile(byte[] file, bool enqueue)
        {
            var msg = new UploadFileMessage(file);
            if (msg.UniqueID == null)
            {
                msg.UniqueID = counterId++;
            }
            ExecuteMessage(msg, enqueue);
            return msg;
        }
    }
}

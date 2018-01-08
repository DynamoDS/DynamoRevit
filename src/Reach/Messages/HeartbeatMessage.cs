using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// This message keeps Reach alive
    /// </summary>
    class HeartbeatMessage : Message
    {
        internal override void Execute(MessageHandler handler) 
        {
            // should do nothing
        }
    }
}

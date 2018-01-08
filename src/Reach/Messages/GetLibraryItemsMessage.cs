using Reach.Execution;
using Reach.Responses;

namespace Reach.Messages
{
    /// <summary>
    /// This message triggers sending <see cref="LibraryItemsListResponse"/> after this message is received
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"GetLibraryItemsMessage"
    /// }
    /// </code>
    class GetLibraryItemsMessage : Message
    {
        internal override void Execute(MessageHandler handler)
        {
            handler.SendAllLibraryItems();
        }
    }
}

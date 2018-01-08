namespace Reach.Responses
{
    public class MessageConsumedResponse : Response
    {
        /// <summary>
        /// Indicates that message with specified ID has been consumed
        /// </summary>
        public MessageConsumedResponse(int? uniqueID)
        {
            this.UniqueID = uniqueID;
        }
    }
}

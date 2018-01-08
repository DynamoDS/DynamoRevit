namespace Reach.Responses
{
    public class ContentResponse : Response
    {
        /// <summary>
        /// Text message with any kind of information
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The response with a text message
        /// </summary>
        /// <param name="message">Message which will be sent to Reach editor</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"ContentResponse",
        ///     "message":"3/3/2015 Message received",
        ///     "status":0
        /// }
        /// </code>
        public ContentResponse(string message, ResponseStatuses status, int? uniqueID = null) : base(status)
        {
            Message = message;
            UniqueID = uniqueID;
        }
    }
}

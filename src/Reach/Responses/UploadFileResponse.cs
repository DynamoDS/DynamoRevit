using System.Collections.Generic;

namespace Reach.Responses
{
    public class UploadFileResponse:Response
    {
        /// <summary>
        /// Message which indicates if uploading was successful
        /// </summary>
        public string StatusMessage { get; private set; }

        /// <summary>
        /// List of node names which are not allowed in the client
        /// </summary>
        public IEnumerable<string> InvalidNodeNames { get; private set; }

        /// <summary>
        /// Response with status of the file uploading
        /// </summary>
        /// <param name="status">Upload status</param>
        /// <param name="message">Status message</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"UploadFileResponse",
        ///     "statusMessage":"Bad file request",
        ///     "status":1
        /// }
        /// </code>
        public UploadFileResponse(ResponseStatuses status, string message, 
            IEnumerable<string> invalidNodeNames)
            : base(status)
        {
            StatusMessage = message;
            InvalidNodeNames = invalidNodeNames;
        }
    }
}

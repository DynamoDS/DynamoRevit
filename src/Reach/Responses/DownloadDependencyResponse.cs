using System.Collections.Generic;


namespace Reach.Responses
{
    public class DownloadDependencyResponse : Response
    {
        /// <summary>
        /// Message which indicates status of file save
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// The response which contains an information about a saved file
        /// </summary>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"DownloadDependencyResponse",
        ///     "statusMessage":"OK",
        ///     "status":0
        /// }
        /// </code>

        public DownloadDependencyResponse(ResponseStatuses status, string message) : base(status)
        {
            StatusMessage = message;
        }
    }
}

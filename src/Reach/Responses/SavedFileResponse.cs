using System.Collections.Generic;


namespace Reach.Responses
{
    public class SavedFileResponse: Response
    {
        /// <summary>
        /// Saved file name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Saved file content
        /// </summary>
        public IEnumerable<byte> FileContent { get; private set; }

        /// <summary>
        /// Message which indicates status of file save
        /// </summary>
        public string StatusMessage { get; private set; }

        /// <summary>
        /// The response which contains an information about a saved file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="fileContent">File content</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"SavedFileResponse",
        ///     "fileName":"Home.dyn",
        ///     "fileContent":[ 123,13,10,9,34,110,97,109 ... 48,48,34,13,10,125 ],
        ///     "statusMessage":"OK",
        ///     "status":0
        /// }
        /// </code>
        public SavedFileResponse(string fileName, IEnumerable<byte> fileContent, string message)
        {
            this.FileName = fileName;
            this.FileContent = fileContent;
            this.StatusMessage = message;
        }

        public SavedFileResponse(ResponseStatuses status, string message) : base(status)
        {
            StatusMessage = message;
        }
    }
}

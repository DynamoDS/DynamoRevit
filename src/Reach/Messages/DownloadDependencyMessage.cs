using System.Runtime.Serialization;
using Reach.Execution;
using System.Collections.Generic;

namespace Reach.Messages
{
    /// <summary>
    /// This message instructs Reach that it needs to obtain and store on disk 
    /// some dependent data that the workspace requires to run. 
    /// The data is specified either as an accessible HTTP URI
    /// or a RFC 2397 base64-encoded data URL.
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"DownloadDependencyMessage",
    ///     "name":"program.csv",
    ///     "nodeIds":["c9f3f8e6-3d38-4ce7-828e-2f5c31a5bacc"],
    ///     "url":"https://dynamo.autodesk.com/<workspaceId>/<fileId>"
    ///       or
    ///     "url":"data:application/zip;base64,iVBORAAAAAA654K0ENBAOOO..."
    /// }
    /// </code>
    [DataContract]
    public class DownloadDependencyMessage : Message
    {
        /// <summary>
        /// Unique human readable name of this resource 
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Id of the workspace containing the nodes that this file should be attached to
        /// </summary>
        [DataMember]
        public System.Guid WorkspaceId { get; set; }

        /// <summary>
        /// List of Ids of nodes that this dependency should be attached to
        /// </summary>
        [DataMember]
        public IEnumerable<System.Guid> NodeIds { get; set; }
        
        /// <summary>
        /// Url of the resource to be downloaded to the Reach server's temporary storage
        /// </summary>
        [DataMember]
        public string Url { get; set; }

        public DownloadDependencyMessage() { }
        
        internal override void Execute(MessageHandler handler)
        {
            handler.DownloadDependency(this);
        }
    }
}

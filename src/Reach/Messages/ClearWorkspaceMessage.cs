using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// This message cause home workspace to clear, 
    /// and if <see cref="ClearOnlyHome"/> equals to false 
    /// this message will also trigger removing all custom node workspaces
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"ClearWorkspaceMessage",
    ///     "ClearOnlyHome":true
    /// }
    /// </code>
    [DataContract]
    public class ClearWorkspaceMessage : Message
    {
        /// <summary>
        /// Indicates if custom nodes shouldn't be removed along with clearing Home workspace
        /// </summary>
        [DataMember]
        public bool ClearOnlyHome { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.ClearWorkspace(this);
        }
    }
}

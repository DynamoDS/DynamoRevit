using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// Checks if a specified workspace has any unsaved changes
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"HasUnsavedChangesMessage",
    ///     "WorkspaceGuid":"00000000-0000-0000-0000-000000000000"
    /// }
    /// </code>
    [DataContract]
    public class HasUnsavedChangesMessage : Message
    {
        #region Class Data Members

        /// <summary>
        /// Guid of a specified workspace. Empty string for Home workspace
        /// </summary>
        [DataMember]
        public string WorkspaceGuid { get; set; }

        #endregion

        internal override void Execute(MessageHandler handler)
        {
            handler.SendUnsavedChangesInfo(this);
        }
    }
}

using System.Runtime.Serialization;
using System.Collections.Generic;
using Dynamo.Storage.Data;
using Reach.Execution;
using Reach.Messages.Data;

namespace Reach.Messages
{
    [DataContract]
    public class RestoreDynamoGroupsMessage: Message
    {
        /// <summary>
        /// Guid of a specified workspace. Empty string for Home workspace
        /// </summary>
        [DataMember]
        public string WorkspaceGuid { get; set; }

        /// <summary>
        /// Contains all necessary information about groups
        /// </summary>
        [DataMember]
        public IEnumerable<GroupData> Groups { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.RestoreDynamoGroups(this);
        }
    }
}
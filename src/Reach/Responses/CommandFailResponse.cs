using Dynamo.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Reach.Synchronization;

namespace Reach.Responses
{
    /// <summary>
    /// Response with commands which have not been executed in Dynamo
    /// </summary>
    public class CommandFailResponse : Response
    {
        #region Class Data Members

        /// <summary>
        /// Data to synchronize workspace
        /// </summary>
        [DataMember]
        public IEnumerable<SyncData> SyncDataList { get; private set; }

        /// <summary>
        /// Messages for displaying to a user
        /// </summary>
        [DataMember]
        public IEnumerable<string> Messages { get; private set; }

        #endregion

        /// <summary>
        /// This response contains a set of <see cref="DynamoModel.RecordableCommand"/>
        /// which have not been executed in Dynamo
        /// </summary>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "type":"CommandFailResponse",
        ///     "syncDataList":[
        ///         {
        ///             "$type":"Dynamo.Models.DynamoModel+UpdateModelValueCommand, DynamoCore",
        ///             "nodeId":"61e47bad-8d44-6d3e-0b81-557ca672c880",
        ///             "key":"NodeDeletion__"61e47bad-8d44-6d3e-0b81-557ca672c880",
        ///             ...
        ///         },
        ///         ...
        ///     ],
        ///     "messages":["Node was not found"],
        ///     "status":0
        /// }
        /// </code>
        public CommandFailResponse(IEnumerable<SyncData> data, IEnumerable<string> messages)
        {
            this.SyncDataList = data;
            this.Messages = messages;
        }
    }
}

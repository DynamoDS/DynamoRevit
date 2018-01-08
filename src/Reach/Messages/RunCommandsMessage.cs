using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Dynamo.Models;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// This message contains a set of <see cref="DynamoModel.RecordableCommand"/> which will be executed in Dynamo
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"RunCommandsMessage",
    ///     "Commands":[
    ///         {
    ///             "$type":"Dynamo.Models.DynamoModel+UpdateModelValueCommand, DynamoCore",
    ///             "modelGuid":"61e47bad-8d44-6d3e-0b81-557ca672c880",
    ///             "name":"ArgumentLacing",
    ///             "value":"CrossProduct"
    ///         },
    ///         ...
    ///     ],
    ///     "WorkspaceGuid":"00000000-0000-0000-0000-000000000000"
    /// }
    /// </code>
    [DataContract]
    public class RunCommandsMessage : Message
    {
        #region Class Data Members

        /// <summary>
        /// List of recordable commands that should be executed on server
        /// </summary>
        [DataMember]
        public IEnumerable<DynamoModel.RecordableCommand> Commands { get; set; }

        [DataMember]
        public Guid WorkspaceGuid { get; set; }

        [DataMember]
        public bool SendGeometryImmediately { get; set; }

        #endregion

        internal override void Execute(MessageHandler handler)
        {
            handler.ExecuteCommands(this);
        }
    }
}

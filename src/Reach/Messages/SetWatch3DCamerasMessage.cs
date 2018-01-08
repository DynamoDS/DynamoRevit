using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dynamo.Storage.Data;
using Reach.Execution;
using Reach.Messages.Data;

namespace Reach.Messages
{
    /// <summary>
    /// Stores positions of all watch 3d cameras
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"SetWatch3DCameraMessage",
    ///     "WorkspaceGuid":"00000000-0000-0000-0000-000000000000",
    ///     "nodeGuids":["761132b8-b5fa-46b2-98a9-82436e0d8812"],
    ///     "cameras":
    ///     [{
    ///         "name":"761132b8-b5fa-46b2-98a9-82436e0d8812 Preview",
    ///         "eyeX":7.577991962432861,         
    ///         "eyeY":-14.262261390686035,
    ///         "eyeZ":-61.67133712768555,
    ///         "lookX":-12.577991485595703,
    ///         "lookY":25.26226234436035,
    ///         "lookZ":53.67133712768555,
    ///         "upX":0.1373162716627121,
    ///         "upY":0.7986355423927307,
    ///         "upZ":-0.5859399437904358
    ///     }]
    /// }
    /// </code>
    [DataContract]
    public class SetWatch3DCameraMessage : Message
    {

        /// <summary>
        /// Position of watch 3d cameras.
        /// </summary>
        [DataMember]
        public List<CameraData> Cameras { get; set; }

        /// <summary>
        /// Guid of watch 3d nodes.
        /// </summary>
        [DataMember]
        public List<Guid> NodeGuids { get; set; }

        /// <summary>
        /// Guid of a specified workspace. Empty string for Home workspace
        /// </summary>
        [DataMember]
        public string WorkspaceGuid { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.SetWatch3DCameras(this);
        }
    }
}

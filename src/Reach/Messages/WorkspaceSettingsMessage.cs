using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// Process some workspace settings, such as offset or zoom.
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"WorkspaceSettingsMessage",
    ///     "Guid":"00000000-0000-0000-0000-000000000000",
    ///     "x":0,
    ///     "y":0,
    ///     "zoom":1
    /// }
    /// </code>
    [DataContract]
    public class WorkspaceSettingsMessage : Message
    {
        /// <summary>
        /// Guid of the specified workspace
        /// </summary>
        [DataMember]
        public string Guid { get; set; }

        /// <summary>
        /// Offset by X
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Offset by Y
        /// </summary>
        [DataMember]
        public double Y { get; set; }

        /// <summary>
        /// Zoom of workspace ()
        /// </summary>
        [DataMember]
        public double Zoom { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.SetWorkspaceSettings(this);
        }
    }
}

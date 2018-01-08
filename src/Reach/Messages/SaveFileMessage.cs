using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    /// <summary>
    /// Triggers saving a workspace with <see cref="Guid"/> identifier
    /// into the path specified in <see cref="FilePath"/> or
    /// if <see cref="FilePath"/> is not specified the workspace 
    /// will be saved into a temporary folder and will be sent back as a byte array
    /// </summary>
    /// <example>
    /// This sample shows Json structure
    /// </example>
    /// <code>
    /// {
    ///     "type":"SaveFileMessage",
    ///     "Guid":"00000000-0000-0000-0000-000000000000",
    ///     "FilePath":"c:\\Home.dyn"
    /// }
    /// </code>
    [DataContract]
    public class SaveFileMessage : Message 
    { 
        /// <summary>
        /// Guid of the specified workspace
        /// </summary>
        [DataMember]
        public string Guid { get; set; }

        /// <summary>
        /// Path to save
        /// </summary>
        [DataMember]
        public string FilePath { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.SaveFile(this);
        }
    }
}

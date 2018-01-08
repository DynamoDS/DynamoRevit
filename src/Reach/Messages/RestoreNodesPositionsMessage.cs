using Reach.Execution;
using System.Runtime.Serialization;

namespace Reach.Messages
{
    [DataContract]
    public class RestoreNodesPositionsMessage: Message
    {
        /// <summary>
        /// Guid of a specified workspace. Empty string for Home workspace
        /// </summary>
        [DataMember]
        public string WorkspaceGuid { get; set; }

        /// <summary>
        /// Bounding box for early uploaded nodes
        /// </summary>
        [DataMember]
        public NodesBoundingBox BoundingBox { get; set; }

        internal override void Execute(MessageHandler handler)
        {
            handler.RestoreNodesPositions(this);
        }
    }

    public class NodesBoundingBox
    {
        public double MinX { get; private set; }
        public double MaxX { get; private set; }
        public double MinY { get; private set; }
        public double MaxY { get; private set; }
    }
}

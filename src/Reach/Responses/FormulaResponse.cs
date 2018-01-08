using Reach.Responses.Data;
using System.Runtime.Serialization;

namespace Reach.Responses
{
    class FormulaResponse : Response
    {
        /// <summary>
        /// Guid of the workspace that contains specified formula node.
        /// Empty string for Home workspace
        /// </summary>
        [DataMember]
        public string WorkspaceGuid { get; set; }

        /// <summary>
        /// Guid of the specified formula node
        /// </summary>
        [DataMember]
        public string NodeId { get; private set; }

        /// <summary>
        /// String representing of the data about input,
        /// output ports
        /// </summary>
        [DataMember]
        public FormulaData Data { get; private set; }

        /// <summary>
        /// String representing of the Ncalc formula
        /// </summary>
        [DataMember]
        public string FormulaString { get; private set; }

        public FormulaResponse(string wsGuid, string nodeGuid, FormulaData data, string fs)
        {
            WorkspaceGuid = wsGuid;
            NodeId = nodeGuid;
            Data = data;
            FormulaString = fs;
        }
    }
}

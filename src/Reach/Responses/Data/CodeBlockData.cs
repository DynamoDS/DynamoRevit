using System.Collections.Generic;

namespace Reach.Responses.Data
{
    /// <summary>
    ///     This class represents extra data for CodeBlock node
    /// </summary>
    public class CodeBlockData
    {
        /// <summary>
        /// InPorts field
        /// </summary>
        /// <value>Contains all inputs of CodeBlock node</value>
        public IEnumerable<string> InPorts;

        /// <summary>
        /// OutPorts field
        /// </summary>
        /// <value>Contains all outputs of CodeBlock node</value>
        public IEnumerable<string> OutPorts;

        /// <summary>
        /// LineIndices field
        /// </summary>
        /// <value>Contains LineIndices of CodeBlock node</value>
        public IEnumerable<string> LineIndices;

        /// <summary>
        /// Code field
        /// </summary>
        /// <value>Contains script of CodeBlock node</value>
        public string Code;
    }
}

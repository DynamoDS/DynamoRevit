using System.Collections.Generic;

namespace Reach.Responses.Data
{
    /// <summary>
    ///     This class represents extra data for Formula node
    /// </summary>
    class FormulaData
    {
        /// <summary>
        /// InPorts field
        /// </summary>
        /// <value>Contains all inputs of Formula node</value>
        public IEnumerable<string> InPorts;

        /// <summary>
        /// OutPorts field
        /// </summary>
        /// <value>Contains output of Formula node</value>
        public IEnumerable<string> OutPorts;
    }
}

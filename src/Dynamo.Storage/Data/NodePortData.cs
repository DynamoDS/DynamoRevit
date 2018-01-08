using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.Storage.Data
{
    public class NodePortData
    {
        public int index { get; set; }

        public bool useLevels { get; set; }

        public int level { get; set; }

        public bool shouldKeepListStructure { get; set; }
    }
}

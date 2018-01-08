using System.Collections.Generic;
using Dynamo.Storage.Data;

namespace Dynamo.Storage
{
    public class WorkspaceProperties
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<CameraData> Cameras { get; set; } 
    }
}

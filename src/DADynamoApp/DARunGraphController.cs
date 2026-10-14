using Dynamo.Models;
using DynamoPlayer;

namespace DADynamoApp
{
    internal class DARunGraphController : DynamoController
    {
        IDynamoController implementation;
        string workFolder = string.Empty;
        DynamoModel model;

        public DARunGraphController(IDynamoController implementation, DynamoModel model, string workFolder) : base(implementation)
        {
            this.workFolder = workFolder;
            this.implementation = implementation;
            this.model = model;
        }

        [Route("graph/run", Name = "post-graph-run")]
        public new Task<GraphResult> PostGraphRun([FromBody] RunGraph body)
        {
            if (body.Target is CurrentGraphTarget)
            {
                this.model.OpenFileFromPath(Path.Combine(workFolder, "run.dyn"));
            }
            else if (body.Target is PathGraphTarget pGraphTarget)
            {
                pGraphTarget.Path = Path.Combine(this.workFolder, pGraphTarget.Path);
            }
            return implementation.PostGraphRunAsync(body);
        }
    }
}
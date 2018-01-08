using Dynamo.Scheduler;
using System.Collections.Generic;
using System.Linq;
using Reach.Messages.Data;
using Reach.Rendering;
using Dynamo.Engine;
using Dynamo.Graph.Workspaces;

namespace Reach.Execution
{
    class GetExecutedNodesAsyncTask: AsyncTask
    {
        private readonly HomeWorkspaceModel homeWorkspace;
        private readonly EngineController engine;
        private readonly IRenderPackageCache renderPackageCache;

        internal IEnumerable<ExecutedNode> Nodes { get; private set; }
        
        public override TaskPriority Priority
        {
            get { return TaskPriority.Normal; }
        }

        internal GetExecutedNodesAsyncTask(IScheduler scheduler,
            HomeWorkspaceModel homeWorkspace, EngineController engine, IRenderPackageCache renderPackageCache)
            : base(scheduler)
        {
            this.homeWorkspace = homeWorkspace;
            this.renderPackageCache = renderPackageCache;
            this.engine = engine;
        }

        protected override void HandleTaskExecutionCore()
        {
            Nodes = GetExecutedNodes();
        }

        protected override void HandleTaskCompletionCore() { }

        // it makes sense only in Home workspace
        private IEnumerable<ExecutedNode> GetExecutedNodes()
        {
            return (from node in homeWorkspace.Nodes 
                    where node.WasInvolvedInExecution
                    let data = node.GetValue(engine) 
                    select new ExecutedNode(node, data, this.renderPackageCache.HasRenderPackages(node.GUID))).ToList();
        }
    }
}

using System.IO;

using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    public class WorkflowTests : RevitSystemTestBase
    {
        

        [Test]
        [TestModel(@".\Workflow\empty.rfa")]
        public void Workflow_test01()
        {
            // Create Wall.ByCurveAndHeight
            string samplePath = Path.Combine(workingDirectory, @".\Workflow\WorkFlow_test01.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);

            RunCurrentModel();
            
            
        }

        
    }
}

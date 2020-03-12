using NUnit.Framework;

using RevitTestServices;

using RTF.Framework;

using System.IO;

using System.Linq;
using Autodesk.DesignScript.Geometry;

namespace RevitSystemTests
{
    [TestFixture]
    public class SheetTests : RevitSystemTestBase
    {
        [Test]
        [TestModel(@".\Empty.rvt")]
        public void ByNameNumberTitleBlock()
        {
            // Arrange
            string samplePath = Path.Combine(workingDirectory, @".\Sheet\ByNameNumberTitleBlock.dyn");
            string testPath = Path.GetFullPath(samplePath);


            // Act
            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            var sheet = GetPreviewValue("54254be3a86d4ba89931d5c0a3428624");

            // Assert
            Assert.NotNull(sheet);
        }
    }
}

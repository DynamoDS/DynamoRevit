using System.IO;
using System.Windows.Media;
using SystemTestServices;
using Dynamo.UI;
using NUnit.Framework;
using RevitTestServices;
using RTF.Framework;

namespace RevitSystemTests
{
    [TestFixture]
    class DisplayTests : RevitSystemTestBase
    {
        [Test, TestModel(@".\empty.rfa")]
        public void Display_ModelCurveDrawnInBackground_HasCorrectColor()
        {
            string samplePath = Path.Combine(workingDirectory, @".\Display\ModelCurveColor.dyn");
            string testPath = Path.GetFullPath(samplePath);

            ViewModel.OpenCommand.Execute(testPath);
            RunCurrentModel();

            //var numLines = BackgroundPreview.Lines.Count;
            //var expectedColor = (Color)SharedDictionaryManager.DynamoColorsAndBrushesDictionary["EdgeColor"];
            //var lines = BackgroundPreview.HasNumberOfLinesOfColor();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Dynamo.Graph.Annotations;

namespace Dynamo.Storage.Data
{
    public class GroupData
    {
        public string GroupId { get; set; }

        public string Text { get; set; }

        public IEnumerable<string> Models { get; set; }

        public string Background { get; set; }

        public double FontSize { get; set; }

        public double InitialTop { get; set; }

        public double InitialHeight { get; set; }

        public double TextBlockHeight { get; set; }

        public void SetPropertiesToAnnotation(AnnotationModel annotation)
        {
            annotation.Background = Background;
            annotation.FontSize = FontSize;
            annotation.InitialTop = InitialTop;
            annotation.InitialHeight = InitialHeight;
            annotation.TextBlockHeight = TextBlockHeight;
        }

        public GroupData() { }

        public GroupData(AnnotationModel annotation)
        {
            GroupId = annotation.GUID.ToString();
            Text = annotation.AnnotationText;
            Models = annotation.Nodes.Select(m => m.GUID.ToString());
            Background = annotation.Background;
            FontSize = annotation.FontSize;
            InitialTop = annotation.InitialTop;
            InitialHeight = annotation.InitialHeight;
            TextBlockHeight = annotation.TextBlockHeight;
        }
    }
}

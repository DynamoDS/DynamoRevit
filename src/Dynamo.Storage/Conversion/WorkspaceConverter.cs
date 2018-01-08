using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Dynamo.Graph.Annotations;
using Dynamo.Graph.Connectors;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Notes;
using Dynamo.Graph.Workspaces;
using Dynamo.Storage.Conversion.NodeConverters;
using Dynamo.Storage.Data;

namespace Dynamo.Storage.Conversion
{
    public class ReachWorkspaceConverter : IWorkspaceConverter
    {
        #region Converters
        private readonly IEnumerable<BaseConverter> _converters = new List<BaseConverter>
        {
            new BoolConverter(),
            new CodeBlockConverter(),
            new DoubleInputConverter(),
            new DoubleInputSliderConverter(),
            new FormulaConverter(),
            new FunctionConverter(),
            new IntegerSliderConverter(),
            new OutputConverter(),
            new PythonNodeConverter(),
            new StringInputConverter(),
            new SymbolConverter(),
            new VariableInputNodeConverter(),
            new DSVarArgFunctionConverter(),
            new Watch3DConverter(),
            new WatchConverter(),
            new ConvertNodeConverter()
        };
        #endregion

        public object CreateWorkspace(WorkspaceModel workspace)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException();
            }

            var nodes = CreateNodes(workspace.Nodes);
            var connections = CreateConnections(workspace.Connectors);
            var cnWorkspace = workspace as CustomNodeWorkspaceModel;
            var isCustomNode = cnWorkspace != null;

            dynamic result = new ExpandoObject();

            result.guid = isCustomNode ? cnWorkspace.CustomNodeId.ToString() : workspace.Guid.ToString();

            result.name = workspace.GetSharedName();

            result.nodes = nodes;
            result.connections = connections;
            result.notes = CreateNotes(workspace.Notes);
            result.groups = CreateGroups(workspace.Annotations);
            result.isCustomNode = isCustomNode;
            result.customNodeDescription = isCustomNode ? cnWorkspace.Description : "";
            result.customNodeCategory = isCustomNode ? cnWorkspace.Category : "";
            result.offset = new[] { workspace.X, workspace.Y };
            result.zoom = workspace.Zoom;
            result.isCustomizer = !isCustomNode;

            return result;
        }

        public IEnumerable<object> CreateNodes(IEnumerable<NodeModel> nodes)
        {
            var result = new List<object>();

            foreach (var nodeModel in nodes)
            {
                var converter = _converters.FirstOrDefault(c => c.ConvertsType(nodeModel));
                result.Add(converter != null ? converter.Convert(nodeModel) : new NodeToPublish(nodeModel));
            }

            return result;
        }

        public IEnumerable<object> CreateConnections(IEnumerable<ConnectorModel> connectors)
        {
            var result = new List<object>();

            foreach (var connector in connectors)
            {
                dynamic conn = new ExpandoObject();

                conn._id = connector.GUID;
                conn.kind = "addConnection";

                conn.startPortIndex = connector.Start.Index;
                conn.endPortIndex = connector.End.Index;

                conn.startNodeId = connector.Start.Owner.GUID.ToString();
                conn.endNodeId = connector.End.Owner.GUID.ToString();

                conn.startProxy = false;
                conn.endProxy = false;

                conn.startProxyPosition = new[] { 0, 1 };
                conn.endProxyPosition = new[] { 0, 1 };

                conn.hidden = false;

                result.Add(conn);
            }

            return result;
        }

        public IEnumerable<object> CreateNotes(IEnumerable<NoteModel> notes)
        {
            var result = new List<object>();

            foreach (var n in notes)
            {
                dynamic note = new ExpandoObject();

                note.id = n.GUID;
                note.noteText = n.Text;
                note.x = n.X;
                note.y = n.Y;

                result.Add(note);
            }

            return result;
        }

        public IEnumerable<object> CreateGroups(IEnumerable<AnnotationModel> groups)
        {
            return groups.Select(g => new GroupData(g)).ToList();
        }

        public object ConvertProps(WorkspaceProperties props)
        {
            dynamic result = new ExpandoObject();

            result.name = props.Name;
            result.description = props.Description;
            result.cameras = props.Cameras;

            return result;
        }
    }
}

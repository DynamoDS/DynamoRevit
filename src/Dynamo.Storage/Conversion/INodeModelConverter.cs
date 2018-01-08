using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion
{
    interface INodeModelConverter<T> where T : NodeModel
    {
        bool ConvertsType(T type);
        NodeToPublish Convert(T nodelModel);
    }
}

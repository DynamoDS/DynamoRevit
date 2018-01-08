using System.Reflection;

namespace Reach.Serialization
{
    public class NodeValueSerializerLibrary
    {
        private readonly Assembly assembly;
        private readonly string format;
        private readonly string typeName;

        public NodeValueSerializerLibrary(Assembly assembly, string format, string typeName)
        {
            this.assembly = assembly;
            this.format = format;
            this.typeName = typeName;
        }

        public INodeValueSerializer GetSerializer()
        {
            var result = assembly.CreateInstance(typeName) as INodeValueSerializer;
            return result;
        }

        public bool CanGetSerializer(string format)
        {
            return this.format == format;
        }
    }
}

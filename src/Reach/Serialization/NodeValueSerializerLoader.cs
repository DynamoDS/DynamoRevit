using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Reach.Serialization
{
    class NodeValueSerializerLoader
    {
        private NodeValueSerializerLibrary Load(string defDir, NodeValueSerializerDefinition definition)
        {
            var assembly = Assembly.LoadFrom(Path.Combine(defDir, definition.AssemblyPath));

            var result = new NodeValueSerializerLibrary(assembly, definition.Format, definition.TypeName);
            return result;
        }

        public NodeValueSerializerLibrary Load(string libraryPath)
        {
            var document = new XmlDocument();
            document.Load(libraryPath);

            var topNode = document.GetElementsByTagName("NodeValueSerializerDefinition");

            if (topNode.Count == 0)
            {
                return null;
            }

            var definition = new NodeValueSerializerDefinition();
            foreach (XmlNode item in topNode[0].ChildNodes)
            {
                if (item.Name == "AssemblyPath")
                {
                    definition.AssemblyPath = item.InnerText;
                }
                else if (item.Name == "TypeName")
                {
                    definition.TypeName = item.InnerText;
                }
                else if (item.Name == "Format")
                {
                    definition.Format = item.InnerText;
                }
            }

            var defPath = Uri.UnescapeDataString(new UriBuilder(document.BaseURI).Path);
            var defDir = Path.GetDirectoryName(defPath);
            var library = Load(defDir, definition);
            return library;
        }

        public IEnumerable<NodeValueSerializerLibrary> LoadDirectory(string librariesPath)
        {
            var result = new List<NodeValueSerializerLibrary>();
            string serializersDir = null;

            if(librariesPath != null)
            {
                if(Directory.Exists(librariesPath))
                {
                    serializersDir = librariesPath;
                }
                else if(!Path.IsPathRooted(librariesPath))
                {
                    var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var serializersFullpathDir = Path.GetFullPath(Path.Combine(assemblyDir, librariesPath));

                    if(Directory.Exists(serializersFullpathDir))
                    {
                        serializersDir = serializersFullpathDir;
                    }
                }
            }

            if(serializersDir != null)
            {
                var files = Directory.GetFiles(serializersDir, "*_NodeValueSerializerDefinition.xml");
                foreach (var file in files)
                {
                    var library = Load(file);
                    if (library != null)
                    {
                        result.Add(library);
                    }
                }
            }

            return result;
        }
    }
}

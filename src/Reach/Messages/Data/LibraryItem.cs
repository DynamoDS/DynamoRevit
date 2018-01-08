using Dynamo.Search.SearchElements;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Reach.Messages.Data
{
    /// <summary>
    /// A simple version of the SearchElementBase class needed for sending data to a web client
    /// </summary>
    [DataContract]
    public class LibraryItem
    {
        /// <summary>
        /// Full category name
        /// </summary>
        [DataMember]
        public string Category { get; private set; }

        /// <summary>
        /// Model name in the list of all node models
        /// </summary>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// Unique name that is used during node creation
        /// </summary>
        [DataMember]
        public string CreationName { get; private set; }

        /// <summary>
        /// The name that will be displayed on node itself 
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// A string describing what the node does
        /// </summary>
        [DataMember]
        public string Description { get; private set; }

        /// <summary>
        /// Number defining the relative importance of the element in search. 
        /// Higher = closer to the top of search results
        /// </summary>
        [DataMember]
        public double Weight { get; private set; }

        /// <summary>
        /// This property represents the list of words used for element search.
        /// </summary>
        [DataMember]
        public IEnumerable<string> Keywords { get; private set; }

        /// <summary>
        /// This property represents the list of weights associated with each search keyword.
        /// </summary>
        [DataMember]
        public IEnumerable<double> KeywordWeights { get; private set; }

        /// <summary>
        /// This property represents the list of node inputs.
        /// </summary>
        [DataMember]
        public IEnumerable<PortInfo> Parameters { get; set; }
        
        /// <summary>
        /// This property represents the list of function arguments
        /// which this node has as inputs - this property is only populated
        /// if the function is overloaded, and may have fewer values than
        /// the Parameters Property as not all inputs to nodes are inputs to
        /// the functions they call - i.e. Instance methods.
        /// </summary>
        [DataMember]
        public IEnumerable<string> Arguments { get; set; }
        
        /// <summary>
        /// This property represents the list of node outputs.
        /// </summary>
        [DataMember]
        public IEnumerable<PortInfo> ReturnKeys { get; set; }

        /// <summary>
        /// Additional data to create the node
        /// </summary>
        [DataMember]
        public object Extra { get; set; }

        /// <summary>
        /// ConcreteType of the node in Dynamo, represented as the fully qualified assembly name
        /// </summary>
        [DataMember]
        public string ConcreteType { get; set; }

        [JsonConstructor]
        public LibraryItem(string Category, string Name, string CreationName, string Description, string ConcreteType)
        {
            this.Category = Category;
            this.DisplayName = this.Name = Name;
            this.CreationName = CreationName;
            this.Description = Description;
            this.Arguments = new List<string>();
            this.ConcreteType = ConcreteType;

        }

        public LibraryItem(NodeSearchElement node, IEnumerable<string> keywords, IEnumerable<double> keywordWeights)
        {
            Category = node.FullCategoryName;
            DisplayName = Name = node.Name;
            CreationName = node.CreationName;
            Description = node.Description;
            Keywords = keywords;
            KeywordWeights = keywordWeights;
            this.Arguments = new List<string>();
            //this is how we serialize type to JSON with json.net
            var nodeModel = node.CreateNode();
            this.ConcreteType = nodeModel.GetType().FullName+", "+ nodeModel.GetType().Assembly.GetName().Name;
            
        }


        public struct PortInfo
        {
            /// <summary>
            /// This property represents displayed name of the port.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// This property represents the type of the port. It can contain type name such
            /// as 'String', 'Point', 'Vertex' and other.
            /// It doesn't care about exact type, value can be 'Autodesk.DesignScript.Geometry.Point'
            /// or just 'Point' as well.
            /// This property only help users to understand what kind of value does the port expects.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// This property represents the default value for each of the input parameters.
            /// It can potentially contain primitive value types such as 'int', 'double', 'bool'
            /// and 'string'. If no default value is given to an input parameter, then it has 
            /// the corresponding entry in 'DefaultValues' as 'null'.
            /// </summary>
            public object DefaultValue { get; set; }

            /// <summary>
            /// The tooltip content of this port.
            /// </summary>
            public string ToolTip { get; set; }
        }
    }
}

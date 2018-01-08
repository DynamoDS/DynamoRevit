using System.Linq;
using System.Runtime.Serialization;
using Dynamo.Graph.Nodes;
using ProtoCore.Mirror;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System;

namespace Reach.Responses
{
    class ArrayItemsDataResponse: Response
    {
        /// <summary>
        /// Guid of the specified node
        /// </summary>
        [DataMember]
        public string NodeId { get; private set; }

        /// <summary>
        /// String representing of the data about input,
        /// output ports, text of specified code block node
        /// </summary>
        [DataMember]
        public string Items { get; private set; }

        /// <summary>
        /// Index from which Dynamo responds array items
        /// </summary>
        [DataMember]
        public int IndexFrom { get; set; }

        /// <summary>
        /// The response with a Json text representation of an array range or nested array range
        /// contained in a node with specified <see cref="NodeId"/> identifier
        /// </summary>
        /// <param name="node">Node with data</param>
        /// <param name="indexFrom">Index from which Dynamo responds array items</param>
        /// <param name="length">Count of returned items</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"ArrayItemsDataResponse",
        ///     "nodeId":"61e47bad-8d44-6d3e-0b81-557ca672c880",
        ///     "items":[
        ///         "Point(X = 1.000, Y = 1.000, Z = 0.000)",
        ///         "Point(X = 2.000, Y = 2.000, Z = 0.000)",
        ///         "Point(X = 3.000, Y = 3.000, Z = 0.000)"
        ///     ]
        ///     "indexFrom":0,
        ///     "status":0
        /// }
        /// </code>
        public ArrayItemsDataResponse(NodeModel node, int indexFrom, int length)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            
            NodeId = node.GUID.ToString();

            if (node.CachedValue == null)
            {
                IndexFrom = -1;
                Items = "";
                return;
            }

            if (node.CachedValue.IsCollection)
            {
                var allItems = node.CachedValue.GetElements();
                if (allItems.Count() < indexFrom)
                {
                    Items = "";
                    IndexFrom = allItems.Count();
                }
                else
                {
                    length--; //remove one for the top level collection
                    Items = JsonConvert.SerializeObject(GetValueFromMirrorData(allItems,ref length),jsonSettings);
                    IndexFrom = indexFrom;
                }
            }
        }

        /// <summary>
        /// returns nested elements of the node result up to length requested
        /// </summary>
        /// <param name="allItems">collection of node MirrorData</param>
        /// <param name="length">total display items to return</param>
        /// <returns></returns>
        private object GetValueFromMirrorData(IEnumerable<MirrorData> allItems, ref int length)
        {
            var returnValues = new List<object>();

            foreach (MirrorData cachedValue in allItems)
            {

                if (length <= 0) break;

                length--;

                if (cachedValue == null)
                {
                    returnValues.Add("null");
                    continue;
                }

                if (cachedValue.IsCollection)
                {
                    var subItems = cachedValue.GetElements();

                    if (subItems.Count() == 0)
                    {
                        returnValues.Add("Empty List");
                        continue;
                    }

                    returnValues.Add(GetValueFromMirrorData(subItems, ref length));
                    continue;
                }

                if (cachedValue.Data != null)
                {
                    returnValues.Add(cachedValue.Data.ToString());
                    continue;
                }

                if (!cachedValue.IsNull && cachedValue.Class != null)
                {
                    returnValues.Add(cachedValue.Class.ClassName);
                }
                else
                { 
                    returnValues.Add("null");
                }
            }

            return returnValues.ToArray();

        }

    }
}

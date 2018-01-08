using System.Runtime.Serialization;
using System.Linq;
using CoreNodeModels;
using Dynamo.Graph.Nodes;
using ProtoCore.Mirror;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace Reach.Messages.Data
{
    /// <summary>
    /// The class that represents calculated result for a node
    /// </summary>
    [DataContract]
    public class ExecutedNode
    {
        /// <summary>
        /// Guid of the specified node
        /// </summary>
        [DataMember]
        public string NodeId { get; private set; }

        /// <summary>
        /// State of the node after executing
        /// </summary>
        [DataMember]
        public string State { get; private set; }

        /// <summary>
        /// State description. It is empty when state has Active or Dead value
        /// </summary>
        [DataMember]
        public string StateMessage { get; private set; }

        /// <summary>
        /// String representing of the result object
        /// </summary>
        [DataMember]
        public string Data { get; private set; }

        /// <summary>
        /// Total number of items the node result object contains.
        /// It will be null when the node result is not a list.
        /// </summary>
        [DataMember]
        public object NumItems { get; private set; }

        /// <summary>
        /// Maximum depth of nested lists the node result object contains.
        /// It will be null when the node result is not a list.
        /// </summary>
        [DataMember]
        public object MaxDepth { get; private set; }

        /// <summary>
        /// Total number of items the node result contains including collections.
        /// This value would represent the length of the fully expanded tree view of the data
        /// It will be null when the node result is not a list.
        /// </summary>
        [DataMember]
        public object NumElements { get; private set; }

        // <summary>
        /// Total number of items the node result contains per results nested list structure.
        /// Allows for recursive access to the totals as you traverse the data's tree structure.
        /// The string format matches ArrayItemsDataResponce.Items.
        /// It will be null when the node result is not a list.
        /// </summary>
        [DataMember]
        public string Totals { get; private set; }

        /// <summary>
        /// Indicates whether the result object should be drawn on the canvas
        /// </summary>
        [DataMember]
        public bool ContainsGeometryData { get; private set; }

        /// <summary>
        /// call this method to explicitly set the calculated response for a node
        /// </summary>
        /// <param name="NodeId"></param>
        /// <param name="State"></param>
        /// <param name="StateMessage"></param>
        /// <param name="Data"></param>
        /// <param name="NumItems"></param>
        /// <param name="MaxDepth"></param>
        /// <param name="Totals"></param>
        /// <param name="ContainsGeometryData"></param>
        [JsonConstructor]
        public ExecutedNode(string NodeId, string State, string StateMessage, string Data, object NumItems, object MaxDepth, string Totals, bool ContainsGeometryData)
        {
            this.NodeId = NodeId;
            this.State = State;
            this.StateMessage = StateMessage;
            this.Data = Data;
            this.NumItems = NumItems;
            this.MaxDepth = MaxDepth;
            this.Totals = Totals;
            this.ContainsGeometryData = ContainsGeometryData;
        }

        /// <summary>
        /// call this method to set the calculated response for a node with it's node model
        /// </summary>
        /// <param name="node">Node with data</param>
        /// <param name="data">Node result data, varies per node type and node.cachedValue type(</param>
        /// <param name="hasRenderPackages">boolean defining if result has associated geometry</param>
        public ExecutedNode(NodeModel node, string data, bool hasRenderPackages)
        {
            this.NodeId = node.GUID.ToString();
            this.State = node.State.ToString();
            this.StateMessage = node.ToolTipText;
            this.Data = data;
            this.ContainsGeometryData = hasRenderPackages;

            // This is a stop gap to prevent a crash during race condition REACH-290
            try
            {
                // ColorRange node does not show its value
                // So it is not included into Data
                if (!(node is ColorRange) && node.CachedValue != null && node.CachedValue.IsCollection)
                {
                    // Select maximum list depth and total item qauntity
                    Result depthAndNumbers = GetMaximumDepthAndItemNumber(node.CachedValue);
                    this.NumItems = depthAndNumbers.numItems;
                    this.MaxDepth = depthAndNumbers.maxDepth;
                    this.NumElements = depthAndNumbers.numElements;
                    // Gather sublist data structure 
                    // Todo need to create json data structure for Totals for key value access on client
                    this.Totals = JsonConvert.SerializeObject(GatherListTotals(depthAndNumbers));
                }
            }
            catch
            {    
            }

        }

        /// <summary>
        /// Return List data through recursive traversal for total non-list items and max tree depth.
        /// Stores all intermediate results for item totals to send tree structure data to client. 
        /// </summary>
        private Result GetMaximumDepthAndItemNumber(MirrorData cachedValue)
        {
            if (!cachedValue.IsCollection)
            {
                var resultItem = new Result { numItems = 1, maxDepth = 1, numElements = 1};
                return resultItem;
            }
            else
            {   
                // call the method rescursively and evaluate the results
                var depthAndNumbers = cachedValue.GetElements().Select(GetMaximumDepthAndItemNumber).ToList();
                var maxDepth = depthAndNumbers.Select(t => t.maxDepth).DefaultIfEmpty(1).Max() + 1;
                var numItems = depthAndNumbers.Select(t => t.numItems).Sum();
                var numElements = depthAndNumbers.Select(t => t.numElements).Sum() + 1;
                var arrayLength = cachedValue.GetElements().Count();

                if (numItems == 0 && maxDepth == 2) //if the collection is an empty list
                {
                    var resultEmptyList = new Result { numItems = numItems, maxDepth = maxDepth, numElements = numElements, arrayLength = arrayLength};
                    return resultEmptyList;
                }
                else //if the collection contains items or other collections
                {
                    var resultList = new Result { numItems = numItems, maxDepth = maxDepth, numElements = numElements, arrayLength = arrayLength, results = depthAndNumbers};
                    return resultList;
                }

            }
        }

        /// <summary>
        /// Return nested list items total and display element totals in format that matches the nested array response format for ArrayItemDataResponce. 
        /// totals[list length, list total, element total, [totals[...]]] at the top and totals[list length, list total, element total, []] at the deepest list.
        /// </summary>
        private object GatherListTotals(Result depthAndNumbers)
        {

            if (depthAndNumbers.results != null)
            {
                var totals = depthAndNumbers.results.Select(GatherListTotals).ToList();
                //Typical case when all items are value and not collection -> {A,B,C} returns [3, 3, 4, []]
                //In this case all the totals values will be null
                //Use First() == null as fast initial test.  Disctict is expensive on long lists 
                if (totals.First() == null && totals.Distinct().Count() == 1)
                {
                    object[] empty = { };
                    object[] values = { depthAndNumbers.arrayLength.ToString(), depthAndNumbers.numItems.ToString(), depthAndNumbers.numElements.ToString(), empty };
                    return values;

                }
                //The eles handles case for jagged lists where you have a combination of values and collections 
                //-> {A,{B, C},D} returns [3, 4, 6, [null, [2, 2, 3, []], null]]
                //We ensure there are null place holders when a list contains both values and collections
                //so when clients are travering the node result lists, array index will allign with this data.
                else
                {
                    object[] values = { depthAndNumbers.arrayLength.ToString(), depthAndNumbers.numItems.ToString(), depthAndNumbers.numElements.ToString(), totals.ToArray() };
                    return values;
                }


            }
            else if(depthAndNumbers.numItems == 0) //Handle empty lists
            {
                object[] empty = { };
                object[] values = { depthAndNumbers.arrayLength.ToString(), depthAndNumbers.numItems.ToString(), depthAndNumbers.numElements.ToString(), empty };
                return values;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// The class that represents meta-data for lists in node result
        /// </summary>
        private class Result
        {
            public int numItems { get; set; }
            public int maxDepth { get; set; }
            public int numElements { get; set; }
            public int arrayLength { get; set; }

            public IEnumerable<Result> results { get; set; }
        }
    }
}

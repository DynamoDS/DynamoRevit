using System.IO;
using System.Linq;
using System.Text;
using CoreNodeModels;
using CoreNodeModels.Input;
using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Dynamo.Storage.Conversion.ExtraInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Watch3DNodeModels;
using Newtonsoft.Json.Linq;

namespace Reach
{
    public static class Extensions
    {
        private static readonly JsonSerializer CamelCaseSerializer = new JsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string GetExtraData(this CodeBlockNodeModel codeBlock)
        {
            var stringBuilder = new StringBuilder();
            var allDefs = CodeBlockUtils.GetDefinitionLineIndexMap(codeBlock.CodeStatements);
            var lineIndices = allDefs.Select(def => def.Value - 1).ToList();

            stringBuilder.Append("\"Code\":\"");
            stringBuilder.Append(codeBlock.Code.
                                 Replace("\n", "\\n").
                                 Replace("\"", "\\\""));
            stringBuilder.Append("\", ");
            stringBuilder.Append("\"LineIndices\": [");
            stringBuilder.Append(string.Join(", ", lineIndices.Select(x => x.ToString()).ToArray()));
            stringBuilder.Append("],");

            return stringBuilder.ToString();
        }

        public static string GetExtraData(this VariableInputNode varNode)
        {
            var stringBuilder = new StringBuilder();
            var type = varNode.GetType();

            if (type.Name == "PythonNode")
            {
                var script = type.GetProperty("Script").GetValue(varNode, null).ToString();

                stringBuilder.Append("\"Script\":\"");
                stringBuilder.Append(script.
                                     Replace("\n", "\\n").
                                     Replace("\"", "\\\""));
                stringBuilder.Append("\", ");
            }

            return stringBuilder.ToString();
        }

        internal static string GetExtraData(this Watch3D watchNode, string data)
        {
            var extra = new Watch3DExtra(watchNode);
            var jo = JObject.FromObject(extra, CamelCaseSerializer);
            jo.Add("value", data);
            return jo.ToString();
        }

        internal static string GetExtraData(this DynamoConvert cNode, string value)
        {
            return new ConvertNodeExtra(cNode).ToJsonString(false);
        }

        public static string GetInOutPortsData(this NodeModel node)
        {
            var stringBuilder = new StringBuilder("{");

            if (node is CodeBlockNodeModel)
            {
                stringBuilder.Append((node as CodeBlockNodeModel).GetExtraData());
            }
            else if (node is VariableInputNode)
            {
                stringBuilder.Append((node as VariableInputNode).GetExtraData());
            }

            stringBuilder.Append(node.GetInOutPorts());
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        public static string GetValue(this NodeModel node, EngineController engine)
        {
            // ColorRange node does not show its value
            // So it is not included into Data
            if (node is ColorRange)
            {
                var drawData = (node as ColorRange).ComputeColorRange(engine).IndexedColors;
                return drawData.ToJsonString();
            }

            if (node is DoubleSlider)
            {
                return new DoubleInputSliderExtra(node).ToJsonString(false);
            }

            if (node is IntegerSlider)
            {
                return new IntegerSliderExtra(node).ToJsonString(false);
            }

            if (node is DoubleInput)
            {
                return (node as DoubleInput).Value;
            }
            
            if (node is StringInput)
            {
                return (node as StringInput).Value;
            }
            
            if (node is Symbol)
            {
                return (node as Symbol).InputSymbol;
            }
            
            if (node is Output)
            {
                return (node as Output).Symbol;
            }

            if (node.CachedValue == null) return "null";

            if (node.CachedValue.IsCollection)
            {
                return "Array";
            }
                
            if (node.CachedValue.Data != null)
            {
                try
                {
                    return node.CachedValue.Data.ToString();
                }
                catch { }
            }

            return "null";
        }

        public static string GetInOutPorts(this NodeModel node)
        {
            var isVarInputNode = node is VariableInputNode;
            var stringBuilder = new StringBuilder();

            var pattern = isVarInputNode ? "{{ \"name\":\"{0}\", \"type\": \"{1}\" }}" : "\"{0}\"";
            var inPorts = node.InPorts.Select(port => string.Format(pattern, port.Name, port.ToolTip)).ToList();
            var outPorts = node.OutPorts.Select(port =>
            {
                var toolTipContent = port.ToolTip;
                var toolTip_guid = ProtoCore.DSASM.Constants.kTempVarForNonAssignment;

                // replace dynamo's prefix with flood's one for all CBN with unassigned variables
                if (toolTipContent.IndexOf(toolTip_guid) == -1) return "\"" + toolTipContent + "\"";
                var tempArray = toolTipContent.Split(new string[] { toolTip_guid }, System.StringSplitOptions.None);
                return (tempArray.Length > 1) 
                    ? "\"" + Dynamo.Storage.Utilities.Constants.ReachTempVarForNonAssignment + tempArray[1] + "\""
                    : "\"" + toolTipContent + "\"";
            }).ToList();

            stringBuilder.Append(isVarInputNode ? "\"varInputs\": [" : "\"InPorts\": [");
            stringBuilder.Append(inPorts.Any() ? inPorts.Aggregate((i, j) => i + "," + j) : "");
            stringBuilder.Append("], \"OutPorts\": [");
            stringBuilder.Append(outPorts.Any() ? outPorts.Aggregate((i, j) => i + "," + j) : "");
            stringBuilder.Append("]");

            return stringBuilder.ToString();
        }

        public static string ToJsonString(this object obj, bool includeTypeName = true)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = includeTypeName ? TypeNameHandling.Objects : TypeNameHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public static Stream ToJsonStream(this object obj)
        {
            return ToJsonString(obj).ToStream();
        }

        public static Stream ToStream(this string text)
        {
            return new MemoryStream(new UTF8Encoding().GetBytes(text)) { Position = 0 };
        }

        /// <summary>
        ///     Test whether two objects' members are deeply equal by-value, by converting them both to JSON 
        ///     and comparing the resulting strings.
        ///     Warning: do not use on objects that could contain cyclic references.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>Whether the two objects have the same JSON representation.</returns>
        public static bool MemberwiseEquals(this object a, object b)
        {
           string jsonStringA = JsonConvert.SerializeObject(a, new JsonSerializerSettings
           {
              TypeNameHandling = TypeNameHandling.None
           });
           
           string jsonStringB = JsonConvert.SerializeObject(b, new JsonSerializerSettings
           {
              TypeNameHandling = TypeNameHandling.None
           });

           return jsonStringA.Equals(jsonStringB);
        }
    }
}

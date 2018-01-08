using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dynamo.Models;

namespace Reach.Conversion
{
    public class MessageSerializationBinder: SerializationBinder
    {
        public string TypeFormat { get; private set; }

        private static readonly Dictionary<string, string> MapTypes = new Dictionary<string, string>
        {
            {"ccn", typeof(DynamoModel.CreateCustomNodeCommand).Name},
            {"cn", typeof(DynamoModel.CreateNodeCommand).Name},
            {"cnt", typeof(DynamoModel.CreateNoteCommand).Name},
            {"cnp", typeof(DynamoModel.CreateProxyNodeCommand).Name},
            {"dm", typeof(DynamoModel.DeleteModelCommand).Name},
            {"mc", typeof(DynamoModel.MakeConnectionCommand).Name},
            {"me", typeof(DynamoModel.ModelEventCommand).Name},
            {"rc", typeof(DynamoModel.RecordableCommand).Name},
            {"rcn", typeof(DynamoModel.RunCancelCommand).Name},
            {"umv", typeof(DynamoModel.UpdateModelValueCommand).Name}
        };

        public MessageSerializationBinder(string typeFormat)
        {
            TypeFormat = typeFormat;
        }
 
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
 
        public override Type BindToType(string assemblyName, string typeName)
        {
            string resolvedTypeName;
            if (!MapTypes.ContainsKey(typeName)) 
            {
                resolvedTypeName = string.Format(typeName + ", {0}", assemblyName);
            }
            else 
            {
                typeName = MapTypes[typeName];
                resolvedTypeName = string.Format(TypeFormat, typeName);
            }           
 
            return Type.GetType(resolvedTypeName, true);
        }
    }
}

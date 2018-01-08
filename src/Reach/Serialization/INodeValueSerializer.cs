using System;
using System.Collections.Generic;

namespace Reach.Serialization
{
    /// <summary>
    /// An interface describing a component that serializes a NodeModel's 
    /// CachedValue property
    /// </summary>
    public interface INodeValueSerializer
    {
        /// <summary>
        /// A unique name for the Serializer.  This is used for logging and so users can specify the serializer.
        /// </summary>
        string UniqueName { get; }

        /// <summary>
        /// Returns the Type of object that this instance can serialize.
        /// </summary>
        Type GetSerializerType();

        /// <summary>
        /// Take a value extracted from NodeModel.CachedValue and serializes it.
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <param name="serializerParams"></param>
        /// <returns>The serialized object</returns>
        string Serialize(object value, Dictionary<string, object> serializerParams);
    }
}

using System.Runtime.Serialization;
using Reach.Execution;

namespace Reach.Messages
{
    [DataContract]
    public abstract class Message
    {
        [DataMember]
        public int? UniqueID { get; set; }

        internal abstract void Execute(MessageHandler handler);
    }
}

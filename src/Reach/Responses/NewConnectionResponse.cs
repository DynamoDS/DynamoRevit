using System.Runtime.Serialization;

namespace Reach.Responses
{
    /// <summary>
    /// Message sent to a WebSocket client upon making a connection
    /// </summary>
    public class NewConnectionResponse : Response
    {
        /// <summary>
        /// The name of the machine the user has connected to 
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        /// Dynamo version value
        /// </summary>
        [DataMember]
        public string DynamoVersion { get; set; }
    }
}

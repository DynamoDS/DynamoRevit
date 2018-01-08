using System.Runtime.Serialization;

namespace Reach.Responses
{
    /// <summary>
    /// Specifies whether a valid token has been supplied or not.
    /// </summary>
    public class TokenValidationResponse : Response
    {
        /// <summary>
        /// Whether the validation was a success or not
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// A message indicating why the validation was not a success
        /// </summary>
        [DataMember]
        public string FailureDetails { get; set; }
    }
}

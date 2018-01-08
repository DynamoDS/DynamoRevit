using System.Runtime.Serialization;
using Reach.Responses;

namespace Reach.WebSocket
{
    [DataContract]
    public class TokenValidation
    {
        private readonly bool success;

        public string FailureDetails { get; set; }

        [DataMember]
        public string UserId { get; set; }

        public TokenValidation()
        {
            success = true;
        }

        public TokenValidation(string userId, string sessionId)
        {
            this.success = true;
            this.UserId = userId;
        }

        public TokenValidation(string failureDetails)
        {
            this.success = false;
            this.FailureDetails = failureDetails;
        }

        public TokenValidationResponse ToResponse()
        {
            return new TokenValidationResponse()
            {
                Success = this.success,
                FailureDetails = this.FailureDetails
            };
        }

        public static implicit operator bool(TokenValidation v)
        {
            return v.success;
        }
    }
}
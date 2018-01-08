namespace Reach.Responses
{
    public enum ResponseStatuses
    {
        Success,
        Error,
        ResetByPeer
    }

    public abstract class Response
    {
        public ResponseStatuses Status { get; private set; }
        public int? UniqueID { get; set; }

        public Response(ResponseStatuses status)
        {
            Status = status;
        }

        public Response()
        {
            Status = ResponseStatuses.Success;
        }
    }
}

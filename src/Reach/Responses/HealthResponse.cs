namespace Reach.Responses
{
    /// <summary>
    /// Simple response to send on a health check.  Currently it's a yes or no
    /// response.  If the response is received, and only if it is received,
    /// the server is healthy.
    /// </summary>
    public class HealthResponse : Response { }
}

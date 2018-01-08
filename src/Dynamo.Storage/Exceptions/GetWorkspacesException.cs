using System;

namespace Dynamo.Storage.Exceptions
{
    public class GetWorkspacesException : Exception
    {
        public GetWorkspacesException(string message)
            : base(message)
        {
        }
    }
}

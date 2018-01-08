using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph.Workspaces;
using Dynamo.Storage.Conversion;
using Dynamo.Storage.Data;
using Dynamo.Storage.Exceptions;
using Greg;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Extensions.MonoHttp;

namespace Dynamo.Storage.Upload
{
    /// <summary>
    /// A HTTP response from the Flood persistence service
    /// </summary>
    public interface IFloodHttpResponse
    {
        string Content { get; }
        HttpStatusCode StatusCode { get; }
    }

    public class FileDependencyParameters
    {
        private string workspaceId;
        private string originalPath;
        private IEnumerable<Guid> nodes;

        /// <summary>
        /// The unique identifier of the Workspace with which you
        /// would like to associate this FileDependency.
        /// </summary>
        [JsonProperty("workspace_id")]
        public string WorkspaceId { get { return workspaceId; } }

        /// <summary>
        /// The path to the file on disk.
        /// </summary>
        [JsonProperty("original_path")]
        public string OriginalPath { get { return originalPath; } }

        /// <summary>
        /// A collection of NodeModels which use this
        /// FileDependency
        /// </summary>
        [JsonProperty("nodes")]
        public IEnumerable<Guid> Nodes { get { return nodes; } }

        public FileDependencyParameters(string workspaceId, string originalPath, IEnumerable<Guid> nodes)
        {
            if(string.IsNullOrEmpty(workspaceId))
            {
                throw new ArgumentException("The id cannot be empty.", "workspaceId");
            }

            this.workspaceId = workspaceId;

            if (!File.Exists(originalPath))
            {
                throw new FileNotFoundException("The specified file path does not exist.");
            }

            if (!Path.IsPathRooted(originalPath))
            {
                throw new ArgumentException("The path must be an absolute path.", "originalPath");
            }

            this.originalPath = originalPath;

            this.nodes = nodes;
        }
    }

    public class WorkspaceStorageClient : IWorkspaceStorageClient
    {
        private readonly IAuthProvider authProvider;
        private readonly string url;
        private readonly ReachWorkspaceConverter workspaceConverter;

        /// <summary>
        /// A session cookie returned in a response.
        /// This cookie can be used in downstream requests,
        /// bypassing other authentication strategies.
        /// </summary>
        private RestResponseCookie sessionCookie;

        /// <summary>
        /// The connect.sid cookie originates from the connect
        /// session middleware used by the Flood server.
        /// </summary>
        private const string sessionCookieName = "connect.sid";

        /// <summary>
        /// Constructor for WorkspaceStorageClient
        /// </summary>
        /// <param name="authProvider">Instance of <seealso cref="IAuthProvider"/></param>
        /// <param name="url">Base url for <seealso cref="RestClient"/></param>
        public WorkspaceStorageClient(IAuthProvider authProvider, string url)
        {
            this.workspaceConverter = new ReachWorkspaceConverter();
            this.authProvider = authProvider;
            this.url = url;
        }

        /// <summary>
        /// This method sends workspace and it dependencies using <seealso cref="RestClient"/> and
        /// <seealso cref="IAuthProvider"/> for signature
        /// </summary>
        /// <param name="workspace">Home workspace</param>
        /// <param name="dependencies">Home workspace dependencies</param>
        /// <param name="props">Home workspace properties</param>
        /// <returns>Response from Rest POST request</returns>
        public async Task<IFloodHttpResponse> Send(HomeWorkspaceModel workspace, IEnumerable<CustomNodeWorkspaceModel> dependencies, WorkspaceProperties props = null)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException();
            }

            var wss = new List<WorkspaceModel> { workspace };
            wss.AddRange(dependencies);

            var invalidNodes = wss.SelectMany(ws => NodeFilter.GetDisabledNodes(ws.Nodes));
            if (invalidNodes.Any())
            {
                throw new InvalidNodesException(invalidNodes);
            }

            var workspaces = CreateWorkspaces(wss);
            var properties = props != null ? ConvertProps(props) : new {};
            var postRequest = new
            {
                Workspaces = workspaces,
                Props = properties
            };

            var request = new RestRequest("ws")
            {
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            try
            {
                //serialize with camel case property names
                var serialized = await Task.Run(() => JsonConvert.SerializeObject(postRequest, Formatting.None, settings));

                // Flakiness with Revit OAuth requires that we pass
                // this as a file parameter
                var f = Encoding.UTF8.GetBytes(serialized);
                var p = FileParameter.Create("data", f, "data");
                request.Files.Add(p);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Unable to serialize objects", e);
            }

            var client = new RestClient(url);
            SignRequest(ref request, client);

            var response = await Task.Run(() => client.Execute(request));

            // We cache the sessionCookie here to allow downstream requests to
            // re-use it. This is required for workflows like posting, then rolling 
            // back changes, where you need to request DELETE for /ws. The AdWebServices 
            // component does not correctly sign Delete requests, so a call to /validateAuthorization will 
            // fail on the server. If we attach this cookie to the request, we bypass
            // other authentication strategies.
            sessionCookie = response.Cookies.FirstOrDefault(c => c.Name == sessionCookieName);

            HasIncorrectTimeStamp(response);

            return new WorkspaceStorageHttpResponse(response);
        }

        public async Task<IEnumerable<Workspace>> GetWorkspaces()
        {
            var req = new RestRequest("ws/")
            {
                Method = Method.GET,
                RequestFormat = DataFormat.Json
            };

            var restClient = new RestClient(url);
            SignRequest(ref req, restClient);

            var resp = await Task.Run(() => restClient.Execute(req));

            HasIncorrectTimeStamp(resp);

            if (resp.StatusCode != HttpStatusCode.OK)
            {
                throw new GetWorkspacesException("Internal server error!");
            }

            IEnumerable<Workspace> result = null;

            try
            {
                result = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<IEnumerable<Workspace>>(resp.Content));
            }
            catch
            {
                throw new GetWorkspacesException("Malformed response!");
            }
            return result;
        }

        public Task<bool> GetTermsOfUseAcceptanceStatus()
        {
            return ExecuteTermsOfUseCall(true);
        }

        public Task<bool> SetTermsOfUseAcceptanceStatus()
        {
            return ExecuteTermsOfUseCall(false);
        }

        /// <summary>
        /// Delete a Workspace.
        /// </summary>
        /// <returns>An <see cref="IFloodHttpResponse"/> object.</returns>
        public async Task<IFloodHttpResponse> DeleteWorkspaceAsync(string workspaceId)
        {
            try
            {
                var request = new RestRequest("ws/" + workspaceId)
                {
                    Method = Method.DELETE,
                };

                var client = new RestClient(url);

                // Due to a bug in AdWebServices, signing of DELETE requests does
                // not work. Such a request will fail to validate on the server.
                // If we have the session cookie available, we use it here
                // to avoid this failure.
                if(sessionCookie != null)
                {
                    request.AddCookie(sessionCookie.Name, sessionCookie.Value);
                }else
                {
                    SignRequest(ref request, client);
                }
                
                var response = await Task.Run(() => client.Execute(request));
                return new WorkspaceStorageHttpResponse(response);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Create a file dependency for a Workspace.
        /// </summary>
        /// <param name="parameters">A <see cref="FileDependencyParameters"/> object.</param>
        /// <returns>An <see cref="IFloodHttpResponse"/> object.</returns>
        public async Task<IFloodHttpResponse> UploadFileDependencyAsync(FileDependencyParameters parameters)
        {
            try
            {
                var request = new RestRequest("file_dependency")
                {
                    Method = Method.POST,
                    RequestFormat = DataFormat.Json,
                    AlwaysMultipartFormData = true
                };

                request.AddFile("data", parameters.OriginalPath);

                var client = new RestClient(url);
                SignRequest(ref request, client);

                // Add this parameter AFTER signing the request to avoid a bug in certain authenticators, which 
                // puts all parameters, regardless of whether they will end up as form data, into the query string.
                request.AddParameter("fileDependencyParameters", JsonConvert.SerializeObject(parameters), ParameterType.GetOrPost);

                var response = await Task.Run(() => client.Execute(request));
                return new WorkspaceStorageHttpResponse(response);
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> ExecuteTermsOfUseCall(bool queryAcceptanceStatus)
        {
            var reqMethod = queryAcceptanceStatus ? Method.GET : Method.PUT;
            var request = new RestRequest("tou/")
            {
                Method = reqMethod,
                RequestFormat = DataFormat.Json
            };

            var restClient = new RestClient(url);

            SignRequest(ref request, restClient);
 
            var response = await Task.Run(() => restClient.Execute(request));

            // If connection to flood failed, throw the exception contained in the response so client could handle it.
            if(response.ErrorException != null && response.StatusCode != HttpStatusCode.OK)
            {
                throw response.ErrorException;
            }

            HasIncorrectTimeStamp(response);

            return (await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<TermsOfUseResponse>(response.Content))).Content.Accepted;

        }

        private IEnumerable<object> CreateWorkspaces(IEnumerable<WorkspaceModel> wss)
        {
            return wss.Select(workspaceConverter.CreateWorkspace).ToList();
        }

        private object ConvertProps(WorkspaceProperties props)
        {
            return workspaceConverter.ConvertProps(props);
        }

        private void SignRequest(ref RestRequest req, IRestClient client)
        {
            var unsignedRequestResource = req.Resource + '?';
            authProvider.SignRequest(ref req, (RestClient) client);
            
            if (unsignedRequestResource == req.Resource)
            {
                throw new FormatException("Request has not been signed due to incorrect local date");
            }
        }

        private static void HasIncorrectTimeStamp(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.NotAcceptable) return;

            var content = HttpUtility.UrlDecode(response.Content).ToLower();
            if (content.Contains("invalid") && content.Contains("timestamp"))
            {
                throw new InvalidTimeZoneException();
            }
        }

        private sealed class WorkspaceStorageHttpResponse : IFloodHttpResponse
        {
            private readonly IRestResponse internalResponse;

            public WorkspaceStorageHttpResponse(IRestResponse response)
            {
                this.internalResponse = response;
            }
            public string Content { get { return internalResponse.Content; } }

            public HttpStatusCode StatusCode { get { return internalResponse.StatusCode; } }
        }

        private sealed class TermsOfUseStatus
        {
            public string User_id;
            public bool Accepted;
        }

        private sealed class TermsOfUseResponse
        {
            public string Message;
            public string Timestamp;
            public TermsOfUseStatus Content;
            public bool Status;
        }
    }
}

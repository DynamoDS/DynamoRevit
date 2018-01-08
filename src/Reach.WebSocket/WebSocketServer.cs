using System;
using System.Net;
using System.Threading;
using System.Diagnostics;

using Dynamo.Models;
using System.ServiceModel.Channels;

using System.Text;
using Reach.Logging;

using System.IO;
using Dynamo.Configuration;
using vtortola.WebSockets;
using vtortola.WebSockets.Deflate;
using System.Threading.Tasks;

using System.Configuration;
using Dynamo.Storage.Conversion;
using Reach.Messages;
using Reach.Responses;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Reach.Conversion;
using Reach.Execution;

namespace Reach.WebSocket
{
    public class WebSocketServer : IDisposable
    {
        public class StartParams
        {
            public bool DisableTokenValidator { get; set; }
            public string TokenValidatorAddress { get; set; }
            public EventWaitHandle WaitHandle { get; set; }
            public Func<DynamoModel> DynamoModelSource { get; set; }
            public ILogger SessionLogger { get; set; }
        }

        #region Immutable fields

        private readonly ILogger logger;
        private readonly WebSocketListener server;
        private readonly CancellationTokenSource cancellationSource;
        private readonly Task listeningTask;
        private readonly IPEndPoint endpoint;
        private readonly ITokenValidator tokenValidator;
        private readonly JsonSerializerSettings jsonSettings;
        private readonly EventWaitHandle exitHandler;
        private readonly IAsyncMessageExecutive executive;

        #endregion

        #region Mutable fields

        private string userId = "";
        private string sessionId = "";
        private TokenValidation tokenValidation;
        private vtortola.WebSockets.WebSocket activeWebSocket;

        #endregion

        /// <summary>
        /// Creates and starts the WebSocketServer
        /// </summary>
        /// <param name="startParams"></param>
        public WebSocketServer(StartParams startParams)
        {
            logger = startParams.SessionLogger ?? new ConsoleLogger();

            logger.Log("Welcome to Reach.WebSocket!");

            logger.Log("DynamoCore.dll Version : " + typeof(DynamoModel).Assembly.GetName().Version);
            logger.Log("Reach.dll Version : " + typeof(MessageHandler).Assembly.GetName().Version);
            logger.Log("Reach.WebSocket.dll Version : " + typeof(WebSocketServer).Assembly.GetName().Version);

            exitHandler = startParams.WaitHandle;
            cancellationSource = new CancellationTokenSource();

            // The executive for this instance
            executive = new AsyncMessageExecutive(startParams.DynamoModelSource(), logger);

            // Register executive events
            executive.ResponseReady += SendResponse;
            executive.MessageConsumed += HandleMessageConsumed;
            executive.ExceptionRaised += HandleOnException;

            // When done with executive, clean up
            executive.Disposed += (_) =>
            {
                executive.ResponseReady -= SendResponse;
                executive.MessageConsumed -= HandleMessageConsumed;
                executive.ExceptionRaised -= HandleOnException;
            };

            // Describes how we will serialize Response data
            jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Binder = new TypeNameSerializationBinder("Reach.Responses.{0}, Reach"),
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Setup Http address and port
            string httpBindingAddress = ConfigurationManager.AppSettings["bindingAddress"] ?? "";
            string httpBindingport = ConfigurationManager.AppSettings["bindingPort"] ?? "";

            IPAddress ipAddress;
            int port;

            if (!IPAddress.TryParse(httpBindingAddress, out ipAddress))
            {
                logger.Log(String.Format("WebSocketServer: Can't parse binding address: {0}. Using default of 0.0.0.0", httpBindingAddress));
                ipAddress = IPAddress.Any;
            }

            if (!int.TryParse(httpBindingport, out port))
            {
                logger.Log(String.Format("WebSocketServer: Can't parse binding port: {0}. Using default of 2100", httpBindingport));
                port = 2100;
            }

            endpoint = new IPEndPoint(ipAddress, port);

            // Setup all default parameters for WebSocket transport
            const int mxNumberOfConnections = 2;
            const int sendBufferSize = 32*1024; // 32k

            server = new WebSocketListener(endpoint, new WebSocketListenerOptions()
            {
                PingTimeout = Timeout.InfiniteTimeSpan,
                SendBufferSize = sendBufferSize, // Large buffer size necessary for large geometry payloads

                WebSocketReceiveTimeout = TimeSpan.FromSeconds(120),
                WebSocketSendTimeout = TimeSpan.FromSeconds(120),

                // Recommended buffer manager size: sendBufferSize + 1k
                // It is recommended to add 5 extra connections in addition to the expected maximum connections 
                // in order to to ensure that there always are buffers available in reconnect scenarios.    
               BufferManager = BufferManager.CreateBufferManager((1024 + sendBufferSize) * (mxNumberOfConnections + 5), 1024 + sendBufferSize)
            });

            // Register the standard with the server
            var rfc6455 = new vtortola.WebSockets.Rfc6455.WebSocketFactoryRfc6455(server);

            // Enable compression
            rfc6455.MessageExtensions.RegisterExtension(new WebSocketDeflateExtension());
            server.Standards.RegisterStandard(rfc6455);

            // Define whether token validation will be used for this session
            if (!startParams.DisableTokenValidator)
            {
                string validatorAddress = !String.IsNullOrEmpty(startParams.TokenValidatorAddress) ?
                  startParams.TokenValidatorAddress :
                  ConfigurationManager.AppSettings["tokenValidatorAddress"];

                if (validatorAddress == null)
                {
                    throw new Exception("Token validation is enabled but you did not supply" +
                        " the validatorAddress in the config file or as a parameter to the CLI");
                }

                logger.Log("TokenValidatorAddress : " + validatorAddress);

                tokenValidator = new FloodTokenValidator(validatorAddress, logger);
            }
            else
            {
                logger.Log("Token validation will be disabled for this session");
                tokenValidator = new TolerantTokenValidator();
            }

            if (!DependentFileStorage.TryClear())
            {
                logger.Log("Can't clear dependent file storage!");
            }

            // Actually start the server

            logger.Log(String.Format("Attempting to start WebSocketServer at : {0}:{1}", endpoint.Address, endpoint.Port));

            try
            {
                server.Start();
                listeningTask = Task.Run(() => AcceptWebSocketClientsAsync(server, cancellationSource.Token));
                logger.Log("WebSocketServer: The server started successfully!");
            }
            catch (Exception ex)
            {
                logger.Log(ex, Environment.MachineName, endpoint.ToString());
                throw ex;
            }
        }

        async private Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var ws = await server.AcceptWebSocketAsync(token);

                    if (ws == null) continue;

                    // A simple request to find out if the server is healthy.
                    if (ws.HttpRequest.RequestUri.ToString() == "/health")
                    {
                        SendResponse(ws, new HealthResponse());
                        ws.Close();
                        continue;
                    }

                    // A request to exit for package refresh if not currently serving
                    if (ws.HttpRequest.RequestUri.ToString() == "/refresh")
                    {
                        logger.Log("Handling /refresh response");
                        SendResponse(ws, new RefreshResponse());
                        ws.Close();

                        // If this instance hasn't yet served a client, then exit.
                        // This is expected to be used with auto-exit.
                        if(tokenValidation == null)
                        {
                            break;
                        }

                        continue;
                    }

                    // If we get this far and currently have a ws client, we
                    // can't do anything with it.
                    if (activeWebSocket != null)
                    {
                        // Error Code 503 indicating server is currently busy.
                        SendResponse(ws, new Reach.Responses.ContentResponse("This instance is busy.", ResponseStatuses.ResetByPeer));
                        ws.Close();
                        continue;
                    }

                    var validation = tokenValidator.Validate(ws.HttpRequest.RequestUri);

                    if (!validation)
                    {
                        SendResponse(ws, validation.ToResponse());
                        ws.Close();
                        continue;
                    }

                    // Setup completed, store the websocket
                    activeWebSocket = ws;

                    tokenValidation = validation;
                    userId = Hash.GetHashId(tokenValidation.UserId);

                    logger.Log("Beginning new session with hashed user id : " + userId);

                    SendResponse(activeWebSocket, new NewConnectionResponse()
                    {
                        MachineName = Environment.MachineName,
                        DynamoVersion = typeof(DynamoModel).Assembly.GetName().Version.ToString()
                    });

                    try
                    {
                        executive.ExecuteMessage(new ClearWorkspaceMessage(), true);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex, Environment.MachineName, sessionId);
                    }

                    logger.Log("Connected user id : " + userId);

                    Task.Run(() => HandleConnectionAsync(activeWebSocket, token));
                }
                catch (Exception aex)
                {
                    logger.Log("Error Accepting clients: " + aex.GetBaseException().Message);
                    logger.Log(aex.ToString());
                }
            }

            logger.Log("Server has stopped accepting clients");

            if (exitHandler != null) exitHandler.Set();
        }

        async private Task HandleConnectionAsync(vtortola.WebSockets.WebSocket ws, CancellationToken cancellation)
        {
            try
            {
                while (ws.IsConnected && !cancellation.IsCancellationRequested)
                {
                    var messageReadStream = await ws.ReadMessageAsync(cancellation).ConfigureAwait(false);

                    if (!ws.IsConnected)
                    {
                        // Connection has closed
                        logger.Log("Connection closed by peer");
                        continue;
                    }

                    if (messageReadStream == null)
                    {
                        SendResponse(ws, new ContentResponse(DateTime.Now.ToShortDateString() + " Unrecognized message type received", ResponseStatuses.Error));
                        continue;
                    }

                    if (messageReadStream.MessageType == WebSocketMessageType.Text)
                    {
                        var msgContent = String.Empty;
                        using (var sr = new StreamReader(messageReadStream, Encoding.UTF8))
                        {
                            msgContent = await sr.ReadToEndAsync();

                            var msg = executive.ExecuteMessage(msgContent);
                            logger.Log("#" + msg.UniqueID + " received", sessionId, userId, msg.GetType().Name, msg.ToJsonString());
                        }
                    }
                    else if (messageReadStream.MessageType == WebSocketMessageType.Binary)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await messageReadStream.CopyToAsync(ms);

                            var msg = executive.ExecuteFile(ms.ToArray());
                            logger.Log((string)("#" + msg.UniqueID + " received"), sessionId, userId, "ExecuteFile");
                            SendResponse(ws, new ContentResponse(DateTime.Now.ToShortDateString() + " Binary message received", ResponseStatuses.Success, msg.UniqueID));
                        }
                    }
                    else
                    {
                        SendResponse(ws, new ContentResponse(DateTime.Now.ToShortDateString() + " Unrecognized message type received", ResponseStatuses.Error));
                    }
                }
            }
            catch (Exception aex)
            {
                logger.Log(aex.GetBaseException(), Environment.MachineName, endpoint.ToString());
                SendResponse(ws, new ContentResponse("Something went wrong. Details: " + aex.Message, ResponseStatuses.ResetByPeer));

                try { ws.Close(); }
                catch { }
            }
            finally
            {
                activeWebSocket = null;
                ws.Dispose();
                if (exitHandler != null) exitHandler.Set(); // destroys the session
            }
        }

        // This function sends the response to the active web socket.
        private void SendResponse(Response response)
        {
            SendResponse(activeWebSocket, response);
        }

        // This function sends the response to a specific web socket.
        private void SendResponse(vtortola.WebSockets.WebSocket ws, Response response)
        {
            var jsonContent = String.Empty;

            try
            {
                if (ws == null || !ws.IsConnected)
                {
                    // Writing to closed connection
                    return;
                }

                var serializeStopwatch = Stopwatch.StartNew();

                var json = JsonConvert.SerializeObject(response, jsonSettings);

                serializeStopwatch.Stop();

                var sendStopwatch = Stopwatch.StartNew();

                ws.WriteString(json);

                // Do not write HealhResponse messages to the log
                if (response is HealthResponse) {
                    return;
                }

                var responseName = response.GetType().Name;

                if (responseName != "GeometryDataResponse")
                {
                    jsonContent = response.ToJsonString();
                }

                logger.Log(
                    string.Format( "#{0}, SerializationElapsed: {1}ms, SendElapsed: {2}ms", response.UniqueID, serializeStopwatch.ElapsedMilliseconds, sendStopwatch.ElapsedMilliseconds)
                    , sessionId, userId, response.GetType().Name, jsonContent);
            }
            catch (Exception e)
            {
                logger.Log("#" + response.UniqueID + "was not sent", sessionId, userId, response.GetType().Name,
                       jsonContent);
                logger.Log(e, Environment.MachineName, endpoint.ToString());

                if ((e is WebSocketException) && !ws.IsConnected)
                {
                    // If closing the active websocket, clear it so another
                    // connection can be made
                    if (ws == activeWebSocket)
                    {
                        activeWebSocket = null;

                        if (exitHandler != null)
                        {
                            exitHandler.Set();
                        }
                    }

                    try { ws.Close(); }
                    catch { }
                }
            }

            if ((ws == activeWebSocket) && (response is TimeoutResponse) &&
                (exitHandler != null))
            {
                exitHandler.Set();
            }
        }

        private void HandleMessageConsumed(Reach.Messages.Message message)
        {
            if (message != null)
            {
                SendResponse(activeWebSocket, new MessageConsumedResponse(message.UniqueID));
            }
        }

        private void HandleOnException(Exception ex)
        {
            logger.Log(ex, Environment.MachineName, endpoint.ToString());
        }

        public void Dispose()
        {
            try
            {
                logger.Log("Beginning shutdown");

                executive.Dispose();

                cancellationSource.Cancel();
                listeningTask.Wait();
            }
            catch (Exception ex)
            {
                logger.Log(ex, Environment.MachineName, endpoint.ToString());
            }

            logger.Log("Shutdown completed");
        }

    }
}


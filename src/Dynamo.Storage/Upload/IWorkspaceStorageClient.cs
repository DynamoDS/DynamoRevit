using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamo.Graph.Workspaces;
using Dynamo.Storage.Data;

namespace Dynamo.Storage.Upload
{
    public interface IWorkspaceStorageClient
    {
        /// <summary>
        /// Send a workspace and custom node dependencies to the node.js server for storage in MongoDB
        /// </summary>
        /// <param name="workspace">Home workspce model</param>
        /// <param name="dependencies">List of home workspace dependencies</param>
        /// <param name="prop">Home workspace properties</param>
        /// <returns>Node.js server response with information about the success of the operation</returns>
        Task<IFloodHttpResponse> Send(HomeWorkspaceModel workspace, IEnumerable<CustomNodeWorkspaceModel> dependencies, WorkspaceProperties prop);

        /// <summary>
        /// Obtains all workspaces for the current user.
        /// </summary>
        Task<IEnumerable<Workspace>> GetWorkspaces();

        /// <summary>
        /// Specifies whether the client has accepted Terms of use or not.
        /// </summary>
        Task<bool> GetTermsOfUseAcceptanceStatus();

        /// <summary>
        /// Save Terms of use status to database 
        /// </summary>
        Task<bool> SetTermsOfUseAcceptanceStatus();

        /// <summary>
        /// Delete a Workspace.
        /// </summary>
        /// <param name="workspaceId">The id of the Workspace.</param>
        /// <returns></returns>
        Task<IFloodHttpResponse> DeleteWorkspaceAsync(string workspaceId);

        /// <summary>
        /// Upload a file dependency.
        /// </summary>
        /// <returns>True if the upload was successful otherwise false.</returns>
        /// <param name="parameters">A <see cref="FileDependencyParameters"/> object.</param>
        Task<IFloodHttpResponse> UploadFileDependencyAsync(FileDependencyParameters parameters);
    }
}

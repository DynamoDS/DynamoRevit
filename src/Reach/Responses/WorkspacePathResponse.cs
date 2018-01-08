namespace Reach.Responses
{
    public class WorkspacePathResponse : Response
    {
        string guid;

        /// <summary>
        /// Guid of the specified workspace.
        /// Home workspace should have empty string as guid
        /// </summary>
        public string Guid
        {
            get { return guid; }
            private set { guid = value ?? string.Empty; }
        }

        /// <summary>
        /// Path where the workspace is saved
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The response which contains a path to a saved file
        /// and a corresponding workspace guid. This response is used only with NWK
        /// </summary>
        /// <param name="guid">Workspace guid</param>
        /// <param name="path">Path to the saved workspace</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"WorkspacePathResponse",
        ///     "guid":"00000000-0000-0000-0000-000000000000",
        ///     "path":"c:\\home.dyn",
        ///     "status":0
        /// }
        /// </code>
        public WorkspacePathResponse(string guid, string path)
        {
            Guid = guid;
            Path = path;
        }
    }
}

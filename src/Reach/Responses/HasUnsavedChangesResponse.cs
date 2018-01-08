using System;


namespace Reach.Responses
{
    public class HasUnsavedChangesResponse : Response
    {
        string guid;

        /// <summary>
        /// Guid of the specified workspace.
        /// Home workspace should have empty string as guid
        /// </summary>
        public string Guid
        {
            get { return guid; }
            private set
            {
                Guid guidValue;
                guid = System.Guid.TryParse(value, out guidValue) ? value : string.Empty;
            }
        }

        /// <summary>
        /// Bool value which represents if a specified workspace has unsaved changes
        /// </summary>
        public bool HasUnsavedChanges { get; private set; }

        /// <summary>
        /// The response with an information about the presence of unsaved workspace changes
        /// </summary>
        /// <param name="guid">Workspace guid</param>
        /// <param name="hasUnsavedChanges">Unsaved changes flag</param>
        /// <example>
        /// This sample shows
        /// </example>
        /// <code>
        /// {
        ///     "$type":"HasUnsavedChangesResponse",
        ///     "guid":"00000000-0000-0000-0000-000000000000",
        ///     "hasUnsavedChanges":true,
        ///     "status":0
        /// }
        /// </code>
        public HasUnsavedChangesResponse(string guid, bool hasUnsavedChanges)
        {
            Guid = guid;
            HasUnsavedChanges = hasUnsavedChanges;
        }
    }
}

using Newtonsoft.Json;

namespace DADynamoApp
{
    /// <summary>
    /// Configuration details for a cloud model.
    /// </summary>
    internal class OpenCloudModelLocation
    {
        /// <summary>
        /// Represents the region where the cloud model is hosted.
        /// </summary>
        public string Region;

        /// <summary>
        /// Represents the unique identifier for a project.
        /// </summary>
        public Guid ProjectGuid;

        /// <summary>
        /// Represents the unique identifier for the model.
        /// </summary>
        public Guid ModelGuid;
    }

    /// <summary>
    /// Represents the save location for the current DA work model.
    /// </summary>
    internal class SaveCloudModelLocation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the account.
        /// </summary>
        public Guid AccountId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the project.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the folder.
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// Gets or sets the name of the model to be created on the save operation.
        /// </summary>
        public string ModelName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request body used to setup aspects of the Design Automation environment.
    /// </summary>
    internal class SetupDARequest
    {
        //
        // Summary:
        // Save the revit document to the default result.rvt file.
        [JsonProperty(nameof(GenerateOutputModel), Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool GenerateOutputModel { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of the Revit model file.
        /// </summary>
        public string LocalModelFileName { get; set; } = string.Empty;

        public OpenCloudModelLocation? OpenCloudModelLocation { get; set; } = null;

        public SaveCloudModelLocation? SaveCloudModelLocation { get; set; } = null;
    }
}

using Newtonsoft.Json;

namespace DADynamoApp
{
    /// <summary>
    /// Configuration details for a cloud model.
    /// </summary>
    internal class CloudModelConfig
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

    internal class NewCloudModelLocation
    {
        public Guid AccountId { get; set; }
        public Guid ProjectId { get; set; }
        public string FolderId { get; set; }
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
        [JsonProperty(nameof(SaveRevitFile), Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool SaveRevitFile { get; set; } = false;

        public NewCloudModelLocation? SaveToNewCloudLocation { get; set; } = null;

        /// <summary>
        /// Gets or sets the configuration for the cloud model.
        /// </summary>
        [JsonProperty(nameof(CloudModel), Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public CloudModelConfig? CloudModel { get; set; } = null;
    }
}

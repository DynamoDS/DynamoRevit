using Newtonsoft.Json;

namespace DADynamoApp
{
    /// <summary>
    /// Request body used to setup aspects of the Design Automation environment.
    /// </summary>
    internal class SetupDARequest
    {
        //
        // Summary:
        // Save the revit document to the default result.rvt file.
        [JsonProperty("saveRvt", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool SaveRvt { get; set; } = false;
    }
}

using DynamoPlayer;
using Newtonsoft.Json;

namespace DADynamoApp
{
    internal class SetupDARequest
    {
        //
        // Summary:
        // Save the revit document to the default result.rvt file.
        [JsonProperty("saveRvt", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool SaveRvt { get; set; } = false;
    }

    internal class SetupDAController(IDynamoController implementation) : DynamoController(implementation)
    {
        internal SetupDARequest? setupDA;

        [HttpPost]
        [Route("setup", Name = "post-setup")]
        public Task<SetupDARequest> Setup([FromBody] SetupDARequest req)
        {
            return Task.FromResult(req);
        }
    }
}

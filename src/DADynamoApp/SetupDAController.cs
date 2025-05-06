using DynamoPlayer;
using Newtonsoft.Json;

namespace DADynamoApp
{
    public class SetupDARequest
    {
        //
        // Summary:
        // Save the revit document to the default result.rvt file.
        [JsonProperty("saveRvt", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? SaveRvt { get; set; } = false;
    }

    internal class SetupDAController(IDynamoController implementation) : DynamoController(implementation)
    {
        internal SetupDARequest? setupDA;

        [HttpPost]
        [Route("setup", Name = "post-setup")]
        public Task Setup([FromBody] SetupDARequest req)
        {
            setupDA = req;
            return Task.CompletedTask;
        }
    }
}

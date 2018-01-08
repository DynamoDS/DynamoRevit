using Reach.Messages.Data;


namespace Reach.Responses
{
    class GeometryDataResponse : Response
    {
        /// <summary>
        /// Contains Guid of the specified node and a list of graphic primitives
        /// that the result object consists of.
        /// </summary>
        public GeometryData GeometryData { get; private set; }

        /// <summary>
        /// Response contains all computed node geometry
        /// </summary>
        /// <param name="geometryData">Binary encoded geometry data</param>
        /// <example>
        /// This sample shows Json structure
        /// </example>
        /// <code>
        /// {
        ///     "$type":"GeometryDataResponse",
        ///     "geometryData":{
        ///         "$type":"DynamoWebServer.Messages.GeometryData",
        ///         "nodeId":"61e47bad-8d44-6d3e-0b81-557ca672c880",
        ///         "graphicPrimitivesData":{
        ///             "$type":"DynamoWebServer.Messages.GeometryData+GraphicPrimitives",
        ///             "pointVertices":"AACAPwAAgD8AAAAAAAAAQAAAAEAAAAAAAABAQAAAQEAAAAAAAACAQAAAQEAAAAAAAACgQAAAQEAAAAAA",
        ///             "lineStripVertices":"",
        ///             "triangleVertices":"",
        ///             "triangleNormals":"",
        ///             "lineStripCounts":"",
        ///             "lineStripColors":""
        ///         }
        ///     },
        ///     "status":0
        /// }
        /// </code>
        public GeometryDataResponse(GeometryData geometryData)
        {
            GeometryData = geometryData;
        }
    }
}

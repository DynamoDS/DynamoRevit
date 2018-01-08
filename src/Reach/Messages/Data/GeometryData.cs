using Autodesk.DesignScript.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Reach.Messages.Data
{
    public class GeometryData
    {
        /// <summary>
        /// Guid of the specified node
        /// </summary>
        [DataMember]
        public string NodeId { get; private set; }

        /// <summary>
        /// List of the graphic primitives that result object consist of.
        /// It is empty for nongraphic objects
        /// </summary>
        [DataMember]
        public IEnumerable<GraphicPrimitives> GraphicPrimitivesData { get; private set; }

        [JsonConstructor]
        public GeometryData(string NodeId, IEnumerable<GraphicPrimitives> GraphicPrimitivesData)
        {
            this.NodeId = NodeId;
            this.GraphicPrimitivesData = GraphicPrimitivesData;
        }

        public GeometryData(string id, IEnumerable<IRenderPackage> packages)
        {
            this.NodeId = id;
            GeneratePrimitives(packages);
        }

        private void GeneratePrimitives(IEnumerable<IRenderPackage> packages)
        {
            if (packages == null || !packages.Any())
                return;

            var data = new List<GraphicPrimitives>();

            foreach (var package in packages)
            {
                string textureCoordinates = String.Empty,
                       colorsStride = String.Empty,
                       colors = String.Empty;

                if (package.Colors != null)
                {
                    colors = EncodeNumbers(package.Colors);
                    textureCoordinates = EncodeNumbers(package.MeshTextureCoordinates);
                    colorsStride = package.ColorsStride.ToString();
                }

                string pointVertices = EncodeNumbers(package.PointVertices);
                string pointVertexColors = EncodeNumbers(package.PointVertexColors);

                string meshVertices = EncodeNumbers(package.MeshVertices);
                string meshNormals = EncodeNumbers(package.MeshNormals);
                string meshVertexColors = EncodeNumbers(package.MeshVertexColors);

                string lineStripVertices = EncodeNumbers(package.LineStripVertices);
                string lineStripCounts = EncodeNumbers(package.LineStripVertexCounts);
                string lineStripColors = EncodeNumbers(package.LineStripVertexColors);

                data.Add(new GraphicPrimitives()
                {
                    Colors = colors,
                    TextureCoordinates = textureCoordinates,
                    ColorsStride = colorsStride,
                    PointVertexColors = pointVertexColors,
                    PointVertices = pointVertices,
                    MeshNormals = meshNormals,
                    MeshVertexColors = meshVertexColors,
                    MeshVertices = meshVertices,
                    LineStripColors = lineStripColors,
                    LineStripCounts = lineStripCounts,
                    LineStripVertices = lineStripVertices,
                    RequiresPerVertexColoration = package.RequiresPerVertexColoration,
                    RequiresTextureMapping = !String.IsNullOrEmpty(colors)
                });
            }

            this.GraphicPrimitivesData = data;
        }

        private static string EncodeNumbers<T>(IEnumerable<T> coordinates)
        {
            var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                if (typeof(T) == typeof(double))
                {
                    foreach (T value in coordinates)
                        writer.Write(Convert.ToSingle(value));
                }
                else
                {
                    foreach (T value in coordinates)
                        writer.Write(value as dynamic);
                }
            }

            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        /// The class that represents data for drawing a graphic primitive 
        /// </summary>
        public class GraphicPrimitives
        {
            [DataMember]
            public string Colors { get; set; }

            [DataMember]
            public string TextureCoordinates { get; set; }

            [DataMember]
            public string ColorsStride { get; set; }

            [DataMember]
            public string PointVertices { get; set; }

            [DataMember]
            public string PointVertexColors { get; set; }

            [DataMember]
            public string MeshVertices { get; set; }

            [DataMember]
            public string MeshNormals { get; set; }

            [DataMember]
            public string MeshVertexColors { get; set; }

            [DataMember]
            public string LineStripVertices { get; set; }

            [DataMember]
            public string LineStripCounts { get; set; }

            [DataMember]
            public string LineStripColors { get; set; }

            [DataMember]
            public bool RequiresPerVertexColoration { get; set; }

            [DataMember]
            public bool RequiresTextureMapping { get; set; }
        }

    }
}

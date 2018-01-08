using System.Globalization;
using System.Xml;
using DynamoUtilities;
using Newtonsoft.Json;
using Watch3DNodeModels;

namespace Dynamo.Storage.Data
{
    public class CameraData
    {
        private const string DefaultCameraName = "Background Preview";

        public string Name { get; set; }

        public double EyeX { get; set; }

        public double EyeY { get; set; }

        public double EyeZ { get; set; }

        public double LookX { get; set; }

        public double LookY { get; set; }

        public double LookZ { get; set; }

        [JsonProperty("upX")]
        public double UpX { get; set; }

        [JsonProperty("upY")]
        public double UpY { get; set; }

        [JsonProperty("upZ")]
        public double UpZ { get; set; }

        /// <summary>
        /// Default constructor. Creates default camera.
        /// </summary>
        public CameraData()
        {
            Name = DefaultCameraName;
            EyeX = -17;
            EyeY = 24;
            EyeZ = 50;
            LookX = 12;
            LookY = -13;
            LookZ = -58;
            UpX = 0;
            UpY = 1;
            UpZ = 0;
        }

        /// <summary>
        /// Creates camera based on Watch3DCamera.
        /// </summary>
        /// <param name="camera">Watch3DCamera</param>
        public CameraData(Watch3DCamera camera)
        {
            Name = camera.Name;
            EyeX = camera.EyeX;
            EyeY = camera.EyeY;
            EyeZ = camera.EyeZ;
            LookX = camera.LookX;
            LookY = camera.LookY;
            LookZ = camera.LookZ;
            UpX = camera.UpX;
            UpY = camera.UpY;
            UpZ = camera.UpZ;
        }

        public void SerializeCamera(XmlNode camerasElement)
        {
            try
            {
                var node = XmlHelper.AddNode(camerasElement, "Camera");
                XmlHelper.AddAttribute(node, "Name", Name ?? DefaultCameraName);
                AddAttribute(node, "eyeX", EyeX);
                AddAttribute(node, "eyeY", EyeY);
                AddAttribute(node, "eyeZ", EyeZ);
                AddAttribute(node, "lookX", LookX);
                AddAttribute(node, "lookY", LookY);
                AddAttribute(node, "lookZ", LookZ);
                AddAttribute(node, "upX", UpX);
                AddAttribute(node, "upY", UpY);
                AddAttribute(node, "upZ", UpZ);
                camerasElement.AppendChild(node);
            }
            catch { }
        }

        public static CameraData DeserializeCamera(XmlNode node)
        {
            if (node.Attributes == null || node.Attributes.Count == 0) return new CameraData();

            try
            {
                return new CameraData
                {
                    Name = node.Attributes["Name"].Value,
                    EyeX = GetValue(node, "eyeX"),
                    EyeY = GetValue(node, "eyeY"),
                    EyeZ = GetValue(node, "eyeZ"),
                    LookX = GetValue(node, "lookX"),
                    LookY = GetValue(node, "lookY"),
                    LookZ = GetValue(node, "lookZ"),
                    UpX = GetValue(node, "upX"),
                    UpY = GetValue(node, "upY"),
                    UpZ = GetValue(node, "upZ")
                };
            }
            catch
            {
                return new CameraData();
            }
        }

        private static float GetValue(XmlNode node, string attr)
        {
            return float.Parse(node.Attributes[attr].Value, CultureInfo.InvariantCulture);
        }

        private static void AddAttribute(XmlNode node, string name, double value)
        {
            XmlHelper.AddAttribute(node, name, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}

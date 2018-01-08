using Dynamo.Storage.Data;
using Watch3DNodeModels;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class Watch3DExtra
    {
        public CameraData CameraData { get; private set; }

        public Watch3DExtra(Watch3D nodeModel)
        {
            CameraData = nodeModel == null ? new CameraData() : new CameraData(nodeModel.Camera);
        }
    }
}

using CoreNodeModels;
using Dynamo.Graph.Nodes;

namespace Dynamo.Storage.Conversion.ExtraInfo
{
    public class WatchExtra: BaseExtraInfo
    {
        public object Value { get; private set; }

        public WatchExtra(NodeModel nodeModel)
        {
            var CachedValue = ((Watch)nodeModel).CachedValue;

            if (CachedValue != null)
            {
                if (CachedValue.GetType() != typeof(double) &&
                    CachedValue.GetType() != typeof(int) &&
                    CachedValue.GetType() != typeof(System.Int64) &&
                    CachedValue.GetType() != typeof(bool) &&
                    CachedValue.GetType() != typeof(string))
                {
                    Value = CachedValue.ToString();
                }
                else
                {
                    Value = CachedValue;
                }
            }
        }
    }
}

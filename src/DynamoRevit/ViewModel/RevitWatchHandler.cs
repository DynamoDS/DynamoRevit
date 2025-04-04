using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Dynamo.Interfaces;
using Dynamo.ViewModels;
using ProtoCore.Mirror;
using Revit.GeometryConversion;
using RevitServices.Persistence;
using Element = Revit.Elements.Element;

namespace Dynamo.Applications
{
    /// <summary>
    ///     An Revit-specific implementation of IWatchHandler that is set on the DynamoViewModel at startup.
    ///     The main GenerateWatchViewModelForData method dynamically dispatches to the appropriate
    ///     internal method based on the type. For every time for which you would like
    ///     to have a custom representation in the watch, you will need an additional
    ///     method on this handler
    /// </summary>
    public class RevitWatchHandler : IWatchHandler
    {
        private readonly IWatchHandler baseHandler;
        private readonly IPreferences preferences;

        public RevitWatchHandler(IPreferences prefs)
        {
            baseHandler = new DefaultWatchHandler(prefs);
            preferences = prefs;
        }

    // helper for zooming to clicked green Id
    internal static void ZoomToLinkedElement(Element element)
    {

      UIDocument uiDoc = DocumentManager.Instance.CurrentUIDocument;
      Autodesk.Revit.DB.View activeView = uiDoc.ActiveView;
      // get active UI view to use
      UIView uiview = uiDoc.GetOpenUIViews().FirstOrDefault<UIView>(uv => uv.ViewId.Equals(activeView.Id));
      Transform linkTransform = Revit.Elements.LinkElement.LinkTransform(element).ToTransform();

      // use the center of the BoundingBox as zoom center
      BoundingBoxXYZ bb = element.InternalElement.get_BoundingBox(null);
      // if the BBox cannot be found, attempt to find it using the active view
      if (bb == null)
      {
        bb = element.InternalElement.get_BoundingBox(activeView);
      }
      // finally, if the BB cannot be found at all
      if (bb == null)
      {
        TaskDialog.Show("Revit", "No good view can be found.");
        return;
      }
      XYZ bbCenter = (bb.Max + bb.Min) / 2;
      double zoomOffsetX = bb.Max.X - bbCenter.X;
      double zoomOffsetY = bb.Max.Y - bbCenter.Y;
      double zoomOffsetZ = bb.Max.Z - bbCenter.Z;
      XYZ locationPt = Revit.Elements.LinkElement.TransformPoint(bbCenter, linkTransform);

      if (locationPt != null)
      {
        XYZ min = new XYZ(locationPt.X - zoomOffsetX, locationPt.Y - zoomOffsetY, locationPt.Z - zoomOffsetZ);
        XYZ max = new XYZ(locationPt.X + zoomOffsetX, locationPt.Y + zoomOffsetY, locationPt.Z + zoomOffsetZ);
        uiview.ZoomAndCenterRectangle(min, max);
      }
    }

    private WatchViewModel ProcessThing(Element element, List<string> preferredDictionaryOrdering, ProtoCore.RuntimeCore runtimeCore, string tag, bool showRawData, WatchHandlerCallback callback)
        {
            var id = element.Id;

            var node = new WatchViewModel(element.ToString(preferences.NumberFormat, CultureInfo.InvariantCulture), tag, RequestSelectGeometry);

            node.Clicked += () =>
            {
                if (element.InternalElement.IsValidObject)
                {
                    Document elementDoc = element.InternalElement.Document;
                    Document currrentDoc = Revit.Application.Document.Current.InternalDocument;
                    if (!(elementDoc.Equals(currrentDoc)))
                    {
                        ZoomToLinkedElement(element);
                    }
                    else
                    { 
                        DocumentManager.Instance.CurrentUIDocument.ShowElements(element.InternalElement);
                    }
                }    
            };

            node.Link = id.ToString(CultureInfo.InvariantCulture);

            return node;
        }

        //If no dispatch target is found, then invoke base watch handler.
        private WatchViewModel ProcessThing(object obj, List<string> preferredDictionaryOrdering, ProtoCore.RuntimeCore runtimeCore, string tag, bool showRawData, WatchHandlerCallback callback)
        {
            return baseHandler.Process(obj, preferredDictionaryOrdering, runtimeCore, tag, showRawData, callback);
        }

        private WatchViewModel ProcessThing(MirrorData data, List<string> preferredDictionaryOrdering, ProtoCore.RuntimeCore runtimeCore, string tag, bool showRawData, WatchHandlerCallback callback)
        {
            try
            {
                return baseHandler.Process(data, preferredDictionaryOrdering, runtimeCore, tag, showRawData, callback);
            }
            catch (Exception)
            {
                return callback(data.Data, preferredDictionaryOrdering, runtimeCore, tag, showRawData);
            }
        }

        public WatchViewModel Process(dynamic value, IEnumerable<string> preferredDictionaryOrdering, ProtoCore.RuntimeCore runtimeCore, string tag, bool showRawData, WatchHandlerCallback callback)
        {
            return Object.ReferenceEquals(value, null)
                ? new WatchViewModel("null", tag, RequestSelectGeometry)
                : ProcessThing(value, preferredDictionaryOrdering?.ToList(), runtimeCore, tag, showRawData, callback);
        }


        public event Action<string> RequestSelectGeometry;
    }
}

using System;

using Autodesk.Revit.DB;

using Dynamo.Applications.Models;
using Dynamo.Applications.Properties;
using Dynamo.Interfaces;
using Dynamo.ViewModels;
using Dynamo.Wpf.ViewModels.Core;

using RevitServices.Persistence;

namespace Dynamo.Applications.ViewModel
{
    public class DynamoRevitViewModel : DynamoViewModel
    {
        private DynamoRevitViewModel(StartConfiguration startConfiguration) :
            base(startConfiguration)
        {
            var model = (RevitDynamoModel)Model;
            model.RevitDocumentChanged += model_RevitDocumentChanged;
            model.RevitContextAvailable += model_RevitContextAvailable;
            model.RevitContextUnavailable += model_RevitContextUnavailable;
            model.RevitDocumentLost += model_RevitDocumentLost;
            model.RevitViewChanged += model_RevitViewChanged;
            model.InvalidRevitDocumentActivated += model_InvalidRevitDocumentActivated;
        }

        public static DynamoRevitViewModel Start(StartConfiguration startConfiguration)
        {
            if (startConfiguration.DynamoModel == null)
            {
                startConfiguration.DynamoModel = RevitDynamoModel.Start();
            }
            else
            {
                if (startConfiguration.DynamoModel.GetType() != typeof(RevitDynamoModel))
                    throw new Exception("An instance of RevitDynamoViewModel is required to construct a DynamoRevitViewModel.");
            }

            if (startConfiguration.VisualizationManager == null)
                startConfiguration.VisualizationManager = new VisualizationManager(startConfiguration.DynamoModel);

            if (startConfiguration.WatchHandler == null)
                startConfiguration.WatchHandler = new DefaultWatchHandler(startConfiguration.VisualizationManager,
                    startConfiguration.DynamoModel.PreferenceSettings);

            return new DynamoRevitViewModel(startConfiguration);
        }

        void model_InvalidRevitDocumentActivated()
        {
            var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
            hsvm.CurrentNotificationLevel = NotificationLevel.Error;
            hsvm.CurrentNotificationMessage = Resources.DocumentPointingWarning;
        }

        void model_RevitViewChanged(View view)
        {
            var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
            hsvm.CurrentNotificationLevel = NotificationLevel.Moderate;
            hsvm.CurrentNotificationMessage =
                String.Format(Resources.ActiveViewWarning, view.Name);
        }

        void model_RevitDocumentLost()
        {
            var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
            hsvm.CurrentNotificationLevel = NotificationLevel.Error;
            hsvm.CurrentNotificationMessage = Resources.DocumentLostWarning;
        }

        void model_RevitContextUnavailable()
        {
            var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
            hsvm.CurrentNotificationLevel = NotificationLevel.Error;
            hsvm.CurrentNotificationMessage = Resources.RevitInvalidContextWarning;
        }

        void model_RevitContextAvailable()
        {
            var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
            hsvm.CurrentNotificationLevel = NotificationLevel.Moderate;
            hsvm.CurrentNotificationMessage = Resources.RevitValidContextMessage;
        }

        void model_RevitDocumentChanged(object sender, EventArgs e)
        {
            var hsvm = (HomeWorkspaceViewModel)HomeSpaceViewModel;
            hsvm.CurrentNotificationLevel = NotificationLevel.Moderate;
            hsvm.CurrentNotificationMessage =
                String.Format(GetDocumentPointerMessage());
        }

        private static string GetDocumentPointerMessage()
        {
            var docPath = DocumentManager.Instance.CurrentUIDocument.Document.PathName;
            var message = String.IsNullOrEmpty(docPath)
                ? "a new document."
                : String.Format("document: {0}", docPath);
            return String.Format(Resources.DocumentPointerMessage, message);
        }

        protected override void UnsubscribeAllEvents()
        {
            var model = (RevitDynamoModel)Model;
            model.RevitDocumentChanged -= model_RevitDocumentChanged;
            model.RevitContextAvailable -= model_RevitContextAvailable;
            model.RevitContextUnavailable -= model_RevitContextUnavailable;
            model.RevitDocumentLost -= model_RevitDocumentLost;
            model.RevitViewChanged -= model_RevitViewChanged;
            model.InvalidRevitDocumentActivated -= model_InvalidRevitDocumentActivated;

            base.UnsubscribeAllEvents();
        }
    }
}

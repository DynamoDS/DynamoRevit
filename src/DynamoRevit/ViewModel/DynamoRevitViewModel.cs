using System;

using Autodesk.Revit.DB;

using Dynamo.Applications.Models;
using Dynamo.Applications.Properties;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Interfaces;
using Dynamo.Wpf.ViewModels.Core;

using RevitServices.Persistence;

namespace Dynamo.Applications.ViewModel
{
    public class DynamoRevitViewModel : DynamoViewModel
    {
        protected DynamoRevitViewModel(
            DynamoModel dynamoModel,
            IWatchHandler watchHandler,
            IVisualizationManager vizManager,
            string commandFilePath,
            IBrandingResourceProvider resourceProvider,
            bool showLogin = false) :
                base(
                dynamoModel,
                watchHandler,
                vizManager,
                commandFilePath,
                resourceProvider,
                showLogin)
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
            var model = startConfiguration.DynamoModel ?? DynamoModel.Start();
            var vizManager = startConfiguration.VisualizationManager ?? new VisualizationManager(model);
            var watchHandler = startConfiguration.WatchHandler ?? new DefaultWatchHandler(vizManager,
                model.PreferenceSettings);

            return new DynamoRevitViewModel(model, watchHandler, vizManager, startConfiguration.CommandFilePath, startConfiguration.BrandingResourceProvider,
                startConfiguration.ShowLogin);
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

﻿using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Dynamo.Scheduler;

namespace RevitServices.Threading
{
    public class RevitSchedulerThread : ISchedulerThread
    {
        private bool shutdownRequested;
        private IScheduler scheduler;
        private readonly UIApplication revitApplication;

        public RevitSchedulerThread(UIApplication revitApplication)
        {
            this.revitApplication = revitApplication;
        }

        public void Initialize(IScheduler owningScheduler)
        {
            scheduler = owningScheduler;
            revitApplication.Idling += OnRevitIdle; // Register idle handler.
        }

        public void Shutdown()
        {
            shutdownRequested = true;
        }

        /// <summary>
        /// When Revit goes idle it gets here and process all tasks in the scheduler 
        /// queue. The control returns to Revit only when all tasks are processed. 
        /// This method will be called again the next time Revit goes into idle state.
        /// </summary>
        /// <param name="sender">Reference to the current Revit application.</param>
        /// <param name="e">Idling event argument.</param>
        /// 
        private void OnRevitIdle(object sender, IdlingEventArgs e)
        {
            if (shutdownRequested)
            {
                revitApplication.Idling -= OnRevitIdle; // Stop getting called.
                return;
            }

            // Don't run the graph if no active Revit doucment.
            if (revitApplication.ActiveUIDocument == null)
                return;

            const bool waitIfTaskQueueIsEmpty = false;
            while (scheduler.ProcessNextTask(waitIfTaskQueueIsEmpty))
            {
                // Does nothing here, loop ends when all tasks processed.
            }
        }
    }
}

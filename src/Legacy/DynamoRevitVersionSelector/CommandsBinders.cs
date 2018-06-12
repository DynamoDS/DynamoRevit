using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Linq;
using DynamoRevitVersionSelector.Properties;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using Autodesk.Revit.UI.Events;

namespace Dynamo.Applications
{
    /// <summary>
    /// Bind/Unbind Ribbons Icon actions
    /// </summary>
    internal class CommandsBinders
    {
        #region Statics
        /// <summary>
        /// Dynamo Command ID
        /// </summary>
        public static readonly string DYNAMOID = "ID_VISUAL_PROGRAMMING_DYNAMO";

        /// <summary>
        /// Dynamo Player Command ID
        /// </summary>
        public static readonly string PLAYERID = "ID_PLAYLIST_DYNAMO";
        #endregion

        public event EventHandler<ExecutedEventArgs> OnDynamoClick;
        public event EventHandler<ExecutedEventArgs> OnDynamoPlayerClick;

        private UIControlledApplication uiControlledApplication;
        private RevitCommandId DynamoCmdId;
        private AddInCommandBinding DynamoCommand = null;
        private RevitCommandId DynamoPlayerCmdId;
        private AddInCommandBinding DynamoPlayerCommand = null;

        private bool BindDynamoPlayer;

        /// <summary>
        /// Is Binding has been done
        /// </summary>
        public bool IsBinded { get; private set; }

        /// <summary>
        /// Constructor, Class to Bind/Unbind Ribbons Icon actions
        /// </summary>
        /// <param name="application"></param>
        /// <param name="bindDynamoPlayer"></param>
        public CommandsBinders(UIControlledApplication application, bool bindDynamoPlayer)
        {
            uiControlledApplication = application;
            BindDynamoPlayer = bindDynamoPlayer;
            DynamoCmdId = RevitCommandId.LookupCommandId(DYNAMOID);
            DynamoPlayerCmdId = RevitCommandId.LookupCommandId(PLAYERID);
            if (uiControlledApplication == null || DynamoCmdId == null || DynamoPlayerCmdId == null)
                throw new ArgumentNullException();
            IsBinded = false;
        }

        /// <summary>
        /// Bind Ribbons Icon actions
        /// </summary>
        public void Bind()
        {
            if (IsBinded) return;
            if (null == DynamoCommand)
            {
                DynamoCommand = uiControlledApplication.CreateAddInCommandBinding(DynamoCmdId);
                DynamoCommand.CanExecute += canExecute;
                DynamoCommand.BeforeExecuted += beforeExecuted;
                DynamoCommand.Executed += executedDynamo;
            }
            if (null == DynamoPlayerCommand && BindDynamoPlayer)
            {
                DynamoPlayerCommand = uiControlledApplication.CreateAddInCommandBinding(DynamoPlayerCmdId);
                DynamoPlayerCommand.CanExecute += canExecute;
                DynamoPlayerCommand.BeforeExecuted += beforeExecuted;
                DynamoPlayerCommand.Executed += executedPlayer;
            }
            IsBinded = true;
        }

        /// <summary>
        /// UnBind Ribbons Icon actions
        /// </summary>
        public void UnBind()
        {
            if (!IsBinded) return;
            if (null != DynamoCommand)
            {
                uiControlledApplication.RemoveAddInCommandBinding(DynamoCmdId);
                DynamoCommand.BeforeExecuted -= beforeExecuted;
                DynamoCommand.CanExecute -= canExecute;
                DynamoCommand.Executed -= executedDynamo;
                DynamoCommand = null;
            }
            if (null != DynamoPlayerCommand)
            {
                uiControlledApplication.RemoveAddInCommandBinding(DynamoPlayerCmdId);
                DynamoPlayerCommand.BeforeExecuted -= beforeExecuted;
                DynamoPlayerCommand.CanExecute -= canExecute;
                DynamoPlayerCommand.Executed -= executedPlayer;
                DynamoPlayerCommand = null;
            }
            IsBinded = false;
        }

        private void beforeExecuted(object sender, BeforeExecutedEventArgs e)
        {
            e.UsingCommandData = true;
        }

        private void canExecute(object sender, CanExecuteEventArgs e)
        {
            e.CanExecute = e.ActiveDocument != null;
        }

        private void executedDynamo(object sender, ExecutedEventArgs e)
        {
            OnDynamoClick(sender, e);
        }

        private void executedPlayer(object sender, ExecutedEventArgs e)
        {
            OnDynamoPlayerClick(sender, e);
        }
    }
}

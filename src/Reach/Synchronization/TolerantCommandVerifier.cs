using System;
using System.Collections.Generic;

using Dynamo.Models;

namespace Reach.Synchronization
{
    internal class TolerantCommandVerifier : ICommandVerifier 
    {
        #region Private members

        private readonly DynamoModel dynamoModel;

        #endregion

        #region ICommandVerifier members

        public IEnumerable<string> InfoMessages { get { return null; } }

        public bool ExecuteCommands(DynamoModel.MakeConnectionCommand firstCommand,
            DynamoModel.MakeConnectionCommand secondCommand)
        {
            try
            {
                dynamoModel.ExecuteCommand(firstCommand);
                dynamoModel.ExecuteCommand(secondCommand);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool ExecuteCommand(DynamoModel.RecordableCommand command)
        {
            try
            {
                dynamoModel.ExecuteCommand(command);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion

        internal TolerantCommandVerifier(DynamoModel dynamoModel)
        {
            this.dynamoModel = dynamoModel;
        }
    }
}

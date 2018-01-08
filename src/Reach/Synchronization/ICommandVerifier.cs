using Dynamo.Models;

namespace Reach.Synchronization
{
    /// <summary>
    /// The contract for a class which executes commands.
    /// </summary>
    public interface ICommandVerifier
    {
        /// <summary>
        /// Execute a command if it is not disabled.
        /// </summary>
        /// <param name="command">The RecordableCommand to check and execute</param>
        /// <returns>Returns true if execution of commands was started,
        /// and false, if commands were rejected.</returns>
        bool ExecuteCommand(DynamoModel.RecordableCommand command);

        /// <summary>
        /// Executes MakeConnectionCommands if it is not disabled.
        /// </summary>
        /// <param name="command">MakeConnectionCommand command to check and execute</param>
        /// <param name="secondCommand">MakeConnectionCommand to check and execute</param>
        /// <returns>Returns true if execution of commands was started,
        /// and false, if commands were rejected.</returns>
        bool ExecuteCommands(DynamoModel.MakeConnectionCommand firstCommand,
            DynamoModel.MakeConnectionCommand secondCommand);
    }
}

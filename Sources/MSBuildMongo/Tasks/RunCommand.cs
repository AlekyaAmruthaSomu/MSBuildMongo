using System;
using Microsoft.Build.Framework;
using MongoDB.Driver;

namespace MongoDB.MSBuild.Tasks
{
    public class RunCommand : MongoTaskBase
    {
        [Required]
        public string Command { get; set; }

        public override bool Execute()
        {
            CommandResult commandResult;

            try
            {
                this.Log.LogMessage("Executing command: " + this.Command);
                commandResult = this.Database.RunCommand(this.Command);
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
            
            this.Log.LogMessage(string.Format("Command returns: Response: '{0}' ErrorMessage: '{1}'", commandResult.Response, commandResult.ErrorMessage));
            return commandResult.Ok;
        }
    }
}

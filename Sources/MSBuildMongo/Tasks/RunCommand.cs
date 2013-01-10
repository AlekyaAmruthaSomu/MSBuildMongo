using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MongoDB.Driver;

namespace MSBuildMongo.Tasks
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
                System.Diagnostics.Debugger.Launch();
                Log.LogMessage("Executing command: " + Command);
                commandResult = this.Database.RunCommand(this.Command);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            
            Log.LogMessage(string.Format("Command returns: Response: '{0}' ErrorMessage: '{1}'", commandResult.Response, commandResult.ErrorMessage));
            return commandResult.Ok;
        }
    }
}

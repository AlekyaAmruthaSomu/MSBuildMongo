using System;
using Microsoft.Build.Utilities;

namespace MSBuildMongo.Tasks
{
    public class DeployDatabase : Task
    {
        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Deploying database");
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            return true;
        }
    }
}
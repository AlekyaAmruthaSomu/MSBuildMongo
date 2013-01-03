using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBulidMongo.Tasks
{
    public class DeployDatabase : ITask
    {
        private TaskLoggingHelper log;

        private TaskLoggingHelper Log
        {
            get { return log ?? (log = new TaskLoggingHelper(this)); }
        }

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
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
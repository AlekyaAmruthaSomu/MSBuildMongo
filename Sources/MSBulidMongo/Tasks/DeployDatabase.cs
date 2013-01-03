using System;
using Microsoft.Build.Framework;

namespace MSBulidMongo.Tasks
{
    public class DeployDatabase : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
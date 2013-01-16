using System;
using Microsoft.Build.Framework;
using MongoDB.Driver;

namespace MongoDB.MSBuild.Tasks
{
    public class AddUser : MongoTaskBase
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool Admin { get; set; }

        public override bool Execute()
        {
            this.Log.LogMessage(string.Format("Adding user '{0}' into database '{1}'", this.UserName, this.Database));
            try
            {
                MongoUser user = this.GetMongoUser();
                this.Database.AddUser(user);
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }

            this.Log.LogMessage(string.Format("User '{0}' was successfully added", this.UserName));
            return true;
        }


        private MongoUser GetMongoUser()
        {
            var credentials = new MongoCredentials(this.UserName, this.Password, this.Admin);
            return new MongoUser(credentials, !credentials.Admin);
        }
    }
}
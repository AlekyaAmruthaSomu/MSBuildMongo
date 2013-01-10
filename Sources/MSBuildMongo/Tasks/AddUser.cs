using System;
using Microsoft.Build.Framework;
using MongoDB.Driver;

namespace MSBuildMongo.Tasks
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
            this.Log.LogMessage("Creating database: " + this.DatabaseName);
            try
            {
                var user = this.GetMongoUser();
                this.Database.AddUser(user);
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }

            this.Log.LogMessage("Database was created");
            return true;
        }


        private MongoUser GetMongoUser()
        {
            var credentials = new MongoCredentials(this.UserName, this.Password, this.Admin);
            return new MongoUser(credentials, !credentials.Admin);
        }
    }
}
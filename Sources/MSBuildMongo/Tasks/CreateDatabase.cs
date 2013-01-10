using System;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MongoDB.Driver;

namespace MSBuildMongo.Tasks
{
    public class CreateDatabase : Task
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string DatabaseName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool Admin { get; set; }

        public override bool Execute()
        {
            this.Log.LogMessage("Creating database: " + this.DatabaseName);
            try
            {   
                this.EnsureAccessRights();
                this.CreateDb();
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }

            this.Log.LogMessage("Database was created");
            return true;
        }

        private void CreateDb()
        {
            var url = new MongoUrl(this.ConnectionString);
            var settings = MongoClientSettings.FromUrl(url);
            var databaseUser = this.GetDatabaseUser(settings);

            var adminCredentials = GetAdminCredentials();
            settings.CredentialsStore.AddCredentials("admin", adminCredentials);
            settings.DefaultCredentials = null;

            var client = new MongoClient(settings);
            var server = client.GetServer();
            var db = server.GetDatabase(this.DatabaseName);

            if (databaseUser == null)
            {
                return;
            }

            var user = db.FindUser(databaseUser.Username);
            if (databaseUser.Equals(user))
            {
                return;
            }
            
            Log.LogMessage(string.Format("Adding user credentials UserName: '{0}' Password: '{1}' Admin: '{2}'", this.UserName, this.Password, this.Admin));
            db.AddUser(databaseUser);
        }

        private MongoUser GetDatabaseUser(MongoClientSettings settings)
        {
            MongoUser user = null;
            MongoCredentials credentials = this.GetUserCredentials(settings);

            if (credentials != null)
            {
                user = new MongoUser(credentials, !credentials.Admin);
            }
            return user;
        }

        private MongoCredentials GetUserCredentials(MongoClientSettings settings)
        {
            return string.IsNullOrEmpty(this.UserName) ? settings.DefaultCredentials : new MongoCredentials(this.UserName, this.Password, this.Admin);
        }

        private void EnsureAccessRights()
        {
            var url = new MongoUrl(this.ConnectionString);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);
            settings.DefaultCredentials = null;

            MongoCredentials adminCredentials = GetAdminCredentials();
            var client = new MongoClient(settings);
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase("admin");

            MongoUser user = db.FindUser("root");
            if (IsUserAdmin(user))
            {
                return;
            }

            user = new MongoUser(adminCredentials, false);
            db.AddUser(user);
        }

        private static bool IsUserAdmin(MongoUser user)
        {
            return user != null && !user.IsReadOnly;
        }

        private static MongoCredentials GetAdminCredentials()
        {
            var adminCredentials = new MongoCredentials("root", "secretPassword", true);
            return adminCredentials;
        }
    }
}
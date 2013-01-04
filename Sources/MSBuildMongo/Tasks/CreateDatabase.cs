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
                Debugger.Launch();
                this.EnsureAccessRights();
                this.CreateDb();
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }

            return true;
        }

        private void CreateDb()
        {
            var url = new MongoUrl(this.ConnectionString);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);

            MongoUser user = null;
            MongoCredentials credentials = this.GetUserCredentials(settings);
            
            if (credentials != null)
            {
                user = new MongoUser(credentials, !credentials.Admin);
            }

            settings.DefaultCredentials = null;
            
            MongoCredentials adminCredentials = GetAdminCredentials();
            settings.CredentialsStore.AddCredentials("admin", adminCredentials);
            var client = new MongoClient(settings);
            MongoServer server = client.GetServer();
            MongoDatabase db = server.GetDatabase(this.DatabaseName);

            if (user == null)
            {
                return;
            }

            MongoUser dbUser = db.FindUser(user.Username);
            if (user.Equals(dbUser))
            {
                return;
            }
            
            db.AddUser(user);
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
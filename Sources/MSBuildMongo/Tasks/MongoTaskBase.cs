using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MongoDB.Driver;

namespace MSBuildMongo.Tasks
{
    public abstract class MongoTaskBase : Task
    {
        private MongoDatabase db;

        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string DatabaseName { get; set; }

        protected MongoDatabase Database
        {
            get { return this.db ?? (this.db = this.GetDatabase()); }
        }

        private static bool IsUserAdmin(MongoUser user)
        {
            return user != null && !user.IsReadOnly;
        }

        protected static MongoCredentials GetAdminCredentials()
        {
            var adminCredentials = new MongoCredentials("root", "secretPassword", true);
            return adminCredentials;
        }

        private MongoDatabase GetDatabase()
        {
            this.EnsureAccessRights();

            var url = new MongoUrl(this.ConnectionString);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);

            MongoCredentials adminCredentials = GetAdminCredentials();
            settings.CredentialsStore.AddCredentials("admin", adminCredentials);
            settings.DefaultCredentials = null;

            var client = new MongoClient(settings);
            MongoServer server = client.GetServer();
            return server.GetDatabase(this.DatabaseName);
        }

        protected void EnsureAccessRights()
        {
            var url = new MongoUrl(this.ConnectionString);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);
            settings.DefaultCredentials = null;

            MongoCredentials adminCredentials = GetAdminCredentials();
            var client = new MongoClient(settings);
            MongoServer server = client.GetServer();
            MongoDatabase adminDatabase = server.GetDatabase("admin");

            MongoUser user = adminDatabase.FindUser("root");
            if (IsUserAdmin(user))
            {
                return;
            }

            user = new MongoUser(adminCredentials, false);
            adminDatabase.AddUser(user);
            server.Disconnect();
        }
    }
}
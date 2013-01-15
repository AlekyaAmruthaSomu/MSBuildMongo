using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MongoDB.Driver;

namespace MongoDB.MSBuild.Tasks
{
    public abstract class MongoTaskBase : Task
    {
        private Configuration configuration;
        private MongoDatabase db;

        private string AssemblyLocation
        {
            get { return Assembly.GetExecutingAssembly().Location; }
        }

        private Configuration Configuration
        {
            get { return this.configuration ?? (this.configuration = this.OpenConfiguration()); }
        }


        private string MongoAdministratorDatabase
        {
            get { return this.GetAppSettings("MongoAdministratorDatabase"); }
        }

        private string MongoAdministratorName
        {
            get { return this.GetAppSettings("MongoAdministratorName"); }
        }

        private string MongoAdministrtorPassword
        {
            get { return this.GetAppSettings("MongoAdministratorPassword"); }
        }

        [Required]
        public virtual string ConnectionString { get; set; }

        [Required]
        public virtual string DatabaseName { get; set; }

        protected MongoDatabase Database
        {
            get { return this.db ?? (this.db = this.GetDatabase()); }
        }

        private MongoDatabase GetDatabase()
        {
            this.EnsureAccessRights();

            var url = new MongoUrl(this.ConnectionString);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);

            MongoCredentials adminCredentials = this.GetAdminCredentials();

            settings.CredentialsStore.AddCredentials(this.MongoAdministratorDatabase, adminCredentials);
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

            MongoCredentials adminCredentials = this.GetAdminCredentials();
            var client = new MongoClient(settings);
            MongoServer server = client.GetServer();
            MongoDatabase adminDatabase = server.GetDatabase(this.MongoAdministratorDatabase);

            MongoUser user = adminDatabase.FindUser(this.MongoAdministratorName);
            if (IsUserAdmin(user))
            {
                return;
            }

            user = new MongoUser(adminCredentials, false);
            adminDatabase.AddUser(user);
            server.Disconnect();
        }

        private MongoCredentials GetAdminCredentials()
        {
            return new MongoCredentials(this.MongoAdministratorName, this.MongoAdministrtorPassword, true);
        }

        private static bool IsUserAdmin(MongoUser user)
        {
            return user != null && !user.IsReadOnly;
        }

        private string GetAppSettings(string appSettingsKey)
        {
            KeyValueConfigurationElement element = this.Configuration.AppSettings.Settings[appSettingsKey];
            if (element == null)
            {
                throw new ConfigurationErrorsException(string.Format("appSettings key '{0}' not found", appSettingsKey));
            }

            if (string.IsNullOrEmpty(element.Value))
            {
                throw new ConfigurationErrorsException(string.Format("appSettings key '{0}' does not have a value.",
                                                                     appSettingsKey));
            }

            return element.Value;
        }

        private Configuration OpenConfiguration()
        {
            Configuration result = ConfigurationManager.OpenExeConfiguration(this.AssemblyLocation);
            if (!result.HasFile)
            {
                throw new FileNotFoundException(result.FilePath);
            }

            return result;
        }
    }
}
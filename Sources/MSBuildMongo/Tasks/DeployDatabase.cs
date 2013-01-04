using System;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MSBuildMongo.Tasks
{
    public class DeployDatabase : Task
    {
        [Required]
        public string ConnectionString { get; set; }

        [Required]
        public string Database { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Deploying database");
                var settings = new MongoClientSettings();
                
                var client = new MongoClient(ConnectionString);
                MongoServer server = client.GetServer();
                server.Connect();
                server.Disconnect();
                Debugger.Launch();
                var credentials = new MongoCredentials("admin", "qwerty", true);
                var defaultCredentials = client.Settings.DefaultCredentials;
                var database = server.GetDatabase(Database);
                if (!string.IsNullOrWhiteSpace(defaultCredentials.Username))
                {
                    var user = database.FindUser(defaultCredentials.Username);
                    if (user == null)
                    {
                        var c = new MongoCredentials(defaultCredentials.Username, defaultCredentials.Password);
                        user = new MongoUser(c, false);
                        database.AddUser(user);
                    }
                }

                var collection = database.GetCollection("Users");
                var document = new BsonDocument
                    {
                        {
                            "name", BsonValue.Create("sergey")
                        }
                    };
                collection.Save(document);
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
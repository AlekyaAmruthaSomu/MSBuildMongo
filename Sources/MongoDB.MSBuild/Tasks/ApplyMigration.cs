using System;
using System.Linq;
using Microsoft.Build.Framework;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoDB.MSBuild.Tasks
{
    public class ApplyMigration : MongoTaskBase
    {
        private const string MigrationId = "MigrationId";

        [Required]
        public ITaskItem[] Migrations { get; set; }

        public override bool Execute()
        {
            try
            {
                this.Log.LogMessage(string.Format("Applying '{0}' migration.", this.DatabaseName));
                MongoCollection<BsonDocument> migrations = this.Database.GetCollection("Migrations");
                foreach (ITaskItem migration in this.Migrations)
                {
                    var migrationId = new Guid(migration.GetMetadata(MigrationId));
                    if (migrations.AsQueryable<Migration>().Any(m => m.MigrationId == migrationId))
                    {
                        continue;
                    }

                    string fileName = migration.GetMetadata("Identity");
                    this.Log.LogMessage(string.Format("Applying migration id: '{0}' fileName: '{1}'.", migrationId,
                                                      fileName));
                    RunJavascript.Eval(this.Database, fileName);
                    var document = new BsonDocument {{MigrationId, migrationId}};
                    migrations.Save(document);
                }
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }

            this.Log.LogMessage(string.Format("'{0}' migration completed.", this.DatabaseName));
            return true;
        }
    }
}
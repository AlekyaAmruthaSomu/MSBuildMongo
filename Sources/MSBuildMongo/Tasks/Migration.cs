using System;
using MongoDB.Bson;

namespace MongoDB.MSBuild.Tasks
{
    internal class Migration
    {
        public ObjectId _id { get; set; }
        public Guid MigrationId { get; set; }
        public string Identity { get; set; }
    }
}

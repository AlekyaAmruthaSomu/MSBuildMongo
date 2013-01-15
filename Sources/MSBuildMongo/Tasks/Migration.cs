using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace MSBuildMongo.Tasks
{
    internal class Migration
    {
        public ObjectId _id { get; set; }
        public Guid MigrationId { get; set; }
        public string Identity { get; set; }
    }
}

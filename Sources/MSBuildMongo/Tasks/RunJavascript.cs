using System;
using System.IO;
using Microsoft.Build.Framework;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDB.MSBuild.Tasks
{
    public class RunJavascript : MongoTaskBase
    {
        [Required]
        public string FileName { get; set; }

        public override bool Execute()
        {
            try
            {
                this.Log.LogMessage(string.Format("Executing javascript: '{0}'", this.FileName));
                Eval(this.Database, this.FileName);
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
            return true;
        }

        public static void Eval(MongoDatabase database, string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    var javaScript = new BsonJavaScript(reader.ReadToEnd());
                    database.Eval(javaScript);
                }
            }
        }
    }
}
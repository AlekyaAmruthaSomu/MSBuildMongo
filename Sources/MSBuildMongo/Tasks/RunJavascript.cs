using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using MongoDB.Bson;

namespace MSBuildMongo.Tasks
{
    public class RunJavascript : MongoTaskBase
    {
        [Required]
        public string FileName { get; set; }

        public override bool Execute()
        {
            try
            {
                System.Diagnostics.Debugger.Launch();
                using (var stream = new FileStream(FileName, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var javascript = new BsonJavaScript(reader.ReadToEnd());
                        var result = this.Database.Eval(javascript);
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
            return true;
        }
    }
}

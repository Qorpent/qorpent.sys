using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.IO;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// 
    /// </summary>
    public class ScriptTextDbUpdateTask : DbUpdateTaskBase {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateCode"></param>
        public ScriptTextDbUpdateTask(string name, string text, string explicithash = null) {
            Script = text;
            Name = name;
            var hash = explicithash;
            if (string.IsNullOrWhiteSpace(hash)) {
                hash = Script.GetMd5();
            }
            Source = new FileDescriptorEx {Name = name, Hash = hash, Version = DateTime.Now};
        }
        /// <summary>
        /// 
        /// </summary>
        public string Script {
            get { return Get("script", ""); }
            set {  Set("script", value); }
        }

        public override IEnumerable<string> GetScripts() {
            var queries = Regex.Split(Script, @"(?i)[\r\n]+\s*GO\s*?[\r\n]+");
            return queries.Select(query => query.Trim()).Where(q => !string.IsNullOrWhiteSpace(q));
        }
    }
}
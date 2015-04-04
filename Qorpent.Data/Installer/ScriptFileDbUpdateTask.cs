using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Utils.IO;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public class ScriptFileDbUpdateTask : DbUpdateTaskBase {
        /// <summary>
        /// </summary>
        public ScriptFileDbUpdateTask() {
        }

        /// <summary>
        /// </summary>
        /// <param name="filename"></param>
        public ScriptFileDbUpdateTask(string filename, string prefix = null) {
            if (!string.IsNullOrWhiteSpace(filename)) {
                Source = new FileDescriptorEx {FullName = filename};
                Source.Name = Path.GetFileNameWithoutExtension(filename);
                if (!string.IsNullOrWhiteSpace(prefix)) {
                    Source.Name = prefix + "." + Source.Name;
                }
            }
        }

        public override IEnumerable<string> GetScripts() {
            var srcScript = File.ReadAllText(Source.FullName);
            var queries = Regex.Split(srcScript, @"(?i)[\r\n]+\s*GO\s*?[\r\n]+");
            return queries.Select(query => query.Trim()).Where(q => !string.IsNullOrWhiteSpace(q));
        }
    }
}
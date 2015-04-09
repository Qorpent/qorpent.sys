using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;
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
                Source = new FileDescriptorEx {FullName = filename,Overrides = ResolvePathOverrides};
                Source.Name = Path.GetFileNameWithoutExtension(filename);
                if (!string.IsNullOrWhiteSpace(prefix)) {
                    Source.Name = prefix + "." + Source.Name;
                }
            }
        }

        public override IEnumerable<string> GetScripts() {
            if (File.Exists(Source.FullName)) {
                var name = Source.FullName;
                foreach (var script in GetFileScripts(name)) {
                    yield return script;
                }
                ;
            }
            else if (Directory.Exists(Source.FullName)) {
                var files = Directory.GetFiles("*.sql");
                var descs = files.Select(_ => new FileDescriptorEx {FullName = _}).OrderBy(_=>_.Header.Attr("idx","100000").ToInt());
                foreach (var desc in descs) {
                    foreach (var fileScript in GetFileScripts(desc.FullName)) {
                        yield return fileScript;
                    }

                }

            }
        }

        private static IEnumerable<string> GetFileScripts(string name) {
            var srcScript = File.ReadAllText(name);
            var queries = Regex.Split(srcScript, @"(?i)[\r\n]+\s*GO\s*?[\r\n]+");
            return queries.Select(query => query.Trim()).Where(q => !string.IsNullOrWhiteSpace(q));
        }
    }
}
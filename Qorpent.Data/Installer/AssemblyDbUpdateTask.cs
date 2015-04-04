using System.Collections.Generic;
using System.IO;
using Qorpent.Data.Installer.SqlExtensions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.IO;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public class AssemblyDbUpdateTask : DbUpdateTaskBase {
        public const int Index = InitClrTask.Index + 10000;
        private string _schema;

        /// <summary>
        /// </summary>
        public AssemblyDbUpdateTask() {
            Idx = Index;
        }

        /// <summary>
        /// </summary>
        /// <param name="filename"></param>
        public AssemblyDbUpdateTask(string filename, string prefix = null) {
            if (!string.IsNullOrWhiteSpace(filename)) {
                Source = new FileDescriptorEx {FullName = filename};
                Source.Name = Path.GetFileNameWithoutExtension(filename);
                if (!string.IsNullOrWhiteSpace(prefix)) {
                    Source.Name = prefix + "." + Source.Name;
                }
            }
        }

        /// <summary>
        /// </summary>
        public string Schema {
            get {
                if (options.ContainsKey("schema")) {
                    return Get("schema", "qorpent");
                }
                if (null != Source && null != Source.Header) {
                    if (Source.Header.Attr("Schema").ToBool()) {
                        return Source.Header.Attr("Schema");
                    }
                }
                return Get("schema", "qorpent");
            }
            set { Set("schema", value); }
        }

        public override IEnumerable<string> GetScripts() {
            var generator = new SqlInstallerConsoleProgram();
            var parameters = new SqlInstallerConsoleProgramArgs {
                AssemblyName = Source.FullName,
                UseAssemblyPath = true,
                NoScriptDatabase = true,
                GenerateScript = true,
                Schema = Schema,
                NoOutput = true
            };
            generator.Run(parameters);
            return generator.Scripts;
        }
    }
}
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model;
using Qorpent.Tasks;
using Qorpent.Utils.IO;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// 
    /// </summary>
    public abstract class BxlDataProjectUpdateTaskBase : DbUpdateTaskBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectfile"></param>
        /// <param name="projectname"></param>
        protected BxlDataProjectUpdateTaskBase(string projectfile, string projectname = null)
        {
            ResolvedFile = Path.GetFullPath(EnvironmentInfo.ResolvePath(projectfile));
            if (string.IsNullOrWhiteSpace(projectname)) {
                if (File.Exists(ResolvedFile) && Path.GetExtension(ResolvedFile).ToLowerInvariant() == ".bsproj") {
                    ProjectName = Path.GetFileNameWithoutExtension(ResolvedFile);
                }
            }
            else {
                ProjectName = projectname;
            }
      
        
            
        }

        public string Suffix { get; set; }

        public string ProjectName {
            get { return Get("projectname", ""); }
            set { Set("projectname", value); }
        }

        public string ResolvedFile {
            get { return Get("resolvedfile", ""); }
            set { Set("resolvedfile",value); }
        }

        public override void Initialize(IJob package = null) {
            var name = Path.GetFileName(ResolvedFile) + "_" + Suffix;
            Source = new FileDescriptorEx { FullName = ResolvedFile, Name = name, UseRepositoryCommit = true };
            base.Initialize(package);
            Compile();
        }

        static readonly IDictionary<string,IBSharpContext> _cache = new ConcurrentDictionary<string, IBSharpContext>();

        private void Compile() {
            var dir = ResolvedFile;
            if (!Directory.Exists(ResolvedFile)) {
                dir = Path.GetDirectoryName(ResolvedFile);
            }
            var key = dir + "-" + ProjectName + "-" + Source.Hash;
            if (!_cache.ContainsKey(key)) {
                _cache[key] = BscHelper.Execute(dir, ProjectName);
            }
            Context = _cache[key];
            this.Model = new PersistentModel().Setup(Context);
        }

        public PersistentModel Model {
            get { return Get<PersistentModel>("model", null); }
            set { Set("model", value); }
        }

        public IBSharpContext Context {
            get { return Get<IBSharpContext>("bscontext", null); }
            set { Set("bscontext",value); }
        }

        public override void Refresh() {
            base.Refresh();
            Compile();
        }

        
    }
}
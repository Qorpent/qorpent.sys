using System.Collections;
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
            var name = Path.GetFileName(ResolvedFile);
             Source = new FileDescriptorEx { FullName = ResolvedFile, Name = name, UseRepositoryCommit = true };
        
            
        }

        public string ProjectName {
            get { return Get("projectname", ""); }
            set { Set("projectname", value); }
        }

        public string ResolvedFile {
            get { return Get("resolvedfile", ""); }
            set { Set("resolvedfile",value); }
        }

        public override void Initialize(IJob package = null) {
            base.Initialize(package);
            Compile();
        }

        private void Compile() {
            var dir = ResolvedFile;
            if (!Directory.Exists(ResolvedFile)) {
                dir = Path.GetDirectoryName(ResolvedFile);
            }
            Context = BscHelper.Execute(dir, ProjectName);
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
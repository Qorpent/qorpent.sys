using System.IO;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Tasks.WriteTasks {
    /// <summary>
    /// 
    /// </summary>
    public class WriteOrphansOutputTask : WriteTaskBase {
		
        /// <summary>
        /// 
        /// </summary>
        public WriteOrphansOutputTask() {
			Phase = BSharpBuilderPhase.PostProcess;
			Index = TaskConstants.WriteOrphansOutputTaskIndex;
            IncludeFlag = BSharpBuilderOutputAttributes.IncludeOrphans;
            DataType = BSharpContextDataType.Orphans;
            WorkingDirectory = BSharpBuilderDefaults.OrphansOutputDirectory;
		}

		/// <summary>
		/// Установить целевой проект
		/// </summary>
		/// <param name="project"></param>
		public override void SetProject(IBSharpProject project)
		{
			base.SetProject(project);
			WorkingDirectory = Path.Combine( project.GetOutputDirectory(),BSharpBuilderDefaults.OrphansOutputDirectory);
		}
    }
}
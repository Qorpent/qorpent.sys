using Qorpent.BSharp.Builder;
using System.IO;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
    /// 
    /// </summary>
    public class CleanUpTask : BSharpBuilderTaskBase {
        /// <summary>
        ///     Производит очистку выходных директорий
        /// </summary>
        public CleanUpTask() {
            Phase = BSharpBuilderPhase.PostProcess;
            Index = TaskConstants.CleanUpTaskIndex;
        }
        /// <summary>
        ///     выполнение задачи
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(Qorpent.BSharp.IBSharpContext context) {
            CleanDirectory(Project.GetOutputDirectory());
            CleanDirectory(Project.GetLogDirectory());
            Project.Log.Trace("output cleaned");
        }
        /// <summary>
        ///     Зачищает директорию путём удаление и создания
        /// </summary>
        /// <param name="target">Целевая директория</param>
        private void CleanDirectory(string target) {
            BSharpBuilderFsUtils.DeleteDirectory(target);
            Directory.CreateDirectory(target);
        }
    }
}

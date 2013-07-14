namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Действие для возврата скрипта загрузки
    /// </summary>
    [Action("_sys.compileloadscript", Role = "ADMIN", Arm="admin")]
    public class CompileLoadScriptAction : ActionBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess()
        {
            LoadService.Default.Compile();
            return "started";
        }
    }
}
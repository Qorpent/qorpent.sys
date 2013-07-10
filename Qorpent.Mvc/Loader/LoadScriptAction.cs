using Qorpent.IO;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Действие для возврата скрипта загрузки
    /// </summary>
    [Action("_sys.loadscript",Role="DEFAULT")]
    public class LoadScriptAction : ActionBase {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            var level = LoadLevel.Guest;
            if (User.Identity.IsAuthenticated) {
                level = LoadLevel.Auth;
                if (IsInRole("ADMIN")) {
                    level = LoadLevel.Admin;
                }
            }
            LoadService.Default.Synchronize();
            var redirectPath = LoadService.Default.GetFileName(level, FileSearchResultType.LocalUrl);
            Context.Redirect(redirectPath);
            return true;
        }
    }
}
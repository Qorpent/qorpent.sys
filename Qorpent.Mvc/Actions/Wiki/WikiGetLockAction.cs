using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions.Wiki {
    /// <summary>
    /// 
    /// </summary>
    [Action("wiki.getlock", Help = "установить блокировку на страницу, если это возможно", Role="DOCWRITER", Arm="sys")]
    class WikiGetLockAction : WikiActionBase {
        /// <summary>
        /// 
        /// </summary>
        [Bind(Required = true)] public string Code;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            return WikiSource.GetLock(Code);
        }
    }
}

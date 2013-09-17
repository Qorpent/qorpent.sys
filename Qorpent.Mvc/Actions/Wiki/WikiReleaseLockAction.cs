using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions.Wiki {
    /// <summary>
    /// 
    /// </summary>
    [Action("wiki.releaselock", Help = "Снять блокировку, если это возможно", Role="DOCWRITER", Arm="sys")]
    class WikiReleaseLockAction : WikiActionBase {
        /// <summary>
        /// 
        /// </summary>
        [Bind(Required = true)]
        public string Code;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            return WikiSource.Releaselock(Code);
        }
    }
}
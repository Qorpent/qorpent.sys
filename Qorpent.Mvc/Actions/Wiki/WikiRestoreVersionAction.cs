using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions.Wiki {
    [Action("wiki.restoreversion", Role="DOCWRITER" , Arm="sys")]
    class WikiRestoreVersionAction : WikiActionBase {
        /// <summary>
        ///     Код
        /// </summary>
        [Bind(Required = true)]
        public string Code;

        /// <summary>
        ///     Комментарий к версии страницы
        /// </summary>
        [Bind(Required = true)]
        public string Version;

        /// <summary>
        /// Возвращает страницы Wiki по запросу
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            return WikiSource.RestoreVersion(Code, Version);
        }
    }
}

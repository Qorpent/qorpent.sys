using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
    /// <summary>
    ///     Действие создания версии страницы Wiki
    /// </summary>
    [Action("wiki.getversionslist", Help = "Получить список версий страницы Wiki", Role="DOCWRITER" )]
    public class WikiGetVersionsListAction : WikiActionBase {
        /// <summary>
        ///     Код
        /// </summary>
        [Bind(Required = true)] public string Code;

        /// <summary>
        /// Возвращает страницы Wiki по запросу
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {
            return WikiSource.GetVersionsList(Code);
        }
     
    }
}
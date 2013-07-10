using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Qorpent.Mvc.Binding;
using Qorpent.Mvc.Renders;
using Qorpent.Utils.Extensions;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Actions {
    /// <summary>
    ///     Действие создания версии страницы Wiki
    /// </summary>
    [Action("wiki.createversion", Help = "Создать версию Wiki")]
    public class WikiCreateVersionAction : WikiActionBase {
        /// <summary>
        ///     Код
        /// </summary>
        [Bind(Required = true)] public string Code;

        /// <summary>
        ///     Комментарий к версии страницы
        /// </summary>
        [Bind] public string Comment;

        /// <summary>
        /// Возвращает страницы Wiki по запросу
        /// </summary>
        /// <returns></returns>
        protected override object MainProcess() {

            return WikiSource.CreateVersion(Code, Comment);
        }
     
    }
}
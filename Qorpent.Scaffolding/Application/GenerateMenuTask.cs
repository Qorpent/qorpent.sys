using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// Генератор меню
    /// </summary>
    public class GenerateMenuTask : CodeGeneratorTaskBase {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetclasses"></param>
        protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses) {
            foreach (var targetclass in targetclasses) {
                var c = targetclass.Compiled;
                yield return new Production {
                    FileName = c.Attr("code") + "-menu.html",
                    GetContent = () => c.Elements("MenuGroup").ToArray().Length > 0 ? GenerateGroupedMenu(c) : GenerateSimpleMenu(c)
                };
            }
        }

        private static string GenerateGroupedMenu(XElement root) {
            var menuroot = new XElement("div",
                new XAttribute("root", 1),
                new XAttribute("class", "menu__root menu__ribbon"),
                new XAttribute("id", root.Attr("code")));
            var menugroups = new XElement("menu",
                new XAttribute("class", "menu__text menu__horizontal"),
                new XAttribute("id", root.Attr("code") + "Groups"));
            var menubody = new XElement("menu",
                new XAttribute("class", "menu__withicons menu__horizontal"),
                new XAttribute("id", root.Attr("code") + "Items"));
            foreach (var g in root.Elements("MenuGroup")) {
                foreach (var m in g.Elements("MenuItem")) {
                    menubody.Add(GenerateIconItem(m));
                }
                menugroups.Add(GenerateTextItem(g));
            }
            menuroot.Add(menubody);
            menuroot.Add(menugroups);
            return menuroot.ToString();
        }

        private static XElement GenerateIconItem(XElement el) {
            var result = new XElement("div",
                new XAttribute("class", "menu__item"));
            var icon = new XElement("div",
                new XAttribute("class", "menu__item-icon"));
            if (el.HasAttribute("iconclass")) {
                icon.Add(new XElement("i", new XAttribute("class", el.Attr("iconclass"))));
            }
            else if (el.HasAttribute("iconsrc")) {
                icon.Add(new XElement("img", new XAttribute("src", el.Attr("iconsrc"))));
            }
            else if (el.HasAttribute("view")) {
                icon.Add(new XElement("ng-include", new XAttribute("src", el.Attr("view"))));
            }
            var title = new XElement("div",
                new XAttribute("class", "menu__item-title"));
            title.SetValue(el.HasAttribute("title") ? el.Attr("title") : el.Attr("name"));
            result.Add(icon);
            result.Add(title);
            return result;
        }

        private static XElement GenerateTextItem(XElement el) {
            var result = new XElement("div",
                                         new XAttribute("class", "menu__item"));
            var link = new XElement("a", new XAttribute("href", ""));
            link.SetValue(el.Attr("name"));
            result.Add(link);
            return result;
        }

        private static string GenerateSimpleMenu(XElement root) {
            return "";
        }

        /// <summary>
        /// Конструктор генератора меню
        /// </summary>
        public GenerateMenuTask()
            : base() {
            ClassSearchCriteria = "ui-menu";
            DefaultOutputName = "View";
        }
    }
}
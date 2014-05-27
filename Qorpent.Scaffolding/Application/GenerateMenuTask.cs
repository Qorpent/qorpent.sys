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
                    GetContent = () => GenrateMenu(c)
                };
            }
        }

        private string GenrateMenu(XElement root) {
            switch (root.Attr("type")) {
                case "ribbon" : return GenerateRibbonMenu(root);
                case "inline": return GenerateInlineMenu(root);
                case "dropdown": return GenerateDropdownMenu(root);
                default: return GenerateInlineMenu(root);
            }
        }

        private static XElement GenerateMenuItem(XElement el) {
            if (el.Attr("name") == "divider") {
                return GenerateMenuDivider();
            } else {
                if (!el.HasAttribute("type")) el.SetAttr("type", "text");
                switch (el.Attr("type")) {
                    case "text": return GenerateTextItem(el);
                    case "icon": return GenerateIconItem(el);
                    case "icon_with_text": return GenerateIconWithTextItem(el);
                    default: return GenerateTextItem(el);
                }   
            }
        }

        private static XElement GenerateMenuDivider() {
            return new XElement("div", 
                new XAttribute("class", "menu__item menu__item-divider"),
                new XAttribute("type", "divider"));
        }

        private static XElement GenerateDropdownGroup(XElement el) {
            var result = GenerateMenuItem(el);
            result.SetAttr("menu-group", 1);
            result.SetAttr("code", el.Attr("code"));
            result.SetAttr("class", result.Attr("class") + " open-" + (el.HasAttribute("direction") ? el.Attr("direction") : "right"));
            var menuGroup = new XElement("div", new XAttribute("class", "submenu"));
            foreach (var m in el.Elements("MenuItem")) {
                menuGroup.Add(m.HasElements ? GenerateDropdownGroup(m) : GenerateDropdownItem(m));
            }
            result.Add(menuGroup);
            return result;
        }

        private static XElement GenerateDropdownItem(XElement el) {
            var result = GenerateMenuItem(el);
            result.SetAttr("menu-item", 1);
            result.SetAttr("group", el.Parent.Attr("code"));
            return result;
        }

        private string GenerateRibbonMenu(XElement root) {
            var menuRoot = GenerateMenuRoot(root);
            var menuGroups = GenerateMenuElement(root);
            menuGroups.SetAttr("id", root.Attr("code") + "_Groups");
            menuGroups.SetAttr("class", menuGroups.Attr("class") + " menu__ribbon-groups");
            var menuBody = GenerateMenuElement(root);
            menuBody.SetAttr("id", root.Attr("code") + "_Items");
            menuBody.SetAttr("class", menuBody.Attr("class") + " menu__ribbon-items");
            foreach (var g in root.Elements("MenuItem")) {
                foreach (var m in g.Elements("MenuItem")) {
                    menuBody.Add(GenerateMenuItem(m)
                        .SetAttr("group", g.Attr("code"))
                        .SetAttr("menu-item", 1));
                }
                menuGroups.Add(GenerateMenuItem(g)
                    .SetAttr("menu-group", 1)
                    .SetAttr("code", g.Attr("code")));
            }
            menuRoot.Add(menuGroups);
            menuRoot.Add(menuBody);
            return menuRoot.ToString();
        }

        private static XElement GenerateMenuRoot(XElement root) {
            var menuRoot = new XElement("div",
                new XAttribute("menu", root.Attr("type")),
                new XAttribute("class", "menu__root menu__" + root.Attr("type")),
                new XAttribute("id", root.Attr("code") + "_Menu"));
            return menuRoot;
        }

        private static XElement GenerateMenuElement(XElement root) {
            var orientation = root.Attr("orientation");
            var menuElement = new XElement("div",
                new XAttribute("class", "menu__" + (orientation == "" ? "horizontal" : orientation)));
            return menuElement;
        }

        private static string GenerateDropdownMenu(XElement root) {
            var menuRoot = GenerateMenuRoot(root);
            var menuBody = GenerateMenuElement(root);
            menuBody.SetAttr("id", root.Attr("code") + "_Items");
            foreach (var g in root.Elements("MenuItem")) {
                if (g.HasElements) {
                    menuBody.Add(GenerateDropdownGroup(g)
                        .SetAttr("code", g.Attr("code"))
                        .SetAttr("menu-group", 1));
                }
                else {
                    menuBody.Add(GenerateDropdownItem(g)
                        .SetAttr("group", g.Attr("code"))
                        .SetAttr("menu-item", 1));
                }
            }
            menuRoot.Add(menuBody);
            return menuRoot.ToString();
        }

        private static XElement GenerateIconItem(XElement el) {
            var result = new XElement("div",
                new XAttribute("class", "menu__item"));
            CopyAttributes(result, el);
            var icon = new XElement("div",
                new XAttribute("class", "icon menu__item-icon menu__item-element"));
            if (el.HasAttribute("iconclass")) {
                icon.Add(new XElement("i", new XAttribute("class", el.Attr("iconclass"))));
            } else if (el.HasAttribute("icon")) {
                icon.Add(new XElement("i", new XAttribute("class", el.Attr("icon"))));
            } else if (el.HasAttribute("src")) {
                icon.Add(new XElement("img", new XAttribute("src", el.Attr("src"))));
            } else if (el.HasAttribute("iconsrc")) {
                icon.Add(new XElement("img", new XAttribute("src", el.Attr("iconsrc"))));
            } else if (el.HasAttribute("view")) {
                icon.Add(new XElement("ng-include", new XAttribute("src", el.Attr("view"))));
            }
            result.Add(icon);
            return result;
        }

        private static void CopyAttributes(XElement target, XElement el) {
            var ignoreAttributes = new[] { "code", "name", "iconclass", "icon" };
            foreach (var a in el.Attributes().Where(_ => !_.Name.ToString().IsIn(ignoreAttributes))) {
                target.SetAttr(a.Name.ToString(), a.Value);
            }
        }

        private static XElement GenerateIconWithTextItem(XElement el) {
            var result = GenerateIconItem(el);
            result.SetAttr("class", "menu__item");
            var title = new XElement("div",
                new XAttribute("class", "menu__item-title menu__item-element"));
            title.SetValue(el.HasAttribute("title") ? el.Attr("title") : el.Attr("name"));
            result.Add(title);
            return result;
        }

        private static XElement GenerateTextItem(XElement el) {
            var result = new XElement("div",
                new XAttribute("class", "menu__item"));
            CopyAttributes(result, el);
            var link = new XElement("div", new XAttribute("class", "menu__item-title menu__item-element"));
            link.SetValue(el.Attr("name"));
            result.Add(link);
            return result;
        }

        private static string GenerateInlineMenu(XElement root) {
            var menuRoot = GenerateMenuRoot(root);
            var menuBody = GenerateMenuElement(root);
            menuBody.SetAttr("items", 1);
            menuBody.SetAttr("id", root.Attr("code") + "_Items");
            foreach (var m in root.Elements("MenuItem")) {
                menuBody.Add(GenerateMenuItem(m)
                    .SetAttr("menu-item", 1));
            }
            menuRoot.Add(menuBody);
            return menuRoot.ToString();
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
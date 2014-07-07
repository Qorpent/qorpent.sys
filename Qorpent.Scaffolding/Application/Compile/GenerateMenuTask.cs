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
                XElement result;
                switch (el.Attr("type")) {
                    case "text": result = GenerateTextItem(el); break;
                    case "icon": result = GenerateIconItem(el); break;
                    case "icon_with_text": result = GenerateIconWithTextItem(el); break;
                    default: result = GenerateTextItem(el); break;
                }
                CopyAttributes(result, el);
                /*result.SetAttr("menu-title", el.Attr("name"));*/
                result.SetAttr("class", result.Attr("class") + " menu__item-" + (el.HasAttribute("size") ? el.Attr("size") : "small"));
                if (el.HasAttribute("class")) {
                    result.SetAttr("class", result.Attr("class") + " " + el.Attr("class"));
                }
                if (el.HasAttribute("action")) {
                    result.Attr("ng-click", el.Attr("action"));
                }
                return result;
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
            if (el.HasElements) {
                var menuGroup = new XElement("div", new XAttribute("class", "submenu"));
                foreach (var m in el.Elements("MenuItem")) {
                    menuGroup.Add(m.HasElements ? GenerateDropdownGroup(m) : GenerateDropdownItem(m));
                }
                result.Add(menuGroup);   
            }
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
                    // по-умолчанию кнопки в риббон-меню - большие
                    if (!m.HasAttribute("size")) m.SetAttr("size", "large");
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
            CopyAttributes(menuRoot, root);
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
            if (root.HasElements) {
                foreach (var g in root.Elements("MenuItem")) {
                    if (g.HasElements) {
                        menuBody.Add(GenerateDropdownGroup(g)
                            .SetAttr("code", g.Attr("code"))
                            .SetAttr("menu-group", 1));
                    } else {
                        XElement menuItem;
                        if (g.HasAttribute("model")) {
                            menuItem = GenerateDropdownGroup(g);
                            menuItem
                                .SetAttr("code", g.Attr("code"))
                                .SetAttr("menu-group", 1)
                                .SetAttr("model", g.Attr("model"));
                            menuItem.Add(GenerateSubmenuFromModel(g));
                        } else {
                            menuItem = GenerateDropdownItem(g);
                            menuItem
                                .SetAttr("group", g.Attr("code"))
                                .SetAttr("menu-item", 1);
                        }
                        menuBody.Add(menuItem);
                    }
                }  
            }
            else if (root.HasAttribute("model")) {
                 menuBody.Add(GenerateMenuFromModel(root));
            }
            menuRoot.Add(menuBody);
            return menuRoot.ToString();
        }

        private static XElement GenerateIconItem(XElement el) {
            var result = new XElement("div",
                new XAttribute("class", "menu__item"));
            var icon = new XElement("div",
                new XAttribute("class", "icon menu__item-icon menu__item-element"), new XAttribute("menu-icon", 1));
            if (el.HasAttribute("iconclass")) {
                icon.Add(new XElement("i", " ", new XAttribute("class", el.Attr("iconclass"))));
            } else if (el.HasAttribute("icon")) {
                icon.Add(new XElement("i", " ", new XAttribute("class", el.Attr("icon"))));
            } else if (el.HasAttribute("src")) {
                icon.Add(new XElement("img", new XAttribute("src", el.Attr("src"))));
            } else if (el.HasAttribute("iconsrc")) {
                icon.Add(new XElement("img", new XAttribute("src", el.Attr("iconsrc"))));
            } else if (el.HasAttribute("view")) {
                icon.Add(new XElement("ng-include", new XAttribute("src", el.Attr("view")), " "));
            }
            result.Add(icon);
            if (el.Attr("type") == "icon") {
                var title = new XElement("div",
                   new XAttribute("class", "menu__item-title menu__item-element"));
                result.Add(title);
            }
            return result;
        }

        private static void CopyAttributes(XElement target, XElement el = null) {
            if (null == el) el = target;
            var ignoreAttributes = new[] { "code", "name", "iconclass", "icon", "prototype", "fullcode", "action", "size", "class", "titleclass", "wrapper", "model" };
            foreach (var a in el.Attributes().Where(_ => !_.Name.ToString().IsIn(ignoreAttributes))) {
                target.SetAttr(a.Name.ToString(), a.Value);
            }
            if (el.HasAttribute("action")) {
                target.SetAttr("ng-click", el.Attr("action"));
            }
        }

        private static XElement GenerateIconWithTextItem(XElement el) {
            var result = GenerateIconItem(el);
            var title = new XElement("div",
                new XAttribute("class", (el.Attr("titleclass") + "menu__item-title menu__item-element").Trim()));
            title.SetValue(el.HasAttribute("title") ? el.Attr("title") : el.Attr("name"));
            result.Add(title);
            return result;
        }

        private static XElement GenerateTextItem(XElement el) {
            var result = new XElement("div",
                new XAttribute("class", "menu__item"));
            var link = new XElement("div", new XAttribute("class", (el.Attr("titleclass") + " menu__item-title menu__item-element").Trim()));
            link.SetValue(el.Attr("name"));
            result.Add(link);
            return result;
        }

        private static XElement GenerateMenuFromModel(XElement el) {
            if (!el.HasAttribute("model")) return null;
            var expression = el.Attr("model");
            if (el.HasAttribute("wrapper")) {
                expression = el.Attr("wrapper") + '(' + expression + ')';
            }
            if (el.HasAttribute("filter")) {
                expression += " | " + el.Attr("filter");
            }
            var result = new XElement("div",
                new XAttribute("class", "menu__item"),
                new XAttribute("menu-item", 1),
                new XAttribute("ng-repeat", "m in " + expression),
                new XAttribute("ng-click", "exec(m)"),
                new XAttribute("ng-class", "'menu__item-' + (!!m.size ? m.size : 'small') + (m.Name == 'divider' ? ' menu__item-divider' : '')"),
                new XAttribute("color", "{{m.color || m.Color}}"),
                new XAttribute("type", "{{m.type || m.Type}}"),
                new XAttribute("ng-include", "'menu-item.html'"),
                " "/*,
                new XElement("div",
                    new XAttribute("ng-if", "(m.type || m.Type) == \'view\' && m.Name != \'divider\'"),
                    new XAttribute("class", "menu__item-title menu__item-element" + (el.HasAttribute("titleclass") ? " " + el.Attr("titleclass") : "")),
                    new XElement("ng-include",
                        new XAttribute("ng-src", "\' + (m.view.indexOf(\'.html\') != -1 ? m.view : m.view + \'.html\')\'"),
                        " "
                    )
                ),
                new XElement("div",
                    new XAttribute("ng-if", "(m.type || m.Type) == \'icon_with_text\' && m.Name != \'divider\'"),
                    new XAttribute("class", "icon menu__item-icon menu__item-element"),
                    new XAttribute("menu-icon", 1),
                    new XAttribute("color", "{{m.color || m.Color}}"),
                    new XElement("img",
                        new XAttribute("ng-if", "(m.icon || m.Icon).indexOf(\'/\') != -1"),
                        new XAttribute("ng-src", "m.icon || m.Icon")
                    ),
                    new XElement("i",
                        new XAttribute("ng-if", "(m.icon || m.Icon).indexOf(\'/\') == -1"),
                        new XAttribute("ng-class", "m.icon || m.Icon"),
                        " "
                    )
                ),
                    new XElement("div",
                        new XAttribute("ng-if", "m.Name != \'divider\'"),
                        new XAttribute("class", "menu__item-title menu__item-element" + (el.HasAttribute("titleclass") ? " " + el.Attr("titleclass") : "")),
                        new XAttribute("ng-bind", "(m.name || m.Name)"),
                        " "
                    )*/
            );
            return result;
        }

        private static XElement GenerateSubmenuFromModel(XElement root) {
            var result = new XElement("div", new XAttribute("class", "submenu"), " ");
            // result.Add(GenerateMenuFromModel(root));
            return result;
        }

        private static string GenerateInlineMenu(XElement root) {
            var menuRoot = GenerateMenuRoot(root);
            var menuBody = GenerateMenuElement(root);
            menuBody.SetAttr("items", 1);
            menuBody.SetAttr("id", root.Attr("code") + "_Items");
            if (root.HasAttribute("model")) {
                menuBody.Add(GenerateMenuFromModel(root));
            }
            else {
                foreach (var m in root.Elements("MenuItem")) {
                    menuBody.Add(GenerateMenuItem(m)
                        .SetAttr("menu-item", 1));
                }   
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
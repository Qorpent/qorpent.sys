using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    public class AppStruct:AppObject<AppStruct> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        public override AppStruct Setup(ApplicationModel model, IBSharpClass cls) {
            base.Setup(model, cls);
            Fields = new List<StructField>();
            foreach (var field in cls.Compiled.Elements()) {
                Fields.Add(ResolveField(field));
            }
            return this;
        }

        private StructField ResolveField(XElement field) {
            return new StructField {
                Type = field.Name.ToString(),
                Code = field.GetCode(),
                Name = field.GetName(),
                DefaultValue = field.Value
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var fieldsString = new StringBuilder();
            fieldsString.AppendLine(string.Format("{0} {1} '{2}'", GetType().Name, Code, Name));
            foreach (var field in Fields) {
                var line = string.Format("\t{0}({1})", field.Code, field.Type);
                if (null != field.DefaultValue) {
                    line += " : " + field.DefaultValue;
                }
                line += " //" + field.Name;
                fieldsString.AppendLine(line);
            }
            return fieldsString.ToString();
        }

        /// <summary>
        /// Аттрибуты структуры
        /// </summary>
        public IList<StructField> Fields { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppController:AppObject<AppController> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        public override AppController Setup(ApplicationModel model, IBSharpClass cls) {
            base.Setup(model, cls);
            Services = new List<AppService>();
            Items = new List<AppItem>();
            Menus = new List<AppMenu>();
            foreach (var item in cls.Compiled.Elements("item")) {
                Items.Add(new AppItem().Setup(item));
            }
            foreach (var service in cls.Compiled.Elements("service")) {
                Services.Add(new AppService().Setup(service));
            }
            foreach (var menu in cls.Compiled.Elements("menu")) {
                MenuItems.Add(new AppMenu().Setup(menu));
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AppService> Services { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AppItem> ItemItems { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AppItem> Items { get; set; }
        /// <summary>
        /// Элементы с сылками на меню
        /// </summary>
        public List<AppMenu> MenuItems { get; set; } 
        /// <summary>
        /// Сами менюхи
        /// </summary>
        public List<AppMenu> Menus { get; set; } 
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppLayout:AppObject<AppLayout> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parent"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public AppLayout Setup(XElement el, AppLayout parent = null, ApplicationModel model = null) {
            ApplyAttributes(el);
            this.Widgets = new Dictionary<string, AppWidget>();
            this.Layouts = new Dictionary<string, AppLayout>();
            if (null == parent) {
                this.Root = true;
            }
            if (null != model) {
                this.Model = model;
            }
            else {
                this.Parent = parent;
            }
            if (el.HasElements) {
                foreach (var ch in el.Elements()) {
                    if (ch.Name.LocalName.ToLower() == "layout") {
                        var layout = new AppLayout().Setup(ch, this);
                        this.Layouts.Add(layout.Code, layout);
                    } 
                    else if (ch.Name.LocalName.ToLower() == "widget") {
                        var widget = new AppWidget().Setup(ch, this);
                        this.Widgets.Add(widget.Code, widget);
                    }
                }
            }
            return this;
        }

        private void AssignControllersWithWidgets(AppLayout layout) {
            if (null != layout.Widgets) {
                foreach (var w in layout.Widgets) {
                    var controller = Model.Resolve<AppController>(w.Key);
                    if (null != controller) {
                        w.Value.Controller = controller;
                    }
                }
            }
            if (null != layout.Layouts) {
                foreach (var l in layout.Layouts) {
                    AssignControllersWithWidgets(l.Value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Bind() {
            AssignControllersWithWidgets(this);
        }

        /// <summary>
        /// Является ли рутовым элементом
        /// </summary>
        public bool Root { get; set; }
        /// <summary>
        /// Направление лаяута
        /// </summary>
        public string Orientation { get; set; }
        /// <summary>
        /// Использовать ли разделитель
        /// </summary>
        public bool Split { get; set; }
        /// <summary>
        /// Дочерние лаяуты
        /// </summary>
        public IDictionary<string, AppLayout> Layouts { get; set; }
        /// <summary>
        /// Дочерние виджеты
        /// </summary>
        public IDictionary<string, AppWidget> Widgets { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppView:AppObject<AppView> {
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppWidget:AppObject<AppWidget> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public AppWidget Setup(XElement el, AppLayout parent = null) {
            ApplyAttributes(el);
            this.Views = new Dictionary<string, AppView>();
            if (null != parent) {
                this.Layout = parent;
            }
            if (el.HasElements) {
                foreach (var v in el.Elements("view")) {
                    var view = new AppView().Setup(v);
                    Views.Add(view.Code, view);
                }
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public AppLayout Layout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Overflow { get; set; }
        /// <summary>
        /// Ширина лаяута
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// Высота лаяута
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, AppView> Views { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AppController Controller { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppService:AppObject<AppService> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public override AppService Setup(XElement el) {
            ApplyAttributes(el);
            Subscriptions = new List<string>();

            if (el.HasElements) {
                foreach (var s in el.Elements("subscribe")) {
                    Subscriptions.Add(s.Attr("code"));
                }
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IList<string> Subscriptions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppItem : AppObject<AppItem> {
        /// <summary>
        /// 
        /// </summary>
        public string Action { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppMenu : AppObject<AppMenu> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public override AppMenu Setup(XElement el) {
            ApplyAttributes(el);
            this.Items = new Dictionary<string, AppMenuItem>();
            if (el.HasElements) {
                foreach (var item in el.Elements()) {
                    var newmenuitem = new AppMenuItem().Setup(item, null, this);
                    this.Items.Add(newmenuitem.Code, newmenuitem);
                }
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public AppMenuType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string,AppMenuItem> Items { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public AppMenuItem GetItem(string code) {
            return GetItemByCode(Items, code);
        }

        private AppMenuItem GetItemByCode(IDictionary<string,AppMenuItem> items, string code) {
            AppMenuItem result = null;
            if (items.ContainsKey(code)) {
                result = items[code];
            }
            else {
                foreach (var item in items) {
                    if (null != result) continue;
                    result = GetItemByCode(item.Value.Items, code);
                }   
            }
            return result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AppMenuItem:AppObject<AppMenuItem> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parent"></param>
        /// <param name="menu"></param>
        /// <returns></returns>
        public AppMenuItem Setup(XElement el, AppMenuItem parent = null, AppMenu menu = null) {
            el.Apply(this, new[] {"type"});
            foreach (var attr in el.Attributes()) {
                this.Attributes.Add(attr.Name.LocalName, attr.Value);
            }
            this.Items = new Dictionary<string, AppMenuItem>();
            if (null != menu) {
                this.Menu = menu;
            }
            if (null != parent) {
                this.Parent = parent;
            }
            switch (el.Attr("Type")) {
                case "text": this.Type = AppMenuItemType.Text; break;
                case "icon": this.Type = AppMenuItemType.Icon; break;
                case "icon_with_text": this.Type = AppMenuItemType.IconWithText; break;
                default : this.Type = AppMenuItemType.Default; break;
            }
            if (el.HasElements) {
                foreach (var item in el.Elements()) {
                    var newmenuitem = new AppMenuItem().Setup(item, this, menu);
                    this.Items.Add(newmenuitem.Code, newmenuitem);
                }
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, AppMenuItem> Items { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public AppMenuItemType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AppMenu Menu { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum AppMenuItemType {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Text = 1,
        /// <summary>
        /// 
        /// </summary>
        Icon = 2,
        /// <summary>
        /// 
        /// </summary>
        IconWithText = 4,
        /// <summary>
        /// 
        /// </summary>
        Default = Text
    }

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum AppMenuType {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Inline = 1,
        /// <summary>
        /// 
        /// </summary>
        Ribbon = 2,
        /// <summary>
        /// 
        /// </summary>
        DropDown = 4,
        /// <summary>
        /// 
        /// </summary>
        Default = Inline
    }
}
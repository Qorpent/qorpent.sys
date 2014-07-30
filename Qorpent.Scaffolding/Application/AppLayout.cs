using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Scaffolding.Application {
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
}
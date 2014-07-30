using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Scaffolding.Application {
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
}
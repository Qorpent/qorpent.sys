using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.Scaffolding.Application {
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
}
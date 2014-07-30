using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
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
}
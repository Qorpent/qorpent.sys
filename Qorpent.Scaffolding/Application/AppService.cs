using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
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
}
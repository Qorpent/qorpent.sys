using System.Collections.Generic;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Application {
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
}
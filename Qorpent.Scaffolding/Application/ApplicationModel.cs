using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// Обобщенная модель приложения
    /// </summary>
    public class ApplicationModel {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationModel() {
            IsValid = true;
            Actions = new Dictionary<string, AppAction>();
            Structs = new Dictionary<string, AppStruct>();
            Controllers = new Dictionary<string, AppController>();
            Menus = new Dictionary<string, AppMenu>();
            Layouts = new Dictionary<string, AppLayout>();
            Errors = new List<BSharpError>();
            Services = new Dictionary<string, AppService>();
        }

        /// <summary>
        /// Setup model for context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public ApplicationModel Setup(IBSharpContext context, string projectName = null) {
            this.Context = context;
            if (null != projectName) {
                this.ProjectName = projectName;
            }
            else {
                this.ProjectName = "default_project_name";
            }
            SetupActions(context);
            SetupStructs(context);
            SetupLayout(context);
            SetupControllers(context);
            SetupMenus(context);
            SetupServices(context);
            Bind();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProjectName { get; set; }

        private void Bind() {
            foreach (var s in Structs.Values.ToArray()) {
                s.Bind();
            }
            foreach (var a in Actions.Values.ToArray()) {
                a.Bind();
            }
            foreach (var a in Layouts.Values.ToArray()) {
                a.Bind();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IBSharpContext Context { get; set; }

        private void SetupActions(IBSharpContext context) {
            var actionClasses = context.ResolveAll("ui-action").ToArray();
            foreach (var actionClass in actionClasses) {
                var action = new AppAction().Setup(this, actionClass);
                Actions[action.Code] = action;
            }
        }

        private void SetupStructs(IBSharpContext context) {
            var structClasses = context.ResolveAll("ui-data").ToArray();
            foreach (var structClass in structClasses) {
                var structure = new AppStruct().Setup(this, structClass);
                Structs[structure.Code] = structure;
            }
        }

        private void SetupLayout(IBSharpContext context) {
            var layoutClasses = context.ResolveAll("ui-layout").ToArray();
            foreach (var layoutClass in layoutClasses) {
                var layout = new AppLayout().Setup(layoutClass.Compiled, null, this);
                Layouts[layout.Code] = layout;
            }
        }

        private void SetupControllers(IBSharpContext context) {
            var controllerClasses = context.ResolveAll("ui-controller").ToArray();
            foreach (var controllerClass in controllerClasses) {
                var controller = new AppController().Setup(this, controllerClass);
                if (null != controller.MenuItems) {
                    foreach (var m in controller.MenuItems) {
                        var menu = Menus[m.Code];
                        if (null != menu) {
                            controller.Menus.Add(menu);
                        }
                    }   
                }
                Controllers[controller.Code] = controller;
            }
        }

        private void SetupMenus(IBSharpContext context) {
            var menuClasses = context.ResolveAll("ui-menu").ToArray();
            foreach (var menuClass in menuClasses) {
                var menu = new AppMenu().Setup(menuClass.Compiled);
                Menus[menu.Code] = menu;
            }
        }

        private void SetupServices(IBSharpContext context) {
            var serviceClasses = context.ResolveAll("ui-service").ToArray();
            foreach (var serviceClass in serviceClasses) {
                var service = new AppService().Setup(serviceClass.Compiled);
                Services[service.Code] = service;
            }
        }

        /// <summary>
        /// Действия
        /// </summary>
        public IDictionary<string, AppAction> Actions { get; private set; }

        /// <summary>
        /// Сервисы
        /// </summary>
        public IDictionary<string, AppService> Services { get; private set; }

        /// <summary>
        /// Структуры
        /// </summary>
        public IDictionary<string, AppStruct> Structs { get; private set; }

        /// <summary>
        /// Лаяуты
        /// </summary>
        public IDictionary<string, AppLayout> Layouts { get; private set; }

        /// <summary>
        /// Контроллеры
        /// </summary>
        public IDictionary<string, AppController> Controllers { get; private set; }

        /// <summary>
        /// Менюхи
        /// </summary>
        public IDictionary<string, AppMenu> Menus { get; private set; }

        /// <summary>
        /// Дайджест-описание модели
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var result = new StringBuilder();
            foreach (var structs in Structs.Values.OrderBy(_ => _.Code)) {
                result.AppendLine(structs.ToString());
            }
            foreach (var action in Actions.Values.OrderBy(_=>_.Code)) {
                result.AppendLine(action.ToString());
            }
            return result.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public void RegisterError(BSharpError error) {
            if (error.Level >= ErrorLevel.Error) {
                this.IsValid = false;
            }
            Errors.Add(error);
            if (null != Context) {
                Context.RegisterError(error);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<BSharpError> Errors { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public T Resolve<T>(string reference) where T:class {
            if (typeof (T) == typeof (AppStruct)) {
                return Structs.Values.FirstOrDefault(_ => _.Code == reference || _.Class.FullName == reference) as T;
            }
            if (typeof(T) == typeof(AppAction)) {
                return Actions.Values.FirstOrDefault(_ => _.Code == reference || _.Class.FullName == reference) as T;
            }
            if (typeof(T) == typeof(AppMenu)) {
                return Menus.Values.FirstOrDefault(_ => _.Code == reference || _.Class.FullName == reference) as T;
            }
            if (typeof(T) == typeof(AppLayout)) {
                return Layouts.Values.FirstOrDefault(_ => _.Code == reference || _.Class.FullName == reference) as T;
            }
            if (typeof(T) == typeof(AppController)) {
                return Controllers.Values.FirstOrDefault(_ => _.Code == reference || _.Class.FullName == reference) as T;
            }
            if (typeof(T) == typeof(AppService)) {
                return Services.Values.FirstOrDefault(_ => _.Code == reference || _.Class.FullName == reference) as T;
            }
            throw new NotSupportedException(typeof(T).Name+" "+reference);
        }
    }
}

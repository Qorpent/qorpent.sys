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
            Errors = new List<BSharpError>();
        }

        /// <summary>
        /// Setup model for context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ApplicationModel Setup(IBSharpContext context) {
            this.Context = context;
            SetupActions(context);
            SetupStructs(context);
            Bind();
            return this;
        }

        private void Bind() {
            foreach (var s in Structs.Values.ToArray()) {
                s.Bind();
            }
            foreach (var a in Actions.Values.ToArray()   ) {
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

        /// <summary>
        /// Действия
        /// </summary>
        public IDictionary<string, AppAction> Actions { get; private set; }

        /// <summary>
        /// Действия
        /// </summary>
        public IDictionary<string, AppStruct> Structs { get; private set; }

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
            throw new NotSupportedException(typeof(T).Name+" "+reference);
        }
    }
}

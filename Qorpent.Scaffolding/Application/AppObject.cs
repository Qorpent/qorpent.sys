using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AppObject<T> where T:AppObject<T> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        public virtual T Setup(ApplicationModel model, IBSharpClass cls) {
            this.Class = cls;
            this.Model = model;
            this.Code = Class.Name;
            this.Name = Class.Compiled.GetName();
            return (T)this;
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationModel Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IBSharpClass Class { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Читаемое имя, камент
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0} {1} '{2}'", GetType().Name, Code,Name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributeNames"></param>
        /// <returns></returns>
        protected string Resolve(params string[] attributeNames) {
            return Class.Compiled.ChooseAttr(attributeNames);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Bind(){}
    }
}
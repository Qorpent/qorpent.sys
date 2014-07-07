using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
/// 
/// </summary>
    public abstract class AppObjectBase {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AppObject<T> : AppObjectBase where T:AppObject<T> {
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
        /// <param name="el"></param>
        /// <returns></returns>
        public virtual T Setup(XElement el) {
            ApplyAttributes(el);
            return (T) this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        public void ApplyAttributes(XElement el) {
            this.Attributes = new Dictionary<string, string>();
            el.Apply(this);
            foreach (var attr in el.Attributes()) {
                this.Attributes.Add(attr.Name.LocalName, attr.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="el"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual T Setup(XElement el, T parent) {
            ApplyAttributes(el);
            this.Parent = parent;
            return (T)this;
        }

        /// <summary>
        /// Ссылка на родительский объект (используется, например для лаяута)
        /// </summary>
        public T Parent { get; set; }

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
        public string Order { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Grow { get; set; }

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
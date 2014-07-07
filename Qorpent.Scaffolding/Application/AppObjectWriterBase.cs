using System.IO;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    public abstract class AppObjectWriterBase<T> where T:AppObject<T> {
        private T _obj;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public virtual AppObjectWriterBase<T> Setup(T obj) {
            this._obj = obj;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        public abstract void Write(TextWriter output);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var sw = new StringWriter();
            Write(sw);
            return sw.ToString();
        }
    }
}
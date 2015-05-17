using System.IO;
using System.Reflection;

namespace Qorpent.Experiments {
    /// <summary>
    /// 
    /// </summary>
    public interface ISerializationAnnotator {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        SerializeMode GetMode(object target, PropertyInfo info);

        bool Prepend(object data, TextWriter output);
    }
}
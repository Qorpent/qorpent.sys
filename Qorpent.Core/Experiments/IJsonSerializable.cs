using System.IO;

namespace Qorpent.Experiments {
    /// <summary>
    /// 
    /// </summary>
    public interface IJsonSerializable {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="annotator"></param>
        void Write(TextWriter output, ISerializationAnnotator annotator);
    }
}
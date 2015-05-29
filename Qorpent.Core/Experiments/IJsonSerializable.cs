using System.IO;
using Qorpent.Core.Tests.Experiments;

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
        void Write(TextWriter output, string mode, ISerializationAnnotator annotator);
    }

    public interface IJsonDeserializable {
        void LoadFromJson(object jsonsrc);
    }

    public interface IJsonSerializationExtension {
        void WriteExtensions(JsonWriter writer, string mode, ISerializationAnnotator annotator);
        void ReadExtensions(object jsonsrc);
    }
}
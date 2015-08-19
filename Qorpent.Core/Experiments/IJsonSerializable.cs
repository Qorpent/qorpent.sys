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
        /// <param name="mode"></param>
        /// <param name="annotator"></param>
        /// <param name="pretty"></param>
        /// <param name="level"></param>
        void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0);
    }

    public interface IJsonDeserializable {
        void LoadFromJson(object jsonsrc);
    }

    public interface IJsonSerializationExtension {
        void WriteExtensions(JsonWriter writer, string mode, ISerializationAnnotator annotator);
        void ReadExtensions(object jsonsrc);
    }
}
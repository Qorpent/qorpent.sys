using System;
using System.IO;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;

namespace Qorpent {
    public class OperationResult:IJsonSerializable {
        public bool ok { get; set; }
        public string message { get; set; }
        public Exception error { get; set; }
        public object data { get; set; }
        public void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0) {
            var jw = new JsonWriter(output, pretty: pretty, level: level);
            jw.OpenObject();
            jw.WriteProperty(nameof(ok),ok);
            jw.WriteProperty(nameof(message), message,true);
            jw.WriteProperty(nameof(error), error,true);
            jw.WriteProperty(nameof(data), data, true);
            jw.CloseObject();
        }
    }
}
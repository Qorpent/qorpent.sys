using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.XsltConsole {
    /// <summary>
    /// Параметры для Xslt-консоли
    /// </summary>
    public class XsltConsoleParameters : ConsoleApplicationParameters {
        /// <summary>
        /// Имя исходного файла
        /// </summary>
        public string SourceFile {
            get { return ResolveFileName(".xml","src", "s", "arg1"); } 
        }
        /// <summary>
        /// Имя файла XSLT
        /// </summary>
        public string XsltFile {
            get { return ResolveFileName(".xslt", "xslt", "x", "arg2"); } 
        }
        /// <summary>
        /// Имя файла для записи результата
        /// </summary>
        public string OutFile
        {
            get { return ResolveFileName("", "out", "o", "arg3"); }
        }

        private IDictionary<string, string> _xsltParameters;
        /// <summary>
        /// Расширенные параметры XSLT
        /// </summary>
        public IDictionary<string, string> XsltParameters {
            get {
                if (null == _xsltParameters) {
                    _xsltParameters = new Dictionary<string, string>();
                    foreach (var pair in this) {
                        if (pair.Key.StartsWith("set")) {
                            _xsltParameters[pair.Key.Substring(3)] = pair.Value.ToStr();
                        }
                    }
                }
                return _xsltParameters;
            }
        }
        /// <summary>
        /// Валидизирует параметры вызова
        /// </summary>
        protected override void InternalCheckValid() {
            Assert("invalid arguments",
                CheckFile("source file", SourceFile),
                CheckFile("xslt file", XsltFile));
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void DebugPrintArguments() {
            base.DebugPrintArguments();
            Log.Debug("Xslt specials:");
            Log.Debug(string.Format("{0,-20} : {1}", "Source File", SourceFile));
            Log.Debug(string.Format("{0,-20} : {1}", "Xslt File", XsltFile));
            Log.Debug(string.Format("{0,-20} : {1}", "Out File", OutFile));
            if (XsltParameters.Any()) {
                Log.Debug("Xslt stylesheet parameters");
                foreach (var xsltParameter in XsltParameters) {
                    Log.Debug(string.Format("{0,-20} : {1}", xsltParameter.Key, xsltParameter.Value));
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp {
    /// <summary>
    ///     Производит вызов bsc в консольном режиме
    /// </summary>
    public static class BscHelper {
        /// <summary>
        ///     Выполняет компиляцию директории и/или проекта c возвратом контекста BSharp
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public static IBSharpContext Execute(string dir, string projectname = null) {
            return ParseContext(ExecuteXml(dir, projectname));
        }

        private static IBSharpContext ParseContext(XElement xml) {
            var result = new BSharpContext();
            result.Working = new List<IBSharpClass>();
            foreach (var cls in xml.Elements("cls")) {
                var bcls = new BSharpClass(result);
                bcls.Set(BSharpClassAttributes.Built);
                bcls.Name = cls.Attr("name");
                bcls.Namespace = cls.Attr("ns");
                bcls.Compiled = cls.Elements().First();
                result.Working.Add(bcls);
            }
            foreach (var error in xml.Elements("error")) {
                var e = new BSharpError {Type = error.Attr("type").To<BSharpErrorType>(), Message = error.Value};
                var lexe = error.Element("lexinfo");
                if (null != lexe) {
                    e.LexInfo = new LexInfo(lexe.Attr("file"), lexe.Attr("line").ToInt());
                }
                result.RegisterError(e);
            }
            result.BuildIndexes();
            return result;
        }

        /// <summary>
        ///     Выполняет компиляцию директории и/или проекта с возвратом XML
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="projectname"></param>
        /// <returns></returns>
        public static XElement ExecuteXml(string dir, string projectname = null) {
            var consoleCall = new ConsoleApplicationHandler {
                ExePath = EnvironmentInfo.GetExecutablePath("bsc"),
                Encoding = Encoding.UTF8,
                WorkingDirectory =
                    string.IsNullOrWhiteSpace(dir) ? EnvironmentInfo.RootDirectory : EnvironmentInfo.ResolvePath(dir),
                Arguments = string.Format(" {0} --console-mode", projectname),
                NoWindow = true,
                Timeout = 20000,
                Redirect = true
            };
            var result = consoleCall.RunSync();
            if (result.IsOK) {
                return XElement.Parse(result.Output);
            }
            throw new Exception(result.Error + " " + result.Exception);
        }
    }
}
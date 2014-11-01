using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Qorpent.Utils;

namespace Qorpent.XsltConsole
{
    /// <summary>
    /// Приложение для вызова XSLT трансформаций
    /// </summary>
    public static class Program
    {
        /// <summary/>
        public static int Main(string[] args) {
            return ConsoleApplication.Execute(args, (XsltConsoleParameters _) => {
                var xsl = new XslCompiledTransform();
                _.Log.Debug("Loading xslt...");
                xsl.Load(_.XsltFile,XsltSettings.TrustedXslt,new XmlUrlResolver());
                _.Log.Debug("Xslt ok...");
                using (var s = string.IsNullOrWhiteSpace(_.OutFile) ? Console.Out : new StreamWriter(_.OutFile)) {
                    _.Log.Debug("Prepare args...");
                    var xargs = new XsltArgumentList();
                    foreach (var p in _.XsltParameters  ) {
                        xargs.AddParam(p.Key,"",p.Value);
                    }
                    xargs.AddExtensionObject("qorpent://std",  new XsltStdExtensions());
                    _.Log.Debug("Args ok...");
                    _.Log.Debug("Start transform...");
                    xsl.Transform(_.SourceFile,xargs,s);
                    s.Flush();
                    _.Log.Debug("Transform ok...");
                }
                return 0;
            });
        }
    }
}

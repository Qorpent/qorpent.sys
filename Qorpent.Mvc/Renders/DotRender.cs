using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Qorpent.IO;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Renders {
    /// <summary>
    ///     Render для отрисовки графиков DOT
    /// </summary>
    [Render("dot")]
    public class DotRender : RenderBase {
        /// <summary>
        ///     Параметр указания целевого формата, по умолчанию SVG
        /// </summary>
        public const string FORMATPARAM = "__format";

        /// <summary>
        ///     Параметр указания алгоритма по умолчанию DOT
        /// </summary>
        public const string ALGORITHMPARAM = "__algorithm";

        /// <summary>
        ///     Параметр включения тюнинга графа
        /// </summary>
        public const string TUNEPARAM = "__tune";

        /// <summary>
        ///     Префикс парамтеров перекрытия настрок графа
        /// </summary>
        public const string OVERRIDEPARAMPREFIX = "g.";

        private const string ENDOFSVGMARKER = "</svg>";
        private const string PNGENDMARKER = "IEND";
        private const int INITIAL_PNG_BUFFER_SIZE = 8; //size of initial title and start of chank length;
        private const int CHUNK_LENGTH_BLOCK_SIZE = 4;
        private const int CHUNK_TYPE_BLOCK_SIZE = 4;
        private const int CHUNK_SRC_BLOCK_SIZE = 4;

        private string _dotpath;

        /// <summary>
        ///     Renders given context
        /// </summary>
        /// <param name="context"> </param>
        public override void Render(IMvcContext context) {
            GraphOptions options = ExtractOptions(context);
            string dotscript = ExtractDotScript(options);
            context.ContentType = MimeHelper.GetMimeByExtension(options.Format);
            Process p = null;
            string script = dotscript.GetUnicodeSafeXmlString();

            if (options.Format == "dot") {
                context.ContentType = "text/plain";
                context.Output.Write(script);
            }
            else if (options.Format == "rawxml") {
                WriteOutRawXml(context,options);
            }
            else {
                p = GetProcess(options);
                p.Start();
                try {
                    p.StandardInput.WriteLine(script);
                    if (options.Format == GraphOptions.SVGFORMAT) {
                        WriteOutSvg(context, p, options);
                    }
                    else {
                        WriteOutPng(context, p, options);
                    }
                }
                finally {
                    p.StandardInput.Close();
                }
            }
        }

        private void WriteOutRawXml(IMvcContext context, GraphOptions options) {
            var val = context.ActionResult;
            context.ContentType = MimeHelper.GetMimeByExtension("xml");
            if (val is IGraphSource) {
                val = ((IGraphSource) val).BuildGraph(options);
            }
            var xmls = Application.Container.Get<ISerializer>("xml.serializer");
            xmls.Serialize("dot.xml", val, context.Output);

        }

        private static GraphOptions ExtractOptions(IMvcContext context) {
            string format = context.Get(FORMATPARAM, GraphOptions.SVGFORMAT);
            string algorithm = context.Get(ALGORITHMPARAM, GraphOptions.DOTAGORITHM);
            bool tune = context.Get(TUNEPARAM, true);
            IEnumerable<KeyValuePair<string, string>> overrides = context.GetAll(OVERRIDEPARAMPREFIX);
            var options = new GraphOptions {
                Tune = tune,
                Algorithm = algorithm,
                Format = format,
                Dialect = GraphOptions.DOTDIALECT
            };
            foreach (var o in overrides) {
                options.OverrideGraphAttributes[o.Key] = o.Value;
            }
            options.Context = context;
            return options;
        }

        private static void WriteOutPng(IMvcContext context, Process p, GraphOptions options) {
            var buffer = new byte[INITIAL_PNG_BUFFER_SIZE];
            Stream s = p.StandardOutput.BaseStream;
            int idx = 0;
            idx += s.Read(buffer, 0, INITIAL_PNG_BUFFER_SIZE);
            while (true) {
                int al = 0;
                bool endchunk = false;
                var subbuf = new byte[CHUNK_LENGTH_BLOCK_SIZE];
                s.Read(subbuf, 0, CHUNK_LENGTH_BLOCK_SIZE);
                uint length = BitConverter.ToUInt32(subbuf.Reverse().ToArray(), 0);
                int expand = CHUNK_LENGTH_BLOCK_SIZE + CHUNK_TYPE_BLOCK_SIZE + (int) length + CHUNK_SRC_BLOCK_SIZE;
                Array.Resize(ref buffer, buffer.Length + expand);
                Array.Copy(subbuf, 0, buffer, idx, subbuf.Length);
                idx += subbuf.Length;
                s.Read(subbuf, 0, CHUNK_TYPE_BLOCK_SIZE);
                string str = Encoding.ASCII.GetString(subbuf);
                if (str == PNGENDMARKER) {
                    endchunk = true;
                }
                Array.Copy(subbuf, 0, buffer, idx, subbuf.Length);
                idx += subbuf.Length;

                idx += al = s.Read(buffer, idx, (int) length);
                if (al < (int) length) {
                    idx += s.Read(buffer, idx, (int) length - al);
                }
                idx += s.Read(buffer, idx, CHUNK_SRC_BLOCK_SIZE);
                if (endchunk) {
                    break;
                }
            }
            context.WriteOutBytes(buffer);
        }

        private static void WriteOutSvg(IMvcContext context, Process p, GraphOptions options) {
            string result = "";
            string line = "";
            while (true) {
                line = p.StandardOutput.ReadLine();
                result += line;
                if (line.Contains(ENDOFSVGMARKER)) break;
            }
            if (options.Context.ActionResult is IGraphSource) {
                XElement xml = XElement.Parse(result);
                xml = ((IGraphSource) options.Context.ActionResult).PostprocessGraphSvg(xml, options);
                context.Output.Write(xml.ToString());
            }
            else {
                context.Output.Write(result);
            }
        }

        private Process GetProcess(GraphOptions options) {
            string callargs = "-T" + options.Format;
            var process_start = new ProcessStartInfo(ResolveDotPath(options), callargs) {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                StandardOutputEncoding = Encoding.UTF8
            };
            var p = new Process {StartInfo = process_start};
            return p;
        }


        /// <summary>
        ///     Renders error, occured in given context
        /// </summary>
        /// <param name="error"> </param>
        /// <param name="context"> </param>
        public override void RenderError(Exception error, IMvcContext context) {
            context.ContentType = "text/plain";
            context.Output.Write(error.ToString());
        }

        private string ResolveDotPath(GraphOptions options) {
            if (null == _dotpath) {
                string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
                foreach (string p in paths) {
                    string realp = Environment.ExpandEnvironmentVariables(p.Trim());
                    string checkpath = Path.Combine(realp, options.Algorithm + ".exe");
                    if (File.Exists(checkpath)) {
                        _dotpath = realp;
                    }
                }
            }
            if (null != _dotpath) {
                return Path.Combine(_dotpath, options.Algorithm + ".exe");
            }
            throw new Exception("cannot find " + options.Algorithm + ".exe");
        }

        private string ExtractDotScript(GraphOptions options) {
            object result = options.Context.ActionResult;
            string dotscript = "";
            if (null == result) {
                dotscript = "digraph N{ null }";
            }
            else if (result is string) {
                dotscript = (string) result;
            }
            else if (result is IGraphConvertible) {
                dotscript = ((IGraphConvertible) result).GenerateGraphScript(options);
            }
            else if (result is IGraphSource) {
                dotscript = ((IGraphSource) result).BuildGraph(options).GenerateGraphScript(options);
            }
            else {
                dotscript = ConvertToDotScript(result);
            }
            return dotscript;
        }

        private string ConvertToDotScript(object result) {
            throw new NotImplementedException("for now any-to-dot mode is not realized");
        }
    }
}
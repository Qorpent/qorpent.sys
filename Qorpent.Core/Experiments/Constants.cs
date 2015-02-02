using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#if EXBRIDGE
namespace Qorpent.Experiments.Utils
#else
namespace Qorpent.Utils
#endif
{
    /// <summary>
    /// Contains common constants, used by utils
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Base datetime value for unix-compatible data 1970-01-01
        /// </summary>
        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1);
        /// <summary>
        /// Minimal Unix - datetime (as Int32)
        /// </summary>
        public static readonly DateTime MinUnixTime = UnixBaseTime.AddSeconds(int.MinValue);
        /// <summary>
        /// Maximal Unix - datetime (as Int32)
        /// </summary>
        public static readonly DateTime MaxUnixTime = UnixBaseTime.AddSeconds(int.MaxValue);
        /// <summary>
        /// Usable min date that used as logical zero/null for dates
        /// </summary>
        public static readonly DateTime MinDateTime = new DateTime(1900,1,1);
        /// <summary>
        /// Usable max date that used as logical infinity/null for dates
        /// </summary>
        public static readonly DateTime MaxDateTime = new DateTime(3000,1,1);

        /// <summary>
        /// 	Standard date formats for usage when standard DateTime.Parse fails
        /// 	most of cases are of Ru style and It sortable ones
        /// </summary>
        public static readonly string[] StandardDateFormats = new[]
				{
					"dd.MM.yyyy HH:mm:ss","dd.MM.yyyy HH:mm", "dd.MM.yyyy", "yyyy-MM-dd HH:mm","yyyy-MM-dd HH:mm:ss","yyyyMMdd HH:mm","yyyyMMdd","yyyyMMdd HH:mm:ss",
					"yyyy-MM-dd", "yyyyMMddHHmm", "yyyyMMddHHmmss", "yyyy-MM-ddTHH:mm:ss.fffZ", "dd-MM-yyyy"
				};
        /// <summary>
        /// 	Пространство имен для расширений XmlInclude - import,include, применяется в <see
        /// 	 href="Qorpent.Dsl~Qorpent.Dsl.XmlIncludeProcessor" />
        /// </summary>
        public const string XmlIncludeNamespace = "http://qorpent/xml/include";

        /// <summary>
        /// 	oficial XSLT namespace
        /// </summary>
        public const string XsltNameSpace = "http://www.w3.org/1999/XSL/Transform";

        /// <summary>
        /// </summary>
        public const string CSharpDslExtensionNameSpace = "http://qorpent/dsl/csharp";
        /// <summary>
        /// 	Пространство имен для расширений SmartXslt - import,include,param,extension, применяется в <see
        /// 	 href="Qorpent.Dsl~Qorpent.Dsl.SmartXslt.XsltHelper" />
        /// </summary>
        public const string SmartXsltNamespace = "http://qorpent/xml/xslt/extensions";
        /// <summary>
        /// 	namespaces and prefixes which can processed implicitly
        /// </summary>
        public static readonly IDictionary<string, string> WellKnownNamespaces
            = new Dictionary<string, string>
					{
						{XsltNameSpace, "xsl"},
						{XmlIncludeNamespace, "qxi"},
						{SmartXsltNamespace, "qsx"},
					};
    }
}

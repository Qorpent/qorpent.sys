using System.IO;
using System.Text;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Bxl;
using Qorpent.IO;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Класс считывания конфигов из файлов приложения
    /// </summary>
    public class LoadConfigReader : ILoadConfigReader {
        /// <summary>
        /// Объект резолюции файлов
        /// </summary>
        public IFileNameResolver Resolver { get; set; }
        /// <summary>
        /// Парсер BXL
        /// </summary>
        public IBxlParser Parser { get; set; }
        private string[] GetLoadFiles() {
            if (null == Resolver) Resolver = Application.Current.Files;
            var manifestfiles = Resolver.ResolveAll(
                new FileSearchQuery
                {
                    ExistedOnly = true,
                    PathType = FileSearchResultType.FullPath,
                    ProbeFiles = new[] { "*.ui-load.bxl" },
                    ProbePaths = new[] {  "~/.config" }
                });
            return manifestfiles;
        }
        /// <summary>
        /// Загружест BXL конфигурацию приложения
        /// </summary>
        /// <returns></returns>
        public XElement LoadConfig() {
            if (null == Parser) Parser = Application.Current.Bxl;
            var allfiles = GetLoadFiles();
            var totalfile = new StringBuilder();
            foreach (var file in allfiles) {
                totalfile.AppendLine(File.ReadAllText(file));
            }
            return Parser.Parse(totalfile.ToString(), "app.config");
        }
    }
}
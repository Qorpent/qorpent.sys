using System.IO;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Генерирует основной набор скриптов
    /// </summary>
    public class LoadFileGenerator : ILoadFileGenerator {
        /// <summary>
        /// Загрузчик конфигурации
        /// </summary>
        public ILoadConfigReader ConfigReader;
        /// <summary>
        /// Парсер пакетов
        /// </summary>
        public ILoadPackageReader PackageReader;
        /// <summary>
        /// Генератор скриптов
        /// </summary>
        public ILoadScriptGenerator ScriptGenerator;
        /// <summary>
        /// Резольвер путей
        /// </summary>
        public IFileNameResolver Resolver;
        /// <summary>
        /// Формирует на диске готовые к загрузке JS файлы
        /// </summary>
        public void Generate(string rootdir = "~/.tmp/",string template= "load.{0}.js") {
            ConfigReader = ConfigReader ?? new LoadConfigReader();
            PackageReader = PackageReader ?? new LoadPackageReader();
            ScriptGenerator = ScriptGenerator ?? new LoadScriptGenerator();
            Resolver = Resolver ?? Application.Current.Files;

            var targetdir = Resolver.Resolve(rootdir);
            Directory.CreateDirectory(targetdir);
            var guestfile = Path.Combine(targetdir, string.Format(template,LoadLevel.Guest.ToString().ToLower()));
            var authfile = Path.Combine(targetdir, string.Format(template, LoadLevel.Auth.ToString().ToLower()));
            var adminfile = Path.Combine(targetdir, string.Format(template, LoadLevel.Admin.ToString().ToLower()));

            var config = ConfigReader.LoadConfig();
            var packages = PackageReader.Read(config);

            var normalpackages = new LoadPackageSet(packages);

            File.WriteAllText(guestfile,ScriptGenerator.Generate(normalpackages[LoadLevel.Guest]));
            File.WriteAllText(authfile, ScriptGenerator.Generate(normalpackages[LoadLevel.Auth]));
            File.WriteAllText(adminfile, ScriptGenerator.Generate(normalpackages[LoadLevel.Admin]));

        }
    }
}
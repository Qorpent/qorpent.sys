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
        public void Generate() {
            ConfigReader = ConfigReader ?? new LoadConfigReader();
            PackageReader = PackageReader ?? new LoadPackageReader();
            ScriptGenerator = ScriptGenerator ?? new LoadScriptGenerator();
            Resolver = Resolver ?? Application.Current.Files;

            var targetdir = Resolver.Resolve("~/.tmp/");
            Directory.CreateDirectory(targetdir);
            var guestfile = Path.Combine(targetdir, "__quest.load.js");
            var authfile = Path.Combine(targetdir, "__auth.load.js");
            var adminfile = Path.Combine(targetdir, "__admin.load.js");

            var config = ConfigReader.LoadConfig();
            var packages = PackageReader.Read(config);

            var normalpackages = new LoadPackageSet(packages);

            File.WriteAllText(guestfile,ScriptGenerator.Generate(normalpackages[LoadLevel.Guest]));
            File.WriteAllText(authfile, ScriptGenerator.Generate(normalpackages[LoadLevel.Auth]));
            File.WriteAllText(adminfile, ScriptGenerator.Generate(normalpackages[LoadLevel.Admin]));

        }
    }
}
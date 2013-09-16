using System;
using System.Threading.Tasks;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Класс сервиса для автоматического формирования файлов
    /// </summary>
    public class LoadService : ILoadService {

        static LoadService() {
            Default = new LoadService();
        }
        /// <summary>
        /// Инстанция по умолчанию
        /// </summary>
        public static ILoadService Default { get; set; }

        /// <summary>
        /// Время последней компиляции
        /// </summary>
        public DateTime LastCompileTime;

        /// <summary>
        /// Внутренний статус нахождения в режиме компиляции
        /// </summary>
        public bool InCompilation;

        /// <summary>
        /// Резольвер файлов
        /// </summary>
        public IFileNameResolver Resolver;



        private Task CompileTask;
        /// <summary>
        /// Генератор файлов
        /// </summary>
        public ILoadFileGenerator Generator;
        /// <summary>
        /// Корневая директория для скриптов
        /// </summary>
        public string RootDir = "~/.tmp/";
        /// <summary>
        /// Шаблон имени файлов
        /// </summary>
        public string Template = "load.{0}.js";

        /// <summary>
        /// Метод синхронизации на чтение/доступ к файлам скрипта
        /// </summary>
        public void Synchronize() {
            if (null != CompileTask && !CompileTask.IsFaulted && !CompileTask.IsCanceled) {
                CompileTask.Wait();
                return;
            }
            if (LastCompileTime < Application.Current.StartTime) {
                Compile();
                CompileTask.Wait();
                return;
            }
            CompileTask = null;
        }

        /// <summary>
        /// Асинхронно форсирует компиляцию скриптов
        /// </summary>
        public void Compile() {
            Resolver = Resolver ?? Application.Current.Files;
            CompileTask = Task.Run((Action) CompileWorker);
        }

        private void CompileWorker() {
            Generator = Generator ?? new LoadFileGenerator();
            Generator.Generate(RootDir,Template);
            LastCompileTime = DateTime.Now;
        }

        /// <summary>
        /// Получает имя ресурса
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pathtype"></param>
        /// <returns></returns>
        public string GetFileName(LoadLevel level, FileSearchResultType pathtype = FileSearchResultType.FullPath) {
            Resolver = Resolver ?? Application.Current.Files;
            var name = "~/.tmp/load." + level.ToString().ToLower() + ".js";
            return Resolver.Resolve(name, false, pathtype: pathtype);
        }

    }
}
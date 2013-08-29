using Qorpent.BSharp.Builder;
using System;
using System.Collections.Generic;

namespace Qorpent.BSharp.Builder {
    /// <summary>
    ///     Класс ля резольвинга инклудов и эксклудов
    /// </summary>
    public abstract class BSharpBuilderTargetResolverBase : IBSharpBuilderTargetResolver {
        /// <summary>
        ///     Проект
        /// </summary>
        public virtual IBSharpProject Project { get; set; }
        /// <summary>
        ///     Перечисление найденных включений
        /// </summary>
        protected virtual IEnumerable<string> FoundIncludes { get; set; }
        /// <summary>
        ///     Резольвинг  с учётом инклудов и эксклудов
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> Resolve() {
            IsCorrectConfiguration();

            var resolvedIncludes = new List<string>();

            foreach (var target in FoundIncludes) {
                resolvedIncludes.Add(target);

                if (ExcludesMatch(target)) {
                    resolvedIncludes.Remove(target);
                }
            }

            return resolvedIncludes;
        }
        /// <summary>
        ///     проверить путь на соответствие эксклюдам
        /// </summary>
        /// <param name="target">цель для проверки</param>
        /// <returns></returns>
        protected virtual bool ExcludesMatch(string target) {
            return true;
        }
        /// <summary>
        ///     Запись в лог
        /// </summary>
        protected virtual void WriteLog(string message) {
            if (Project.Log == null) {
                return;
            }

            Project.Log.Info("TargetResolver: [" + message + "]");
        }
        /// <summary>
        ///     Генерирует регулярку для проверкии соответствия
        ///     эксклюдов
        /// </summary>
        /// <param name="target">Единичный эксклюд (маска пути)</param>
        /// <returns>Регулярное выражение</returns>
        protected virtual string GenerateRegex(string target) {
            return new string(String.Empty.ToCharArray());
        }
        /// <summary>
        ///     Проверка корректности конфигурации
        /// </summary>
        protected virtual void IsCorrectConfiguration() {
            if (Project == null) {
                throw new Exception("Incorrect configuration in TargetResolver");
            }

            if (Project == null) {
                throw new Exception("Incorrect configuration in TargetResolver");
            }
        }
        /// <summary>
        ///     Поиск всех вхождений по указанным инклукдам
        /// </summary>
        /// <returns>
        ///     Перечисление путей файлов, подходящих по маске
        ///     к списку инклудов
        /// </returns>
        protected virtual IEnumerable<string> GetIncludedTargets() {
            return new List<string>();
        }
    }
}

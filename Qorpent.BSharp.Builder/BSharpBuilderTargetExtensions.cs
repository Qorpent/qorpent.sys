using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Integration.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public static class BSharpBuilderTargetExtensions {
        /// <summary>
        ///     Возвращает исходное перечисление. Если оно пусто, то
        ///     перечисление с единственным элеметом «*»
        /// </summary>
        /// <param name="enumerator">Исходный енумертор</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateTargets(this IDictionary<string, BSharpBuilderTargetType> enumerator) {
            return (enumerator.Count() != 0) ? (enumerator.Keys) : (new string[] { "*" });
        }
        /// <summary>
        ///     Возвращает перечисление всех элементов с определённым
        ///     типом: включение или исключение
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateTargets(this IDictionary<string, BSharpBuilderTargetType> enumerator, BSharpBuilderTargetType type) {
            foreach(var el in enumerator) {
                if (el.Value == type) {
                    yield return el.Key;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="target">Элемент для записи</param>
        /// <param name="type">Тип: включение или выключение</param>
        public static void AppendTarget(this IDictionary<string, BSharpBuilderTargetType> enumerator, string target, BSharpBuilderTargetType type) {
            if (enumerator.ContainsKey(target)) {
                enumerator[target] = type;
            } else {
                enumerator.Add(target, type);
            }
        }
        /// <summary>
        ///     Проверить существование цели без учёта типа включения
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool TargetExists(this IDictionary<string, BSharpBuilderTargetType> enumerator, string target) {
            return enumerator.ContainsKey(target);
        }
        /// <summary>
        ///     Проверить существование цели с учётом типа включения
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool TargetExists(this IDictionary<string, BSharpBuilderTargetType> enumerator, string target, BSharpBuilderTargetType type) {
            if (!enumerator.TargetExists(target)) {
                return false;
            }

            return (enumerator[target] == type);
        }
<<<<<<< HEAD
=======
        /// <summary>
        ///     Удаление цели из перечисления
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="target"></param>
        public static void RemoveTarget(this IDictionary<string, BSharpBuilderTargetType> enumerator, string target) {
            if (enumerator.TargetExists(target)) {
                enumerator.Remove(target);
            }
        }
>>>>>>> b3303b9... Incorrect including all fixed
    }
}

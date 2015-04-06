using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Utils.Extensions {
    /// <summary>
    ///     Коллекция расширений для тасков
    /// </summary>
    public static class TaskExtensions {
        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T GetResult<T>(this Task<T> task) {
            task.Wait();
            return task.Result;
        }
        /// <summary>
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetResult<T>(this IEnumerable<Task<T>> tasks) {
            Task.WaitAll(tasks.ToArray());
            return tasks.Select(_ => _.Result);
        }
    }
}

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Qorpent.Log;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Integration.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public class BSharpBuilderWriteManager {
		static ConcurrentBag<Task> Pending = new ConcurrentBag<Task>();
		/// <summary>
		/// 
		/// </summary>
		public static void Join(){
			Task.WaitAll(Pending.ToArray());
			Pending = new ConcurrentBag<Task>();
		}
        /// <summary>
        ///     Класс для логгирования
        /// </summary>
        public IUserLog Log { get; set; }
        /// <summary>
        ///     Плоский массив из целей для записи
        /// </summary>
        private ConcurrentBag<BSharpBuilderWriteTarget> _targets;
        /// <summary>
        ///     Акцессор для получения целей на запись
        /// </summary>
        public ConcurrentBag<BSharpBuilderWriteTarget> Targets {
            get { return _targets;  }
        }
        /// <summary>
        ///     Возвращает перечисление целей
        /// </summary>
        /// <returns>Перечисление целей</returns>
        public IEnumerable<BSharpBuilderWriteTarget> EnumerateTargets {
            get { return _targets; }
        }
        /// <summary>
        /// 
        /// </summary>
        public BSharpBuilderWriteManager() {
            _targets = new ConcurrentBag<BSharpBuilderWriteTarget>();
        }
        /// <summary>
        ///     Прокатывает запись на диск всех целей
        /// </summary>
        public void Roll() {
            
            foreach (var target in _targets) {
                Pending.Add(RollTarget(target));
            }
        }
        /// <summary>
        ///     Добавление цели
        /// </summary>
        /// <param name="target">Цель</param>
        public void Add(BSharpBuilderWriteTarget target) {
	        _targets.Add(target);
        }
        /// <summary>
        ///     Производит реальную запись целевого файла на диск
        /// </summary>
        /// <param name="target">Целевой файл</param>
        private async Task RollTarget(BSharpBuilderWriteTarget target){
            if (!Directory.Exists(target.Directory)) {
                Directory.CreateDirectory(target.Directory);
            }

			using (var sw = new StreamWriter(target.Path)){
				await sw.WriteAsync(target.Entity.ToString());	
			}
            WriteLog("Wrote: " + target.Path);
        }
        /// <summary>
        ///     Поиск в локальном хранилище аналогичной цели
        /// </summary>
        /// <param name="pattern">Шаблон поиска</param>
        /// <returns>Цель</returns>
        private BSharpBuilderWriteTarget FindSimilarTarget(BSharpBuilderWriteTarget pattern) {
            foreach (var target in _targets) {
                if (target.Directory != pattern.Directory) {
                    continue;
                }

                if (target.Filename != pattern.Filename) {
                    continue;
                }

                if (target.Extension != pattern.Extension) {
                    continue;
                }

                return target;
            }

            return null;
        }
        /// <summary>
        ///     Запись в лог
        /// </summary>
        /// <param name="message">Мессэдж</param>
        private void WriteLog(string message) {
            if (Log != null) {
                Log.Trace(message);
            }
        }

        public void SingleFile()
        {
            var etalong = Targets.FirstOrDefault();
            if (null == etalong) return;
            if (!Directory.Exists(etalong.Directory))
            {
                Directory.CreateDirectory(etalong.Directory);
            }
            XElement x = null;
            foreach (var target in Targets)
            {
                var ent = target.Entity;
                if (null == x)
                {
                    x = ent;
                }
                else
                {
                    x.Add(ent.Elements());
                }
            }
            using (var sw = new StreamWriter(etalong.Path))
            {
                sw.Write(x.ToString());
            }
            WriteLog("Wrote: " + etalong.Path);
        }
    }
}

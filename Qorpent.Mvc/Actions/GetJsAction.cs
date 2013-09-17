using System;
using System.IO;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Возвращает javascript с контролем ETag
	/// </summary>
    [Action("_sys.getjs", Role = "DEFAULT", Help = "Возвращает javascript с контролем ETag")]
	public class GetJsAction : ActionBase {
		private string _filename;


		/// <summary>
        /// 	Инициализация - первая фаза запуска Действия. Перекрывается при необходимости дополнительной обработки входных параметров. 
		/// </summary>
		protected override void Initialize() {
			base.Initialize();
			EvalScriptPath();
		}

		private void EvalScriptPath() {
			_filename = FileNameResolver.Resolve("~/scripts/" + ScriptName + ".js");
		}

		/// <summary>
        /// Вторая фаза - проверка входных параметров, параметров запроса (вызывается до стадии подготовки, так что не
        /// пытайтесь проверить авторизацию или что либо кроме входных параметров)
        /// </summary>
		protected override void Validate()
		{
			if (string.IsNullOrWhiteSpace(_filename)) {
				throw new Exception("script not exists");
			}
		}

		/// <summary>
        /// Перекрываем, если Yr action возвращает 304 статус и True
		/// </summary>
		/// <returns> </returns>
		protected override bool GetSupportNotModified() {
			return true;
		}

		/// <summary>
        /// Перекрываем, если Yr action возвращает 304 статус и заголовок Last-Modified-Stateer
		/// </summary>
		/// <returns> </returns>
		protected override DateTime EvalLastModified() {
			EvalScriptPath();
			if (string.IsNullOrWhiteSpace(_filename)) return DateTime.Now;
			return File.GetLastWriteTime(_filename);
		}

		
		/// <summary>
        /// Основная фаза - тело действия
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			return _filename;
		}
		/// <summary>
		/// Имя скрипта
		/// </summary>
		[Bind(true)] protected string ScriptName { get; set; }

	}
}
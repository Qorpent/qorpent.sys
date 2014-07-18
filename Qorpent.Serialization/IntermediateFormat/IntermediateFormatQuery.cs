using System.Collections.Generic;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.Mvc;

namespace Qorpent.IntermediateFormat{
	/// <summary>
	/// Запрос на промежуточный формат
	/// </summary>
	public  class IntermediateFormatQuery : ConfigBase,IConfigurableFromDictionary{
		/// <summary>
		///		Определяет, является ли запрос корректным
		/// </summary>
		/// <returns>Признак корректности запроса</returns>
		public virtual ValidationResult GetIsValid(){
			return ValidationResult.OK;
		}

		/// <summary>
		///		Формат
		/// </summary>
		public IntermediateFormatOutputType Format { 
			get { return Get<IntermediateFormatOutputType>("format"); }
			set { Set("format",value); }
		}

		/// <summary>
		/// Уровень ведения журнала обработки запроса
		/// </summary>
		public LogLevel LogLevel {
			get { return Get<LogLevel>("loglevel"); }
			set { Set("loglevel", value); }
		}

		/// <summary>
		/// Формат ведения журнала обработки запроса
		/// </summary>
		public string LogFormat {
			get { return Get<string>("logformat"); }
			set { Set("logformat", value); }
		}

		/// <summary>
		/// Формат ведения журнала обработки запроса
		/// </summary>
		public IntermediateFormatLayer Layers
		{
			get { return Get<IntermediateFormatLayer>("layers"); }
			set { Set("layers", value); }
		}
		/// <summary>
		/// Заносит недообработанные парамтеры в конфигурацию
		/// </summary>
		/// <param name="args"></param>
		public virtual void Setup(IDictionary<string, string> args){
			foreach (var arg in args){
				var key = arg.Key.ToLowerInvariant();
				if (!Exists(key)){
					Set(key,arg.Value);
				}
			}
		}
	}
}
using System;
using System.Runtime.Serialization;
using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Описание конфига чарта
    /// </summary>
    public interface IChartConfig : IConfig {
        /// <summary>
        /// Тип чарта
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// Ширина чарта
        /// </summary>
        string Width { get; set; }
        /// <summary>
        /// Высота чарта
        /// </summary>
        string Height { get; set; }
        /// <summary>
        /// Режим отладки чарта
        /// </summary>
        string Debug { get; set; }
        /// <summary>
        /// Id чарта
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Контейнер чарта
        /// </summary>
        string Container { get; set; }
        /// <summary>
        /// Рендер чарта средствами svg
        /// </summary>
        bool JavaScriptRender { get; set; }
        /// <summary>
        /// Тип данных графика
        /// </summary>
        string DataType { get; set; }
        /// <summary>
        ///     Требуется ли подгонка максимальных и минимальных значений осей
        /// </summary>
        bool FitAxis { get; set; }
        /// <summary>
        ///     Признак того, что нужно использовать скалинг значений по умолчанию.
        /// </summary>
        bool UseDefaultScaling { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string MaxValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string MinValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ChartShowValuesAs ShowValuesAs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int Divlines { get; set; }
        /// <summary>
        ///     Признак того, нужно ли удерживать шапку
        /// </summary>
        bool KeepHead { get; set; }
		/// <summary>
		/// Состояние графика
		/// </summary>
	    ChartState State { get; set; }
		/// <summary>
		/// Нативный результат
		/// </summary>
	    object NativeResult { get; set; }
    }

	/// <summary>
	/// Описатель состояния графика
	/// </summary>
	public class ChartState {
	/// <summary>
	/// 
	/// </summary>
		public ChartState() {
			
		}


	
		/// <summary>
		/// Общая степень готовности и адекватности графика
		/// </summary>
		public ErrorLevel Level { get; set; }
		/// <summary>
		///		Экзепшен, если есть
		/// </summary>
		public Exception Exception { get; set; }
		/// <summary>
		///		Заголовок, сопровождающий график
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		///		Сообщение, сопровождающее график
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		///		Признак того, что всё нормально и можно рисовать
		/// </summary>
		public bool IsNormal {
			get { return Exception == null && Level <= ErrorLevel.Warning; }
		}
	}
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ChartException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//
		/// <summary>
		/// 
		/// </summary>
		public ChartException() {
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="config"></param>
		public ChartException(string message, IChartConfig config) : base(message) {
			this.Config = config;
		}
		/// <summary>
		/// 
		/// </summary>
		public IChartConfig Config { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="config"></param>
		/// <param name="inner"></param>
		public ChartException(string message, IChartConfig config, Exception inner) : base(message, inner) {
			Config = config;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ChartException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
	/// <summary>
    /// 
    /// </summary>
    public enum ChartShowValuesAs {
        /// <summary>
        ///     Как есть
        /// </summary>
        AsIs,
        /// <summary>
        ///     Десятки
        /// </summary>
        Tens,
        /// <summary>
        ///     сотни
        /// </summary>
        Hundreds,
        /// <summary>
        ///     Тысячи
        /// </summary>
        Thousands,
        /// <summary>
        ///     Десятки тысяч
        /// </summary>
        TensOfThousands,
        /// <summary>
        ///     Сотни тысяч
        /// </summary>
        HundredsOfThousands,
        /// <summary>
        ///     Миллионы
        /// </summary>
        Millions
    }
}

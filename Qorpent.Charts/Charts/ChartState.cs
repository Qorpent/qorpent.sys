using System;
using System.Collections.Generic;
using Qorpent.Serialization;

namespace Qorpent.Charts {
	/// <summary>
	///		Описатель состояния графика
	/// </summary>
	public class ChartState {
		/// <summary>
		///		Описатель состояния графика
		/// </summary>
		public ChartState() {
			Properties = new Dictionary<string, string>();
		}
		/// <summary>
		/// 
		/// </summary>
		[SerializeNotNullOnly]
		public Dictionary<string, string> Properties { get; private set; }
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
}
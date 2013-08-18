using System;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Дескриптор класса
	/// </summary>
	public class BSharpRuntimeClassDescriptor {
		/// <summary>
		/// Полное имя
		/// </summary>
		public string Fullname { get; set; }
		/// <summary>
		/// Время последнего изменения
		/// </summary>
		public DateTime LastWrite { get; set; }

		/// <summary>
		/// Имя в ресурсах
		/// </summary>
		public string ResourceName { get; set; }

		/// <summary>
		/// Хэш-запись класса
		/// </summary>
		public string Hash { get; set; }

		/// <summary>
		/// Кэшированный загруженный класс
		/// </summary>
		public IBSharpRuntimeClass CachedClass { get; set; }
	}
}
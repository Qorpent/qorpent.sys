using System;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Data {
	/// <summary>
	/// Описание соединения
	/// </summary>
	[Serialize]
	public class ConnectionDescriptor {
		/// <summary>
		/// Имя соединения
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		///  Строка подключения
		/// </summary>
		[SerializeNotNullOnly]
		public string ConnectionString { get; set; }
		/// <summary>
		/// Тип подключения
		/// </summary>
		public Type ConnectionType { get; set; }
		/// <summary>
		/// Имя подключения в контейнере
		/// </summary>
		[SerializeNotNullOnly]
		public string ContainerName { get; set; }
		/// <summary>
		/// True - использовать контейнер для загрузки
		/// </summary>
		public bool InstantiateWithContainer { get; set; }

		/// <summary>
		/// Ссылка на контейнер
		/// </summary>
		[SerializeNotNullOnly]
		public IContainer Container { get; set; }
	}
}
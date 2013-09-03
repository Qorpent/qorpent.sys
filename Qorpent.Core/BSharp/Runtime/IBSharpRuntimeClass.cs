using System;
using System.Xml.Linq;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Промежуточный интерфейс для описания класса BSharp в runtime
	/// </summary>
	public interface IBSharpRuntimeClass {
		/// <summary>
		///     Имя класса
		/// </summary>
		string Name { get; }

		/// <summary>
		///     Пространство имен
		/// </summary>
		string Namespace { get; }

		/// <summary>
		///     Полное имя
		/// </summary>
		string Fullname { get; }

		/// <summary>
		///     Определение
		/// </summary>
		XElement Definition { get; }

		/// <summary>
		/// Дескриптор рантайм-класса
		/// </summary>
		RuntimeClassDescriptor RuntimeDescriptor { get; }

		/// <summary>
		/// Создает 
		/// </summary>
		object Create();
	}
}
﻿namespace Qorpent.Json {
	/// <summary>
	/// Пара свойство-значене
	/// </summary>
	public class JsonTuple {
		/// <summary>
		/// Имя свойства
		/// </summary>
		public JsonItem Name;
		/// <summary>
		/// Значение свойства
		/// </summary>
		public JsonItem Value;

		/// <summary>
		/// Признак установления двоеточия
		/// </summary>
		public bool HasColon;
	}
}
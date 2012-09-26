using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qorpent
{
	/// <summary>
	/// Описывает исключения для обработки в ExceptionRegistry (добавление дополнительных параметров для регистратора)
	/// </summary>
	public interface IExceptionRegistryDataException
	{
		/// <summary>
		/// Дополнительные параметры
		/// </summary>
		IDictionary<string, string> ExceptionRegisryData { get; set; } 
	}
}

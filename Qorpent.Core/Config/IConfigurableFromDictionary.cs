using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Config
{
	/// <summary>
	/// 
	/// </summary>
	public interface IConfigurableFromDictionary{
		/// <summary>
		/// Метод дополнительной конфигурации объекта из словаря конфигурации
		/// </summary>
		/// <param name="args"></param>
		void Setup(IDictionary<string, string> args);
	}
}

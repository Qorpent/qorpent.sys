using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.BSharp;
using Qorpent.Bxl;

namespace Qorpent.Utils
{
	/// <summary>
	/// Вспомогательный класс для быстрой загрузки сервисов по интерфейсу без связи со сборками
	/// </summary>
	public static class WellKnownHelper
	{
		/// <summary>
		/// Возврщает инстанцию по умолчанию
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T Create<T>() where T:class{
			Type type = null;
			if (typeof (T) == typeof (IBxlParser)){
				type = Type.GetType("Qorpent.Bxl.BxlParser, Qorpent.Core");
			}else if (typeof (T) == typeof (IBSharpCompiler)){
				type = Type.GetType("Qorpent.BSharp.BSharpCompiler, Qorpent.Serialization");
			}
			if (null != type){
				return (T)Activator.CreateInstance(type);
			}
			throw new Exception("cannot create service for "+typeof(T).FullName);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Json
{
	/// <summary>
	/// Abstract Json scner
	/// </summary>
	public interface IJsonScaner {
		
	}

	/// <summary>
	/// Standard JSON scaner with all common stricts
	/// </summary>
	public class JsonScaner:IJsonScaner
	{
	}

	/// <summary>
	/// JS-friendly non-stricted JSON scaner
	/// </summary>
	public class NoStrictJsonScaner:IJsonScaner {
		
	}
}

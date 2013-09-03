using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent
{
	namespace Serialization {
		/// <summary>
		/// Edge.js related startup class
		/// </summary>
		public class Startup {
			/// <summary>
			/// Унифицированный вызов различных сериализаций
			/// </summary>
			/// <returns></returns>
#pragma warning disable 1998
			public async Task<object> Invoke(object input) {
#pragma warning restore 1998
				var arguments = (IDictionary<string, object>) input;
				if (arguments.ContainsKey("bxl")) {
					return new Bxl.BxlParser().Parse(arguments["bxl"].ToString()).ToString();
				}
				throw new Exception("does not understand task");
			}

		}
	}
}

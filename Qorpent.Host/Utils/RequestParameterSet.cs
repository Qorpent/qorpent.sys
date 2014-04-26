using System.Collections.Generic;

namespace Qorpent.Host.Utils
{
	/// <summary>
	/// Набор параметров запроса
	/// </summary>
	public class RequestParameterSet
	{
		/// <summary>
		/// 
		/// </summary>
		public RequestParameterSet()
		{
			Query =new Dictionary<string, string>();
			Form = new Dictionary<string, string>();
			Files =new Dictionary<string, PostFile>();
		}
		/// <summary>
		/// 
		/// </summary>
		public string PostData { get; set; }

		/// <summary>
		/// GET параметры
		/// </summary>
		public IDictionary<string, string> Query { get; private set; } 
		/// <summary>
		/// Form -данные
		/// </summary>
		public IDictionary<string,string> Form { get; private set; } 

		/// <summary>
		/// Form -данные
		/// </summary>
		public IDictionary<string,PostFile> Files { get; private set; } 

		/// <summary>
		/// Получить простой строковый параметр по имени
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string Get(string name)
		{
			if (Form.ContainsKey(name)) return Form[name];
			if (Query.ContainsKey(name)) return Query[name];
			return "";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetParameters()
		{
			var result = new Dictionary<string, string>();
			foreach (var p in Query)
			{
				result[p.Key] = p.Value;
			}
			foreach (var p in Form)
			{
				result[p.Key] = p.Value;
			}
			if (!string.IsNullOrWhiteSpace(PostData))
			{
				result["__postdata"] = PostData;
			}
			return result;
		} 
	}
}
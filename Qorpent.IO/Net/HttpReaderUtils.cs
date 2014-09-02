using System;
using System.Collections.Generic;
using System.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Утилиты парсинга HTTP
	/// </summary>
	public static class HttpReaderUtils{
		/// <summary>
		/// Преобразует хидер куки в набор объектов куки
		/// </summary>
		/// <param name="headerValue"></param>
		/// <returns></returns>
		public static IEnumerable<Cookie> ParseCookies(string headerValue)
		{
			Cookie current = null;
			foreach (string c in headerValue.SmartSplit(false, true, ';'))
			{
				if (null == current)
				{
					current = new Cookie();
				}
				if (c == ",")
				{
					yield return current;
					current = null;
					continue;
				}
				IList<string> part = c.SmartSplit(false, true, '=');


				if (part[0].ToLowerInvariant() == "expires")
				{
					current.Expires = DateTime.Parse(part[1]);
				}
				else if (part[0].ToLowerInvariant() == "path")
				{
					current.Path = part[1];
				}
				else if (part[0].ToLowerInvariant() == "domain")
				{
					current.Domain = part[1];
				}
				else
				{
					current.Name = part[0];
					current.Value = part[1];
				}
			}
			yield return current;
		}
	}
}
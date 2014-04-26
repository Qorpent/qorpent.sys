namespace Qorpent.Host
{
	/// <summary>
	/// Биндинг хоста
	/// </summary>
	public class HostBinding
	{
		/// <summary>
		/// 
		/// </summary>
		public HostBinding()
		{
			AppName = "/";
		}
		/// <summary>
		/// 
		/// </summary>
		public string AppName { get; set; }
		/// <summary>
		/// Порт
		/// </summary>
		public int Port { get; set; }
		/// <summary>
		/// Интерфейс
		/// </summary>
		public string Interface { get; set; }

		/// <summary>
		/// Схема (HTTP/S)
		/// </summary>
		public HostSchema Schema { get; set; }

		
		/// <summary>
		/// Преобразует биндинг в префикс листенера
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (!AppName.EndsWith("/"))
			{
				AppName += "/";
			}
			if (!AppName.StartsWith("/"))
			{
				AppName = "/" + AppName;
			}
			return Schema.ToString().ToLower() + "://" + Interface + ":" + Port + AppName;
		}
	}
}
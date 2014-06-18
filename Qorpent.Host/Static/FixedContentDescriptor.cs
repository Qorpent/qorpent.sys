using System;
using System.IO;
using System.Text;

namespace Qorpent.Host.Static{
	/// <summary>
	/// </summary>
	public class FixedContentDescriptor : StaticContentDescriptor{
		/// <summary>
		/// </summary>
		/// <param name="content"></param>
		/// <param name="name"></param>
		public FixedContentDescriptor(string content, string name){
			IsFixedContent = true;
			FixedContent = content;
			FullName = name;
			Name = name;
			Version = DateTime.Now;
		}

		/// <summary>
		///     Версия контента
		/// </summary>
		protected DateTime Version { get; set; }

		/// <summary>
		/// </summary>
		public override DateTime GetLastVersion(){
			return Version;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public override Stream Open(){
			return new MemoryStream(Encoding.UTF8.GetBytes(FixedContent ?? ""));
		}
	}
}
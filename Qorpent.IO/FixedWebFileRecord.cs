using System;
using System.Diagnostics;
using System.IO;

namespace Qorpent.IO{
	/// <summary>
	/// Описатель фиксированного контента
	/// </summary>
	public class FixedWebFileRecord : WebFileRecord{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="mime"></param>
		/// <param name="content"></param>
		/// <param name="data"></param>
		public FixedWebFileRecord(string name, string mime = null, string content = null, byte[] data = null){
			this.Name = name;
			this.MimeType = mime;
			this.FixedContent = content;
			this.FixedData = data;
			this.IsFixedContent = true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override DateTime GetVersion(){
			return Process.GetCurrentProcess().StartTime;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="output"></param>
		/// <returns></returns>
		public override long Write(Stream output){
			throw new NotImplementedException();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Stream Open(){
			throw new NotImplementedException();
		}
	}
}
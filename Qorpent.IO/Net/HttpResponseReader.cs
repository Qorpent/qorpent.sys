using System;
using System.IO;

namespace Qorpent.IO.Net{
	/// <summary>
	/// 
	/// </summary>
	public class HttpResponseReader{

		/// <summary>
		/// Размер буфера
		/// </summary>
		public int BufferSize { get; set; }

		/// <summary>
		/// Считывает результат запроса HTTP
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>Результат запроса</returns>
		public HttpResponse Read(Stream stream){
			using (var context = new HttpResponseReaderExecutor(stream,BufferSize)){
				try{
					context.Read();
				}
				catch (IOException knownError){
					context.Result.Error = knownError;
				}
				catch (Exception error){
					context.Result.Error = new IOException("General error occured", error);
				}
				return context.Result;
			}
		}

		
	}
}
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Класс, упрощающий работу с API ресурсов
	/// </summary>
	public static class ResourceExtensions {


		/// <summary>
		/// Асинхронный макро-метод получения ресурса
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="url"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<IResource> GetResourceAsync(this IResourceProvider provider, string url, IResourceConfig config = null) {
			var request = provider.CreateRequest(new Uri(url));
			var response = await request.GetResponse(config);
			var resource = await response.GetResource();
			return resource;
		}
		/// <summary>
		/// Асинхронный макро-метод чтения строки по URL
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="url"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<Stream> GetStreamAsync(this IResourceProvider provider, string url, IResourceConfig config = null) {
			var resource = await provider.GetResourceAsync(url, config);
			return await resource.Open();
		}

		/// <summary>
		/// Асинхронный макро-метод чтения строки по URL
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="url"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<byte[]> GetDataAsync(this IResourceProvider provider, string url, IResourceConfig config = null) {
			var resource = await provider.GetResourceAsync(url, config);
			return await resource.GetData();
		}


		/// <summary>
		/// Асинхронный макро-метод чтения строки по URL
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="url"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static async Task<string> GetStringAsync(this IResourceProvider provider, string url, IResourceConfig config = null) {
			var resource = await provider.GetResourceAsync(url, config);
			var finalConfig = resource.Config;
			var enc = Encoding.UTF8;
			if (finalConfig != null && null != finalConfig.ResponseEncoding) {
				enc = finalConfig.ResponseEncoding;
			}
			using (var sw = new StreamReader(await resource.Open(), enc)) {
				return await sw.ReadToEndAsync();
			}
		}


		/// <summary>
		/// Синхронизированный метод получения данных
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="url"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static byte[] GetData(this IResourceProvider provider, string url, IResourceConfig config = null) {
			var getdatatask = GetDataAsync(provider, url, config);
			getdatatask.Wait();
			return getdatatask.Result;
		}

		/// <summary>
		/// Синхронизированный метод получения строки
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="url"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static string GetString(this IResourceProvider provider, string url, IResourceConfig config = null)
		{
			var getdatatask = GetStringAsync(provider, url, config);
			getdatatask.Wait();
			return getdatatask.Result;
		}
	}
}
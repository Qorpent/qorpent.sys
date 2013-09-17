using System.IO;
using System.Xml.Linq;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Расширения для более удобной работс с хранилищем
	/// </summary>
	public static class DirtyVersionStorageExtensions {
		/// <summary>
		/// Возвращает полную историю
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static MappingInfo Explain(this IDirtyVersionStorage storage, string name)
		{
			using (var o = storage.GetMapper().Open(name))
			{
				return o.MappingInfo;
			}
		}

		/// <summary>
		/// Возвращает информацию о последнем или указанном коммите
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="name"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static Commit ExplainVersion(this IDirtyVersionStorage storage, string name, string hash = null)
		{
			using (var o = storage.GetMapper().Open(name))
			{
				if (string.IsNullOrWhiteSpace(hash))
				{
					return o.MappingInfo.GetHead();
				}
				return o.Resolve(hash);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="name"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static string ReadString(this IDirtyVersionStorage storage, string name, string hash = null) {
			return new StreamReader(storage.Open(name, hash)).ReadToEnd();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="name"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		public static XElement ReadXml(this IDirtyVersionStorage storage, string name, string hash = null) {
			return XElement.Load(storage.Open(name, hash));
		}
		
		/// <summary>
		/// Выдает информацию о файле в XML
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static XElement ExplainAsXml(this IDirtyVersionStorage storage, string name) {
			return new MappingInfoSerializer().Serialize(storage.Explain(name));
		}
	}
}
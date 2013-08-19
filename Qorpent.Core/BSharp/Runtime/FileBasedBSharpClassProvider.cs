using System.IO;
using System.Xml.Linq;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Источник классов BSharp на основе директории
	///     имя ресурса для дескриптора - имя файла, работает с плоскими директориями
	/// </summary>
	public class FileBasedBSharpClassProvider : BSharpClassProviderBase {
		/// <summary>
		///     Корневая директория,содержащая классы
		/// </summary>
		public string RootDirectory { get; set; }

		/// <summary>
		///     Строит список из всех кдассов с расширением ".bsclass"
		/// </summary>
		protected override void RebuildIndex() {
			Cache.Clear();
			foreach (string fn in Directory.GetFiles(RootDirectory, "*.bsclass", SearchOption.AllDirectories)) {
				string fullname = Path.GetFileNameWithoutExtension(fn);
				var desc = new BSharpRuntimeClassDescriptor {
					ResourceName = fn,
					Fullname = fullname,
					LastWrite = File.GetLastWriteTime(fn)
				};
				Cache[fullname] = desc;
			}
		}

		/// <summary>
		///     Перегружает класс из файла на диске
		/// </summary>
		/// <param name="descriptor"></param>
		protected override void ReloadClass(BSharpRuntimeClassDescriptor descriptor) {
			XElement xml = XElement.Load(descriptor.ResourceName);
			var cls = new BSharpRuntimeClass(Container) {Definition = xml};
			descriptor.CachedClass = cls;
		}

		/// <summary>
		///     Проверяет наличие файла на диске и его время записи
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		protected override bool IsActual(BSharpRuntimeClassDescriptor descriptor) {
			return descriptor.LastWrite >= File.GetLastWriteTime(descriptor.ResourceName);
		}
	}
}
namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Обобщенный истоник классов BSharp
	/// </summary>
	public class BSharpGenericClassProvider : BSharpClassProviderBase {
		/// <summary>
		///     Вносит класс напрямую в кэш
		/// </summary>
		/// <param name="cls"></param>
		public void Set(BSharpRuntimeClass cls) {
			if (!Cache.ContainsKey(cls.Fullname)) {
				Cache[cls.Fullname] = new BSharpRuntimeClassDescriptor {
					Fullname = cls.Fullname,
					CachedClass = cls,
				};
			}
			else {
				Cache[cls.Fullname].CachedClass = cls;
			}
		}
	}
}
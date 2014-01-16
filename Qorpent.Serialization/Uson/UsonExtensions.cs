namespace Qorpent.Uson
{
	/// <summary>
	/// Расширения для USon
	/// </summary>
	public static class UsonExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static UObj ToUson(this object obj)
		{
			if (obj is UObj) return (UObj) obj;
			return (UObj)UObjSerializerSupport.ToUson(obj) ;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonDeepExtend(this object obj, params object[] objects)
		{
			return obj.ToUson().deepextend((object[]) objects);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonExtend(this object obj, params object[] objects)
		{
			return obj.ToUson().extend((object[])objects);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonDeepDefaults(this object obj, params object[] objects)
		{
			return obj.ToUson().deepdefaults((object[])objects);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objects"></param>
		/// <returns></returns>
		public static UObj UsonDefaults(this object obj, params object[] objects)
		{
			return obj.ToUson().defaults((object[])objects);
		}
	}
}
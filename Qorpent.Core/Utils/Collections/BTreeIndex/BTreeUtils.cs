using System.IO;

namespace Qorpent.Utils.Collections.BTreeIndex{
	/// <summary>
	/// 
	/// </summary>
	public static class BTreeUtils{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="r"></param>
		/// <returns></returns>
		public static T Read<T>(BinaryReader r){
			if (typeof(T) == typeof(int))
			{
				return (T)(object)r.ReadInt32();
			}
			if (typeof(T) == typeof(long))
			{
				return (T)(object)r.ReadInt64();
			}
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)r.ReadBoolean();
			}
			return (T)(object)r.ReadString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="w"></param>
		/// <param name="value"></param>
		public static void Write<T>(BinaryWriter w, T value)
		{
			if (typeof(T) == typeof(int))
			{
				w.Write((int)(object)value);
			}
			else if (typeof(T) == typeof(long))
			{
				w.Write((long)(object)value);
			}
			else if (typeof(T) == typeof(bool))
			{
				w.Write((bool)(object)value);
			}
			else
			{
				w.Write(value.ToString());
			}
		}
	}
}
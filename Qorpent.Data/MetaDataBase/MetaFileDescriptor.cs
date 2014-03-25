using System;
using System.Security.Cryptography;
using System.Text;
using Qorpent.Model;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Описатель файла
	/// </summary>
	public class MetaFileDescriptor:RevisionDescriptor,IWithCode,IWithName{
		/// <summary>
		/// Unique memo-code
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		///Name of the entity
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Исходный контент файла
		/// </summary>
		public string Content { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public void CheckHash(){
			if (string.IsNullOrWhiteSpace(Hash)){
				if (string.IsNullOrWhiteSpace(Content)) return;
				Hash = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Content)));
			}
		}
		/// <summary>
		/// Признак полного определения дескриптора
		/// </summary>
		/// <returns></returns>
		public bool IsFullyDefined(){
			if (string.IsNullOrWhiteSpace(Code)) return false;
			if (string.IsNullOrWhiteSpace(Content)) return false;
			CheckHash();
			return true;
		}
		/// <summary>
		/// Получает копию
		/// </summary>
		/// <returns></returns>
		public MetaFileDescriptor Copy(){
			return MemberwiseClone() as MetaFileDescriptor;
		}
	}
}
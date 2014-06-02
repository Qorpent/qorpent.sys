using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Scaffolding.Model.SqlObjects
{
	/// <summary>
	/// Описывает файловую группу
	/// </summary>
	public class FileGroup:SqlObject
	{
		/// <summary>
		/// 
		/// </summary>
		public FileGroup(){
			this.ObjectType = SqlObjectType.FileGroup;
			this.PreTable = true;
		}
		/// <summary>
		/// Признак группы по умолчанию
		/// </summary>
		public bool IsDefault { get; set; }
	}
}

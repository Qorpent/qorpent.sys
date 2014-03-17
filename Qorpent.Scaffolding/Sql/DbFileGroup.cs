using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class DbFileGroup : DbObject{

		/// <summary>
		/// 
		/// </summary>
		public DbFileGroup(){
			this.ObjectType = DbObjectType.FileGroup;
		}

		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="xml"></param>
		protected override IEnumerable<DbObject> Setup(XElement xml){
			WithIdx = xml.Attr("withidx").ToBool();
			FileSize = xml.Attr("filesize").ToInt();
			FileCount = xml.Attr("filecount").ToInt();
			IsDefault = xml.Attr("isdefault").ToBool();
			var result = base.Setup(xml).ToArray();
			return result;
		}
		/// <summary>
		/// Признак первичной файловой группы
		/// </summary>
		public bool IsDefault { get; set; }

		/// <summary>
		/// Количество файлов
		/// </summary>
		public int FileCount { get; set; }

		/// <summary>
		/// Изначальный размер каждого файла
		/// </summary>
		public int FileSize { get; set; }

		/// <summary>
		/// Количество подгрупп для партиций
		/// </summary>
		public bool WithIdx { get; set; }
	}
}
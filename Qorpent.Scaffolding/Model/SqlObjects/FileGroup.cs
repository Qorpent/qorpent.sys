using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

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
			ObjectType = SqlObjectType.FileGroup;
			PreTable = true;
			UpperCase = true;
			FileSize = 10;
			FileCount = 1;
		}

		/// <summary>
		/// Признак группы по умолчанию
		/// </summary>
		public bool IsDefault { get; set; }


		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml)
		{
			base.Setup(model,cls,bscls,xml);
			WithIndex = xml.Attr("withidx","0").ToBool();
			FileSize = xml.Attr("filesize","10").ToInt();
			FileCount = xml.Attr("filecount","1").ToInt();
			IsDefault = xml.Attr("isdefault","0").ToBool();
			return this;
		}


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
		public bool WithIndex { get; set; }
		/// <summary>
		/// Представление WithIndex в виде bit
		/// </summary>
		public int WithIndexAsBit { get { return WithIndex ? 1 : 0; } }
		/// <summary>
		/// Представление IsDefault в виде bit
		/// </summary>
		public int IsDefaultAsBit { get { return IsDefault ? 1 : 0; } }
	}
}

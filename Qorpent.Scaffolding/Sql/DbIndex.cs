using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class DbIndex : DbObject
	{
		/// <summary>
		/// 
		/// </summary>
		public DbIndex()
		{
			ObjectType = DbObjectType.Index;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override IEnumerable<DbObject> Setup(XElement xml)
		{
			var result = base.Setup(xml).ToArray();
			this.Fields = xml.Attr("for").SmartSplit();
			this.Includes = xml.Attr("include").SmartSplit();
			
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		public IList<string> Includes { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IList<string> Fields { get; set; }
		/// <summary>
		/// Проверка настроек индекса
		/// </summary>
		public void Verify(){
			foreach (var field in Fields)
			{
				if (!((DbTable)ParentElement).Fields.ContainsKey(field))
				{
					throw new Exception("illegal field in index " + FullName + " : " + field + " not exists in parent table");
				}
			}

			foreach (var field in Includes)
			{
				if (!((DbTable)ParentElement).Fields.ContainsKey(field))
				{
					throw new Exception("illegal include field in index " + FullName + " : " + field + " not exists in parent table");
				}
			}
		}
	}
}
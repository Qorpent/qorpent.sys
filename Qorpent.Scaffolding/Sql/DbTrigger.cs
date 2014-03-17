using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class DbTrigger:DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbTrigger(){
			this.ObjectType =DbObjectType.Trigger;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override System.Collections.Generic.IEnumerable<DbObject> Setup(System.Xml.Linq.XElement xml)
		{
			var result = base.Setup(xml).ToArray();

			this.Insert = xml.Attr("insert").ToBool();
			this.Update = xml.Attr("update").ToBool();
			this.Delete = xml.Attr("delete").ToBool();
			this.Before = xml.Attr("before").ToBool();

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		public bool Before { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool Insert { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool Update { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool Delete { get; set; }
	}
}
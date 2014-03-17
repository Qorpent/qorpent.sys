using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class DbFunction : DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbFunction(){
			ObjectType = DbObjectType.Function;
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		protected override IEnumerable<DbObject> Setup(XElement xml)
		{
			var result =  base.Setup(xml).ToArray();
			SetupReturnType(xml);
			foreach (var parameter in xml.Attributes().Where(_=>_.Name.LocalName.StartsWith("__AT__"))){
				Parameters.Add(new Parameter{Code = parameter.Name.LocalName.Substring(6), DataType = _types[parameter.Value]});
			}
			return result;
		}

		private void SetupReturnType(XElement xml){
			var returntype = xml.Attr("returns");
			if(string.IsNullOrWhiteSpace(returntype) && xml.Name.LocalName!="void" && xml.Name.LocalName!="function"){
				returntype = xml.Name.LocalName;
			}
			if (string.IsNullOrWhiteSpace(returntype) || "void" == returntype || "void"==xml.Name.LocalName){
				IsProcedure = true;
				ObjectType = DbObjectType.Procedure;
			}
			else{
				IsProcedure = false;
				ReturnType = _types[returntype];
			}
			if (string.IsNullOrWhiteSpace(this.Body)){ //abstract methods
				if (IsProcedure){
					Body = "THROW 50003, '" + this.FullName + " not implemented', 1";
				}
				else{
					var tsql = new TSQLProvider();
					var tp = tsql.GetSql(_types[returntype]);
					Body = "return cast( (cast ('Error 50003, "+this.FullName+" not implemented') ) as " + tp + ")";
				}
			}
		}

		

		private IList<Parameter> parameters = new List<Parameter>();

		/// <summary>
		/// 
		/// </summary>
		public DbDataType ReturnType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsProcedure { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IList<Parameter> Parameters{
			get { return parameters; }
			
		}
	}
}
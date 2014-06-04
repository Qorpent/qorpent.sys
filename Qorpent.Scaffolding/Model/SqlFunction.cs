using System.Collections.Generic;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Sql;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// Обертка над SQL функцией/процедурой
	/// </summary>
	public class SqlFunction : SqlObject{
		/// <summary>
		/// 
		/// </summary>
		public SqlFunction(){
			Arguments = new Dictionary<string, SqlFunctionArgument>();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, BSharp.IBSharpClass bscls,
		                                System.Xml.Linq.XElement xml){
			base.Setup(model, cls, bscls, xml);
			var xname = xml.Name.LocalName;
			IsProcedure = xname == "void" || (xname == "function" && string.IsNullOrWhiteSpace(xml.Attr("returns")));
			if (!IsProcedure){
				var dtype = xname;
				if (dtype == "function"){
					dtype = xml.Attr("returns");
				}
				this.ReturnType = this.Table.DataTypeMap[dtype];
			}
			var i = 0;
			foreach (var a in xml.Attributes()){
				var name = a.Name.LocalName.Unescape(EscapingType.XmlName);
				var val = a.Value;
				if (name.StartsWith("@")){
					var namepair = name.Substring(1).Split('-');
					var argname = namepair[0];
					SqlFunctionArgument arg;
					if (!Arguments.ContainsKey(argname)){
						Arguments[argname] = new SqlFunctionArgument{Name = argname, DataType = Table.DataTypeMap["string"], Index = i++};
					}
					arg = Arguments[argname];
					if (namepair.Length == 1){
//only type determine
						arg.DataType = Table.DataTypeMap[val];
					}
					else{
						bool hastype = false;
						bool hasdefault = false;
						for (var j = 1; j < namepair.Length; j++){
							var part = namepair[j];
							if (part == "default"){
								hasdefault = true;
							}
							else if (part == "out"){
								arg.IsOutput = true;
							}
							else if (Table.DataTypeMap.ContainsKey(part)){
								arg.DataType = Table.DataTypeMap[part];
								hastype = true;
							}
							else{
								Model.RegisterError(new BSharpError{Message = "unknown arg part " + part, Xml = xml, Class = Table.TargetClass});
							}
						}
						if ((hastype || hasdefault) && !string.IsNullOrWhiteSpace(val)){
							arg.DefaultValue = new DefaultValue{DefaultValueType = DbDefaultValueType.Native, Value = val};
						}
					}
				}
			}
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, SqlFunctionArgument> Arguments { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public DataType ReturnType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsProcedure { get; set; }


	}
}
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Обертка над SQL функцией/процедурой
	/// </summary>
	public class SqlFunction : SqlObject{
		/// <summary>
		/// </summary>
		public SqlFunction(){
			Arguments = new Dictionary<string, SqlFunctionArgument>();
			UseTablePrefixedName = true;
		}

		/// <summary>
		/// </summary>
		public IDictionary<string, SqlFunctionArgument> Arguments { get; private set; }

		/// <summary>
		/// </summary>
		public DataType ReturnType { get; set; }

		/// <summary>
		/// </summary>
		public bool IsProcedure { get; set; }
		/// <summary>
		/// Признак SQL-методов
		/// </summary>
		public SqlMethodOptions SqlMethod { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public override SqlObject Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls,
		                                XElement xml){
			base.Setup(model, cls, bscls, xml);
			string xname = xml.Name.LocalName;
			IsProcedure = xname == "void" || (xname == "function" && string.IsNullOrWhiteSpace(xml.Attr("returns")));
			if (!IsProcedure){
				string dtype = xname;
				if (dtype == "function"){
					dtype = xml.Attr("returns");
				}
				ReturnType = Table.DataTypeMap[dtype];
			}
			int i = 0;
			foreach (XAttribute a in xml.Attributes()){
				string name = a.Name.LocalName.Unescape(EscapingType.XmlName);
				string val = a.Value;
				if (name.StartsWith("@")){
					string[] namepair = name.Substring(1).Split('-');
					string argname = namepair[0];
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
						for (int j = 1; j < namepair.Length; j++){
							string part = namepair[j];
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
	}
}
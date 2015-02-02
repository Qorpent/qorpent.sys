using System.IO;
using System.Linq;
using Qorpent.Serialization;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Object cache for model generator
	/// </summary>
	public class PokoObjectCacheWriter : CodeWriterBase
	{
		/// <summary>
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="output"></param>
		public PokoObjectCacheWriter(PersistentClass cls, TextWriter output = null)
			: base(cls, output)
		{
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun()
		{
			WriteStartClass();
			WriteGetTableQuery();
			WriteModelLink();
			WriteSqlMethods();
			WriteEndClass();
		}

		private void WriteSqlMethods(){
			foreach (var method in Cls.SqlObjects.OfType<SqlFunction>().Where(_=>_.SqlMethod.HasFlag(SqlMethodOptions.IsMethod)).OrderBy(_=>_.Name)){
				WriteSqlMethod(method);				
			}
		}

		private void WriteSqlMethod(SqlFunction method){
			o.WriteLine();
			o.Write("\t\t///<summary>{0} (Id notation)</summary>\r\n", method.Comment);
			o.Write("\t\tpublic {0}[] {1} (long {2}Id) {{\r\n", method.ReturnType.TargetType.Name, method.Name, Cls.Name.ToLowerInvariant());
			o.Write(@"			return Model.{0}.GetAll (""select id from {1} ( '""+{2}Id+""')"");
", method.ReturnType.TargetType.Name, method.FullName.Replace("\"", "\\\""), Cls.Name.ToLowerInvariant());
			o.WriteLine("\t\t}");
			o.WriteLine();
			o.Write("\t\t///<summary>{0} (Code notation)</summary>\r\n", method.Comment);
			o.Write("\t\tpublic {0}[] {1} (string {2}Code) {{\r\n", method.ReturnType.TargetType.Name, method.Name, Cls.Name.ToLowerInvariant());
			o.Write(@"			return Model.{0}.GetAll (""select id from {1} ( '""+{2}Code+""')"");
",method.ReturnType.TargetType.Name,method.FullName.Replace("\"","\\\""),Cls.Name.ToLowerInvariant());
			o.WriteLine("\t\t}");
			o.WriteLine();
			o.Write("\t\t///<summary>{0}</summary>\r\n", method.Comment);
			o.Write("\t\tpublic {0}[] {1} ({2} {3}) {{\r\n", method.ReturnType.TargetType.Name, method.Name, Cls.Name,Escaper.EscapeCSharpKeyword(Cls.Name.ToLowerInvariant()));
			o.Write(@"			return {0} ({1}.Id);
", method.Name,Escaper.EscapeCSharpKeyword(Cls.Name.ToLowerInvariant()));
			o.WriteLine("\t\t}");
		}

		private void WriteGetTableQuery(){
			o.WriteLine("\t\t///<summary>Creates cache with typed adapter </summary>");
			o.Write("\t\tpublic {0}DataCache() {{\r\n",Cls.Name);
			//o.Write("\t\t\tthis.Adapter = new {0}DataAdapter();\r\n",Cls.Name);
			o.WriteLine("\t\t}");
		}

		private void WriteModelLink()
		{
			o.WriteLine("\t\t///<summary>Back reference to model</summary>");
			o.Write("\t\tpublic {0}.Adapters.Model Model {{get;set;}}\r\n",DefaultNamespce);
		}

		private void WriteEndClass()
		{
			o.WriteLine("\t}");
			o.WriteLine("}");
		}

		private void WriteStartClass()
		{
			WriteHeader();
			o.WriteLine("using System;");
			o.WriteLine("using System.Collections.Generic;");
			o.WriteLine("using System.Text;");
			o.WriteLine("using System.Data;");
			o.WriteLine("using Qorpent.Data;");
			o.WriteLine("using Qorpent.Data.DataCache;");
			
			o.Write("using {0}.Adapters;\r\n", Cls.Namespace);
			o.Write("namespace {0}.ObjectCaches {{\r\n", Cls.Namespace);
			o.WriteLine("\t///<summary>");
			o.WriteLine("\t/// Object cache for " + Cls.Name);
			o.WriteLine("\t///</summary>");
			o.Write("\tpublic partial class {0}DataCache : ObjectDataCache<{0}> {{\r\n", Cls.Name);
			
		}
	}
}
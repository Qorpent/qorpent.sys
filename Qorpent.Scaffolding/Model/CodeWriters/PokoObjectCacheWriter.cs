using System.IO;
using System.Linq;
using Qorpent.Data;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Object cache for model generator
	/// </summary>
	public class PokoObjectCacheWriter : CodeWriterBase
	{
	    private bool _doWriteTableQuery =true;
	    private bool _doWriteModelLink = true;
	    private bool _doWriteSqlMethods = true;
	    private bool _doWriteClassWrapper = true;
	    private bool _doWritePreamble = true;
	    private bool _doWriteObjectWrappers = true;

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
		    if (DoWriteClassWrapper) {
                WriteStartClass();   
		    }
		    if (DoWriteTableQuery) {
		        WriteGetTableQuery();
		    }
		    if (DoWriteModelLink) {
		        WriteModelLink();
		    }
		    if (DoWriteSqlMethods) {
		        WriteSqlMethods();
		    }
		    if (DoWriteObjectWrappers) {
                WriteWrappers();
		    }
		    if (DoWriteClassWrapper) {
		        WriteEndClass();
		    }
		}

        private void WriteWrappers()
        {
            var wrappers = Cls.FindObjects<SqlFunction>("cs-wrap");
            foreach (var function in wrappers)
            {
                WriteWrapper(function);
            }
        }

        private void WriteWrapper(SqlFunction function) {
            var notation = function.Definition.Attr("cs-wrap").To<DbCallNotation>();
            var name = function.Name;
            var privateCommandName = "_" + name + "Wrapper";
            o.WriteLine("\t\tprivate DbCommandWrapper {0} = new DbCommandWrapper{{ObjectName=\"{1}\",Notation=DbCallNotation.{2}}};", privateCommandName, name,notation);
            var type = "object";
            if (notation == DbCallNotation.Scalar) {
                var returntype = function.Definition.Attr("cs-return");
                if (!string.IsNullOrWhiteSpace(returntype)) {
                    var datatype = Cls.DataTypeMap[returntype];
                    type = datatype.CSharpDataType;
                }
            }
            else if (notation == DbCallNotation.None) {
                type = "void";
            }
            o.Write("\t\t///<summary>{0}</summary>\r\n", function.Comment);
            o.WriteLine("\t\tpublic {0} {1}Generic (object parameters) {{",type,name);
            o.WriteLine("\t\t\tvar command = {0}.Clone(parameters,GetConnection());",privateCommandName);
            o.WriteLine("\t\t\treturn DbCommandExecutor.Default.GetResultSync(command);");
            o.WriteLine("\t\t}");
            o.Write("\t\t///<summary>{0}</summary>\r\n", function.Comment);
            o.Write("\t\tpublic {0} {1} (", type, name);
            var first = true;
            foreach (var argument in function.Arguments) {
                if (!first) {
                    o.Write(", ");
                }
                first = false;
                o.Write(argument.Value.DataType.CSharpDataType);
                o.Write(" ");
                o.Write(argument.Value.Name);
                o.Write(" = default({0})", argument.Value.DataType.CSharpDataType);
            }
            o.WriteLine("){");
            o.Write("\t\t\treturn {0}Generic (new{{",name);
            first = true;
            foreach (var argument in function.Arguments)
            {
                if (!first)
                {
                    o.Write(", ");
                }
                first = false;
                o.Write(argument.Value.Name);
            }
            o.WriteLine("});");
            o.WriteLine("\t\t}");

        }

        

	    /// <summary>
	    /// 
	    /// </summary>
	    public bool DoWriteObjectWrappers {
	        get { return _doWriteObjectWrappers; }
	        set { _doWriteObjectWrappers = value; }
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    public bool DoWriteClassWrapper {
	        get { return _doWriteClassWrapper; }
	        set { _doWriteClassWrapper = value; }
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    public bool DoWriteSqlMethods   {
	        get { return _doWriteSqlMethods; }
	        set { _doWriteSqlMethods = value; }
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    public bool DoWriteModelLink {
	        get { return _doWriteModelLink; }
	        set { _doWriteModelLink = value; }
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    public bool DoWriteTableQuery {
	        get { return _doWriteTableQuery; }
	        set { _doWriteTableQuery = value; }
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
            if (DoWritePreamble) { 
			    WriteHeader();
			    o.WriteLine("using System;");
			    o.WriteLine("using System.Collections.Generic;");
			    o.WriteLine("using System.Text;");
			    o.WriteLine("using System.Data;");
			    o.WriteLine("using Qorpent.Data;");
			    o.WriteLine("using Qorpent.Data.DataCache;");
			
			    o.Write("using {0}.Adapters;\r\n", Cls.Namespace);
			    o.Write("namespace {0}.ObjectCaches {{\r\n", Cls.Namespace);
            }
			o.WriteLine("\t///<summary>");
			o.WriteLine("\t/// Object cache for " + Cls.Name);
			o.WriteLine("\t///</summary>");

			o.Write("\tpublic partial class {0}DataCache : ObjectDataCache<{0}> {{\r\n", Cls.Name);
			
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    public bool DoWritePreamble {
	        get { return _doWritePreamble; }
	        set { _doWritePreamble = value; }
	    }
	}
}
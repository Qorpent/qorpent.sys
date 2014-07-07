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
			WriteEndClass();
		}

		private void WriteGetTableQuery(){
			o.WriteLine("\t\t///<summary>Creates cache with typed adapter </summary>");
			o.Write("\t\tpublic {0}DataCache() {{\r\n",Cls.Name);
			//o.Write("\t\t\tthis.Adapter = new {0}DataAdapter();\r\n",Cls.Name);
			o.WriteLine("\t\t}");
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
			o.Write("using {0}.Adapters;\r\n", Cls.Namespace);
			o.Write("namespace {0}.ObjectCaches {{\r\n", Cls.Namespace);
			o.WriteLine("\t///<summary>");
			o.WriteLine("\t/// Object cache for " + Cls.Name);
			o.WriteLine("\t///</summary>");
			o.Write("\tpublic partial class {0}DataCache : ObjectDataCache<{0}> {{\r\n", Cls.Name);
			
		}
	}
}
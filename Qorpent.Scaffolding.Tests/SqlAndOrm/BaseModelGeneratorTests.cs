using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class BaseModelGeneratorTests{
		[Test]
		public void SimplestModel(){
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new BaseModelWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace Orm.Adapters {
	///<summary>Model for Orm definition</summary>
	public partial class Model {
		///<summary>Retrieve data adapter by type (generic)</summary>
#if NOQORPENT
		public static object GetAdapter<T>(){
			return GetAdapter(typeof(T));
#else
		public static IObjectDataAdapter<T> GetAdapter<T>() where T:class,new(){
			return (IObjectDataAdapter<T>)GetAdapter(typeof(T));
#endif
		}
		///<summary>Retrieve data adapter by type</summary>
#if NOQORPENT
		public static object GetAdapter(Type objectType){
#else
		public static IObjectDataAdapter GetAdapter(Type objectType){
#endif
			switch(objectType.Name){
				case ""a"": return new aDataAdapter();
			}
			return null;
		}
	}
}

".Trim(), code.Trim());
		}


		[Test]
		public void ModelWithReferencesAndAutoAndLazySetup()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable 
class b prototype=dbtable
	ref a reverse auto reverse-lazy
class c prototype=dbtable
	ref b reverse reverse-auto lazy
");
			var code = new BaseModelWriter(model) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace Orm.Adapters {
	///<summary>Model for Orm definition</summary>
	public partial class Model {
		///<summary>Retrieve data adapter by type (generic)</summary>
#if NOQORPENT
		public static object GetAdapter<T>(){
			return GetAdapter(typeof(T));
#else
		public static IObjectDataAdapter<T> GetAdapter<T>() where T:class,new(){
			return (IObjectDataAdapter<T>)GetAdapter(typeof(T));
#endif
		}
		///<summary>Retrieve data adapter by type</summary>
#if NOQORPENT
		public static object GetAdapter(Type objectType){
#else
		public static IObjectDataAdapter GetAdapter(Type objectType){
#endif
			switch(objectType.Name){
				case ""a"": return new aDataAdapter();
				case ""b"": return new bDataAdapter();
				case ""c"": return new cDataAdapter();
			}
			return null;
		}
		///<summary>Marks active auto foreign key link from b to a with Id (reverse)</summary>
		public bool AutoLoadba = true;
		///<summary>Marks active auto foreign key link from b to a with Id (reverse) as LazyLoad </summary>
		public bool Lazyba = false;
		///<summary>Marks active auto collection in b of b with a (reverse)</summary>
		public bool AutoLoadabs=false;
		///<summary>Marks active auto collection in b of b with a (reverse) as Lazy</summary>
		public bool Lazyabs=true;
		///<summary>Marks active auto foreign key link from c to b with Id (reverse)</summary>
		public bool AutoLoadcb = false;
		///<summary>Marks active auto foreign key link from c to b with Id (reverse) as LazyLoad </summary>
		public bool Lazycb = true;
		///<summary>Marks active auto collection in c of c with b (reverse)</summary>
		public bool AutoLoadbcs=true;
		///<summary>Marks active auto collection in c of c with b (reverse) as Lazy</summary>
		public bool Lazybcs=false;
	}
}

".Trim(), code.Trim());
		}


	}
}
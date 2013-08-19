using System.Xml.Linq;
using Qorpent.BSharp.Runtime;
using Qorpent.IoC;

namespace Qorpent.Core.Tests.BSharp.Runtime {
	public class BSharpRuntimeTestBase {

		public class A : I { }
		public class B : I, IBSharpRuntimeBound
		{
			public IBSharpRuntimeClass Cls;
			public void Initialize(IBSharpRuntimeClass cls)
			{
				Cls = cls;
			}
		}
		public interface I { }

		public static BSharpRuntimeClass CreateCls(IContainer c = null) {
			return CreateCls(null, null);
		}

		public static BSharpRuntimeClass CreateCls(string name,IContainer c = null )
		{
			return CreateCls<object>(name, c);
		}

		public static BSharpRuntimeClass CreateCls<T>(IContainer c = null) {
			return CreateCls<T>(null, c);
		}

		public static BSharpRuntimeClass CreateCls<T>(string name,IContainer c = null) {
			var n = typeof (T).Name;
			if (typeof (object) == typeof (T)) {
				if(!string.IsNullOrWhiteSpace(name)) {
					n = name.Replace(".", "_");
				}
			}
			var rt = typeof (T).AssemblyQualifiedName;
			if (!string.IsNullOrWhiteSpace(name)) {
				rt = name;
			}
			var cls = new BSharpRuntimeClass(c) {
				Definition = XElement.Parse(string.Format("<a code='{0}' fullcode='my.test.{0}' runtime='{1}'/>",n,rt))
			};
			return cls;
		}
	}
}
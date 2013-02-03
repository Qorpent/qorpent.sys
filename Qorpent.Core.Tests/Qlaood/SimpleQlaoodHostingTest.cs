using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Qorpent.Qlaood;

namespace Qorpent.Core.Tests.Qlaood
{
	public class ClassToTestLoader:MarshalByRefObject {
		public string GetDomainName() {
			return AppDomain.CurrentDomain.FriendlyName;
			;
		}
	}
	
	public class SimpleQlaoodHostingTest
	{

		class TestHost :MarshalByRefObject, IQlaoodHost {
			/// <summary>
			/// 	Accessor for remote assembly loader
			/// </summary>
			public IAssemblyLoader AssemblyLoader {
				get { return ThisAssemblyLoader; }
			}

			public ThisAssemblyLoader ThisAssemblyLoader { get; set; }

			/// <summary>
			/// Hook to setup link to hosted domain
			/// </summary>
			/// <param name="interop"></param>
			public void SetInterop(QlaoodBridgeInterop interop) {
				this.Interop = interop;
			}

			public QlaoodBridgeInterop Interop { get; set; }
		}
		class ThisAssemblyLoader : MarshalByRefObject,IAssemblyLoader {
			/// <summary>
			/// 	Retrieves assembly as it's usual for ResolveAssembly event of AppDomain
			/// </summary>
			/// <param name="name"> full or short name of assembly to resolve </param>
			/// <param name="behavior"> loader behavior options </param>
			/// <returns> </returns>
			public Assembly GetAssembly(string name, AssemblyLoaderBehavior behavior = AssemblyLoaderBehavior.Default) {
				throw new NotImplementedException();
			}

			/// <summary>
			/// 	Retrieves binary data of assembly and symbol file (optional) without loading of assembly
			/// </summary>
			/// <param name="name"> </param>
			/// <param name="behavior"> </param>
			/// <returns> </returns>
			public BinaryAssemblyData GetAssemblyData(string name, AssemblyLoaderBehavior behavior = AssemblyLoaderBehavior.Default) {
				if (name.Contains("Qorpent.Core.Tests")) {
					WasCalled = true;
					var path = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///","");
					var data = File.ReadAllBytes(path);
					var result = new BinaryAssemblyData { Assembly = data, Name = "Qorpent.Core.Tests" };
					return result;
				}
				return new BinaryAssemblyData();
			}

			public bool WasCalled { get; set; }
		}


		[Test]
		public void Can_Create_Host_Load_Assembly_And_Call_Class_From_It() {
			var root = Path.Combine(Path.GetTempPath(), "qlaood");
			Directory.CreateDirectory(root);
			var maina = Path.Combine(root, "Qorpent.Core.dll");
			if (!File.Exists(maina) || File.GetLastWriteTime(maina) < File.GetLastWriteTime(Assembly.GetAssembly(typeof(QlaoodBridge)).CodeBase.Replace("file:///", "")))
			{
				File.Copy(Assembly.GetAssembly(typeof(QlaoodBridge)).CodeBase.Replace("file:///", ""), maina, true);
			}
			var host = new TestHost();
			var loader = new ThisAssemblyLoader();
			host.ThisAssemblyLoader = loader;
			QlaoodBridge.CreateHostedDomain("test", host,root);
			Assert.NotNull(host.Interop);
			var service = (ClassToTestLoader)host.Interop.Create(typeof(ClassToTestLoader).AssemblyQualifiedName);
			Assert.AreEqual("test", service.GetDomainName());
			Assert.True(loader.WasCalled);
		}
	}
}

#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : LocalConnection.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using Qorpent.IoC;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// 	Local code connection to MVC
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "local.mvc.connection")]
	public class LocalConnection : ConnectionBase, IContainerBound {
		/// <summary>
		/// </summary>
		protected IContainer Container { get; set; }


		/// <summary>
		/// 	called on object after creation in IoC with current component context
		/// 	object can perform container bound logic here
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="component"> </param>
		public void SetContainerContext(IContainer container, IComponentDefinition component) {
			Container = container;
		}

		/// <summary>
		/// 	/called then Release is called and container process it
		/// </summary>
		public void OnContainerRelease() {}


		/// <summary>
		/// 	implement for real calling to mvc
		/// </summary>
		/// <param name="query"> </param>
		/// <returns> </returns>
		protected override MvcResult internalCall(MvcQuery query) {
			var ctx = new MvcContext();
			var sw = new StringWriter();
			ctx.Output = sw;
			query.Setup("http://localhost/", ctx);
			var handler = Container.Get<IMvcHandler>();
			var result = new MvcResult();
			try {
				handler.ProcessRequest(ctx);

				result.ActionResult = ctx.ActionResult;
				result.Content = sw.ToString();
				result.ETag = ctx.Etag;
				result.LastModified = ctx.LastModified;
				result.Status = ctx.StatusCode;
				result.Error = ctx.Error;
			}
			catch (Exception e) {
				result.Error = e;
			}
			return result;
		}
	}
}
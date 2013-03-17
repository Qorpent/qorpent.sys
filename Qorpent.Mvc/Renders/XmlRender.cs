#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Mvc/XmlRender.cs
#endregion
using System;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Renders action result as XML object
	/// </summary>
	[Render("xml")]
	public class XmlRender : SerializerRenderBase {
		/// <summary>
		/// 	return format to be used
		/// </summary>
		/// <returns> </returns>
		protected override SerializationFormat GetFormat() {
			return SerializationFormat.Xml;
		}

		/// <summary>
		/// 	output mime type
		/// </summary>
		/// <returns> </returns>
		protected override string GetContentType() {
			return "text/xml";
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			context.ContentType = "text/xml";
			//var x = new XElement("error", new XCData(error.ToString()));
			//context.Output.Write(x.ToString());
			base.RenderError(error, context);
		}
	}
}
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
// Original file : BxlRender.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Renders action result as BxlString
	/// </summary>
	[Render("bxl")]
	public class BxlRender : SerializerRenderBase {
		/// <summary>
		/// 	return format to be used
		/// </summary>
		/// <returns> </returns>
		protected override SerializationFormat GetFormat() {
			return SerializationFormat.Bxl;
		}

		/// <summary>
		/// 	output mime type
		/// </summary>
		/// <returns> </returns>
		protected override string GetContentType() {
			return "text/plain";
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			context.ContentType = "text/plain";
			var x = ResolveService<IBxlParser>().Generate(new XElement("error", new XCData(error.ToString())));
			context.Output.Write(x);
		}
	}
}
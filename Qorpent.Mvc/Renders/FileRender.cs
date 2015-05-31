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
// PROJECT ORIGIN: Qorpent.Mvc/FileRender.cs
#endregion
using System;
using System.IO;
using System.Security;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Write out file, with using action result as file name
	/// </summary>
	[Render("file", Role = "DEFAULT")]
	public class FileRender : RenderBase {
		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			var filename = context.ActionResult as string;
			if (null == filename) {
				throw new Exception("file is null");
			}
			if (!File.Exists(filename)) {
				throw new Exception("file not existed " + filename);
			}
			var extension = Path.GetExtension(filename);
			if (extension != null) {
				context.ContentType = resoleMimeType(extension.Substring(1));
			}
			if (context.ContentType == "application/bin") {
                if (!Application.Roles.IsInRole(context.User.Identity, "DEVELOPER"))
                {
					throw new SecurityException("only developers can access this file");
				}
				context.WriteOutFile(filename);
			}
			else {
				context.Output.Write(File.ReadAllText(filename));
			}
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			context.ContentType = "text/plain";
			context.Output.Write(error.ToString());
		}


		private string resoleMimeType(string ext) {
			switch (ext) {
				case "css":
					return "text/css";
				case "js":
					return "text/javascript";
				case "txt":
					return "text/plain";
				default:
					return "application/bin";
			}
		}
	}
}
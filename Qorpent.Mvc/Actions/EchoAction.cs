﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Mvc/EchoAction.cs
#endregion

using System.Text;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	QWebEchoAction (qweb/echo) return Request data wrapped
	/// 	into class and rendered by selected render
	/// 	echo intended to be used as 'ping' or
	/// 	as remote server based serializer of data
	/// </summary>
	[Action("_sys.echo",Arm="admin")]
	public class EchoAction : ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			if (Context.RenderName == "dot") {
				var result = new StringBuilder();
				result.AppendLine("digraph Echo { ");
				result.AppendLine("echo");
				foreach (var c in Context.Parameters) {
					result.AppendLine("echo->" + c.Key + "[label=\"" + c.Key + ":" + c.Value + "\"]");
				}
				result.AppendLine("}");
				return result.ToString();
			}
			return Context.Parameters;
		}
	}
}
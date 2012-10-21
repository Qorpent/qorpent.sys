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
// Original file : LoginView.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Mvc.QView;

namespace Qorpent.Mvc.Renders.LoginViews {
	/// <summary>
	/// 	Простая форма входа в систему
	/// </summary>
	[QView("_sys/login")]
	[ContainerComponent(Name = "_sys/login.code.view", Lifestyle = Lifestyle.Transient, ServiceType = typeof (IQView))
	]
	public class LoginView : QViewBase {
		/// <summary>
		/// 	Draws very simple login form
		/// </summary>
		protected override void Render() {
			var form =
				new XElement("form",
				             new XAttribute("method", "POST"),
				             new XElement("div", "Login"),
				             new XElement("input", new XAttribute("name", "_l_o_g_i_n_")),
				             new XElement("div", "Password"),
				             new XElement("input", new XAttribute("name", "_p_a_s_s_"), new XAttribute("type", "password")),
				             new XElement("br"),
				             new XElement("input", new XAttribute("type", "submit"))
					).ToString();

			write(form);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Mvc.QView;

namespace Qorpent.Mvc.Renders.LoginViews
{
	/// <summary>
	/// 	Простая форма входа в систему
	/// </summary>
	[QView("_sys/login")]
	[ContainerComponent(Name = "_sys/login.code.view", Lifestyle = Lifestyle.Transient, ServiceType = typeof(IQView))
	]
	public class LoginView : QViewBase
	{
		/// <summary>
		/// Draws very simple login form
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

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
// Original file : ActionFormParameter.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
using Qorpent.Mvc.QView;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Renders.ActionFormViews {
	/// <summary>
	/// 	Вид для отрисовки параметров
	/// </summary>
	[QView("_sys/actionformparameter")]
	[ContainerComponent(Name = "_sys/actionformparameter.code.view", Lifestyle = Lifestyle.Transient,
		ServiceType = typeof (IQView))]
	public class ActionFormParameter : QViewBase {
		/// <summary>
		/// 	Отрисовывает строку с отдельным параметром в таблице
		/// </summary>
		protected override void Render() {
			{
				var inner = new XElement("input",
				                         new XAttribute("name", p.Name ?? ""),
				                         
				                         new XAttribute("pattern", p.ValidatePattern ?? ""),
				                         new XAttribute("value", p.Default ?? ""));
				if(p.Required) {
					inner.Add(new XAttribute("required", p.Required));
				}
				if (p.IsLargeText) {
					inner.Name = "textarea";
					inner.Add(new XText("   "));
				}
				if (p.IsBool) {
					inner.SetAttributeValue("type", "checkbox");
					inner.SetAttributeValue("value", "true");
				}
				if (p.Constraint.IsNotEmpty()) {
					inner.Name = "select";
					inner.Add(new XElement("option", new XAttribute("value", "--не выбрано--")));
					foreach (var c in p.Constraint) {
						inner.Add(new XElement("option", c));
					}
				}
				if (p.TargetType.IsEnum) {
					inner.Name = "select";
					inner.Add(new XElement("option", new XAttribute("value", ""), "--не выбрано--"));
					foreach (var c in Enum.GetNames(p.TargetType)) {
						inner.Add(new XElement("option", c));
					}
				}


				write(
					new XElement("tr",
					             new XElement("td", p.Name),
					             new XElement("td", inner),
					             new XElement("td", p.Help)
						).ToString(SaveOptions.DisableFormatting)
					);
			}
		}

		/// <summary>
		/// 	Переданный на отрисовку параметр
		/// </summary>
		[QViewBind] protected BindAttribute p;
	}
}
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
				                         new XAttribute("name", Parameter.Name ?? ""),
				                         
				                         new XAttribute("value", Parameter.Default ?? ""));
				if(!string.IsNullOrWhiteSpace(Parameter.ValidatePattern)) {
					inner.Add(new XAttribute("pattern", Parameter.ValidatePattern));
				}
				if (Parameter.Required && !Parameter.IsBool) {
					inner.Add(new XAttribute("required", Parameter.Required));
				}
				if (Parameter.IsLargeText) {
					inner.Name = "textarea";
					inner.Add(new XText("   "));
				}else {
					inner.Add(new XAttribute("size","50"));
				}
				if (Parameter.IsBool) {
					inner.SetAttributeValue("type", "checkbox");
					inner.SetAttributeValue("value", "true");
				}
				if (Parameter.Constraint.IsNotEmpty()) {
					inner.Name = "select";
					inner.Add(new XElement("option", new XAttribute("value", "--не выбрано--")));
					foreach (var c in Parameter.Constraint) {
						inner.Add(new XElement("option", c));
					}
				}
				if (Parameter.TargetType.IsEnum) {
					inner.Name = "select";
					inner.Add(new XElement("option", new XAttribute("value", ""), "--не выбрано--"));
					foreach (var c in Enum.GetNames(Parameter.TargetType)) {
						inner.Add(new XElement("option", c));
					}
				}


				write(
					new XElement("tr",
					             new XElement("td", Parameter.Name),
					             new XElement("td", inner),
					             new XElement("td", Parameter.Help)
						).ToString(SaveOptions.DisableFormatting)
					);
			}
		}

		/// <summary>
		/// 	Переданный на отрисовку параметр
		/// </summary>
		[QViewBind(Name="p")] protected BindAttribute Parameter;
	}
}
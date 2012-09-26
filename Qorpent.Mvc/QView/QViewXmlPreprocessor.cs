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
// Original file : QViewXmlPreprocessor.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.IoC;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Осуществляет подмену словарных типов и обрабатывает конструкции ${}
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, "qview.dsl.xml.preprocessor")]
	public class QViewXmlPreprocessor : ServiceBase, IDslXmlPreprocessor {
		/// <summary>
		/// 	Вызывается после загрузки XML из исходного кода
		/// </summary>
		/// <param name="context"> </param>
		public void PreprocessXml(DslContext context) {
			foreach (var src in context.LoadedXmlSources) {
				Process(src.Value);
			}
		}


		private void Process(XElement value) {
			ApplyGlobalSubst(value);
			PreprocessBooLikeStringInterpolations(value);
		}

		private void ApplyGlobalSubst(XElement xml) {
			foreach (var node in xml.DescendantNodes()) {
				if (node is XElement) {
					var attrs = (node as XElement).Attributes();
					foreach (var a in attrs) {
						a.Value = GlobalSubst(a.Value);
					}
				}
				else if (node is XText) {
					(node as XText).Value = GlobalSubst((node as XText).Value);
				}
			}
		}

		private void PreprocessBooLikeStringInterpolations(XElement xml) {
			ResolveService<IXmlInterpolationPreprocessor>().Execute(xml);
		}

		private string GlobalSubst(string input) {
			return input.Replace("DICTSO", "Dictionary<string,object>")
				.Replace("DICTSS", "Dictionary<string,string>");
		}
	}
}
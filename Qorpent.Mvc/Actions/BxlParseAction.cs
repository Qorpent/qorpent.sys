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
// PROJECT ORIGIN: Qorpent.Mvc/BxlParseAction.cs
#endregion

using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Mvc.Binding;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Actions {
	/// <summary>
    /// 	Конвертирует BXL в XML
	/// </summary>
	[Action("_sys.bxlparse", Help = "Конвертирует BXL в XML", Arm="dev")]
	public class BxlParseAction : ActionBase {
		/// <summary>
        /// 	Основная фаза - тело действия
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var opts = BxlParserOptions.None;
			if (NoLexData) {
				opts = opts | BxlParserOptions.NoLexData;
			}
			if (SafeAttributes) {
				opts = opts | BxlParserOptions.SafeAttributeNames;
			}
			if (Interpolate)
			{
				opts = opts | BxlParserOptions.PerformInterpolation;
			}
			
			var xml = Context.Application.Bxl.Parse(Text, "bxlparse.action", opts);
		    if (BSharp) {
		        xml = CompileWithBSharp(xml);
		    }

		    return xml;
		}

        private static XElement CompileWithBSharp(XElement result)
        {
            var compileroptions = new BSharpConfig
            {
                UseInterpolation = true,
                SingleSource = true
            };
            var compileresult = BSharpCompiler.Compile(new[] { result }, compileroptions);
            var newresult = new XElement("bsharp");

            foreach (var w in compileresult.Get(BSharpContextDataType.Working))
            {
                var copy = new XElement(w.Compiled);
                if (null != w.Error)
                {
                    copy.AddFirst(new XElement("error", new XText(w.Error.ToString())));
                }
                newresult.Add(copy);
            }
            var e = new XElement("errors");
            foreach (var er in compileresult.GetErrors())
            {
                e.Add(XElement.Parse(new XmlSerializer().Serialize("error", er)).Element("error"));
            }
            if (e.HasElements)
            {
                newresult.Add(e);
            }
            result = newresult;
            return result;
        }

		/// <summary>
        /// Не генерировать информацию о позиции исходных файлов в BXL файле в XML
        /// </summary>
		[Bind] protected bool NoLexData;

		/// <summary>
        /// Использовать префиксы "__" стандартных атрибутов (code,name,id) вместо оригинального имени
        /// </summary>
		[Bind] protected bool SafeAttributes;
		/// <summary>
		/// Использовать интерполяцию
		/// </summary>
		[Bind] protected bool Interpolate;
		/// <summary>
		/// Использовать BSharp
		/// </summary>
		[Bind] protected bool BSharp;

		/// <summary>
		/// BXL текст
        /// </summary>
		[Bind(IsLargeText = true)] protected string Text;
	}
}
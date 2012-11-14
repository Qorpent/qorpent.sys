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
// Original file : QorpentConst.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent {
	/// <summary>
	/// 	Константы, применяемые в ядре Qorpent
	/// </summary>
	public static class QorpentConst {
		/// <summary>
		/// 	code to call reset for ioc container
		/// </summary>
		public const string ContainerResetCode = "ioc.container";

		/// <summary>
		/// 	code to call reset for FileNameResolver
		/// </summary>
		public const string FileNameResolverResetCode = "files.resolver";

		/// <summary>
		/// 	code to call reset for MvcFactory
		/// </summary>
		public const string MvcFactoryResetCode = "mvc.factory";

		/// <summary>
		/// </summary>
		public const string DefaultIocContainerTypeName = "Qorpent.IoC.Container, Qorpent.IoC";

		/// <summary>
		/// 	Язык по умолчанию приянтый в Qorpent
		/// </summary>
		public const string DefaultLanguage = "ru";

		/// <summary>
		/// 	Default split symbol set for Spliting list
		/// </summary>
		public static readonly char[] DefaultSplitters = new[] {',', ';', '/'};

		#region Nested type: Config

		/// <summary>
		/// 	configuration-bound constants
		/// </summary>
		public static class Config {
			/// <summary>
			/// 	point to override default application class in configuration file
			/// </summary>
			public const string ApplicationTypeAppSetting = "qorpent.application.type";

			/// <summary>
			/// </summary>
			public const string IocContainerTypeAppSetting = "qorpent.ioc.container.type";
		}

		#endregion

		#region Nested type: Date

		/// <summary>
		/// 	DateTime-bound consts
		/// </summary>
		public static class Date {
			/// <summary>
			/// 	Logical start (minimal date) , conformant to Unix standard (1900-1-1)
			/// </summary>
			public static readonly DateTime Begin = new DateTime(1900, 1, 1);

			/// <summary>
			/// 	Logical finish (max date) , conformant to sql limitations and reasonable max (3000-1-1)
			/// </summary>
			public static readonly DateTime End = new DateTime(3000, 1, 1);

			/// <summary>
			/// 	Standard date formats for usage when standard DateTime.Parse fails
			/// 	most of cases are of Ru style and It sortable ones
			/// </summary>
			public static readonly string[] StandardDateFormats = new[]
				{
					"dd.MM.yyyy HH:mm", "dd.MM.yyyy", "yyyy-MM-dd HH:mm",
					"yyyy-MM-dd", "yyyyMMddHHmm", "yyyyMMddHHmmss"
				};
		}

		#endregion

		#region Nested type: Xml

		/// <summary>
		/// 	Xml-bound constants
		/// </summary>
		public static class Xml {
			/// <summary>
			/// 	Пространство имен для расширений SmartXslt - import,include,param,extension, применяется в <see
			/// 	 href="Qorpent.Dsl~Qorpent.Dsl.SmartXslt.XsltHelper" />
			/// </summary>
			public const string SmartXsltNamespace = "http://qorpent/xml/xslt/extensions";

			/// <summary>
			/// 	Пространство имен для расширений XmlInclude - import,include, применяется в <see
			/// 	 href="Qorpent.Dsl~Qorpent.Dsl.XmlIncludeProcessor" />
			/// </summary>
			public const string XmlIncludeNamespace = "http://qorpent/xml/include";

			/// <summary>
			/// 	oficial XSLT namespace
			/// </summary>
			public const string XsltNameSpace = "http://www.w3.org/1999/XSL/Transform";

			/// <summary>
			/// </summary>
			public const string CSharpDslExtensionNameSpace = "http://qorpent/dsl/csharp";

			/// <summary>
			/// </summary>
			public const string SqlDslExtensionNameSpace = "http://qorpent/dsl/sql";

			/// <summary>
			/// 	name for qxi:include element - includes query result or full document instead if
			/// 	self
			/// </summary>
			public static readonly XName XmlIncludeIncludeElementName = "{" + XmlIncludeNamespace + "}" + "include";

			/// <summary>
			/// 	name for qxi:import element - import behavior imports inner content of single element into containing element,
			/// 	include attribute merging child-win
			/// </summary>
			public static readonly XName XmlIncludeImportElementName = "{" + XmlIncludeNamespace + "}" + "import";

			/// <summary>
			/// 	name for qxi:safeimport element - import behavior imports inner content of single element into containing element,
			/// 	include attribute merging parent-win
			/// </summary>
			public static readonly XName XmlIncludeSafeImportElementName = "{" + XmlIncludeNamespace + "}" + "safeimport";

			/// <summary>
			/// 	name for qxi:template элемент - позволяет описывать шаблоны элементов по имени для последущей замены
			/// </summary>
			public static readonly XName XmlIncludeTemplateElementName = "{" + XmlIncludeNamespace + "}" + "template";

			/// <summary>
			/// 	name for qxi:replace element - описывает текстовые подстановки для имен атрибутов и для текстовых элементов
			/// </summary>
			public static readonly XName XmlIncludeReplaceElementName = "{" + XmlIncludeNamespace + "}" + "replace";

			/// <summary>
			/// 	name for qi:import|qi:include delay attribute
			/// </summary>
			public static readonly XName XmlIncludeDelayAttributeName = "delay";

			/// <summary>
			/// 	name for qi:import|qi:include href attribute
			/// </summary>
			public static readonly XName XmlIncludeHrefAttributeName = "href";

			/// <summary>
			/// 	name for qi:import|qi:include query attribute
			/// </summary>
			public static readonly XName XmlIncludeQueryAttributeName = "query";


			/// <summary>
			/// 	namespaces and prefixes which can processed implicitly
			/// </summary>
			public static readonly IDictionary<string, string> WellKnownNamespaces
				= new Dictionary<string, string>
					{
						{XsltNameSpace, "xsl"},
						{XmlIncludeNamespace, "qxi"},
						{SmartXsltNamespace, "qsx"},
					};


			/// <summary>
			/// 	name for sxslt:import element
			/// </summary>
			public static readonly XName SmartXsltImportElementName = "{" + SmartXsltNamespace + "}" + "import";

			/// <summary>
			/// 	name for sxslt:include element
			/// </summary>
			public static readonly XName SmartXsltIncludeElementName = "{" + SmartXsltNamespace + "}" + "include";

			/// <summary>
			/// 	name for sxslt:param element
			/// </summary>
			public static readonly XName SmartXsltParamElementName = "{" + SmartXsltNamespace + "}" + "param";

			/// <summary>
			/// 	name for sxslt:extension element
			/// </summary>
			public static readonly XName SmartXsltExtensionElementName = "{" + SmartXsltNamespace + "}" + "extension";
		}

		#endregion
	}
}
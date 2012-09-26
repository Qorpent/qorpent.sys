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
// Original file : IBxlParser.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.Bxl {
	///<summary>
	///	���������� BXL ������� � ����������
	///</summary>
	///<qorpentimplemented ref="Qorpent.Bxl~Qorpent.Bxl.BxlParser">BxlParser</qorpentimplemented>
	///<source>Qorpent/Qorpent.Core/Bxl/IBxlParser.cs</source>
	///<remarks>
	///	����������� IBxlParser �� ��������� �������� <see href="Qorpent.Bxl~Qorpent.Bxl.BxlParser">BxlParser</see>.<br />
	///	������ ������������� ������������ ������ � IBxlParser ����� ���������� ��� ���������.<br />
	///	���� � �������� ��������� ������� ������������� ����������, ��� ����� ���������� �������� ����������� ����������
	///	������� �� <see cref="Parse" />, <see cref="Generate" />, <see cref="BxlParserOptions" />
	///	<h3>����������� � ����������</h3>
	///	������������� IBxlParser �������������, ��� ��������� ����������� ������������ ����� BXL 2.0, ������������ � 
	///	<see href="https://docs.google.com/document/d/1TPOeYyc3QOOY6-3KjqZeD4cYKn7BbL6wjNkzN27NHVQ/edit?pli=1" target="_blank">����������� �� BXL/TBXL</see>
	///	<br />
	///	��� ��� ��������� ������� ������������ ������� ������� ��������� Qorpent, �� ������� ����������� ��� ��������� ��������������� ���������� IBxlParser
	///	�� Qorpent.Bxl.dll. ����������� ����� ���� ���������� ������ <see href="Qorpent.Bxl~Qorpent.Bxl.BxlParser">BxlParser</see> � ������ �����������
	///	������������� � �������� �������.
	///</remarks>
	///<example>
	///	������� ������� � ����������:
	///	<br />
	///	1. ������ �������� BxlParser (�� �������������!)
	///	<code>//����� Qorpent.Bxl.dll ������ ���� ���� ����������� � ������ �������
	///		using Qorpent.Bxl;
	///		...
	///		IBxlParser myparser = new BxlParser();</code>
	///	2. ������ ����� ��������� ���������� �� ��������� (��� ���������� ���������� � ������� �������)
	///	<code lang="C#">using Qorpent.Bxl;
	///		using Qorpent.Applications;
	///		...
	///		IBxlParser myparser = Application.Current.Bxl; //������ � ����������
	///		IBxlParser myparser2 = Application.Current.Bxl.GetParser(); //��������� ����������� �����
	///		IBxlParser myparser3 = Application.Current.Container.Get&lt;IBxlParser&gt;(); // ���������� ����������� �������</code>
	///	3. ��� ���������� � ���������� <see cref="ServiceBase" />, �������  <see
	///	  href="Qorpent.Mvc~Qorpent.Mvc.ActionBase.html">ActionBase</see>, <see
	///	 href="Qorpent.Mvc~Qorpent.Mvc.QView.QViewBase.html">QViewBase</see>
	///	� �.�.
	///	<code>using Qorpent.Bxl;
	///		...
	///		IBxlParser myparser = ResolveService&lt;IBxlParser&gt;();</code>
	///	4. ��� �������������� ��������� � ������� ������� ������� ��� �������� �� ����������
	///	<code>using Qorpent.Bxl;
	///		using Qorpent.IoC;
	///		...
	///		[
	///		<see cref="InjectAttribute">Inject</see>
	///		]
	///		public IBxlParser MyParser {get;set;}</code>
	///</example>
	///<example>
	///	������� ������ ������ XML �� ���� Bxl (��������� �� ���������):
	///	<code>using Qorpent.Bxl;
	///		using Qorpent.Bxl;
	///		var myxml = Application.Current.Bxl.Parse(@"
	///		thema X 
	///		a = 23
	///		");
	///		// myxml ������
	///		// &lt;root>
	///		//		&lt;thema a="23" code="X" id="X" _file="code.bxl" _line="1"/>
	///		// &lt;/root></code>
	///</example>
	///<example>
	///	������� ������ ������ XML � ���� BXL
	///	<code>/// using Qorpent.Bxl;
	///		using Qorpent.Bxl;
	///		using System.Xml.Linq;
	///		var mybxl = Application.Current.Bxl.Generate(XElement.Parse("
	///		<root>
	///			<thema code="X" a="23" />
	///		</root>
	///		");
	///		// mybxl == @"
	///		//    thema X
	///		//         a=23
	///		// "</code>
	///</example>
	///<seealso cref="BxlParserOptions" />
	///<seealso cref="BxlGeneratorOptions" />
	public interface IBxlParser {
		/// <summary>
		/// 	Perform Bxl parsing of given code
		/// </summary>
		/// <param name="code"> source code (null to read from file) </param>
		/// <param name="filename"> filename for lexinfo </param>
		/// <param name="options"> BxlParsing options </param>
		/// <returns> XElement with xml equivalent </returns>
		XElement Parse(string code = null, string filename = "code.bxl", BxlParserOptions options = BxlParserOptions.None);

		/// <summary>
		/// 	Generates BXL code from XML with given settings
		/// </summary>
		/// <param name="sourcexml"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		string Generate(XElement sourcexml, BxlGeneratorOptions options = null);
	}
}
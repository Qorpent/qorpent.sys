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
// PROJECT ORIGIN: Qorpent.Dsl/JsonToXmlParser.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.Dsl {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, Name = "json.xml.parser")]
	public class JsonToXmlParser : ISpecialXmlParser {
		/// <summary>
		/// 	Gets the current.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private XElement current {
			get { return els.Peek(); }
		}

		/// <summary>
		/// 	Gets the stage.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private ParseStage stage {
			get { return ss.Peek(); }
		}


		/// <summary>
		/// 	Parses the specified json.
		/// </summary>
		/// <param name="json"> The json. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public XElement Parse(string json) {
			var r = pushe(new XElement("root"));
			pushs(ParseStage.Start);
			executeParsing(json);
			return r.Elements().First();
		}


		/// <summary>
		/// 	Executes the parsing.
		/// </summary>
		/// <param name="json"> The json. </param>
		/// <remarks>
		/// </remarks>
		private void executeParsing(string json) {
			foreach (var c in json) {
				processToken(c);
			}
			if (ParseStage.InLiteral == stage) {
				endLiteral();
			}
			else if (ParseStage.InDigit == stage) {
				endDigit();
			}
			else if (ParseStage.InString == stage) {
				throw new JsonParserException("string not closed");
			}
		}

		/// <summary>
		/// 	Ends the digit.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void endDigit() {
			lastproduction = currentDigit;
			pops();
			if (currentDigit == "-") {
				throw new JsonParserException("single minus sign is not valid number");
			}
			if (currentDigit.EndsWith(".")) {
				throw new JsonParserException("number cannot ends with dot");
			}
			if (ParseStage.AfterOpenBlock == stage || ParseStage.AfterComma == stage) {
				pushe(new XElement("prop", new XAttribute("name", currentDigit)));
				pushs(ParseStage.AfterPropName);
				return;
			}

			tryFinishValue(lastproduction);
		}

		/// <summary>
		/// 	Tries the finish value.
		/// </summary>
		/// <param name="val"> The val. </param>
		/// <remarks>
		/// </remarks>
		private void tryFinishValue(string val) {
			if (ParseStage.Start == stage || ParseStage.AfterColumn == stage) {
				pushe(new XElement("value", lastproduction));
				pops();
			}
			if (ParseStage.AfterPropName == stage) {
				pope();

				var p = pope();
				if (null == p.Attribute("name")) {
					throw new JsonParserException("some syntax error");
				}
				var name = p.Attribute("name").Value;
				if (char.IsDigit(name[0])) {
					name = "_" + name;
				}
				p.Remove();
				current.Add(new XAttribute(name, val));

				pops();
				pushs(ParseStage.AfterValue);
				return;
			}

			if (ParseStage.AfterOpenArray == stage || ParseStage.AfterArrayComma == stage) {
				pushe(new XElement("value", lastproduction));
				pope();
				pushs(ParseStage.AfterArrayValue);
				return;
			}
		}

		/// <summary>
		/// 	Processes the token.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processToken(char c) {
			switch (c) {
				case '\r':
					if (stage == ParseStage.InString) {
						throw new JsonParserException("\\r in string");
					}
					goto case ' ';
				case '\n':
					if (stage == ParseStage.InString) {
						throw new JsonParserException("\\n in string");
					}
					goto case ' ';
				case '\t':
					goto case ' ';
				case ' ':
					if (stage == ParseStage.End) {
						return;
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processWhiteSpace(c);
					break;
				case '\\':
					processEscaper(c);
					break;
				case '1':
					goto case '0';
				case '2':
					goto case '0';
				case '3':
					goto case '0';
				case '4':
					goto case '0';
				case '5':
					goto case '0';
				case '6':
					goto case '0';
				case '7':
					goto case '0';
				case '8':
					goto case '0';
				case '9':
					goto case '0';
				case '0':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processDigit(c);
					break;
				case '"':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					processQuot(c);
					break;
				case '{':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processOpenBlock(c);
					break;
				case '}':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processCloseBlock(c);
					break;
				case '[':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processOpenArray(c);
					break;
				case ']':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processCloseArray(c);
					break;
				case '.':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processDot(c);
					break;
				case ':':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processColumn(c);
					break;
				case ',':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processComma(c);
					break;
				case '-':
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					if (stage == ParseStage.InString) {
						goto default;
					}
					processMinus(c);
					break;
				default:
					if (stage == ParseStage.End) {
						throw new JsonParserException("symbols after end");
					}
					processSymbol(c);
					return;
			}
		}

		/// <summary>
		/// 	Processes the minus.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processMinus(char c) {
			if (ParseStage.Start == stage || ParseStage.AfterColumn == stage) {
				beginDidgit(c);
				return;
			}
			throw new JsonParserException("illegal place for minus");
		}

		/// <summary>
		/// 	Processes the column.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processColumn(char c) {
			if (ParseStage.InLiteral == stage) {
				endLiteral();
			}
			else if (ParseStage.InDigit == stage) {
				endDigit();
			}
			if (ParseStage.AfterPropName == stage) {
				pushs(ParseStage.AfterColumn);
				return;
			}
			throw new JsonParserException("illegal column position");
		}

		/// <summary>
		/// 	Processes the comma.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processComma(char c) {
			var wasvalue = false;
			if (ParseStage.InDigit == stage) {
				endDigit();
				wasvalue = true;
			}
			if (ParseStage.InLiteral == stage) {
				endLiteral();
				wasvalue = true;
			}
			if (ParseStage.AfterValue == stage) {
				pushs(ParseStage.AfterComma);
				return;
			}
			if (ParseStage.AfterArrayValue == stage || (ParseStage.AfterOpenArray == stage && wasvalue)) {
				if (ParseStage.AfterArrayValue == stage) {
					pops();
				}
				pushs(ParseStage.AfterArrayComma);
				return;
			}

			throw new JsonParserException("illegal comma position");
		}

		/// <summary>
		/// 	Processes the dot.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processDot(char c) {
			if (ParseStage.InDigit == stage) {
				if (currentDigit == "-") {
					throw new JsonParserException("need digit after minus");
				}
				if (wasdot) {
					throw new JsonParserException("double dot in digit");
				}
				wasdot = true;
				currentDigit += c;
				return;
			}
			throw new JsonParserException("illegal place for dot");
		}

		/// <summary>
		/// 	Processes the close array.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processCloseArray(char c) {
			if (ParseStage.InLiteral == stage) {
				endLiteral();
			}
			if (ParseStage.InDigit == stage) {
				endDigit();
			}

			if (ParseStage.AfterArrayComma == stage || ParseStage.AfterArrayValue == stage || ParseStage.AfterOpenBlock == stage) {
				closeArray();
				return;
			}
			throw new JsonParserException("illegal close array position");
		}

		/// <summary>
		/// 	Closes the array.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void closeArray() {
			pops();
			var x = pope();
			if (ParseStage.AfterColumn == stage || ParseStage.AfterOpenArray == stage) {
				pushs(ParseStage.AfterValue);
			}
			if (x.Name.LocalName == "array") {
				x = pope();
				pops();
				pushs(ParseStage.AfterValue);
			}

			tryCollapseProperty(x);
		}

		/// <summary>
		/// 	Tries the collapse property.
		/// </summary>
		/// <param name="x"> The x. </param>
		/// <remarks>
		/// </remarks>
		private void tryCollapseProperty(XElement x) {
			if (x.Name.LocalName == "prop") {
				var n = x.Attribute("name").Value;
				if (char.IsDigit(n[0])) {
					n = "_" + n;
				}
				var e = new XElement(n);
				var src = x.Elements().First();
				foreach (var attribute in src.Attributes()) {
					e.Add(attribute);
				}
				foreach (var element in src.Elements()) {
					e.Add(element);
				}
				x.ReplaceWith(e);
			}
		}

		/// <summary>
		/// 	Processes the open array.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processOpenArray(char c) {
			if (ParseStage.Start == stage || ParseStage.AfterColumn == stage) {
				startArray();
				return;
			}
			throw new JsonParserException("illegal open array place");
		}

		/// <summary>
		/// 	Starts the array.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void startArray() {
			pushe(new XElement("array"));
			pushs(ParseStage.AfterOpenArray);
		}

		/// <summary>
		/// 	Processes the close block.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processCloseBlock(char c) {
			if (ParseStage.InLiteral == stage) {
				endLiteral();
			}
			if (ParseStage.InDigit == stage) {
				endDigit();
			}

			if (ParseStage.AfterComma == stage || ParseStage.AfterValue == stage || ParseStage.AfterOpenBlock == stage) {
				closeBlock();
				return;
			}
			throw new JsonParserException("illegal close block position");
		}

		/// <summary>
		/// 	Closes the block.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void closeBlock() {
			pops();
			var x = pope();
			if (ParseStage.AfterColumn == stage || ParseStage.AfterOpenBlock == stage) {
				var pr = prev();
				if (ParseStage.AfterArrayComma == pr || ParseStage.AfterOpenArray == pr) {
					pushs(ParseStage.AfterArrayValue);
				}
				else {
					pushs(ParseStage.AfterValue);
				}
			}
			if (x.Name.LocalName == "object") {
				x = pope();
				pops();
				var pr = prev();
				if (ParseStage.AfterArrayComma == pr || ParseStage.AfterOpenArray == pr) {
					pushs(ParseStage.AfterArrayValue);
				}
				else {
					pushs(ParseStage.AfterValue);
				}
			}
			tryCollapseProperty(x);
		}

		/// <summary>
		/// 	Processes the open block.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processOpenBlock(char c) {
			if (ParseStage.Start == stage || ParseStage.AfterColumn == stage || ParseStage.AfterOpenArray == stage ||
			    ParseStage.AfterArrayComma == stage) {
				startBlock();
				return;
			}
			throw new JsonParserException("illegal open block place");
		}

		/// <summary>
		/// 	Starts the block.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void startBlock() {
			pushe(new XElement("object"));
			pushs(ParseStage.AfterOpenBlock);
		}

		/// <summary>
		/// 	Processes the quot.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processQuot(char c) {
			if (ParseStage.Escaped == stage) {
				pops();
				appendString("\"");
				return;
			}
			if (ParseStage.InString == stage) {
				endString();
				return;
			}
			beginString();
		}

		/// <summary>
		/// 	Begins the string.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void beginString() {
			pushs(ParseStage.InString);
			currentString = "";
		}

		/// <summary>
		/// 	Ends the string.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void endString() {
			lastproduction = currentString;
			pops();
			if (ParseStage.AfterOpenBlock == stage || ParseStage.AfterComma == stage) {
				pushe(new XElement("prop", new XAttribute("name", currentString)));
				pushs(ParseStage.AfterPropName);
				return;
			}

			tryFinishValue(lastproduction);
		}

		/// <summary>
		/// 	Processes the digit.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processDigit(char c) {
			if (ParseStage.InDigit == stage) {
				currentDigit += c;
				return;
			}
			if (ParseStage.InLiteral == stage) {
				currentLiteral += c;
				return;
			}
			beginDidgit(c);
		}

		/// <summary>
		/// 	Begins the didgit.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void beginDidgit(char c) {
			pushs(ParseStage.InDigit);
			currentDigit = c.ToString();
			wasdot = false;
		}

		/// <summary>
		/// 	Processes the symbol.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processSymbol(char c) {
			switch (stage) {
				case ParseStage.Start:
					goto case ParseStage.AfterColumn;
				case ParseStage.AfterOpenBlock:
					goto case ParseStage.AfterColumn;
				case ParseStage.AfterOpenArray:
					goto case ParseStage.AfterColumn;
				case ParseStage.AfterArrayComma:
					goto case ParseStage.AfterColumn;
				case ParseStage.AfterComma:
					goto case ParseStage.AfterColumn;
				case ParseStage.AfterColumn:
					startLiteral(c);
					break;
				case ParseStage.InString:
					appendString(c);
					break;
				case ParseStage.InLiteral:
					appendLiteral(c);
					break;
			}
		}

		/// <summary>
		/// 	Appends the literal.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void appendLiteral(char c) {
			if (!char.IsLetterOrDigit(c) && c != '_') {
				throw new JsonParserException("literal cannot contains with non-letter-dig symbol " + c);
			}
			currentLiteral += c;
		}

		/// <summary>
		/// 	Appends the string.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void appendString(string c) {
			currentString += c;
		}

		/// <summary>
		/// 	Appends the string.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void appendString(char c) {
			currentString += c;
		}

		/// <summary>
		/// 	Starts the literal.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void startLiteral(char c) {
			currentLiteral = "";
			if (!char.IsLetter(c) && c != '_') {
				throw new JsonParserException("literal cannot start with non-letter symbol " + c);
			}
			currentLiteral += c;
			pushs(ParseStage.InLiteral);
		}

		/// <summary>
		/// 	Processes the escaper.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processEscaper(char c) {
			if (stage == ParseStage.Escaped) {
				pops();
				appendString('\\');
			}
			else {
				pushs(ParseStage.Escaped);
			}
		}

		/// <summary>
		/// 	Processes the white space.
		/// </summary>
		/// <param name="c"> The c. </param>
		/// <remarks>
		/// </remarks>
		private void processWhiteSpace(char c) {
			if (stage == ParseStage.InLiteral) {
				endLiteral();
			}
			else if (stage == ParseStage.InDigit) {
				endDigit();
			}
		}

		/// <summary>
		/// 	Ends the literal.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private void endLiteral() {
			lastproduction = currentLiteral;
			pops();
			if (ParseStage.AfterOpenBlock == stage || ParseStage.AfterComma == stage) {
				pushe(new XElement("prop", new XAttribute("name", currentLiteral)));
				pushs(ParseStage.AfterPropName);
				return;
			}

			tryFinishValue(lastproduction);
		}


		/// <summary>
		/// 	Pushes the specified e.
		/// </summary>
		/// <param name="e"> The e. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private XElement pushe(XElement e) {
			if (els.Count != 0 && null != current) {
				current.Add(e);
			}
			els.Push(e);
			return e;
		}

		/// <summary>
		/// 	Popes this instance.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private XElement pope() {
			return els.Pop();
		}

		/// <summary>
		/// 	Pushses the specified stage.
		/// </summary>
		/// <param name="stage"> The stage. </param>
		/// <remarks>
		/// </remarks>
		private void pushs(ParseStage stage) {
			ss.Push(stage);
		}

		/// <summary>
		/// 	Prevs this instance.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private ParseStage prev() {
			if (ss.Count < 2) {
				return ParseStage.None;
			}
			var s = pops();
			var res = stage;
			pushs(s);
			return res;
		}

		/// <summary>
		/// 	Popses this instance.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private ParseStage pops() {
			var r = ss.Pop();
			if (ParseStage.Start == r) {
				ss.Push(ParseStage.End);
				return ParseStage.End;
			}
			return r;
		}

		#region Nested type: JsonParserException

		/// <summary>
		/// </summary>
		/// <remarks>
		/// </remarks>
		[Serializable]
		public class JsonParserException : Exception {
			/// <summary>
			/// 	Initializes a new instance of the <see cref="JsonParserException" /> class.
			/// </summary>
			/// <param name="message"> The message. </param>
			/// <remarks>
			/// </remarks>
			public JsonParserException(string message) : base(message) {}

			/// <summary>
			/// 	Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.
			/// </summary>
			/// <param name="info"> The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
			/// <param name="context"> The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
			/// <exception cref="T:System.ArgumentNullException">The
			/// 	<paramref name="info" />
			/// 	parameter is null.</exception>
			/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or
			/// 	<see cref="P:System.Exception.HResult" />
			/// 	is zero (0).</exception>
			/// <remarks>
			/// </remarks>
			protected JsonParserException(
				SerializationInfo info,
				StreamingContext context) : base(info, context) {}
		}

		#endregion

		#region Nested type: ParseStage

		/// <summary>
		/// </summary>
		/// <remarks>
		/// </remarks>
		private enum ParseStage {
			/// <summary>
			/// </summary>
			Start,

			/// <summary>
			/// </summary>
			InString,

			/// <summary>
			/// </summary>
			AfterOpenBlock,

			/// <summary>
			/// </summary>
			InLiteral,

			/// <summary>
			/// </summary>
			AfterColumn,

			/// <summary>
			/// </summary>
			AfterComma,

			/// <summary>
			/// </summary>
			AfterValue,

			/// <summary>
			/// </summary>
			End,

			/// <summary>
			/// </summary>
			Escaped,

			/// <summary>
			/// </summary>
			AfterPropName,

			/// <summary>
			/// </summary>
			InDigit,

			/// <summary>
			/// </summary>
			AfterOpenArray,

			/// <summary>
			/// </summary>
			AfterArrayValue,

			/// <summary>
			/// </summary>
			AfterArrayComma,

			/// <summary>
			/// </summary>
			None
		}

		#endregion

		/// <summary>
		/// </summary>
		private readonly Stack<XElement> els = new Stack<XElement>();

		/// <summary>
		/// </summary>
		private readonly Stack<ParseStage> ss = new Stack<ParseStage>();

		/// <summary>
		/// </summary>
		private string currentDigit = "";

		/// <summary>
		/// </summary>
		private string currentLiteral = "";

		/// <summary>
		/// </summary>
		private string currentString = "";

		/// <summary>
		/// </summary>
		private string lastproduction;

		/// <summary>
		/// </summary>
		private bool wasdot;
	}
}
#region LICENSE
// Copyright 2007-2014 Qorpent Team - http://github.com/Qorpent
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
#endregion
#region INFO
/* MODULE INFORMATION :
 * NAME : ConsoleArgumentsReader
 * 
 * This module provides scan support for parsing console arguments strings in following format:
 * ARGLINE = ARGUMENT*
 * ARGUMENT = WS+( NAMEDARGUMENT | FLAGATTRIBUTE | NUMEREDATTRIBUTE )
 * WS = [ SPACE | TAB ]
 * NAMEDARGUMENT = PREFIX ARGUMENTNAME VALUE  #named argument with value
 * FLAGATTRIBUTE = { PREFIX [ARGUMENTNAME] } # named argument flag, VALUE==true automatically  
 * NUMEREDATTRIBUTE = { VALUE } # attribute without name, numer of position among other numered will be used
 * PREFIX = "--"
 * ARGUMENTNAME = LITERAL
 * LITERAL = LETTER (LETTER | DIGIT | LITERALSYMBOL)*
 * LETTER = [a..z|A..Z]
 * DIGIT = [0..9]
 * LITERLSYMBOL = [ '.', '_', '-' ]
 * VALUE = !PREFIX (LITERAL | STRING)
 * STRING = " (ANY | (\") * "  
 * 
 * for example argument string :
 * myfile.txt myfile2.txt --copy-to /mydir --override --quiet -- value
 * will be treat as :
 * NUMERED(0)	==	myfile.txt
 * NUMERED(1)	==	myfile2.txt
 * copy-to		==	/mydir
 * override		==	true
 * quiet		==	true
 * [empty]		==  value # treats -- as named but 'empty name' argument
 * 
 * SUPPORTED DIRECTIVES:
 * 1) Not_Embed - marks that module are stored as part of legacy library, so it uses "export" namespace
 * 2) No_ConsoleArgumentReader - removes all ConsoleArgumentReader-bound classes, only ConsoleArgumentTokenizer still
 * 3) ConsoleArgumentsReader_NUnit - enables NUnit module tests to be compiled
 * 
 * USAGE :
 * 1) ConsoleArgumentReader is top-most class wich return Enumerable of  KeyValuePair of prepared
 * arguments, which can be parsed from string[] or string, it can be removed from
 * compilation with NO_ConsoleArgumentReader directive; in addition it has several modes of
 * work with duplicates - allow|disallow|firstwin|lastwin which can be determined on creation,
 * modes are defined in ConsoleArgumentReaderMode enumeration
 * 2) ConsoleArgumentTokenizer is internal class which parses source string or string[] and 
 * emits tokens to given delegate, you can setup self client for this emiter without using
 * ConsoleArgumentReader, can be used both synchronous or asynchronous, you can hide
 * ConsoleArgumentTokenizer with Hide_ConsoleArgumentTokenizer directive
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if ConsoleArgumentsReader_NUnit
using NUnit.Framework;
#endif

#if !DEEMBEDQPT
// ReSharper disable CheckNamespace
namespace Qorpent.Embed.Serialization
// ReSharper restore CheckNamespace
#else
namespace Qorpent.Serialization
#endif
{

#if !No_ConsoleArgumentReader
#pragma warning disable 1591
	/// <summary>
	/// Describes all Exceptions of ConsoleArgumentsReader module
	/// </summary>
	[Serializable]
	public sealed class ConsoleArgumentsReaderException : Exception
	{

		public ConsoleArgumentsReaderException()
		{

		}
		public ConsoleArgumentsReaderException(string message)
			: base(message)
		{
		}
		public ConsoleArgumentsReaderException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}//
#pragma warning restore 1591
	/// <summary>
	/// Modes for ConsoleArgumentReader
	/// </summary>
	[Flags]
	public enum ConsoleArgumentReaderMode:byte{
		/// <summary>
		/// No any modes applyed, is equal to set <see cref="Default"/> mode
		/// </summary>
		None = 0,
		/// <summary>
		/// Reader enumerates all duplicate arguments
		/// </summary>
		DuplicatesAllow = 1,
 		/// <summary>
 		/// Reader throws exception if duplicate occured
 		/// </summary>
		DuplicatesError = 2,
		/// <summary>
		/// Reader yields first occurance of argument and skips others
		/// </summary>
		DuplicatesFirstWin =4,
		/// <summary>
		/// Reader stacks all named attributes and returns last one after all is parsed
		/// </summary>
		DuplicatesLastWin = 8,
		/// <summary>
		/// By default - <see cref="DuplicatesFirstWin"/> strategy used - it allows
		/// use <see cref="ConsoleArgumentReader"/> as dictionary source,
		/// but does not cause buffering of named attributes, most of valid arg strings
		/// are intended to be unique, so DuplicatesFirstWin is most wanted mode
		/// </summary>
		Default = DuplicatesFirstWin
	}

	/// <summary>
	/// 
	/// </summary>
// ReSharper disable PartialTypeWithSinglePart
	public  sealed partial class ConsoleArgumentReader
// ReSharper restore PartialTypeWithSinglePart
	{
		private readonly ConsoleArgumentTokenizer _tokenizer;
		/// <summary>
		/// Mode of console argument reader
		/// </summary>
		readonly ConsoleArgumentReaderMode Mode;
	

		/// <summary>
		/// Creates console argument reader over string
		/// </summary>
		/// <param name="argline"></param>
		/// <param name="mode"></param>
		public ConsoleArgumentReader(string argline, ConsoleArgumentReaderMode mode = ConsoleArgumentReaderMode.Default){
			_tokenizer = ConsoleArgumentTokenizer.Create(argline);
			Mode = mode;
		}
		/// <summary>
		/// Creates console argument reader over string enumeration
		/// </summary>
		/// <param name="arglines"></param>
		/// <param name="mode"></param>
		public ConsoleArgumentReader(IEnumerable<string> arglines, ConsoleArgumentReaderMode mode = ConsoleArgumentReaderMode.Default)
		{
			_tokenizer = ConsoleArgumentTokenizer.Create(arglines);
			Mode = mode;
		}
		/// <summary>
		/// Creates console argument reader on given token
		/// </summary>
		/// <param name="tokenizer"></param>
		/// <param name="mode"></param>
		public ConsoleArgumentReader(ConsoleArgumentTokenizer tokenizer, ConsoleArgumentReaderMode mode = ConsoleArgumentReaderMode.Default)
		{
			if(null==tokenizer)throw new ConsoleArgumentsReaderException("tokenizer not defined");
			_tokenizer = tokenizer;
			Mode = mode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> Read(){
			var result = new Dictionary<string, string>();
			foreach (var pair in RawRead())
			{
				if (result.ContainsKey(pair.Key)){
					if (Mode.HasFlag(ConsoleArgumentReaderMode.DuplicatesAllow)) continue;
					if (Mode.HasFlag(ConsoleArgumentReaderMode.DuplicatesError)) throw new ConsoleArgumentsReaderException("double key "+pair.Key);
					if (Mode.HasFlag(ConsoleArgumentReaderMode.DuplicatesFirstWin)) continue;

				}
				result[pair.Key] = pair.Value;
			}
			return result;
		} 

		/// <summary>
		/// Считывает аргументы в виде перечисления пар имя-значение без контроля дубляжей
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, string>> RawRead(){
			if (!Mode.HasFlag(ConsoleArgumentReaderMode.DuplicatesAllow)){
				
			}
			foreach (var token in _tokenizer.Read()){
				foreach (var k in Emit(token)){
					yield return k;
				}
				
			}
			foreach (var k in Rest()){
				yield return k;
			}
			
		}

		IEnumerable<KeyValuePair<string, string>> Rest(){
			if (_lastToken.Type == ConsoleArgumentTokenizer.TokenType.Name){
				yield return new KeyValuePair<string, string>(_lastToken.Value, "true");
			}
		}

		private int _anoId = 1;
		private ConsoleArgumentTokenizer.Token _lastToken = ConsoleArgumentTokenizer.Token.Null;
		IEnumerable<KeyValuePair<string, string>> Emit(ConsoleArgumentTokenizer.Token token){
			if (token.Type == ConsoleArgumentTokenizer.TokenType.Name){
				if (_lastToken.Type == ConsoleArgumentTokenizer.TokenType.Name){
					yield return new KeyValuePair<string, string>(_lastToken.Value, "true");
				}
				_lastToken = token;
			}else{
				if (ConsoleArgumentTokenizer.TokenType.Null == _lastToken.Type){
					yield return new KeyValuePair<string, string>("arg"+_anoId++,token.Value);
				}else if (ConsoleArgumentTokenizer.TokenType.Name == _lastToken.Type){
					yield return new KeyValuePair<string, string>(_lastToken.Value, token.Value);
					_lastToken = ConsoleArgumentTokenizer.Token.Null;
				}
			}
		}
	}

#endif
	
	/// <summary>
	/// Lazy, pull mode (getnext()) scaner/tokenizer for argument strings or string*
	/// </summary>
// ReSharper disable PartialTypeWithSinglePart
		public abstract partial class ConsoleArgumentTokenizer{
// ReSharper restore PartialTypeWithSinglePart
#pragma warning disable 1591
			/// <summary>
			/// Describes all Exceptions of ConsoleArgumentsReader module
			/// </summary>
			[Serializable]
			protected internal sealed class ConsoleTokenizerException : Exception
			{

				public ConsoleTokenizerException()
				{

				}
				public ConsoleTokenizerException(string message)
					: base(message)
				{
				}
				public ConsoleTokenizerException(string message, Exception inner)
					: base(message, inner)
				{
				}
			}
#pragma warning restore 1591
			/// <summary>
		/// Символ префикса аргумента
		/// </summary>
		public const char ArgumentPrefixCharacter = '-';
		/// <summary>
		/// Types of token
		/// </summary>
		[Flags]
		public enum TokenType:byte{
			/// <summary>
			/// Undefined
			/// </summary>
			None = 0,
			/// <summary>
			/// Name of argument
			/// </summary>
			Name =1,
			/// <summary>
			/// Value of argument
			/// </summary>
			Value=2,
			/// <summary>
			/// Признак завершения парса
			/// </summary>
			Null = 128
		}
		/// <summary>
		/// Token structure
		/// </summary>
		public struct Token{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="type"></param>
			/// <param name="value"></param>
			public Token(TokenType type, string value){
				Type = type;
				Value = value;
			}
			/// <summary>
			/// Тип токена
			/// </summary>
			public readonly TokenType Type;
			/// <summary>
			/// Значение токена
			/// </summary>
			public readonly string Value;
			/// <summary>
			/// Стандартное значение, обозначающее окончания скана
			/// </summary>
			public static  Token Null = new Token(TokenType.Null,string.Empty);
			/// <summary>
			/// Пустое значение атрибута, для указания пустых но квортированных агрументов
			/// </summary>
			public static  Token Empty = new Token(TokenType.Value, string.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		private bool _finished;



		/// <summary>
		/// Возвращает следующий токен, должен вернуть Eol по окончании
		/// </summary>
		/// <returns></returns>
		public Token Next(){
			if (_finished) return Token.Null;
			Token token;
			_finished = !Next(out token);
			return token;
		}

		/// <summary>
		/// Выполняет возврат набора токенов в виде перечисления (для упрощения использования)
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Token> Read()
		{
			while (true)
			{
				Token token;
				var finished = Next(out token);
				if (token.Type == TokenType.Null) yield break;
				yield return token;
				if (!finished) yield break;
			}
		} 
		/// <summary>
		/// override to provide scan logic for arg helper
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		protected abstract bool Next(out Token token);
		/// <summary>
		/// Creates tokenizer for prepared enumeration of arguments, no quoting applyed
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static ConsoleArgumentTokenizer Create(IEnumerable<string> arguments){
			return new StringEnumerationTokenizer(arguments) ;
		}

		/// <summary>
		/// Implements logic over string[] 
		/// </summary>
		private sealed class StringEnumerationTokenizer:ConsoleArgumentTokenizer{
			private readonly IEnumerator<string> _enumerator; 
			
			internal StringEnumerationTokenizer(IEnumerable<string> arguments){
				_enumerator = arguments.GetEnumerator();
			}

			protected override bool Next(out Token token){
				if (!_enumerator.MoveNext())
				{
					token = Token.Null;
					return false;
				}
				var current = _enumerator.Current;
				if (current.Length == 0 || string.IsNullOrWhiteSpace(current)){// пустой параметр - игнорируем
					token = Token.Empty; //we must return it due to fact that it can be --some-var "" result
				}
				else if (current.Length == 1){
					// it cannot be valid argument name even empty
					token = new Token(TokenType.Value,current);
				}
				else{
					if (current[0] == ArgumentPrefixCharacter && current[1] == ArgumentPrefixCharacter){
						token = new Token(TokenType.Name,current.Substring(2));
					}
					else{
						token = new Token(TokenType.Value, current);
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Creates tokenizer for full command line with raw quoting
		/// </summary>
		/// <param name="argumentString"></param>
		/// <returns></returns>
		public static ConsoleArgumentTokenizer Create(string argumentString)
		{

			return new StringTokenizer(ref argumentString);
		}

		

		/// <summary>
		/// Implements string scaning logic
		/// </summary>
		private sealed class StringTokenizer:ConsoleArgumentTokenizer{
			private readonly string _argumentString;
			private int _position;
			private readonly StringBuilder _buffer = new StringBuilder();
			internal StringTokenizer(ref string argumentString){
				_argumentString = argumentString;
				
			}
			private enum State:byte{
				None,
				Prefix,
				Prefix2,
				Name,
				Value,
				Escape,
				String,
				RequireWhiteSpace
			}

			private State _state = State.None;
			

			protected override bool Next(out Token token){
				if (_position == _argumentString.Length){
					token = Token.Null;
					return false;
				}
				_buffer.Clear();
				while (_position < _argumentString.Length){
					var c = _argumentString[_position++];
					switch (c){
						case ' ':
						case '\t':
							switch (_state){
								case State.RequireWhiteSpace:
									_state = State.None;
									continue;
								case State.None:
									continue;
								case State.Prefix:
									token = new Token(TokenType.Value, "-");
									_state = State.None;
									return true;
								case State.Prefix2:
									token = new Token(TokenType.Name, string.Empty);
									_state = State.None;
									return true;
								case State.Name:
									token = new Token(TokenType.Name, _buffer.ToString());
									_state = State.None;
									return true;
								case State.Value:
									token = new Token(TokenType.Value, _buffer.ToString());
									_state = State.None;
									return true;
								default:
									_buffer.Append(c);
									continue;
							}
						case '-':
							switch (_state){
								case State.RequireWhiteSpace:
									throw new ConsoleTokenizerException("Require whitespace at "+_position);
								case State.None:
									_state = State.Prefix;
									continue;
								case State.Prefix:
									_state = State.Prefix2;
									continue;
								default:
									_buffer.Append(c);
									continue;
							}
						case '"':
							switch (_state){
								case State.RequireWhiteSpace:
									throw new ConsoleTokenizerException("Require whitespace at "+_position);
								case State.None:
									_state = State.String;
									continue;
								case State.Prefix:
								case State.Prefix2:
								case State.Name:
									throw new ConsoleTokenizerException("Quote in prefix or names " + _position);
								case State.String:
									token = new Token(TokenType.Value, _buffer.ToString());
									_state = State.RequireWhiteSpace;
									return true;
								case State.Value:
									throw new ConsoleTokenizerException("Quote are illegal in values " + _position);
								case State.Escape:
									_buffer.Append(c);
									_state =State.String;
									continue;
							}
							break;
						case '\\': //can escapes " in strings - otherwise treats as illegal in names and legal in values
							switch (_state){
								case State.RequireWhiteSpace:
									throw new ConsoleTokenizerException("Require whitespace at "+_position);
								case State.None:
									_state = State.Value;
									_buffer.Append(c);
									continue;
								case State.Prefix:
									_state = State.Value;
									_buffer.Append('-');
									_buffer.Append(c);
									continue;
								case State.Prefix2:
								case State.Name:
									throw new ConsoleTokenizerException("Escapes cannot be in names " + _position);
								case State.Value:
									_buffer.Append(c);
									continue;
								case State.Escape:
									_buffer.Append('\\'); //self escaping
									_state = State.String;
									continue;
								case State.String:
									_state = State.Escape;
									continue;
							}
							break;
						default:
							switch (_state){
								case State.RequireWhiteSpace:
									throw new ConsoleTokenizerException("Require whitespace at "+_position);
								case State.None:
									_state = State.Value;
									_buffer.Append(c);
									continue;
								case State.Prefix:
									_state = State.Value;
									_buffer.Append('-');
									_buffer.Append(c);
									continue;
								case State.Prefix2:
									_state = State.Name;
									_buffer.Append(c);
									continue;
								case State.Escape: //break escaping
									_state = State.String;
									_buffer.Append('\\');
									_buffer.Append(c);
									continue;
								default:
									_buffer.Append(c);
									continue;
							}


					}


				}
				if (0 != _buffer.Length){
					switch (_state){
						case State.Prefix:
							token = new Token(TokenType.Value, "-");
							return false;
						case State.Prefix2:
							token = new Token(TokenType.Name, string.Empty);
							return false;
						case State.Value:
							token = new Token(TokenType.Value, _buffer.ToString());
							return false;
						case State.Name:
							token = new Token(TokenType.Name, _buffer.ToString());
							return false;
						case State.Escape:
						case State.String:
							throw new Exception("not terminated string at end");
						default:
							throw new Exception("illegal state at end");

					}
				}
				token = Token.Null;
				return false;
			}

		
		}
	}



#if ConsoleArgumentsReader_NUnit
	
#if !No_ConsoleArgumentReader
#endif

	/// <summary>
	/// Tests for native string scaning
	/// </summary>
	[TestFixture]
	public class ConsoleArgumentTokenizerForStringTests{
		/// <summary>
		/// Tests that tokenizer can parse single literal argument
		/// </summary>
		/// <param name="argstring"></param>
		[TestCase("1")]
		[TestCase("_")]
		[TestCase("abc")]
		[TestCase("\\abc/")]
		public void CanParseSimpleValues(string argstring){
			var t = ConsoleArgumentTokenizer.Create(argstring);
			var fst = t.Next();
			var sec = t.Next();
			Assert.AreEqual(ConsoleArgumentTokenizer.Token.Null,sec,"does not sygnal end of parsing");
			Assert.AreEqual(ConsoleArgumentTokenizer.TokenType.Value,fst.Type,"invalid type returned "+fst.Type);
			Assert.AreEqual(argstring,fst.Value,"invalid value returned "+fst.Value);
		}

		

		/// <summary>
		/// Tests that tokenizer can parse single string argument (enquoted in test)
		/// </summary>
		/// <param name="argstring"></param>
		[TestCase("1")]
		[TestCase("_")]
		[TestCase("a b c")]
		[TestCase("\\a\tb\tc/")]
		[TestCase("\\a")]
		public void CanParseSimpleStringValues(string argstring)
		{
			var t = ConsoleArgumentTokenizer.Create("\""+argstring+"\"");
			var fst = t.Next();
			var sec = t.Next();
			Assert.AreEqual(ConsoleArgumentTokenizer.Token.Null, sec, "does not sygnal end of parsing");
			Assert.AreEqual(ConsoleArgumentTokenizer.TokenType.Value, fst.Type, "invalid type returned " + fst.Type);
			Assert.AreEqual(argstring, fst.Value, "invalid value returned " + fst.Value);
		}

		/// <summary>
		/// Tests that tokenizer can parse single string with escapes
		/// </summary>
		/// <param name="argstring"></param>
		/// <param name="result"></param>
		[TestCase("\\\"","\"")]
		[TestCase("\\\\","\\")]
		[TestCase("\\a","\\a")]
		public void CanParseStringWithEscapes(string argstring, string result)
		{
			var t = ConsoleArgumentTokenizer.Create("\"" + argstring + "\"");
			var fst = t.Next();
			var sec = t.Next();
			Assert.AreEqual(ConsoleArgumentTokenizer.Token.Null, sec, "does not sygnal end of parsing");
			Assert.AreEqual(ConsoleArgumentTokenizer.TokenType.Value, fst.Type, "invalid type returned " + fst.Type);
			Assert.AreEqual(result, fst.Value, "invalid value returned " + fst.Value);
		}

		/// <summary>
		/// Tests parsing of named argument
		/// </summary>
		/// <param name="argstring"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		[TestCase("--x 1", "x", "1")]
		[TestCase("--x\t 1", "x", "1")]
		[TestCase("--x  1", "x", "1")]
		[TestCase("--x-a \"\\\"1+1\\\"\"", "x-a", "\"1+1\"")]
		public void CanParseSingleNamedArgument(string argstring, string name, string value)
		{
			var t = ConsoleArgumentTokenizer.Create(argstring);
			var n = t.Next();
			var v = t.Next();
			var sec = t.Next();
			Assert.AreEqual(ConsoleArgumentTokenizer.Token.Null, sec, "does not sygnal end of parsing "+ sec.Type+" : "+sec.Value);
			Assert.AreEqual(ConsoleArgumentTokenizer.TokenType.Name, n.Type, "invalid type returned for name" + n.Type);
			Assert.AreEqual(ConsoleArgumentTokenizer.TokenType.Value, v.Type, "invalid type returned for value" + v.Type);
			Assert.AreEqual(name, n.Value, "invalid name returned " + n.Value);
			Assert.AreEqual(value, v.Value, "invalid value returned " + v.Value);
		}
		/// <summary>
		/// Test of detection invalid argument strings
		/// </summary>
		/// <param name="invalidarg"></param>
		[TestCase("--a\\x",Description = "Escape in name")]
		[TestCase("--a\"x",Description = "Quotes in name")]
		[TestCase("--a \"x\"\"y\"",Description = "No ws between strings")]
		public void DetectsInvalidArgumentStrings(string invalidarg){
			Assert.Throws<ConsoleArgumentTokenizer.ConsoleTokenizerException>(() => {
				var t = ConsoleArgumentTokenizer.Create(invalidarg);
				while ((t.Next()).Type!=ConsoleArgumentTokenizer.TokenType.Null){
				}
			});
		}

		/// <summary>
		/// Test on long real-like string
		/// </summary>
		[Test]
		public void ComplexTestOnLongCall(){
			const string test = "myfile.txt \"other file.txt\" --copy --dir /mydir --override -- pattern";
			var testarray = new[]{
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Value, "myfile.txt"),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Value, "other file.txt"),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Name, "copy"),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Name, "dir"),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Value, "/mydir"),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Name, "override"),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Name, ""),
				new ConsoleArgumentTokenizer.Token(ConsoleArgumentTokenizer.TokenType.Value, "pattern"),
			};
			var result = ConsoleArgumentTokenizer.Create(test).Read().ToArray();
			CollectionAssert.AreEqual(testarray,result);
		}
	}

#endif


}

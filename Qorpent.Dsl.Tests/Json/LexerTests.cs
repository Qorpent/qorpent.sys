using System;
using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Dsl.Json;

namespace Qorpent.Dsl.Tests.Json
{
	[TestFixture]
	public class LexerTests
	{
		[TestCase("123",TType.Num)]
		[TestCase("123",TType.Str)]
		[TestCase("l123",TType.Lit)]
		[TestCase("true", TType.Bool)]
		public void CanReturnSingleValues(string val,TType type) {
			var token = new JsonToken(type, val);
			var item = new Lexer().Collect(new[] {token});
			Console.WriteLine(item.ToString());
			Console.WriteLine("------------------------------------------");
			Assert.NotNull(item);
			Assert.IsInstanceOf<JsonValue>(item);
			var v = item as JsonValue;
			Assert.AreEqual(val,v.Value);
			Assert.AreEqual(type,v.Type);
		}


		[TestCase( TType.Null, "a",  1, false)]
		[TestCase( TType.Bool, "a",  1, false)]
		[TestCase(TType.Num, "a",  2, false)]
		[TestCase( TType.Lit, "a",  3, true)]
		[TestCase( TType.Str, "a",  1, false)]
		[TestCase(TType.Str, "a",  2, false)]
		[TestCase( TType.Str, "a", 2, true)]
		public void CanCollectSimpleArray( TType valt, string value, int itemcount, bool trailcomma)
		{
			var tokens = new List<JsonToken> { JsonToken.OpenArray };
			for (var i = 0; i < itemcount; i++)
			{
				if (0 != i) tokens.Add(JsonToken.Comma);
				tokens.Add(new JsonToken(valt, value + i));
				}
			if (trailcomma)
			{
				tokens.Add(JsonToken.Comma);
			}
			tokens.Add(JsonToken.CloseArray);
			var item = new Lexer().Collect(tokens);
			Console.WriteLine(item.ToString());
			Console.WriteLine("------------------------------------------");
			Assert.IsInstanceOf<JsonArray>(item);
			var obj = item as JsonArray;
			Assert.AreEqual(itemcount, obj.Values.Count);
			for (var i = 0; i < itemcount; i++)
			{
				var testval = value + i;
				var val = obj.Values[i].Value;
				Assert.AreEqual(testval,val);
			}
		}


		[TestCase(TType.Str,TType.Null,"a","b",1,false)]
		[TestCase(TType.Lit,TType.Bool,"a","b",1,false)]
		[TestCase(TType.Lit,TType.Num,"a","b",2,false)]
		[TestCase(TType.Str,TType.Lit,"a","b",3,true)]
		[TestCase(TType.Lit,TType.Str,"a","b",1,false)]
		[TestCase(TType.Lit,TType.Str,"a","b",2,false)]
		[TestCase(TType.Lit,TType.Str,"a","b",2,true)]
		public void CanCollectSimpleObject(TType leftt,TType rightt, string left, string right, int itemcount, bool trailcomma) {
			var tokens = new List<JsonToken> {JsonToken.Open};
			for (var i = 0; i < itemcount; i++) {
				if (0 != i) tokens.Add(JsonToken.Comma);
				tokens.Add(new JsonToken(leftt, left+i));
				tokens.Add(JsonToken.Colon);
				tokens.Add(new JsonToken(rightt,right+i));

			}
			if (trailcomma) {
				tokens.Add(JsonToken.Comma);
			}
			tokens.Add(JsonToken.Close);
			var item = new Lexer().Collect(tokens);
			Console.WriteLine(item.ToString());
			Console.WriteLine("------------------------------------------");
			Assert.IsInstanceOf<JsonObject>(item);
			var obj = item as JsonObject;
			Assert.AreEqual(itemcount,obj.Properties.Count);
			for (var i = 0; i < itemcount; i++) {
				var name = left + i;
				Assert.True(obj.Contains(name));
				var val = right + i;
				Assert.AreEqual(val,obj[name].Value);
			}
		}

		[Test]
		public void NestedObjects() {
			var tokens = new[] {
				JsonToken.Open,
					JsonToken.String("a"),
					JsonToken.Colon,
						JsonToken.Open,
							JsonToken.Lit("b"),
							JsonToken.Colon,
								JsonToken.OpenArray,
									JsonToken.Open,
										JsonToken.Lit("c"),
										JsonToken.Colon,
										JsonToken.False,
									JsonToken.Close,
									JsonToken.Comma,
									JsonToken.Num(3),
								JsonToken.CloseArray,
							JsonToken.Comma,
							JsonToken.Lit("d"),
							JsonToken.Colon,
							JsonToken.True,
						JsonToken.Close,
					JsonToken.Close,				
			};

			var item = new Lexer().Collect(tokens);
			Console.WriteLine(item.ToString(false));
			Assert.AreEqual(@"{""a"":{b:[{c:false,},3,],d:true,},}",item.ToString(false));
		}
	}
}

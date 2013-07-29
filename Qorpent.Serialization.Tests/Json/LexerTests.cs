using System;
using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Json;

namespace Qorpent.Dsl.Tests.Json
{
	[TestFixture]
	public class LexerTests
	{
		[TestCase("123",JsonTokenType.Number)]
		[TestCase("123",JsonTokenType.String)]
		[TestCase("l123",JsonTokenType.Literal)]
		[TestCase("true", JsonTokenType.Bool)]
		public void CanReturnSingleValues(string val,JsonTokenType type) {
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


		[TestCase( JsonTokenType.Null, "a",  1, false)]
		[TestCase( JsonTokenType.Bool, "a",  1, false)]
		[TestCase(JsonTokenType.Number, "a",  2, false)]
		[TestCase( JsonTokenType.Literal, "a",  3, true)]
		[TestCase( JsonTokenType.String, "a",  1, false)]
		[TestCase(JsonTokenType.String, "a",  2, false)]
		[TestCase( JsonTokenType.String, "a", 2, true)]
		public void CanCollectSimpleArray( JsonTokenType valt, string value, int itemcount, bool trailcomma)
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


		[TestCase(JsonTokenType.String,JsonTokenType.Null,"a","b",1,false)]
		[TestCase(JsonTokenType.Literal,JsonTokenType.Bool,"a","b",1,false)]
		[TestCase(JsonTokenType.Literal,JsonTokenType.Number,"a","b",2,false)]
		[TestCase(JsonTokenType.String,JsonTokenType.Literal,"a","b",3,true)]
		[TestCase(JsonTokenType.Literal,JsonTokenType.String,"a","b",1,false)]
		[TestCase(JsonTokenType.Literal,JsonTokenType.String,"a","b",2,false)]
		[TestCase(JsonTokenType.Literal,JsonTokenType.String,"a","b",2,true)]
		public void CanCollectSimpleObject(JsonTokenType leftt,JsonTokenType rightt, string left, string right, int itemcount, bool trailcomma) {
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

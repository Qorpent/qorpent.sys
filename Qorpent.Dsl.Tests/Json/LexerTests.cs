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
			Assert.NotNull(item);
			Assert.IsInstanceOf<JsonValue>(item);
			var v = item as JsonValue;
			Assert.AreEqual(val,v.Value);
			Assert.AreEqual(type,v.Type);
		}

		[TestCase(TType.Lit,TType.Str,"a","b")]
		public void CanCollectSimpleObject(TType leftt,TType rightt, string left, string right,b)
		{
			var tokens = new[] {
				new JsonToken(TType.Open),
				new JsonToken(leftt,left),
				new JsonToken(TType.Colon),
				new JsonToken(rightt,right),
				new JsonToken(TType.Comma),
				new JsonToken(leftt,left),
				new JsonToken(TType.Colon),
				new JsonToken(rightt,right),
				new JsonToken(TType.Comma),
				new JsonToken(TType.Close)
			}
		}
	}
}

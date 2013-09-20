using System;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp {
    [TestFixture]
    public class BSharpCodeBuilderTest {
        [Test]
        public void CommentBlockTest() {
            var builder = new BSharpCodeBuilder();
            builder.WriteCommentBlock(new{test="The best",rest="Not zest!"});
            var result = builder.ToString();
            Console.WriteLine(result);
        }
    }
}
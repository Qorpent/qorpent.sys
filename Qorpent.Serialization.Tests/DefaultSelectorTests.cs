using System.Linq;
using System.Xml.Linq;
using Minerva.Core.Processing.Selector;
using Minerva.Core.Processing.Selector.Implementations;
using NUnit.Framework;
using Qorpent.IoC;
using Qorpent.Selector;
using Qorpent.Serialization;

namespace Minerva.Core.Tests {
    class DefaultSelectorTests {
	    private Container _container;

        [SetUp]
        public void FixTureSetUp() {
            _container = new Container();
            _container.Register(new ComponentDefinition<ISelectorImpl, CssSelectorImpl>(Lifestyle.Default, "zeus.robot.selector.unified"));
        }

        [Test]
        public void CanLoadUnifiedSelectorImplFromContainer() {
            var t = _container.Get<ISelectorImpl>("zeus.robot.selector.unified");
            Assert.NotNull(t);
        }




		/// <summary>
		/// Мы вполне можем и ДОЛЖНЫ делать все же тестовую среду реалистичной
		/// если это возможно
		/// </summary>
	    private const string SAMPLE_HTML = @"
<body>
	<header>
	</header>
	<div id='body' class=' main bootstrap-main '>
		<h1>Заголовок</h1>
	</div>
	<div id='nobody' class = 'no-main' >
		<h1>Другой заголовок</h1>
		<div>
			<p class='c1'>test</p>
		</div>
	</div>
	<p>
		<img id='x' />
		<i>1</i>	
		<b type='2 3'/>
		<b type='2 33'/>
		<b type='3 45'/>
		<span class='c1'>test2</span>
	</p>
	<i>2</i>
</body>
";
		[TestCase(".main h1", new[] { "<h1>Заголовок</h1>" })]
		[TestCase(".main h1 , #x, p i ", new[] { "<h1>Заголовок</h1>", "<img id=\"x\" />" ,"<i>1</i>"})]
		[TestCase("p b[type~=3]", new[] { "<b type=\"2 3\" />", "<b type=\"3 45\" />" })]
		[TestCase(".no-main .c1", new[] { "<p class=\"c1\">test</p>" })]
		public void NormalUnifiedAndDefaultTest(string query,string[] eresult) {
			var us = new CssSelectorImpl();
			var elements = us.Select(XElement.Parse(SAMPLE_HTML), query);
			var result = elements.Select(_ => _.ToString()).ToArray();
			CollectionAssert.AreEquivalent(eresult,result);

			var ds = new DefaultSelector();
			elements = ds.Select(XElement.Parse(SAMPLE_HTML), query);
			result = elements.Select(_ => _.ToString()).ToArray();
			CollectionAssert.AreEquivalent(eresult, result);
		}
    }
}

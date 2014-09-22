using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.PortableHtml;

namespace Qorpent.Serialization.Tests.PortableHtml {
	/// <summary>
	///		Нормалайзер параграфов текста
	/// </summary>
	public class PortableHtmlConverterTest {
		private PortableHtmlConverter _converter;
		[SetUp]
		public void SetUp() {
			_converter = new PortableHtmlConverter{
				KeepFormatting = false, 
				ConvertHeadersToStrongs = true ,
				ConvertLineBreaksToHtmlBreaks = true,
				Context = new PortableHtmlContext{Level = PortableHtmlStrictLevel.TrustAllLinks|PortableHtmlStrictLevel.TrustAllImages}
			};
		}
		[TestCase(@"    <div>
      <br class='1' />
      <em>
        <br class='2' />
        <span>От редакции</span>
        <br class='3' />
      </em>
      <br class='4' />
    </div>", @"<div>
  <p>От редакции</p>
</div>")]
		[TestCase(@"<div>
  <p><span phtml_tag=""td""><img src=""alexey_chabin_lj/images/c43e6abe.gif"" /></span></p>
  <p></p>
  <p><span phtml_tag=""td""><p><a href=""http://www.newsru.com/russia/09aug2014/prikaz.html"">""Они выполняли приказ"": арестованных в Ростовской области украинских военных отпустили</a></p><p><a href=""http://www.newsru.com/russia/09aug2014/prikaz.html"">Арестованные пятеро украинских военнослужащих из 72-й бригады освобождены без предъявления обвинений, сообщил в субботу официальный представитель Следственного комитета РФ Владимир Маркин.</a></p><p><a href=""http://www.newsru.com"">NEWSru.com</a></p></span></p>
  <p></p>
  <p>Когда совершается групповое преступление и ловят часть негодяев, как называется самый шустро соображающий из преступников?</p>
  <p>-Свидетель!</p>
</div>", @"<div>
  <p>
    <img src=""alexey_chabin_lj/images/c43e6abe.gif"" />
  </p>
  <p>
    <a href=""http://www.newsru.com/russia/09aug2014/prikaz.html"">""Они выполняли приказ"": арестованных в Ростовской области украинских военных отпустили</a>
  </p>
  <p>
    <a href=""http://www.newsru.com/russia/09aug2014/prikaz.html"">Арестованные пятеро украинских военнослужащих из 72-й бригады освобождены без предъявления обвинений, сообщил в субботу официальный представитель Следственного комитета РФ Владимир Маркин.</a>
  </p>
  <p>
    <a href=""http://www.newsru.com"">NEWSru.com</a>
  </p>
  <p>Когда совершается групповое преступление и ловят часть негодяев, как называется самый шустро соображающий из преступников?</p>
  <p>-Свидетель!</p>
</div>")]
		[TestCase(@"<div>Text<br /><br />Txet</div>", @"<div><p>Text</p><p>Txet</p></div>")]

		[TestCase(@"<div>a<strong></strong>a</div>", @"<div><p>aa</p></div>")]
		[TestCase(@"<div>a<br/>b<br/>c</div>",@"<div><p>a</p><p>b</p><p>c</p></div>")]

		[TestCase(@"<div>a<strong><img/></strong>a</div>", @"<div><p>aa</p></div>")]

		[TestCase(@"<div __auto='True'>a<strong __auto='True'>x</strong>a</div>", @"<div><p>a<strong>x</strong>a</p></div>")]

		[TestCase(@"<div><p>Text1</p><div><img src=""img.png"" /></div><p>Text2</p></div>", @"<div><p>Text1</p><p><img src=""img.png"" /></p><p>Text2</p></div>")]
		[TestCase(@"<div><p>Text1</p><div><img src=""#"" /></div><p>Text2</p></div>", @"<div><p>Text1</p><p><img  src='/phtml_non_trust_image.png' phtml_src='#' /></p><p>Text2</p></div>")]
		[TestCase(@"<div><p>Text1</p><div><a href=""#"" >Ярлык</a></div><p>Text2</p></div>", @"<div><p>Text1</p><p><a href='/phtml_non_trust_link.html' phtml_href='#' >Ярлык</a></p><p>Text2</p></div>")]
		[TestCase(@"<div><span>Text</span></div>", @"<div><p>Text</p></div>")]
		[TestCase(@"<div><h1>Text</h1></div>", @"<div><p phtml_tag='h1'><strong>Text</strong></p></div>")]
		[TestCase(@"<div><ol><li>Text</li><li>Text2</li></ol></div>", @"<div><p phtml_tag='li'>Text</p><p phtml_tag='li'>Text2</p></div>")]
		[TestCase(@"<div><table><tr><td>Text</td><td>Text2</td></tr></table></div>", @"<div>
  <p phtml_tag=""tr"">
    <span phtml_tag=""td"">Text</span>&#160;<span phtml_tag=""td"">Text2</span></p>
</div>")]
		[TestCase(@"<div><div><img src=""./img.png"" /></div></div>", @"<div><p><img src=""./img.png"" /></p></div>")]
		[TestCase(@"<p><img SRC=""http://example.com""></img></p>", @"<div><p><img src=""http://example.com""></img></p></div>")]
		[TestCase(@"<r><p>text</p><img src=""http://example.com/i.jpg"" /><p>two</p></r>", @"<div><p>text</p><p><img src=""http://example.com/i.jpg"" /></p><p>two</p></div>")]
		[TestCase(@"<d>&lt;p&gt;test&lt;/p&gt;&lt;p&gt;&lt;img src=""http://example.com/i.jpg"" /&gt;&lt;/p&gt;</d>", @"<div><p>test</p><p><img src=""http://example.com/i.jpg"" /></p></div>")]
		[TestCase(@"<d><img src=""http://example.com/i.jpg"" /><img src=""http://example.com/i.jpg"" /></d>", @"<div><p><img src=""http://example.com/i.jpg"" /><img src=""http://example.com/i.jpg"" /></p></div>")]
		[TestCase(@"<d>поменял юзерпик.&lt;br /&gt;взял с этой чудесной фотки.&lt;br /&gt;&lt;br /&gt;&lt;img src=""http://example.com/img.jpg"" /&gt;</d>", @"<div><p>поменял юзерпик.</p><p>взял с этой чудесной фотки.</p><p><img src=""http://example.com/img.jpg"" /></p></div>")]
		[TestCase(@"<r>Sample<br />text</r>", @"<div><p>Sample</p><p>text</p></div>")]
		[TestCase(@"<div>B   <span><a href=""http://example.com"" class=""to_listen"">S</a></span> ...</div>", @"<div><p>B <a href=""http://example.com"">S</a> ...</p></div>")]
		[TestCase(@"<div><div>Text  <div /></div></div>", @"<div><p>Text</p></div>")]
		[TestCase(@"<p>bla <a href=""http://example.com"">link</a> <!--xxx-->bla-bla</p>", @"<div><p>bla <a href=""http://example.com"">link</a> bla-bla</p></div>")]
		[TestCase(@"<r><a HreF=""http://example.com"" id=""c""><img class=""text"" src=""b"" /></a></r>", @"<div><p><a href=""http://example.com""><img src=""b"" /></a></p></div>")]
		[TestCase(@"<r>first paragraph <br />Second paragraph</r>", @"<div><p>first paragraph</p><p>Second paragraph</p></div>")]
		[TestCase(@"<r>first paragraph <br /><br />Second paragraph<br /></r>", @"<div><p>first paragraph</p><p>Second paragraph</p></div>")]
		[TestCase(@"<r>f <br /> r <a href=""http://example.com/i.jpg"">link</a></r>", @"<div><p>f</p><p>r <a href=""http://example.com/i.jpg"">link</a></p></div>")]
		[TestCase(@"<r>title<br /><div>Some<br />text</div></r>", @"<div><p>title</p><p>Some</p><p>text</p></div>")]
		[TestCase(@"<r>title<br/><div />Text</r>", @"<div><p>title</p><p>Text</p></div>")]
		[TestCase(@"<r>title<br/><div>Some text</div>Text</r>", @"<div><p>title</p><p>Some text</p><p>Text</p></div>")]
		[TestCase(@"<r><![CDATA[f <br /> r <a href=""http://example.com/i.jpg"">link</a>]]></r>", @"<div><p>f</p><p>r <a href=""http://example.com/i.jpg"">link</a></p></div>")]
		[TestCase(@"<r><![CDATA[f <br /> r <a href=""http://example.com/i.jpg"">link</a>]]></r>", @"<div><p>f</p><p>r <a href=""http://example.com/i.jpg"">link</a></p></div>")]
		[TestCase(CDataWithLfSource, @"<div><p>f</p><p>r <a href=""http://example.com/i.jpg"">link</a></p></div>",Description = "Очень сомнительная фича")]
		[TestCase(@"<r>Test text</r>", @"<div><p>Test text</p></div>")]
		[TestCase(@"<r><![CDATA[Test text]]></r>", @"<div><p>Test text</p></div>")]
		[TestCase(@"<?my proc?><r><![CDATA[Test<br />text]]></r>", @"<div><p>Test</p><p>text</p></div>")]
		[TestCase(@"<r>Test<br />text<br />text<?my proc?></r>", @"<div><p>Test</p><p>text</p><p>text</p></div>")]
		[TestCase(@"<div><a name='x'>Test</a></div>", @"<div><p>Test</p></div>")]
		[TestCase(@"<div><a>Test</a></div>", @"<div><p>Test</p></div>")]
		[TestCase(@"<div><img /></div>", @"<div />")]
		[TestCase(@"<div><img src=''/></div>", @"<div />")]
		[TestCase(@"<r><![CDATA[Test text&amp;nbsp;]]></r>", @"<div><p>Test text</p></div>")]
		[TestCase(@"<r>&lt;a href=""http://example.com""&gt;test&lt;/a&gt;</r>", @"<div><p><a href=""http://example.com"">test</a></p></div>")]
		[TestCase(@"<div><div><div><div><strong>Text</strong></div></div></div></div>", "<div><p>Text</p></div>")]
		[TestCase(@"<div><div><div><div><strong>Text</strong></div></div></div><p>Text2</p></div>", "<div><p>Text</p><p>Text2</p></div>")]
		[TestCase(@"<p><strong>xxxxx</strong></p>", "<div><p>xxxxx</p></div>")]
		[TestCase(@"<p>x<strong>y</strong>z</p>", @"<div><p>x<strong>y</strong>z</p></div>")]
		[TestCase(@"<r>B  <br /> T   <br /> G</r>", @"<div><p>B</p><p>T</p><p>G</p></div>")]
		[TestCase(@"<p>Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу после минуты молчания. В церквях Нью-Йорка пройдут поминальные службы, а в честь погибших зазвонят колокола.<br />После заката в небо над городом будут направлены 2 голубых луча прожекторов, которые символизируют башни Всемирного торгового центра. Инсталляцию «Посвящение в свете» включают ежегодно с 2002 года.<br />Мемориальные мероприятия пройдут во всех районах Нью-Йорка. Жители зажгут свечи в память о жертвах террористов, исполнят музыкальные и поэтические композиции. Также дань уважения отдадут спасателям, которые помогали пострадавшим и разбирали завалы.<br />Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и Пенсильвании погибли 2977 человек из 90 стран мира и пострадали 6 тысяч человек. Европейско-Азиатские Новости.</p>", @"<div>
  <p>Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу после минуты молчания. В церквях Нью-Йорка пройдут поминальные службы, а в честь погибших зазвонят колокола.</p>
  <p>После заката в небо над городом будут направлены 2 голубых луча прожекторов, которые символизируют башни Всемирного торгового центра. Инсталляцию «Посвящение в свете» включают ежегодно с 2002 года.</p>
  <p>Мемориальные мероприятия пройдут во всех районах Нью-Йорка. Жители зажгут свечи в память о жертвах террористов, исполнят музыкальные и поэтические композиции. Также дань уважения отдадут спасателям, которые помогали пострадавшим и разбирали завалы.</p>
  <p>Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и Пенсильвании погибли 2977 человек из 90 стран мира и пострадали 6 тысяч человек. Европейско-Азиатские Новости.</p>
</div>")]
		[TestCase(@"<div>
            <div>
              <div><div><div><strong>Полицейские будут наказывать пешеходов и автомобилистов за нарушение правил дорожного движения.</strong></div></div></div>
            </div>
            <p>Сегодня автоинспекторы Екатеринбурга перешли на особый режим несения службы. Сотрудники ДПС в субботу и воскресенье будут дежурить на самых опасных участках дорог и пешеходных переходах города, сообщает управление ГИБДД ГУ МВД России по Свердловской области.</p>
</div>", @"<div>
  <p>Полицейские будут наказывать пешеходов и автомобилистов за нарушение правил дорожного движения.</p>
  <p>Сегодня автоинспекторы Екатеринбурга перешли на особый режим несения службы. Сотрудники ДПС в субботу и воскресенье будут дежурить на самых опасных участках дорог и пешеходных переходах города, сообщает управление ГИБДД ГУ МВД России по Свердловской области.</p>
</div>")]
		[TestCase(@"<r><![CDATA[Test
text]]></r>", @"<div><p>Test</p><p>text</p></div>")]
		[TestCase(@"<p>
              <br />
              <a target='_blank' href='http://66.ru/news/incident/163250/photoreportage/' class='pic-with-repo'>
                <br />
                <img class=' qlevel-16 qpos-0' src='66_ru/images/e2bb16c2.jpg' />
                <br />
                <span class='pic-with-repo__text'>
                  <br />
                  <span class='pic-with-repo__text-pad'>
                    <br />
                    <span>Lada Kalina перевернулась на ул. Альпинистов</span>
                    <br />
                    <span class='pic-with-repo__count'>8 фотографий</span>
                    <br />
                  </span>
                  <br />
                </span>
                <br />
              </a>
              <br />
            </p>", @"<div><p><a href='http://66.ru/news/incident/163250/photoreportage/'><img src='66_ru/images/e2bb16c2.jpg' /> Lada Kalina перевернулась на ул. Альпинистов 8 фотографий</a></p></div>")]
		public void Test(string source, string expected) {
			var ex = XElement.Parse(expected);
			Assert.True(PortableHtmlSchema.Validate(ex,PortableHtmlStrictLevel.TrustAllImages|PortableHtmlStrictLevel.TrustAllLinks).Ok);

			var phtml = _converter.Convert(XElement.Parse(source));
			var r = phtml.ToString();
			
			Console.WriteLine("EXPECTED:\n====================");
			Console.WriteLine(ex.ToString());
			Console.WriteLine("\nRESULT:\n====================");
			Console.WriteLine(r);
			Assert.AreEqual(ex.ToString(SaveOptions.DisableFormatting).Replace("\u00A0", " "),phtml.ToString(SaveOptions.DisableFormatting).Replace("\u00A0", " "));
			var ctx = PortableHtmlSchema.Validate(phtml, PortableHtmlStrictLevel.TrustAllImages | PortableHtmlStrictLevel.TrustAllLinks);
			if (!ctx.Ok){
				Console.WriteLine(ctx.ToString());
				Assert.Fail("No PHTML");
			}
		}
		public const string CDataWithLfSource = @"<r><![CDATA[f
r <a href=""http://example.com/i.jpg"">link</a>]]></r>";

	}
}

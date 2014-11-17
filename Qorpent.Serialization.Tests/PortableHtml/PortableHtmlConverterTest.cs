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
		[TestCase(@"<div>
  <p>
    <br />
    <span class='c2'>Добрый день!</span>
    <br />
    <br class='c3 qlevel-8 qpos-1' />
    <br />
    <span class='c2'>Прошу обратить внимание на адрес писем для Леонида Васильевича:</span>
    <br />
    <br class='c3 qlevel-8 qpos-3' />
    <br />
    <span class='c5'>
      <br />
      <strong class='c4'>655017, г. Абакан, п. Молодежный-11, ФКУ ИК-35<br class=' qlevel-10 qpos-0' />  Все тоже самое, только без отряда.</strong>
      <br />
    </span>
    <br />
    <br class='c3 qlevel-8 qpos-5' />
    <br />
    <span class='c2'>Что касаемо новостей, особых изменений нет - 'все нормально' - сидит.</span>
    <br />
  </p>
</div>", @"<div>
  <p>Добрый день!</p>
  <p>Прошу обратить внимание на адрес писем для Леонида Васильевича:</p>
  <p>655017, г. Абакан, п. Молодежный-11, ФКУ ИК-35</p>
  <p>Все тоже самое, только без отряда.</p>
  <p>Что касаемо новостей, особых изменений нет - 'все нормально' - сидит.</p>
</div>")]

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


		private const string Text_To_Digest =
			@"<div>
  <p>Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу после минуты молчания. В церквях Нью-Йорка пройдут поминальные службы, а в честь погибших зазвонят колокола.</p>
  <p>После заката в небо над городом будут направлены 2 голубых луча прожекторов, которые символизируют башни Всемирного торгового центра.</p> 
  <p>Инсталляцию «Посвящение в свете» включают ежегодно с 2002 года.</p>
  <p>Мемориальные мероприятия пройдут во всех районах Нью-Йорка. Жители зажгут свечи в память о жертвах террористов, исполнят музыкальные и поэтические композиции.</p> 
  <p>Также дань уважения отдадут спасателям, которые помогали пострадавшим и разбирали завалы.</p>
  <p>Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и Пенсильвании погибли 2977 человек из 90 стран мира и пострадали 6 тысяч человек. Европейско-Азиатские Новости.</p>
</div>";

		[TestCase(50, "Сегодня Америка почтит память 2977 погибших в...")]
		[TestCase(100, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает...")]
		[TestCase(200, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре... Напомним, 11 сентября 2011 года в...")]
		[TestCase(300, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте... Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в...")]
		[TestCase(400, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу... Мемориальные мероприятия пройдут во всех районах... Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и...")]
		[TestCase(500, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу после минуты молчания. В церквях... Мемориальные мероприятия пройдут во всех районах Нью-Йорка. Жители зажгут свечи в... Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и Пенсильвании погибли 2977 человек...")]
		[TestCase(600, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу после минуты молчания. В церквях Нью-Йорка пройдут поминальные службы, а в... Инсталляцию «Посвящение в свете»... Также дань уважения отдадут спасателям, которые помогали... Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и Пенсильвании погибли 2977 человек из 90 стран мира и пострадали 6 тысяч...")]
		[TestCase(700, "Сегодня Америка почтит память 2977 погибших в результате терактов 11 сентября 2011 года, передает корреспондент агентства ЕАН. Имена жертв зачитают в центре мемориального комплекса, расположенного на месте башен-близнецов, сразу после минуты молчания. В церквях Нью-Йорка пройдут поминальные службы, а в честь погибших зазвонят колокола... Инсталляцию «Посвящение в свете» включают ежегодно с 2002 года... Также дань уважения отдадут спасателям, которые помогали пострадавшим и разбирали завалы... Напомним, 11 сентября 2011 года в результате терактов, организованных «Аль-Каидой» в Нью-Йорке, Вашингтоне и Пенсильвании погибли 2977 человек из 90 стран мира и пострадали 6 тысяч человек. Европейско-Азиатские Новости.")]
		public void DigestTest(int size, string result){
			var xml = XElement.Parse(Text_To_Digest);
			
			var digest = new PortableHtmlConverter().GetDigest(xml, size);
			Console.WriteLine(digest);
			Assert.AreEqual(result, digest);
			Assert.Greater(size*1.1,digest.Length);
		}
		[Test]
		public void BugDigest(){
			Assert.AreEqual("(Документ не содержит текста)", new PortableHtmlConverter().GetDigest(XElement.Parse("<div/> ")));
		}

		[Test]
		public void BugDigestNoText()
		{
			Assert.AreEqual("(Документ не содержит текста)", new PortableHtmlConverter().GetDigest(XElement.Parse("<div><p><img src='xxx'/></p></div> ")));
		}

		[Test]
		public void PremiallyCropImages(){
			var xml =
				XElement.Parse(
					@"<div><p><img src=""_img/uraru/23f0ba61.jpg"" /></p><p>Сегодня в матче десятого тура российской премьер-лиги Екатеринбургский «Урал» одержал свою первую домашнюю победу в новом сезоне. Повержен московский «Спартак». Счет матча — 2:0!</p><p>Сегодняшний матч между «Уралом» и «Спартаком» мог и не состояться вовсе. В ночь с пятницы на субботу в Свердловской области начался снегопад, который продолжался до утра сегодняшнего дня. Поэтому проведение поединка был под угрозой — поле Центрального стадиона было полностью засыпано снегом. Еще утром известный комментатор Константин Генич написал в своем твиттере, что на 99 процентов игра не состоится. Но службы ЦС не подвели — почти 300 человек чистили поле, и как результат почти зеленое поле. Подфортило, если так можно сказать с погодой, — снег идти перестал как раз за несколько минут до игры, хотя еще за полчаса валил большими хлопьями.</p><p><a href=""http://ura.ru/images/news/upload/news/192/606/1052192606/83488_FK_Ural_FK_Spartak_Ekaterinburg_1413718112_original.jpg""><img src=""_img/uraru/85e8a455.jpg"" /> </a></p><p>В 15.30, как и было запланировано команды вышли на поле. Несмотря на «минусовую температру» на ЦС пришло около десяти тысяч человек. Гостевой фан-сектор столичного «Спартака» был пуст. Решение таким образом наказать московский «Спартак» принял контрольно-дисциплинарный комитет Российского футбольного союза. Столичный клуб был наказан за оскорбительные выкрики болельщиков красно-белых в адрес бразильского нападающего «Зенита» Халка во время матча, который состоялся в Санкт-Петербурге 27 сентября. Однако фанатам «Спартака» разрешили разместить на гостевом секторе баннер в память об ушедшем из жизни 4 октября в возрасте 55 лет легендарном футболисте Федоре Черенкове: «вечная память Федор Федорович».</p><p>Расскажите о новости своим друзьям</p><p><a href=""http://ura.ru/content/svrd/19-10-2014/news/1052192606.html"">Твитнуть</a></p><p><a href=""http://ura.ru/content/svrd/19-10-2014/news/1052192606.html#add_blog""><strong>Код для вставки в блог</strong><img src=""_img/uraru/3961b7f8.gif"" /></a></p><p phtml_tag=""h1""><a href=""http://ura.ru/content/svrd/19-10-2014/news/1052192606.html"">Зимний футбол оранжевым мячом. Первая домашняя победа «Урала»! Повержен московский «Спартак». ФОТО</a></p><p>Сегодня в матче десятого тура российской премьер-лиги Екатеринбургский «Урал» одержал свою первую домашнюю победу в новом сезоне. Повержен московский «Спартак». Счет матча — 2:0!</p><p><a href=""http://ura.ru/content/svrd/19-10-2014/news/1052192606.html"">Читать целиком</a></p><p>Вставьте этот код себе в блог — получится красивая ссылка на статью «URA.Ru» с картинкой.</p><p>...и продолжайте читать дальше!</p><p><a href=""http://ura.ru/images/news/upload/news/192/606/1052192606/83489_FK_Ural_FK_Spartak_Ekaterinburg_1413718138_original.jpg""><img src=""_img/uraru/0a63c3df.jpg"" /> </a></p><p>Но Самих болельщиков красно-белых можно было увидеть на трибунах. При этом они были заняли места рядом с фан-сектором и оказывали весь матч внушительную поддержку. Им разрешили прийти в атрибутике, но без «средств активной поддержки», таких как флаги, барабан и др. Как говорили сами москвичи, ни в одном из городов так расположиться им бы не разрешили и благодарили руководство «Урала».</p><p><a href=""http://ura.ru/images/news/upload/news/192/606/1052192606/83486_FK_Ural_FK_Spartak_Ekaterinburg_1413718248_original.jpg""><img src=""_img/uraru/be0b2ce0.jpg"" /> </a></p><p>Первый опасный момент в матче, который транслировался на всю страну на федеральном НТВ, возник на 8-ой минуте, когда опасно бил Федор Смолов, правда из положения «вне игры». Примечательно, что футболисты играли оранжевым мячом, а разметка была красного цвета, которую, правда практически видно не было. На 12-ой вновь опасный момент у ворот «Спартака» — мяч после Смолова удара летит в штангу. Затем стали давить красно-белые, но ударов в створ было немного, но опасность у ворот екатеринбуржцев стала возникать. Хотя атаки «Урала» смотрелись гораздо опаснее, но голов так и не было.</p><p><a href=""http://ura.ru/images/news/upload/news/192/606/1052192606/83490_FK_Ural_FK_Spartak_Ekaterinburg_1413718318_original.jpg""><img src=""_img/uraru/a8f85b34.jpg"" /> </a></p><p>Но на 36-ой минуте произошло то, к чему все и шло логически — «Урал» забил! Ставпец заработал очень опасный штрафной, навес Манучаряна и отличный удар Ерохина в падении головой — 1:0! На кураже екатеринбуржцы пошли в атаку и имели сразу несколько очень опасных моментов, но забить не смогли.</p><p>Во втором тайме поле выглядело абсолютно зеленым, трактор дочистил оставшиеся снежные полосы. «Урал» продолжал давить и создавать опасные моменты. Счет должен был стать 2:0 на 52-ой минуте, когда Ставпец вышел один на один, благодаря шикарному пасу Смолова, и обязан был забивать, но попал в штангу. Но свой гол «Урал» все-таки забил и сделал счет 2:0! Это Фонтанелло отлично пробил головой после углового, после чего получил травму — защитник красно-белых попал ему прямо в голову. Несколько минут Фонтанелло провел за пределами поля, но врачи подлатали его и он с перебинтованной головой вернулся в игру. В итоге матч завершился со счетом 2:0 в пользу «Урала» — есть три очка!</p><p><a href=""http://ura.ru/images/news/upload/news/192/606/1052192606/83491_FK_Ural_FK_Spartak_Ekaterinburg_1413718407_original.jpg""><img src=""_img/uraru/0d37f49b.jpg"" /> </a></p><p>Это первая домашняя победа екатеринбуржцев в этом сезоне. Сейчас в активе клуба — 7 очков. Свой следующий матч «Урал» снова проведет дома — 24 октября против тульского «Арсенала».</p></div>");
			var digest = new PortableHtmlConverter().GetDigest(xml, 100);
			Console.WriteLine(digest);
			Assert.AreEqual(@"Сегодня в матче десятого тура российской премьер-лиги Екатеринбургский «Урал» одержал свою первую...", digest);

		}

	    [Test]
	    public void Q295_InvalidInlinesInRow() {
	        var xml = XElement.Parse("<div>Бла бла <strong>Сергей</strong>\r\n\r\n\r\n<strong>Иванов</strong></div>");
            var ctx = PortableHtmlSchema.Validate(xml, PortableHtmlStrictLevel.TrustAllImages | PortableHtmlStrictLevel.TrustAllLinks);
            Assert.False(ctx.Ok);
	        var converted = new PortableHtmlConverter().Convert(xml);
            var x = XElement.Parse("<div><p>Бла бла <strong>Сергей Иванов</strong></p></div>");
            Console.WriteLine(x.ToString(SaveOptions.DisableFormatting));
            Assert.AreEqual(x.ToString(SaveOptions.DisableFormatting).Replace("\u00A0", " "), converted.ToString(SaveOptions.DisableFormatting).Replace("\u00A0", " "));
           
	    }

    

	}
}

using System;
using NUnit.Framework;
using Qorpent.Log;

namespace Qorpent.Utils.Tests
{
	[TestFixture]
	public class WebDateTimeParserTest {
		[TestFixture]
		public class HumanReadableCases {
			[TestCase("34 минуты назад", 34)]
			[TestCase("56 минут назад", 56)]
			[TestCase("1 минуту назад", 1)]
			[TestCase("минуту назад", 1)]
			[TestCase("десять минут назад", 10)]
			[TestCase("двадцать минут назад", 20)]
			[TestCase("пятнадцать минут назад", 15)]
			public void IsCorrectParsingNMinutesAgo(string str, int o) {
				var n = new DateTime(2015, 1, 1, 12, o, 0);
				var p = WebDateTimeParser.Parse(str, baseDate: n);
				Assert.AreEqual(2015, p.Year);
				Assert.AreEqual(1, p.Month);
				Assert.AreEqual(1, p.Day);
				Assert.AreEqual(12, p.Hour);
				Assert.AreEqual(0, p.Minute);
				Assert.AreEqual(0, p.Second);
			}
			[TestCase("час назад", 1)]
			[TestCase("10 часов назад", 10)]
			[TestCase("десять часов назад", 10)]
			public void IsCorrectParsingNHoursAgo(string str, int o) {
				var n = new DateTime(2015, 1, 1, o, 0, 0);
				var p = WebDateTimeParser.Parse(str, baseDate: n);
				Assert.AreEqual(2015, p.Year);
				Assert.AreEqual(1, p.Month);
				Assert.AreEqual(1, p.Day);
				Assert.AreEqual(0, p.Hour);
				Assert.AreEqual(0, p.Minute);
				Assert.AreEqual(0, p.Second);
			}
			[TestCase("день назад", 1)]
			[TestCase("1 день назад", 1)]
			[TestCase("один день назад", 1)]
			[TestCase("2 дня назад", 1)]
			[TestCase("два дня назад", 1)]
			[TestCase("7 дней назад", 7)]
			[TestCase("неделю назад", 1)]
			[TestCase("позавчера", 2)]
			public void IscorrectPArsingNDaysAgo(string str, int o) {
				var n = new DateTime(2015, 1, o + 1, 0, 0, 0);
				var p = WebDateTimeParser.Parse(str, baseDate: n);
				Assert.AreEqual(2015, p.Year);
				Assert.AreEqual(1, p.Month);
				Assert.AreEqual(1, p.Day);
				Assert.AreEqual(0, p.Hour);
				Assert.AreEqual(0, p.Minute);
				Assert.AreEqual(0, p.Second);
			}
		}
		[TestCase("22.12.2013 22:40:52", 2013, 12, 22, 22, 40, 52, '\0', '\0', "", false, false)]
		[TestCase("22:40:52 22.12.2013", 2013, 12, 22, 22, 40, 52, '\0', '\0', "", false, false)]
		[TestCase("22:40:52 22.12.13", 2013, 12, 22, 22, 40, 52, '\0', '\0', "", false, false)]
		[TestCase("22:40 22.12.2013", 2013, 12, 22, 22, 40, 0, '\0', '\0', "", false, false)]
		[TestCase("22:40 22.12", -1, 12, 22, 22, 40, 0, '\0', '\0', "", false, false)]
		[TestCase("22:40, 22 apRiL 2013", 2013, 4, 22, 22, 40, 0, '\0', '\0', "en-US", false, false)]
		[TestCase("25.12.2013 18:23", 2013, 12, 25, 18, 23, 0, '\0', '\0', "", false, false)]
		[TestCase("25 Декабрь 2013, 18:00", 2013, 12, 25, 18, 00, 0, '\0', '\0', "ru-RU", false, false)]
		[TestCase("15:17 25 декабря 2013", 2013, 12, 25, 15, 17, 0, '\0', '\0', "ru-RU", false, false)]
		[TestCase("\n 15:17 25 декабря 2013  ", 2013, 12, 25, 15, 17, 0, '\0', '\0', "ru-RU", false, false)]
		[TestCase("\n вчера в 15:17  ", -1, 12, 0, 15, 17, 0, ' ', '\0', "ru-RU", true, false)]
		[TestCase("Сегодня 11:29", -1, 0, 0, 11, 29, 0, '\0', '\0', "ru-RU", true, true)]
		[TestCase("сегодня в 13:12", -1, 0, 0, 13, 12, 0, '\0', '\0', "ru-RU", true, true)]
		[TestCase("26/09/2013", 2013, 9, 26, 0, 0, 0, '/', '\0', "", false, false)]
		[TestCase("25 сентября 2013, 17:21", 2013, 9, 25, 17, 21, 0, '\0', '\0', "ru-RU", false, false)]
		[TestCase("Sun, 09 Feb 2014 17:18:42 +0600", 2014, 2, 9, 17, 18, 42, '\0', '\0', "", false, false)]
		[TestCase("2014-02-09T14:45:18+06:00", 2014, 2, 9, 14, 45, 18, '\0', '\0', "", false, false)]
		public void CanParse(string input, int y, int m, int d, int h, int min, int s, char dd, char td, string cult, bool dyn = false, bool istoday = false)
		{
			
			var parsed = WebDateTimeParser.Parse(input);

			if (dyn)
			{
				var f = istoday ? DateTime.Now : DateTime.Now.AddDays(-1);
				Assert.AreEqual(f.Year, parsed.Year);
				Assert.AreEqual(f.Month, parsed.Month);
				Assert.AreEqual(f.Day, parsed.Day);
			}
			else
			{
				Assert.AreEqual(y == -1 ? DateTime.Now.Year : y, parsed.Year);
				Assert.AreEqual(m, parsed.Month);
				Assert.AreEqual(d, parsed.Day);
			}

			Assert.AreEqual(h, parsed.Hour);
			Assert.AreEqual(min, parsed.Minute);
			Assert.AreEqual(s, parsed.Second);
		}

		[TestCase(-4)]
		[TestCase(4)]
		public void ExplicitTimeZoneSupport(int timeZone){
			var src = "23.12.2014 23:25";
			var tres = WebDateTimeParser.Parse(src, timeZone: timeZone);
			var lres = WebDateTimeParser.Parse(src);
			var currentZone = (DateTime.Today - DateTime.Today.ToUniversalTime()).TotalHours;
			var diff = timeZone - currentZone;
			Console.WriteLine(lres);
			Console.WriteLine(tres);
			Assert.AreEqual(diff, (tres - lres).TotalHours);

		}

		[Test]
		public void ZU185_Bug_Invalid_DateTimeParsed_Ctor(){
			var safeReturn = new DateTime(1900, 1, 1);

			Assert.AreEqual(safeReturn,safeReturn);
		}

		[Test]
		public void MI_344_Invalid_DateParsing(){
			var date = WebDateTimeParser.Parse("Автор: Роман Слюнов, 2014-06-18 01:28:01");
			Assert.AreEqual(new DateTime(2014,6,18,1,28,0),date);
		}

        [Test]
        public void MI_481_Mk_Date()
        {
            var date = WebDateTimeParser.Parse("Два дня назад в 18:37");
            var day2b = DateTime.Today.AddDays(-2);
            Assert.AreEqual(new DateTime(day2b.Year, day2b.Month, day2b.Day, 18, 37, 0), date);

            date = WebDateTimeParser.Parse("Три дня назад в 18:37");
            var day3b = DateTime.Today.AddDays(-3);
            Assert.AreEqual(new DateTime(day3b.Year, day3b.Month, day3b.Day, 18, 37, 0), date);
        }

		[Test]
		public void ZU_261_Nakanune_MSK()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("14.09.2014 11:01 Мск (13:01 Екб) политика | Челябинская область | Уральский ФО");
			Assert.AreEqual(new DateTime(2014, 9, 14, 13, 01, 0), date);
		}
		[Test]
		public void ZU_261_UralPolit_GMT(){
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("Sat, 13 Sep 2014 09:33:35 GMT");
			Assert.AreEqual(new DateTime(2014,9,13,15,33,35),date);
		}
		[Test]
		public void MI_364_Invalid_DateParsing_Echo(){
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("14:42, 05 сентября 2014, 36 просмотров");
			Assert.AreEqual(new DateTime(2014,9,5,14,42,0),date);
		}
		[Test]
		public void MI_344_Invalid_DateParsing_MStrok()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("27 Авг 2014 11:32 Экономисты предупреждают, что сильнее всего санкции ударят по простым гражданам");
			Assert.AreEqual(new DateTime(2014, 8,27, 11, 32, 0), date);
		}
		[Test]
		public void MI_395_Invalid_DateParsing_DoubledE1()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("08 Сентябрь 2014, 18:56 - 07.09.2014 - 26.11.2013 - 01.11.2013");
			Assert.AreEqual(new DateTime(2014, 9,8, 18,56, 0), date);
		}
		[Test]
		public void MI_344_Invalid_DateParsing_Veved()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("17:40 | 21.08.2014");
			Assert.AreEqual(new DateTime(2014, 8, 21, 17, 40, 0), date);
		}

		[Test]
		public void MI_395_Invalid_Date_WithToday()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("Сегодня в 18:30");
			var today = DateTime.Today;
			Assert.AreEqual(new DateTime(today.Year,today.Month,today.Day, 18, 30, 0), date);
		}
		[Test]
		public void MI_344_Invalid_DateParsing_PravdaUrfo()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("10.09.2014-19:23");
			Assert.AreEqual(new DateTime(2014, 9, 10, 19, 23, 0), date);
		}	
		[Test]
		public void MI_395_Invalid_DateParsing_Veved2()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("10:22 | 8.09.2014");
			Assert.AreEqual(new DateTime(2014, 9,8, 10, 22, 0), date);
		}	
		
		
		[Test]
		public void MI_353_Invalid_DateParsing_JustMedia()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("Вчера в 22:01");
			var yesterday = DateTime.Today.AddDays(-1);
			Assert.AreEqual(new DateTime(yesterday.Year,yesterday.Month,yesterday.Day, 22, 01, 0), date);
		}


		[Test]
		public void MI_353_Invalid_DateParsing_JustMedia_NoYear()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse("17 сентября в 22:01");
			Assert.AreEqual(new DateTime(DateTime.Today.Year,9,17, 22, 01, 0), date);
		}

		[Test]
		public void MI_344_Invalid_DateParsing_Upmonitor()
		{
			var parser = new WebDateTimeParser();
			var date = WebDateTimeParser.Parse(" 27.08.2014, 10:40 Источник: http://www.regnum.ru/news/1840829.html");
			Assert.AreEqual(new DateTime(2014, 8, 27, 10, 40, 0), date);
		}
	}
}

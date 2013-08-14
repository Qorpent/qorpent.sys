using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class XmlInterpolationTests {
		private XmlInterpolation _xi;

		[SetUp]
		public void SetUp()
		{
			_xi = new XmlInterpolation();
		}

		/// <summary>
		/// Тест в котором один атрибут просто прошивается в другой в одном элементе
		/// </summary>
		[Test]
		public void SimleInterpolation() {
			var x = XElement.Parse("<a x='1' y='${x}${x}2'/>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1",x.Attribute("x").Value);
			Assert.AreEqual("112", x.Attribute("y").Value);
		}

		/// <summary>
		/// Тест в котором один атрибут просто прошивается в другой в одном элементе,
		/// но при этом производится 2 преобразования
		/// </summary>
		[Test]
		public void IterativeSimleInterpolation()
		{
			var x = XElement.Parse("<a x='1' y='${x}${x}2' z='${y}${x}3'/>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1", x.Attribute("x").Value);
			Assert.AreEqual("112", x.Attribute("y").Value);
			Assert.AreEqual("11213", x.Attribute("z").Value);
		}

		/// <summary>
		/// Простой проброс интрполяции вниз
		/// </summary>
		[Test]
		public void SimleInterpolation2Level()
		{
			var x = XElement.Parse("<a x='1'><b y='${x}'/></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("1", x.Element("b").Attribute("y").Value);
			
		}

		/// <summary>
		/// Простой проброс интрполяции вниз с перекрытием родительского атрибута
		/// </summary>
		[Test]
		public void SimleInterpolation2LevelOverride()
		{
			var x = XElement.Parse("<a x='1' y='2'><b y='${x}${x}${x}' z='${y}z${y}'/></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("111z111", x.Element("b").Attribute("z").Value);

		}
		/// <summary>
		/// Простой проброс интрполяции вниз с явной ссылкой на родительский атрибут
		/// </summary>
		[Test]
		public void SimleInterpolation2LevelExplicitParent()
		{
			var x = XElement.Parse("<a x='1' y='2'><b y='${x}${x}${x}' z='${.y}z${.y}'/></a>");
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("2z2", x.Element("b").Attribute("z").Value);

		}

		/// <summary>
		/// Простой проброс интрполяции вниз с явной ссылкой на родительский атрибут
		/// </summary>
		[Test]
		public void BugNotResolveWithLevel()
		{
			var x = new BxlParser().Parse(
@"
test11 x=A
	test12 
		test14 x=B
			test15
				test16 x='${..x}${.x}C' y='${x}+'"
				);
			x = _xi.Interpolate(x);
			Console.WriteLine(x);
			Assert.AreEqual("ABC", x.Descendants("test16").First().Attribute("x").Value);

		}
	}
}
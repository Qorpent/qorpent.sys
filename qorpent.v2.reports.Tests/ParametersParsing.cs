using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using qorpent.v2.model;
using qorpent.v2.reports.model;
using Qorpent.Bxl;
using Qorpent.Experiments;

namespace qorpent.v2.reports.Tests
{
    [TestFixture]
    public class ParametersParsingTest {
        [Test]
        public void BxlTest() {
            var code = @"
report tophot 'ТОП ""горячих"" объектов'
	param metrica ""Метрика"" type=list
		item percentile ""Статистическая значимость""
		item top ""Фиксированный топ""
	param percentile ""Уровень, %"" default=95 list=""50|60|70|80|90|95|99"" idx=10
	param top ""Макс кол-во"" list=""5|10|20|50|100|0:Без ограничений"" default=20";
            var xml = new BxlParser().Parse(code, options: BxlParserOptions.ExtractSingle);
            ExecuteTest(xml);
        }

        [Test]
        public void JsonTest() {
            var json = @"
{
    '_id':'tophot',
    'name':'ТОП \""горячих\"" объектов',
    'parameters' : {
        'metrica' : {
            'name':'Метрика',
            'list' : {
                'percentile' : 'Статистическая значимость',
                'top' : 'Фиксированный топ'
            }
        },
        'percentile': {
            'idx':10,
            'name' :'Уровень, %',
            'default':'95',
            'list':'50|60|70|80|90|95|99'
        },
        'top': {
            'name' :'Макс кол-во',
            'default':'20',
            'list':'5|10|20|50|100|0:Без ограничений'
        },
    }
}
".Replace("\'","\"").jsonify();
            ExecuteTest(json);
        }

        private static void ExecuteTest(object src) {
            var report = Item.Create<Report>(src);
            var p = report.Parameters;
            Assert.AreEqual(3, p.Count);
            Assert.True(p.ContainsKey("metrica"));
            Assert.True(p.ContainsKey("percentile"));
            Assert.True(p.ContainsKey("top"));
            var m = p["metrica"];
            var r = p["percentile"];
            var t = p["top"];
            Assert.AreEqual("percentile", m.Default);
            Assert.AreEqual("95", r.Default);
            Assert.AreEqual("20", t.Default);
            Assert.AreEqual("metrica", m.Id);
            Assert.AreEqual("Метрика", m.Name);
            Assert.True(m.Idx > r.Idx);
            Assert.True(t.Idx > m.Idx);
            Assert.AreEqual(2, m.List.Count);
            Assert.AreEqual(6, t.List.Count);
            var m1 = m.List[1];
            Assert.AreEqual("top", m1.Id);
            Assert.AreEqual("Фиксированный топ", m1.Name);
            Assert.AreEqual("10", t.List[1].Id);
            Assert.AreEqual("10", t.List[1].Name);
            Assert.AreEqual("0", t.List[5].Id);
            Assert.AreEqual("Без ограничений", t.List[5].Name);
        }
    }
}

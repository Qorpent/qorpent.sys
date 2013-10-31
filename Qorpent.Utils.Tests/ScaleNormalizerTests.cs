using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ScaleNormalizerTests {
        private ScaleNormalizer _scaleNormalizer;
        /// <summary>
        /// 
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            _scaleNormalizer = new ScaleNormalizer();
        }
        /// <summary>
        ///     Тест проверяет правильность работы нормализатора согласно #AP-347
        /// </summary>
        /// <param name="dataRow">Ряд данных через запятую</param>
        /// <param name="expectedMin">Ожидаемое минимальное значение чарта</param>
        /// <param name="expectedMax">Ожидаемое максимальное значение чарта</param>
        /// <param name="divline">Количество дивлайнов</param>
        [TestCase("2139,2066,1870,1854,1882,1823,1870,2033,2129,1936,1853,1829,1841,2033", 1800.0, 2200.0, 3.0)]
        public void CanUseDefaultNormalizer(string dataRow, double expectedMin, double expectedMax, double divline) {
            var data = dataRow.SmartSplit(false, true, new[] {','}).Select(Convert.ToDouble);
            var normalized = _scaleNormalizer.Normalize(data);
            Console.WriteLine("Expected: from {0} to {1} with {2} divlines", expectedMin, expectedMax, divline);
            Console.WriteLine("Gotten: from {0} to {1} with {2} divlines", normalized.Minimal, normalized.Maximal, normalized.Divline);
            Assert.AreEqual(expectedMin, normalized.Minimal);
            Assert.AreEqual(expectedMax, normalized.Maximal);
            Assert.AreEqual(divline, normalized.Divline);
        }
    }
}

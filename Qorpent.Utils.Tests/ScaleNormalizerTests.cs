using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Scaling;

namespace Qorpent.Utils.Tests {
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ScaleNormalizerTests {
        /// <summary>
        ///     Тест проверяет правильность работы нормализатора согласно #AP-347
        /// </summary>
        /// <param name="dataRow">Ряд данных через запятую</param>
        /// <param name="expectedMin">Ожидаемое минимальное значение чарта</param>
        /// <param name="expectedMax">Ожидаемое максимальное значение чарта</param>
        /// <param name="divline">Количество дивлайнов</param>
        [TestCase("2139,2066,1870,1854,1882,1823,1870,2033,2129,1936,1853,1829,1841,2033", 1700.0, 2300.0, 3.0)]
        [TestCase("216790,238688,103771,192571,105145,38828", 0, 250000, 5)]
        [TestCase("200,500,250,233,286", 0, 600, 5)]
        [TestCase("100,200,250,150", 0, 300, 2)]
        [TestCase("100,200,350,150", 0, 400, 2)]
        [TestCase("100,200,450,150", 0, 500, 4)]
        [TestCase("100,200,550,150", 0, 600, 5)]
        [TestCase("100,200,650,150", 0, 700, 5)]
        [TestCase("100,200,750,150", 0, 800, 3)]
        [TestCase("100,200,850,150", 0, 900, 2)]
        public void CanUseDefaultNormalizer(string dataRow, double expectedMin, double expectedMax, double divline) {
            var data = dataRow.SmartSplit(false, true, new[] {','}).Select(Convert.ToDouble);
            var normalized = ScaleNormalizer.Normalize(data);
            Console.WriteLine("Expected: from {0} to {1} with {2} divlines", expectedMin, expectedMax, divline);
            Console.WriteLine("Gotten: from {0} to {1} with {2} divlines", normalized.Minimal, normalized.Maximal, normalized.Divline);
            Assert.AreEqual(expectedMin, normalized.Minimal);
            Assert.AreEqual(expectedMax, normalized.Maximal);
            Assert.AreEqual(divline, normalized.Divline);
        }
    }
    public class SlickSortTests {
        [TestCase("1", "1")]
        [TestCase("2,3,5,4", "4,2,5,3")]
        [TestCase("100,200,250", "200,100,250")]
        [TestCase("100,200,4,250", "200,100,250,4")]
        [TestCase("100,240,230,200,137,250", "200,100,250,240,230,137")]
        public void Can(string notSlick, string slick) {
            var notSlickList = notSlick.SmartSplit(false, true, new[] {','}).Select(Convert.ToDouble);
            var slickList = slick.SmartSplit(false, true, new[] { ',' }).Select(Convert.ToDouble);
            var realSlick = SlickNumbers.SlickSort(notSlickList.AsEnumerable());

            Assert.AreEqual(ArrayOfDoubleToString(slickList), ArrayOfDoubleToString(realSlick));
        }
        [TestCase("1,4,8,9,12", "3,4,1,3")]
        public void Dispersion(string numbers, string dispersion) {
            var nbs = numbers.SmartSplit(false, true, new[] { ',' }).Select(Convert.ToDouble);
            var exp = dispersion.SmartSplit(false, true, new[] { ',' }).Select(Convert.ToDouble);
            var res = SlickNumbers.Dispersion(nbs);
            Assert.AreEqual(ArrayOfDoubleToString(exp), ArrayOfDoubleToString(res));
        }
        [TestCase("1,4,8,9,12", 4)]
        [TestCase("1,4,8,9,12,16", 4)]
        public void MaxDispersion(string numbers, double max) {
            var nbs = numbers.SmartSplit(false, true, new[] { ',' }).Select(Convert.ToDouble);
            Assert.AreEqual(max, SlickNumbers.MaxDispersion(nbs));
        }
        [TestCase("1,4,8,9,12", 1)]
        [TestCase("-1,0,1,4,8,9,12", 1)]
        public void MinDispersion(string numbers, double min) {
            var nbs = numbers.SmartSplit(false, true, new[] { ',' }).Select(Convert.ToDouble);
            Assert.AreEqual(min, SlickNumbers.MinDispersion(nbs));
        }

        private string ArrayOfDoubleToString(IEnumerable<double> d) {
            var t = d.Aggregate("", (current, d1) => current + (d1 + ","));

            return t.Substring(0, (t.Length != 0) ? t.Length - 1 : t.Length);
        }
    }
}

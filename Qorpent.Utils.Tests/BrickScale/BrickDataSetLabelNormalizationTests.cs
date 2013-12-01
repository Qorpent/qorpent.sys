using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale {
    /// <summary>
    ///     Фикстура, тестирующая разнесение лэйблов в графике
    /// </summary>
    [TestFixture]
    public class BrickDataSetLabelNormalizationTests {
        [Test]
        public void SimpleTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);

            ds.Add(2, 1, 30);
            ds.Add(2, 1, 55);
            ds.Add(2, 1, 41);
            ds.Calculate();
            // Первая пара слишком далеко друг от друга, так что они Auto
            Assert.AreEqual(LabelPosition.Auto, ds.GetItem(1, 1, 1).LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, ds.GetItem(2, 1, 1).LabelPosition);
            // Вторая пара уже близко, так что лэйбел меньшего значения снизу, большего — вверху
            Assert.AreEqual(LabelPosition.Below, ds.GetItem(1, 1, 2).LabelPosition);
            Assert.AreEqual(LabelPosition.Above, ds.GetItem(2, 1, 2).LabelPosition);
            // Третья пара совсем билзко друг к другу, так что у одного отключён
            Assert.IsTrue(ds.GetItem(1, 1, 3).LabelPosition.HasFlag(LabelPosition.Hidden) || ds.GetItem(2, 1, 3).LabelPosition.HasFlag(LabelPosition.Hidden));
        }
        /// <summary>
        ///     Тест выражает правильность сбора колонок в односерийном простом графике типа «линия»
        /// </summary>
        [Test]
        public void SimpleChartBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 20);
            ds.Add(1, 1, 30);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(3, colons.Length);
            Assert.AreEqual(1, colons[0].Items.Length);
            Assert.AreEqual(1, colons[1].Items.Length);
            Assert.AreEqual(1, colons[2].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(30, colons[2].Items[0].Value);
        }
        /// <summary>
        ///     Тест выражает то, что в графике с двумя сериями (по три значения в каждой) соберётся три колонки с двумя <see cref="DataItem"/>
        ///     внутри и правильном порядке
        /// </summary>
        [Test]
        public void MultiLineChartBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 20);
            ds.Add(1, 1, 30);
            ds.Add(2, 1, 100);
            ds.Add(2, 1, 200);
            ds.Add(2, 1, 300);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(3, colons.Length);
            Assert.AreEqual(2, colons[0].Items.Length);
            Assert.AreEqual(2, colons[1].Items.Length);
            Assert.AreEqual(2, colons[2].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(30, colons[2].Items[0].Value);
            Assert.AreEqual(100, colons[0].Items[1].Value);
            Assert.AreEqual(200, colons[1].Items[1].Value);
            Assert.AreEqual(300, colons[2].Items[1].Value);
        }
        /// <summary>
        ///     Тест выражает то, что в мультисерийном графике с 3 рядами соберутся 3 колонки с четырьмя значениями в каждой:
        ///     1с1р,1с2р,2с1р,2с2р (с — серия, р — ряд)
        /// </summary>
        [Test]
        public void MiltilineChartWithRowsBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 2, 15);
            ds.Add(1, 1, 20);
            ds.Add(1, 2, 25);
            ds.Add(1, 1, 30);
            ds.Add(1, 2, 35);
            ds.Add(2, 1, 100);
            ds.Add(2, 2, 150);
            ds.Add(2, 1, 200);
            ds.Add(2, 2, 250);
            ds.Add(2, 1, 300);
            ds.Add(2, 2, 350);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(3, colons.Length);
            Assert.AreEqual(4, colons[0].Items.Length);
            Assert.AreEqual(4, colons[1].Items.Length);
            Assert.AreEqual(4, colons[2].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(30, colons[2].Items[0].Value);
            Assert.AreEqual(15, colons[0].Items[1].Value);
            Assert.AreEqual(25, colons[1].Items[1].Value);
            Assert.AreEqual(35, colons[2].Items[1].Value);
            Assert.AreEqual(100, colons[0].Items[2].Value);
            Assert.AreEqual(200, colons[1].Items[2].Value);
            Assert.AreEqual(300, colons[2].Items[2].Value);
            Assert.AreEqual(150, colons[0].Items[3].Value);
            Assert.AreEqual(250, colons[1].Items[3].Value);
            Assert.AreEqual(350, colons[2].Items[3].Value);
        }
        private BrickDataSet GetEmptyDataSet(SeriaCalcMode calcMode, int height) {
            return new BrickDataSet {
                Preferences = new UserPreferences {
                    SeriaCalcMode = calcMode,
                    Height = height
                }
            };
        }
    }
}
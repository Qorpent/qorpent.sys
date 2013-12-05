using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale {
    /// <summary>
    ///     Фикстура, тестирующая разнесение лэйблов в графике
    /// </summary>
    [TestFixture]
    public class BrickDataSetLabelNormalizationTests : BrickDataSetTestBase {
        [Test]
        public void CanHideLabelWhenTwoLabelsDiffersAboutOnePixel() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(2, 1, 10);
            ds.Calculate();
            Assert.AreEqual(10, ds.Rows[0].Items[0].Value);
            Assert.AreEqual(10, ds.Rows[1].Items[0].Value);
            Assert.IsTrue(
                ((ds.Rows[0].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden) && !ds.Rows[1].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden)))
                    ||
                ((ds.Rows[1].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden) && !ds.Rows[0].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden)))
            );
        }
        [Test]
        public void SimpleLabelNormalizationTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);
            ds.Add(2, 1, 50);
            ds.Add(2, 1, 55);
            ds.Add(2, 1, 41);
            ds.Calculate();
            Console.WriteLine(ds.Rows[0].Items[0].LabelPosition + ", " + ds.Rows[1].Items[0].LabelPosition);
            Console.WriteLine(ds.Rows[0].Items[1].LabelPosition + ", " + ds.Rows[1].Items[1].LabelPosition);
            Console.WriteLine(ds.Rows[0].Items[2].LabelPosition + ", " + ds.Rows[1].Items[2].LabelPosition);
            Assert.AreEqual(10, ds.Rows[0].Items[0].Value);
            Assert.AreEqual(50, ds.Rows[0].Items[1].Value);
            Assert.AreEqual(40, ds.Rows[0].Items[2].Value);
            Assert.AreEqual(50, ds.Rows[1].Items[0].Value);
            Assert.AreEqual(55, ds.Rows[1].Items[1].Value);
            Assert.AreEqual(41, ds.Rows[1].Items[2].Value);
            // Первая пара слишком далеко друг от друга, так что они Auto
            Assert.AreEqual(LabelPosition.Auto, ds.Rows[0].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, ds.Rows[1].Items[0].LabelPosition);
            // Вторая пара уже близко, так что лэйбел меньшего значения снизу, большего — вверху
            Assert.AreEqual(LabelPosition.Below, ds.Rows[0].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, ds.Rows[1].Items[1].LabelPosition);
            // Вначале я думал, что алгоритм должен отключить один из лэйблов, но он смог развести, так что правлю тест
            Assert.AreEqual(LabelPosition.Below, ds.Rows[0].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, ds.Rows[1].Items[2].LabelPosition);
        }
        /// <summary>
        ///     Это реально лютый тест. По моим представлениям, отработал даже лучше, чем я предполагал.
        /// </summary>
        [Test]
        public void HardLabelNormalizationTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 2, 15);
            ds.Add(1, 1, 20);
            ds.Add(1, 2, 25);
            ds.Add(1, 1, 30);
            ds.Add(1, 2, 35);
            ds.Add(2, 1, 20);
            ds.Add(2, 2, 150);
            ds.Add(2, 1, 70);
            ds.Add(2, 2, 250);
            ds.Add(2, 1, 300);
            ds.Add(2, 2, 350);
            ds.Calculate();
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(LabelPosition.Auto, colons[0].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Below, colons[0].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[0].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[0].Items[3].LabelPosition);
            Assert.AreEqual(LabelPosition.Below, colons[1].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[1].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[1].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[1].Items[3].LabelPosition);
            Assert.AreEqual(LabelPosition.Below, colons[2].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[2].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[2].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[2].Items[3].LabelPosition);
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
        ///     Тест выражает то, что в мультисерийном графике с 2 рядами соберутся 3 колонки с четырьмя значениями в каждой:
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
        /// <summary>
        ///     Тест выражает возможность собрать колонки по графику с двумя Y-ками
        /// </summary>
        [Test]
        public void SingleLineTwoScaleBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10, false);
            ds.Add(1, 1, 20, false);
            ds.Add(1, 1, 15, true);
            ds.Add(1, 1, 25, true);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(2, colons[0].Items.Length);
            Assert.AreEqual(2, colons[1].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(15, colons[0].Items[1].Value);
            Assert.AreEqual(25, colons[1].Items[1].Value);
        }
        /// <summary>
        ///     Тест выражает то, что при работе с двумя осями полностью поддежривается логика работы с Row
        /// </summary>
        [Test]
        public void SingleLineTwoScaleWithRowsBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10, false);
            ds.Add(1, 2, 11, false);
            ds.Add(1, 1, 20, false);
            ds.Add(1, 2, 21, false);
            ds.Add(1, 1, 15, true);
            ds.Add(1, 2, 16, true);
            ds.Add(1, 1, 25, true);
            ds.Add(1, 2, 26, true);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(4, colons[0].Items.Length);
            Assert.AreEqual(4, colons[1].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(11, colons[0].Items[1].Value);
            Assert.AreEqual(21, colons[1].Items[1].Value);
            Assert.AreEqual(15, colons[0].Items[2].Value);
            Assert.AreEqual(25, colons[1].Items[2].Value);
            Assert.AreEqual(16, colons[0].Items[3].Value);
            Assert.AreEqual(26, colons[1].Items[3].Value);
        }
        /// <summary>
        ///     Тест показывает то, что поддерживается две оси с множеством серий и рядов
        /// </summary>
        [Test]
        public void MultiLineTwoScaleWithRowsBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10, false);
            ds.Add(1, 2, 11, false);
            ds.Add(1, 1, 20, false);
            ds.Add(1, 2, 21, false);
            ds.Add(2, 1, 12, false);
            ds.Add(2, 2, 13, false);
            ds.Add(2, 1, 22, false);
            ds.Add(2, 2, 23, false);
            ds.Add(1, 1, 15, true);
            ds.Add(1, 2, 16, true);
            ds.Add(1, 1, 25, true);
            ds.Add(1, 2, 26, true);
            ds.Add(2, 1, 17, true);
            ds.Add(2, 2, 18, true); 
            ds.Add(2, 1, 27, true);
            ds.Add(2, 2, 28, true);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(8, colons[0].Items.Length);
            Assert.AreEqual(8, colons[1].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(11, colons[0].Items[1].Value);
            Assert.AreEqual(21, colons[1].Items[1].Value);
            Assert.AreEqual(12, colons[0].Items[2].Value);
            Assert.AreEqual(22, colons[1].Items[2].Value);
            Assert.AreEqual(13, colons[0].Items[3].Value);
            Assert.AreEqual(23, colons[1].Items[3].Value);
            Assert.AreEqual(15, colons[0].Items[4].Value);
            Assert.AreEqual(25, colons[1].Items[4].Value);
            Assert.AreEqual(16, colons[0].Items[5].Value);
            Assert.AreEqual(26, colons[1].Items[5].Value);
            Assert.AreEqual(17, colons[0].Items[6].Value);
            Assert.AreEqual(27, colons[1].Items[6].Value);
            Assert.AreEqual(18, colons[0].Items[7].Value);
            Assert.AreEqual(28, colons[1].Items[7].Value);
        }
    }
    public class BrickDataSetTestBase {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcMode"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected BrickDataSet GetEmptyDataSet(SeriaCalcMode calcMode, int height) {
            return new BrickDataSet {
                Preferences = new UserPreferences {
                    SeriaCalcMode = calcMode,
                    Height = height
                }
            };
        }
    }
}